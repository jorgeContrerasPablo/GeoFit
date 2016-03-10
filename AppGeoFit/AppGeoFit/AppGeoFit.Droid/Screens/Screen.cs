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

namespace AppGeoFit.Droid.Screens
{
    public class Screen : Activity
    {
        public Screen() {}

        public AlertDialog BotonAlert(string title, string message, string botton, string botton2, string botton3)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog ad = builder.Create();
            ad.SetTitle(title);
            ad.SetMessage(message);
            ad.SetButton(botton, (s, e) => { });
            ad.SetButton2(botton2, (s, e) => { });
            ad.SetButton3(botton3, (s, e) => { });

            return ad;
        }
    }
}