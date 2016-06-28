using System;
using System.Collections.Generic;
using System.Text;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.DataAccesLayer.Data.GameRestService;
using Xamarin.Forms;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.GameManager.GameManager))]
namespace AppGeoFit.BusinessLayer.Managers.GameManager
{
    public class GameManager : IGameManager
    {
        IGameRestService gameRestService;
        INoticeRestService noticeRestService;

        public GameManager(){}

        public IGameManager InitiateServices(bool test)
        {
            gameRestService = DependencyService.Get<IGameRestService>();
            gameRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            noticeRestService = DependencyService.Get<INoticeRestService>();
            noticeRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
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
                bool onTime = gameRestService.FindOnTime(game.CreatorID, game.StartDate.ToString("yyyyMMddHHmmss"), game.EndDate.ToString("yyyyMMddHHmmss")).Result;
                throw new GameOnTimeException("You have an other game on this time");
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
            Notice notice = new Notice();
            foreach(Player p in game.Players)
            {
                if(p.PlayerId != game.CreatorID)
                {
                    notice.MessengerID = game.CreatorID;
                    notice.ReceiverID = p.PlayerId;
                    notice.SportID = game.SportId;
                    notice.Type = Constants.PLAYER_ADD_TO_A_GAME;
                    noticeRestService.CreateNoticeAsync(notice);
                }
            }            
            return gameIdReturn;
        }
    }
}
