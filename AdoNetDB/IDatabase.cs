using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDB
{
    public interface IDatabase
    {
        // 데이터베이스 접속 문자열 => CarInfoManagement 프로젝트의 App.config의 접속 문자열
        string ConnectionString { get; set; }

        // IDbConnection 인터페이스로 구현됨
        // 다운캐스팅을 하면 SqlConnection 으로 사용가능
        IDbConnection Connection { get; set; }
    }
}