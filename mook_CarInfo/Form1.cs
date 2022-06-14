using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace mook_CarInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string connectionStr = "Server=(local);database=ADOTest;" +
                "Integrated Security=true";

        private void Form1_Load(object sender, EventArgs e)
        {
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
                MessageBox.Show("프로그램 실행중에 문제가 발생해서, 프로그램을 종료합니다. \r\n\r\n" + ex.Message, "에러",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            
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