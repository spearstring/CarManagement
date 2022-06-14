# CarManagement
<br/>

C# WinForm 학습 리포지토리
<hr/>

<br/>

## 자동차 정보 관리 앱 추가 버전

<br/>

 이전에 학습한 자동차정보관리 앱에 프로젝트를 추가하여 개발과정에서 테스트가 쉽고 테스트 결과 품질이 향상되어 유지보수가 좋아지도록 수정
 
 ### 추가 항목
 
  1. DB관련
     - 접속
       - 접속정보를 공통으로 사용할 수 있게 수정
     - Connection 객체 생성
       - 하나의 접속 객체만 갖도록 수정 => 싱글톤 패턴 

  2. Form1의 부모 Form을 생성하여 재상용성을 높이도록 수정 => DBConnection

  3. SQL(리포지토리), Model, View로 분리
     - SQL (CRUD) => 리포지토리
     - SQL (R) => Model
     - Model의 데이터를 View 출력
