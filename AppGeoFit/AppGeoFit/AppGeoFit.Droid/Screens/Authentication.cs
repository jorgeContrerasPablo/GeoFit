using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.BusinessLayer.Managers;
using Android.Views;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Authentication : Screen
    {
        AppSession appSession;

        protected override void OnCreate(Bundle bundle)
        {
            appSession = new AppSession(this.ApplicationContext);
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.Authentication);

            PlayerManager playerManager = new PlayerManager(false);

            //Recuperamos elementos
            EditText emailOrNickT = FindViewById<EditText>(Resource.Id.NickOrEmailText);
            EditText password = FindViewById<EditText>(Resource.Id.passwordText);
            Button signInB = FindViewById<Button>(Resource.Id.SignInButton);
            TextView signUpLink = FindViewById<TextView>(Resource.Id.SignUpTextL);

            //Se crea el icono exclamation_error
            Drawable error = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);         
            error.SetBounds(0, 0, error.IntrinsicWidth, error.IntrinsicHeight);

            #region ButtonSignin
            int response;
            bool okE;
            bool okP;
            signInB.Click += (o, e) =>
            {
                okE = false;
                okP = false;

                if (emailOrNickT.Text.ToString().Length == 0)
                {
                    emailOrNickT.SetError("Email is required", error);
                    okE = false;
                }
                else {
                    emailOrNickT.SetError(String.Empty, null);
                    emailOrNickT.Error = null;
                    okE = true;
                }

                if (password.Text.ToString().Length == 0)
                {
                    password.SetError("Password is required", error);
                    okP = false;
                }
                else {
                    password.SetError(String.Empty, null);
                    password.Error = null;
                    okP = true;
                }

                response = 0;
                if (okE && okP)
                {
                    try
                    {
                        response = playerManager.FindPlayerByNickOrMail(emailOrNickT.Text).Result;
                    }
                    catch (AggregateException aex)
                    {                      
                        foreach (var ex in aex.Flatten().InnerExceptions)
                        {
                            BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
                        }
                    }
                    if (response != 0)
                    {
                        appSession.setPlayer(playerManager.GetPlayer(response).Result);
                        StartActivity(typeof(MainActivity));
                        this.Finish();
                    }
                }
             };
            #endregion

            //Sign Up Button
            signUpLink.Click += (o, e) => StartActivity(typeof(SignUp));

        }

        protected override void OnResume()
        {
            base.OnResume();

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


        }


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