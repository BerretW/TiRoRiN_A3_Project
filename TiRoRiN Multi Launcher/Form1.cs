using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Drawing;


namespace TiRoRiN_Multi_Launcher
{
    public partial class Form1 : Form
    {
        string DBIP = "tirorin.cz";
        string DBPort = "3306";
        string DB = "launcher";
        string DBUser = "core";
        string DBPass = "A4ac9380?";
        string ver = "1.1";
        string arma2dir = "";
        string arma2oadir = "";
        string exefile = "ArmA2OA_BE.exe";
        string customip = "";
        string customport = "";
        string customgame = "";
        string customname = "";
        string custompara = "";
        string userID = "0";

        Process prcDayZ = new Process();
        //Server server = ServerQuery.GetServerInstance(EngineType.Source, "192.168.1.107", 27017);

        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";

        public Form1()
        {
            InitializeComponent();
        }
        public void version_check()
        {
            string mysqlver = "0";
            string connection = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
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
            command.CommandText = "SELECT * FROM data ";
            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    mysqlver = reader["ver"].ToString();


                }

            }
            catch
            {
                MessageBox.Show("Error while geting mysql parameters");
            }
            conn.Close();

            if (mysqlver == ver) MessageBox.Show("Your Launcher is up to Date.");
            else MessageBox.Show("You using outdated version of launcher. Please Update from WebPage http://tirorin.cz");
        }

        public void load_servers()
        {
            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from server";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string serverid = reader["id"].ToString();
                    string servername = reader["name"].ToString();
                    string serverip = reader["ip"].ToString();
                    string serverport = reader["port"].ToString();
                    string servergame = reader["game"].ToString();
                    string serverpara = reader["para"].ToString();
                    int serverintport = 0;
                    Int32.TryParse(serverport, out serverintport);
                    string online = "ONLINE";
                    //ServerInfo info = server.GetInfo();
                   // if (servergame == "CSS" | servergame == "CSGO") online = info.Map;
                    //if (TCPPingHost(serverip,serverintport )) online = "ONLINE";
                    dataGridView1.Rows.Add(serverid, serverpara, servername, servergame,serverip, serverport,online,"CONNECT");
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                    //MessageBox.Show(customname);
                    /*int serverintport = 0;
                    Int32.TryParse(customport, out serverintport);
                    string online = "offline";
                    if (TCPPingHost(customip,serverintport )) online = "ONLINE";
                    */
                    if (customname != "") dataGridView1.Rows.Add("999", custompara, customname, customgame, customip, customport, online, "CONNECT");
                     
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
            file.WriteLine(arma2dir + "|" + arma2oadir + "|" + textBox3.Text +"|"+textBox4.Text +"|DayZ|"+ textBox5.Text +"|"+ textBox6.Text);
            file.Close();
        }
        public string DayZ_construct_arguments(string parameters)
        {
            return " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + parameters + "\"";

        }
        public void dayz_run(string IP, string Port, string param)
        {
            arma2oadir = arma2oadir.Replace(@"
", "");
            
          
                
                string args = "";
                args += DayZ_construct_arguments(param);

                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + IP + " -port=" + Port + args;
                prcDayZ.Start();
                //System.Diagnostics.Process.Start(@"cd");

        }

        public void run_game(string game, string ip, string port, string para)
        {
            if (game == "DayZ") dayz_run(ip, port, para);
            //if (game == "DayZ") System.Diagnostics.Process.Start("steam://run/33930// -skipIntro -nosplash -noFilePatching -world chernarus -mod=D:\\Steam\\SteamApps\\common\\Arma%202;expansion;" + para + " -connect=" + ip + " -port=" + port);
            if (game == "Arma3") System.Diagnostics.Process.Start("steam://run/107410//-mod=" + para + " -connect=" + ip + " -port=" + port + " -nosplash -skipIntro -noPause");

            if (game == "CSS") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "CSGO") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "SE") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "TF2") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "HL2DM") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "L4D2") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            version_check();
            load_file_config();
            load_servers();
            
           /* foreach (DataGridViewRow row in dataGridView1.Rows)
                if (row.Cells[6].Value == "ONLINE")
                {
                    row.DefaultCellStyle.BackColor = Color.Blue;
                }
            */
            //if (PingHost("tirorin.eu", 80)) MessageBox.Show("je to OKEy");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                string selgame = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                string selip = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                string selport = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                string selpara = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                run_game(selgame, selip, selport, selpara);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            settings settingsForm = new settings();

            // Show the settings form
            settingsForm.Show();
        }

        public static bool TCPPingHost(string _HostURI, int _PortNumber)
        {
            try
            {
                TcpClient client = new TcpClient(_HostURI, _PortNumber);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //return UDPPingHost(_HostURI,_PortNumber);
            }
        }
        public static bool UDPPingHost(string _HostURI, int _PortNumber)
        {
            try
            {
                UdpClient client = new UdpClient(_HostURI, _PortNumber);
                return true;
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Error pinging host:'" + _HostURI + ":" + _PortNumber.ToString() + "'");
                return false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            download downloadForm = new download();

            // Show the settings form
            downloadForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://dayz.tirorin.cz/?page_id=9");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.eu");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/tirorin.eu");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userID = "0";
            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE name = '" + textBox1.Text+"'";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    userID = reader["id"].ToString();
                    string username = reader["name"].ToString();
                    string userpass = reader["pass"].ToString();
                    string usermail = reader["email"].ToString();
                    if (userID != "0")
                    {
                        bool passok = false;
                        if (textBox2.Text == userpass) passok = true;
                        else MessageBox.Show("Wrong Password");
                        if (passok) MessageBox.Show("You are loged in");
                        button4.Visible = true;
                    }

                    
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            if (userID == "0") MessageBox.Show("Wrong UserName");
            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            load_file_config();
            write_file_config();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            load_file_config();
            load_servers();
        }

    }
}
