using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AppConfiguration
{
    // 1. App.config xml의 데이터베이스 접속 문자열을 읽어와야 함
    // => AdoNetDb 프로젝트에 MsSql 클래스의 멤버변수에 초기화 값으로 사용
    // => System.Configuration을 참조체 반드시 추가를 해야함

    // 2. 접속 문자열 기준으로 SqlConnection을 생성
    // => AdoNetDb 프로젝트의 MsSql 클래스의 SqlConnection 멤버 초기화로 사용

    // 향후에 최종적으로 CarInfoManagement 프로젝트의 ApplicationRootForm 클래스에서 사용하게됨
    // 앞으로 추가되는 모든 Form에서 ApplicationRootForm 클래스를 상속받아서
    // SqlConnection 계속 재사용이 가능해 짐
    public class ConfigurationMgr
    {
        private static ConfigurationMgr instance;

        public string ConnectionString { get; set; }

        IDbConnection connection;

        public ConfigurationMgr()
        {
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Runtime)
            {
                // App.config 파일을 읽어서 로딩
                LoadConfiguration();

                // connection 생성
                MakeConnection();
            }
        }

        private void LoadConfiguration()
        {
            // App.config 파일을 읽어서 로딩
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // "WinConnection" 이름으로 설정된 데이터베이스 접속 문자열을 탐색
            ConnectionString =
                config.ConnectionStrings.ConnectionStrings["WinConnection"].ConnectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                if (connection == null)
                    connection = new SqlConnection(ConnectionString); //간단하게 만든 싱글톤 패턴.

                // SqlConnection 생성과 동시에 connection open 처리를 추가해도 됨.
                //connection.Open();

                return connection;
            }
            private set { }
        }

        private void MakeConnection()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        // Instance() 메서드가 public static으로 선언이 되어 있음
        // => 외부에서 SqlConnection을 사용하고 싶은데, 이때, ConfigurationMgr 클래스의
        // 객체를 생성하지 않고도 SqlConnection을 사용할 수 있음
        public static ConfigurationMgr Instance()
        {
            if (instance == null)
                instance = new ConfigurationMgr();//ConfigurationMgr 의 싱글톤

            return instance;
        }
    }
}
