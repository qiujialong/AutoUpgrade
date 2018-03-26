using ICSharpCode.SharpZipLib.Zip;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_upgrade
{
    /// <summary>
    /// 需要外部调用该程序才能打开
    /// </summary>
    public partial class AutoUpgrade : Form
    {
        string[] args = null;
        string type = string.Empty;
        public AutoUpgrade()
        {
            MessageBox.Show("运行失败");
            System.Environment.Exit(0);
        }

        public AutoUpgrade(string[] args)
        {
            if (args[0] == "Test")
            {
                InitializeComponent();
                type = args[0];
            }
            else
            {
                MessageBox.Show("运行失败");
                System.Environment.Exit(0);
            }
        }

        private void Upgrade()
        {
            rtxtMsg.AppendText("下载目标程序...... \n");
            this.btnCancel.Enabled = false;
            this.btnUpgrade.Enabled = false;

            timer2.Interval = 1000;
            timer2.Enabled = true;
            timer2.Start();
        }

        private bool ZipOut()
        {
            bool flag = false;
            try
            {
                Directory.CreateDirectory(@"C:\" + type + "AutoUpgrade");
                ZipInputStream zipInputStream = new ZipInputStream(File.Open(@"C:\" + type + ".zip", FileMode.Open));
                ZipEntry zipEntryFromZippedFile = zipInputStream.GetNextEntry();
                while (zipEntryFromZippedFile != null)
                {
                    if (zipEntryFromZippedFile.IsFile)
                    {
                        FileInfo fInfo = new FileInfo(string.Format(@"C:\" + type + "AutoUpgrade\\{0}", zipEntryFromZippedFile.Name));
                        if (!fInfo.Directory.Exists) fInfo.Directory.Create();

                        FileStream file = fInfo.Create();
                        byte[] bufferFromZip = new byte[zipInputStream.Length];
                        zipInputStream.Read(bufferFromZip, 0, bufferFromZip.Length);
                        file.Write(bufferFromZip, 0, bufferFromZip.Length);
                        file.Close();
                    }
                    zipEntryFromZippedFile = zipInputStream.GetNextEntry();
                }
                zipInputStream.Close();
                File.Delete(@"C:\" + type + ".zip");
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        private bool DownLoadFile()
        {
            bool flag = false;
            try
            {
                var appName = "Test";

                System.Net.WebClient myWebClient = new System.Net.WebClient();
                var downAddress = HttpUtility.UrlDecode(myWebClient.DownloadString("http://baidu.com" + appName));

                if (!string.IsNullOrEmpty(downAddress))
                    myWebClient.DownloadFile(downAddress, @"C:\" + type + ".zip");
                else
                    return false;

                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        private void OpenUpgrade()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo myStartInfo = new System.Diagnostics.ProcessStartInfo();
                myStartInfo.FileName = @"C:\" + type + @"AutoUpgrade\setup.exe";
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo = myStartInfo;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText("目标程序运行失败，自动更新结束。\n");
            }
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            Upgrade();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OpenUpgrade();

            System.Environment.Exit(0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var res = DownLoadFile();
            if (res)
            {
                rtxtMsg.AppendText("解压目标程序...... \n");

                timer3.Interval = 1000;
                timer3.Enabled = true;
                timer3.Start();
                timer2.Stop();
            }
            else
            {
                rtxtMsg.AppendText("目标程序下载失败，请检查当前网络。\n");
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            var zipres = ZipOut();
            if (zipres)
            {
                rtxtMsg.AppendText("运行目标程序...... \n");
                rtxtMsg.AppendText("关闭更新程序...... \n");

                timer1.Interval = 2000;
                timer1.Enabled = true;
                timer1.Start();
                timer3.Stop();
            }
            else
            {
                rtxtMsg.AppendText("目标程序解压失败，自动更新结束。\n");
            }
        }
    }
}
