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

namespace AppGeoFit.Droid.Screens
{
	[Activity (Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class FragmentActivity_MainActivity : FragmentActivity
	{
        AppSession appSession;
        private FragmentTabHost mTabHost;
        //Inicializamos los servicios rest de los manager
        //con la url específica de la BD no Test
        public IPlayerManager playerManager { get; set; } 
            = DependencyService.Get<IPlayerManager>().InitiateServices(false);
        public ITeamManager teamManager { get; set; } 
            = DependencyService.Get<ITeamManager>().InitiateServices(false);

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            SetContentView(Resource.Layout.MainActivity);
                     
            //Recuperamos la sesion
            appSession = new AppSession(ApplicationContext);
            Player player = appSession.getPlayer();
            string tabTag = Intent.GetStringExtra("toOpen") ?? "TabProfile";

            // Init TabHost
            mTabHost = FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            mTabHost.Setup(this, SupportFragmentManager, Android.Resource.Id.TabContent);

            mTabHost.AddTab(mTabHost.NewTabSpec("TabProfile").SetIndicator("Player Profile"),
                Java.Lang.Class.FromType(typeof(Fragment_PlayerProfile)), null);
            mTabHost.AddTab(mTabHost.NewTabSpec("TabTeam").SetIndicator("Team"),
                Java.Lang.Class.FromType(typeof(Fragment_Team)), null);

            mTabHost.SetCurrentTabByTag(tabTag);

            List<Sport> sportL = appSession.getSports().ToList();
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarPrincipal);
            Spinner spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);

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

