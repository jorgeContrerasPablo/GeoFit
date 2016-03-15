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
using Android.Graphics.Drawables;
using AppGeoFit.BusinessLayer.Managers;

namespace AppGeoFit.Droid.Screens
{
    public class Screen : Activity
    {
        public Screen() { }

        public AlertDialog BotonAlert(string title, string message, string positiveButton, string negativeButton)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButton, (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(negativeButton, (EventHandler<DialogClickEventArgs>)null);

            return builder.Create();
        }

        public bool IsRequired(EditText editText, string message, Drawable error)
        {
            if (editText.Text.ToString().Length == 0)
            {
                editText.SetError(message, error);
                return true;
            }
            else {
                editText.SetError(String.Empty, null);
                editText.Error = null;
                return false;
            }
        }

        public bool IsValid (EditText editText, string message, Drawable error, bool match)
        {
            if (!match)
            {
                editText.SetError(message, error);
                return false;
            }
            else {
                editText.SetError(String.Empty, null);
                editText.Error = null;
                return true;
            }
        }

        public string[] split (string email)
        {
            return email.Split('.');
        }

      /*  protected override void OnStop()
        {
            base.OnStop();
            PlayerManager playerManager = new PlayerManager(false);
            AppSession appSession = new AppSession(this.ApplicationContext);
            playerManager.OutSession(appSession.getPlayer().PlayerId);
            appSession.updateSession(false);

        }

        protected override void OnRestart()
        {
            base.OnRestart();
            PlayerManager playerManager = new PlayerManager(false);
            AppSession appSession = new AppSession(this.ApplicationContext);
            if (appSession.getPlayer().PlayerSesion)
            {
                StartActivity(typeof(Authentication));
            }
            playerManager.Session(appSession.getPlayer().PlayerId);
            appSession.updateSession(true);
        }*/
    }
}