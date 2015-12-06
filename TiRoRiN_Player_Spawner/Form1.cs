using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TiRoRiN_Player_Spawner
{
    public partial class Form1 : Form
    {
        string GameDBServer = "192.168.1.100";
        string GameDBPort = "3306";
        string GameDB = "server1";
        string GameDBUser = "dayz";
        string GameDBPass = "123456";
        string DBServer = "tirorin.eu";
        string DBPort = "3306";
        string DB = "eshop";
        string DBUser = "eshop";
        string DBPass = "a1d3";

        string [] dead_players;
        string inventory = "";
        string backpack = "";
        string currentstate = "";
        string selected_player_id = "";


        public Form1()
        {
            InitializeComponent();
        }
        public void get_dead_players(string server)
        {
            List<string> list = new List<string>();
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `character_data` WHERE `Alive` = 0";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    string deadpid = reader["PlayerUID"].ToString();
                    list.Add(deadpid);

                    dead_players = list.ToArray();
                    get_loadout_for_dead_players();
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
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=eshop;Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `pid_loadout` WHERE `pid` = "+ playerid;
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
            
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=eshop;Uid=" + DBUser + ";password=" + DBPass;
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
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=web;Uid=" + DBUser + ";password=" + DBPass;
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
                command.CommandText = "UPDATE `character_data` SET `Inventory`='" + inventory + "',`Backpack`='" + backpack + "',`Worldspace`='" + generate_spawn(server) + "',`Medical`='[]',Alive='1',currentState='[]' WHERE PlayerUID = "+ selected_player_id;
                // write_log("UPDATE `character_data` SET `Inventory`='" + inventory + "',`Backpack`='" + backpack + "',`Worldspace`='" + generate_spawn(server) + "',`Medical`='[]',Alive='1',currentState='[]' WHERE Alive = 0");
                //command.CommandText = "UPDATE character_data SET Inventory=\""+loadout+"\",Backpack=\"[]\",Worldspace=\"generate_spawn\",Medical=\"[]\",Alive=\"0\",currentState=\"[]\" WHERE Alive = \"0\"";
                command.ExecuteNonQuery();
                textBox1.Text = command.CommandText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_dead_players("1");
            
        }
    }
}
