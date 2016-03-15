using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.BusinessLayer.Managers;
using System;
using AppGeoFit.Droid.Screens;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;

namespace AppGeoFit.Droid
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditPlayer : Screen
    {
        AppSession appSession;

        protected override void OnCreate(Bundle bundle)
        {
            appSession = new AppSession(this.ApplicationContext);
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.EditPlayer);

            PlayerManager playerManager = new PlayerManager(false);
            Player player = appSession.getPlayer();

            //Recuperamos elementos
            EditText name_et = FindViewById<EditText>(Resource.Id.Edit_Name);
            name_et.Text = player.PlayerName;
            EditText lastName_et = FindViewById<EditText>(Resource.Id.Edit_LastName);
            lastName_et.Text = player.LastName;
            EditText nick_et = FindViewById<EditText>(Resource.Id.Edit_Nick);
            nick_et.Text = player.PlayerNick;
            EditText phoneNumber_et = FindViewById<EditText>(Resource.Id.Edit_PhoneNumber);
            phoneNumber_et.Text = player.PhoneNum.ToString();
            EditText email_et = FindViewById<EditText>(Resource.Id.Edit_Email);
            email_et.Text = player.PlayerMail;
            //TODO
            //EditText spinnerFavoriteSport_et = FindViewById<EditText>(Resource.Id.Edit_SpinnerFavoriteSport);
            //spinnerFavoriteSport_et.Text = player.FavoriteSport;
            Button acept_bn = FindViewById<Button>(Resource.Id.Edit_AceptButton);
            Button cancel_bn = FindViewById<Button>(Resource.Id.Edit_CancelButton);
            cancel_bn.Click += (o, e) => StartActivity(typeof(MainActivity));

            //Se crea el icono exclamation_error
            Drawable error = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            error.SetBounds(0, 0, error.IntrinsicWidth, error.IntrinsicHeight);

            #region Edit
            bool okN = false;
            bool okNi = false;
            bool okP = false;
            bool okE = false;
            int id_responseMail;
            int id_responseNick;
            bool okmail = false;
            bool oknick = false;
            bool okphone = false;
            bool update = false;
            int n = 0;
            string finalEmail = String.Empty;
            string[] emailParts;

            acept_bn.Click += (o, e) =>
            {
                okN = IsRequired(name_et, "Name is required", error);
                okNi = IsRequired(nick_et, "Nick is required", error);
                okP = IsRequired(phoneNumber_et, "Phone is required", error); 
                okE = IsRequired(email_et, "Email is required", error);

                okmail = IsValid(email_et, "It's not a correct email", error, Android.Util.Patterns.EmailAddress.Matcher(email_et.Text.ToString()).Matches());
                oknick = IsValid(nick_et, "Use only alphabets characters", error, Java.Util.Regex.Pattern.Compile("^[a-zA-Z ]+$").Matcher(nick_et.Text.ToString()).Matches());
                okphone = IsValid(phoneNumber_et, "It's not a correct phone", error, Android.Util.Patterns.Phone.Matcher(phoneNumber_et.Text.ToString()).Matches());

                id_responseMail = 0;
                id_responseNick = 0;
                update = false;
                
                if (!okN && !okNi && !okP && !okE && okmail && oknick && okphone)
                {
                    n = 0;
                    finalEmail = String.Empty;
                    okmail = false;
                    oknick = false;

                    // Comprobamos si el nick o el mail existen ya en base de datos
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

                    //Dado que puede ser nuestro propio nick el que encuentre, comprobaremos que no sea ese caso.
                    if (okmail && oknick)
                    {
                        update = true;
                    }
                    else
                    {
                        if (okmail && !oknick)
                        {
                            if (id_responseNick == player.PlayerId)
                                update = true;
                        }
                        if((!okmail && oknick))
                        {
                            if (id_responseMail == player.PlayerId)
                                update = true;
                        }                  
                        if(!oknick && !okmail)
                        {
                            if (id_responseNick == player.PlayerId && id_responseMail == player.PlayerId)
                                update = true;
                        }                         
                    }
                    if (update)
                    {
                        player.PlayerName = name_et.Text;
                        player.LastName = lastName_et.Text;
                        player.PlayerNick = nick_et.Text;
                        player.PhoneNum = Convert.ToInt32(phoneNumber_et.Text);
                        player.PlayerMail = email_et.Text;
                        appSession.setPlayer(player);
                        try
                        {
                            playerManager.UpdatePlayer(player);
                        }
                        catch (AggregateException aex)
                        {
                            foreach (var ex in aex.Flatten().InnerExceptions)
                            {
                                BotonAlert("Alert", ex.Message, "OK", "Cancel").Show();
                            }
                        }
                        StartActivity(typeof(MainActivity));
                        //this.Finish();
                    }
                    else
                    {
                        if(id_responseNick != player.PlayerId && !oknick)
                            BotonAlert("Alert", "This nick is allready used", "OK", "Cancel").Show();
                        else
                            BotonAlert("Alert", "This mail is allready used", "OK", "Cancel").Show();
                    }

                }
            };
            #endregion

        }
    }
}