using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using AppConfiguration;
using Model;

namespace mook_CarInfo
{
    // 자식 Form에서는 반드시 ApplicationRootForm을 상속 받아서 사용해야 함
    // => 결과적으로 프로그램 전체에서 SqlConnection이 하나로 사용되게 됨
    // => 서버의 공유성(서버의 성능을 낮추지 않음)을 높이고, 프로그램의 재사용(유지보수성 좋아짐) 측면을 모두 고려한 내용

    public partial class Form1 : ApplicationRootForm
    {
        private SqlConnection Conn;

        private string connectionStr = "Server=(local);database=ADOTest;" +
                "Integrated Security=true";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            configurationMgr = ConfigurationMgr.Instance();
            Conn = (SqlConnection)configurationMgr.Connection;
            Conn.Close();

            //MessageBox.Show(configurationMgr.Connection.State.ToString());

            listView_initialize();
        }

        private void listView_initialize()
        {
            this.lvList.Items.Clear();
            var Conn = new SqlConnection(connectionStr);

            try
            {
                Conn.Open();

                var Comm = new SqlCommand("Select * From TB_CAR_INFO", Conn);
                var myRead = Comm.ExecuteReader(CommandBehavior.CloseConnection);
                while (myRead.Read())
                {
                    //var strArray = new String[] { myRead["id"].ToString(),
                    //myRead["carName"].ToString(), myRead["carYear"].ToString(),
                    //myRead["carPrice"].ToString(), myRead["carDoor"].ToString() };

                    // SQL과 View를 분리하기 위한 진행 과정
                    // 최종적으로 listView_initialize()에서 SQL이 없어짐
                    // 없어지는 SQL은 리포지토리로 이동하게 됨
                    // => SQL과 View가 분리 됨 => 비즈니스 로직, SQL 등이 완전 분리하게 됨
                    // => 유지 보수성이 높아짐 테스트가 편해지고, 품질이 높아지게 됨
                    var lvt = new ListViewItem(GetCarInfoModel(myRead).ToStringList());
                    this.lvList.Items.Add(lvt);
                }
                myRead.Close();
                //Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로그램 실행중에 문제가 발생해서, 프로그램을 종료합니다. \r\n\r\n" + ex.Message, "에러",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            
        }

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!textBox_DataChk())
            {
                return;
            }

            var Conn = new SqlConnection(connectionStr);

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

                Comm.Parameters["@carName"].Value = this.txtName.Text;
                Comm.Parameters["@carYear"].Value = this.txtYear.Text;
                Comm.Parameters["@carPrice"].Value = Convert.ToInt32(this.txtPrice.Text);
                Comm.Parameters["@carDoor"].Value = Convert.ToInt32(this.txtDoor.Text);

                /*string Sql = "insert into TB_CAR_INFO(carName, carYear, carPrice, carDoor) values('";
                Sql += this.txtName.Text + "'," + this.txtYear.Text + "," +
                    Convert.ToInt32(this.txtPrice.Text) + "," + Convert.ToInt32(this.txtDoor.Text) + ")";
                var Comm = new SqlCommand(Sql, Conn);*/

                int i = Comm.ExecuteNonQuery();
                Conn.Close();
                if (i == 1)
                {
                    MessageBox.Show("정상적으로 데이터가 저장되었습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listView_initialize();
                    textBox_Data_Initialize();
                }
                else
                {
                    MessageBox.Show("정상적으로 데이터가 저장되지 않았습니다.", "에러",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로그램 실행중에 문제가 발생했습니다.  \r\n\r\n" + ex.Message, "에러",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox_Data_Initialize()
        {
            this.txtName.Clear();
            this.txtYear.Clear();
            this.txtPrice.Clear();
            this.txtDoor.Clear();
        }

        private void lvList_Click(object sender, EventArgs e)
        {
            if (this.lvList.SelectedItems.Count > 0)
            {
                this.txtName.Text = this.lvList.SelectedItems[0].SubItems[1].Text;
                this.txtYear.Text = this.lvList.SelectedItems[0].SubItems[2].Text;
                this.txtPrice.Text = this.lvList.SelectedItems[0].SubItems[3].Text;
                this.txtDoor.Text = this.lvList.SelectedItems[0].SubItems[4].Text;

                /*MessageBox.Show(this.lvList.SelectedItems[0].SubItems[0].Text + "\r\n"
                    + lvList.SelectedItems[0].SubItems[1].Text + "\r\n"
                    + lvList.SelectedItems[0].SubItems[2].Text + "\r\n" 
                    + lvList.SelectedItems[0].SubItems[3].Text + "\r\n"
                    + lvList.SelectedItems[0].SubItems[4].Text);*/
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (!listView_Secected_Row_Check())
            {
                return;
            }

            if (!textBox_DataChk())
            {
                return;
            }

            var Conn = new SqlConnection(connectionStr);

            try {
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

                Comm.Parameters["@id"].Value =
                    Convert.ToInt32(this.lvList.SelectedItems[0].SubItems[0].Text);
                Comm.Parameters["@carName"].Value = this.txtName.Text;
                Comm.Parameters["@carYear"].Value = this.txtYear.Text;
                Comm.Parameters["@carPrice"].Value = Convert.ToInt32(this.txtPrice.Text);
                Comm.Parameters["@carDoor"].Value = Convert.ToInt32(this.txtDoor.Text);

                int i = Comm.ExecuteNonQuery();

                Conn.Close();
                if (i == 1)
                {
                    MessageBox.Show("정상적으로 데이터가 수정되었습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listView_initialize();
                    textBox_Data_Initialize();
                }
                else
                {
                    MessageBox.Show("정상적으로 데이터가 수정되지 않았습니다.", "에러",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로그램 실행중에 문제가 발생했습니다.  \r\n\r\n" + ex.Message, "에러",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.lvList.Items.Clear();
            var Conn = new SqlConnection(connectionStr);

            try {
                Conn.Open();

                string Sql = "Select * From TB_CAR_INFO "
                            + "where carName = @carName or carYear = @carYear "
                            + "or carPrice = @carPrice or carDoor = @carDoor ";

                var Comm = new SqlCommand(Sql, Conn);

                Comm.Parameters.Add("@carName", SqlDbType.NVarChar, 30);
                Comm.Parameters.Add("@carYear", SqlDbType.VarChar, 4);
                Comm.Parameters.Add("@carPrice", SqlDbType.Int);
                Comm.Parameters.Add("@carDoor", SqlDbType.Int);

                Comm.Parameters["@carName"].Value = this.txtName.Text;
                Comm.Parameters["@carYear"].Value = this.txtYear.Text;
                Comm.Parameters["@carPrice"].Value =
                    Convert.ToInt32((this.txtPrice.Text == "") ? 0 : Convert.ToInt32(this.txtPrice.Text));
                Comm.Parameters["@carDoor"].Value =
                    Convert.ToInt32((this.txtDoor.Text == "") ? 0 : Convert.ToInt32(this.txtDoor.Text));


                /*var Comm = new SqlCommand("Select * From TB_CAR_INFO where carName = '" + this.txtName.Text + 
                    "' or carYear = '" + this.txtYear.Text + 
                    "' or carPrice = "
                    + Convert.ToInt32((this.txtPrice.Text == "") ? 0 : Convert.ToInt32(this.txtPrice.Text)) + 
                    " or carDoor = "
                    + Convert.ToInt32((this.txtDoor.Text == "") ? 0 : Convert.ToInt32(this.txtDoor.Text)), Conn);*/

                //var myRead = Comm.ExecuteReader();
                var myRead = Comm.ExecuteReader(CommandBehavior.CloseConnection);

                while (myRead.Read())
                {
                    var strArray = new String[] { myRead["id"].ToString(),
                    myRead["carName"].ToString(), myRead["carYear"].ToString(),
                    myRead["carPrice"].ToString(), myRead["carDoor"].ToString() };
                    var lvt = new ListViewItem(strArray);
                    this.lvList.Items.Add(lvt);
                }
                myRead.Close();
                //Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로그램 실행중에 문제가 발생했습니다.  \r\n\r\n" + ex.Message, "에러",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.lvList.SelectedItems.Count > 0)
            {
                DialogResult dlr = MessageBox.Show("데이터를 삭제할까요?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlr == DialogResult.Yes)
                {
                    var Conn = new SqlConnection(connectionStr);

                    try {
                        Conn.Open();

                        string Sql = "delete from TB_CAR_INFO "
                            + "where id = @id ";

                        var Comm = new SqlCommand(Sql, Conn);

                        Comm.Parameters.Add("@id", SqlDbType.Int);

                        Comm.Parameters["@id"].Value =
                            Convert.ToInt32(this.lvList.SelectedItems[0].SubItems[0].Text);

                        /*string Sql = "delete from TB_CAR_INFO where id = " + Convert.ToInt32(this.lvList.SelectedItems[0].SubItems[0].Text) + "";
                        var Comm = new SqlCommand(Sql, Conn);*/

                        int i = Comm.ExecuteNonQuery();
                        if (i == 1)
                            MessageBox.Show("데이터가 정상적으로 삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("데이터를 삭제하지 못하였습니다..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox_Data_Initialize();
                        listView_initialize();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("프로그램 실행중에 문제가 발생했습니다.  \r\n\r\n" + ex.Message, "에러",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
                MessageBox.Show("삭제할 행을 선택하세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnAllSearch_Click(object sender, EventArgs e)
        {
            textBox_Data_Initialize();
            listView_initialize();
        }

        private bool textBox_DataChk()
        {
            if (this.txtName.Text != "" && this.txtYear.Text != "" &&
                this.txtPrice.Text != "" && this.txtDoor.Text != "")
                return true;
            else
            {
                MessageBox.Show("입력 항목의 데이터를 확인해주세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
        }

        private bool listView_Secected_Row_Check()
        {
            if (this.lvList.SelectedItems.Count > 0)
                return true;
            else
                MessageBox.Show("선택된 데이터가 없습니다. 데이터를 선택해주세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return false;
        }
    }
}