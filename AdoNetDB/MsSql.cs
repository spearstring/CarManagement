using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDB
{
    // Appconfiguration 프로젝트의 ConfigurationMgr에서 사용되게 됨
    public class MsSql : IDatabase
    {
        public string ConnectionString { get; set; }
        public IDbConnection Connection { get; set; }

        public MsSql(string connection_string)
        {
            // 데이터베이스 접속 문자열
            ConnectionString = connection_string;

            // 접속 문자열을 사용하여 데이터베이스 접속 커넥션 => SqlConnection
            Connection = new SqlConnection(ConnectionString);
        }
    }
}