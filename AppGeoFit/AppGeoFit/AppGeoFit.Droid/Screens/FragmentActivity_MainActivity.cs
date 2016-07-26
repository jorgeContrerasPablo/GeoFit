using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AppGeoFit.BusinessLayer.Managers;
using System.Threading.Tasks;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using Android.Content;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using AppGeoFit.DataAccesLayer.Models;
using Android.Support.V4.App;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.BusinessLayer.Managers.NoticeManager;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.BusinessLayer.Managers.GameManager;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FragmentActivity_MainActivity : FragmentActivity
    {
        AppSession appSession;
        private FragmentTabHost mTabHost;
        //Inicializamos los servicios rest de los manager
        //con la url específica de la BD no Test
        public IPlayerManager playerManager { get; set; }
        public ITeamManager teamManager { get; set; }
        public INoticeManager noticeManager { get; set; }
        public IGameManager gameManager { get; set; }
        Player player;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            SetContentView(Resource.Layout.MainActivity);

            playerManager = DependencyService.Get<IPlayerManager>().InitiateServices(false);
            teamManager = DependencyService.Get<ITeamManager>().InitiateServices(false);
            noticeManager = DependencyService.Get<INoticeManager>().InitiateServices(false);
            gameManager = DependencyService.Get<IGameManager>().InitiateServices(false);
            //Recuperamos la sesion
            appSession = new AppSession(ApplicationContext);
            player = appSession.getPlayer();
            string tabTag = Intent.GetStringExtra("toOpen") ?? "TabGames";

           

            List<Sport> sportL = appSession.getSports();
            bool errorConnection = false;
            if (sportL.Count == 0)
            {
                errorConnection = true;
                BotonAlert("Alert", "There are problems with server connection, please try again in few minutes", "OK", "Cancel", this).Show();
            }
            if (!errorConnection)
            {
                // Init TabHost
                mTabHost = FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
                mTabHost.Setup(this, SupportFragmentManager, Android.Resource.Id.TabContent);

                mTabHost.AddTab(mTabHost.NewTabSpec("TabGames").SetIndicator("", ContextCompat.GetDrawable(this, Resource.Drawable.Games)),
                    Java.Lang.Class.FromType(typeof(Fragment_Games)), null);
                mTabHost.AddTab(mTabHost.NewTabSpec("TabTeam").SetIndicator("", ContextCompat.GetDrawable(this, Resource.Drawable.Team)),
                    Java.Lang.Class.FromType(typeof(Fragment_Team)), null);
                mTabHost.AddTab(mTabHost.NewTabSpec("TabProfile").SetIndicator("", ContextCompat.GetDrawable(this, Resource.Drawable.Player)),
                    Java.Lang.Class.FromType(typeof(Fragment_PlayerProfile)), null);

                mTabHost.SetCurrentTabByTag(tabTag);

                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarPrincipal);
                Spinner spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);

                //Mostramos las peticiones pendientes si es que las hay.
                List<Notice> pendingNotice = new List<Notice>();
                try
                {
                    pendingNotice = noticeManager.GetAllPendingNotice(player.PlayerId).ToList();
                    ShowNotice(pendingNotice);
                }
                catch (NotPendingNoticeException ex) { }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext,
                               ex.Message, ToastLength.Short).Show();
                }

                var adapter = new ArrayAdapter<Sport>(
                    this, Android.Resource.Layout.SimpleSpinnerItem, sportL);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerFavoriteSport_et.Adapter = adapter;
                spinnerFavoriteSport_et.SetSelection(player.Sport != null ? sportL.
                            FindIndex(s => s.SportName == player.Sport.SportName) : 0);
                SetActionBar(toolbar);
                ActionBar.Title = "GeoFit";

                /*            TabHost.TabSpec spec = tabH.NewTabSpec("TabGames");
                            //spec.SetContent(Resource.Id.Games);
                            spec.SetContent(new Intent(this, typeof(PlayerProfile)));
                            spec.SetIndicator("Games");
                            tabH.AddTab(spec);

                           spec = tabH.NewTabSpec("TabTeam");
                            spec.SetContent(Resource.Id.Team);
                            spec.SetIndicator("Team");
                            tabH.AddTab(spec);*/

                /*TabHost.TabSpec spec = tabH.NewTabSpec("TabProfile");
                spec.SetIndicator("Player Profile");
                spec.SetContent(new Intent(this, typeof(PlayerProfile)));
                //tabH.AddTab(spec, Java.Lang.Class.FromType(typeof(Android.Support.V4.App.Fragment)), null);
                tabH.AddTab(spec);*/
            }
        }
        void ShowNotice(List<Notice> pendingNotice)
        {
            AlertDialog noticeAD;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("New message");
            builder.SetPositiveButton("Acept", (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton("Deny", (EventHandler<DialogClickEventArgs>)null);

            //Por cada mensaje crearemos un alertDialog personalizado.
            foreach (Notice notice in pendingNotice)
            {
                switch (notice.Type)
                {
                    case Constants.TEAM_ADD_PLAYER:
                        builder.SetMessage("Team captain: " + notice.Messenger.PlayerNick + " want's add you to her/his team.");
                        noticeAD = builder.Create();
                        noticeAD.Show();
                        noticeAD.GetButton((int)DialogButtonType.Positive).Click += (oDb, eDb) =>
                        {
                            teamManager.AddPlayer(notice.ReceiverID, playerManager.FindTeamCaptainOnSport(notice.MessengerID, notice.SportID).TeamID);
                            notice.Accepted = true;
                            noticeManager.UpdateNotice(notice);
                            noticeAD.Cancel();
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
                            noticeManager.UpdateNotice(notice);
                            noticeAD.Cancel();
                        };
                        noticeAD.GetButton((int)DialogButtonType.Negative).Click += (oDb, eDb) =>
                        {
                            notice.Accepted = true;
                            noticeManager.UpdateNotice(notice);
                            noticeAD.Cancel();
                        };
                        break;
                    default:
                        break;
                }
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.principalMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        public AlertDialog BotonAlert(string title, string message, string positiveButton, string negativeButton, Context cntx)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(cntx);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButton, (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(negativeButton, (EventHandler<DialogClickEventArgs>)null);

            return builder.Create();
        }

        protected override void OnPause()
        {
            appSession = new AppSession(ApplicationContext);

            if (appSession.getPlayer() != null)
            {
                playerManager.OutSession(appSession.getPlayer().PlayerId);
                appSession.updateSession(false);
            }
            base.OnPause();

        }

        protected override void OnDestroy()
        {
            /* PlayerManager playerManager = new PlayerManager(false);
             appSession = new AppSession(ApplicationContext);
             playerManager.OutSession(appSession.getPlayer().PlayerId);
             appSession.deletePlayer();*/
            base.OnDestroy();

        }

        protected override void OnRestart()
        {
            base.OnRestart();
            appSession = new AppSession(this.ApplicationContext);
            if (appSession.getPlayer() != null)
            {
                try
                {
                    appSession.setPlayer(playerManager.GetPlayer(appSession.getPlayer().PlayerId).Result);
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        appSession.deletePlayer();
                        StartActivity(typeof(Screen_Authentication));
                    }
                }
                if (appSession.getPlayer().PlayerSesion)
                {
                    appSession.deletePlayer();
                    StartActivity(typeof(Screen_Authentication));
                }
                playerManager.Session(appSession.getPlayer().PlayerId);
                appSession.updateSession(true);
            }

        }
    }
}

