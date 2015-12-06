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

namespace TiRoRiN_New_Launcher
{
    public partial class install : Form
    {
        string ver = "0.16";
        string DBServer = "tirorin.cz";
        string DBPort = "3306";
        string DB = "launcher";
        string DBUser = "core";
        string DBPass = "A4ac9380?";
        Process prcDayZ = new Process();

        string appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\TiRoRiN\";
        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";

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

        /// <summary>
        /// ///////////////////////////////////////////////mysql parameters
        /// </summary>

        string server1Game;
        string server1Para;
        string server2Game;
        string server2Para;
        string server3Game;
        string server3Para;
        string server4Game;
        string server4Para;
        string server5Game;
        string server5Para;
        string server6Game;
        string server6Para;
        string server1Files;
        string server2Files;
        string server3Files;
        string server4Files;
        string server5Files;
        string server6Files;
        string server1Name;
        string server2Name;
        string server3Name;
        string server4Name;
        string server5Name;
        string server6Name;
        /// <summary>
        /// //////////////////////////////////////////End of mysql parameters
        /// </summary>
        //////////////////////////////////////////////File settings
        string Server1Para;
        string Server2Para;
        string Server3Para;
        string Server4Para;
        string Server5Para;
        string Server6Para;
        /// <summary>
        /// /////////////////////////////////////////End of file settings
        /// </summary>
        ///////////////////////////////////////////// Local Var
        string arma2dir = "";
        string arma2oadir = "";




        public install()
        {
            InitializeComponent();
        }

        public void load_mysql()
        {
            string connection = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM launcher ";
            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {


                   
                    server1Para = reader["server1Para"].ToString();
                    server1Game = reader["server1Game"].ToString();
                    server1Files = reader["server1Files"].ToString();
                    server2Para = reader["server2Para"].ToString();
                    server2Game = reader["server2Game"].ToString();
                    server2Files = reader["server2Files"].ToString();
                    server3Para = reader["server3Para"].ToString();
                    server3Game = reader["server3Game"].ToString();
                    server3Files = reader["server3Files"].ToString();
                    server4Para = reader["server4Para"].ToString();
                    server4Game = reader["server4Game"].ToString();
                    server4Files = reader["server4Files"].ToString();
                    server5Para = reader["server5Para"].ToString();
                    server5Game = reader["server5Game"].ToString();
                    server5Files = reader["server5Files"].ToString();
                    server6Para = reader["server6Para"].ToString();
                    server6Game = reader["server6Game"].ToString();
                    server6Files = reader["server6Files"].ToString();
                    server1Name = reader["server1Name"].ToString();
                    server2Name = reader["server2Name"].ToString();
                    server3Name = reader["server3Name"].ToString();
                    server4Name = reader["server4Name"].ToString();
                    server5Name = reader["server5Name"].ToString();
                    server6Name = reader["server6Name"].ToString();
                    button1.Text = "Download All Files for " + server1Name;
                    button2.Text = "Download All Files for " + server2Name;
                    button3.Text = "Download All Files for " + server3Name;
                    button4.Text = "Download All Files for " + server4Name;
                    button5.Text = "Download All Files for " + server5Name;
                    button6.Text = "Download All Files for " + server6Name;
                }

            }
            catch
            {
                MessageBox.Show("Error while geting mysql parameters");
            }
            conn.Close();
        }

        public void load_file_config()
        {
            if (File.Exists(globalAdresar + @"\config.cfg"))
            {
                using (StreamReader nacteni = new StreamReader(globalAdresar + "config.cfg", Encoding.Default, true))
                {
                    string config = nacteni.ReadToEnd();
                    string[] split1 = new string[] { "|&|" };
                    string[] split2 = config.Split(split1, StringSplitOptions.None);
                    Server1Para = split2[0];
                    Server2Para = split2[1];
                    Server3Para = split2[2];
                    Server4Para = split2[3];
                    Server5Para = split2[4];
                    Server6Para = split2[5];
                }
                //MessageBox.Show("config loaded");

                if (server1Game == "DayZ") DayZPara_Split(Server1Para);
                if (server2Game == "DayZ") DayZPara_Split(Server2Para);
                if (server3Game == "DayZ") DayZPara_Split(Server3Para);
                if (server4Game == "DayZ") DayZPara_Split(Server4Para);
                if (server5Game == "DayZ") DayZPara_Split(Server5Para);
                if (server6Game == "DayZ") DayZPara_Split(Server6Para);
            }
         
        }

        public void DayZPara_Split(string file_para)
        {
            string[] split = new string[] { "|" };
            string[] split1 = file_para.Split(split, StringSplitOptions.None);
            arma2dir = split1[0];
            arma2oadir = split1[1];
            //MessageBox.Show(arma2oadir);

        }

        private void install_Load(object sender, EventArgs e)
        {
            load_mysql();
            load_file_config();
        }

        //Download fnc
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private void dayz_Completed(object sender, AsyncCompletedEventArgs e)
        {
            dayz_unrar(appdir + "data.rar");
        }


        private void dayz_unrar(string file)
        {
            MessageBox.Show("Unpacking file");
            Chilkat.Rar rar = new Chilkat.Rar();
            rar.Open(file);
            bool success = rar.Unrar(arma2oadir);
            if (success == true) MessageBox.Show("Unpacking complete");
            else
                MessageBox.Show(rar.LastErrorText);
        }

        public bool dayz_file_check(string mods)
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
                        MessageBox.Show("You have currenty instaled this mod: " + mod1 + ". Please delete this mod.");
                        MessageBox.Show("You have currenty instaled this mod: " + mods + ". Please delete this mod.");
                        var confirmResult = MessageBox.Show("You have already instalated " + mod1 + "In Directory " + arma2oadir + @"\" + mod1 + ". Can i delete this file ??",
                            "Confirm Delete!!",
                            MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes) Directory.Delete(arma2oadir + @"\" + mod1, true);
                        modok = true;
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
                    var confirmResult = MessageBox.Show("You have already instalated " + mods + "In Directory " + arma2oadir + @"\" + mods + ". Can i delete this file ??",
                        "Confirm Delete!!",
                        MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes) Directory.Delete(arma2oadir + @"\" + mods, true);
                    modok = true;

                }
                else
                {

                    modok = false;
                }

            }
            return modok;
        }

        public void download_unrar(string link, string game,string para)
        {
            if (game == "DayZ")
            {
                if (dayz_file_check(null))
                {
                    if (!mod_download_check(para))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(dayz_Completed);
                        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        webClient.DownloadFileAsync(new Uri(link), appdir + "data.rar");
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            download_unrar(server1Files, server1Game, server1Para);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            download_unrar(server2Files, server2Game, server2Para);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            download_unrar(server3Files, server3Game, server3Para);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            download_unrar(server4Files, server4Game, server4Para);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            download_unrar(server5Files, server5Game, server5Para);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            download_unrar(server6Files, server6Game, server6Para);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz/data/@DayZ_Epoch.rar");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz/data/@Dayz.rar");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz/data/@CBA_CO.rar");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz/data/MultiMC.rar");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz/data/MultiMC-warez.rar");
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
