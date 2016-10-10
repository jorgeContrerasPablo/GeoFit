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
using AppGeoFit.BusinessLayer.Managers;
using AppGeoFit.DataAccesLayer.Models;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using Xamarin.Forms;
using AppGeoFit.BusinessLayer.Managers.TeamManager;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_EditTeam : Screen
    {
        AppSession appSession;
        ITeamManager teamManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            SetContentView(Resource.Layout.EditTeam);

            teamManager = DependencyService.Get<ITeamManager>().InitiateServices(false);


            //Recuperamos la sesion
            appSession = new AppSession(ApplicationContext);

            Team team = new Team();
            Player captain = new Player();
            Player newCaptain = new Player();
            int teamId = Intent.GetIntExtra("teamId", 0);
            if (teamId != 0)
            {
                try
                {
                    team = teamManager.GetTeam(teamId);
                    captain = teamManager.GetCaptainAsync(teamId);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext,ex.Message, ToastLength.Short).Show();
                }
            }
            string colorCode = Intent.GetStringExtra("ColorCode") ?? team.ColorTeam;
            string teamName = Intent.GetStringExtra("teamName") ?? team.TeamName;

            Android.Widget.Button updateButton = FindViewById<Android.Widget.Button>(Resource.Id.EditTeam_AceptButton);
            Android.Widget.Button cancelButton = FindViewById<Android.Widget.Button>(Resource.Id.EditTeam_CancelButton);
            Android.Widget.Button selectColor = FindViewById<Android.Widget.Button>(Resource.Id.EditTeam_ColorButton);
            EditText teamNameET = FindViewById<EditText>(Resource.Id.EditTeam_Name);
            Spinner spinnerCaptains = FindViewById<Spinner>(Resource.Id.EditTeam_SpinnerCaptain);
            ImageView colorView = FindViewById<ImageView>(Resource.Id.EditTeam_imageColor);
            //Iniciamos con valores actuales.
            colorView.SetBackgroundColor(Android.Graphics.Color.ParseColor(colorCode));
            teamNameET.Text = teamName;

            //Se crea el icono exclamation_error
            Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            //Spinner control
            List<Player> lplayersSelectCaptain = new List<Player>();
            var n = 0;
            while (n < team.Joineds.Count)
            {
                lplayersSelectCaptain.Add(team.Joineds.ElementAt(n).Player);
                n++;
            }
            spinnerCaptains.ItemSelected += (o, e) =>
            {
                newCaptain = lplayersSelectCaptain.ElementAt(e.Position);
            };
            var adapter = new ArrayAdapter<Player>(
                    this, Android.Resource.Layout.SimpleSpinnerItem, lplayersSelectCaptain);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerCaptains.Adapter = adapter;

            bool okName = false;
            updateButton.Click += (o, e) =>
            {
                okName = IsRequired(teamNameET, "Team name is required", errorD);
                if (!okName)
                {
                    team.TeamName = teamNameET.Text;
                    team.ColorTeam = colorCode;
                    try
                    {
                        teamManager.UpdateTeam(team, newCaptain);
                        Toast.MakeText(ApplicationContext,
                            "Your Team has been update correctly", ToastLength.Short).Show();
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
                    catch (AlreadyCaptainOnSportException ex)
                    {
                        Toast.MakeText(this,ex.Message, ToastLength.Long).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
            };

            cancelButton.Click += (o, e) =>
            {
                Finish();
            };
            selectColor.Click += (o, e) =>
            {
                var colorPicker = new Intent(ApplicationContext, typeof(Screen_ColorPicker));
                colorPicker.PutExtra("teamId", teamId);
                StartActivity(colorPicker);
            };

        }
    }
}