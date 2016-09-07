using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NUnitTest
{
    [TestFixture()]
    class NunitTestGame
    {
        PlayerManager playerManager;
        Player player1 = new Player();
        Player player2 = new Player();
        Player player3 = new Player();
        Player player4 = new Player();
        Player player5 = new Player();

        TeamManager teamManager;
        Team team1 = new Team();
        Team team2 = new Team();

        GameManager gameManager;
        Game game1 = new Game();
        Game game2 = new Game();

        [SetUp()]
        public void Setup()
        {
            //Necesario para poder usar el assembly con xamarin.forms.
            var platformServicesProperty = typeof(Device)
                .GetProperty("PlatformServices", System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.NonPublic);
            platformServicesProperty.SetValue(null, new PlatformServicesMock());
            playerManager = new PlayerManager();
            playerManager.InitiateServices(true);
            teamManager = new TeamManager();
            teamManager.InitiateServices(true);
            gameManager = new GameManager();
            gameManager.InitiateServices(true);

            team1 = new Team();
            team2 = new Team();

            player1 = new Player();
            player2 = new Player();
            player3 = new Player();
            player4 = new Player();
            player5 = new Player();

            game1 = new Game();
            game2 = new Game();

            player1.PlayerNick = "P1";
            player1.PlayerName = "Player1";
            player1.PlayerMail = "player1@hotmail.com";
            player1.PhoneNum = Convert.ToInt32(1245783);
            player1.Password = "1234";
            player1.FavoriteSportID = 1;
            player1.MedOnTime = 0;
            player1.Level = 0;

            player2.PlayerNick = "P2";
            player2.PlayerName = "Player2";
            player2.PlayerMail = "player2@hotmail.com";
            player2.PhoneNum = Convert.ToInt32(1245783);
            player2.Password = "1234";
            player2.FavoriteSportID = 1;
            player2.MedOnTime = 0;
            player2.Level = 0;

            player3.PlayerNick = "P3";
            player3.PlayerName = "Player3";
            player3.PlayerMail = "player3@hotmail.com";
            player3.PhoneNum = Convert.ToInt32(1245783);
            player3.Password = "1234";
            player3.FavoriteSportID = 2;
            player3.MedOnTime = 0;
            player3.Level = 0;

            player4.PlayerNick = "P4";
            player4.PlayerName = "Player4";
            player4.PlayerMail = "player4@hotmail.com";
            player4.PhoneNum = Convert.ToInt32(1245783);
            player4.Password = "1234";
            player4.FavoriteSportID = null;
            player4.MedOnTime = 0;
            player4.Level = 0;

            player5.PlayerNick = "P5";
            player5.PlayerName = "Player5";
            player5.PlayerMail = "player5@hotmail.com";
            player5.PhoneNum = Convert.ToInt32(1245783);
            player5.Password = "1234";
            player5.FavoriteSportID = null;
            player5.MedOnTime = 0;
            player5.Level = 0;

            player1.PlayerId = playerManager.CreatePlayer(player1);
            player2.PlayerId = playerManager.CreatePlayer(player2);
            player3.PlayerId = playerManager.CreatePlayer(player3);
            player4.PlayerId = playerManager.CreatePlayer(player4);
            player5.PlayerId = playerManager.CreatePlayer(player5);

            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";
            team1.TeamID = teamManager.CreateTeam(team1, player1);

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "MakinasF7";
            team2.TeamID = teamManager.CreateTeam(team2, player2);
        }

        [Test()]
        public void TestCreateGameTest()
        {
            try
            {
                game1.CreatorID = player1.PlayerId;
                game1.StartDate = DateTime.Now.AddHours(2);
                game1.PlaceID = 6;
                game1.PlayersNum = 1;
                game1.SportId = 1;
                game1.EndDate = DateTime.Now.AddHours(3);
                //game1.Team1ID =;
                //game1.Team2ID =;
                player3 = playerManager.GetPlayer(player3.PlayerId);
                game1.Players.Add(player3);
                game1.GameID=gameManager.CreateGame(game1);
            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch(GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestCreateGameWrongTimeTest()
        {
            try
            {
                game1.CreatorID = player1.PlayerId;
                game1.StartDate = DateTime.Now.AddHours(0);
                game1.PlaceID = 6;
                game1.PlayersNum = 1;
                game1.SportId = 1;
                game1.EndDate = DateTime.Now.AddHours(1);
                //game1.Team1ID =;
                //game1.Team2ID =;
                player3 = playerManager.GetPlayer(player3.PlayerId);
                game1.Players.Add(player3);
                game1.GameID = gameManager.CreateGame(game1);
            }
            catch (WrongTimeException)
            {
                Assert.True(true);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestCreateGameOnTimeTest()
        {
            try
            {
                game1.CreatorID = player1.PlayerId;
                game1.StartDate = DateTime.Now.AddHours(3);
                game1.PlaceID = 6;
                game1.PlayersNum = 1;
                game1.SportId = 1;
                game1.EndDate = DateTime.Now.AddHours(4);
                //game1.Team1ID =;
                //game1.Team2ID =;
                player3 = playerManager.GetPlayer(player3.PlayerId);
                game1.Players.Add(player3);
                game1.GameID = gameManager.CreateGame(game1);

                game2.CreatorID = player3.PlayerId;
                game2.StartDate = DateTime.Now.AddHours(3);
                game2.PlaceID = 5;
                game2.PlayersNum = 1;
                game2.SportId = 1;
                game2.EndDate = DateTime.Now.AddHours(4);
                //game1.Team1ID =;
                //game1.Team2ID =;
                game2.GameID = gameManager.CreateGame(game2);

            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(true);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestCreateGameOnTimeAndPlaceTest()
        {
            try
            {
                game1.CreatorID = player1.PlayerId;
                game1.StartDate = DateTime.Now.AddHours(3);
                game1.PlaceID = 6;
                game1.PlayersNum = 1;
                game1.SportId = 1;
                game1.EndDate = DateTime.Now.AddHours(4);
                game1.Team1ID = team1.TeamID;
                //game1.Team2ID =;
                game1.GameID = gameManager.CreateGame(game1);

                game2.CreatorID = player3.PlayerId;
                game2.StartDate = DateTime.Now.AddHours(3);
                game2.PlaceID = 6;
                game2.PlayersNum = 1;
                game2.SportId = 1;
                game2.EndDate = DateTime.Now.AddHours(4);
                //game1.Team1ID =;
                //game1.Team2ID =;
                game2.GameID = gameManager.CreateGame(game2);

            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateGameTest()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(2);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(3);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);
            try
            {
                game1.PlaceID = 4;
                gameManager.UpdateGame(game1);
            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateGameWrongTime()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);
            try
            {
                game1.StartDate = DateTime.Now.AddHours(0);
                game1.EndDate = DateTime.Now.AddHours(1);
                gameManager.UpdateGame(game1);
            }
            catch (WrongTimeException)
            {
                Assert.True(true);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateGameOnTime()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);

            game2.CreatorID = player3.PlayerId;
            game2.StartDate = DateTime.Now.AddHours(6);
            game2.PlaceID = 5;
            game2.PlayersNum = 1;
            game2.SportId = 1;
            game2.EndDate = DateTime.Now.AddHours(8);
            //game1.Team1ID =;
            //game1.Team2ID =;
            game2.GameID = gameManager.CreateGame(game2);
            try
            {                
                game2.StartDate = DateTime.Now.AddHours(2);
                game2.EndDate = DateTime.Now.AddHours(4);
                gameManager.UpdateGame(game2);

            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(true);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateGameOnTimeAndPlace()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            game1.Team1ID = team1.TeamID;
            //game1.Team2ID =;
            game1.GameID = gameManager.CreateGame(game1);

            game2.CreatorID = player3.PlayerId;
            game2.StartDate = DateTime.Now.AddHours(3);
            game2.PlaceID = 5;
            game2.PlayersNum = 1;
            game2.SportId = 1;
            game2.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            game2.GameID = gameManager.CreateGame(game2);
            try
            {
                game2.PlaceID = 6;
                gameManager.UpdateGame(game2);
            }
            catch (WrongTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (GameOnTimeAndPlaceException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestAddPlayer()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);

            try
            {
                gameManager.AddPlayer(game1.GameID, player4.PlayerId);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (PlayerOnGameException)
            {
                Assert.True(false);
            }
            catch (MaxPlayerOnGameException)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestAddPlayerGameOnTime()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);

            game2.CreatorID = player4.PlayerId;
            game2.StartDate = DateTime.Now.AddHours(3);
            game2.PlaceID = 5;
            game2.PlayersNum = 1;
            game2.SportId = 1;
            game2.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            game2.GameID = gameManager.CreateGame(game2);

            try
            {
                gameManager.AddPlayer(game1.GameID, player4.PlayerId);
            }
            catch (GameOnTimeException)
            {
                Assert.True(true);
            }
            catch (PlayerOnGameException)
            {
                Assert.True(false);
            }
            catch (MaxPlayerOnGameException)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestAddPlayerOnGame()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 1;
            game1.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);

            try
            {
                gameManager.AddPlayer(game1.GameID, player3.PlayerId);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (PlayerOnGameException)
            {
                Assert.True(true);
            }
            catch (MaxPlayerOnGameException)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestAddPlayerMaxPlayerOnGame()
        {
            game1.CreatorID = player1.PlayerId;
            game1.StartDate = DateTime.Now.AddHours(3);
            game1.PlaceID = 6;
            game1.PlayersNum = 1;
            game1.SportId = 6;
            game1.EndDate = DateTime.Now.AddHours(4);
            //game1.Team1ID =;
            //game1.Team2ID =;
            player3 = playerManager.GetPlayer(player3.PlayerId);
            game1.Players.Add(player3);
            game1.GameID = gameManager.CreateGame(game1);
            gameManager.AddPlayer(game1.GameID, player4.PlayerId);
            gameManager.AddPlayer(game1.GameID, player2.PlayerId);

            try
            {

                gameManager.AddPlayer(game1.GameID, player5.PlayerId);
            }
            catch (GameOnTimeException)
            {
                Assert.True(false);
            }
            catch (PlayerOnGameException)
            {
                Assert.True(false);
            }
            catch (MaxPlayerOnGameException)
            {
                Assert.True(true);
            }
        }

        [TearDown()]
        public void Tear()
        {
            if(game1.GameID != 0)
                gameManager.DeleteGame(game1.GameID);
            if (game2.GameID != 0)
                gameManager.DeleteGame(game2.GameID);
            if (team1.TeamID != 0)
                teamManager.DeleteTeam(team1.TeamID);
            if (team2.TeamID != 0)
                teamManager.DeleteTeam(team2.TeamID);
            if (player1.PlayerId != 0)
                playerManager.DeletePlayer(player1.PlayerId);
            if (player2.PlayerId != 0)
                playerManager.DeletePlayer(player2.PlayerId);
            if (player3.PlayerId != 0)
                playerManager.DeletePlayer(player3.PlayerId);
            if (player4.PlayerId != 0)
                playerManager.DeletePlayer(player4.PlayerId);
            if (player5.PlayerId != 0)
                playerManager.DeletePlayer(player5.PlayerId);

        }
    }
}
