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

namespace AppGeoFit.Droid
{
	[Activity (Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Screens.Screen
	{
        protected ListView taskListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.MainActivity);
            AppSession appSession = new AppSession(this.ApplicationContext);

            Player player;
            PlayerManager playerManager = new PlayerManager(false);

            // Init TabHost
            TabHost tabH = FindViewById<TabHost>(Resource.Id.tabHost);
            tabH.Setup();

            TabHost.TabSpec spec = tabH.NewTabSpec("TabGames");
            spec.SetContent(Resource.Id.Games);
            spec.SetIndicator("Games");
            tabH.AddTab(spec);

            spec = tabH.NewTabSpec("TabTeam");
            spec.SetContent(Resource.Id.linearLayout3);
            spec.SetIndicator("Team");
            tabH.AddTab(spec);

            spec = tabH.NewTabSpec("TabProfile");
            spec.SetContent(Resource.Id.PlayerProfileL);
            spec.SetIndicator("Player Profile");
            tabH.AddTab(spec);

            //Player Profile Views
            TextView NameT = FindViewById<TextView>(Resource.Id.Name);
            TextView NickT = FindViewById<TextView>(Resource.Id.Nick);
            TextView LastNameT = FindViewById<TextView>(Resource.Id.LastName);
            TextView PhoneNumberT = FindViewById<TextView>(Resource.Id.PhoneNumber);
            TextView EmailT = FindViewById<TextView>(Resource.Id.Email);
            TextView OnTime = FindViewById<TextView>(Resource.Id.MedOnTime);
            RatingBar rating = FindViewById<RatingBar>(Resource.Id.ratingBar);

            //Indicar valores en pestaña profile mediante el usuario de la sesion
            player = appSession.getPlayer();
            NameT.Text = player.PlayerName;
            NickT.Text = player.PlayerNick;
            LastNameT.Text = player.LastName;
            PhoneNumberT.Text = player.PhoneNum.ToString();
            EmailT.Text = player.PlayerMail;
            rating.Rating = (int)player.Level;
            OnTime.Text = player.MedOnTime.ToString();

            //Button Edit
            ImageButton buttonEdit = FindViewById<ImageButton>(Resource.Id.imageButtonEdit);
            /*Drawable edit = ContextCompat.GetDrawable(this, Resource.Drawable.Edit);
            edit.SetBounds(0, 0, edit.IntrinsicWidth, edit.IntrinsicHeight);
            buttonEdit.SetImageDrawable(edit);*/
            buttonEdit.Click += (o, e) => StartActivity(typeof(EditPlayer));

            //Button Trash
            ImageButton buttonTrash = FindViewById<ImageButton>(Resource.Id.imageButtonDelete);
            /*Drawable trash = ContextCompat.GetDrawable(this, Resource.Drawable.Trash);
            trash.SetBounds(0, 0, trash.IntrinsicWidth, trash.IntrinsicHeight);
            buttonTrash.SetImageDrawable(trash);*/
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            buttonTrash.Click += (o, e) =>
            {
                baDelete = BotonAlert("Alert", "Are you sure? Do you want to delete your account?", "OK", "Cancel");
                baDelete.Show();
                baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                baDeletePositiveButton.Click += (oc, ec) =>
                {
                    playerManager.DeletePlayer(player.PlayerId);
                    appSession.deletePlayer();
                    StartActivity(typeof(Screens.Authentication));
                };
            };         

        }


        protected override void OnPause()
        {
            PlayerManager playerManager = new PlayerManager(false);
            AppSession appSession = new AppSession(this.ApplicationContext);

            if (appSession.getPlayer() != null)
            {
                playerManager.OutSession(appSession.getPlayer().PlayerId);
                appSession.updateSession(false);
            }
            base.OnPause();

        }

        protected override void OnDestroy()
        {
            PlayerManager playerManager = new PlayerManager(false);
            AppSession appSession = new AppSession(this.ApplicationContext);
            playerManager.OutSession(appSession.getPlayer().PlayerId);
            appSession.deletePlayer();
            base.OnDestroy();
            //Finish();

        }
    }
}

