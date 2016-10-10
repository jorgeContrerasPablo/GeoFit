using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_AddTeamToGame : Screen
    {
        int actualSportId;
        int actualGameId;
        Player actualPlayer;
        Game actualGame;
        List<Player> teamList = new List<Player>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.dialog_SelectPlayers_Captain);
            actualSportId = Intent.GetIntExtra("sportId", 0);
            actualGameId = Intent.GetIntExtra("gameId", 0);
            IPlayerManager playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);
            IGameManager gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);
            AppSession appSession = new AppSession(ApplicationContext);
            actualPlayer = appSession.getPlayer();
            Team actualTeam = new Team();
            try
            {
                actualTeam = playerManager.FindTeamCaptainOnSport(actualPlayer.PlayerId, actualSportId);
            }
            catch(CaptainNotFoundException){}
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext,ex.Message, ToastLength.Short).Show();
            }
            
            ListView playerListView = FindViewById<ListView>(Resource.Id.SelectPlayers_Captain_playerListView);
            Button aceptButton = FindViewById<Button>(Resource.Id.SelectPlayers_Captain_AceptButton);

            List<int> positionsCheckeds = new List<int>();
            List<Player> playerList = new List<Player>();
            List<Joined> joineds = new List<Joined>();
            if (actualTeam.Joineds != null)
                joineds = actualTeam.Joineds.ToList();
            joineds.RemoveAt(joineds.FindIndex(j => j.Player.PlayerId == actualPlayer.PlayerId));
            foreach (Joined j in joineds)
            {
                playerList.Add(j.Player);
            }
            PlayerArrayAdapter adapterLPlayers = new PlayerArrayAdapter(
                                                this, playerList,
                                                0, actualSportId, true, positionsCheckeds, false);
            playerListView.Adapter = adapterLPlayers;
            playerListView.ItemClick += (oc, ec) =>
            {
                Player playerCheked = adapterLPlayers.GetItem(ec.Position);
                CheckBox cBox = ec.View.FindViewById<CheckBox>(Resource.Id.ElementPlayerListCheckB_CheckBox);
                if (cBox.Checked)
                {
                    cBox.Checked = false;
                    teamList.Remove(playerCheked);
                }
                else
                {   //Comprobar jugador no está ya vinculado a esta partida.           
                    try
                    {
                        if (gameManager.IsPlayerOnGame(actualGameId, playerCheked.PlayerId))
                        {
                            Toast.MakeText(ApplicationContext,
                                "Player: " + playerCheked.PlayerId + " is already joining this game", ToastLength.Short).Show();
                        }
                        else
                        {
                            cBox.Checked = true;
                            teamList.Add(playerCheked);
                        }
                    }
                    catch (PlayerOnGameException)
                    {
                        Toast.MakeText(ApplicationContext,
                               "Player: " + playerCheked.PlayerId + " is already joining this game", ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                    }                  
                }
            };
            aceptButton.Click += (oc, ec) =>
            {
                //Comprobar capitan ya vinculado a esta partida.
                try
                {
                    if (!gameManager.IsPlayerOnGame(actualGameId, actualPlayer.PlayerId))
                    {
                        teamList.Add(actualPlayer);
                    }
                }
                catch (PlayerOnGameException){}
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }

                try
                {
                    gameManager.AddTeam(actualGameId, teamList, actualTeam.TeamID);
                    Toast.MakeText(ApplicationContext,
                        "Your Team has been create correctly", ToastLength.Short).Show();
                    //Creamos intent y le asignamos el fragment 
                    //que debe abrir y después finalizamos la actual activity
                    //con el flag cleartop.
                    var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                    mainActivity.PutExtra("toOpen", "TabTeam");
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(mainActivity);
                }
                catch (MaxPlayerOnGameException ex)
                {
                    Toast.MakeText(ApplicationContext,
                        ex.Message, ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext,
                        ex.Message, ToastLength.Short).Show();
                }
                

            };
        }
    }
}