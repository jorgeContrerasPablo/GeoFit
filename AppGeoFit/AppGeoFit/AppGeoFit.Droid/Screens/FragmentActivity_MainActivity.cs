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
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;

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
        public IFeedBackManager feedBackManager { get; set; }
        Player player;
        int sportPosition = 0;
        List<Sport> sportL;
        Spinner spinnerFavoriteSport_et;

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

            sportL = appSession.getSports();

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
            spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);
            
            ImageButton noticesButton = FindViewById<ImageButton>(Resource.Id.Toolbar_noticeButton);
            noticesButton.Click += (o, e) =>
            {
                StartActivity(typeof(Screen_Notices));
            };
            SetActionBar(toolbar);
            ActionBar.Title = "GeoFit";
        }

        protected override void OnResume()
        {
            base.OnStart();
            //Se cambiará el icono de los avisos según haya o no avisos pendientes.
            ImageButton noticeButton = FindViewById<ImageButton>(Resource.Id.Toolbar_noticeButton);
            try
            {
                if (noticeManager.TotalNoticesCount(player.PlayerId) > 0)
                {
                    noticeButton.SetImageResource(Resource.Drawable.MessagePlus);
                }
                else
                {
                    noticeButton.SetImageResource(Resource.Drawable.Message);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

            spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);

            //Se actualiza la posicion del spinner de deportes segun tenga ya uno elegido en la sesion o 
            // si no es el caso, se selecciona el favorito.
            var adapter = new ArrayAdapter<Sport>(
                this, Android.Resource.Layout.SimpleSpinnerItem, sportL);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerFavoriteSport_et.Adapter = adapter;

            if (appSession.getSelectedSport() == 0)
            {
                sportPosition = (player.Sport != null ? sportL.
                        FindIndex(s => s.SportName == player.Sport.SportName) : 0);
                spinnerFavoriteSport_et.SetSelection(sportPosition);
                appSession.setSelectedSport(spinnerFavoriteSport_et.SelectedItem.GetHashCode());
            }
            else {
                sportPosition = (sportL.FindIndex(s => s.SportID == appSession.getSelectedSport()));
                spinnerFavoriteSport_et.SetSelection(sportPosition);
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
            playerManager = DependencyService.Get<IPlayerManager>().InitiateServices(false);
            if (appSession.getPlayer() != null)
            {
                try
                {
                    playerManager.OutSession(appSession.getPlayer().PlayerId);
                    appSession.updateSession(false);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            }
            base.OnPause();

        }
        protected override void OnRestart()
        {
            base.OnRestart();
            playerManager
                = DependencyService.Get<IPlayerManager>().InitiateServices(false);
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
                else
                {
                    try
                    {
                        playerManager.Session(appSession.getPlayer().PlayerId);
                        appSession.updateSession(true);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                    
                }
            }
        }
    }        
}

