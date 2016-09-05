using System;
using System.Collections.Generic;
using System.Text;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.DataAccesLayer.Data.GameRestService;
using Xamarin.Forms;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using System.Linq;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using Plugin.Geolocator;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.GameManager.GameManager))]
namespace AppGeoFit.BusinessLayer.Managers.GameManager
{
    public class GameManager : IGameManager
    {
        IGameRestService gameRestService;
        INoticeRestService noticeRestService;
        IPlayerRestService playerRestService;


        public GameManager(){}

        public IGameManager InitiateServices(bool test)
        {
            gameRestService = DependencyService.Get<IGameRestService>();
            gameRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            noticeRestService = DependencyService.Get<INoticeRestService>();
            noticeRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<IPlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }

        public Game GetGame(int gameId)
        {
            Game game = new Game();
            try
            {
                game = gameRestService.GetGameAsync(gameId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return game;
        }

        public int CreateGame(Game game)
        {
            DateTime timeNow = DateTime.Now;
            //Comprobamos hora valida
            int compareResult = DateTime.Compare(timeNow, game.StartDate);
            int gameIdReturn = 0;
            if (0 < compareResult || game.StartDate.Subtract(timeNow).TotalHours < 1 )
            {
                throw new WrongTimeException("Remember, the game has to start 1 hour after now");
            }
            try
            {
                //TODO comprobacion tambien del equipo.
                int onTimeGameId = gameRestService.FindOnTime(game.CreatorID, game.StartDate.ToString("yyyyMMddHHmmss"), game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                throw new GameOnTimeException("You have an other game in this time");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException){}
                    else
                        throw new Exception(ex.Message);
                }
            }
            //TODO COORDINATES
        /* try
            {
                bool onTimePlace;
                if (game.PlaceID != null)
                    onTimePlace = gameRestService.FindOnTimeAndPlace((int) game.PlaceID, game.StartDate, game.EndDate).Result;
                else
                    onTimePlace = gameRestService.FindOnTimeAndPlace((double)game.Latitude,(double) game.Longitude, game.StartDate, game.EndDate).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameOnTimeAndPlaceException)
                    {
                        throw new GameOnTimeAndPlaceException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }*/
            try
            {
                gameIdReturn = gameRestService.CreateGameAsync(game).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                        throw new Exception(ex.Message);
                }
            }
            //Avisos a los jugadores.
            foreach(Player p in game.Players)
            {
                Notice notice = new Notice();
                notice.MessengerID = game.CreatorID;
                notice.ReceiverID = p.PlayerId;
                notice.SportID = game.SportId;
                if (p.PlayerId != game.CreatorID)
                {
                    notice.Type = Constants.PLAYER_ADD_TO_A_GAME;
                    noticeRestService.CreateNoticeAsync(notice);
                    notice.GameID = gameIdReturn;
                    notice.Type = Constants.FEEDBACK_GAME;
                    noticeRestService.CreateNoticeAsync(notice);
                }
                else
                {
                    notice.GameID = gameIdReturn;
                    notice.Type = Constants.FEEDBACK_GAME;
                    noticeRestService.CreateNoticeAsync(notice);
                }
                
            }            
            return gameIdReturn;
        }

        public bool UpdateGame(Game game)
        {
            DateTime timeNow = DateTime.Now;
            //Comprobamos hora valida
            int compareResult = DateTime.Compare(timeNow, game.StartDate);
            int gameIdReturn = 0;
            bool updateResult = false;
            if (0 < compareResult || game.StartDate.Subtract(timeNow).TotalHours < 1)
            {
                throw new WrongTimeException("Remember, the game has to start 1 hour after now");
            }
            try
            {
                //TODO comprobacion tambien del equipo.
                int onTimeGameId = gameRestService.FindOnTime(game.CreatorID, game.StartDate.ToString("yyyyMMddHHmmss"), game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                if(game.GameID != onTimeGameId)
                {
                    throw new GameOnTimeException("You have an other game in this time");
                }
               
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException) { }
                    else
                        throw new Exception(ex.Message);
                }
            }
            //TODO COORDINATES
            /* try
                {
                    bool onTimePlace;
                    if (game.PlaceID != null)
                        onTimePlace = gameRestService.FindOnTimeAndPlace((int) game.PlaceID, game.StartDate, game.EndDate).Result;
                    else
                        onTimePlace = gameRestService.FindOnTimeAndPlace((double)game.Latitude,(double) game.Longitude, game.StartDate, game.EndDate).Result;
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        if (ex is GameOnTimeAndPlaceException)
                        {
                            throw new GameOnTimeAndPlaceException(ex.Message);
                        }
                        else
                            throw new Exception(ex.Message);
                    }
                }*/
            try
            {
                updateResult = gameRestService.UpdateGameAsync(game).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            //TODO Avisos a los jugadores. GameID
         /*   Notice notice = new Notice();
            foreach (Player p in game.Players)
            {
                if (p.PlayerId != game.CreatorID)
                {
                    notice.MessengerID = game.CreatorID;
                    notice.ReceiverID = p.PlayerId;
                    notice.SportID = game.SportId;
                    notice.Type = Constants.PLAYER_ADD_TO_A_GAME;
                    noticeRestService.CreateNoticeAsync(notice);
                }
            }*/
            return updateResult;
        }

        public bool DeleteGame(int gameId)
        {
            bool isDelete = false;
            try
            {
                isDelete = gameRestService.DeleteGameAsync(gameId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return isDelete;
        }

        public List<Game> GetAllPagination(int pages, int rows, int sportId, string selectedType,double longitude, double latitude)
        {
            List<Game> returnGameList = new List<Game>();
            try
            {
                switch (selectedType)
                {
                    case "distance":
                        returnGameList = gameRestService.GetAllPaginationByDistance(pages, rows, sportId, longitude, latitude).Result.ToList();
                        break;
                    case "time":
                        returnGameList = gameRestService.GetPaginationByTime(pages, rows, sportId).Result.ToList();
                        break;
                    case "numPlayers":
                        returnGameList = gameRestService.GetAllPaginationByNumPlayers(pages, rows, sportId).Result.ToList();
                        break;
                    default:
                        break;
                }
            }catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return returnGameList;
        }

        public int TotalGamesCount(int sportId)
        {
            int numGames = 0;
            try
            {
                numGames = gameRestService.TotalGamesCount( sportId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return numGames;
        }

        public bool AddPlayer(int gameId, int playerId)
        {
            bool playerOnGame = false;
            Game game = new Game();
            bool playerAdd = false;
            int onTimeGameId = 0;
            try
            {
                playerOnGame= gameRestService.IsPlayerOnGame(gameId, playerId).Result;
                game = gameRestService.GetGameAsync(gameId).Result;
                //Comprobar partida misma horario jugador.
                onTimeGameId = gameRestService.FindOnTime(playerId,
                   game.StartDate.ToString("yyyyMMddHHmmss"),
                   game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                if (gameId != onTimeGameId)
                {
                    throw new GameOnTimeException("You have an other game in this time");
                }
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException) { }
                    if(ex is PlayerOnGameException)
                        throw new PlayerOnGameException("You are already joining this game");
                    else {
                        if (ex is Exception)
                            throw new Exception(ex.Message);
                    }
                }
            }
            if (game.PlayersNum + 1 > game.Sport.NumPlayers)
            {
                throw new MaxPlayerOnGameException("This game is full");
            }            
            try
            {
                //TODO ARREGLAR ESTO Y LLAMAR A ADDPLAYERS -> esto no es una peticion get.
               playerAdd = gameRestService.AddPlayer(gameId, playerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }           
            return playerAdd;
        }

        public bool RemovePlayer(int gameId, int playerId)
        {
            Game game = new Game();
            Player player = new Player();
            try
            {
                game = gameRestService.GetGameAsync(gameId).Result;
                player = playerRestService.GetPlayerAsync(playerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException)
                    {
                        throw new PlayerNotFoundException(ex.Message);
                    }
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            bool response = false;
            List<Player> participatePlayers = new List<Player>();
            try
            {
                participatePlayers = gameRestService.GetParticipatePlayers(gameId).Result.ToList();
    
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException)
                    {
                        throw new PlayerNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            Random random = new Random();
            bool creator = false;
            if (participatePlayers.Count == 1)
            {
                gameRestService.DeleteGameAsync(gameId);
            }
            else
            {
                if(player.PlayerId == game.CreatorID)
                {
                    //Si somos el creador de la lista, hay que actualizar el creador
                    // eligiendo a uno aleatoriamente.
                    participatePlayers.Remove(player);
                    int r = random.Next(participatePlayers.Count);
                    game.CreatorID = participatePlayers[r].PlayerId;
                    creator = true;
                }
                try
                {
                    game.Players.Clear();
                    game.Players.Add(player);
                    response = gameRestService.RemovePlayers(game).Result;
                    if (creator)
                    {
                        game.Players.Clear();
                        response = response & gameRestService.UpdateGameAsync(game).Result;
                    }
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            return response;
        }

        public bool IsPlayerOnGame(int gameId, int playerId)
        {
            bool playerOnGame = false;
            try
            {
                playerOnGame = gameRestService.IsPlayerOnGame(gameId, playerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerOnGameException)
                    {
                        throw new PlayerOnGameException("You are already on this game");
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return playerOnGame;
        }

        public bool AddTeam(int gameId, List<Player> playerList, int teamId)
        {
            Game actualGame = new Game();
            try
            {
                actualGame = gameRestService.GetGameAsync(gameId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {                 
                    throw new Exception(ex.Message);
                }
            }
            if (playerList.Count + actualGame.PlayersNum > actualGame.Sport.NumPlayers)
            {
                throw new MaxPlayerOnGameException("Your select is higer than max allow players, " + actualGame.Sport.NumPlayers + ".");
            }
            bool response = false;
            //Update game para vincular el equipo visitante.
            actualGame.Team2ID = teamId;
            try
            {
                gameRestService.UpdateGameAsync(actualGame);
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                actualGame.Players = playerList;
                response = gameRestService.AddPlayers(actualGame).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return response;
        }

        public bool TeamVisitorOnGame(int gameId)
        {
            Game game = new Game();
            try
            {
                game = gameRestService.GetGameAsync(gameId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return game.Team2ID != null;
        }
        public int PlayerGameOnTime(int playerId, Game game)
        {
            int onTimeGameId = 0;
            try
            {
                onTimeGameId = gameRestService.FindOnTime(playerId,
                    game.StartDate.ToString("yyyyMMddHHmmss"),
                    game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                throw new GameOnTimeException("You have an other game in this time");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return onTimeGameId;
        }
        public int TeamGameOnTime(int teamId, Game game)
        {
            int onTimeGameId = 0;
            try
            {
                onTimeGameId = gameRestService.FindTeamOnTime(teamId, 
                    game.StartDate.ToString("yyyyMMddHHmmss"),
                    game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                throw new GameOnTimeException("Your team has other game at this time");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException){
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return onTimeGameId;
        }

        public List<Player> GetParticipatePlayers(int gameId)
        {
            List<Player> listPlayers = new List<Player>();
            try
            {
                listPlayers = gameRestService.GetParticipatePlayers(gameId).Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException)
                    {
                        throw new PlayerNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return listPlayers;
        }

        public List<Place> GetPlaces(int sportId)
        {
            List<Place> placeListBySport = new List<Place>();
            List<Place> placeListWithOutSport = new List<Place>();
            try
            {
                placeListBySport = gameRestService.GetPlacesBySport(sportId).Result.ToList();
               
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlaceNotFoundException){}
                    else
                        throw new Exception(ex.Message);
                }
            }
            try
            {
                placeListWithOutSport = gameRestService.GetPlacesWithOutSport().Result.ToList();               
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlaceNotFoundException)
                    {                         
                        return placeListBySport;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            if (placeListBySport.Count != 0)
            {
                placeListBySport.Concat(placeListWithOutSport);
                return placeListBySport;
            }
            else
                return placeListWithOutSport;
        }

    }
}
