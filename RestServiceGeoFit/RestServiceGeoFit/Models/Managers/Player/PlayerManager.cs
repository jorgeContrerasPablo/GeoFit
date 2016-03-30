using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using RestServiceGeoFit.Models;
using System.Data;
using RestServiceGeoFit.Models.Managers.Player.Exceptions;

namespace RestServiceGeoFit.Models
{
    public class PlayerManager
    {
        readonly SqlConnection con;
        AuxFunctions auxfunction = new AuxFunctions();

        public PlayerManager(bool test)
        {
            if (test)
            {
                con = new SqlConnection(Constants.SqlConTest);
            }
            else
            {
                con = new SqlConnection(Constants.SqlCon);
            }

        }

        public Player GetPlayer(int PlayerId)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            Player player = new Player();
            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO Photo
                selectCommand.CommandText =
                    "SELECT PlayerID, Password, PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, Level, MedOnTime, PlayerSesion " +
                    "FROM Player " +
                    "WHERE PlayerId = @playerid";

                selectCommand.Transaction = transaction;

                selectCommand.Parameters.Add(auxfunction.createParameter("@playerid", PlayerId, selectCommand, DbType.Int32));

                DbDataReader dataReader = selectCommand.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    throw new PlayerNotFoundException(PlayerId);
                }


                while (dataReader.Read())
                {
                    player.PlayerId = dataReader.GetInt32(0);
                    player.Password = dataReader.GetString(1);
                    player.PlayerNick = dataReader.GetString(2);
                    player.PlayerName = dataReader.GetString(3);
                    player.LastName = dataReader.IsDBNull(4) ? String.Empty : dataReader.GetString(4);
                    player.PhoneNum = dataReader.GetInt32(5);
                    player.PlayerMail = dataReader.GetString(6);
                    //TODO
                    //                    player.PhotoId = dataReader.IsDBNull(7) ? Guid.Empty:dataReader.GetGuid(7);
                    player.Level = dataReader.IsDBNull(7) ? new double() : dataReader.GetDouble(7);
                    player.MedOnTime = dataReader.IsDBNull(8) ? new double() : dataReader.GetDouble(8);
                    player.PlayerSesion = dataReader.GetBoolean(9);
                }
                dataReader.Close();

         /*       selectCommand.CommandText =
                    "SELECT TeamID "+
                    "FROM Joined " +
                    "WHERE PlayerId = @playerid";*/

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return player;
        }

        public int CreatePlayer(Player player)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            int response = 0;

            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO add Photo
                selectCommand.CommandText =
                    "INSERT INTO Player (Password, PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, Level, MedOnTime, PlayerSesion) " +
                    "VALUES (@Password, @PlayerNick, @PlayerName, @LastName, @PhoneNum, @PlayerMail, @Level, @MedOnTime, 0) ; SELECT CONVERT(int, SCOPE_IDENTITY())" ;

                selectCommand.Transaction = transaction;

                selectCommand.Parameters.Add(auxfunction.createParameter("@Password", player.Password, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerNick", player.PlayerNick, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerName", player.PlayerName, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@LastName", player.LastName, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PhoneNum", player.PhoneNum, selectCommand, DbType.Int32));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerMail", player.PlayerMail, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@Level", player.Level, selectCommand, DbType.Double));
                selectCommand.Parameters.Add(auxfunction.createParameter("@MedOnTime", player.MedOnTime, selectCommand, DbType.Double));

                object idPlayer = selectCommand.ExecuteScalar();
                if (idPlayer != null)
                {
                    response = (Int32)idPlayer;
                }

                if (response == 0)
                {
                    throw new Exception("Error in data base acces!");
                }

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return response;
        }

        public bool DeletePlayer(int PlayerId)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            int response = 0;
            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                selectCommand.CommandText =
                    "DELETE FROM Player " +
                    "WHERE PlayerId = @playerid";

                selectCommand.Transaction = transaction;

                selectCommand.Parameters.Add(auxfunction.createParameter("@playerid", PlayerId, selectCommand, DbType.Int32));

                response = selectCommand.ExecuteNonQuery();

                if (response != 1)
                {
                    throw new Exception("Error in data base acces!, Number of rows afected : " + response);
                }

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return (response == 1);
        }

        public bool UpdatePlayer(Player player)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            int response = 0;

            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO add Photo
                selectCommand.CommandText =
                    "UPDATE Player " +
                    "SET Password = @Password, PlayerNick = @PlayerNick, PlayerName = @PlayerName," +
                    " LastName = @LastName, PhoneNum = @PhoneNum, PlayerMail = @PlayerMail, Level = @Level, MedOnTime= @MedOnTime " +
                    "WHERE PlayerID = @PlayerID";


                selectCommand.Transaction = transaction;

                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerID", player.PlayerId, selectCommand, DbType.Int32));
                selectCommand.Parameters.Add(auxfunction.createParameter("@Password", player.Password, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerNick", player.PlayerNick, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerName", player.PlayerName, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@LastName", player.LastName, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PhoneNum", player.PhoneNum, selectCommand, DbType.Int32));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerMail", player.PlayerMail, selectCommand, DbType.String));
                selectCommand.Parameters.Add(auxfunction.createParameter("@Level", player.Level, selectCommand, DbType.Double));
                selectCommand.Parameters.Add(auxfunction.createParameter("@MedOnTime", player.MedOnTime, selectCommand, DbType.Double));

                response = selectCommand.ExecuteNonQuery();

                if (response != 1)
                {
                    throw new Exception("Error in data base acces!");
                }

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return (response == 1);
        }

        public int FindPlayerByNickOrMail(string mailOrNick)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            int response = 0;

            try
            {
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO add Photo
                selectCommand.CommandText =
                    "SELECT * " +
                    "FROM Player " +
                    "WHERE PlayerMail = @mailOrNick " +
                    "OR PlayerNick = @mailOrNick ";

                selectCommand.Transaction = transaction;
                selectCommand.Parameters.Add(auxfunction.createParameter("@mailOrNick", mailOrNick, selectCommand, DbType.String));

                object idPlayer = selectCommand.ExecuteScalar();
                if (idPlayer != null)
                {
                    response = (Int32)idPlayer;
                }

                if (response == 0)
                {
                    throw new PlayerNotFoundException(mailOrNick);
                }

                transaction.Commit();
                commited = true;
            }

            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return response;
        }

        public void Session(int playerId, bool OnSession)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            int response = 0;

            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO add Photo
                selectCommand.CommandText =
                    "UPDATE Player " +
                    "SET PlayerSesion = @OnSession " +
                    "WHERE PlayerID = @PlayerID";

                selectCommand.Transaction = transaction;
                selectCommand.Parameters.Add(auxfunction.createParameter("@OnSession", OnSession, selectCommand, DbType.Boolean));
                selectCommand.Parameters.Add(auxfunction.createParameter("@PlayerID", playerId, selectCommand, DbType.Int32));

                response = selectCommand.ExecuteNonQuery();

                if (response != 1)
                {
                    throw new Exception("Error in data base acces!");
                }

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public bool IsOnSession(int playerId)
        {
            DbCommand selectCommand;
            bool commited = false;
            SqlTransaction transaction = null;
            bool response = false;

            try
            {
                //open Connection
                con.Open();
                transaction = con.BeginTransaction(IsolationLevel.Serializable);

                //Create command and set properties
                selectCommand = con.CreateCommand();
                selectCommand.Connection = con;

                //TODO add Photo
                selectCommand.CommandText =
                    "SELECT * " +
                    "FROM Player " +
                    "WHERE PlayerID = @playerId " +
                    "AND OnSession = 1 ";

                selectCommand.Transaction = transaction;
                object idPlayer = selectCommand.ExecuteScalar();
                if (idPlayer != null)
                {
                    response = true;
                }

                transaction.Commit();
                commited = true;

            }
            catch (DbException e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (!commited)
                {
                    transaction.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
            }

            return response;

        }
    }
}