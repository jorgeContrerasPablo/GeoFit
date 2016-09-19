using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;
using Android.Graphics;
using Android.Util;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_CreateGame : Screen
    {
        int actualSportId;
        List<Player> ListPlayers_Captain = new List<Player>();
        Team team = new Team();
        bool captain = true;
        bool addTeam = false;
        Player actualPlayer;
        ListView finalSelectView;
        PlayerArrayAdapter finalArrayAdapter;
        List<Player> finalSelectList = new List<Player>();
        List<Player> SelectedList_captain = new List<Player>();
        List<Player> SelectedList_Individual = new List<Player>();
        IPlayerManager playerManager;
        IGameManager gameManager;
        IFeedBackManager feedBackManager;
        Game game = new Game();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CreateGame);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            actualSportId = Intent.GetIntExtra("sportId", 0);
            AppSession appSession = new AppSession(ApplicationContext);
            actualPlayer = appSession.getPlayer();

            playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);
            gameManager = Xamarin.Forms.DependencyService.Get<IGameManager>().InitiateServices(false);

            feedBackManager = Xamarin.Forms.DependencyService.Get<IFeedBackManager>().InitiateServices(false);
            try
            { team = playerManager.FindTeamCaptainOnSport(actualPlayer.PlayerId,
                     actualSportId);
            }
            catch(CaptainNotFoundException ex)
            {
                captain = false;
            }            

            #region datePicker
            //DatePicker
            TextView selectDate = FindViewById<TextView>(Resource.Id.CreateGame_Date);
            DateTime date = DateTime.Now;
            selectDate.Text = date.Day + "/" + date.Month + "/" + date.Year;           
            DatePicker datePicker;
            AlertDialog dialogDate;
            MyOnDateChangeListener changeListener = new MyOnDateChangeListener();
            selectDate.Click += (o, e) =>
            {
                dialogDate = CreateAlertDialog(Resource.Layout.dialog_DatePicker, this);
                dialogDate.Show();
                datePicker = dialogDate.FindViewById<DatePicker>(Resource.Id.datePicker1);
                changeListener = new MyOnDateChangeListener(dialogDate, selectDate, date);
                changeListener.setDate(date);
                datePicker.Init(date.Year, date.Month -1, date.Day, changeListener);
            };
            #endregion
            #region timePicker
            //TimePicker
            TextView selectHour = FindViewById<TextView>(Resource.Id.CreateGame_Time);
            DateTime timeNow = DateTime.Now;
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
                timePicker.CurrentHour = (Java.Lang.Integer) hour;
                timePicker.CurrentMinute = (Java.Lang.Integer) minute;
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
            #region SelectPlayer_Captain
            //SelectPlayer_Captain
            Button addPlayers = FindViewById<Button>(Resource.Id.CreateGame_SelectPlayers);
            finalSelectView = FindViewById<ListView>(Resource.Id.CreateGame_PlayersList);
            finalSelectList.Add(actualPlayer);
            finalArrayAdapter = new PlayerArrayAdapter(
                       this, finalSelectList,
                       0, actualSportId, false, null, false);
            finalSelectView.Adapter = finalArrayAdapter;
            RegisterForContextMenu(finalSelectView);
            RegisterForContextMenu(addPlayers);
            addPlayers.Click += (o, e) =>
            {
                addPlayers.PerformLongClick();
            };
            #endregion
            #region SpinnerPlaces
            List<Place> places = gameManager.GetPlaces(actualSportId);
            PlaceSpinner placeSpinner = FindViewById<PlaceSpinner>(Resource.Id.CreateGame_SpinnerPlaces);
           // PlaceSpinner placeSpinner = new PlaceSpinner(normalPlaceSpinner.Context);
            ArrayAdapter<Place> adapter_place = new ArrayAdapter<Place>(this, Android.Resource.Layout.SimpleSpinnerItem, places);
            placeSpinner.Adapter = adapter_place;

            int actualPositionSpinner = 0;
            Place placeSelected = new Place();
            AlertDialog dialogPlaceDetails = CreateAlertDialog(Resource.Layout.PlaceDetails, this);
            int timesOnSpinner = 0;
            game.PlaceID = places.ElementAt(0).PlaceID;
            placeSpinner.ItemSelected += (o, e) =>
            {
                actualPositionSpinner = placeSpinner.SelectedItemPosition;
                placeSelected = places.ElementAt(actualPositionSpinner);
                if (timesOnSpinner > 0)
                {
                    placeSelected = places.ElementAt(actualPositionSpinner);
                    dialogPlaceDetails = ShowPlaceDetails(placeSelected);
                    ImageButton aceptButton = dialogPlaceDetails.FindViewById<ImageButton>(Resource.Id.PlaceDetails_AcceptButton);
                    aceptButton.Click += (oB, eB) =>
                    {
                        game.PlaceID = placeSelected.PlaceID;
                        dialogPlaceDetails.Cancel();
                    };
                }
                else
                    timesOnSpinner++;                                  
            };
            #endregion
            #region SpinnerDuration
            Spinner durationSpinner = FindViewById<Spinner>(Resource.Id.CreateGame_SpinnerDuration);
            int[] durations = new int[]{1,2,3};
            ArrayAdapter<int> adapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleSpinnerItem, durations);
            durationSpinner.Adapter = adapter;
            int gameDuration = 1;
            durationSpinner.ItemSelected += (o, e) =>
            {
                gameDuration = durationSpinner.SelectedItemPosition + 1;
            };
            #endregion
            Button acept = FindViewById<Button>(Resource.Id.CreateGame_AceptButton);
            Button cancel = FindViewById<Button>(Resource.Id.CreateGame_CancelButton);
            DateTime dateTimeStart = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
            cancel.Click += (o, e) => 
            {
                var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                mainActivity.PutExtra("toOpen", "TabGame");
                mainActivity.SetFlags(ActivityFlags.ClearTop);
                StartActivity(mainActivity);
            };
            acept.Click += (o, e) =>
            {
                dateTimeStart = new DateTime(changeListener.getDate().Year, changeListener.getDate().Month, changeListener.getDate().Day, hour, minute, 0);               
                game.Players = finalSelectList;
                game.PlayersNum = finalSelectList.Count;
                game.SportId = actualSportId;
                game.CreatorID = actualPlayer.PlayerId;
                game.StartDate = dateTimeStart;
                game.EndDate = game.StartDate.AddHours(gameDuration);
                if (addTeam)
                {
                    game.Team1ID = team.TeamID;
                }
                try
                {
                    gameManager.CreateGame(game);
                    Toast.MakeText(ApplicationContext,
                            "Your Game has been create correctly", ToastLength.Short).Show();
                    //Creamos intent y le asignamos el fragment 
                    //que debe abrir y después finalizamos la actual activity
                    //con el flag cleartop.
                    var mainActivity = new Intent(ApplicationContext, typeof(FragmentActivity_MainActivity));
                    mainActivity.PutExtra("toOpen", "TabGame");
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(mainActivity);
                }
                catch(WrongTimeException ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                catch(GameOnTimeException ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                catch (GameOnTimeAndPlaceException ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                }
                
            };
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
            MenuInflater inflater = new MenuInflater(this);
            if (v.Id == Resource.Id.CreateGame_PlayersList)
            {   
                if(finalSelectView.Adapter.GetItem(info.Position).GetHashCode() == actualPlayer.PlayerId)
                    inflater.Inflate(Resource.Menu.MenuPlayerListNoC, menu);
                else
                    inflater.Inflate(Resource.Menu.MenuPlayerList, menu);
            }
            else
            {
                menu.SetHeaderTitle("What do you prefere?");
                inflater.Inflate(Resource.Menu.MenuCreateTeam, menu);
            }

        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            switch (item.ItemId)
            {                
                case Resource.Id.MenuCTCaptain:
                    if(!captain)
                    {
                        Toast.MakeText(ApplicationContext,
                            "You aren't a captain on this sport for any team", ToastLength.Short).Show();
                    }
                    else
                    {
                        AlertDialog dialogSelect_Captain;
                        dialogSelect_Captain = CreateAlertDialog(Resource.Layout.dialog_SelectPlayers_Captain, this);
                        dialogSelect_Captain.Show();
                        ListView playerList = dialogSelect_Captain.FindViewById<ListView>(Resource.Id.SelectPlayers_Captain_playerListView);
                        Button aceptButton = dialogSelect_Captain.FindViewById<Button>(Resource.Id.SelectPlayers_Captain_AceptButton);
                        Team actualTeam;
                        List<int> positionsCheckeds = new List<int>();

                        //SelectedList_captain.Clear();

                        ListPlayers_Captain.Clear();
                        positionsCheckeds.Clear();
                        int i = 0;
                        List<Joined> joineds = team.Joineds.ToList();
                        joineds.RemoveAt(joineds.FindIndex(j => j.Player.PlayerId == actualPlayer.PlayerId));
                        foreach (Joined j in joineds)
                        {
                            ListPlayers_Captain.Add(j.Player);
                            //Si está contenido en nuestra lista actual, guardamos las posiciones,
                            //para especificar al adapter las posiciones que estan marcadas
                            if (finalSelectList.Contains(j.Player))
                                positionsCheckeds.Add(i);
                            i++;
                        }
                        PlayerArrayAdapter adapterLPlayers = new PlayerArrayAdapter(
                                                this, ListPlayers_Captain,
                                                0, actualSportId, true, positionsCheckeds, false);
                        playerList.Adapter = adapterLPlayers;
                        playerList.ItemClick += (oc, ec) =>
                        {
                            CheckBox cBox = ec.View.FindViewById<CheckBox>(Resource.Id.ElementPlayerListCheckB_CheckBox);
                            if (cBox.Checked)
                            {
                                cBox.Checked = false;
                                SelectedList_captain.Remove(adapterLPlayers.GetItem(ec.Position));
                                if (SelectedList_Individual.Contains(adapterLPlayers.GetItem(ec.Position)))
                                    SelectedList_Individual.Remove(adapterLPlayers.GetItem(ec.Position));
                            }
                            else
                            {
                                cBox.Checked = true;
                                SelectedList_captain.Add(adapterLPlayers.GetItem(ec.Position));
                            }
                        };

                        aceptButton.Click += (oc, ec) =>
                        {
                            addTeam = false;
                            finalSelectList.Clear();
                            finalSelectList.Add(actualPlayer);
                            finalSelectList.AddRange(SelectedList_Individual);
                            finalSelectList.AddRange(SelectedList_captain);
                            finalArrayAdapter = new PlayerArrayAdapter(
                                                   this, finalSelectList,
                                                   0, actualSportId, false, null, false);
                            finalSelectView.Adapter = finalArrayAdapter;
                            if (SelectedList_captain.Count != 0)
                            {
                                addTeam = true;
                            }
                            dialogSelect_Captain.Cancel();

                        };

                    }
                    return true;
                case Resource.Id.MenuCTIndividual:
                    //Se crea el icono exclamation_error.
                    Drawable errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
                    errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

                    AlertDialog dialogSelect_Individual = CreateAlertDialog(Resource.Layout.dialog_SearchPlayer, this);
                    dialogSelect_Individual.Show();
                    List<Player> playersOnOurTeams = playerManager.FindAllPlayersOnOurTeams(actualPlayer.PlayerId, actualSportId);
                    AutoCompleteTextView autoCompleteTextView = dialogSelect_Individual.FindViewById<AutoCompleteTextView>(Resource.Id.D_SPlayer_playerToFind);
                    var adapterAutoComplete = new PlayerArrayAdapter(this, playersOnOurTeams, actualPlayer.PlayerId, actualSportId, false, null, false);
                    //Rellenamos y creamos el autocompleteView
                    autoCompleteTextView.Adapter = adapterAutoComplete;
                    Button addButton = dialogSelect_Individual.FindViewById<Button>(Resource.Id.D_SPlayer_addButton);
                    addButton.Click += (oadd, eadd) =>
                    {
                        if (finalSelectList.FindIndex(p => p.PlayerNick == autoCompleteTextView.Text) != -1)
                        {
                            IsValid(autoCompleteTextView, "Player: " + autoCompleteTextView.Text + " is already on final select", errorD, false);
                        }
                        else
                            try
                            {
                                int playerId = playerManager.FindPlayerByNick(autoCompleteTextView.Text);
                                SelectedList_Individual.Add(playerManager.GetPlayer(playerId));
                                finalSelectList.Clear();
                                finalSelectList.Add(actualPlayer);
                                finalSelectList.AddRange(SelectedList_Individual);
                                finalSelectList.AddRange(SelectedList_captain);
                                finalArrayAdapter = new PlayerArrayAdapter(
                                                       this, finalSelectList,
                                                       0, actualSportId, false, null, false);
                                finalSelectView.Adapter = finalArrayAdapter;
                                dialogSelect_Individual.Cancel();
                            }
                            catch(PlayerNotFoundException ex)
                            {
                                IsValid(autoCompleteTextView, ex.Message, errorD, false);
                            }
                    };
                    break;
                case Resource.Id.CtxLstProfile:
                    Player player = finalArrayAdapter.GetItem(info.Position);
                    AlertDialog dialogProfile = ShowPlayerDetails(player);
                    TextView commentsLink = dialogProfile.FindViewById<TextView>(Resource.Id.PlayerDetails_ShowCommentsLink);
                    if (feedBackManager.TotalPlayerCommentsCount((int)player.PlayerId) > 0)
                    {
                        commentsLink.SetTextColor(Color.ParseColor("#4785F4"));
                    }
                    else
                    {
                        commentsLink.Click += (o, e) => { };
                    }

                    return true;
                case Resource.Id.CtxLstDelete:
                    finalSelectList.Remove(finalArrayAdapter.GetItem(info.Position));
                    if (SelectedList_Individual.Contains(finalArrayAdapter.GetItem(info.Position)))
                        SelectedList_Individual.Remove(finalArrayAdapter.GetItem(info.Position));
                    finalArrayAdapter = new PlayerArrayAdapter(
                                               this, finalSelectList,
                                               0, actualSportId, false, null, false);
                    finalSelectView.Adapter = finalArrayAdapter;
                    return true;
                default:
                    break;
            }
            return true;
        }
    }

    public class PlaceSpinner : Spinner
    {
        public PlaceSpinner (Context context) : base(context) { }
        public PlaceSpinner(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public PlaceSpinner(Context context, IAttributeSet attrs, int defstyle) : base(context, attrs, defstyle) { }

        public override void SetSelection(int position, bool animate)
        {
            bool sameSelected = position == SelectedItemPosition;
            base.SetSelection(position, animate);
               if (sameSelected)
               {
                   OnItemSelectedListener.OnItemSelected(null, SelectedView, position, SelectedItemId);
               }
        }
        public override void SetSelection(int position)
        {
            bool sameSelected = position == SelectedItemPosition;
            base.SetSelection(position);
            if (sameSelected)
            {
                OnItemSelectedListener.OnItemSelected(null, SelectedView, position, SelectedItemId);
            }
        }
    }

public class MyOnDateChangeListener : Java.Lang.Object, DatePicker.IOnDateChangedListener
    {
        AlertDialog dialogDate;
        TextView selectDate;
        DateTime date;
        public MyOnDateChangeListener() { }
        public MyOnDateChangeListener(AlertDialog dialogDate, TextView selectDate, DateTime date)
        {
            this.dialogDate = dialogDate;
            this.selectDate = selectDate;
            this.date = date;
        }

        public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            date = view.DateTime;
            selectDate.Text = date.Day + "/" + date.Month + "/" + date.Year;
            dialogDate.Cancel();
        }
        public DateTime getDate()
        {
            return this.date;
        }
        public void setDate(DateTime date)
        {
            this.date = date;
        }
    }   
}