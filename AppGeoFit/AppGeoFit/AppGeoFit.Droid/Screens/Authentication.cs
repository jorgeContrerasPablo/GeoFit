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
            /*if(appSession.getPlayer() != null && appSession.getPlayer().PlayerSesion)
            {
                StartActivity(typeof(MainActivity));
            }*/

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
            string finalEmail = String.Empty;
            int n = 0;
            int response;
            bool okE;
            bool okP;
            int response2 = 0;
            string[] emailParts;

            signInB.Click += (o, e) =>
            {
                okE = IsRequired(emailOrNickT, "Name is required", error);
                okP = IsRequired(password, "password is required", error);

                response = 0;
                n = 0;
                finalEmail = String.Empty;
                if (!okE && !okP)
                {
                    try
                    {
                        emailParts = emailOrNickT.Text.Split('.');
                       
                        if(emailParts.Length > 1)
                        {
                            while (n <= emailParts.Length - 2)
                            {
                                if(n==0)
                                    finalEmail += emailParts[n];
                                else
                                {
                                    finalEmail += "."+emailParts[n];
                                }
                                n++;
                            }
                            response = playerManager.FindPlayerByMail(finalEmail, emailParts[emailParts.Length-1]).Result;
                        }
                        else response = playerManager.FindPlayerByNick(emailParts[0]).Result;
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
                        Player player = playerManager.GetPlayer(response).Result;
                        if (player.PlayerSesion)
                        {
                            BotonAlert("Alert", "User : " + player.PlayerNick + " is already connected", "OK", "Cancel").Show();
                        }
                        else
                        {
                            response2 = 0;
                            try
                            {
                                playerManager.Session(player.PlayerId);
                                response2 = 1;
                            }
                            catch (AggregateException aex)
                            {
                                foreach (var ex in aex.Flatten().InnerExceptions)
                                {
                                    BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
                                }
                            }
                            if (response2 != 0)
                            {
                                appSession.setPlayer(player);
                                StartActivity(typeof(MainActivity));
                                //this.Finish();
                            }
                        }
                       
                    }
                }
             };
            #endregion

            //Sign Up Button
            signUpLink.Click += (o, e) => StartActivity(typeof(SignUp));

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