using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;

namespace NUnitTest
{
    [TestFixture()]
    class NunitTestTeam
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

            team1 = new Team();
            team2 = new Team();

            player1 = new Player();
            player2 = new Player();
            player3 = new Player();
            player4 = new Player();
            player5 = new Player();

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

        }

        [Test()]
        public void TestCreateTeamTest()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            player1 = playerManager.GetPlayer(player1.PlayerId);

            try
            {
                team1.TeamID = teamManager.CreateTeam(team1, player1);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(false);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            Assert.True(team1.Equals(teamManager.GetTeam(team1.TeamID)));
        }

        [Test()]
        public void TestCreateTeamDuplicateTeamName()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";
            
            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "PollitosF7";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            team1.TeamID = teamManager.CreateTeam(team1, player1);

            try
            {
                team2.TeamID = teamManager.CreateTeam(team2, player2);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(true);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestCreateTeamAlreadyCaptainOnSport()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Makinas";

            player1 = playerManager.GetPlayer(player1.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);

            try
            {
                team2.TeamID = teamManager.CreateTeam(team2, player1);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(false);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateTeam()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";
            player1 = playerManager.GetPlayer(player1.PlayerId);
            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            try
            {
                team1.TeamName = "Pollitos";
                teamManager.UpdateTeam(team1, player1);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(false);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
            Assert.True(team1.Equals(teamManager.GetTeam(team1.TeamID)));
        }

        [Test()]
        public void TestUpdateTeamDuplicateTeamName()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Team";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            player2 = playerManager.GetPlayer(player2.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            team2.TeamID = teamManager.CreateTeam(team2, player2);

            try
            {
                team1.TeamName = "F7Team";
                teamManager.UpdateTeam(team1, player1);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(true);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestUpdateTeamAlreadyCaptainOnSport()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Makinas";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            player2 = playerManager.GetPlayer(player2.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            team2.TeamID = teamManager.CreateTeam(team2, player2);
            team2 = teamManager.GetTeam(team2.TeamID);
            teamManager.AddPlayer(player2.PlayerId, team1.TeamID);

            try
            {
                teamManager.UpdateTeam(team1, player2);
            }
            catch (DuplicateTeamNameException)
            {
                Assert.True(false);
            }
            catch (AlreadyCaptainOnSportException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestRemovePlayer()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Makinas";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            player2 = playerManager.GetPlayer(player2.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            teamManager.AddPlayer(player2.PlayerId, team1.TeamID);

            try
            {
                teamManager.RemovePlayer(team1.TeamID, player2.PlayerId);
            }
            catch (OnlyOnePlayerException)
            {
                Assert.True(false);
            }
            catch (CaptainRemoveException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestRemovePlayerOnlyOnePlayer()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Makinas";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            player2 = playerManager.GetPlayer(player2.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);

            try
            {
                teamManager.RemovePlayer(team1.TeamID, player1.PlayerId);
            }
            catch (OnlyOnePlayerException)
            {
                Assert.True(true);
            }
            catch (CaptainRemoveException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }


        [Test()]
        public void TestRemovePlayerCaptainRemoveException()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            team2.ColorTeam = "#096ab1";
            team2.SportID = 1;
            team2.TeamName = "F7Makinas";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            player2 = playerManager.GetPlayer(player2.PlayerId);

            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            teamManager.AddPlayer(player2.PlayerId, team1.TeamID);

            try
            {
                teamManager.RemovePlayer(team1.TeamID, player1.PlayerId);
            }
            catch (OnlyOnePlayerException)
            {
                Assert.True(false);
            }
            catch (CaptainRemoveException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestSendNoticeAddPlayer()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);

            try
            {
                teamManager.SendNoticeAddPlayer(player2.PlayerNick, team1);
            }
            catch (DuplicatePlayerOnTeamException)
            {
                Assert.True(false);
            }
            catch (DuplicateNoticeException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestSendNoticeAddPlayerDuplicatePlayerOnTeam()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);
            teamManager.AddPlayer(player2.PlayerId, team1.TeamID);
            try
            {
                teamManager.SendNoticeAddPlayer(player2.PlayerNick, team1);
            }
            catch (DuplicatePlayerOnTeamException)
            {
                Assert.True(true);
            }
            catch (DuplicateNoticeException)
            {
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Test()]
        public void TestSendNoticeAddPlayerDuplicateNotice()
        {
            team1.ColorTeam = "#096ab1";
            team1.SportID = 1;
            team1.TeamName = "PollitosF7";

            player1 = playerManager.GetPlayer(player1.PlayerId);
            team1.TeamID = teamManager.CreateTeam(team1, player1);
            team1 = teamManager.GetTeam(team1.TeamID);

            teamManager.SendNoticeAddPlayer(player2.PlayerNick, team1);
            try
            {
                teamManager.SendNoticeAddPlayer(player2.PlayerNick, team1);
            }
            catch (DuplicatePlayerOnTeamException)
            {
                Assert.True(false);
            }
            catch (DuplicateNoticeException)
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
