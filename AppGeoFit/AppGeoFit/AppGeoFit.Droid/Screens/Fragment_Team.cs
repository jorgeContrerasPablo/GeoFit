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
using Android.Support.V7.App;
using AppGeoFit.BusinessLayer.Exceptions;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;

namespace AppGeoFit.Droid.Screens
{
    public class Fragment_Team : Fragment
    {
        Player captain = new Player();
        ListView playerList;
        Player actualPlayer = new Player();
        readonly TeamManager teamManager = new TeamManager(false);
        readonly PlayerManager playerManager = new PlayerManager(false);
        List<DataAccesLayer.Models.Team> Teams;
        List<Player> LPlayersOnTeam = new List<Player>();
        DataAccesLayer.Models.Team actualTeam = new DataAccesLayer.Models.Team();
        PlayerArrayAdapter adapterLPlayers;
        View view;
        Drawable errorD;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);
            view = inflater.Inflate(Resource.Layout.Team, container, false);

      
            AppSession appSession = new AppSession(Activity.ApplicationContext);

            ImageButton createTeamB = view.FindViewById<ImageButton>(Resource.Id.Team_createTeamB);
            playerList = view.FindViewById<ListView>(Resource.Id.Team_playerListView);
            //Añadimos el boton addPlayer al final de la lista de jugadores.
            LinearLayout footerView = (LinearLayout)inflater.Inflate(Resource.Layout.buttonFooterPlayerList, null, false);
            playerList.AddFooterView(footerView);

            TextView teamNameT = view.FindViewById<TextView>(Resource.Id.Team_teamName);
            TextView captainNameT = view.FindViewById<TextView>(Resource.Id.Team_captainName);
            ImageButton addPlayerButton = view.FindViewById<ImageButton>(Resource.Id.Team_addPlayerButton);
            ImageButton delteTeamButon = view.FindViewById<ImageButton>(Resource.Id.Team_deleteTeamButton);
            ImageButton editTeamButton = view.FindViewById<ImageButton>(Resource.Id.Team_editTeamButton);
            Spinner spinnerTeams = view.FindViewById<Spinner>(Resource.Id.Team_spinnerTeams);
            ImageView colorView = view.FindViewById<ImageView>(Resource.Id.Team_imageColor);

            //Se crea el icono exclamation_error
            errorD = ContextCompat.GetDrawable(Context, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            Spinner spinnerFavoriteSport_et = this.Activity.FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);

            actualPlayer = appSession.getPlayer();

            //Actualizamos el spinner al principio.
            UpdateTeams(view);          

            spinnerFavoriteSport_et.ItemSelected += (o, e) =>
            {
                UpdateTeams(view);
            };
            
            spinnerTeams.ItemSelected += (oT, eT) =>
            {              
                LPlayersOnTeam.Clear();
                teamNameT.Text = spinnerTeams.GetItemAtPosition(eT.Position).ToString();
                actualTeam = Teams.ElementAt(Teams.FindIndex(t => t.TeamName == spinnerTeams.SelectedItem.ToString()));
                colorView.SetBackgroundColor(Android.Graphics.Color.ParseColor(actualTeam.ColorTeam));
                captain = actualTeam.Joineds.ElementAt(actualTeam.Joineds.ToList().
                                FindIndex(j => (j.Captain) && (j.TeamID == spinnerTeams.SelectedItem.GetHashCode()))).Player;
                captainNameT.Text = captain.PlayerNick;
                if (captain != null)
                {
                    if (captain.PlayerId != actualPlayer.PlayerId)
                    {
                        addPlayerButton.Visibility = ViewStates.Invisible;
                        delteTeamButon.Visibility = ViewStates.Invisible;
                        editTeamButton.Visibility = ViewStates.Invisible;
                    }
                    else {
                        addPlayerButton.Visibility = ViewStates.Visible;
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
            delteTeamButon.Click += (o, e) =>
             {
                 //TODO MENSAJE DE ASEGURAR
                 teamManager.DeleteTeam(actualTeam.TeamID);
                 Toast.MakeText(Context, "Team: " + actualTeam.TeamName +
                    " has been deleted correctly", ToastLength.Long).Show();
                 UpdateTeams(view);
             };

            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            string playerNickSelected = string.Empty;
            addPlayerButton.Click += (o, e) =>
            {               
                View dialogView = inflater.Inflate(Resource.Layout.dialog_SearchPlayer, null);
                builder.SetView(dialogView);

                AutoCompleteTextView AutocompleteView = dialogView.FindViewById<AutoCompleteTextView>(Resource.Id.D_SPlayer_playerToFind);
                var adapterAutoComplete = new PlayerArrayAdapter(Context, playerManager.GetAll().Result.ToList());

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
                        teamManager.AddPlayer(playerNickSelected, actualTeam);
                        Toast.MakeText(Context, "Player: "+ playerNickSelected + " has been added correctly", ToastLength.Long).Show();
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
            LPlayersOnTeam.Clear();
            Spinner spinnerTeams = view.FindViewById<Spinner>(Resource.Id.Team_spinnerTeams);
            var n = 0;
            actualTeam = Teams.ElementAt(Teams.FindIndex(t => t.TeamName == spinnerTeams.SelectedItem.ToString()));
            var updateTeam = teamManager.GetTeam(actualTeam.TeamID).Result;
            while (n < updateTeam.Joineds.Count)
            {
                LPlayersOnTeam.Add(updateTeam.Joineds.ElementAt(n).Player);
                n++;
            }
            adapterLPlayers = new PlayerArrayAdapter(
            view.Context, LPlayersOnTeam);
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

            Teams = playerManager.
               FindTeamsJoined(actualPlayer.PlayerId,
               spinnerFavoriteSport_et.SelectedItem
               .GetHashCode()).Result.ToList();
            if (Teams.Count == 0)
            {
                teamNameT.Text = "";
                captainNameT.Text = "";
                addPlayerButton.Visibility = ViewStates.Invisible;
                delteTeamButon.Visibility = ViewStates.Invisible;
                editTeamButton.Visibility = ViewStates.Invisible;
                spinnerTeams.Visibility = ViewStates.Invisible;
                captain = null;
                playerList.Visibility = ViewStates.Invisible;
            }
            else
            {
                spinnerTeams.Visibility = ViewStates.Visible;
                addPlayerButton.Visibility = ViewStates.Visible;
                delteTeamButon.Visibility = ViewStates.Visible;
                editTeamButton.Visibility = ViewStates.Visible;
                spinnerTeams.Visibility = ViewStates.Visible;
                playerList.Visibility = ViewStates.Visible;
            }
            var adapterTeams = new ArrayAdapter<DataAccesLayer.Models.Team>(
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

            //Inflamos un menu con delete solo si el jugador es capitan, o sea el mismo.
            if (captain.PlayerId != actualPlayer.PlayerId)
            {
                if(actualPlayer.PlayerNick.Equals(playerList.Adapter.GetItem(info.Position).ToString()))
                    inflater.Inflate(Resource.Menu.MenuPlayerList, menu);
                else
                    inflater.Inflate(Resource.Menu.MenuPlayerListNoC, menu);
            }  
            else
                inflater.Inflate(Resource.Menu.MenuPlayerList, menu);
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
                            UpdatePlayersList(view);
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
                                teamManager.DeleteTeam(actualTeam.TeamID);
                                UpdateTeams(view);
                                aDialog_onePlayer.Cancel();                                
                                Toast.MakeText(this.Context, "Your team has been delete correctly", ToastLength.Long).Show();
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
                                var adapterAutoComplete = new PlayerArrayAdapter(Context, lplayersSelectCaptain);

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
                                        catch (AlreadyCaptainOnSport e)
                                        {
                                            IsValid(AutocompleteView, e.Message, errorD, false);
                                            // Toast.MakeText(this.Context, "SelectOtherCaptain", ToastLength.Long).Show();
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
                    dialogView = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.PlayerProfile, null);
                    dialogView.FindViewById<ImageButton>(Resource.Id.imageButtonEdit).Visibility = ViewStates.Invisible;
                    dialogView.FindViewById<ImageButton>(Resource.Id.imageButtonDelete).Visibility = ViewStates.Invisible;
                    dialogView.FindViewById<TextView>(Resource.Id.LastName).Visibility = ViewStates.Invisible;
                    dialogView.FindViewById<TextView>(Resource.Id.PhoneNumber).Visibility = ViewStates.Invisible;
                    dialogView.FindViewById<TextView>(Resource.Id.LabelLastName).Visibility = ViewStates.Invisible;
                    dialogView.FindViewById<TextView>(Resource.Id.LabelPhoneNumber).Visibility = ViewStates.Invisible;
                    builder.SetView(dialogView);
                    ad = builder.Create();
                    ad.Show();
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }
    }

    public class PlayerArrayAdapter : ArrayAdapter<Player>
    {

        public PlayerArrayAdapter(Context context, List<Player> objects): base(context, 0, objects){}

        
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
                //Si no existe, entonces inflarlo con two_line_list_item.xml
                listItemView = inflater.Inflate(
                        Resource.Layout.ElementPlayerList,
                        parent,
                        false);
            }
            //Obteniendo instancias de los text views
            TextView Nick = listItemView.FindViewById<TextView>(Resource.Id.ElementPlayerList_Nick);
            RatingBar RatingBar = (RatingBar)listItemView.FindViewById(Resource.Id.ElementPlayerList_RatingBar);

            //Obteniendo instancia de la Tarea en la posición actual
            Player item = GetItem(position);

            Nick.Text = item.PlayerNick;
            RatingBar.Rating =(int)item.Level;

            //Devolver al ListView la fila creada
            return listItemView;
        }
    }
}