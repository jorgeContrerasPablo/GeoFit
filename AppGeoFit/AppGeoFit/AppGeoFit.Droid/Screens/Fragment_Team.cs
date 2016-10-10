using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppGeoFit.DataAccesLayer.Models;
using Android.Support.V7.App;
using AppGeoFit.BusinessLayer.Exceptions;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.BusinessLayer.Managers.NoticeManager;
using AppGeoFit.Droid.Adapters;
using Android.Graphics;
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;

namespace AppGeoFit.Droid.Screens
{
    public class Fragment_Team : Fragment
    {
        Player captain = new Player();
        ListView playerList;
        Player actualPlayer = new Player();
        ITeamManager teamManager;
        IPlayerManager playerManager;
        INoticeManager noticeManager;
        IFeedBackManager feedBackManager;
        List<Team> Teams = new List<Team>();
        List<Player> LPlayersOnTeam = new List<Player>();
        Team actualTeam = new Team();
        int actualSportId;
        PlayerArrayAdapter adapterLPlayers;
        View view;
        Drawable errorD;
        LinearLayout footerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);
            view = inflater.Inflate(Resource.Layout.Team, container, false);

            FragmentActivity_MainActivity myActivity = (FragmentActivity_MainActivity)Activity;
            playerManager = myActivity.playerManager;
            teamManager = myActivity.teamManager;
            noticeManager = myActivity.noticeManager;
            feedBackManager = myActivity.feedBackManager;
            AppSession appSession = new AppSession(Activity.ApplicationContext);

            ImageButton createTeamB = view.FindViewById<ImageButton>(Resource.Id.Team_createTeamB);
            playerList = view.FindViewById<ListView>(Resource.Id.Team_playerListView);
            //Añadimos el boton addPlayer al final de la lista de jugadores.
            LinearLayout footerView = (LinearLayout)inflater.Inflate(Resource.Layout.buttonFooterPlayerList, null, false);
            playerList.AddFooterView(footerView);
            //Recuperamos los elementos de la vista(layouts)
            TextView teamNameT = view.FindViewById<TextView>(Resource.Id.Team_teamName);
            TextView captainNameT = view.FindViewById<TextView>(Resource.Id.Team_captainName);
            ImageButton addPlayerButton = view.FindViewById<ImageButton>(Resource.Id.Team_addPlayerButton);
            ImageButton delteTeamButon = view.FindViewById<ImageButton>(Resource.Id.Team_deleteTeamButton);
            ImageButton editTeamButton = view.FindViewById<ImageButton>(Resource.Id.Team_editTeamButton);
            Spinner spinnerTeams = view.FindViewById<Spinner>(Resource.Id.Team_spinnerTeams);
            ImageView colorView = view.FindViewById<ImageView>(Resource.Id.Team_imageColor);
            Spinner spinnerFavoriteSport_et = this.Activity.FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);

            //Se crea el icono exclamation_error.
            errorD = ContextCompat.GetDrawable(Context, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);            
            //Jugador de la session.
            actualPlayer = appSession.getPlayer();
            //Actualizamos el spinnerTeams al principio.
            UpdateTeams(view);
            //Recuparamos el deporte actual
            actualSportId = spinnerFavoriteSport_et.SelectedItem.GetHashCode();
            //Recuperamos el equipo actual
            if (Teams.Count != 0)
                actualTeam = Teams.ElementAt( Teams.FindIndex(t => t.TeamName == spinnerTeams.SelectedItem.ToString()));

            //Cada vez que cambiemos de deporte, deberemos actualizar los equipos.
            spinnerFavoriteSport_et.ItemSelected += (o, e) =>
            {
                actualSportId = spinnerFavoriteSport_et.GetItemAtPosition(e.Position).GetHashCode();
                appSession.setSelectedSport(actualSportId);
                UpdateTeams(view);
            };
            
            spinnerTeams.ItemSelected += (oT, eT) =>
            {              
                LPlayersOnTeam.Clear();
                teamNameT.Text = spinnerTeams.GetItemAtPosition(eT.Position).ToString();
                actualTeam = Teams.ElementAt(Teams.FindIndex(t => t.TeamName == spinnerTeams.SelectedItem.ToString()));
                colorView.SetBackgroundColor(Android.Graphics.Color.ParseColor(actualTeam.ColorTeam));
                var positionCaptainOnList = actualTeam.Joineds.ToList().FindIndex(j => (j.Captain) && (j.TeamID == spinnerTeams.SelectedItem.GetHashCode()));
                captain = positionCaptainOnList == -1? null : actualTeam.Joineds.ElementAt(positionCaptainOnList).Player;
                captainNameT.Text = captain == null ? "" : captain.PlayerNick;
                //Mostraremos los botones editar, borrar y añadir a equipo, según seamos capitán o no.
                if (captain != null)
                {
                    if (captain.PlayerId != actualPlayer.PlayerId)
                    {
                        playerList.RemoveFooterView(footerView);
                        delteTeamButon.Visibility = ViewStates.Invisible;
                        editTeamButton.Visibility = ViewStates.Invisible;
                    }
                    else {
                        playerList.AddFooterView(footerView);
                        delteTeamButon.Visibility = ViewStates.Visible;
                        editTeamButton.Visibility = ViewStates.Visible;
                    }
                }
                UpdatePlayersList(view);
            };

            editTeamButton.Click += (o, e) =>
            {
                 var mainActivity = new Intent(Context, typeof(Screen_EditTeam));
                 mainActivity.PutExtra("teamId", actualTeam.TeamID);
                 Activity.StartActivity(mainActivity);
            };
            createTeamB.Click += (o, e) => Activity.StartActivity(typeof(Screen_CreateTeam));
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            delteTeamButon.Click += (o, e) =>
             {
                 baDelete = BotonAlert("Alert", "Do you want to delete this team?", "OK", "Cancel", Context);
                 baDelete.Show();
                 baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                 baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                 baDeletePositiveButton.Click += (oPB, ePB) =>
                 {
                     bool isDelete = false;
                     try
                     {
                         isDelete = teamManager.DeleteTeam(actualTeam.TeamID);
                     }
                     catch (Exception ex)
                     {
                         Toast.MakeText(Activity.ApplicationContext,
                             ex.Message, ToastLength.Short).Show();
                     }
                     if (isDelete)
                     {
                         Toast.MakeText(Context, "Team: " + actualTeam.TeamName +
                                        " has been deleted correctly", ToastLength.Long).Show();
                         UpdateTeams(view);                         
                     }
                     baDelete.Cancel();
                 };
                 baDeleteNegativeButton.Click += (oNB, eNB) =>
                 {
                     baDelete.Cancel();
                 };
             };

            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            string playerNickSelected;
            addPlayerButton.Click += (o, e) =>
            {
                playerNickSelected = string.Empty;
                View dialogView = inflater.Inflate(Resource.Layout.dialog_SearchPlayer, null);
                builder.SetView(dialogView);

                AutoCompleteTextView AutocompleteView = dialogView.FindViewById<AutoCompleteTextView>(Resource.Id.D_SPlayer_playerToFind);
                List<Player> playerList = new List<Player>();
                try
                {
                    playerList = playerManager.GetAll().ToList();
                }
                catch (PlayerNotFoundException){}
                catch (Exception ex)
                {
                    Toast.MakeText(Activity.ApplicationContext,
                             ex.Message, ToastLength.Short).Show();
                }
                var adapterAutoComplete = new PlayerArrayAdapter(Context, playerList, captain.PlayerId, actualSportId, false, null, true);

                AutocompleteView.Adapter = adapterAutoComplete;
                AlertDialog ad = builder.Create();
                ad.Show();
                Button addButton = dialogView.FindViewById<Button>(Resource.Id.D_SPlayer_addButton);
                Button cancelButton = dialogView.FindViewById<Button>(Resource.Id.D_SPlayer_cancelButton);
                
                AutocompleteView.AfterTextChanged += (so, se) =>
                {
                    playerNickSelected = AutocompleteView.Text;
                };

                addButton.Click += (oadd, eadd) => 
                {
                    try
                    {
                        teamManager.SendNoticeAddPlayer(playerNickSelected, actualTeam);
                        Toast.MakeText(Context, "Player: "+ playerNickSelected + " has recived a petition", ToastLength.Long).Show();
                        ad.Cancel();
                        UpdatePlayersList(view);
                    }
                    catch (MaxPlayerOnTeamException ex)
                    {
                        Toast.MakeText(Context, ex.Message, ToastLength.Long).Show();
                    }
                    catch (DuplicatePlayerOnTeamException ex)
                    {
                        IsValid(AutocompleteView, ex.Message, errorD, false);
                    }
                    catch (PlayerNotFoundException ex)
                    {
                        IsValid(AutocompleteView, ex.Message, errorD, false);
                    }
                    catch (DuplicateNoticeException ex)
                    {
                        IsValid(AutocompleteView, ex.Message, errorD, false);
                    }
                    catch (Exception ex)
                    {
                        BotonAlert("Alert", ex.Message, "OK", "Cancel", Context).Show();
                    }
                };
                cancelButton.Click += (oc, ec) =>
                {
                    ad.Cancel();
                };
            };

            return view;
        }
        private void UpdatePlayersList(View view)
        {
            Spinner spinnerTeams = view.FindViewById<Spinner>(Resource.Id.Team_spinnerTeams);
            List<Player> LPlayersPending = new List<Player>();
            try
            {
                LPlayersPending = teamManager.GetAllPlayersPendingToAdd(captain.PlayerId, actualSportId, Constants.TEAM_ADD_PLAYER).ToList();
            }
            catch (NotPendingPlayersToAddException)
            {
                LPlayersPending.Clear();
            }
            LPlayersOnTeam.Clear();            
            //Recuperamos el equipo actual seleccionado
            actualTeam = Teams.ElementAt(Teams.FindIndex(t => t.TeamName == spinnerTeams.SelectedItem.ToString()));
            //Recuperamos el equipo de base de datos por si ha sufrido alguna alta o baja.
            Team updateTeam = new Team();
            try {
                updateTeam = teamManager.GetTeam(actualTeam.TeamID); }
            catch(TeamNotFoundException ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Short).Show();
            }
            catch(Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Short).Show();
            }
            var n = 0;
            while (n < updateTeam.Joineds.Count)
            {
                LPlayersOnTeam.Add(updateTeam.Joineds.ElementAt(n).Player);
                n++;
            }
            if(LPlayersPending.Count != 0 && actualPlayer.PlayerId == captain.PlayerId)
            {
                LPlayersOnTeam.AddRange(LPlayersPending);
            }            
            adapterLPlayers = new PlayerArrayAdapter(
            view.Context, LPlayersOnTeam,
            captain.PlayerId, actualSportId, false, null, true);
            playerList.Adapter = adapterLPlayers;
            RegisterForContextMenu(playerList);
        }

        private void UpdateTeams(View view)
        {          
            playerList = view.FindViewById<ListView>(Resource.Id.Team_playerListView);
            TextView teamNameT = view.FindViewById<TextView>(Resource.Id.Team_teamName);
            TextView captainNameT = view.FindViewById<TextView>(Resource.Id.Team_captainName);
            ImageButton addPlayerButton = view.FindViewById<ImageButton>(Resource.Id.Team_addPlayerButton);
            ImageButton delteTeamButon = view.FindViewById<ImageButton>(Resource.Id.Team_deleteTeamButton);
            ImageButton editTeamButton = view.FindViewById<ImageButton>(Resource.Id.Team_editTeamButton);
            Spinner spinnerTeams = view.FindViewById<Spinner>(Resource.Id.Team_spinnerTeams);
            Spinner spinnerFavoriteSport_et = this.Activity.FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);
            ImageView imageColor = view.FindViewById<ImageView>(Resource.Id.Team_imageColor);

            try
            {
                Teams = playerManager.FindTeamsJoined(actualPlayer.PlayerId,spinnerFavoriteSport_et.SelectedItem
                                                      .GetHashCode()).ToList();
            }
            catch(NotTeamJoinedOnSportException){}
            catch(Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Short).Show();
            }
                       
            if (Teams.Count == 0)
            {
                teamNameT.Text = "";
                captainNameT.Text = "";
                delteTeamButon.Visibility = ViewStates.Invisible;
                editTeamButton.Visibility = ViewStates.Invisible;
                spinnerTeams.Visibility = ViewStates.Invisible;
                captain = null;
                playerList.Visibility = ViewStates.Invisible;
                imageColor.Visibility = ViewStates.Invisible;
            }
            else
            {
                spinnerTeams.Visibility = ViewStates.Visible;
                delteTeamButon.Visibility = ViewStates.Visible;
                editTeamButton.Visibility = ViewStates.Visible;
                spinnerTeams.Visibility = ViewStates.Visible;
                playerList.Visibility = ViewStates.Visible;
                imageColor.Visibility = ViewStates.Visible;
            }
            var adapterTeams = new ArrayAdapter<Team>(
                view.Context, Android.Resource.Layout.SimpleSpinnerItem, Teams);
            adapterTeams.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerTeams.Adapter = adapterTeams;
        }

        public bool IsValid(EditText editText, string message, Drawable error, bool match)
        {
            if (!match)
            {
                editText.SetError(message, error);
                return false;
            }
            else {
                editText.SetError(String.Empty, null);
                editText.Error = null;
                return true;
            }
        }
        public AlertDialog BotonAlert(string title, string message, string positiveButton, string negativeButton, Context cntx)
        {
            // BOTON ALERT
            AlertDialog.Builder builder = new AlertDialog.Builder(cntx);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButton, (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton(negativeButton, (EventHandler<DialogClickEventArgs>)null);

            return builder.Create();
        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            var info = (AdapterView.AdapterContextMenuInfo) menuInfo;
            MenuInflater inflater = new MenuInflater(Context);

            menu.SetHeaderTitle(
                playerList.Adapter.GetItem(info.Position).ToString());

            int idplayer = playerList.Adapter.GetItem(info.Position).GetHashCode();
            //Solo inflamos un menu, si no estan pendientes
            try
            {
                if (noticeManager.NoticeIsPending(idplayer, captain.PlayerId, actualSportId, Constants.TEAM_ADD_PLAYER)) { }
                else
                {
                    //Inflamos un menu con delete solo si el jugador es capitan, o sea el mismo.
                    if (captain.PlayerId != actualPlayer.PlayerId)
                    {
                        if (actualPlayer.PlayerNick.Equals(playerList.Adapter.GetItem(info.Position).ToString()))
                            inflater.Inflate(Resource.Menu.MenuPlayerList, menu);
                        else
                            inflater.Inflate(Resource.Menu.MenuPlayerListNoC, menu);
                    }
                    else
                        inflater.Inflate(Resource.Menu.MenuPlayerList, menu);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
            }         
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            AlertDialog baDelete;
            Button baDeletePositiveButton;
            Button baDeleteNegativeButton;
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            Player player = adapterLPlayers.GetItem(info.Position);
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            View dialogView;
            AlertDialog ad;
            switch (item.ItemId)
            {
                case Resource.Id.CtxLstDelete:
                    baDelete = BotonAlert("Alert", "Do you want to remove this player?", "OK", "Cancel", Context);
                    baDelete.Show();
                    baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                    baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                    baDeletePositiveButton.Click += (oDb, eDb) =>
                    {
                        try
                        {
                            teamManager.RemovePlayer(actualTeam.TeamID, player.PlayerId);
                            UpdateTeams(view);
                            baDelete.Cancel();
                            Toast.MakeText(this.Context, "Player: "+player.PlayerNick+" has been delete correctly", ToastLength.Long).Show();
                        }
                        catch (OnlyOnePlayerException ex)
                        {
                            baDelete.Cancel();
                            AlertDialog aDialog_onePlayer = BotonAlert("Alert", ex.Message, "OK", "Cancel", Context);
                            aDialog_onePlayer.Show();
                            Button aDialog_onePlayerPositiveButton = aDialog_onePlayer.GetButton((int)DialogButtonType.Positive);
                            aDialog_onePlayerPositiveButton.Click += (oDOP, eDOP) =>
                            {
                                try
                                {
                                    teamManager.DeleteTeam(actualTeam.TeamID);
                                    UpdateTeams(view);
                                    aDialog_onePlayer.Cancel();
                                    Toast.MakeText(this.Context, "Your team has been delete correctly", ToastLength.Long).Show();
                                }
                                catch (Exception)
                                {
                                    Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
                                }
                               
                            };
                        }
                        catch (CaptainRemoveException ex)
                        {
                            baDelete.Cancel();
                            AlertDialog aDialog_CaptainRemove = BotonAlert("Alert", ex.Message, "OK", "Cancel", Context);
                            aDialog_CaptainRemove.Show();
                            Button aDialog_CaptainRemovePositiveButton = aDialog_CaptainRemove.GetButton((int)DialogButtonType.Positive);                            
                            string playerNickSelected = string.Empty;
                            aDialog_CaptainRemovePositiveButton.Click += (oDOP, eDOP) =>
                            {
                                dialogView = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.dialog_SearchPlayer, null);
                                builder.SetView(dialogView);
                                AutoCompleteTextView AutocompleteView = dialogView.FindViewById<AutoCompleteTextView>(Resource.Id.D_SPlayer_playerToFind);
                                List<Player> lplayersSelectCaptain = LPlayersOnTeam;

                                //Eliminamos el capitan actual de la lista
                                lplayersSelectCaptain.RemoveAt(LPlayersOnTeam.FindIndex(p => p.PlayerNick == captain.PlayerNick));
                                var adapterAutoComplete = new PlayerArrayAdapter(Context, lplayersSelectCaptain, captain.PlayerId, actualSportId, false, null, true);
                                //Rellenamos y creamos el autocompleteView
                                AutocompleteView.Adapter = adapterAutoComplete;
                                ad = builder.Create();
                                ad.Show();
                                Button addButton = dialogView.FindViewById<Button>(Resource.Id.D_SPlayer_addButton);

                                Player playerSelected = new Player();
                                bool invalidPlayer = false;
                                AutocompleteView.AfterTextChanged += (so, se) =>
                                {
                                    invalidPlayer = false;
                                    playerNickSelected = AutocompleteView.Text;                     
                                    int position = lplayersSelectCaptain.FindIndex(p => p.PlayerNick == playerNickSelected);
                                    if (position != -1)
                                        playerSelected = lplayersSelectCaptain.ElementAt(position);
                                    else
                                        invalidPlayer = true;
                                        
                                };

                                addButton.Click += (oadd, eadd) =>
                                {
                                    aDialog_CaptainRemove.Cancel();
                                    if (invalidPlayer)
                                    {
                                        IsValid(AutocompleteView, "Player: "+playerNickSelected+" don't exist on this team", errorD, false);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            captain = teamManager.UpdateCaptain(captain.PlayerId, playerSelected.PlayerId, actualTeam.TeamID);
                                            Toast.MakeText(Context, "Player: " + playerNickSelected + " is the new captain of : " + actualTeam.TeamName + ".", ToastLength.Long).Show();
                                            teamManager.RemovePlayer(actualTeam.TeamID, player.PlayerId);
                                            ad.Cancel();
                                            UpdateTeams(view);
                                        }
                                        catch (AlreadyCaptainOnSportException e)
                                        {
                                            IsValid(AutocompleteView, e.Message, errorD, false);
                                        }
                                        catch (Exception e)
                                        {
                                            BotonAlert("Alert", e.Message, "OK", "Cancel", Context).Show();
                                        }
                                    }
                                };                                
                            };
                        }
                        catch (NotJoinedException ex)
                        {
                            Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
                        }
                    };
                    return true;
                case Resource.Id.CtxLstProfile:                
                    dialogView = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.PlayerDetails, null);                   
                    dialogView.FindViewById<TextView>(Resource.Id.PlayerDetails_Name).Text = player.PlayerName;
                    dialogView.FindViewById<TextView>(Resource.Id.PlayerDetails_Nick).Text = player.PlayerNick;
                    dialogView.FindViewById<RatingBar>(Resource.Id.PlayerDetails_ratingBar).Rating = (int)player.Level;
                    dialogView.FindViewById<TextView>(Resource.Id.PlayerDetails_MedOnTime).Text = string.Format("{0:P2}", player.MedOnTime);
                    dialogView.FindViewById<TextView>(Resource.Id.PlayerDetails_Email).Text = player.PlayerMail;
                    TextView commentsLink = dialogView.FindViewById<TextView>(Resource.Id.PlayerDetails_ShowCommentsLink);
                    try
                    {
                        if (feedBackManager.TotalPlayerCommentsCount((int)player.PlayerId) > 0)
                        {
                            commentsLink.SetTextColor(Color.ParseColor("#4785F4"));
                            commentsLink.Click += (o, e) =>
                            {
                                var screen_Comments = new Intent(Context, typeof(Screen_Comments));
                                screen_Comments.PutExtra("playerId", player.PlayerId);
                                StartActivity(screen_Comments);
                            };
                        }
                        builder.SetView(dialogView);
                        ad = builder.Create();
                        ad.Show();
                    }
                    catch(Exception ex)
                    {
                        Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
                    }                   
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }
    }    
}