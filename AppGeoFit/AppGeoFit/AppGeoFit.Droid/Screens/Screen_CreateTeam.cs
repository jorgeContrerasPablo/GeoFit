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
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using Xamarin.Forms;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_CreateTeam : Screen
    {
        AppSession appSession;
        ITeamManager teamManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            SetContentView(Resource.Layout.CreateTeam);

            teamManager = DependencyService.Get<ITeamManager>().InitiateServices(false);

            //Recuperamos la sesion
            appSession = new AppSession(ApplicationContext);            

            appSession.setSports(teamManager.GetSports().Result);
            string colorCode = Intent.GetStringExtra("ColorCode") ?? "#ffffff";
            string teamName = Intent.GetStringExtra("teamName") ?? "";

            Android.Widget.Button aceptButton = FindViewById<Android.Widget.Button>(Resource.Id.CreateTeam_AceptButton);
            Android.Widget.Button cancelButton = FindViewById<Android.Widget.Button>(Resource.Id.CreateTeam_CancelButton);
            Android.Widget.Button selectColor = FindViewById<Android.Widget.Button>(Resource.Id.CreateTeam_ColorButton);
            EditText teamNameET = FindViewById<EditText>(Resource.Id.CreateTeam_Name);
            //Controlamos que hemos escrito el nombre del equipo, antes
            //de darle un color.
            if (teamName != "")
                teamNameET.Text = teamName;
            Spinner spinnerSports = FindViewById<Spinner>(Resource.Id.CreateTeam_SpinnerSport);
            ImageView colorView = FindViewById<ImageView>(Resource.Id.CreateTeam_imageColor);
            colorView.SetBackgroundColor(Android.Graphics.Color.ParseColor(colorCode));
            DataAccesLayer.Models.Team team = new DataAccesLayer.Models.Team();

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            ICollection<Sport> sports = appSession.getSports();
            //Spinner control
            string sportName = "";
            spinnerSports.ItemSelected += (o, e) =>
                                    {
                                            sportName = sports.ElementAt<Sport>(e.Position).SportName;
                                            team.SportID = sports.ElementAt<Sport>(e.Position).SportID;
                                            //team.Sport = sports.ElementAt<Sport>(e.Position);                                        
                                    };
            var adapter = new ArrayAdapter<Sport>(
                    this, Android.Resource.Layout.SimpleSpinnerItem, sports.ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerSports.Adapter = adapter;            

            bool okName = false;
            aceptButton.Click += (o, e) =>
            {
                okName = IsRequired(teamNameET, "Team name is required", errorD);
                if (!okName)
                {                  
                    team.TeamName = teamNameET.Text;
                    team.ColorTeam = colorCode;                   
                    try
                    {
                        teamManager.CreateTeam(team, appSession.getPlayer());
                        Toast.MakeText(ApplicationContext, 
                            "Your Team has been create correctly", ToastLength.Short).Show();
                        //Creamos intent y le asignamos el fragment 
                        //que debe abrir y después finalizamos la actual activity
                        //con el flag cleartop.
                        var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                        mainActivity.PutExtra("toOpen", "TabTeam");
                        mainActivity.SetFlags(ActivityFlags.ClearTop);                        
                        StartActivity(mainActivity);
                    }
                    catch (DuplicateTeamNameException exN)
                    {
                        okName = IsValid(teamNameET, exN.Message, errorD, false);
                    }
                    catch (AlreadyCaptainOnSport ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
            };

            cancelButton.Click += (o,e) => Finish();
            selectColor.Click += (o, e) =>
            {
                var colorPicker = new Intent(ApplicationContext, typeof(Screen_ColorPicker));
                colorPicker.PutExtra("teamName", teamNameET.Text);
                StartActivity(colorPicker);
            };
        }

    }
}