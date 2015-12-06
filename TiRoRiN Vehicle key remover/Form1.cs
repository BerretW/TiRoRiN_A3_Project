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

namespace TiRoRiN_Vehicle_key_remover
{
    public partial class Form1 : Form
    {
        string GameDBServer = "192.168.1.100";
        string GameDBPort = "3306";    
        string GameDBUser = "dayz";
        string GameDBPass = "123456";

        
        public Form1()
        {
            InitializeComponent();
        }

        public bool too_old(string date, int span) 
        {
            //string lastupdatedatetime = "2014-08-13 21:29:57";
            string lastupdatedatetime = date;


            string[] split = new string[] { " " };
            string[] split1 = lastupdatedatetime.Split(split, StringSplitOptions.None);
            string lastupdatedate = split1[0].ToString();

            string[] split2 = new string[] { "." };
            string[] split3 = lastupdatedate.Split(split2, StringSplitOptions.None);

            int year = Convert.ToInt32(split3[2]);
            int mont = Convert.ToInt32(split3[1]);
            int day = Convert.ToInt32(split3[0]);
           // textBox1.Text = year.ToString();


            TimeSpan timespan = (DateTime.Now - new DateTime(year, mont, day));
            string rozdil = timespan.TotalDays.ToString();
            split = new string[] { "," };
            split1 = rozdil.Split(split, StringSplitOptions.None);
            rozdil = split1[0];
            int difference = Convert.ToInt32(rozdil);
            if (difference > span) return true;
            else return false;
        }

        public string[] get_locked_vehicles(string server)
        {
            List<string> list = new List<string>();
            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM `object_data` WHERE `CharacterID` !=0 AND `Classname` NOT LIKE CONCAT('%Locked%') AND `Classname` NOT LIKE CONCAT('%Plastic_Pole_EP1_DZ%') AND `Classname` NOT LIKE CONCAT('%Wood%') AND `Classname` NOT LIKE CONCAT('%Cinder%') AND `Classname` NOT LIKE CONCAT('%Sand%') AND `Classname` NOT LIKE CONCAT('%fence%') AND `Classname` NOT LIKE CONCAT('%Metal%') AND `Classname` NOT LIKE CONCAT('%Hedgehog_DZ%') AND `Classname` NOT LIKE CONCAT('%work%') AND `Classname` NOT LIKE CONCAT('%pole%') AND `Classname` NOT LIKE CONCAT('%CanvasHut_DZ%')";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    string lastupdate = reader["LastUpdated"].ToString();
                    list.Add(lastupdate);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return list.ToArray();
        }

        public void unlock_vehicle_by_date(string date,string server)
        {
            string lastupdatedatetime = date;


            string[] split = new string[] { " " };
            string[] split1 = lastupdatedatetime.Split(split, StringSplitOptions.None);
            string lastupdatedate = split1[0].ToString();
            string lastupdatetime = split1[1].ToString();
           // textBox1.Text = lastupdatetime;
            string[] split2 = new string[] { "." };
            string[] split3 = lastupdatedate.Split(split2, StringSplitOptions.None);

            int year = Convert.ToInt32(split3[2]);
            int mont = Convert.ToInt32(split3[1]);
            int day = Convert.ToInt32(split3[0]);

            
            int cyear =Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int cmont = Convert.ToInt32(DateTime.Now.ToString("MM"));
            int cday = Convert.ToInt32(DateTime.Now.ToString("dd"));


            string connString = "Server=" + GameDBServer + ";Port=" + GameDBPort + ";Database=server" + server + ";Uid=" + GameDBUser + ";password=" + GameDBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = " UPDATE `object_data` SET `CharacterID`='0',`LastUpdated`='" + cyear + "-" + cmont + "-" + cday + " " + lastupdatetime + "' WHERE `LastUpdated`='" + year + "-" + mont + "-" + day + " " + lastupdatetime + "'";
                //textBox1.Text = command.CommandText;
                command.ExecuteNonQuery();
                textBox1.Text = command.CommandText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
           
        }

        public void unlock_old_vehicles(int span,string server)
        {
            string[] locked_vehicles = get_locked_vehicles("1");
            int i = 0;
            while (locked_vehicles.Length != i)
            {
                if (too_old(locked_vehicles[i], span)) unlock_vehicle_by_date(locked_vehicles[i],server);
                i += 1;
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            unlock_old_vehicles(10,"1");
            
        }
    }
}
