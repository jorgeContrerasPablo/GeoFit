using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit
{
    public static class Constants
    {
        // URL of REST service
       //  public static string RestUrl { get; set; } = "http://192.168.43.71:51830/api/";
       //  public static string RestUrlTest { get; set; } = "http://192.168.43.71:51830/apiTest/";

       // public static string RestUrl { get; set; } = "http://192.168.0.4:51830/api/";
       // public static string RestUrlTest { get; set; } = "http://192.168.0.4:51830/apiTest/";

        public static string RestUrl { get; set; } = "http://10.20.38.201:51830/api/";
        public static string RestUrlTest { get; set; } = "http://10.20.38.201:51830/apiTest/";



        //Type of Notice
        public const string TEAM_ADD_PLAYER = "Team add player";
        public const string PLAYER_ADD_TO_A_GAME = "Player add to a game";
        public const string FEEDBACK_GAME = "You can comment a game";
        public const string GAME_DELETED = "Game has been deleted";
        public const string GAME_UPDATED = "Game has been updated";
        public const string TEAM_DELETED = "Your team has been deleted";

        //public readonly static string RestUrl = "http://10.0.2.2:51830/api/";
        // Credentials that are hard coded into the REST service

         public static string Username = "GeoFit";
         public static string Password = "1234";

    }
}
