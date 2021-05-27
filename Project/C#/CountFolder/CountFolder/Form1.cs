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

        public void ExportDataEntry()
        {
            File.WriteAllText(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\dataentry.txt", String.Empty);
            using (StreamWriter streamWriter = File.AppendText(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\dataentry.txt"))
            {

                SqlConnection sql1 = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True");
                sql1.Open();
                int count = 0;
                sql1.Close();
                using (SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True"))
                {
                    string sql = "use ADDJ_AnGiang; " +
                    "select ProfileName, e.BatchID , BatchName, HoSoSo, RoleNameVie, FullName, m.Metadata " +
                    "from TblDataEntryHistory e join TblUser u on e.UserID = u.Id join TblBatchManagement b " +
                    "on e.BatchID = b.BatchManagementID join TblRole r on r.ID = e.RoleID join TblMetadata m " +
                    "on b.BatchManagementID = m.BatchID " +
                    "where convert(varchar, DateEnd, 103) = '"+ dateTimePicker1.Value.ToString("dd/MM/yyyy") + "' and Status = 6 and StatusOutPut = 2";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            JsonDocument doc = JsonDocument.Parse(dr[6].ToString());
                            JsonElement root = doc.RootElement;

                            streamWriter.Write(dr[0].ToString());
                            //streamWriter.Write("\t" + dr[1].ToString());
                            streamWriter.Write("\t" + dr[2].ToString());
                            //streamWriter.Write("\t" + dr[3].ToString());
                            streamWriter.Write("\t" + dr[4].ToString());
                            streamWriter.Write("\t" + dr[5].ToString());

                            for (int i = 0; i < root.GetArrayLength(); i++)
                            {
                                streamWriter.WriteLine();
                                streamWriter.Write("\t" + root[i].GetProperty("indexName").ToString().Trim());
                                if(dr[4].ToString().Equals("Nhập liệu")) streamWriter.Write("\t" + root[i].GetProperty("indexValue").ToString().Trim());
                                if (dr[4].ToString().Equals("Nhập liệu lần 2")) streamWriter.Write("\t" + root[i].GetProperty("indexValue2").ToString().Trim());
                                //streamWriter.Write("\t" + root[i].GetProperty("indexValueQC").ToString().Trim());
                            }
                            streamWriter.WriteLine();
                        }
                        con.Close();
                    }
                }
            }
        }

        public void Diff()
        {
            using (StreamWriter streamWriter = File.AppendText(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\empty.txt"))
            {

                SqlConnection sql1 = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True");
                string[] loai = { "HSLS", "HSHHC" };
                object[,] arr = null;
                foreach (string s in loai)
                {
                    sql1.Open();
                    int rows = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM TblMetadata " +
                    "where BatchID in (select BatchManagementID from TblBatchManagement where ProfileName like '%"+s+"%')", sql1).ExecuteScalar()).ToString());
                    arr = new object[rows * rows, 10];
                    int countField = 0;
                    int row = 0;
                    int count = 0;
                    //int bm0 = Int32.Parse((new SqlCommand("select COUNT(*) from TblBatchManagement where BatchManagementID not in (select BatchID from TblDataEntryHistory) and ProfileName like '%"+s+"%'", sql1).ExecuteScalar()).ToString());
                    //int bm1 = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) = 1) a", sql1).ExecuteScalar()).ToString());
                    //int bm2 = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) = 2) a", sql1).ExecuteScalar()).ToString());
                    //int kt = Int32.Parse((new SqlCommand("SELECT COUNT(*) FROM (SELECT COUNT(*) AS SoLuong FROM TblDataEntryHistory group by BatchID having count(*) > 2) a", sql1).ExecuteScalar()).ToString());
                    sql1.Close();
                    using (SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ADDJ_AnGiang;Integrated Security=True"))
                    {
                        string sql = "use ADDJ_AnGiang; " +
                        "SELECT Metadata, BatchID FROM TblMetadata " +
                        "where BatchID in (select BatchManagementID from TblBatchManagement where ProfileName like '%" + s + "%')";
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

                                        if ((indexValue == null || indexValue == "") && (indexValue2 == null || indexValue2 == "") && (indexValueQC == null || indexValueQC == ""))
                                        {
                                            streamWriter.WriteLine(dr[1]);
                                        }

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
                                                    string sqlString = "select ProfileName, BatchName from TblBatchManagement where BatchManagementID = " + dr[1].ToString();
                                                    using (SqlCommand sqlCommand = new SqlCommand(sqlString, conStr))
                                                    {
                                                        conStr.Open();
                                                        sqlCommand.CommandType = CommandType.Text;
                                                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                                                        while (sqlDataReader.Read())
                                                        {
                                                            arr[row, 0] = sqlDataReader[0].ToString() + ": " + sqlDataReader[1].ToString();
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
                    MessageBox.Show(s+": " + countField.ToString() + " trường", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("Chưa biên mục: "+bm0+"\nHoàn thành biên mục 1: " + bm1 + " \nHoàn thành biên mục 2: " + bm2+ "\nHoàn thành kiểm tra: " + kt, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    DialogResult res = MessageBox.Show("Bạn có muốn xuất dữ liệu biên mục "+s+" không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes)
                    {
                        Utils.ExportThongKe(arr, "So sánh", "So sánh giữa các lần biên mục");
                    }
                    if (res == DialogResult.No)
                    {

                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ExportDataEntry();
            //ThongKeHoSoLietSi();
            //MessageBox.Show(Directory.GetFiles(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\index.txt", "*.jpg*", SearchOption.AllDirectories).Length+"");
            //MessageBox.Show(Directory.GetParent(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\index.txt")+"");
            //getParentPath(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\CĐHH");
            getParentPath(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\liet si");
            //ExportDataEntry();
            //moveFile(@"C:\Data\CĐHH");
            MessageBox.Show("Xong");
            Close();
        }

        public void moveFile(string path)
        {
            string txtPath = @"\\192.168.31.206\Share\JPG (đã kiểm tra)\path.txt";
            List<string> list = new List<string>();
            List<string> listFolderName = new List<string>();
            using (StreamWriter writer = File.AppendText(txtPath))
            {
                string[] arrPathJpg = Directory.GetFiles(path, "*.jpg*", SearchOption.AllDirectories);
                foreach (string str in arrPathJpg)
                {
                    list.Add(Directory.GetParent(str) + "");
                    listFolderName.Add(new DirectoryInfo(str).Parent.Name.Trim());
                }

                for(int i = 0; i < listFolderName.Count;i++)
                {
                    if (list.Count(s => s.Contains(listFolderName[i])) > 1) 
                        MessageBox.Show(list[i]);
                }
            }
        }

        public void getParentPath(string path)
        {
            File.WriteAllText(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\path.txt", String.Empty);

            string txtPath = @"\\192.168.31.206\Share\JPG (đã kiểm tra)\path.txt";
            using (StreamWriter writer = File.AppendText(txtPath))
            {
                string[] arrPathJpg = Directory.GetFiles(path, "*.jpg*", SearchOption.AllDirectories);
                List<string> list = new List<string>();
                foreach (string str in arrPathJpg)
                {
                    //MessageBox.Show(new DirectoryInfo(str).Parent.Name.Trim());
                    if ((new DirectoryInfo(str).Parent.Name.Trim().Length >= 4) && !IsNumber(new DirectoryInfo(str).Parent.Name.Substring(0, 4))
                        && !IsNumber(new DirectoryInfo(str).Parent.Name.Substring(new DirectoryInfo(str).Parent.Name.Length - 4, 4)))
                        list.Add(Directory.GetParent(str)+"");
                        //writer.WriteLine(Directory.GetParent(str));
                }

                foreach(string strList in list.Distinct().ToList())
                {
                    writer.WriteLine(strList);
                }
            }
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
            string path = @"\\192.168.31.206\Share\JPG (đã kiểm tra)\index.txt";
            string path2 = @"\\192.168.31.206\Share\JPG (đã kiểm tra)\count.txt";
            List<string> list = new List<string>();
            if (!File.Exists(path)) File.Create(path);
            File.Create(path).Close();
            try
            {
                //\\192.168.31.206\Share\JPG(đã kiểm tra)\Thai Binh\liet si\kien xuong
                //string[] dirs = Directory.GetDirectories(txtPath.Text.Trim());
                string[] array = {"DONG HUNG", "hung ha", "kien xuong", "quynh phu", "tien hai", "TP TB", "Thai Thuy", "VU THU" };
                foreach (string index in array)
                {
                    string[] dirs = Directory.GetDirectories(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\Thai Binh\liet si\" + index);
                    using (StreamWriter streamWriter = File.AppendText(path))
                    {
                        //sw.WriteLine(di + "\t" + count + "\t" +fileCount);

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
                                    if ((new DirectoryInfo(j).Name.Trim().Length >= 4) && IsNumber(new DirectoryInfo(j).Name.Substring(0, 4)))
                                        list.Add(new DirectoryInfo(j).Name.Substring(0, 4));
                                    else if ((new DirectoryInfo(j).Name.Trim().Length >= 4) && 
                                        IsNumber(new DirectoryInfo(j).Name.Substring(new DirectoryInfo(j).Name.Length-4, 4)))
                                        list.Add(new DirectoryInfo(j).Name.Substring(new DirectoryInfo(j).Name.Length-4, 4));
                                    else
                                    {
                                        //streamWriter.WriteLine(new DirectoryInfo(j).Name);
                                        streamWriter.WriteLine(j);
                                        list.Add(new DirectoryInfo(j).Name);
                                    }
                                }
                                fileCount += Directory.GetFiles(d[i], "*.jpg*", SearchOption.AllDirectories).Length;
                            }

                            List<string> distinct = list.Distinct().ToList();
                            count += distinct.Count;

                            //wirte text
                            /*using (StreamWriter sw = File.AppendText(path2))
                            {
                                //sw.WriteLine(di + "\t" + count + "\t" +fileCount);

                                arr[r, 0] = di;
                                arr[r, 1] = count;
                                arr[r, 2] = fileCount;
                                r++;
                            }*/

                            count = 0;
                            fileCount = 0;
                            list.Clear();
                        }
                    }
                    //Utils.ExportThongKe(arr, "SHEET 1", "TEST");
                }
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ExportDataEntry();
            MessageBox.Show(@"\\192.168.31.206\Share\JPG (đã kiểm tra)\dataentry.txt");
        }
    }
}
