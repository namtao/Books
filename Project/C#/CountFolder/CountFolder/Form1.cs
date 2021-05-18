using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CountFolder
{
    public partial class Form1 : Form
    {
        int count = 0;
        public Form1()
        {
            InitializeComponent();
        }

        public void Diff()
        {
            SqlConnection sql1 = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True");
            sql1.Open();
            int rows = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM TblMetadata", sql1).ExecuteScalar()).ToString());
            int countField = 0;
            object[,] arr = new object[rows+rows, 10];
            int row = 0;
            int count = 0;
            int bm0 = Int32.Parse((new SqlCommand("select COUNT(*) from TblBatchManagement where BatchManagementID not in (select BatchID from TblDataEntryHistory) and ProfileName like '%HSLS%'", sql1).ExecuteScalar()).ToString());
            int bm1 = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) = 1) a", sql1).ExecuteScalar()).ToString());
            int bm2 = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) = 2) a", sql1).ExecuteScalar()).ToString());
            int kt = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) > 2) a", sql1).ExecuteScalar()).ToString());
            sql1.Close();
            using (SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True"))
            {
                string sql = "use ADDJ_AnGiang; " +
                "SELECT Metadata, BatchID FROM TblMetadata";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString().Trim().Length > 0 && dr[0] != null && !dr[0].ToString().Trim().Equals(""))
                        {
                            JsonDocument doc = JsonDocument.Parse(dr[0].ToString());
                            JsonElement root = doc.RootElement;
                            for (int i = 0; i < root.GetArrayLength(); i++)
                            {
                                string indexName = root[i].GetProperty("indexName").ToString().Trim();
                                string indexValue = root[i].GetProperty("indexValue").ToString().Trim();
                                string indexValue2 = root[i].GetProperty("indexValue2").ToString().Trim();
                                string indexValueQC = root[i].GetProperty("indexValueQC").ToString().Trim();

                                if (indexValueQC != null && indexValueQC != "") countField++;

                                if ((indexValue != null && indexValue != "")
                                    && (indexValue2 != null && indexValue2 != "") && 
                                    !indexValue.ToString().Trim().ToUpper().Equals(indexValue2.ToString().Trim().ToUpper()))
                                {
                                    SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True");
                                    string str = "use ADDJ_AnGiang; SELECT FullName FROM TblDataEntryHistory d join TblUser u on d.UserID = u.Id where BatchID = " + dr[1].ToString() +
                                        " order by DateEnd";
                                    using (SqlCommand cmd2 = new SqlCommand(str, conn))
                                    {
                                        conn.Open();
                                        cmd2.CommandType = CommandType.Text;
                                        SqlDataReader dataReader = cmd2.ExecuteReader();
                                        while (dataReader.Read())
                                        {
                                            SqlConnection conStr = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True");
                                            string sqlString = "select ProfileName, BatchName from TblBatchManagement where BatchManagementID = "+ dr[1].ToString();
                                            using(SqlCommand sqlCommand = new SqlCommand(sqlString, conStr))
                                            {
                                                conStr.Open();
                                                sqlCommand.CommandType = CommandType.Text;
                                                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                                                while (sqlDataReader.Read())
                                                {
                                                    arr[row, 0] = sqlDataReader[0].ToString()+": "+ sqlDataReader[1].ToString();
                                                }
                                                conStr.Close();
                                            }

                                            arr[row, 1] = dr[1].ToString();
                                            arr[row, 2] = indexName;
                                            if (count == 0) arr[row, 3] = dataReader[0].ToString() + ": " + indexValue;
                                            if (count == 1) arr[row, 4] = dataReader[0].ToString() + ": " + indexValue2;
                                            count++;
                                        }
                                        conn.Close();
                                        count = 0;
                                        row++;
                                    }    
                                }

                            }


                        }
                    }
                    con.Close();
                }
            }
            MessageBox.Show("Có " + countField.ToString() + " trường", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show("Chưa biên mục: "+bm0+"\nHoàn thành biên mục 1: " + bm1 + " \nHoàn thành biên mục 2: " + bm2+ "\nHoàn thành kiểm tra: " + kt, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Utils.ExportThongKe(arr, "So sánh", "So sánh giữa các lần biên mục");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Diff();
            //ThongKe(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\huan huy chuong\");
            Close();
        }

        public static bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void ThongKeHoSoLietSi()
        {
            object[,] arr = new object[100, 3];
            int r = 0;
            int count = 0;
            int fileCount = 0;
            string path = @"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\liet si\count.txt";
            List<string> list = new List<string>();
            if (!File.Exists(path)) File.Create(path);
            File.Create(path).Close();
            try
            {
                //\\192.168.31.206\Share\JPG(đã kiểm tra)\Thai Binh\liet si\kien xuong
                //string[] dirs = Directory.GetDirectories(txtPath.Text.Trim());
                string[] dirs = Directory.GetDirectories(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\liet si\TP TB");
                foreach (string dir in dirs)
                {
                    //vào 1 thư mục
                    string[] d = Directory.GetDirectories(dir);
                    string di = "";
                    for (int i = 0; i < d.Length; i++)
                    {
                        di = new DirectoryInfo(dir).Name;
                        string[] d2 = Directory.GetDirectories(d[i]);
                        foreach (string j in d2)
                        {
                            if (IsNumber(new DirectoryInfo(j).Name.Split(' ')[0]))
                                list.Add(new DirectoryInfo(j).Name.Split(' ')[0]);
                            else list.Add(new DirectoryInfo(j).Name);
                        }
                        fileCount += Directory.GetFiles(d[i], "*.jpg*", SearchOption.AllDirectories).Length;
                    }

                    List<string> distinct = list.Distinct().ToList();
                    count += distinct.Count;

                    //wirte text
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        //sw.WriteLine(di + "\t" + count + "\t" +fileCount);

                        arr[r, 0] = di;
                        arr[r, 1] = count;
                        arr[r, 2] = fileCount;
                        r++;
                    }

                    count = 0;
                    fileCount = 0;
                    list.Clear();
                }

                Utils.ExportThongKe(arr, "SHEET 1", "TEST");

                MessageBox.Show("Finish!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                Console.WriteLine("The process failed: {0}", ex.ToString());
            }
        }

        private void ThongKe(string parentPath)
        {
            int row = 0;
            object[,] arr = new object[500, 3];
            foreach (string dirName in Directory.GetDirectories(parentPath))
            {

                string[] jpgPathFile = Directory.GetFiles(parentPath + new DirectoryInfo(dirName).Name, "*.jpg*", SearchOption.AllDirectories);

                List<string> list = new List<string>();

                foreach (string path in jpgPathFile)
                {
                    list.Add(Path.GetFileName(Path.GetDirectoryName(path)));
                }

                List<string> distinct = list.Distinct().ToList();
                count += distinct.Count;

                arr[row, 0] = new DirectoryInfo(dirName).Name;
                arr[row, 1] = count;
                arr[row, 2] = jpgPathFile.Length;
                row++;
                count = 0;
            }
            Utils.ExportThongKe(arr, "Thống kê", "Thống kê số lượng");
            Close();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {

        }
    }
}
