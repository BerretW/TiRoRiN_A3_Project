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

namespace TiRoRiN_Multi_Launcher
{
    public partial class settings : Form
    {
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

        string arma2dir = "";
        string arma2oadir = "";
        string customip = "";
        string customport = "";
        string customgame = "";
        string customname = "";
        string custompara = "";
        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";

        public void load_file_config()
        {
            if (File.Exists(globalAdresar + @"\config.cfg"))
            {
                using (StreamReader nacteni = new StreamReader(globalAdresar + "config.cfg", Encoding.Default, true))
                {
                    string config = nacteni.ReadToEnd();
                    string[] split1 = new string[] { "|" };
                    string[] split2 = config.Split(split1, StringSplitOptions.None);
                    arma2dir = split2[0];
                    arma2oadir = split2[1];

                    customip = split2[2];
                    customport = split2[3];
                    customgame = split2[4];
                    customname = split2[5];
                    custompara = split2[6];
                    
                    
                    

                }
                //MessageBox.Show("config loaded");

            }

            if (arma2dir == "" | arma2oadir == "") MessageBox.Show("Please set Arma2 and Arma2 OA directorys in settings window");
            else write_file_config();
        }

        public void write_file_config()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(globalAdresar + "config.cfg");
            //|192.168.1.204|2302|DayZ|testdoma|@dayz_epoch;
            file.WriteLine(arma2dir + "|" + arma2oadir + "|" + customip + "|" + customport + "|DayZ|" + customname + "|" + custompara);
            file.Close();
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

        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            load_file_config();
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
    }
}
