using System;
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
                    "SELECT PlayerId,Password,PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, PhotoId, Level, MedOnTime " +
                    "FROM Player " +
                    "WHERE PlayerId = @playerid";

                selectCommand.Transaction = transaction;
                
                selectCommand.Parameters.Add(auxfunction.createParameter("@playerid", PlayerId, selectCommand));

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
                    player.PhotoId = dataReader.IsDBNull(7) ? Guid.Empty:dataReader.GetGuid(7);
                    player.Level = dataReader.IsDBNull(8) ? new double() : dataReader.GetDouble(8);
                    player.MedOnTime = dataReader.IsDBNull(9) ? new double() : dataReader.GetDouble(9);
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
    }
}