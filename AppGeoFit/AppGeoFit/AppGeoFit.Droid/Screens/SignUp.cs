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
            Drawable error = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            error.SetBounds(0, 0, error.IntrinsicWidth, error.IntrinsicHeight);

            bool okN = false;
            bool okPass = false;
            bool okRpassR = false;
            bool okRpassV = false;
            bool okNi = false;
            bool okP = false;
            bool okE = false;
            int id_responseMail;
            int id_responseNick;
            bool okmail = false;
            bool oknick = false;
            bool okphone = false;
            string[] emailParts = null;
            int n = 0;
            string finalEmail = String.Empty;

            acept_bn.Click += (o, e) =>
            {
                okN = IsRequired(name_et, "Name is required", error);
                okPass = IsRequired(password_et, "Password is required", error);
                okRpassR = IsRequired(repeatePassword_et, "Repeate password", error);
                okNi = IsRequired(nick_et, "Nick is required", error);
                okP = IsRequired(phoneNumber_et, "Phone is required", error);
                okE = IsRequired(email_et, "Email is required", error);

                id_responseMail = 0;
                id_responseNick = 0;

                okmail = IsValid(email_et, "It's not a correct email", error, Android.Util.Patterns.EmailAddress.Matcher(email_et.Text.ToString()).Matches());
                oknick = IsValid(nick_et, "Use only alphabets characters", error, Java.Util.Regex.Pattern.Compile("^[a-zA-Z ]+$").Matcher(nick_et.Text.ToString()).Matches());
                okphone = IsValid(phoneNumber_et, "It's not a correct phone", error,Android.Util.Patterns.Phone.Matcher(phoneNumber_et.Text.ToString()).Matches());
                okRpassV = IsValid(repeatePassword_et, "Repeate the password correctly", error, password_et.Text.Equals(repeatePassword_et.Text));

                if (!okN && !okNi && !okP && !okE && !okPass && !okRpassR && okRpassV && okmail && oknick && okphone)
                {
                    okmail = false;
                    oknick = false;
                    finalEmail = String.Empty;
                    n = 0;

                    try
                    {
                        emailParts = email_et.Text.Split('.');
                        while (n <= emailParts.Length - 2)
                        {
                            if (n == 0)
                                finalEmail += emailParts[n];
                            else
                            {
                                finalEmail += "." + emailParts[n];
                            }
                            n++;
                        }
                        id_responseMail = playerManager.FindPlayerByMail(emailParts[0], emailParts[1]).Result;
                    }
                    catch (AggregateException aex)
                    {
                        foreach (var ex in aex.Flatten().InnerExceptions)
                        {
                            if (ex is PlayerNotFoundException)
                                okmail = true;
                        }
                    }

                    try
                    {
                        id_responseNick = playerManager.FindPlayerByNick(nick_et.Text).Result;
                    }
                    catch (AggregateException aex)
                    {
                        foreach (var ex in aex.Flatten().InnerExceptions)
                        {
                            if (ex is PlayerNotFoundException)
                                oknick = true;
                        }
                    }

                    if (okmail && oknick)
                    {
                        player.PlayerName = name_et.Text;
                        player.LastName = lastName_et.Text;
                        //TODO encrypt
                        player.Password = password_et.Text;
                        player.PlayerNick = nick_et.Text;
                        player.PhoneNum = Convert.ToInt32(phoneNumber_et.Text);
                        player.PlayerMail = email_et.Text;
                        //TODO favorite sport
                        try
                        {
                            playerManager.CreatePlayer(player);
                        }
                        catch (AggregateException aex)
                        {
                            foreach (var ex in aex.Flatten().InnerExceptions)
                            {
                                BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
                            }
                        }
                        //Todo better button
                        BotonAlert("Atention", "The account has been created correctly", "OK", "Cancel").Show();
                        StartActivity(typeof(Authentication));
                        //this.Finish();
                    }
                    else
                    {
                        if (!oknick)
                            BotonAlert("Alert", "This nick is allready used", "OK", "Cancel").Show();
                        else {
                            if (!okmail)
                                BotonAlert("Alert", "This mail is allready used", "OK", "Cancel").Show();
                        }
                    }


                }
                
            };
        }
    }
}