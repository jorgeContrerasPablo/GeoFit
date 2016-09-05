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
    class NoticeArrayAdapter : ArrayAdapter<Notice>
    {
        Context context;
        List<Notice> noticeList;
        public NoticeArrayAdapter(Context context, List<Notice> objects) : base(context, 0, objects)
        {
            this.context = context;
            this.noticeList = objects;

        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //Obteniendo una instancia del inflater
            LayoutInflater inflater = (LayoutInflater)Context
                    .GetSystemService(Context.LayoutInflaterService);
            //Salvando la referencia del View de la fila
            View listItemView = convertView;
            //Comprobando si el View no existe
            if (null == convertView)
            {
                listItemView = inflater.Inflate(Resource.Layout.ElementNoticeList, parent, false);
            }
            TextView playerSend = listItemView.FindViewById<TextView>(Resource.Id.ElementNoticeL_SendBy);
            TextView message = listItemView.FindViewById<TextView>(Resource.Id.ElementNoticeL_Message);
            Notice item = GetItem(position);

            playerSend.Text = item.Messenger.PlayerNick;
            switch (item.Type)
            {
                case Constants.TEAM_ADD_PLAYER:
                    // message.Text = "Team captain: " + item.Messenger.PlayerNick + " want's add you to her/his team.";
                    message.Text = "Team captain: " + item.Messenger.PlayerNick + " want's...";
                    break;

                case Constants.PLAYER_ADD_TO_A_GAME:
                    //message.Text = "You have been added to a game. Show your current games!";   
                    message.Text = "You have been added to a game...";
                    break;
                default:
                    break;
            }
            return listItemView;
        }       
        
    }
}