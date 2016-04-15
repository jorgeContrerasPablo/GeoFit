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
using Android.Content.PM;
using Android.Support.V4.App;
using AppGeoFit.BusinessLayer.Managers;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Label = "AppGeoFit", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class CreateTeam : Screen
    {
        AppSession appSession;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.CreateTeam);
            appSession = new AppSession(ApplicationContext);
            TeamManager teamManager = new TeamManager(false);

            Button aceptButton = FindViewById<Button>(Resource.Id.CreateTeam_AceptButton);
            Button cancelButton = FindViewById<Button>(Resource.Id.CreateTeam_CancelButton);
            EditText teamNameET = FindViewById<EditText>(Resource.Id.CreateTeam_Name);
            Spinner spinnerSports = FindViewById<Spinner>(Resource.Id.CreateTeam_SpinnerSport);
            DataAccesLayer.Models.Team team = new DataAccesLayer.Models.Team();

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            // TODO METER ESTO EN LA SESSION
            ICollection<Sport> sports = teamManager.GetSports().Result;

            List<String> sportsID = new List<String>();

            //Recojemos la lista de Sports y creamos una lista con los nombres para el spinner
            var n = 0;
            while (n < sports.Count - 1)
            {
                sportsID.Add(sports.ElementAt<Sport>(n).SportName);
                n++;
            }

            //Spinner control
            string sportName = "";
            spinnerSports.ItemSelected += (o, e) =>
                                    {
                                        sportName = sports.ElementAt<Sport>(e.Position).SportName;
                                        team.SportID = sports.ElementAt<Sport>(e.Position).SportID;
                                    };
            var adapter = new ArrayAdapter<String>(
                    this, Android.Resource.Layout.SimpleSpinnerItem, sportsID);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerSports.Adapter = adapter;            

            bool okName = false;
            aceptButton.Click += (o, e) =>
            {
                okName = IsRequired(teamNameET, "Team name is required", errorD);
                if (!okName)
                {                  
                    team.TeamName = teamNameET.Text;
                    //TODO Color picker
                    team.ColorTeam = "Green";                   
                    try
                    {
                        teamManager.CreateTeam(team, appSession.getPlayer());
                    }
                    catch (DuplicateTeamNameException exN)
                    {
                        okName = IsValid(teamNameET, exN.Message, errorD, false);
                    }
                    catch (DuplicateCaptainException)
                    {
                        Toast.MakeText(this, "Player :" + appSession.getPlayer().PlayerName + " is al ready captain on sport: "+ sportName + ".", ToastLength.Long).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
            };

            cancelButton.Click += (o,e) =>  StartActivity(typeof(MainActivity));
        }
    }
}