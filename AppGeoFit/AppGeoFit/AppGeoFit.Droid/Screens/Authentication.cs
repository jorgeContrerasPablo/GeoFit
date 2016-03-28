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

namespace AppGeoFit.Droid.Screens
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Authentication : Screen
    {
        AppSession appSession;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
           
            // Comprobamos si el usuario aun tiene una sesion disponible para conectarse sin loguearse y que no esté ocupada por otro dispositivo
            PlayerManager playerManager = new PlayerManager(false);
            appSession = new AppSession(this.ApplicationContext);
            
            if (appSession.getPlayer() != null)
            {
                if (!appSession.getPlayer().PlayerSesion)
                {
                    appSession.updateSession(true);
                    StartActivity(typeof(MainActivity));
                }
            }
            SetContentView(Resource.Layout.Authentication);

            //Recuperamos elementos
            EditText emailOrNickT = FindViewById<EditText>(Resource.Id.NickOrEmailText);
            EditText password = FindViewById<EditText>(Resource.Id.passwordText);
            Button signInB = FindViewById<Button>(Resource.Id.SignInButton);
            TextView signUpLink = FindViewById<TextView>(Resource.Id.SignUpTextL);

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
                        BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
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