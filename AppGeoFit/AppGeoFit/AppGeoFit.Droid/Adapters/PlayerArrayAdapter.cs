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
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.NoticeManager;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;

namespace AppGeoFit.Droid.Adapters
{
    public class PlayerArrayAdapter : ArrayAdapter<Player>
    {
        int captainId;
        int sportId;
        bool checkBox;
        Context context;
        List<int> checkeds;
        bool notice;

        public PlayerArrayAdapter(Context context, List<Player> objects, int captainId, int sportId, bool checkBox, List<int> checkeds, bool notice) : base(context, 0, objects)
        {
            this.captainId = captainId;
            this.sportId = sportId;
            this.checkBox = checkBox;
            this.context = context;
            this.checkeds = checkeds;
            this.notice = notice;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView Nick = new TextView(context);
            RatingBar RatingBar = new RatingBar(context);
            INoticeManager noticeManager
           = Xamarin.Forms.DependencyService.Get<INoticeManager>().InitiateServices(false);
            //Obteniendo una instancia del inflater
            LayoutInflater inflater = (LayoutInflater)Context
                    .GetSystemService(Context.LayoutInflaterService);

            //Salvando la referencia del View de la fila
            View listItemView = convertView;

            //Comprobando si el View no existe
            if (null == convertView)
            {
                if (checkBox)
                {
                    listItemView = inflater.Inflate(
                            Resource.Layout.ElementPlayerListCheckB,
                            parent,
                            false);                 
                }
                else {
                    listItemView = inflater.Inflate(
                            Resource.Layout.ElementPlayerList,
                            parent,
                            false);
                
                }
            }
            if (checkBox)
            {
                //Obteniendo instancias de los text views
                Nick = listItemView.FindViewById<TextView>(Resource.Id.ElementPlayerListCheckB_Nick);
                RatingBar = (RatingBar)listItemView.FindViewById(Resource.Id.ElementPlayerListCheckB_RatingBar);
                CheckBox checkBoxWidget = (CheckBox)listItemView.FindViewById(Resource.Id.ElementPlayerListCheckB_CheckBox);
                checkBoxWidget.Clickable = false;
                if (checkeds.Contains(position))
                    checkBoxWidget.Checked = true;
            }
            else
            {
                //Obteniendo instancias de los text views
                Nick = listItemView.FindViewById<TextView>(Resource.Id.ElementPlayerList_Nick);
                RatingBar = (RatingBar)listItemView.FindViewById(Resource.Id.ElementPlayerList_RatingBar);
            }

            //Obteniendo instancia de la Tarea en la posición actual
            Player item = GetItem(position);
            if (notice)
            {
                //Si está pendiente de ser agregado, lo oscurecemos.
                try {
                    if (noticeManager.NoticeIsPending(item.PlayerId, captainId, sportId, Constants.TEAM_ADD_PLAYER))
                    {
                        listItemView.SetBackgroundColor(Xamarin.Forms.Color.Default.ToAndroid());
                        listItemView.Background.SetColorFilter(Color.ParseColor("#80000000"), PorterDuff.Mode.Darken);
                    }
                }
                catch (NoticeNotFoundException ex){}
                catch (Exception ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
                }
            }

            Nick.Text = item.PlayerNick;
            RatingBar.Rating = (int)item.Level;

            //Devolver al ListView la fila creada
            return listItemView;
        }
    }
}