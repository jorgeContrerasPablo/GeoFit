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

namespace AppGeoFit.Droid.Adapters
{
    class CommentArrayAdapter : ArrayAdapter<FeedBack>
    {
        Context context;
        List<FeedBack> feedBackList;
        public CommentArrayAdapter(Context context, List<FeedBack> objects) : base(context, 0, objects)
        {
            this.context = context;
            this.feedBackList = objects;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //Obteniendo una instancia del inflater
            LayoutInflater inflater = (LayoutInflater)Context
                    .GetSystemService(Context.LayoutInflaterService);
            //Salvando la referencia del View de la fila
            View listItemView = convertView;
            //Comprobando si el View no existe
            FeedBack item = GetItem(position);
            if (null == convertView)
            {
                listItemView = inflater.Inflate(Resource.Layout.ElementCommentList, parent, false);
            }
            RatingBar rate = listItemView.FindViewById<RatingBar>(Resource.Id.ElementCommentList_Rating);
            //Si es un comentario sobre la partida, no contendrá rate, por lo que se pondría invisible
            if (item.GameID == null)
            {
                rate.Rating = (float)item.Valuation;
            }
            else
                rate.Visibility = ViewStates.Invisible;

            TextView creator = listItemView.FindViewById<TextView>(Resource.Id.ElementCommentList_Creator);
            TextView date = listItemView.FindViewById<TextView>(Resource.Id.ElementCommentList_Date);
            TextView comment = listItemView.FindViewById<TextView>(Resource.Id.ElementCommentList_Comment);
            creator.Text = item.CreatorID == null ? "" : item.Creator.PlayerNick;
            date.Text = item.FeedBackDate.Day + "/" + item.FeedBackDate.Month + "/" + item.FeedBackDate.Year
                + "  " + item.FeedBackDate.Hour + ":" + item.FeedBackDate.Minute;
            comment.Text = item.Description;

            return listItemView;
        }
    }
}