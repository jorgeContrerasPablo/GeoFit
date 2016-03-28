using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.BusinessLayer.Managers;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using Java.Util.Regex;
using Android.OS;
using AppGeoFit.BusinessLayer.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SignUp : Screen
    {
        AppSession appSession;
        protected override void OnCreate(Bundle bundle)
        {
            appSession = new AppSession(this.ApplicationContext);
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.SignUp);

            PlayerManager playerManager = new PlayerManager(false);
            Player player = new Player();

            //Recuperamos elementos
            EditText name_et = FindViewById<EditText>(Resource.Id.SignUp_Name);
            EditText password_et = FindViewById<EditText>(Resource.Id.SignUp_Password);
            EditText repeatePassword_et = FindViewById<EditText>(Resource.Id.SignUp_RepeatPassword);
            EditText lastName_et = FindViewById<EditText>(Resource.Id.SignUp_LastName);
            EditText nick_et = FindViewById<EditText>(Resource.Id.SignUp_Nick);
            EditText phoneNumber_et = FindViewById<EditText>(Resource.Id.SignUp_PhoneNumber);
            EditText email_et = FindViewById<EditText>(Resource.Id.SignUp_Email);
            //TODO
            Spinner spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.SignUp_SpinnerFavoriteSport);

            Button acept_bn = FindViewById<Button>(Resource.Id.SignUp_AceptButton);
            Button cancel_bn = FindViewById<Button>(Resource.Id.SignUp_CancelButton);
            cancel_bn.Click += (o, e) => StartActivity(typeof(MainActivity));

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            bool okN = false;
            bool okPass = false;
            bool okRpassR = false;
            bool okRpassV = false;
            bool okNi = false;
            bool okP = false;
            bool okE = false;
            bool okmail = false;
            bool oknick = false;
            bool okphone = false;
            bool error = false;


            acept_bn.Click += (o, e) =>
            {
                okN = IsRequired(name_et, "Name is required", errorD);
                okPass = IsRequired(password_et, "Password is required", errorD);
                okRpassR = IsRequired(repeatePassword_et, "Repeate password", errorD);
                okNi = IsRequired(nick_et, "Nick is required", errorD);
                okP = IsRequired(phoneNumber_et, "Phone is required", errorD);
                okE = IsRequired(email_et, "Email is required", errorD);

                okmail = IsValid(email_et, "It's not a correct email", errorD, Android.Util.Patterns.EmailAddress.Matcher(email_et.Text.ToString()).Matches());
                oknick = IsValid(nick_et, "Use only alphabets characters", errorD, Java.Util.Regex.Pattern.Compile("^[a-zA-Z ]+$").Matcher(nick_et.Text.ToString()).Matches());
                okphone = IsValid(phoneNumber_et, "It's not a correct phone", errorD, Android.Util.Patterns.Phone.Matcher(phoneNumber_et.Text.ToString()).Matches());
                okRpassV = IsValid(repeatePassword_et, "Repeate the password correctly", errorD, password_et.Text.Equals(repeatePassword_et.Text));

                if (!okN && !okNi && !okP && !okE && !okPass && !okRpassR && okRpassV && okmail && oknick && okphone)
                {
                    error = false;
                    okmail = IsValid(email_et, "", errorD, true);
                    oknick = IsValid(nick_et, "", errorD, true);

                    player.PlayerName = name_et.Text;
                    player.LastName = lastName_et.Text;
                    player.Password = password_et.Text;
                    player.PlayerNick = nick_et.Text;
                    player.PhoneNum = Convert.ToInt32(phoneNumber_et.Text);
                    player.PlayerMail = email_et.Text;
                    //TODO favorite sport
                    try
                    {
                        playerManager.CreatePlayer(player);
                    }
                    catch (DuplicatePlayerNickException exN)
                    {
                        error = true;
                        oknick = IsValid(nick_et, exN.Message, errorD, false);
                    }
                    catch (DuplicatePlayerMailException exM)
                    {
                        error = true;
                        okmail = IsValid(email_et, exM.Message, errorD, false);
                    }
                    catch (Exception ex)
                    {
                        BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
                    }
                   
                    if (!error)
                    {
                       // Dialog a = BotonAlert("The account has been created correctly", "OK");
                        Toast.MakeText(ApplicationContext, "Your account has been created correctly", ToastLength.Short);
                        StartActivity(typeof(Authentication));
                    }
                }
                
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
    }
}