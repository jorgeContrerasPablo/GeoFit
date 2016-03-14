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

        public AlertDialog BotonAlert(string title, string message, string positiveButton, string negativeButton)
        {
            // BOTON ALERT
            AlertDialog builder = new AlertDialog.Builder(this).Create();
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetButton(positiveButton, (e,o)=> { });
            builder.SetButton2(negativeButton, (e, o) => { });

            return builder;
        }

        public AlertDialog BotonAlertb(string title, string message, string positiveButton, string negativeButton)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButton, (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(negativeButton, (EventHandler<DialogClickEventArgs>)null);

            return builder.Create();
        }

       
    }
}