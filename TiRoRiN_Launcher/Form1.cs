using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Text;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.Net.Sockets;
using Chilkat;
using MySql.Data.MySqlClient;


namespace TiRoRiN_Launcher
{    
    public partial class Form1 : Form
    {
        string arma2dir = "";
        string arma2oadir = "";
        string exedir = "";
        string exefile = "ArmA2OA_BE.exe";
        string mods1 = "";
        string mods2 = "";
        string ip1 = "";
        string port1 = "";
        string ip2 = "";
        string port2 = "";
        string ip3 = "";
        string port3 = "";
        string mods3 = "";
        string ip4 = "";
        string port4 = "";
        string mods4 = "";
        string ip5 = "";
        string port5 = "";
        string mods5 = "";
        string config;
        string arguments1;
        string arguments2;
        string arguments3;
        string arguments4;
        string arguments5;
        string exetorun;
        string version;
        string ver = "0.12";
        string mysqlver;
        string appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\TiRoRiN\";        
        string DBServer = "tirorin.eu";
        string DBPort = "3306";
        string DB = "web";
        string DBUser = "launcher";
        string DBPass = "1234";
        Dictionary<string, string> strArmaOARegLocation = new Dictionary<string, string>()
        {
            { "x86", "SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA"},
            { "x64", "SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA" }
        };
        Dictionary<string, string> strArmaRegLocation = new Dictionary<string, string>()
        {
            { "x86", "SOFTWARE\\Bohemia Interactive Studio\\ArmA 2"},
            { "x64", "SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2" }
        };


        Process prcDayZ = new Process();
        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";
        public Form1()
        {
            InitializeComponent();            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            dll_check();           
            //update_check();
            if (File.Exists(globalAdresar + @"\MySql.Data.dll")) update_check();
            label3.Text = version;
            textBox2.Text = arma2dir;
            textBox3.Text = arma2oadir;
            construct_arguments();                     
        }
        private void button1_Click(object sender, EventArgs e)
        {           
            if (file_check(mods1))
            {
                construct_arguments();
                string args = "";
                args += arguments1;
                exetorun = exedir + exefile + arguments1;
                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exedir + exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + ip1 + " -port=" + port1 + args;
                prcDayZ.Start();
                //textBox1.Text = prcDayZ.StartInfo.Arguments;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {            
             if (file_check(mods2))
             {
                 construct_arguments();
                 string args = "";
                 args += arguments2;
                 exetorun = exedir + exefile + arguments1;
                 prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                 prcDayZ.StartInfo.FileName = exedir + exefile;
                 prcDayZ.StartInfo.Arguments = "0 0 -connect=" + ip2 + " -port=" + port2 + args;
                 prcDayZ.Start();
             }            
        }
        //fnc
        public static bool PingHost(string _HostURI, int _PortNumber)
        {
            System.Net.Sockets.TcpClient client = new TcpClient();
            try
            {
                client.Connect(_HostURI, _PortNumber);
                MessageBox.Show("Connection open, host active");
                return true;
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Connection could not be established due to: \n" + ex.Message);
                return false;
            }
            finally
            {
                client.Close();
            }
        }
        void construct_arguments()
        {             
            arguments1 = " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;"+ mods1 + "\"";
            arguments2 = " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + mods2 + "\"";
            arguments3 = " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + mods3 + "\"";
            arguments4 = " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + mods4 + "\"";
            arguments5 = " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + mods5 + "\"";
        }
        private void unrar(string file)
        {
            MessageBox.Show("Unpacking file");
            Chilkat.Rar rar = new Chilkat.Rar();
            rar.Open(file);
            bool success = rar.Unrar(arma2oadir);
            if (success == true) MessageBox.Show("Unpacking complete");
            else
                MessageBox.Show(rar.LastErrorText);
        }
        public void update_check()
        {
            if (File.Exists(appdir + @"\config.cfg"))
            {
                using (StreamReader nacteni = new StreamReader(appdir + "config.cfg", Encoding.Default, true))
                {
                    config = nacteni.ReadToEnd();
                    string[] split1 = new string[] { "\"" };
                    string[] split2 = config.Split(split1, StringSplitOptions.None);
                    ip1 = split2[1];
                    port1 = split2[3];
                    ip2 = split2[5];
                    port2 = split2[7];
                    mods1 = split2[9];
                    mods2 = split2[11];
                    arma2dir = split2[13];
                    arma2oadir = split2[15];
                    version = split2[17];
                    ip3 = split2[19];
                    port3 = split2[21];
                    mods3 = split2[23];
                    ip3 = split2[19];
                    port3 = split2[21];
                    mods3 = split2[23];
                    ip3 = split2[19];
                    port3 = split2[21];
                    mods3 = split2[23];
                }
            }
            else write_config();

            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText = "SELECT * FROM launcher ";
            try
            {
                MySqlDataReader reader = command1.ExecuteReader();
                while (reader.Read())
                {
                    mods1 = reader["server1_mods"].ToString();
                    mods2 = reader["server2_mods"].ToString();
                    mods3 = reader["server3_mods"].ToString();
                    mods4 = reader["server4_mods"].ToString();
                    mods5 = reader["server5_mods"].ToString();
                    ip1 = reader["server1_ip"].ToString();
                    ip2 = reader["server2_ip"].ToString();
                    ip3 = reader["server3_ip"].ToString();
                    ip4 = reader["server4_ip"].ToString();
                    ip5 = reader["server5_ip"].ToString();
                    port1 = reader["server1_port"].ToString();
                    port2 = reader["server2_port"].ToString();
                    port3 = reader["server3_port"].ToString();
                    port4 = reader["server4_port"].ToString();
                    port5 = reader["server5_port"].ToString();
                    mysqlver = reader["ver"].ToString();

                }
                
                write_config();
                if (ver != mysqlver)
                    MessageBox.Show(@"New laucher is available! http://tirorin.eu/");
                else
                {
                    MessageBox.Show("Launcher is up to date.");
                }
                //MessageBox.Show("Server1 Mods: " + onemods + ". Server2 Mods: " + twomods);
            }
            catch
            {
                MessageBox.Show("Error while geting mod list");
            }
            conn.Close();
        }
        private string GetArmaPath()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strArmaRegLocation[GetBitness()]);
                if (subKey != null)
                    return subKey.GetValue("MAIN").ToString();
                else return "";
            }
            catch
            {
                return "";
            }
        }

        private string GetArmaOAPath()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strArmaOARegLocation[GetBitness()]);
                if (subKey != null)
                    return subKey.GetValue("MAIN").ToString();
                else return "";
            }
            catch
            {
                return "";
            }
        }

        private string GetBitness()
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return "x86";
                case 8:
                    return "x64";
                default:
                    return "x86";
            }
        }

        public void write_config()
        {
            string lines = "Server1 ip = \"" + ip1 + "\" Server1 port = \"" + port1 + "\"Server2 ip = \"" + ip2 + "\"Server2 port = \"" + port2 + "\"Server1 mods = \"" + mods1 + "\"Server2 mods = \"" + mods2 + "\"Arma2 Dir = \"" + arma2dir + "\" Arma 2 OA = \"" + arma2oadir + "\" Version = \"" + ver + "\" Server3 IP = \"" + ip3 + "\" Server3 port = \"" + port3 + "\" Server3 Mods = \"" + mods3 + "\" Server4 IP = \"" + ip4 + "\" Server4 port = \"" + port4 + "\" Server4 Mods = \"" + mods4 + "\" Server5 IP = \"" + ip5 + "\" Server5 port = \"" + port5 + "\" Server5 Mods = \"" + mods5 + "\"";
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(appdir + "config.cfg");
            file.WriteLine(lines);
            file.Close();
        }

        public bool file_check( string mods)
        {
            bool ok = false;
            bool modok = true;
            bool arma2ok = false;
            bool arma2oaok = false;
            if (mods != null)
            {
                if (mods.Contains(";"))
                {
                    string[] mod = mods.Split(';');
                    //mod[0] = mod[0].Substring(0, mod[0].Length - 1);
                    foreach (string mod1 in mod)
                    {

                        if (modok)
                        {

                            if (!Directory.Exists(arma2oadir + @"\" + mod1))
                            {
                                MessageBox.Show("You dont have this mod " + mod1 + ". Please install modpack for this server.");
                                modok = false;
                            }
                            else
                            {

                                modok = true;
                            }
                        }
                    }

                }
                else
                {
                    if (Directory.Exists(arma2oadir + @"\" + mods)) modok = true;
                    else
                    {
                        MessageBox.Show("Please install " + mods);
                        modok = false;
                    }
                }
            }
            else { modok = true; }


            if (File.Exists(arma2dir + @"\arma2.exe")) arma2ok = true;
            else
            {
                MessageBox.Show("Please set the correct ArmA directory");
                arma2ok = false;
            }
            if (File.Exists(arma2oadir + @"\arma2oa_be.exe")) arma2oaok = true;
            else
            {
                MessageBox.Show("Please set the correct ArmA 2 OA directory");
                arma2oaok = false;
            }         
            if (modok & arma2ok & arma2oaok) return ok = true;
            else return ok = false;
        }

        public bool mod_download_check(string mods)
        {
            
            bool modok = false;

            if (mods.Contains(";"))
            {
                string[] mod = mods.Split(';');
                //mod[0] = mod[0].Substring(0, mod[0].Length - 1);
                foreach (string mod1 in mod)
                {

                     
                    if (Directory.Exists(arma2oadir + @"\" + mod1))
                    {
                        modok = true;
                        MessageBox.Show("You have currenty instaled this mod: " + mod1+". Please delete this mod.");
                        
                    }
                    else
                    {

                        modok = false;
                    }
                }

            }
            else
            {
                if (Directory.Exists(arma2oadir + @"\" + mods))
                {
                    modok = true;
                    MessageBox.Show("You have currenty instaled this mod: " + mods + ". Please delete this mod.");
                    //var confirmResult = MessageBox.Show("You have already instalated " + mods + ". Can i delete this file ??",
                    //    "Confirm Delete!!",
                    //    MessageBoxButtons.YesNo);
                    //if (confirmResult == DialogResult.Yes) Directory.Delete(arma2oadir + @"\" + mods, true);
                    //modok = true;
                    
                }
                else
                {

                    modok = false;
                }

            }
            return modok;
        }

        public void dll_check()
        {
            bool isExists = System.IO.Directory.Exists(appdir);
            if (!isExists) System.IO.Directory.CreateDirectory(appdir);
            if (!File.Exists(globalAdresar + @"\MySql.Data.dll"))
            {
                 WebClient webClient = new WebClient();
                 webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedDLL);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/dll/MySql.Data.dll"), globalAdresar + "MySql.Data.dll");
            }
            if (!File.Exists(globalAdresar + @"\ChilkatDotNet4.dll"))
            {
                WebClient webClient = new WebClient();              
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/dll/ChilkatDotNet4.dll"), globalAdresar + "ChilkatDotNet4.dll");
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
        //Folder Browse
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                arma2dir = fbd.SelectedPath;
                textBox2.Text = arma2dir;
                construct_arguments();
                write_config();
            }            
        }       
        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;           
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                arma2oadir  = fbd.SelectedPath;
                textBox3.Text = arma2oadir;
                construct_arguments();
                write_config();
            }
        }
        //Registry read
        private void button5_Click(object sender, EventArgs e)
        {
            arma2dir = GetArmaPath();
            arma2oadir = GetArmaOAPath();            
            construct_arguments();
            textBox2.Text = arma2dir;
            textBox3.Text = arma2oadir;
            write_config();
        }
        //Download buttons
        
        private void button7_Click(object sender, EventArgs e)
        {
            
            if (file_check(null))
            {
                if (!mod_download_check(mods1))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/@DayZ_Epoch.rar"), appdir + "data.rar");
                }
                
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {            
            if (file_check(null))
            {
                if (!mod_download_check(mods2))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/@Dayz.rar"), appdir + "data.rar");

                }
                
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (file_check(null))
            {
                if (!mod_download_check(mods3))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/namalsk.rar"), appdir + "data.rar");
                }
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (file_check(null))
            {
                if (!mod_download_check(mods4))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("http://tirorin.eu/data/@DayZ2017.zip"), appdir + "data.rar");
                }

            }
        }
       //Download fnc
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            unrar(appdir + "data.rar");                       
        }
        private void CompletedDLL(object sender, AsyncCompletedEventArgs e)
        {
            //ZipFile.ExtractToDirectory(globalAdresar + "tirorin.zip", arma2oadir);
            update_check();
        }
        private void Completed_update(object sender, AsyncCompletedEventArgs e)
        {
            //ZipFile.ExtractToDirectory(globalAdresar + "tirorin.zip", arma2oadir);
            System.Diagnostics.Process.Start(globalAdresar + "arma2_update.exe");
        }
        private void button9_Click(object sender, EventArgs e)
        {
            update_check();              
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }
        private void button10_Click(object sender, EventArgs e)
        {          
        }
        private void label3_Click(object sender, EventArgs e)
        {
        }
        private void button11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.tirorin.eu");
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {
        }                     
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
        private void button14_Click(object sender, EventArgs e)
        {          
            if (file_check(mods3))
            {
                construct_arguments();
                string args = "";
                args += arguments3;
                exetorun = exedir + exefile + arguments3;
                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exedir + exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + ip3 + " -port=" + port3 + args;
                prcDayZ.Start();
                //textBox1.Text = prcDayZ.StartInfo.Arguments;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (file_check(mods4))
            {
                construct_arguments();
                string args = "";
                args += arguments4;
                exetorun = exedir + exefile + arguments4;
                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exedir + exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + ip4 + " -port=" + port4 + args;
                prcDayZ.Start();
                //textBox1.Text = prcDayZ.StartInfo.Arguments;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (file_check(mods5))
            {
                construct_arguments();
                string args = "";
                args += arguments5;
                exetorun = exedir + exefile + arguments5;
                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exedir + exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + ip5 + " -port=" + port5 + args;
                prcDayZ.Start();
                //textBox1.Text = prcDayZ.StartInfo.Arguments;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
