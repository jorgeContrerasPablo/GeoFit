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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using Xamarin.Forms;

namespace AppGeoFit.Droid
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_EditPlayer : Screen
    {
        AppSession appSession;
        IPlayerManager playerManager;

        protected override void OnCreate(Bundle bundle)
        {
            appSession = new AppSession(this.ApplicationContext);
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            SetContentView(Resource.Layout.EditPlayer);

            playerManager = DependencyService.Get<IPlayerManager>().InitiateServices(false);

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
            
            Spinner spinnerFavoriteSport_et = FindViewById<Spinner>(Resource.Id.Edit_SpinnerFavoriteSport);
            ICollection<Sport> sports = appSession.getSports();

            // Creamos una lista con un elemento en blanco al principio.
            List<Sport> sportLS = sports.ToList();
            Sport sportBlank = new Sport();
            sportBlank.SportName = "";
            sportBlank.SportID = 0;
            sportLS.Insert(0, sportBlank);

            //Spinner control
            spinnerFavoriteSport_et.ItemSelected += (o, e) =>
            {
                if (sportLS.ElementAt<Sport>(e.Position).SportName != "")
                {
                    player.FavoriteSportID = sportLS.ElementAt<Sport>(e.Position).SportID;
                    player.Sport = sportLS.ElementAt<Sport>(e.Position);
                }
                else
                {
                    player.FavoriteSportID = null;
                    player.Sport = null;
                }
            };
                    
            var adapter = new ArrayAdapter<Sport>(
                this, Android.Resource.Layout.SimpleSpinnerItem, sportLS);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerFavoriteSport_et.Adapter = adapter;
            spinnerFavoriteSport_et.SetSelection(player.Sport != null ? sportLS.FindIndex(s => s.SportName == player.Sport.SportName) : 0);

            // TODO EDIT PASSWORD
            Android.Widget.Button acept_bn = FindViewById<Android.Widget.Button>(Resource.Id.Edit_AceptButton);
            Android.Widget.Button cancel_bn = FindViewById<Android.Widget.Button>(Resource.Id.Edit_CancelButton);
            cancel_bn.Click += (o, e) => Finish(); //StartActivity(typeof(MainActivity));

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
                    player.PlayerName = name_et.Text;
                    player.LastName = lastName_et.Text;
                    player.PlayerNick = nick_et.Text;
                    player.PhoneNum = Convert.ToInt32(phoneNumber_et.Text);
                    player.PlayerMail = email_et.Text;
                    appSession.setPlayer(player);

                    try
                    {
                        playerManager.UpdatePlayer(player);
                        Toast.MakeText(this, "Your account has been update correctly", ToastLength.Long).Show();                        
                        Finish();//StartActivity(typeof(MainActivity));

                        
                    }
                    catch (DuplicatePlayerNickException exN)
                    {
                        oknick = IsValid(nick_et, exN.Message, errorD, false);
                    }
                    catch (DuplicatePlayerMailException exM)
                    {
                        okmail = IsValid(email_et, exM.Message, errorD, false);
                    }
                    catch (Exception ex)
                    {
                        BotonAlert("Alert", ex.Message, "OK", "Cancel",this).Show();
                    }

                }
            };
            #endregion

        }

        public static async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }
    }
}