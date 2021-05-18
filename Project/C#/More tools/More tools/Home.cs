using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace More_tools
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            
        }

        public void runCmd(string cmd)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = "/user:Administrator \"cmd /K " + cmd + "\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Verb = "runas";
            process.Start();
            //System.IO.StreamReader SR = process.StandardOutput;
            //System.IO.StreamWriter SW = process.StandardInput;
            process.StandardInput.WriteLine("ipconfig");
            //SW.WriteLine("ipconfig");
            process.StandardInput.Flush();
            process.StandardInput.Close();

            MessageBox.Show(process.StandardOutput.ReadToEnd());
            //MessageBox.Show(SR.ReadToEnd());
        }
    }
}
