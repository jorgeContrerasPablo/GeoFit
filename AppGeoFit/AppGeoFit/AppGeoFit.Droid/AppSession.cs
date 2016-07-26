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
using Android.Preferences;
using Newtonsoft.Json;
using AppGeoFit.DataAccesLayer.Models;

namespace AppGeoFit.Droid
{
    public class AppSession
    {
        private ISharedPreferences appSharedPrefs;
        private ISharedPreferencesEditor appSharedPrefsEditor;

        public AppSession(Context context)
        {
            appSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(context);
            appSharedPrefsEditor = appSharedPrefs.Edit();

        }

        public void setPlayer(Player player)
        {
            var playerJson = JsonConvert.SerializeObject(player);
            appSharedPrefsEditor.PutString("Player", playerJson);
            appSharedPrefsEditor.Commit();
        }

        public Player getPlayer()
        {
            string playerJson = appSharedPrefs.GetString("Player", "");
            if (playerJson == "")
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Player>(playerJson);
        }

        public void deletePlayer()
        {
            appSharedPrefsEditor.Remove("Player");
            appSharedPrefsEditor.Commit();
        }

        public void setSports(List<Sport> listSports)
        {
            var listSportJson = JsonConvert.SerializeObject(listSports);
            appSharedPrefsEditor.PutString("Sports", listSportJson);
            appSharedPrefsEditor.Commit();
        }

        public List<Sport> getSports()
        {
            string listSportJson = appSharedPrefs.GetString("Sports", "");
            if (listSportJson == "")
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<Sport>>(listSportJson);
        }

        public void deleteSports()
        {
            appSharedPrefsEditor.Remove("Sports");
            appSharedPrefsEditor.Commit();
        }

        public void updateSession(bool state)
        {
            Player playerToUpdate = this.getPlayer();
            playerToUpdate.PlayerSesion = state;
            this.setPlayer(playerToUpdate);
        }
    }
}