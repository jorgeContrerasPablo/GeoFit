﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using RestServiceGeoFit.Models;
using System.Data;

namespace RestServiceGeoFit.Models
{
    public class PlayerManager
    {
        readonly SqlConnection con = new SqlConnection(Constants.SqlCon);       
        AuxFunctions auxfunction = new AuxFunctions();

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

                selectCommand.CommandText =
                    "SELECT PlayerId,Password,PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, Level, MedOnTime " +
                    "FROM Player " +
                    "WHERE PlayerId = @playerid";

                selectCommand.Transaction = transaction;
                
                selectCommand.Parameters.Add(auxfunction.createParameter("@playerid", PlayerId, selectCommand, DbType.Int32));

                DbDataReader dataReader = selectCommand.ExecuteReader();
               
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    throw new Exception("Error in data base acces!");
                }

               
                while (dataReader.Read())
                {
                    player.PlayerId = dataReader.GetInt32(0);
                    player.Password = dataReader.GetString(1);
                    player.PlayerNick = dataReader.GetString(2);
                    player.PlayerName = dataReader.GetString(3);
                    player.LastName = dataReader.IsDBNull(4) ? String.Empty:dataReader.GetString(4);
                    player.PhoneNum = dataReader.GetInt32(5);
                    player.PlayerMail = dataReader.GetString(6);
//                    player.PhotoId = dataReader.IsDBNull(7) ? Guid.Empty:dataReader.GetGuid(7);
                    player.Level = dataReader.IsDBNull(7) ? new double() : dataReader.GetDouble(7);
                    player.MedOnTime = dataReader.IsDBNull(8) ? new double() : dataReader.GetDouble(8);
                }
                dataReader.Close();
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
                    "INSERT INTO Player (Password, PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, Level, MedOnTime) " +
                    "VALUES (@Password, @PlayerNick, @PlayerName, @LastName, @PhoneNum, @PlayerMail, @Level, @MedOnTime) ; SELECT CONVERT(int, SCOPE_IDENTITY())";

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

        public  bool UpdatePlayer(Player player)
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
                    " LastName = @LastName, PhoneNum = @PhoneNum, PlayerMail = @PlayerMail, Level = @Level, MedOnTime= @MedOnTime "+
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
            return (response==1);
        }
    }
}