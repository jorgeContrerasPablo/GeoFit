using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;

namespace RestServiceGeoFit
{
    public class AuxFunctions
    {
        DbParameter parameter;
        public DbParameter createParameter(string name, object value, DbCommand selectCommand)
        {
            parameter = selectCommand.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = System.Data.DbType.Int32;
            parameter.Value = value;

            return parameter;
        }
    }
}