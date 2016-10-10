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
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;
using Android.Graphics;

namespace AppGeoFit.Droid.Screens
{
   

    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_GameDetails : Screen
    {
        int gameId;
        IGameManager gameManager;
        IFeedBackManager feedBackManager;
        Game game;
        TextView startDateEt;
        TextView endDateEt;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);
            feedBackManager = Xamarin.Forms.DependencyService.Get<IFeedBackManager>().InitiateServices(false);
            AppSession appSession = new AppSession(ApplicationContext);
            Player player = appSession.getPlayer();
            gameId = Intent.GetIntExtra("gameId", 0);
            try
            {

                game = gameManager.GetGame(gameId);
            }
            catch(Exception ex)
            {
                Toast.MakeText(ApplicationContext,
                               ex.Message, ToastLength.Short).Show();
            }

            bool isCreator = false;

            TextView numPlayers;
            TextView location;
            TextView creator;
            TextView homeTeam;
            TextView awayTeam;
            ListView playerList;
            ImageButton editGameB;
            ImageButton deleteGameB;
            ImageButton leaveGameB;
            TextView showCommentsLink;
            if (game.CreatorID != player.PlayerId)
            {
                SetContentView(Resource.Layout.GameDetails_NoCreator);
                //Recuperamos elementos
                startDateEt = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_StartDate);
                endDateEt = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_EndDate);
                numPlayers = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_NumPlayers);
                location = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_Location);
                creator = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_Creator);
                homeTeam = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_HomeTeam);
                awayTeam = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_AwayTeam);
                playerList = FindViewById<ListView>(Resource.Id.GameDetails_NoCreator_PlayerList);
                showCommentsLink = FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_ShowCommentsLink);

                editGameB = null;
                deleteGameB = null;
                leaveGameB = FindViewById<ImageButton>(Resource.Id.GameDetails_NoCreator_LeaveB);
                isCreator = false;
            }
            else
            {
                SetContentView(Resource.Layout.GameDetails_Creator);
                startDateEt = FindViewById<TextView>(Resource.Id.GameDetails_Creator_StartDate);
                endDateEt = FindViewById<TextView>(Resource.Id.GameDetails_Creator_EndDate);
                numPlayers = FindViewById<TextView>(Resource.Id.GameDetails_Creator_NumPlayers);
                location = FindViewById<TextView>(Resource.Id.GameDetails_Creator_Location);
                creator = FindViewById<TextView>(Resource.Id.GameDetails_Creator_Creator);
                homeTeam = FindViewById<TextView>(Resource.Id.GameDetails_Creator_HomeTeam);
                awayTeam = FindViewById<TextView>(Resource.Id.GameDetails_Creator_AwayTeam);
                playerList = FindViewById<ListView>(Resource.Id.GameDetails_Creator_PlayerList);
                editGameB = FindViewById<ImageButton>(Resource.Id.GameDetails_Creator_EditGameB);
                deleteGameB = FindViewById<ImageButton>(Resource.Id.GameDetails_Creator_DeleteGameB);
                showCommentsLink = FindViewById<TextView>(Resource.Id.GameDetails_Creator_ShowCommentsLink);
                leaveGameB = FindViewById<ImageButton>(Resource.Id.GameDetails_Creator_LeaveB);
                isCreator = true;
            }
            startDateEt.Text = game.StartDate.Day + "/" + game.StartDate.Month + "/" + game.StartDate.Year
                + "  " + game.StartDate.Hour + ":" + game.StartDate.Minute;            
            endDateEt.Text = game.EndDate.Day + "/" + game.EndDate.Month + "/" + game.EndDate.Year
                + "  " + game.EndDate.Hour + ":" + game.EndDate.Minute;            
            numPlayers.Text = game.PlayersNum + "/" + game.Sport.NumPlayers;

            int totalGameComments = 0;
            try
            {
                totalGameComments = feedBackManager.TotalGameCommentsCount(game.GameID);
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext,
                             ex.Message, ToastLength.Short).Show();
            }

            if(totalGameComments > 0)
            {
                showCommentsLink.SetTextColor(Color.ParseColor("#4785F4"));
                showCommentsLink.Click += (o, e) =>
                {
                    var screen_Comments = new Intent(this, typeof(Screen_Comments));
                    screen_Comments.PutExtra("gameId", game.GameID);
                    StartActivity(screen_Comments);
                };
            }

            if (game.PlaceID != null)
            {
                location.SetTextColor(Color.ParseColor("#4785F4"));
                location.Click += (o, e) =>
                {
                    AlertDialog dialogPlaceDetails = ShowPlaceDetails(game.Place);
                    ImageButton aceptButton = dialogPlaceDetails.FindViewById<ImageButton>(Resource.Id.PlaceDetails_AcceptButton);
                    aceptButton.Click += (oB, eB) =>
                    {
                        dialogPlaceDetails.Cancel();
                    };                   
                };
            }
            creator.Text = game.Creator.PlayerNick;
            homeTeam.Text = game.Team1ID != null ? game.Team.TeamName : "";
            awayTeam.Text = game.Team2ID != null ? game.Team1.TeamName : "";
            List<Player> participatePlayersList = new List<Player>();
            try {
                participatePlayersList = gameManager.GetParticipatePlayers(gameId);
            }
            catch (PlayerNotFoundException ex){}
            catch (Exception ex)
            {
                Toast.MakeText(this.ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
            }
            PlayerArrayAdapter adapter = new PlayerArrayAdapter(
                       this, participatePlayersList,
                       0, 0, false, null, false);
            playerList.Adapter = adapter;
            playerList.ItemClick += (o, e) =>
            {
                Player playerClick = adapter.GetItem(e.Position);
                AlertDialog dialogProfile = ShowPlayerDetails(playerClick);
                TextView commentsLink = dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_ShowCommentsLink);
                int totalPlayerComments = 0;
                try
                {
                    totalPlayerComments = feedBackManager.TotalPlayerCommentsCount((int)playerClick.PlayerId);
                }
                catch(Exception ex)
                {
                    Toast.MakeText(ApplicationContext,
                             ex.Message, ToastLength.Short).Show();
                }
                if (totalPlayerComments > 0)
                {
                    commentsLink.SetTextColor(Color.ParseColor("#4785F4"));
                }
                else
                {
                    commentsLink.Click += (ocl, ecl) => { };
                }
            };
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            if (isCreator)
            {
                editGameB.Click += (o, e) =>
                {
                    var screen_UpateGame = new Intent(this, typeof(Screen_UpdateGame));
                    screen_UpateGame.PutExtra("gameId", game.GameID);                    
                    StartActivity(screen_UpateGame);
                };
                deleteGameB.Click += (o, e) =>
                {                    
                    baDelete = BotonAlert("Alert", "Do you want to cancel this game?", "OK", "Cancel", this);
                    baDelete.Show();
                    baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                    baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                    baDeletePositiveButton.Click += (oDb, eDb) =>
                    //Comprobacion , esta seguro ?
                    baDeletePositiveButton.Click += (oPB, ePB) =>
                    {
                        bool isDelete = false;
                        try {
                            isDelete = gameManager.DeleteGame(game.GameID);
                        }catch (Exception ex)
                        {
                            Toast.MakeText(ApplicationContext,
                                ex.Message, ToastLength.Short).Show();
                        }
                        if (isDelete)
                        {
                            Toast.MakeText(ApplicationContext,
                                    "You cancel the game correctly", ToastLength.Short).Show();
                            //Creamos intent y le asignamos el fragment 
                            //que debe abrir y después finalizamos la actual activity
                            //con el flag cleartop.
                            var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                            mainActivity.PutExtra("toOpen", "TabGame");
                            mainActivity.SetFlags(ActivityFlags.ClearTop);
                            StartActivity(mainActivity);
                        }
                    };
                    baDeleteNegativeButton.Click += (oNB, eNB) =>
                    {
                        baDelete.Cancel();
                    };
                };
            }
            
            leaveGameB.Click += (o, e) =>
            {
                baDelete = BotonAlert("Alert", "Do you want to leave this game?", "OK", "Cancel", this);
                baDelete.Show();
                baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                //Comprobacion , esta seguro ?
                bool response;
                baDeletePositiveButton.Click += (oPB, ePB) =>
                {
                    response = false;
                    try {
                        response = gameManager.RemovePlayer(game.GameID, player.PlayerId);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(ApplicationContext,
                                  ex.Message, ToastLength.Short).Show();
                    }
                    Toast.MakeText(ApplicationContext,
                            "You get out from this game correctly", ToastLength.Short).Show();
                    //Creamos intent y le asignamos el fragment 
                    //que debe abrir y después finalizamos la actual activity
                    //con el flag cleartop.
                    var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                    mainActivity.PutExtra("toOpen", "TabGame");
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(mainActivity);
                };
                baDeleteNegativeButton.Click += (oNB, eNB) =>
                {
                    baDelete.Cancel();
                };
            };
        }

        protected override void OnResume()
        {
            base.OnStart();
            try
            {
                game = gameManager.GetGame(gameId);
                startDateEt.Text = game.StartDate.Day + "/" + game.StartDate.Month + "/" + game.StartDate.Year
              + "  " + game.StartDate.Hour + ":" + game.StartDate.Minute;
                endDateEt.Text = game.EndDate.Day + "/" + game.EndDate.Month + "/" + game.EndDate.Year
                    + "  " + game.EndDate.Hour + ":" + game.EndDate.Minute;
            }
            catch(Exception ex)
            {
                Toast.MakeText(ApplicationContext,
                               ex.Message, ToastLength.Short).Show();
            }
          
        }
    }
}