using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.DataAccesLayer.Models;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using Android.Views;
using Android.Graphics;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_FeedBack : Screen
    {
        int actualGameId;
        int actualSportId;
        int noticeId;
        IFeedBackManager feedBackManager;
        IGameManager gameManager;
        IPlayerManager playerManager;
        ITeamManager teamManager;
        PlayerArrayAdapter adapterLPlayersPending;
        List<Player> lPlayersOnGame = new List<Player>();
        List<FeedBack> pendingFeedBacks = new List<FeedBack>();
        List<Player> lPlayersPendingToFeedBack = new List<Player>();
        ListView playerList;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FeedBack);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            actualGameId = Intent.GetIntExtra("gameId", 0);
            actualSportId = Intent.GetIntExtra("sportId", 0);
            noticeId = Intent.GetIntExtra("noticeId", 0);

            AppSession appSession = new AppSession(this.ApplicationContext);
            Player actualPlayer = appSession.getPlayer();

            feedBackManager = Xamarin.Forms.DependencyService.Get<IFeedBackManager>().InitiateServices(false);
            gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);
            playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);

            teamManager = Xamarin.Forms.DependencyService.Get<ITeamManager>().InitiateServices(false);

            Game actualGame = gameManager.GetGame(actualGameId);

            EditText commentGame = FindViewById<EditText>(Resource.Id.FeedBack_GameComent);
            EditText commentPlace = FindViewById<EditText>(Resource.Id.FeedBack_PlaceComent);
            RatingBar ratingPlace = FindViewById<RatingBar>(Resource.Id.FeedBack_RatingBarPlace);
            playerList = FindViewById<ListView>(Resource.Id.FeedBack_PlayerList);
            ImageButton acceptButton = FindViewById<ImageButton>(Resource.Id.FeedBack_AcceptButton);
            ImageButton cancelButton = FindViewById<ImageButton>(Resource.Id.FeedBack_CancelButton);

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            lPlayersOnGame = gameManager.GetParticipatePlayers(actualGameId);
            lPlayersOnGame.Remove(actualPlayer);
            lPlayersPendingToFeedBack.AddRange(lPlayersOnGame);
            UpdatePlayers();

            TextView game = FindViewById<TextView>(Resource.Id.FeedBack_Game);
            TextView place = FindViewById<TextView>(Resource.Id.FeedBack_Place);
            game.SetTextColor(Color.ParseColor("#4785F4"));
            if (actualGame.PlaceID != null)
            {
                place.SetTextColor(Color.ParseColor("#4785F4"));
            }
                game.Click += (o, e) =>
            {
                AlertDialog dialogProfile = CreateAlertDialog(Resource.Layout.GameDetails_NoCreator, this);
                dialogProfile.Show();
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_AwayTeam).Text = actualGame.Team2ID != null ? actualGame.Team1.TeamName : "";
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_Creator).Text = actualGame.Creator.PlayerNick;
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_EndDate).Text = actualGame.EndDate.Day + "/" + actualGame.EndDate.Month + "/" + actualGame.EndDate.Year
                + "  " + actualGame.EndDate.Hour + ":" + actualGame.EndDate.Minute;
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_HomeTeam).Text = actualGame.Team1ID != null ? actualGame.Team.TeamName : "";
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_Location).Text = actualGame.Place.Direction;
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_NumPlayers).Text = actualGame.PlayersNum + "/" + actualGame.Sport.NumPlayers;
                dialogProfile.FindViewById<TextView>(Resource.Id.GameDetails_NoCreator_StartDate).Text = actualGame.StartDate.Day + "/" + actualGame.StartDate.Month + "/" + actualGame.StartDate.Year
                + "  " + actualGame.StartDate.Hour + ":" + actualGame.StartDate.Minute;
                dialogProfile.FindViewById<ImageButton>(Resource.Id.GameDetails_NoCreator_LeaveB).Visibility = ViewStates.Invisible;
            };
            place.Click += (o, e) =>
            {
                if (actualGame.PlaceID != null)
                {
                    AlertDialog dialogPlaceDetails = ShowPlaceDetails(actualGame.Place);

                    if (feedBackManager.TotalPlaceCommentsCount((int)actualGame.PlaceID) > 0)
                    {
                        dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_ShowCommentsLink).SetTextColor(Color.ParseColor("#4785F4"));
                        ImageButton aceptButton = dialogPlaceDetails.FindViewById<ImageButton>(Resource.Id.PlaceDetails_AcceptButton);

                        aceptButton.Click += (oB, eB) =>
                        {
                            dialogPlaceDetails.Cancel();
                        };
                    }
                }
            };

            bool commentN = false;

            #region playerList
            playerList.ItemClick += (o, e) =>
            {
                AlertDialog dialogRatePlayer;
                dialogRatePlayer = CreateAlertDialog(Resource.Layout.dialog_RatePlayer, this);
                dialogRatePlayer.Show();
                TextView playerLink = dialogRatePlayer.FindViewById<TextView>(Resource.Id.dialog_RatePlayer_Player);
                EditText commentPlayer = dialogRatePlayer.FindViewById<EditText>(Resource.Id.dialog_RatePlayer_commentPlayer);
                RatingBar ratingLvlPlayer = dialogRatePlayer.FindViewById<RatingBar>(Resource.Id.dialog_RatePlayer_ratingPlayer);
                CheckBox ratingOnTimePlayer = dialogRatePlayer.FindViewById<CheckBox>(Resource.Id.dialog_RatePlayer_OnTime);
                ImageButton cancelButtonPR = dialogRatePlayer.FindViewById<ImageButton>(Resource.Id.dialog_RatePlayer_CancelButton);
                ImageButton acceptButtonPR = dialogRatePlayer.FindViewById<ImageButton>(Resource.Id.dialog_RatePlayer_AcceptButton);
                Player player = adapterLPlayersPending.GetItem(e.Position);
                playerLink.Click += (op, ep) =>
                {
                    AlertDialog dialogProfile = ShowPlayerDetails(player);
                    TextView commentsLink = dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_ShowCommentsLink);
                    if (feedBackManager.TotalPlayerCommentsCount((int)player.PlayerId) > 0)
                    {
                        commentsLink.SetTextColor(Color.ParseColor("#4785F4"));
                    }
                    else
                    {
                        commentsLink.Click += (ocl, ecl) => { };
                    }
                };
                acceptButtonPR.Click += (oa, ea) =>
                {
                    commentN = IsRequired(commentPlayer, "Insert a comment please", errorD);
                    if (!commentN)
                    {
                        FeedBack feedBackPlayer = new FeedBack();
                        feedBackPlayer.Description = commentPlayer.Text;
                        feedBackPlayer.FeedBackDate = DateTime.Now;
                        feedBackPlayer.OnTime = ratingOnTimePlayer.Checked;
                        feedBackPlayer.PlayerID = player.PlayerId;
                        feedBackPlayer.CreatorID = actualPlayer.PlayerId;
                        feedBackPlayer.Valuation = ratingLvlPlayer.Rating;
                        pendingFeedBacks.Add(feedBackPlayer);
                        lPlayersPendingToFeedBack.Remove(player);
                        UpdatePlayers();
                        dialogRatePlayer.Cancel();
                    }
                };
                cancelButtonPR.Click += (oc, ec) =>
                {
                    dialogRatePlayer.Cancel();
                };
            };
            #endregion

            bool commentGameN = false;
            bool commentPlaceN = false;
            acceptButton.Click += (o, e) =>
            {
                commentGameN = IsRequired(commentGame, "Insert a comment please", errorD);
                commentPlaceN = IsRequired(commentPlace, "Insert a comment please", errorD);
                if (lPlayersPendingToFeedBack.Count > 0)
                    Toast.MakeText(ApplicationContext,
                             "You have players to rate", ToastLength.Long).Show();
                else
                {
                    if (!commentGameN && !commentPlaceN)
                    {
                        FeedBack gameFeedBack = new FeedBack();
                        gameFeedBack.CreatorID = actualPlayer.PlayerId;
                        gameFeedBack.Description = commentGame.Text;
                        gameFeedBack.FeedBackDate = DateTime.Now;
                        gameFeedBack.GameID = actualGameId;
                        FeedBack placeFeedBack = new FeedBack();
                        placeFeedBack.CreatorID = actualPlayer.PlayerId;
                        placeFeedBack.Description = commentPlace.Text;
                        placeFeedBack.FeedBackDate = DateTime.Now;
                        placeFeedBack.PlaceID = actualGame.PlaceID;
                        placeFeedBack.Valuation = ratingPlace.Rating;
                        FeedBack teamLocalFeedBack = new FeedBack();
                        FeedBack teamAwayFeedBack = new FeedBack();
                        double lvlTotalLocalTeam = 0;
                        double lvlTotalAwayTeam = 0;
                        foreach (FeedBack f in pendingFeedBacks)
                        {
                            Player playerF = lPlayersOnGame.ElementAt(lPlayersOnGame.FindIndex(p => p.PlayerId == f.PlayerID));
                            if (actualGame.Team1ID != null && teamManager.IsOnTeam((int)actualGame.Team1ID, playerF.PlayerId))
                            {
                                lvlTotalLocalTeam = lvlTotalLocalTeam + (double)f.Valuation;
                            }
                            else
                               if (actualGame.Team2ID != null && teamManager.IsOnTeam((int)actualGame.Team2ID, playerF.PlayerId))
                                lvlTotalAwayTeam = lvlTotalAwayTeam + (double)f.Valuation;
                        }
                        if (actualGame.Team1ID != null)
                        {
                            teamLocalFeedBack.CreatorID = actualPlayer.PlayerId;
                            teamLocalFeedBack.FeedBackDate = DateTime.Now;
                            teamLocalFeedBack.TeamID = actualGame.Team1ID;
                            teamLocalFeedBack.Valuation = lvlTotalLocalTeam;
                            pendingFeedBacks.Add(teamLocalFeedBack);
                        }
                        if (actualGame.Team2ID != null)
                        {
                            teamAwayFeedBack.CreatorID = actualPlayer.PlayerId;
                            teamAwayFeedBack.TeamID = actualGame.Team2ID;
                            teamAwayFeedBack.Valuation = lvlTotalAwayTeam;
                            pendingFeedBacks.Add(teamAwayFeedBack);
                        }
                        pendingFeedBacks.Add(gameFeedBack);
                        pendingFeedBacks.Add(placeFeedBack);
                        try
                        {

                            feedBackManager.CreateFeedBacks(pendingFeedBacks, noticeId);
                            Toast.MakeText(ApplicationContext,
                            "Your comment has been creted correctly", ToastLength.Short).Show();
                            Finish();
                        }
                        catch (NoticeNotFoundException ex)
                        {
                            Toast.MakeText(ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                        }
                        catch(Exception ex)
                        {
                            Toast.MakeText(ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                        }                        
                    }
                }
            };
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            cancelButton.Click += (o, e) =>
            {
                baDelete = BotonAlert("Alert", "Do you want to cancel all your feedback?", "OK", "Cancel", this);
                baDelete.Show();
                baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                baDeletePositiveButton.Click += (oPB, ePB) =>
                {                    
                    baDelete.Cancel();
                    Finish();
                };
                baDeleteNegativeButton.Click += (oNB, eNB) =>
                {
                    baDelete.Cancel();
                };
            };
        }
        private void UpdatePlayers()
        {
            adapterLPlayersPending = new PlayerArrayAdapter(
            this, lPlayersPendingToFeedBack,
            0, actualSportId, false, null, false);
            playerList.Adapter = adapterLPlayersPending;
            RegisterForContextMenu(playerList);
        }
    }
}