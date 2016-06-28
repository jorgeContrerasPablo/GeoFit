using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace RestServiceGeoFit.Models2
{
    /*Clase necesaria por culpa de la clase DbGeography,
    la cual no podemos añadir a nuestra capa de negocio en la app
    impidiendo su match con json y el servicio.*/
    public class GameLatitudeLongitude
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GameLatitudeLongitude()
        {
            FeedBacks = new HashSet<FeedBack>();
            Players = new HashSet<Player>();
        }

        public int GameID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PlayersNum { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public int? Team1ID { get; set; }

        public int? Team2ID { get; set; }

        public int? PlaceID { get; set; }
        
        public int CreatorID { get; set; }

        public int SportId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        public virtual Place Place { get; set; }

        public virtual Team Team { get; set; }

        public virtual Team Team1 { get; set; }

        public virtual Player Creator { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Player> Players { get; set; }

        public Game GameLlToGame(GameLatitudeLongitude gameLL)
        {
            Game returnGame = new Game();
            if (!(gameLL.Longitude == null || gameLL.Latitude == null))
            {
                string pointwkt = String.Format("POINT({0} {1})", gameLL.Longitude, gameLL.Latitude);
                returnGame.Coordinates = DbGeography.PointFromText(pointwkt, 4326);
            }
            else
                returnGame.Coordinates = null;
            returnGame.Creator = gameLL.Creator;
            returnGame.CreatorID = gameLL.CreatorID;
            returnGame.EndDate = gameLL.EndDate;
            returnGame.FeedBacks = gameLL.FeedBacks;
            returnGame.GameID = gameLL.GameID;
            returnGame.Place = gameLL.Place;
            returnGame.PlaceID = gameLL.PlaceID;
            returnGame.Players = gameLL.Players;
            returnGame.PlayersNum = gameLL.PlayersNum;
            returnGame.Sport = gameLL.Sport;
            returnGame.SportId = gameLL.SportId;
            returnGame.StartDate = gameLL.StartDate;
            returnGame.Team = gameLL.Team;
            returnGame.Team1 = gameLL.Team1;
            returnGame.Team1ID = gameLL.Team1ID;
            returnGame.Team2ID = gameLL.Team2ID;
            
 
            return returnGame;
        }

        public GameLatitudeLongitude GameToGameLl(Game game)
        {
            GameLatitudeLongitude returnGame = new GameLatitudeLongitude();
            returnGame.Longitude = game.Coordinates.Longitude;
            returnGame.Latitude = game.Coordinates.Latitude;
            returnGame.Creator = game.Creator;
            returnGame.CreatorID = game.CreatorID;
            returnGame.EndDate = game.EndDate;
            returnGame.FeedBacks = game.FeedBacks;
            returnGame.GameID = game.GameID;
            returnGame.Place = game.Place;
            returnGame.PlaceID = game.PlaceID;
            returnGame.Players = game.Players;
            returnGame.PlayersNum = game.PlayersNum;
            returnGame.Sport = game.Sport;
            returnGame.SportId = game.SportId;
            returnGame.StartDate = game.StartDate;
            returnGame.Team = game.Team;
            returnGame.Team1 = game.Team1;
            returnGame.Team1ID = game.Team1ID;
            returnGame.Team2ID = game.Team2ID;

            return returnGame;
        }
    }
}