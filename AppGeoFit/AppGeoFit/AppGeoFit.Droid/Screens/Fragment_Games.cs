using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Android.Content;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using System.Collections.Generic;
using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Linq;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using Android.Support.V7.App;
using Android.Locations;
using Android.Runtime;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace AppGeoFit.Droid.Screens
{
    public class Fragment_Games : Fragment
    {
        int actualSportId;
        IGameManager gameManager;
        IPlayerManager playerManager;
        ListView gameListView;
        Player actualPlayer;
        GameArrayAdapter adapterLGames;
        bool captain;
        Team team = new Team();
        List<Game> listGames = new List<Game>();
        int rows = 11;
        int page = 0;
        View view;
        LayoutInflater layoutInflater;
        string selectedType = "time";
        static readonly string TAG = "X:" + typeof(Fragment_Games).Name;
        Position position = new Position();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);
            layoutInflater = inflater;
            view = inflater.Inflate(Resource.Layout.Games, container, false);

            FragmentActivity_MainActivity myActivity = (FragmentActivity_MainActivity)Activity;
            playerManager = myActivity.playerManager;
            gameManager = myActivity.gameManager;

            ImageButton createGameButton = view.FindViewById<ImageButton>(Resource.Id.Games_createGame);
            ImageButton currentGames = view.FindViewById<ImageButton>(Resource.Id.Games_currentGames);
            ImageButton findGames = view.FindViewById<ImageButton>(Resource.Id.Games_findGames);
            //Recuperamos el id del deporte actual.
            Spinner spinnerSport = this.Activity.FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);
            actualSportId = spinnerSport.SelectedItem.GetHashCode();
            
            createGameButton.Click += (o, e) =>
            {
                var screen_CreateGame_Captain = new Intent(Context, typeof(Screen_CreateGame));
                screen_CreateGame_Captain.PutExtra("sportId", actualSportId);
                Activity.StartActivity(screen_CreateGame_Captain);
            };
            currentGames.Click += (o, e) =>
            {
                var Screen_ActualGames = new Intent(Context, typeof(Screen_ActualGames));
                Screen_ActualGames.PutExtra("sportId", actualSportId);
                Activity.StartActivity(Screen_ActualGames);
            };
            findGames.Click += (o,e) =>
            {
                AlertDialog dialogSearchOptions;
                dialogSearchOptions = CreateAlertDialog(Resource.Layout.dialog_SearchOptions, Context);
                dialogSearchOptions.Show();
                CheckBox timeCB = dialogSearchOptions.FindViewById<CheckBox>(Resource.Id.dialog_SearchOptions_TimeCB);
                CheckBox distanceCB = dialogSearchOptions.FindViewById<CheckBox>(Resource.Id.dialog_SearchOptions_DistanceCB);
                CheckBox numPlayersCB = dialogSearchOptions.FindViewById<CheckBox>(Resource.Id.dialog_SearchOptions_NumOfPlayersCB);

                switch (selectedType)
                {
                    case "time":
                        timeCB.Checked = true;
                        distanceCB.Checked = false;
                        numPlayersCB.Checked = false;
                        break;
                    case "distance":
                        timeCB.Checked = false;
                        distanceCB.Checked = true;
                        numPlayersCB.Checked = false;
                        break;
                    case "numPlayers":
                        timeCB.Checked = false;
                        distanceCB.Checked = false;
                        numPlayersCB.Checked = true;
                        break;
                    default:
                        break;
                }

                timeCB.Click += (ot, et) =>
                {

                    timeCB.Checked = true;
                    distanceCB.Checked = false;
                    numPlayersCB.Checked = false;
                    selectedType = "time";
                    updateGameList(selectedType);
                };
                distanceCB.Click += (od,ed) =>//async delegate
                {
                    position.Latitude = 42.1354955;
                    position.Longitude = -8.812848199999962;
                    timeCB.Checked = false;
                    distanceCB.Checked = true;
                    numPlayersCB.Checked = false;
                    selectedType = "distance";

                    LocationManager locationManager = (LocationManager)Context
                   .GetSystemService(Context.LocationService);
                    bool isGPSEnabled = locationManager
                    .IsProviderEnabled(LocationManager.GpsProvider);
                    bool isNetworkEnabled = locationManager
                    .IsProviderEnabled(LocationManager.NetworkProvider);
                   
                    if (!isGPSEnabled && !isNetworkEnabled)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            "Enable GPS to use this option", ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            //var geolocator = CrossGeolocator.Current;
                            //geolocator.DesiredAccuracy = 500;
                            //position = await geolocator.GetPositionAsync(timeoutMilliseconds: 10000);
                            updateGameList(selectedType);

                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                               "Unable to get location, please try again", ToastLength.Short).Show();
                        }
                    }

                };
                numPlayersCB.Click += (onp, enp) =>
                {
                    timeCB.Checked = false;
                    distanceCB.Checked = false;
                    numPlayersCB.Checked = true;
                    selectedType = "numPlayers";
                    updateGameList(selectedType);

                };

            };

            AppSession appSession = new AppSession(Activity.ApplicationContext);
            actualPlayer = appSession.getPlayer();
            try
            {
                team = playerManager.FindTeamCaptainOnSport(actualPlayer.PlayerId,
                       actualSportId);
                captain = true;
            }
            catch (CaptainNotFoundException ex)
            {
                captain = false;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity.ApplicationContext,
                         ex.Message, ToastLength.Short).Show();
            }
            #region listGames    
            gameListView = view.FindViewById<ListView>(Resource.Id.Games_GamesList);
            updateGameList(selectedType);
            spinnerSport.ItemSelected += (oT, eT) =>
            {
                page = 0;
                actualSportId = spinnerSport.GetItemAtPosition(eT.Position).GetHashCode();
                appSession.setSelectedSport(actualSportId);
                updateGameList(selectedType);
                try
                {
                    team = playerManager.FindTeamCaptainOnSport(actualPlayer.PlayerId,
                           actualSportId);
                    captain = true;
                }
                catch (CaptainNotFoundException ex)
                {
                    captain = false;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity.ApplicationContext,
                             ex.Message, ToastLength.Short).Show();
                }
            };
            gameListView.Scroll += (o, e) =>
            {
                if (!(gameListView.Adapter == null || gameListView.Adapter.Count == 0)
                    && gameListView.LastVisiblePosition >= gameListView.Count - 1)
                {
                    try {
                        int totalGamesCount = gameManager.TotalGamesCount(actualSportId);
                        if (totalGamesCount > gameListView.Count)
                        {
                            page = (int)Math.Ceiling((double)totalGamesCount / gameListView.LastVisiblePosition) - 1;
                            updateGameList(selectedType);
                        }
                    }catch(GameNotFoundException ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext, ex.Message, ToastLength.Short).Show();
                    }
                    catch(Exception ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext, ex.Message, ToastLength.Short).Show();
                    }                    
                }
            };

            gameListView.ItemClick += (o, e) =>
            {
                int gameSelectedOnClickId = gameListView.GetItemAtPosition(e.Position).GetHashCode();
                var screen_GameDetails = new Intent(Context, typeof(Screen_GameDetails));
                screen_GameDetails.PutExtra("gameId", gameSelectedOnClickId);
                Activity.StartActivity(screen_GameDetails);
            };

            #endregion
            return view;
        }

        public override void OnResume()
        {
            base.OnStart();
            updateGameList(selectedType);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        private void updateGameList(string selectedTypeSearch)
        {
            try
            {               
                listGames = gameManager.GetAllPagination(page, rows, actualSportId, selectedTypeSearch, position.Longitude, position.Latitude);
                adapterLGames = new GameArrayAdapter(view.Context, listGames);
                gameListView.Adapter = adapterLGames;
                RegisterForContextMenu(gameListView);          
            }
            catch (GameNotFoundException)
            {
                gameListView.Adapter = null;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity.ApplicationContext,
                    ex.Message, ToastLength.Short).Show();
            }
        }

        public AlertDialog CreateAlertDialog(int layout, Context context)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            View dialogView;
            AlertDialog dialog;
            dialogView = layoutInflater.Inflate(layout, null);
            builder.SetView(dialogView);
            dialog = builder.Create();
            return dialog;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View vValue, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, vValue, menuInfo);
            MenuInflater inflater = new MenuInflater(Context);

            menu.SetHeaderTitle("Join?");
            inflater.Inflate(Resource.Menu.MenuCreateTeam, menu);
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {           
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            Game gameSelected = adapterLGames.GetItem(info.Position);
            bool error = false;
            switch (item.ItemId)
            {
                case Resource.Id.MenuCTCaptain:
                    if (!captain)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            "You aren't a captain for this sport for any team", ToastLength.Short).Show();
                    }
                    else
                    {
                        error = false;
                        if (gameSelected.Team2ID != null)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                            "This team has already a visitor team", ToastLength.Short).Show();
                            error = true;
                        }
                        try
                        {
                            gameManager.IsPlayerOnGame(gameSelected.GameID, actualPlayer.PlayerId);
                        }
                        catch (PlayerOnGameException ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                              ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                           ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        try
                        {
                            gameManager.PlayerGameOnTime(actualPlayer.PlayerId, gameSelected);
                        }
                        catch (GameOnTimeException ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                              ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        catch (GameNotFoundException ex) { }
                        catch (Exception ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                           ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        //Comprobar equipo no tenga partido en ese horario.
                        try
                        {
                            gameManager.TeamGameOnTime(team.TeamID, gameSelected);
                        }
                        catch (GameOnTimeException ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        catch (GameNotFoundException ex) { }
                        catch (Exception ex)
                        {
                            Toast.MakeText(Activity.ApplicationContext,
                           ex.Message, ToastLength.Short).Show();
                            error = true;
                        }
                        if (!error)
                        {
                            var screen_AddTeamToGame = new Intent(Context, typeof(Screen_AddTeamToGame));
                            screen_AddTeamToGame.PutExtra("sportId", actualSportId);
                            screen_AddTeamToGame.PutExtra("gameId", adapterLGames.GetItem(info.Position).GameID);
                            Activity.StartActivity(screen_AddTeamToGame);
                        }

                    }
                    return true;
                case Resource.Id.MenuCTIndividual:
                    try
                    {
                        gameManager.AddPlayer(gameSelected.GameID, actualPlayer.PlayerId);
                        Toast.MakeText(Activity.ApplicationContext,
                         "You have been added correctly to this game", ToastLength.Short).Show();
                        updateGameList(selectedType);
                    }
                    catch (PlayerOnGameException ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                    }
                    catch (MaxPlayerOnGameException ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                    }
                    catch (GameOnTimeException ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Activity.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                    }


                    return true;
                default:
                    return base.OnContextItemSelected(item);

            }
        }                
    }
}

       

 