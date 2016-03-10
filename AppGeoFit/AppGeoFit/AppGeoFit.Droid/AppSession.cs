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

namespace AppGeoFit.Droid
{
    class AppSession
    {
        private ISharedPreferences appSharedPrefs;
        private ISharedPreferencesEditor appSharedPrefsEditor;
        //private Context context;

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
            String playerJson = appSharedPrefs.GetString("Player", "");
            return JsonConvert.DeserializeObject<Player>(playerJson);
        }
    }
}