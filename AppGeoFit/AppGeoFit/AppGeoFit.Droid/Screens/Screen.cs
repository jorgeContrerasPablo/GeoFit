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
using AppGeoFit.DataAccesLayer.Models;
using Java.Util;

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

        public AlertDialog ShowPlayerDetails(Player player)
        {
            AlertDialog dialogProfile = CreateAlertDialog(Resource.Layout.PlayerDetails, this);
            dialogProfile.Show();
            dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_Name).Text = player.PlayerName;
            dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_Nick).Text = player.PlayerNick;
            dialogProfile.FindViewById<RatingBar>(Resource.Id.PlayerDetails_ratingBar).Rating = (int)player.Level;
            dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_MedOnTime).Text = string.Format("{0:P2}", player.MedOnTime);
            dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_Email).Text = player.PlayerMail;
            dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_ShowCommentsLink).Click += (o, e) =>
            {
                var screen_Comments = new Intent(this, typeof(Screen_Comments));
                screen_Comments.PutExtra("playerId", player.PlayerId);
                StartActivity(screen_Comments);
            };

            return dialogProfile;
        }
        
        public AlertDialog ShowPlaceDetails(Place place)
        {
            AlertDialog dialogPlaceDetails;
            dialogPlaceDetails = CreateAlertDialog(Resource.Layout.PlaceDetails, this);
            dialogPlaceDetails.Show();
            TextView locationText = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_Map);
            TextView placeEmail = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_Email);
            TextView placeLink = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_Link);
            TextView placeName = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_Name);
            TextView placePhone = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_PhoneNum);
            RatingBar placeValue = dialogPlaceDetails.FindViewById<RatingBar>(Resource.Id.PlaceDetails_ValuationMed);
            TextView placeDirection = dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_Direction);
            dialogPlaceDetails.FindViewById<TextView>(Resource.Id.PlaceDetails_ShowCommentsLink).Click += (o, e) =>
            {
                var screen_Comments = new Intent(this, typeof(Screen_Comments));
                screen_Comments.PutExtra("placeId", place.PlaceID);
                StartActivity(screen_Comments);
            };

            locationText.Text = place.Longitude.ToString() + "  " + place.Latitude.ToString();
            placeEmail.Text = place.PlaceMail;
            placeLink.Text = place.Link == "" ? "" : place.Link.Substring(0, 6) + "...";
            placeName.Text = place.PlaceName;
            placePhone.Text = place.PhoneNum == null ? place.PhoneNum.ToString() : "";

            placeValue.Rating = place.ValuationMed == null ? 0 : (int)place.ValuationMed;
            placeDirection.Text = place.Direction;
            locationText.Click += (ol, el) =>
            {
                string coordinates = Java.Lang.String.Format(Locale.English, "geo:%f,%f?z=16&q=%f,%f ", place.Longitude, place.Latitude, place.Longitude, place.Latitude);/* "geo:" + place.Longitude
                                                                                             + "," + place.Latitude
                                                                                             + "?z=16&q=" + place.Longitude
                                                                                             + "," + place.Latitude);*/
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(coordinates));

                intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
                StartActivity (intent);
            };
            placeLink.Click += (od, ed) =>
            {
                var uri = Android.Net.Uri.Parse(place.Link);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };
            return dialogPlaceDetails;
        }

        protected override void OnPause()
        {
            AppSession appSession = new AppSession(this.ApplicationContext);
            IPlayerManager playerManager = DependencyService.Get<IPlayerManager>().InitiateServices(false);
            if (appSession.getPlayer() != null)
            {
                try
                {
                    playerManager.OutSession(appSession.getPlayer().PlayerId);
                    appSession.updateSession(false);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
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
                    appSession.setPlayer(playerManager.GetPlayer(appSession.getPlayer().PlayerId));
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
                    try
                    {
                        playerManager.Session(appSession.getPlayer().PlayerId);
                        appSession.updateSession(true);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
            }            
        }
    }
}