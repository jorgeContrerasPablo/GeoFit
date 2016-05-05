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

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SelectIp : Activity
    {        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
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
                    StartActivity(typeof(Authentication));                    
                }
            };
            ad.Show();
        }
    }
}