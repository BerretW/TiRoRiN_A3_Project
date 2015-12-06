using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TiRoRiN_Loadout
{
    public partial class Form1 : Form
    {
        string[] dead_players;
        string GameDBServer = "192.168.1.206";
        string GameDBPort = "3306";
        string GameDBUser = "dayz";
        string GameDBPass = "a4ac9380";
        string inventory = "";
        string backpack = "";
        string currentstate = "";
        string selected_player_id = "";
        string selected_player_loadout = "";

        string Server1_Start = " ";
        string Server2_Start = " ";
        string Server3_Start = " ";
        string Server4_Start = " ";
        string Server5_Start = " ";
        string Server6_Start = " ";
        string Server1_Stop = " ";
        string Server2_Stop = " ";
        string Server3_Stop = " ";
        string Server4_Stop = " ";
        string Server5_Stop = " ";
        string Server6_Stop = " ";
        string Server1_Restart = " ";
        string Server2_Restart = " ";
        string Server3_Restart = " ";
        string Server4_Restart = " ";
        string Server5_Restart = " ";
        string Server6_Restart = " ";


        string backup_time = " ";
        string backuper = @"c:\Server_Utils\Backup_DB.bat";
        string server1DB = "server1";
        string server2DB = "server2";
        string server3DB = "server3";
        string server4DB = "server4";
        string server5DB = "server5";
        string server6DB = "server6";
        string server1_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server2_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server3_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server4_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server5_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server6_loadout = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
        string server1_backpack = "[]";
        string server2_backpack = "[]";
        string server3_backpack = "[]";
        string server4_backpack = "[]";
        string server5_backpack = "[]";
        string server6_backpack = "[]";
        static string s1_last_restart = "00:00";
        static string s2_last_restart = "00:00";
        static string s3_last_restart = "00:00";
        static string s4_last_restart = "00:00";
        static string s5_last_restart = "00:00";
        static string s6_last_restart = "00:00";
        static string last_save = "00";
        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";

        public Form1()
        {
            InitializeComponent();
        }

        public void write_log(string text)
        {
            string currenttime = "";
            currenttime = (DateTime.Now.ToString(@"d/M/yyyy HH:mm"));
            textBox1.Text += currenttime + " " + text + System.Environment.NewLine; 
        }

        public void OnTimerTick()
        {

            if (DateTime.Now.Minute.ToString() == "30" || DateTime.Now.Minute.ToString() == "00")
            {
                if (last_save != DateTime.Now.ToString("hh:mm"))
                {
                    last_save = DateTime.Now.ToString("hh:mm");
                    write_log("Zálohuji všechny DB");
                    //write_log" Zálohuji všechny DB");
                    System.Diagnostics.Process.Start(@"D:\Server_Utils\Backup_DB.bat");
                }
            }
           
        }

        void dezomb(string server)
        {
            List<string> list = new List<string>();
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
               
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE `Character_DATA` SET Model='Survivor2_DZ' WHERE Model = 'pz_teacher'";
                command.ExecuteNonQuery();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
           
        }

        public void get_dead_players(string server)
        {
            dezomb(server);
            List<string> list = new List<string>();
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
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
                    get_loadout_for_dead_players(server);
                    respawn_dead_player(server);
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

        public void get_loadout_for_dead_players(string server)
        {
            int i = 0;

            while (i != dead_players.Length)
            {

                get_loadout(get_pid_loadout(dead_players[i],server),server);
                i += 1;


            }
        }

        public string get_pid_loadout(string playerid, string server)
        {
            selected_player_id = playerid;
            string loadout = "1";
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server"+server+";Uid=" + GameDBUser + ";password=" + GameDBPass;
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

        public void get_loadout(string loadout_id,string server)
        {
            selected_player_loadout = loadout_id;

            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server"+server+";Uid=" + GameDBUser + ";password=" + GameDBPass;
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

        private string generate_spawn(string server)
        {
            string serverspawn = "";
            string server_spawn_x_min = "";
            string server_spawn_x_max = "";
            string server_spawn_z_min = "";
            string server_spawn_z_max = "";
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server"+server+";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);

            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from spawn";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    serverspawn = reader["server" + server].ToString();

                }
                conn.Close();
                string[] split = new string[] { ";" };

                string[] split1 = serverspawn.Split(split, StringSplitOptions.None);
                string serverspawnx = split1[0];
                string serverspawnz = split1[1];
                split = new string[] { "|" };
                string[] splitx = serverspawnx.Split(split, StringSplitOptions.None);
                string[] splitz = serverspawnz.Split(split, StringSplitOptions.None);
                server_spawn_x_min = splitx[0];
                server_spawn_x_max = splitx[1];
                server_spawn_z_min = splitz[0];
                server_spawn_z_max = splitz[1];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

        public void respawn_dead_player(string server)
        {


            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                //inventory = "[[\"\"ItemCompass\"\"],[\"\"ItemBandage\"\",\"\"ItemBandage\"\",\"\"ItemPainkiller\"\",\"\"FoodSteakCooked\"\",\"\"ItemWaterBottle\"\",\"\"ItemHeatPack\"\"]]";
                //loadout = "[\"\"]";
                //MessageBox.Show("provedeno");
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE `Character_DATA` SET Inventory='" + inventory + "',Backpack='" + backpack + "',Worldspace='" + generate_spawn(server) + "',Medical='[]',Alive='1',currentState='[]',Infected='0' WHERE PlayerUID = " + selected_player_id;
                // write_log("UPDATE `Character_DATA` SET `Inventory`='" + inventory + "',`Backpack`='" + backpack + "',`Worldspace`='" + generate_spawn(server) + "',`Medical`='[]',Alive='1',currentState='[]' WHERE Alive = 0");
                //command.CommandText = "UPDATE Character_DATA SET Inventory=\""+loadout+"\",Backpack=\"[]\",Worldspace=\"generate_spawn\",Medical=\"[]\",Alive=\"0\",currentState=\"[]\" WHERE Alive = \"0\"";
                command.ExecuteNonQuery();
                textBox31.Text = command.CommandText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
            write_log("Oživuji hráče s PID " + selected_player_id + " a LoadoutID " + selected_player_loadout);
           
        }





        public void write_config()
        {
            Server1_Start = textBox2.Text.Replace("\\","\\\\");
            Server2_Start = textBox3.Text.Replace("\\", "\\\\");
            Server3_Start = textBox4.Text.Replace("\\", "\\\\");
            Server4_Start = textBox5.Text.Replace("\\", "\\\\");
            Server5_Start = textBox6.Text.Replace("\\", "\\\\");
            Server6_Start = textBox7.Text.Replace("\\", "\\\\");
            Server1_Stop = textBox13.Text.Replace("\\", "\\\\");
            Server2_Stop = textBox12.Text.Replace("\\", "\\\\");
            Server3_Stop = textBox11.Text.Replace("\\", "\\\\");
            Server4_Stop = textBox10.Text.Replace("\\", "\\\\");
            Server5_Stop = textBox9.Text.Replace("\\", "\\\\");
            Server6_Stop = textBox8.Text.Replace("\\", "\\\\");
            Server1_Restart = textBox14.Text;
            Server2_Restart = textBox15.Text;
            Server3_Restart = textBox16.Text;
            Server4_Restart = textBox17.Text;
            Server5_Restart = textBox18.Text;
            Server6_Restart = textBox19.Text;
            server1DB = textBox25.Text;
            server2DB = textBox24.Text;
            server3DB = textBox23.Text;
            server4DB = textBox22.Text;
            server5DB = textBox21.Text;
            server6DB = textBox20.Text;
            backup_time = textBox26.Text;

            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server1;Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                //MessageBox.Show("provedeno");
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT INTO `master`(`Server1Start`, `Server2Start`, `Server3Start`, `Server4Start`, `Server5Start`, `Server6Start`, `Server1Stop`, `Server2Stop`, `Server3Stop`, `Server4Stop`, `Server5Stop`, `Server6Stop`, `Server1Restart`, `Server2Restart`, `Server3Restart`, `Server4Restart`, `Server5Restart`, `Server6Restart`, `Backup_time`, `Server1_DB`, `Server2_DB`, `Server3_DB`, `Server4_DB`, `Server5_DB`, `Server6_DB`) VALUES (\"" + Server1_Start + "\",\"" + Server2_Start + "\",\"" + Server3_Start + "\",\"" + Server4_Start + "\",\"" + Server5_Start + "\",\"" + Server6_Start + "\",\"" + Server1_Stop + "\",\"" + Server2_Stop + "\",\"" + Server3_Stop + "\",\"" + Server4_Stop + "\",\"" + Server5_Stop + "\",\"" + Server6_Stop + "\",\"" + Server1_Restart + "\",\"" + Server2_Restart + "\",\"" + Server3_Restart + "\",\"" + Server4_Restart + "\",\"" + Server5_Restart + "\",\"" + Server6_Restart + "\",\"" + backup_time + "\",\"" + server1DB + "\",\"" + server2DB + "\",\"" + server3DB + "\",\"" + server4DB + "\",\"" + server5DB + "\",\"" + server6DB + "\")";
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
        }

        public void load_config()
        {
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server1;Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);

            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from master";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                                          
                   Server1_Start =reader["Server1Start"].ToString();
                   Server2_Start =reader["Server2Start"].ToString();
                   Server3_Start =reader["Server3Start"].ToString();
                   Server4_Start =reader["Server4Start"].ToString();
                   Server5_Start =reader["Server5Start"].ToString();
                   Server6_Start =reader["Server6Start"].ToString();
                   Server1_Stop = reader["Server1Stop"].ToString();
                   Server2_Stop = reader["Server2Stop"].ToString();
                   Server3_Stop = reader["Server3Stop"].ToString();
                   Server4_Stop = reader["Server4Stop"].ToString();
                   Server5_Stop = reader["Server5Stop"].ToString();
                   Server6_Stop = reader["Server6Stop"].ToString();
                   Server1_Restart = reader["Server1Restart"].ToString();
                   Server2_Restart = reader["Server2Restart"].ToString();
                   Server3_Restart = reader["Server3Restart"].ToString();
                   Server4_Restart = reader["Server4Restart"].ToString();
                   Server5_Restart = reader["Server5Restart"].ToString();
                   Server6_Restart = reader["Server6Restart"].ToString();
                   backup_time = reader["Backup_time"].ToString();
                   server1DB = reader["Server1_DB"].ToString();
                   server2DB = reader["Server2_DB"].ToString();
                   server3DB = reader["Server3_DB"].ToString();
                   server4DB = reader["Server4_DB"].ToString();
                   server5DB = reader["Server5_DB"].ToString();
                   server6DB = reader["Server6_DB"].ToString();
                }
                conn.Close();
                textBox2.Text = Server1_Start;
                textBox3.Text = Server2_Start;
                textBox4.Text = Server3_Start;
                textBox5.Text = Server4_Start;
                textBox6.Text = Server5_Start;
                textBox7.Text = Server6_Start;
                textBox13.Text = Server1_Stop;
                textBox12.Text = Server2_Stop;
                textBox11.Text = Server3_Stop;
                textBox10.Text = Server4_Stop;
                textBox9.Text = Server5_Stop;
                textBox8.Text = Server6_Stop;
                textBox14.Text = Server1_Restart;
                textBox15.Text = Server2_Restart;
                textBox16.Text = Server3_Restart;
                textBox17.Text = Server4_Restart;
                textBox18.Text = Server5_Restart;
                textBox19.Text = Server6_Restart;
                textBox25.Text = server1DB;
                textBox24.Text = server2DB;
                textBox23.Text = server3DB;
                textBox22.Text = server4DB;
                textBox21.Text = server5DB;
                textBox20.Text = server6DB;
                textBox26.Text = backup_time;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void backup_db()
        {
            string[] split = new string[] { "|" };

            string[] split1 = backup_time.Split(split, StringSplitOptions.None);
            int arraylengh = split1.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
                //write_log(split1[arraylengh] + "|" + DateTime.Now.ToString("mm"));
                if (DateTime.Now.ToString("mm") == split1[arraylengh])
                {
                    if (last_save != DateTime.Now.ToString("mm"))
                    {
                        last_save = DateTime.Now.ToString("mm");
                        write_log("Zálohuji DB");
                       System.Diagnostics.Process.Start(backuper);
                        
                    }

                }
            }
        }

        public void server1_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server1 = Server1_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server1.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
                //write_log(split_server1[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server1[arraylengh])
                {
                    if (s1_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s1_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server1");
                        System.Diagnostics.Process.Start(Server1_Stop);
                        System.Diagnostics.Process.Start(Server1_Start);
                    }

                }
            }
        }

        public void server2_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server = Server2_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
                //write_log(split_server[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server[arraylengh])
                {
                    if (s2_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s2_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server2");
                        System.Diagnostics.Process.Start(Server2_Stop);
                        System.Diagnostics.Process.Start(Server2_Start);
                    }

                }
            }
        }

        public void server3_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server = Server3_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
               // write_log(split_server[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server[arraylengh])
                {
                    if (s3_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s3_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server3");
                        System.Diagnostics.Process.Start(Server3_Stop);
                        System.Diagnostics.Process.Start(Server3_Start);
                    }

                }
            }
        }

        public void server4_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server = Server4_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
               // write_log(split_server[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server[arraylengh])
                {
                    if (s4_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s4_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server4");
                        System.Diagnostics.Process.Start(Server4_Stop);
                        System.Diagnostics.Process.Start(Server4_Start);
                    }

                }
            }
        }

        public void server5_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server = Server5_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
               // write_log(split_server[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server[arraylengh])
                {
                    if (s5_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s5_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server5");
                        System.Diagnostics.Process.Start(Server5_Stop);
                        System.Diagnostics.Process.Start(Server5_Start);
                    }

                }
            }
        }

        public void server6_restart()
        {
            string[] split = new string[] { "|" };

            string[] split_server = Server6_Restart.Split(split, StringSplitOptions.None);
            int arraylengh = split_server.Length;

            while (arraylengh > 0)
            {

                arraylengh = arraylengh - 1;
               // write_log(split_server[arraylengh] + "|" + DateTime.Now.ToString("hh:mm"));
                if (DateTime.Now.ToString("HH:mm") == split_server[arraylengh])
                {
                    if (s6_last_restart != DateTime.Now.ToString("HH:mm"))
                    {
                        s6_last_restart = DateTime.Now.ToString("HH:mm");
                        write_log("Restartuji Server6");
                        System.Diagnostics.Process.Start(Server6_Stop);
                        System.Diagnostics.Process.Start(Server6_Start);
                    }

                }
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            //spawn_player();
            //OnTimerTick();
            if (checkBox1.Checked) server1_restart();
            if (checkBox2.Checked) server2_restart();
            if (checkBox3.Checked) server3_restart();
            if (checkBox4.Checked) server4_restart();
            if (checkBox5.Checked) server5_restart();
            if (checkBox6.Checked) server6_restart();
            if (checkBox7.Checked) get_dead_players("1");
            if (checkBox8.Checked) get_dead_players("2");
            if (checkBox9.Checked) get_dead_players("3");
            if (checkBox10.Checked) get_dead_players("4");
            if (checkBox11.Checked) get_dead_players("5");
            if (checkBox12.Checked) get_dead_players("6");
            label26.Text = DateTime.Now.ToString("HH:mm");
           // backup_db();
            
        }



        private void server_load(string server)
        {
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            string inventar = "nice";
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from loadout WHERE id = 1";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    inventar = reader["Inventory"].ToString();
                   
                   
                }
                conn.Close();
                
                    textBox27.Text = inventar;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void server_save(int server)
        {
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server"+server+";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                
                    //MessageBox.Show("provedeno");
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE loadout SET Inventory='" + textBox27.Text + "' WHERE `id`=1";
                    //command.CommandText.Replace("\"", "\"\"");    
                command.ExecuteNonQuery();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            write_log("Restartovací program startuje");
            load_config();
            server_load("1");
            label26.Text = DateTime.Now.ToString("HH:mm");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            write_config();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }




        private void button14_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox5.Text = openFileDialog1.FileName;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox6.Text = openFileDialog1.FileName;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox7.Text = openFileDialog1.FileName;
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox13.Text = openFileDialog1.FileName;
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox12.Text = openFileDialog1.FileName;
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox11.Text = openFileDialog1.FileName;
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox10.Text = openFileDialog1.FileName;
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox9.Text = openFileDialog1.FileName;
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox8.Text = openFileDialog1.FileName;
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                backuper = openFileDialog1.FileName;
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            server_load("1");
            
        }

        private void button28_Click(object sender, EventArgs e)
        {
            server_load("2");
           
        }

        private void button29_Click(object sender, EventArgs e)
        {
            server_load("3");
            
        }

        private void button30_Click(object sender, EventArgs e)
        {
            server_load("4");
            
        }

        private void button31_Click(object sender, EventArgs e)
        {
            server_load("5");
            
        }

        private void button32_Click(object sender, EventArgs e)
        {
            server_load("6");
            
        }

        private void button33_Click(object sender, EventArgs e)
        {
            server_save(1);
            
        }

        private void button34_Click(object sender, EventArgs e)
        {
            server_save(2);
            
        }

        private void button35_Click(object sender, EventArgs e)
        {
            server_save(3);
            
        }

        private void button36_Click(object sender, EventArgs e)
        {
            server_save(4);
            
        }

        private void button37_Click(object sender, EventArgs e)
        {
            server_save(5);
           
        }

        private void button38_Click(object sender, EventArgs e)
        {
            server_save(6);
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server2_Start);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server1_Start);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server1_Stop);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server3_Start);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server4_Start);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server5_Start);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server6_Start);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server2_Stop);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server3_Stop);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server4_Stop);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server5_Stop);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Server6_Stop);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
