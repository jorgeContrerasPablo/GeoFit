using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Java.Lang;
using Android.Text;
using Android.Support.V4.Content;

namespace AppGeoFit.Droid
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditPlayer : Activity
    {
        EditText emailT;
        private Context a;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
           // LoadApplication(new App());
            SetContentView(Resource.Layout.EditPlayer);

            emailT = FindViewById<EditText>(Resource.Id.editText1);

            
            Context b = a.ApplicationContext;

        }

        protected override void OnResume()
        {
            base.OnResume();
            Drawable error = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            error.SetBounds(0, 0, error.IntrinsicWidth, error.IntrinsicHeight);
            emailT.TextChanged += (sender, e) =>
             {
                 if (emailT.Text.ToString().Length == 0)
                 {
                     emailT.SetError("Email is required", error);
                 }
                 
             };
        }
    }
}