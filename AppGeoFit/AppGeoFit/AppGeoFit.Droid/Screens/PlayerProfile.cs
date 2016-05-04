using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers;
using Android.Support.V4.App;

namespace AppGeoFit.Droid.Screens
{
    public class PlayerProfile : Fragment
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle );

            View view = inflater.Inflate(Resource.Layout.PlayerProfile, container, false);

            Player player;
            PlayerManager playerManager = new PlayerManager(false);
            AppSession appSession = new AppSession(Activity.ApplicationContext);
            //Player Profile Views
            TextView NameT = view.FindViewById<TextView>(Resource.Id.Name);
            TextView NickT = view.FindViewById<TextView>(Resource.Id.Nick);
            TextView LastNameT = view.FindViewById<TextView>(Resource.Id.LastName);
            TextView PhoneNumberT = view.FindViewById<TextView>(Resource.Id.PhoneNumber);
            TextView EmailT = view.FindViewById<TextView>(Resource.Id.Email);
            TextView OnTime = view.FindViewById<TextView>(Resource.Id.MedOnTime);
            RatingBar rating = view.FindViewById<RatingBar>(Resource.Id.ratingBar);

            //Indicar valores en pestaña profile mediante el usuario de la sesion
            player = appSession.getPlayer();
            NameT.Text = player.PlayerName;
            NickT.Text = player.PlayerNick;
            if(player.LastName != null)
                LastNameT.Text = player.LastName;
            else
                LastNameT.Enabled = false;
            PhoneNumberT.Text = player.PhoneNum.ToString();
            EmailT.Text = player.PlayerMail;
            rating.Rating = (int)player.Level;
            OnTime.Text = player.MedOnTime.ToString();

            //Button Edit
            ImageButton buttonEdit = view.FindViewById<ImageButton>(Resource.Id.imageButtonEdit);
            /*Drawable edit = ContextCompat.GetDrawable(this, Resource.Drawable.Edit);
            edit.SetBounds(0, 0, edit.IntrinsicWidth, edit.IntrinsicHeight);
            buttonEdit.SetImageDrawable(edit);*/
            buttonEdit.Click += (o, e) => Activity.StartActivity(typeof(EditPlayer));

            //Button Trash
            ImageButton buttonTrash = view.FindViewById<ImageButton>(Resource.Id.imageButtonDelete);
            /*Drawable trash = ContextCompat.GetDrawable(this, Resource.Drawable.Trash);
            trash.SetBounds(0, 0, trash.IntrinsicWidth, trash.IntrinsicHeight);
            buttonTrash.SetImageDrawable(trash);*/
            Android.App.AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;

            buttonTrash.Click += (o, e) =>
            {
                Screen screen = new Screen();
                baDelete = screen.BotonAlert("Alert", "Are you sure? Do you want to delete your account?", "OK", "Cancel", Context);
                baDelete.Show();
                baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                baDeletePositiveButton.Click += (oc, ec) =>
                {
                    playerManager.DeletePlayer(player.PlayerId);
                    appSession.deletePlayer();
                    Activity.StartActivity(typeof(Authentication));
                    Activity.Finish();
                };
            };           
            return view;
        }


    }
}