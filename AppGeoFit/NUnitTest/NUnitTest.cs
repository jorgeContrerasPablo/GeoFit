using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.DataAccesLayer.Models;
using NUnit.Framework;
using System;
using Xamarin.Forms;

namespace NUnitTest
{
    [TestFixture()]
    public class NunitTest
    {

        //IPlayerManager playerManager;
        PlayerManager playerManager ;
        Player player1 = new Player();
        Player player2 = new Player();
        Player player3 = new Player();
        Player player4 = new Player();

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
        }

        [Test()]
        public void TestCreatePlayer()
        {
            int player1Id = 0;
            playerManager.DeletePlayer(1);
            try
            {
                player1Id = playerManager.CreatePlayer(player1).Result;
            }
            catch (DuplicatePlayerNickException)
            {
                Assert.True(false);
            }
            catch (DuplicatePlayerMailException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
            Player playerCreated = playerManager.GetPlayer(player1Id).Result;
            player1.PlayerId = player1Id;
            Assert.True(playerCreated.Equals(player1));
        }

        [Test()]
        public void TestCreatePlayerDuplicatePlayerNick()
        {
            int player1Id = 0;
            int player2Id = 0;
            //Lo ponemos igual que el player1
            player2.PlayerNick = "P1";
            player1Id = playerManager.CreatePlayer(player1).Result;
            player1.PlayerId = player1Id;
            try
            {
                player2Id = playerManager.CreatePlayer(player2).Result;
                player2.PlayerId = player2Id;
                Assert.True(false);
            }
            catch (DuplicatePlayerNickException)
            {
                Assert.True(true);
            }
            catch (DuplicatePlayerMailException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }

        }

        [Test()]
        public void TestCreatePlayerDuplicatePlayerMail()
        {
            int player1Id = 0;
            int player2Id = 0;
            //Lo ponemos igual que el player1
            player2.PlayerMail = "player1@hotmail.com";
            player1Id = playerManager.CreatePlayer(player1).Result;
            player1.PlayerId = player1Id;
            try
            {
                player2Id = playerManager.CreatePlayer(player2).Result;
                player2.PlayerId = player2Id;
                Assert.True(false);
            }
            catch (DuplicatePlayerNickException)
            {
                Assert.True(false);
            }
            catch (DuplicatePlayerMailException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }

        }

        [TearDown()]
        public void Tear()
        {
            playerManager.DeletePlayer(player1.PlayerId);
            playerManager.DeletePlayer(player2.PlayerId);
        }

    }
}