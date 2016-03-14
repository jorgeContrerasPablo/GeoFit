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
            bool update = false;

            acept_bn.Click += (o, e) =>
            {
                okN = false;
                okNi = false;
                okP = false;
                okE = false;
                okmail = false;
                oknick = false;
                id_responseMail = 0;
                id_responseNick = 0;
                update = false;

                #region comprobacionCamposVacios
                if (email_et.Text.ToString().Length == 0)
                {
                    email_et.SetError("Email is required", error);
                    okE = false;
                }
                else {
                    email_et.SetError(String.Empty, null);
                    email_et.Error = null;
                    okE = true;
                }

                if (name_et.Text.ToString().Length == 0)
                {
                    name_et.SetError("Name is required", error);
                    okN = false;
                }
                else {
                    name_et.SetError(String.Empty, null);
                    name_et.Error = null;
                    okN = true;
                }

                if (nick_et.Text.ToString().Length == 0)
                {
                    nick_et.SetError("Nick is required", error);
                    okNi = false;
                }
                else {
                    nick_et.SetError(String.Empty, null);
                    nick_et.Error = null;
                    okNi = true;
                }

                if (phoneNumber_et.Text.ToString().Length == 0)
                {
                    phoneNumber_et.SetError("Phone is required", error);
                    okP = false;
                }
                else {
                    phoneNumber_et.SetError(String.Empty, null);
                    phoneNumber_et.Error = null;
                    okP = true;
                }
                #endregion


                if (okN && okNi && okP && okE)
                {

                    // Comprobamos si el nick o el mail existen ya en base de datos
                    try
                    {
                        id_responseMail = playerManager.FindPlayerByNickOrMail(email_et.Text).Result;
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
                        id_responseNick = playerManager.FindPlayerByNickOrMail(nick_et.Text).Result;
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