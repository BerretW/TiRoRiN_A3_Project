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
using MySql.Data.MySqlClient;

namespace TiRoRiN_Mini_Master_Server
{
    public partial class Form1 : Form
    {

        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";
        string config;
        bool s_restart;
        bool s_respawn;
        bool s_compileServer;
        bool s_compileMission;
        bool s_A2;
        bool s_A2epoch;
        bool s_A3;
        bool s_A3epoch;
        bool s_other;
        string delay;
        string serverFolder;
        string missionFolder;
        string DBIP;
        string DBPort;
        string DBUser;
        string DBPass;
        string DB;
        string charTab;
        string objTab;
        string spawnX;
        string spawnZ;
        string Arma2File;
        string BECFile;
        string RestartTime;
        string GamePort;
        string Mods;
        string instance;
        string armaDir;
        string serverFile;
        string[] dead_players;
        string selected_player_id;
        string selected_player_loadout;
        string inventory;
        string backpack;
        string currentstate;
        string last_restart;
        string BECFolder = @"C:\Server1_Epoch\Bec";
        string CPU = "2";

    
        public Form1()
        {
            InitializeComponent();
        }

        public void load_config() 
        {
            
            using (StreamReader Config = new StreamReader(globalAdresar + "config.cfg", Encoding.Default, true))
                {
                    config = Config.ReadToEnd();
                    string[] split = new string[] { "|" };
                    string[] split1 = config.Split(split, StringSplitOptions.None);
                    if (split1[0] == "True") { s_restart = true; checkBox1.Checked = true; } else { s_restart = false; checkBox1.Checked = false; }
                    if (split1[1] == "True") { s_respawn = true; checkBox2.Checked = true; } else s_respawn = false;
                    if (split1[2] == "True") { s_compileServer = true; checkBox3.Checked = true; } else s_compileServer = false;
                    if (split1[3] == "True") { s_compileMission = true; checkBox4.Checked = true; } else s_compileMission = false;
                    if (split1[22] == "True") { s_A2 = true; checkBox6.Checked = true; } else s_A2 = false;
                    if (split1[23] == "True") { s_A2epoch = true; checkBox5.Checked = true; } else s_A2epoch = false;
                    if (split1[24] == "True") { s_A3 = true; checkBox7.Checked = true; } else s_A3 = false;
                    if (split1[25] == "True") { s_A3epoch = true; checkBox9.Checked = true; } else s_A3epoch = false;
                    if (split1[26] == "True") { s_other = true; checkBox8.Checked = true; } else s_other = false;
                    delay = split1[4];textBox3.Text = split1[4];
                    serverFolder = split1[5];textBox4.Text = split1[5];
                    missionFolder = split1[6];textBox5.Text = split1[6];
                    DBIP = split1[7]; textBox6.Text = split1[7];
                    DBPort = split1[8];textBox7.Text = split1[8];
                    DBUser = split1[9]; textBox8.Text = split1[9];
                    DBPass = split1[10]; textBox9.Text = split1[10];
                    DB = split1[11]; textBox10.Text = split1[11];
                    charTab = split1[12]; textBox14.Text = split1[12];
                    objTab = split1[13]; textBox15.Text = split1[13];
                    spawnX = split1[14]; textBox11.Text = split1[14];
                    spawnZ = split1[15]; textBox12.Text = split1[15];
                    Arma2File = split1[16]; textBox1.Text = split1[16];
                    BECFile = split1[17]; textBox2.Text = split1[17];
                    RestartTime = split1[18]; textBox17.Text = split1[18];
                    GamePort = split1[19]; textBox18.Text = split1[19];
                    Mods = split1[20]; textBox19.Text = split1[20];
                    instance = split1[21]; textBox20.Text = split1[21];
                    ;
                    
                }
            using (StreamReader Start_Server = new StreamReader(globalAdresar + "Start_Server.cfg", Encoding.Default, true))
            {
                textBox13.Text = Start_Server.ReadToEnd();
            }
            using (StreamReader Stop_Server = new StreamReader(globalAdresar + "Stop_Server.cfg", Encoding.Default, true))
            {
                textBox16.Text = Stop_Server.ReadToEnd();
            }
            using (StreamReader Respawn_Player = new StreamReader(globalAdresar + "Respawn_Player.cfg", Encoding.Default, true))
            {
                textBox22.Text = Respawn_Player.ReadToEnd();
            }

        }

        public string parseArma2File()
        {
            string[] split = new string[] { "\\" };
            string[] split1 = Arma2File.Split(split, StringSplitOptions.None);
            return split1[split1.Length - 1];

        }

        public string parseArmaDir(string armafile)
        {
            string[] split = new string[] { armafile };
            string[] split1 = Arma2File.Split(split, StringSplitOptions.None);
            return split1[0];
        }

        public void Start_Script()
        {
            if (File.Exists(globalAdresar + "Start_Server.bat"))
            {
                File.Delete(globalAdresar + "Start_Server.bat");
            }

            serverFile = parseArma2File();
            armaDir = parseArmaDir(serverFile);

            string script = "";


            //using (StreamReader startScript = new StreamReader(globalAdresar + "Start_Server.cfg", Encoding.Default, true))
            //{
            //    script = startScript.ReadToEnd();
            //}

            if (textBox24.Text != "") CPU = textBox24.Text;
            script = textBox13.Text;

            string armaScript = serverFile + " -port=" + GamePort + " -config=" + instance + "\\config.cfg -cfg=" + instance + "\\basic.cfg -profiles=" + instance + " -name=" + instance + " -cpuCount="+CPU+" -exThreads=1 -maxmem=2047 -noCB -mod=" + Mods;
            script = script.Replace("@@delay@@", delay);
            script = script.Replace("@@serverfile@@", serverFile);
            script = script.Replace("@@gameport@@", GamePort);
            script = script.Replace("@@instance@@", instance);
            script = script.Replace("@@mods@@", Mods);
            script = script.Replace("@@script@@", armaScript);
            script = script.Replace("@@armadir@@", armaDir);
            script = script.Replace("@@BECFolder@@", BECFolder);

 
            System.IO.StreamWriter file = new System.IO.StreamWriter(globalAdresar + "Start_Server.bat");
            file.WriteLine(script);
            file.Close();
            
        }

        public void stop_script()
        {
            if (File.Exists(globalAdresar + "Stop_Server.bat"))
            {
                File.Delete(globalAdresar + "Stop_Server.bat");
            }

            serverFile = parseArma2File();
            armaDir = parseArmaDir(serverFile);

            string script = "";
            script = textBox16.Text;
            script = script.Replace("@@delay@@", delay);
            script = script.Replace("@@serverfile@@", serverFile);
            script = script.Replace("@@gameport@@", GamePort);
            script = script.Replace("@@instance@@", instance);
            script = script.Replace("@@mods@@", Mods);
            script = script.Replace("@@armadir@@", armaDir);
            script = script.Replace("@@BECFolder@@", BECFolder);
            //using (StreamReader startScript = new StreamReader(globalAdresar + "Start_Server.cfg", Encoding.Default, true))
            //{
            //    script = startScript.ReadToEnd();
            //}

            //string script = "taskkill /im " + serverFile;



            System.IO.StreamWriter file = new System.IO.StreamWriter(globalAdresar + "Stop_Server.bat");
            file.WriteLine(script);
            file.Close();
        }

        public void prepare_write()
        {
            s_restart = checkBox1.Checked;
            s_respawn = checkBox2.Checked;
            s_compileServer = checkBox3.Checked;
            s_compileMission = checkBox4.Checked;
            s_A2epoch = checkBox5.Checked;
            s_A2 = checkBox6.Checked;
            s_A3 = checkBox7.Checked;
            s_A3epoch = checkBox9.Checked;
            s_other = checkBox8.Checked;
            delay = textBox3.Text;
            serverFolder = textBox4.Text;
            missionFolder = textBox5.Text;
            DBIP = textBox6.Text;
            DBPort = textBox7.Text;
            DBUser = textBox8.Text;
            DBPass = textBox9.Text;
            DB = textBox10.Text;
            charTab = textBox14.Text;
            objTab = textBox15.Text;
            spawnX = textBox11.Text;
            spawnZ = textBox12.Text;
            Arma2File = textBox1.Text;
            BECFile = textBox2.Text;
            RestartTime = textBox17.Text;
            GamePort = textBox18.Text;
            Mods = textBox19.Text;
            instance = textBox20.Text;


        }
        public void write_config()
        {
            prepare_write();
            string lines = s_restart + "|" + s_respawn + "|" + s_compileServer + "|" + s_compileMission + "|" + delay + "|" + serverFolder + "|" + missionFolder + "|" + DBIP + "|" + DBPort + "|" + DBUser + "|" + DBPass + "|" + DB + "|" + charTab + "|" + objTab + "|" + spawnX + "|" + spawnZ + "|" + Arma2File + "|" + BECFile + "|" + RestartTime + "|" + GamePort + "|" + Mods + "|" + instance + "|" + s_A2 + "|" + s_A2epoch + "|" + s_A3 + "|" + s_A3epoch + "|" + s_other + "|";
            // Write the string to a file.
            System.IO.StreamWriter Config = new System.IO.StreamWriter(globalAdresar + "config.cfg");
            Config.WriteLine(lines);
            Config.Close();

            System.IO.StreamWriter Start_Server = new System.IO.StreamWriter(globalAdresar + "Start_Server.cfg");
            Start_Server.WriteLine(textBox13.Text);
            Start_Server.Close();

            System.IO.StreamWriter Stop_Server = new System.IO.StreamWriter(globalAdresar + "Stop_Server.cfg");
            Stop_Server.WriteLine(textBox16.Text);
            Stop_Server.Close();

            System.IO.StreamWriter Respawn_Player = new System.IO.StreamWriter(globalAdresar + "Respawn_Player.cfg");
            Respawn_Player.WriteLine(textBox22.Text);
            Respawn_Player.Close();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            write_config();
            Start_Script();
            stop_script();
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void Form1_Load_1(object sender, EventArgs e)
        {
            write_log("Startuji server master");
            load_config();
            Start_Script();
            stop_script();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                serverFolder = fbd.SelectedPath;
                textBox4.Text = serverFolder;
                
                write_config();
            }            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                missionFolder = fbd.SelectedPath;
                textBox5.Text = missionFolder;

                write_config();
            }    
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Arma2File = openFileDialog1.FileName;
                textBox1.Text = Arma2File;
                write_config();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BECFile = openFileDialog1.FileName;
                textBox2.Text = BECFile;
                write_config();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start_Script();
            System.Diagnostics.Process.Start(globalAdresar + "Start_Server.bat");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(globalAdresar + "Stop_Server.bat");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(globalAdresar + "Stop_Server.bat");
            System.Diagnostics.Process.Start(globalAdresar + "Start_Server.bat");
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (checkBox2.Checked) get_dead_players();
            if (checkBox1.Checked) server_restart();
            label24.Text = DateTime.Now.ToString("HH:mm");
        }


        void dezomb()
        {
            List<string> list = new List<string>();
            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE `"+ charTab +"` SET Model='Survivor2_DZ' WHERE Model = 'pz_teacher'";
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

        }

        public void get_dead_players()
        {
            dezomb();
            List<string> list = new List<string>();
            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `Character_DATA` WHERE `Alive` = 0";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    string deadpid = reader["PlayerUID"].ToString();
                    list.Add(deadpid);

                    dead_players = list.ToArray();
                    get_loadout_for_dead_players();
                    respawn_dead_player();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            //textBox1.Text = dead_players[0];
            //int arraylenght = dead_players.Length;
            //textBox1.Text += arraylenght.ToString();
        }

        public void get_loadout_for_dead_players()
        {
            int i = 0;

            while (i != dead_players.Length)
            {

                get_loadout(get_pid_loadout(dead_players[i]));
                i += 1;


            }
        }

        public string get_pid_loadout(string playerid)
        {
            selected_player_id = playerid;
            string loadout = "1";
            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `pid_loadout` WHERE `pid` = " + playerid;
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    loadout = reader["loadout"].ToString();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();


            if (loadout != "") return loadout;
            else return loadout = "1";





        }

        public void get_loadout(string loadout_id)
        {
            selected_player_loadout = loadout_id;

            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `loadout` WHERE `id` = " + loadout_id;
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    inventory = reader["Inventory"].ToString();
                    backpack = reader["Backpack"].ToString();
                    currentstate = reader["CurrentState"].ToString();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();


        }

        private string generate_spawn()
        {
            
            string server_spawn_x_min = "";
            string server_spawn_x_max = "";
            string server_spawn_z_min = "";
            string server_spawn_z_max = "";

                
                string[] split = new string[] { "-" };
                string[] splitx = spawnX.Split(split, StringSplitOptions.None);
                string[] splitz = spawnZ.Split(split, StringSplitOptions.None);
                server_spawn_x_min = splitx[0];
                server_spawn_x_max = splitx[1];
                server_spawn_z_min = splitz[0];
                server_spawn_z_max = splitz[1];



            Random rnd = new Random();
            int server_spawn_x_mi = 0;
            int server_spawn_x_ma = 0;
            int server_spawn_z_mi = 0;
            int server_spawn_z_ma = 0;



            server_spawn_x_mi = Convert.ToInt32(server_spawn_x_min);
            server_spawn_x_ma = Convert.ToInt32(server_spawn_x_max);
            server_spawn_z_mi = Convert.ToInt32(server_spawn_z_min);
            server_spawn_z_ma = Convert.ToInt32(server_spawn_z_max);


            string y = "0.002";
            string dir = rnd.Next(360).ToString();
            string x = rnd.Next(server_spawn_x_mi, server_spawn_x_ma).ToString();
            string z = rnd.Next(server_spawn_z_mi, server_spawn_z_ma).ToString();
            string worldspace = "[" + dir + ",[" + x + "," + z + "," + y + "]]";
            return worldspace;

        }

        public void respawn_dead_player()
        {


            string connString = "Server=" + DBIP + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                //inventory = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
                //loadout = "[\"\"]";
                //MessageBox.Show("provedeno");
                MySqlCommand command = conn.CreateCommand();

                string script;
                script = textBox22.Text;

                    script = script.Replace("@@CharacterTable@@", charTab);
                    script = script.Replace("@@inventory@@", inventory);
                    script = script.Replace("@@backpack@@", backpack);
                    script = script.Replace("@@spawn@@", generate_spawn());
                    script = script.Replace("@@player_id@@", selected_player_id);
                    
                //MessageBox.Show("provedeno");

                    command.CommandText = script;

                    textBox23.Text = command.CommandText;

                    //UPDATE `@@CharacterTable@@` SET Inventory='@@inventory@@',Backpack='@@backpack@@',Worldspace='@@spawn@@',Medical='[]',Alive='1',currentState='[]',Infected='0' WHERE PlayerUID = @@player_id@@
                    //command.CommandText = "UPDATE `Character_DATA` SET Inventory='" + inventory + "',Backpack='" + backpack + "',Worldspace='" + generate_spawn() + "',Medical='[]',Alive='1',currentState='[]',Infected='0' WHERE PlayerUID = " + selected_player_id;
                
                    // write_log("UPDATE `Character_DATA` SET `Inventory`='" + inventory + "',`Backpack`='" + backpack + "',`Worldspace`='" + generate_spawn(server) + "',`Medical`='[]',Alive='1',currentState='[]' WHERE Alive = 0");
                //command.CommandText = "UPDATE Character_DATA SET Inventory=\""+loadout+"\",Backpack=\"[]\",Worldspace=\"generate_spawn\",Medical=\"[]\",Alive=\"0\",currentState=\"[]\" WHERE Alive = \"0\"";
                command.ExecuteNonQuery();
                //textBox31.Text = command.CommandText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
            //MessageBox.Show("Oživuji hráče s PID " + selected_player_id + " a LoadoutID " + selected_player_loadout);
            write_log("Oživuji hráče s PID " + selected_player_id + " a LoadoutID " + selected_player_loadout);

        }

        public void write_log(string text)
        {
            string currenttime = "";
            currenttime = (DateTime.Now.ToString(@"d/M/yyyy HH:mm"));
            textBox21.Text += currenttime + " " + text + System.Environment.NewLine;
        }

        public void server_restart()
        {
            string[] split = new string[] { "," };

            string[] split_server1 = RestartTime.Split(split, StringSplitOptions.None);
            int arraylengh = split_server1.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
                //write_log(split_server1[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server1[arraylengh])
                {
                    if (last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server");
                        //write_log("Restartuji Server1");
                        
                        System.Diagnostics.Process.Start(globalAdresar + "Stop_Server.bat");
                        
                        System.Diagnostics.Process.Start(globalAdresar + "Start_Server.bat");
                    }

                }
            }
        }
    }
}
