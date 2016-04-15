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
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Models;

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

            // TODO EDIT PASSWORD

            Button acept_bn = FindViewById<Button>(Resource.Id.Edit_AceptButton);
            Button cancel_bn = FindViewById<Button>(Resource.Id.Edit_CancelButton);
            cancel_bn.Click += (o, e) => StartActivity(typeof(MainActivity));

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            #region Edit
            bool reN = false;
            bool reNi = false;
            bool reP = false;
            bool reE = false;
            bool okmail = false;
            bool oknick = false;
            bool okphone = false;
            bool error = false;

            acept_bn.Click += (o, e) =>
            {
                reN = IsRequired(name_et, "Name is required", errorD);
                reNi = IsRequired(nick_et, "Nick is required", errorD);
                reP = IsRequired(phoneNumber_et, "Phone is required", errorD); 
                reE = IsRequired(email_et, "Email is required", errorD);

                okmail = IsValid(email_et, "It's not a correct email", errorD, Android.Util.Patterns.EmailAddress.Matcher(email_et.Text.ToString()).Matches());
                oknick = IsValid(nick_et, "Use only alphabets characters", errorD, Java.Util.Regex.Pattern.Compile("^[a-zA-Z ]+$").Matcher(nick_et.Text.ToString()).Matches());
                okphone = IsValid(phoneNumber_et, "It's not a correct phone", errorD, Android.Util.Patterns.Phone.Matcher(phoneNumber_et.Text.ToString()).Matches());

                if (!reN && !reNi && !reP && !reE && okmail && oknick && okphone)
                {
                    error = false;

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
                        BotonAlert("Alert", ex.Message, "OK", "Cancel",this).Show();
                    }
                    if (!error)
                    {
                        //Dialog a = BotonAlert("The account has been created correctly", "OK");
                        Toast.MakeText(ApplicationContext, "Your account has been update correctly", ToastLength.Short);
                        StartActivity(typeof(MainActivity));
                    }

                }
            };
            #endregion

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