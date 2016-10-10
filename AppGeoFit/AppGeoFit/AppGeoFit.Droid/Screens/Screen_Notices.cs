using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.NoticeManager;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using Android.Views;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    class Screen_Notices : Screen
    {
        List<Notice> pendingNotice;
        public IPlayerManager playerManager { get; set; }
        public ITeamManager teamManager { get; set; }
        public INoticeManager noticeManager { get; set; }
        public IGameManager gameManager { get; set; }
        NoticeArrayAdapter adapterLNotice;
        Player actualPlayer;
        ListView noticeList;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Notices);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            AppSession appSession = new AppSession(this.ApplicationContext);
            actualPlayer = appSession.getPlayer();
            playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);
            teamManager = Xamarin.Forms.DependencyService.Get<ITeamManager>().InitiateServices(false);
            noticeManager = Xamarin.Forms.DependencyService.Get<INoticeManager>().InitiateServices(false);
            gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);
            noticeList = FindViewById<ListView>(Resource.Id.Notices_noticeList);
           

            noticeList.ItemClick += (o, e) =>
            {
                ShowNotice(noticeList.GetItemAtPosition(e.Position).GetHashCode());
            };
        }

        protected override void OnResume()
        {
            base.OnStart();           
            updateNoticeList();
        }
        //Mostramos las peticiones pendientes si es que las hay.
        void updateNoticeList()
        {
            try
            {
                pendingNotice = noticeManager.GetAllPendingNotice(actualPlayer.PlayerId).ToList();
                adapterLNotice = new NoticeArrayAdapter(
                this, pendingNotice);
                noticeList.Adapter = adapterLNotice;
                RegisterForContextMenu(noticeList);
            }
            catch (NotPendingNoticeException ex) {
                pendingNotice = new List<Notice>();
                adapterLNotice = new NoticeArrayAdapter(this, pendingNotice);
                noticeList.Adapter = adapterLNotice;
                RegisterForContextMenu(noticeList);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override void OnCreateContextMenu(IContextMenu menu, View vValue, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, vValue, menuInfo);
            MenuInflater inflater = new MenuInflater(this);
            inflater.Inflate(Resource.Menu.MenuNotice, menu);
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {            
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            Notice noticeSelected = adapterLNotice.GetItem(info.Position);
            bool error = false;
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            switch (item.ItemId)
            {
                case Resource.Id.MenuNoticeDelete:

                    baDelete = BotonAlert("Alert", "Do you want to delete this messege?", "OK", "Cancel", this);
                    baDelete.Show();
                    baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                    baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                    baDeletePositiveButton.Click += (oPB, ePB) =>
                    {
                        noticeSelected.Accepted = false;
                        try
                        {
                            noticeManager.UpdateNotice(noticeSelected);
                        }catch(Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        baDelete.Cancel();
                    };
                    baDeleteNegativeButton.Click += (oNB, eNB) =>
                    {
                        baDelete.Cancel();
                    };
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }

        void ShowNotice(int noticeId)
        {
            Notice notice = new Notice();
            try
            { notice = noticeManager.GetNotice(noticeId); }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
            AlertDialog noticeAD;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("New message");
            builder.SetPositiveButton("Acept", (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton("Deny", (EventHandler<DialogClickEventArgs>)null);

            switch (notice.Type)
            {
                case Constants.TEAM_ADD_PLAYER:
                    builder.SetMessage("Team captain: " + notice.Messenger.PlayerNick + " want's add you to her/his team.");
                    noticeAD = builder.Create();
                    noticeAD.Show();
                    noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                    {
                        int teamCaptainOnSportId = 0;
                        try
                        {
                            teamCaptainOnSportId = playerManager.FindTeamCaptainOnSport((int)notice.MessengerID, notice.SportID).TeamID;
                            teamManager.AddPlayer((int)notice.ReceiverID, teamCaptainOnSportId);
                            notice.Accepted = true;
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (CaptainNotFoundException) { }
                        catch (PlayerNotFoundException) { }
                        catch (TeamNotFoundException) { }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                        updateNoticeList();
                    };
                    noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = false;
                        noticeManager.UpdateNotice(notice);
                        noticeAD.Cancel();
                    };
                    break;

                case Constants.PLAYER_ADD_TO_A_GAME:
                    builder.SetMessage("You have been added to a game. Show your current games!");
                    noticeAD = builder.Create();
                    noticeAD.Show();
                    noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                        updateNoticeList();
                    };
                    noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                    };
                    break;
                case Constants.FEEDBACK_GAME:
                    int noiticeId = (int) notice.GameID;
                    var screen_FeedBack = new Intent(this, typeof(Screen_FeedBack));
                    screen_FeedBack.PutExtra("sportId", notice.SportID);
                    screen_FeedBack.PutExtra("gameId", noiticeId);
                    screen_FeedBack.PutExtra("noticeId", notice.NoticeID);
                    StartActivity(screen_FeedBack);
                    break;
                case Constants.GAME_DELETED:
                    builder.SetMessage("A game you are joined, has been delted.");
                    noticeAD = builder.Create();
                    noticeAD.Show();
                    noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                        updateNoticeList();
                    };
                    noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                    };
                    break;
                case Constants.GAME_UPDATED:
                    builder.SetMessage("The game at: "+notice.Game.Place.Direction+" has been updated.");
                    noticeAD = builder.Create();
                    noticeAD.Show();
                    noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                        updateNoticeList();
                    };
                    noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                    };
                    break;
                case Constants.TEAM_DELETED:
                    builder.SetMessage("You have been removed for a team");
                    noticeAD = builder.Create();
                    noticeAD.Show();
                    noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                        updateNoticeList();
                    };
                    noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                    {
                        notice.Accepted = true;
                        try
                        {
                            noticeManager.UpdateNotice(notice);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }
                        noticeAD.Cancel();
                    };
                    break;
                default:
                    break;                
            }
        }
    }
}