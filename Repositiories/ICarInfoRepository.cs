using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositiories
{
    public interface ICarInfoRepository
    {
        // 등록, 삭제, 수정, 전체검색, 조건 검색
        int Add(CarInfoModel model, IDbConnection connection = null);
        int Update(CarInfoModel model, IDbConnection connection = null);
        int Delete(int carNO, IDbConnection connection = null);
        ArrayList GetAllCarInfo(IDbConnection connection = null);
        ArrayList GetCarInfoByCondition(CarInfoModel model, IDbConnection connection = null);
    }
}
