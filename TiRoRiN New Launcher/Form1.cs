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
using Chilkat;
using MySql.Data.MySqlClient;
namespace TiRoRiN_New_Launcher
{
    public partial class Form1 : Form
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
    string server1IP;
    string server1Port;
    string server1Para;
    string server1Game;
    string server2IP;
    string server2Port;
    string server2Para;
    string server2Game;
    string server3IP;
    string server3Port;
    string server3Para;
    string server3Game;
    string server4IP;
    string server4Port;
    string server4Para;
    string server4Game;
    string server5IP;
    string server5Port;
    string server5Para;
    string server5Game;
    string server6IP;
    string server6Port;
    string server6Para;
    string server6Game;
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
        string exefile = "ArmA2OA_BE.exe";

       


        public Form1()
        {
            InitializeComponent();
            IRC_connect();
        }

       
/// //////////////////////////////////////////////////IRC part
        bool loged_in = false;
        private string irc_Server = "irc.rizon.net";
        private int irc_port = 6667;
        bool connected = false;

        public string irc_desc = "TiRoRiN Client User";
        public string irc_channel = "#TiRoRiN";
        public string irc_Jmeno = "TiRoRiN User";
        public string[] irc_user_list;
        private TcpClient irc;
        NetworkStream stream = null;
        StreamReader sr = null;
        StreamWriter sw = null;

        private void IRC_connect()
        {


            irc = new TcpClient(irc_Server, irc_port);
            stream = irc.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);

        }

        private void irc_client()
        {
            irc_Jmeno = textBox3.Text;
            sw.WriteLine("USER " + irc_Jmeno + " 0 * :" + irc_desc); //autorizace uzivatele
            sw.Flush();
            sw.WriteLine("NICK " + irc_Jmeno);                   //autoorizace nicku
            sw.Flush();
            sw.WriteLine("JOIN " + irc_channel);                //pripojeni do kanalu
            sw.Flush();
            string getString = null;
            try
            {

                while ((getString = sr.ReadLine()) != null)     //vypisovani vsech hodnot ze streamu do textboxu
                {

                    //if (!getString.Contains("PING")) ;
                    if (getString.Contains("PING"))
                    {
                        sw.WriteLine("PONG irc.broke-it.com");
                        sw.Flush();
                        //sw.WriteLine("PRIVMSG " + channel + " :PONG");
                        //sw.Flush();

                    }

                    richTextBox1.Invoke(new Action(() =>
                    {
                        string split_jmeno = null;
                        string split_msg = null;

                        string getstring2 = getString;
                        string getstring3 = getString;

                        //zachytavani

                        try
                        {

                            // parsovani normalni zpravy
                            string[] split1 = new string[] { irc_channel + " :" };

                            string[] split2 = getString.Split(split1, StringSplitOptions.None);
                            split_msg = split2[1];
                            split_jmeno = split2[0];

                            split1 = new string[] { "!" };
                            split2 = split_jmeno.Split(split1, StringSplitOptions.None);
                            split_jmeno = split2[0];

                            split1 = new string[] { ":" };
                            split2 = split_jmeno.Split(split1, StringSplitOptions.None);
                            split_jmeno = split2[1];
                        }
                        catch { }


                        ////Parsování seznamu uzivatelu
                        try
                        {

                            if (getString.Contains("353"))
                            {
                                string[] split1 = new string[] { " 353 "+ irc_Jmeno + " = "+ irc_channel + " :" };
                                ///:irc.x2x.cc 353 adolf01 = #TiRoRiN :adolf01 adolf01|2
                                string[] split2 = getString.Split(split1, StringSplitOptions.None);
                                string split_list = split2[1];


                                split1 = new string[] { " " };
                                string[] list = split_list.Split(split1, StringSplitOptions.None);
                                rich_playerlist(list);

                            }
                        }
                        catch { }

                        if (getstring2.Contains(" 366 " + irc_Jmeno)) connected = true;



                        //rich_write(richTextBox1, null, getstring3);

                        if (split_jmeno != null) // text do global chatu
                        {
                            if (!getstring2.Contains(" 353 " + irc_Jmeno + " = " + irc_channel + " :") & !getstring2.Contains(" 366 " + irc_Jmeno))
                            {
                                rich_write(richTextBox1, split_jmeno, split_msg);
                            }
                            
                        }
                        

                    }
                    ));
                }
            }
            catch
            {
                MessageBox.Show("Připojení se nezdařilo. Prosím restartuje aplikaci.");
                ActiveForm.Close();

            }

        }

        void rich_write(RichTextBox box, string from, string message)
        {
            DateTime datumCas = DateTime.Now;
            box.Text = datumCas + " " + from + ": " + message + Environment.NewLine + box.Text;
        }

        void rich_playerlist(string[] users)
        {
            richTextBox2.Text = "";
            int user = users.Length;
            while (user != 0)
            {
                user = user -1;
                richTextBox2.Text = richTextBox2.Text + users[user] + Environment.NewLine;
            }
        }

        public void send_string(string target, string from, TextBox message, string text, RichTextBox box)  //komu, od koho, zprava, richtextbox
        {
            if (text == null)
            {
                sw.WriteLine("PRIVMSG " + target + " :" + message.Text);
                sw.Flush();
                DateTime datumCas = DateTime.Now;
                box.Text = datumCas + " " + from + ": " + message.Text + Environment.NewLine + box.Text;
                message.Text = null;
            }
            else
            {
                sw.WriteLine("PRIVMSG " + target + " :" + text);
                sw.Flush();
                DateTime datumCas = DateTime.Now;
                box.Text = datumCas + " " + from + ": " + text + Environment.NewLine + box.Text;

            }
        }

//////////////////////////////////////////////////////End of IRC part



        public bool login(string username, string pass)
        {
            if (username != "" & pass != "")
            {

                string mysql_pass = "";
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
                command.CommandText = "SELECT * FROM users WHERE `User` = '" + username + "'";
                try
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {


                        mysql_pass = reader["Pass"].ToString();

                    }

                }
                catch
                {
                    MessageBox.Show("Wrong Name or Password");
                    return false;
                }
                conn.Close();
                if (mysql_pass == pass) return true;
                else return false;

            }
            else return false;

        }

        public void irc_send_cmd(string channel, string cmd)
        {
            sw.WriteLine(cmd);
            sw.Flush();
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

                    server1IP = reader["server1IP"].ToString();
                    server1Port = reader["server1Port"].ToString();
                    server1Para = reader["server1Para"].ToString();
                    server1Game = reader["server1Game"].ToString();
                    server2IP = reader["server2IP"].ToString();
                    server2Port = reader["server2Port"].ToString();
                    server2Para = reader["server2Para"].ToString();
                    server2Game = reader["server2Game"].ToString();
                    server3IP = reader["server3IP"].ToString();
                    server3Port = reader["server3Port"].ToString();
                    server3Para = reader["server3Para"].ToString();
                    server3Game = reader["server3Game"].ToString();
                    server4IP = reader["server4IP"].ToString();
                    server4Port = reader["server4Port"].ToString();
                    server4Para = reader["server4Para"].ToString();
                    server4Game = reader["server4Game"].ToString();
                    server5IP = reader["server5IP"].ToString();
                    server5Port = reader["server5Port"].ToString();
                    server5Para = reader["server5Para"].ToString();
                    server5Game = reader["server5Game"].ToString();
                    server6IP = reader["server6IP"].ToString();
                    server6Port = reader["server6Port"].ToString();
                    server6Para = reader["server6Para"].ToString();
                    server6Game = reader["server6Game"].ToString();
                    server1Name = reader["server1Name"].ToString();
                    server2Name = reader["server2Name"].ToString();
                    server3Name = reader["server3Name"].ToString();
                    server4Name = reader["server4Name"].ToString();
                    server5Name = reader["server5Name"].ToString();
                    server6Name = reader["server6Name"].ToString();
                    button1.Text = "Play " + server1Name;
                    button2.Text = "Play " + server2Name;
                    button3.Text = "Play " + server3Name;
                    button4.Text = "Play " + server4Name;
                    button5.Text = "Play " + server5Name;
                    button6.Text = "Play " + server6Name;
            }

            }
            catch
            {
                MessageBox.Show("Error while geting mysql parameters");
            }
            conn.Close();
        }
        
        public void version_check()
        {
            string mysqlver = "0";
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

        public void DayZPara_Split(string file_para)
        {
            string[] split = new string[] { "|" };
            string[] split1 = file_para.Split(split, StringSplitOptions.None);
            arma2dir = split1[0];
            arma2oadir = split1[1];
            //MessageBox.Show(arma2oadir);

        }

        public bool file_check(string mods)
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
            else write_file_config();
        }

        public void write_file_config()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(globalAdresar + "config.cfg");
            file.WriteLine(Server1Para + "|&|" + Server2Para + "|&|" + Server3Para + "|&|" + Server4Para + "|&|" + Server5Para + "|&|" + Server6Para);
            file.Close();
        }

        public string DayZ_construct_arguments(string parameters)
        {
            return " -skipIntro -nosplash -noFilePatching -world chernarus \"-mod=" + arma2dir + ";expansion;" + parameters + "\"";
            
        }

        public void dayz_run(string IP, string Port, string param)
        {
            if (file_check(param))
            {

                string args = "";
                args += DayZ_construct_arguments(param);

                prcDayZ.StartInfo.WorkingDirectory = arma2oadir;
                prcDayZ.StartInfo.FileName = exefile;
                prcDayZ.StartInfo.Arguments = "0 0 -connect=" + IP + " -port=" + Port + args;
                prcDayZ.Start();
                //textBox1.Text = prcDayZ.StartInfo.Arguments;
            }
        }

        public void run_game(string ip, string port, string game, string para)
        {
            if (game == "DayZ") dayz_run(ip, port, para);
            //if (game == "DayZ") System.Diagnostics.Process.Start("steam://run/33930//-mod="+para+" -connect="+ip+" -port="+port+" -nosplash -skipIntro -noPause");
            if (game == "Arma3") System.Diagnostics.Process.Start("steam://run/107410//-mod=" + para + " -connect=" + ip + " -port=" + port + " -nosplash -skipIntro -noPause");
        
            if (game == "CSS") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "CSGO") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
            if (game == "SE") System.Diagnostics.Process.Start("steam://connect/" + ip + ":" + port);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Create a new instance of the Form2 class
            settings settingsForm = new settings();

            // Show the settings form
            settingsForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            load_mysql();
            version_check();
            load_file_config();
        }

       

        private void Form1_Click(object sender, EventArgs e)
        {
          
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            load_file_config();
            if (loged_in) irc_send_cmd(irc_channel, "NAMES " + irc_channel);
           
        }

       
        private void button8_Click(object sender, EventArgs e)
        {
            // Create a new instance of the Form2 class
            install installForm = new install();

            // Show the settings form
            installForm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://tirorin.cz");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/tirorin.eu");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("ts3server://tirorin.cz/?port=9987&nickname=WebGuest");
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = System.Drawing.Color.Firebrick;

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = System.Drawing.Color.Maroon;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = System.Drawing.Color.Firebrick;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = System.Drawing.Color.Maroon;
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor = System.Drawing.Color.Firebrick;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = System.Drawing.Color.Maroon;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.BackColor = System.Drawing.Color.Firebrick;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = System.Drawing.Color.Maroon;
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.BackColor = System.Drawing.Color.Firebrick;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = System.Drawing.Color.Maroon;
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            button6.BackColor = System.Drawing.Color.Firebrick;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.BackColor = System.Drawing.Color.Maroon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            run_game(server1IP, server1Port, server1Game, server1Para);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run_game(server2IP, server2Port, server2Game, server2Para);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            run_game(server3IP, server3Port, server3Game, server3Para);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            run_game(server4IP, server4Port, server4Game, server4Para);
        }

        private void button5_Click(object sender, EventArgs e)
        {
           run_game(server5IP, server5Port, server5Game, server5Para);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            run_game(server6IP, server6Port, server6Game, server6Para);
        }

        private void button10_Click(object sender, EventArgs e)
        {

            if (login(textBox3.Text,textBox1.Text)) { backgroundWorker1.RunWorkerAsync(); loged_in = true; MessageBox.Show("Login OK"); }
            
        }


        private void button9_Click(object sender, EventArgs e)
        {
            send_string(irc_channel, textBox3.Text, textBox2, null, richTextBox1); ///channel,name,message textbox,null,kam
          
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            irc_client();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Create a new instance of the Form2 class
            register registerForm = new register();

            // Show the settings form
            registerForm.Show();
        }



        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

      

       
    }
}
