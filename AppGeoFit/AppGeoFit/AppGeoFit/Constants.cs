using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit
{
    public static class Constants
    {
        // URL of REST service
        public static string RestUrl { get; set; } = "http://192.168.1.34:51830/api/";
        public static string RestUrlTest { get; set; } = "http://192.168.1.34:51830/apiTest/";

        //Type of Notice
        public const string TEAM_ADD_PLAYER = "Team add player";
        public const string PLAYER_ADD_TO_A_GAME = "Player add to a game";

        //public readonly static string RestUrl = "http://10.0.2.2:51830/api/";
        // Credentials that are hard coded into the REST service
        //TODO
        // public static string Username = "Xamarin";
        // public static string Password = "Pa$$w0rd";

    }
}
