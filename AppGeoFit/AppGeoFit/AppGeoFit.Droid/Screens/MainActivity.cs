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
        protected ListView taskListView = null;

        protected override void OnCreate(Bundle bundle)
        {
            
            AppSession appSession = new AppSession(this.ApplicationContext);

            Player player = new Player();
            PlayerManager playerManager = new PlayerManager(false);

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.MainActivity);

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
                baDelete = BotonAlertb("Alert", "Are you sure? Do you want to delete your account?", "OK", "Cancel");
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

                 

            // Prube create and update
            int playerBID = 0;

            Player playerB = new Player(); ;
            Player playerToUpdate = new Player();

            //Create player Bottons
            TextView ResponseT = FindViewById<TextView>(Resource.Id.textView1);
            Button button = FindViewById<Button>(Resource.Id.button1);
            Button button2 = FindViewById<Button>(Resource.Id.button2);
            TextView ResponseT2 = FindViewById<TextView>(Resource.Id.textView2);

            button.Click += (o, e) => 
            {
                
                playerB.PlayerName = "Jugador Creado para rating";
                playerB.Password = "HAbria q cambiar esto";
                playerB.PlayerNick = "otronick";
                playerB.LastName = "lastname";
                playerB.PhoneNum = 12312123;
                playerB.PlayerMail = "@otromail";
                playerB.Level = 1;
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

