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
using MySql.Data.MySqlClient;

namespace TiRoRiN_New_Launcher
{
    public partial class settings : Form
    {

        string DBServer = "tirorin.cz";
        string DBPort = "3306";
        string DB = "launcher";
        string DBUser = "core";
        string DBPass = "A4ac9380?";

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
        /// ///////////////////////////////////////////////mysql parameters
        /// </summary>

        string server1Game;

        string server2Game;

        string server3Game;

        string server4Game;

        string server5Game;

        string server6Game;
        /// <summary>
        /// //////////////////////////////////////////End of mysql parameters
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


        public settings()
        {
            InitializeComponent();
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
                    //MessageBox.Show(Server1Para);
                }
            }
            else write_file_config();
        }

        public void write_file_config()
        {
            if (server1Game == "DayZ") Server1Para = arma2dir + "|" + arma2oadir;
            if (server2Game == "DayZ") Server2Para = arma2dir + "|" + arma2oadir;
            if (server3Game == "DayZ") Server3Para = arma2dir + "|" + arma2oadir;
            if (server4Game == "DayZ") Server4Para = arma2dir + "|" + arma2oadir;
            if (server5Game == "DayZ") Server5Para = arma2dir + "|" + arma2oadir;
            if (server6Game == "DayZ") Server6Para = arma2dir + "|" + arma2oadir;
            System.IO.StreamWriter file = new System.IO.StreamWriter(globalAdresar + "config.cfg");
            file.WriteLine(Server1Para + "|&|" + Server2Para + "|&|" + Server3Para + "|&|" + Server4Para + "|&|" + Server5Para + "|&|" + Server6Para);
            file.Close();
        }

        public void get_server_types()
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

                    server1Game = reader["server1Game"].ToString();
                    server2Game = reader["server2Game"].ToString();
                    server3Game = reader["server3Game"].ToString();
                    server4Game = reader["server4Game"].ToString();
                    server5Game = reader["server5Game"].ToString();
                    server6Game = reader["server6Game"].ToString();


                }

            }
            catch
            {
                MessageBox.Show("Error while geting mysql parameters");
            }
            conn.Close();

        }

        public void DayZPara_Split(string file_para)
        {
            string[] split = new string[] { "|" };
            string[] split1 = file_para.Split(split, StringSplitOptions.None);
            arma2dir = split1[0];
            arma2oadir = split1[1];
            textBox1.Text = arma2dir;
            textBox2.Text = arma2oadir;


        }

        private void settings_Load(object sender, EventArgs e)
        {
            load_file_config();
            get_server_types();
            if (server1Game == "DayZ") DayZPara_Split(Server1Para);
            if (server2Game == "DayZ") DayZPara_Split(Server2Para);
            if (server3Game == "DayZ") DayZPara_Split(Server3Para);
            if (server4Game == "DayZ") DayZPara_Split(Server4Para);
            if (server5Game == "DayZ") DayZPara_Split(Server5Para);
            if (server6Game == "DayZ") DayZPara_Split(Server6Para);
            textBox1.Text = arma2dir;
            textBox2.Text = arma2oadir;
           
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


        private void button3_Click(object sender, EventArgs e)
        {
            arma2dir = GetArmaPath();
            arma2oadir = GetArmaOAPath();
            textBox1.Text = arma2dir;
            textBox2.Text = arma2oadir;
            write_file_config();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            arma2dir = textBox1.Text;
            arma2oadir = textBox2.Text;
            write_file_config();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                arma2dir = fbd.SelectedPath;
                textBox1.Text = arma2dir;
                
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                arma2oadir = fbd.SelectedPath;
                textBox2.Text = arma2oadir;

            } 
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
