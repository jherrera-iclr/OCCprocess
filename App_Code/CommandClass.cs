using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using inteliclear.v1;

namespace OCCprocess
{
    class CommandClass
    {

        public int UpdateOptionExpDt(string sSymbol, string sExDate)
        {
            int ret = 0;

            var connString = icGeneric.GetConnString();
            using var sqlConn = new SqlConnection(connString);
            sqlConn.Open();
            var proc = $"upd_opt_profile_details_sp";
            using var cmd = new SqlCommand(proc, sqlConn) { CommandType = CommandType.StoredProcedure };

            // @symbol
            cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = "@symbol",
                DbType = DbType.String,
                Size = 25,
                Value = sSymbol
            }
            );

            // @expite_dt
            cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = "@expire_dt",
                DbType = DbType.DateTime,
                Size = 10,
                Value = (sExDate.Trim().Length > 0) ? (object) DateTime.Parse(sExDate.ToString()) : DBNull.Value
            }
            );

            // @ReturnValue
            cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.ReturnValue,
                ParameterName = "@ReturnValue",
                DbType = System.Data.DbType.Int32,
                Size = 4,
            }
            );

            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();

            ret = (int)cmd.Parameters["@ReturnValue"].Value;
            return ret;
        }
    }
}
