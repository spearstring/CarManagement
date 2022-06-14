using Libs;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositiories
{
    // 1. Form1.cs에서 분리해서 가져온 SQL을 실행하기 위해서 ClassForDBInstance 상속받아서
    //    SqlConnection을 사용하게 됨 실질적으로 SqlConnection은 ConfiguratuionMgr이 가지고 있음

    // 2. CRUD 4가지의 기능을 수행 동일한 조회라도 앞으로 더 세분화된 기능으로 발전할 수 있음
    //    따라서, 인터페이스를 통해서 기능 구현 시, 지켜야할 약속을 알릴 수 있어,
    //    새로운 기능도 인터페이스를 따르게 됨으로 가독성, 유지보수도 좋아짐 하나의 큰 카테고리로 관리 가능(다형성)

    // 3. ORM 사용을 하고 있지 않지만, 지금과 같은 기준으로 코드를 작성하면 향후에 ORM이 도입되면,
    //    시스템 확정성이 좋을 수 있음
    public class CarInfoRepository : ClassForDBInstance, ICarInfoRepository
    {
        private SqlConnection Conn;

        // 차량정보 전체조회 후 화면에 조회된 결과를 Model 형태로 반환하기 위한 메서드.
        private CarInfoModel GetCarInfoModel(SqlDataReader myRead)
        {
            CarInfoModel model = new CarInfoModel();

            model.id = myRead["id"].ToString();
            model.carName = myRead["carName"].ToString();
            model.carYear = myRead["carYear"].ToString();
            model.carPrice = myRead["carPrice"].ToString();
            model.carDoor = myRead["carDoor"].ToString();

            return model;
        }

        // 차량정보 신규 등록
        int ICarInfoRepository.Add(CarInfoModel model, IDbConnection connection = null)
        {
            try
            {
                Conn.Open();

                string Sql = "insert into TB_CAR_INFO(carName, carYear, carPrice, carDoor) "
                                + "values( @carName, @carYear, @carPrice, @carDoor )";
                var Comm = new SqlCommand(Sql, Conn);
                Comm.Parameters.Add("@carName", SqlDbType.NVarChar, 30);
                Comm.Parameters.Add("@carYear", SqlDbType.VarChar, 4);
                Comm.Parameters.Add("@carPrice", SqlDbType.Int);
                Comm.Parameters.Add("@carDoor", SqlDbType.Int);

                Comm.Parameters["@carName"].Value = model.carName;
                Comm.Parameters["@carYear"].Value = model.carYear;
                Comm.Parameters["@carPrice"].Value = Convert.ToInt32(model.carPrice);
                Comm.Parameters["@carDoor"].Value = Convert.ToInt32(model.carDoor);

                // 등록 후 영향 받은 row 수를 화면으로 반환
                int i = Comm.ExecuteNonQuery();
                Conn.Close();

                return i;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // 차량정보 삭제
        int ICarInfoRepository.Delete(int carNo, IDbConnection connection = null)
        {
            try
            {
                Conn.Open();

                string Sql = "delete from TB_CAR_INFO where id = @id ";

                var Comm = new SqlCommand(Sql, Conn);
                Comm.Parameters.Add("@id", SqlDbType.Int);
                Comm.Parameters["@id"].Value = carNo;

                // 삭제 후 영향 받은 row 수를 화면으로 반환
                int i = Comm.ExecuteNonQuery();
                Conn.Close();

                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 차량정보 전체조회
        ArrayList ICarInfoRepository.GetAllCarInfo(IDbConnection connection = null)
        {
            try
            {
                // Form1.cs에서 SQL 문장을 분리해서 가져왔기 때문에 
                // SQL 문장 실행을 위해 SqlConnection이 필요함
                // Lib 프로젝트의 ClassForDBInstance를 통해서 사용하게 됨
                // => ClassForDBInstance를 추가로 새로 만들었지만, ConfigurationMgr을 사용하고 있음.

                Conn.Open();

                var Comm = new SqlCommand("Select * From TB_CAR_INFO", Conn);
                var myRead = Comm.ExecuteReader(CommandBehavior.CloseConnection);

                // 조회된 결과를 컬렉션 형태로 저장 후 반환하면,
                // 화면의 리스트 뷰의 뷰 아이템으로 초기화하기 편리함.
                ArrayList list = new ArrayList();

                while (myRead.Read())
                {
                    // GetCarInfoModel() 을 사용하면, 조회된 결과의 각 row를 string[] 로 변환할 수 있어,
                    // 화면에서 조회된 결과를 리스트 뷰에 출력하기가 편리해짐.
                    list.Add(GetCarInfoModel(myRead).ToStringList());
                }

                myRead.Close();

                return list;
            }
            catch (Exception ex)
            {
                throw ex;    
            }
        }

        // 차량정보 조건 검색
        ArrayList ICarInfoRepository.GetCarInfoByCondition(CarInfoModel model, IDbConnection connection = null)
        {
            try
            {
                Conn.Open();

                string Sql = "Select * From TB_CAR_INFO "
                            + "where carName = @carName or carYear = @carYear "
                            + "or carPrice = @carPrice or carDoor = @carDoor ";

                var Comm = new SqlCommand(Sql, Conn);

                Comm.Parameters.Add("@carName", SqlDbType.NVarChar, 30);
                Comm.Parameters.Add("@carYear", SqlDbType.VarChar, 4);
                Comm.Parameters.Add("@carPrice", SqlDbType.Int);
                Comm.Parameters.Add("@carDoor", SqlDbType.Int);

                Comm.Parameters["@carName"].Value = model.carName;
                Comm.Parameters["@carYear"].Value = model.carYear;
                Comm.Parameters["@carPrice"].Value =
                    Convert.ToInt32((model.carPrice == "") ? 0 : Convert.ToInt32(model.carPrice));
                Comm.Parameters["@carDoor"].Value =
                    Convert.ToInt32((model.carDoor == "") ? 0 : Convert.ToInt32(model.carDoor));

                var myRead = Comm.ExecuteReader(CommandBehavior.CloseConnection);

                // 조회된 결과를 컬렉션 형태로 저장 후 반환하면,
                // 화면의 리스트 뷰의 뷰 아이템으로 초기화하기 편함
                ArrayList list = new ArrayList();

                while (myRead.Read())
                {
                    // GetCarInfoModel() 을 사용하면, 조회된 결과의 각 row를 string[] 로 변환할 수 있어,
                    // 화면에서 조회된 결과를 리스트 뷰에 출력하기가 편리함.
                    list.Add(GetCarInfoModel(myRead).ToStringList());
                }
                myRead.Close();

                return list;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 차량정보 수정
        int ICarInfoRepository.Update(CarInfoModel model, IDbConnection connection = null)
        {
            try
            {
                Conn.Open();

                string Sql = "update TB_CAR_INFO "
                        + "set carName = @carName, carYear = @carYear, "
                        + "carPrice = @carPrice, carDoor = @carDoor "
                        + "where id = @id ";

                var Comm = new SqlCommand(Sql, Conn);
                Comm.Parameters.Add("@id", SqlDbType.Int);
                Comm.Parameters.Add("@carName", SqlDbType.NVarChar, 30);
                Comm.Parameters.Add("@carYear", SqlDbType.VarChar, 4);
                Comm.Parameters.Add("@carPrice", SqlDbType.Int);
                Comm.Parameters.Add("@carDoor", SqlDbType.Int);

                Comm.Parameters["@id"].Value = Convert.ToInt32(model.id);
                Comm.Parameters["@carName"].Value = model.carName;
                Comm.Parameters["@carYear"].Value = model.carYear;
                Comm.Parameters["@carPrice"].Value = Convert.ToInt32(model.carPrice);
                Comm.Parameters["@carDoor"].Value = Convert.ToInt32(model.carDoor);

                // 수정 후 영향 받은 row 수를 화면으로 반환
                int i = Comm.ExecuteNonQuery();
                Conn.Close();

                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CarInfoRepository()
        {
            Conn = (SqlConnection)configurationMgr.Connection;
        }
    }
}
