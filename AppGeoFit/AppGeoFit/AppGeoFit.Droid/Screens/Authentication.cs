using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.BusinessLayer.Managers;
using Android.Views;
using DevOne.Security.Cryptography.BCrypt;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using Android.Content;
using System.Collections.Generic;
using System.Linq;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Authentication : Screen
    {
        AppSession appSession;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            // Comprobamos si el usuario aun tiene una sesion disponible para conectarse 
            // sin loguearse y que no esté ocupada por otro dispositivo
            PlayerManager playerManager = new PlayerManager(false);          
            appSession = new AppSession(ApplicationContext);
            TeamManager teamManager = new TeamManager(false);
            appSession.setSports(teamManager.GetSports().Result);

            if (appSession.getPlayer() != null)
            {
                if (!appSession.getPlayer().PlayerSesion)
                {
                    appSession.updateSession(true);
                    StartActivity(typeof(MainActivity));
                }
            }

            SetContentView(Resource.Layout.Authentication);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarPrincipal);
            SetActionBar(toolbar);
            ActionBar.Title = "GeoFit";

            //Recuperamos elementos
            EditText emailOrNickT = FindViewById<EditText>(Resource.Id.NickOrEmailText);
            EditText password = FindViewById<EditText>(Resource.Id.passwordText);
            Button signInB = FindViewById<Button>(Resource.Id.SignInButton);
            TextView signUpLink = FindViewById<TextView>(Resource.Id.SignUpTextL);
            TextView changeIpLink = FindViewById<TextView>(Resource.Id.ChangeIpTextL);

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            #region ButtonSignin
            bool okE;
            bool okP;
            bool error;
            Player player = new Player();

            signInB.Click += (o, e) =>
            {
                okE = IsRequired(emailOrNickT, "Name is required", errorD);
                okP = IsRequired(password, "password is required", errorD);

                if (!okE && !okP)
                {
                    error = false;
                    try
                    {
                        player = playerManager.Authentication(emailOrNickT.Text, password.Text);
                    }
                    catch (Exception ex)
                    {
                        BotonAlert("Alert", ex.Message, "OK", "Cancel", this).Show();
                        error = true;
                    }
                    if (!error)
                    {
                        appSession.setPlayer(player);
                        StartActivity(typeof(MainActivity));
                    }
                }
            };
            #endregion

            //Sign Up Button
            signUpLink.Click += (o, e) => StartActivity(typeof(SignUp));

            // Change IP Debug Button
            changeIpLink.Click += (o, e) =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                // Get the layout inflater
                LayoutInflater inflater = LayoutInflater;

                View dialogView = inflater.Inflate(Resource.Layout.dialog_ip, null);
                // Inflate and set the layout for the dialog
                // Pass null as the parent view because its going in the dialog layout
                builder.SetView(dialogView);

                TextView ipText = dialogView.FindViewById<TextView>(Resource.Id.actualIp);
                ipText.Text = Constants.RestUrl;
                EditText ipTextChange = dialogView.FindViewById<EditText>(Resource.Id.editIp);
                Button okButton = dialogView.FindViewById<Button>(Resource.Id.buttonChangeIp);
                AlertDialog ad = builder.Create();
                okButton.Click += (i, p) =>
                {
                    if (ipTextChange.Text != "")
                    {
                        Constants.RestUrl = ipTextChange.Text;
                        ad.Cancel();
                    }
                };              
                ad.Show();
            };            
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        /*emailOrNickT.AfterTextChanged += (sender, e) =>
        {
            if (!imm.IsAcceptingText && emailOrNickT.Text.ToString().Length == 0)
            {                 
                 emailOrNickT.SetError("Email is required", errorEmail);
            }
        };

        password.TextChanged += (sender, e) =>
        {
            if (password.Text.ToString().Length == 0)
            {                 
                password.SetError("Email is required", errorPassword);                    
            }
        };*/

        /* To Hide the keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(emailOrNickT.WindowToken, 0);
        */
        //  emailOrNickT.cle
        //Keyboard keyboard = new Keyboard();


        /*Control del boton return

         public override void OnBackPressed()
         {
             if (emailOrNickT.Length() == 0)
             {
                 emailOrNickT.SetError("", null);
                 emailOrNickT.SetError("Email is required", errorEmail);
             }

             if (password.Length() == 0)
             {
                 password.SetError("", null);
                 password.SetError("Password is required", errorPassword);
             }
         }*/
    }
}
