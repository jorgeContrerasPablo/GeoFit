using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AppGeoFit.BusinessLayer.Managers;
using AppGeoFit.DataAccesLayer.Models;

namespace AppGeoFit.Droid.Screens
{
    public class Team : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);
            View view = inflater.Inflate(Resource.Layout.Team, container, false);

            TeamManager teamManager = new TeamManager(false);
            AppSession appSession = new AppSession(Activity.ApplicationContext);

            
            Button createPlayerB = view.FindViewById<Button>(Resource.Id.createTeamB);
            ListView playerList = view.FindViewById<ListView>(Resource.Id.playerListView);
            TextView teamNameT = view.FindViewById<TextView>(Resource.Id.teamName);
            ImageButton addPlayerButton = view.FindViewById<ImageButton>(Resource.Id.addPlayerButton);

            teamNameT.Text = teamManager.GetTeam(16).Result.Sport.SportName;
            
            createPlayerB.Click += (o,e) => Activity.StartActivity(typeof(CreateTeam));

            //addPlayerButton.Click += (o, e) => teamManager.AddPlayer(1, appSession.getPlayer().PlayerId,true);
            //playerList.Adapter = new playerListAdapter();

            return view;
        }
    }
}