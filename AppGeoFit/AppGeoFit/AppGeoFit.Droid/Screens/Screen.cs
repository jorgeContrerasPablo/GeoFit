using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.Graphics.Drawables;
using AppGeoFit.BusinessLayer.Managers;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using Xamarin.Forms;
using Android.Content.PM;
using Android.Views;

namespace AppGeoFit.Droid.Screens
{
    
    public class Screen : Activity
    {
        public AlertDialog BotonAlert(string title, string message, string positiveButton, string negativeButton, Context cntx)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(cntx);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButton, (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(negativeButton, (EventHandler<DialogClickEventArgs>)null);

            return builder.Create();
        }

        public AlertDialog BotonAlert(string title, string message)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);

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
        public AlertDialog CreateAlertDialog(int layout, Context context)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            LayoutInflater inflater = LayoutInflater;
            Android.Views.View dialogView;
            AlertDialog dialog;
            dialogView = inflater.Inflate(layout, null);
            builder.SetView(dialogView);
            dialog = builder.Create();
            return dialog;
        }
        

        protected override void OnPause()
        {
            AppSession appSession = new AppSession(this.ApplicationContext);
            IPlayerManager playerManager = DependencyService.Get<IPlayerManager>().InitiateServices(false);
            if (appSession.getPlayer() != null)
            {
                playerManager.OutSession(appSession.getPlayer().PlayerId);
                appSession.updateSession(false);
            }
            base.OnPause();

        }

        protected override void OnRestart()
        {
            base.OnRestart();
            IPlayerManager playerManager 
                = DependencyService.Get<IPlayerManager>().InitiateServices(false);
            AppSession appSession = new AppSession(this.ApplicationContext);
            if (appSession.getPlayer() != null)
            {
                try
                {
                    appSession.setPlayer(playerManager.GetPlayer(appSession.getPlayer().PlayerId).Result);
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        appSession.deletePlayer();
                        StartActivity(typeof(Screen_Authentication));
                    }
                }
                if (appSession.getPlayer().PlayerSesion)
                {
                    appSession.deletePlayer();
                    StartActivity(typeof(Screen_Authentication));
                }
                else
                {
                    playerManager.Session(appSession.getPlayer().PlayerId);
                    appSession.updateSession(true);
                }
            }            
        }
    }
}