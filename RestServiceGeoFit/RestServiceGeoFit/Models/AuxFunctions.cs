using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data;

namespace RestServiceGeoFit
{
    public class AuxFunctions
    {
        DbParameter parameter;
        public DbParameter createParameter(string name, object value, DbCommand selectCommand, DbType type )
        {
            parameter = selectCommand.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value;

            return parameter;
        }
    }
}