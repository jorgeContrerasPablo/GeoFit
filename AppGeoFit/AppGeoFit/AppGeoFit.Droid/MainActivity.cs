using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AppGeoFit.BusinessLayer.Managers;
using System.Threading.Tasks;

namespace AppGeoFit.Droid
{
	[Activity (Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
        protected ListView taskListView = null;


        protected override void OnCreate(Bundle bundle)
        {

            Player player = new Player();

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            PlayerManager playerManager = new PlayerManager();

            SetContentView(Resource.Layout.MainActivity);

            TabHost tabH = FindViewById<TabHost>(Resource.Id.tabHost);
            tabH.Setup();

            TabHost.TabSpec spec = tabH.NewTabSpec("TabGames");
            spec.SetContent(Resource.Id.linearLayout2);
            spec.SetIndicator("Games");
            tabH.AddTab(spec);

            spec = tabH.NewTabSpec("TabTeam");
            spec.SetContent(Resource.Id.linearLayout3);
            spec.SetIndicator("Team");
            tabH.AddTab(spec);

            spec = tabH.NewTabSpec("TabProfile");
            spec.SetContent(Resource.Id.linearLayout4);
            spec.SetIndicator("Player Profile");
            tabH.AddTab(spec);

            TextView NameT = FindViewById<TextView>(Resource.Id.Name);
            TextView NickT = FindViewById<TextView>(Resource.Id.Nick);
            TextView LastNameT = FindViewById<TextView>(Resource.Id.LastName);
            TextView ResponseT = FindViewById<TextView>(Resource.Id.textView1);
            Button button = FindViewById<Button>(Resource.Id.button1);
            Button button2 = FindViewById<Button>(Resource.Id.button2);
            TextView ResponseT2 = FindViewById<TextView>(Resource.Id.textView2);

            //player = playerManager.GetPlayer(10).Result;
            NameT.Text = player.PlayerName;
            NickT.Text = player.PlayerNick;
            LastNameT.Text = player.LastName;
            int playerBID = 0;

            Player playerB = new Player(); ;
            Player playerToUpdate = new Player();

            button.Click += (o, e) =>
            {
                
                playerB.PlayerName = "Jugador Creado";
                playerB.Password = "HAbria q cambiar esto";
                playerB.PlayerNick = "nick";
                playerB.LastName = "lastname";
                playerB.PhoneNum = 12312123;
                playerB.PlayerMail = "@mail";
                playerB.Level = 5.3;
                playerB.MedOnTime = 1;
                playerBID = playerManager.CreatePlayer(playerB).Result;
                ResponseT.Text = "Jugador creado con id : " + playerBID;
                //ResponseT.Text = playerManager.DeletePlayer(2).Result.ToString();
            };

            button2.Click += (o, e) =>
            {
                
                playerToUpdate = playerManager.GetPlayer(playerBID).Result;
                playerToUpdate.PlayerName = "Jugador Modificado";
                ResponseT2.Text = "Jugador modificado ? => " + playerManager.UpdatePlayer(playerToUpdate).Result.ToString();
            };
        }
	}
}

