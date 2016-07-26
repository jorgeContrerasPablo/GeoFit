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
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_UpdateGame : Screen
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UpdateGame);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            int gameId = Intent.GetIntExtra("gameId", 0);
            IGameManager gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);
            Game game = new Game();
            try
            {
                game = gameManager.GetGame(gameId);
            }catch(Exception ex)
            {
                Toast.MakeText(ApplicationContext,
                          ex.Message, ToastLength.Short).Show();
            }
            //TODO SPINNER SELECTOR DURACIOON
            #region datePicker
            //DatePicker
            TextView selectDate = FindViewById<TextView>(Resource.Id.UpdateGame_Date);
            DateTime date = game.StartDate;
            selectDate.Text = date.Day + "/" + date.Month + "/" + date.Year;
            DatePicker datePicker;
            AlertDialog dialogDate;
            MyOnDateChangeListener changeListener = new MyOnDateChangeListener();
            changeListener.setDate(date);
            selectDate.Click += (o, e) =>
            {
                dialogDate = CreateAlertDialog(Resource.Layout.dialog_DatePicker, this);
                dialogDate.Show();
                datePicker = dialogDate.FindViewById<DatePicker>(Resource.Id.datePicker1);
                changeListener = new MyOnDateChangeListener(dialogDate, selectDate, date);
                datePicker.Init(date.Year, date.Month, date.Day, changeListener);
            };
            #endregion
            #region timePicker
            //TimePicker
            TextView selectHour = FindViewById<TextView>(Resource.Id.UpdateGame_Time);
            DateTime timeNow = game.StartDate;
            selectHour.Text = timeNow.Hour + ":" + timeNow.Minute;
            AlertDialog dialogHour;
            TimePicker timePicker = new TimePicker(this);
            ImageButton timePicker_OKB;
            int hour = timeNow.Hour;
            int minute = timeNow.Minute;
            selectHour.Click += (o, e) =>
            {
                dialogHour = CreateAlertDialog(Resource.Layout.dialog_TimePicker, this);
                dialogHour.Show();
                timePicker.CurrentHour = (Java.Lang.Integer)hour;
                timePicker.CurrentMinute = (Java.Lang.Integer)minute;
                timePicker = dialogHour.FindViewById<TimePicker>(Resource.Id.timePicker1);
                timePicker_OKB = dialogHour.FindViewById<ImageButton>(Resource.Id.TimePicker_OK);
                timePicker_OKB.Click += (ot, et) => {
                    hour = (int)timePicker.CurrentHour;
                    minute = (int)timePicker.CurrentMinute;
                    selectHour.Text = hour + ":" + minute;
                    dialogHour.Cancel();
                };
            };
            #endregion
            #region SpinnerDuration
            Spinner durationSpinner = FindViewById<Spinner>(Resource.Id.UpdateGame_SpinnerDuration);
            int[] durations = new int[] { 1, 2, 3 };
            ArrayAdapter<int> adapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleSpinnerItem, durations);
            durationSpinner.Adapter = adapter;
            durationSpinner.SetSelection((game.EndDate.Hour - game.StartDate.Hour) -1);
            int gameDuration = (game.EndDate.Hour - game.StartDate.Hour);
            durationSpinner.ItemSelected += (o, e) =>
            {
                gameDuration = e.Position + 1;
            };
            #endregion
            Button acept = FindViewById<Button>(Resource.Id.UpdateGame_AceptButton);
            DateTime dateTimeStart = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
            acept.Click += (o, e) =>
            {
                dateTimeStart = new DateTime(changeListener.getDate().Year, changeListener.getDate().Month, changeListener.getDate().Day, hour, minute, 0);
                game.StartDate = dateTimeStart;
                game.EndDate = game.StartDate.AddHours(gameDuration);
                //TODO COORDINATES
                try
                {
                    gameManager.UpdateGame(game);
                    Toast.MakeText(ApplicationContext,
                            "Your Game has been update correctly", ToastLength.Short).Show();
                    var screen_GameDetails = new Intent(this, typeof(Screen_GameDetails));
                    screen_GameDetails.PutExtra("gameId", gameId);
                    //  screen_AddTeamToGame.PutExtra("gameId", adapterLGames.GetItem(info.Position).GameID);
                    StartActivity(screen_GameDetails);
                }
                catch (WrongTimeException ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                catch (GameOnTimeException ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }

            };
        }
    }
}