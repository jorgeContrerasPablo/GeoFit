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
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;

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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);

            View view = inflater.Inflate(Resource.Layout.Games, container, false);

            FragmentActivity_MainActivity myActivity = (FragmentActivity_MainActivity)Activity;
            playerManager = myActivity.playerManager;
            gameManager = myActivity.gameManager;            

            ImageButton createGameButton = view.FindViewById<ImageButton>(Resource.Id.Games_createGame);
            ImageButton currentGames = view.FindViewById<ImageButton>(Resource.Id.Games_currentGames);
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
            //ListGames
            List<Game> listGames = new List<Game>();
            gameListView = view.FindViewById<ListView>(Resource.Id.Games_GamesList);
            int rows = 11;
            int page = 0;
            listGames = gameManager.GetAllPagination(page, rows, actualSportId);
            adapterLGames = new GameArrayAdapter(view.Context, listGames);
            gameListView.Adapter = adapterLGames;
            RegisterForContextMenu(gameListView);
            spinnerSport.ItemSelected += (oT, eT) =>
            {
                page = 0;
                actualSportId = spinnerSport.GetItemAtPosition(eT.Position).GetHashCode();
                listGames = gameManager.GetAllPagination(page, rows, actualSportId);
                adapterLGames = new GameArrayAdapter(view.Context, listGames);
                gameListView.Adapter = adapterLGames;
                RegisterForContextMenu(gameListView);
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
           // listGames = gameManager.GetAllPagination(page, rows, actualSportId);
           // RelativeLayout footerView = (RelativeLayout)inflater.Inflate(Resource.Layout.LoadingPBFooterGameList, null, false);
           // bool isLoading = false;
            gameListView.Scroll += (o, e) =>
            {
                if(!(gameListView.Adapter == null || gameListView.Adapter.Count == 0) 
                    && gameListView.LastVisiblePosition >= gameListView.Count - 1)
                {
                    int totalGamesCount = gameManager.TotalGamesCount(actualSportId);
                    if (totalGamesCount > gameListView.Count) //&& !isLoading)
                    {
                        //gameListView.AddFooterView(footerView);
                        //isLoading = true;
                        page = (int)Math.Ceiling((double)totalGamesCount / gameListView.LastVisiblePosition) -1;
                        listGames.AddRange(gameManager.GetAllPagination(page, rows, actualSportId));
                        adapterLGames = new GameArrayAdapter(view.Context, listGames);
                        gameListView.Adapter = adapterLGames;
                        RegisterForContextMenu(gameListView);                    
                        // gameListView.RemoveFooterView(footerView);
                        // isLoading = false;
                    }
                } 
            };

            gameListView.ItemClick += (o, e) =>
            {
                int gameSelectedOnClickId = gameListView.GetItemAtPosition(e.Position).GetHashCode();
                var screen_GameDetails = new Intent(Context, typeof(Screen_GameDetails));
                screen_GameDetails.PutExtra("gameId", gameSelectedOnClickId);
              //  screen_AddTeamToGame.PutExtra("gameId", adapterLGames.GetItem(info.Position).GameID);
                Activity.StartActivity(screen_GameDetails);
            };

            #endregion
            return view;
        }
        public override void OnCreateContextMenu(IContextMenu menu, View vValue, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, vValue, menuInfo);
            //var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
            MenuInflater inflater = new MenuInflater(Context);

            menu.SetHeaderTitle("Join?");
            inflater.Inflate(Resource.Menu.MenuCreateTeam, menu);
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            //TODO ARREGLAR LOS AVISOS; QUE SEAN ESO; Y NO PROHIBICIONES.
            //TODO Arreglar catch de excepciones-> o excepeciones o booleanos devueltos para comprobar.
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
                        if(gameSelected.Team2ID != null)
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
                        catch (GameNotFoundException ex){}
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
                        //TODO actualizar lista
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