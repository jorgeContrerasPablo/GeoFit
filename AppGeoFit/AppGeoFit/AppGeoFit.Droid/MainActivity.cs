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
       

        protected override void OnCreate (Bundle bundle)
		{

            Player player = new Player();

            player.PlayerId = 0;

            base.OnCreate (bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            PlayerManager playerManager = new PlayerManager();

            SetContentView(Resource.Layout.MainActivity);
            
            TextView text = FindViewById<TextView>(Resource.Id.textView1);

            player = playerManager.GetPlayer(11).Result;
            if (player != null)
            {
                text.Text = "Id : " + player.PlayerId;
            }
            else { text.Text = "Id : " + 0; }
           
        }
	}
}

