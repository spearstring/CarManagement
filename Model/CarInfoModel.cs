using System;

namespace Model
{
    // SQL(리포지토리), Model, View로 분리를 위해서
    // 데이터베이스와 화면에서 오가는 데이터를 저장 및 공유하기 위한
    // 클래스가 필요하게 됨 => Model 
    public class CarInfoModel
    {
        public string id { get; set; }
        public string carName { get; set; }
        public string carYear { get; set; }
        public string carPrice { get; set; }
        public string carDoor { get; set; }

        // ListView의 ListViewItem 값으로 쉽게 사용하기 위한 메서드
        // ListView에 결과 데이터를 출력하기 위해서는 string[] 형태로 만들어야 함
        public string[] ToStringList()
        {
            return new String[] { id, carName, carYear, carPrice, carDoor };

        }
    }
}
