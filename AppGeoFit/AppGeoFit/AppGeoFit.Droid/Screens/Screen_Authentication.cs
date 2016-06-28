using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.DataAccesLayer.Models;
using Android.Content;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using Android.Views;
using AppGeoFit.BusinessLayer.Managers.TeamManager;


namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_Authentication : Activity
    {
        AppSession appSession;
        IPlayerManager playerManager;
        ITeamManager teamManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);
            teamManager = Xamarin.Forms.DependencyService.Get<ITeamManager>().InitiateServices(false);

            appSession = new AppSession(ApplicationContext);
            appSession.setSports(teamManager.GetSports().Result);

            // Comprobamos si el usuario aun tiene una sesion disponible para conectarse 
            // sin loguearse y que no esté ocupada por otro dispositivo
            if (appSession.getPlayer() != null)
            {
                if (!appSession.getPlayer().PlayerSesion)
                {
                    appSession.updateSession(true);
                    StartActivity(typeof(FragmentActivity_MainActivity));
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
                        StartActivity(typeof(FragmentActivity_MainActivity));
                    }
                }
            };
            #endregion

            //Sign Up Button
            signUpLink.Click += (o, e) => StartActivity(typeof(Screen_SignUp));

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

        public bool IsRequired(EditText editText, string message, Drawable error)
        {
            if (editText.Text.ToString().Length == 0)
            {
                editText.SetError(message, error);
                return true;
            }
            else {
                editText.SetError(String.Empty, null);
                editText.Error = null;
                return false;
            }
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


          #if DEBUG
  [assembly: Application(Debuggable=true)]
  #else
  [assembly: Application(Debuggable=false)]
  #endif
         */
    }
}
