using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.GameManager;

namespace AppGeoFit.Droid.Adapters
{
    class GameArrayAdapter : ArrayAdapter<Game>
    {
        Context context;
        List<Game> gameList;
        public GameArrayAdapter(Context context, List<Game> objects) : base(context, 0, objects)
        {
            this.context = context;
            this.gameList = objects;

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView startDate = new TextView(context);
            TextView endDate = new TextView(context);
            TextView numPlayers = new TextView(context);
            //Obteniendo una instancia del inflater
            LayoutInflater inflater = (LayoutInflater)Context
                    .GetSystemService(Context.LayoutInflaterService);
            //Salvando la referencia del View de la fila
            View listItemView = convertView;
            //Comprobando si el View no existe
            if (null == convertView)
            {
            listItemView = inflater.Inflate(Resource.Layout.ElementGameList, parent, false);

            }
            startDate = listItemView.FindViewById<TextView>(Resource.Id.ElementGameL_StartDate);
            endDate = listItemView.FindViewById<TextView>(Resource.Id.ElementGameL_EndDate);
            numPlayers = listItemView.FindViewById<TextView>(Resource.Id.ElementGameL_NumPlayers);
            Game item = GetItem(position);
            startDate.Text = item.StartDate.Day + "/" + item.StartDate.Month + "/" + item.StartDate.Year 
                + "  " + item.StartDate.Hour + ":" + item.StartDate.Minute;
            endDate.Text = item.EndDate.Day + "/" + item.EndDate.Month + "/" + item.EndDate.Year + "  " 
                + item.EndDate.Hour + ":" + item.EndDate.Minute;
            numPlayers.Text = item.PlayersNum + "/" + item.Sport.NumPlayers;

            return listItemView;
        }
    }  
}