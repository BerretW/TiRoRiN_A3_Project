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
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Net;

namespace TiRoRiN_E_shop
{
        

    public partial class Form1 : Form
    {
        string DBServer = "tirorin.eu";
        string DBPort = "3306";
        string DB = "eshop";
        string DBUser = "eshop";
        string DBPass = "a1d3";
        bool logedin = false;
        string usercash = "00";
        string usercart = "";
        string userpid = "";
        string selected_id = "0";
        string execute_command = "";
        

        public string globalAdresar = new FileInfo(Application.ExecutablePath).Directory.FullName + "\\";
        Form Register_user = new TiRoRiN_E_shop.register();
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Register_user.Show();
        }

        public void update_info()
        {
            label4.Text = usercash + " Kč";
            label7.Text = userpid;
        }

        public void load_items()
        {
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from items";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string itemid = reader["id"].ToString();
                    string itemname = reader["itemname"].ToString();
                    //string iteminfo = reader["iteminfo"].ToString();
                    //string itemimg = reader["itemimg"].ToString();
                    string itemprice = reader["itemprice"].ToString();
                    dataGridView1.Rows.Add(itemid, itemname, itemprice);
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string hash_pass(string password, string salt)
        {
            string input = password + salt;
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public void login(string username, string userpass)
        {
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from users";
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read() & !logedin)
                {
                    string dbname = reader["user"].ToString();
                    string dbpass = reader["pass"].ToString();
                    userpid = reader["pid"].ToString();
                    usercash = reader["cash"].ToString();
                    usercart = reader["cart"].ToString();
                    if (dbname == username & dbpass == userpass) logedin = true;
                    else logedin = false;

                    if (logedin)
                    {
                        update_info();
                        MessageBox.Show("Login OK");
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void download_image(string url, string id)
        {
            if (!File.Exists(globalAdresar + @"\images\"+ id + ".jpg"))
            {
                WebClient webClient = new WebClient();

                webClient.DownloadFileAsync(new Uri(url), globalAdresar + @"\images\" + id + ".jpg");
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!logedin)
            {
                if (textBox1.Text != "" & textBox2.Text != "")
                {
                    login(textBox1.Text, hash_pass(textBox2.Text,textBox1.Text));
                }
               
            }
            if (logedin) load_items();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
            bool isExists = System.IO.Directory.Exists(globalAdresar+ @"\images");
            if (!isExists) System.IO.Directory.CreateDirectory(globalAdresar + @"\images");
        }

        public void nazvy_parametru(int i, string nazev)
        {
            if (i == 1)
            {
                label5.Text = nazev;

            }
            if (i == 2)
            {
                
                label13.Text = nazev;

            }
            if (i == 3)
            {
                

                label14.Text = nazev;

               
            }
           
        }

        public void parametry(int i)
        {
            if (i == 0)
            {
                label5.Visible = false;

                label13.Visible = false;

                label14.Visible = false;

                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox6.Visible = false;
            }
            if (i == 1)
            {
                label5.Visible = true;

                label13.Visible = false;

                label14.Visible = false;

                textBox4.Visible = true;
                textBox5.Visible = false;
                textBox6.Visible = false;
            }
            if (i == 2)
            {
                label5.Visible = true;

                label13.Visible = true;

                label14.Visible = false;

                textBox4.Visible = true;
                textBox5.Visible = true;
                textBox6.Visible = false;
            }
            if (i == 3)
            {
                label5.Visible = true;

                label13.Visible = true;

                label14.Visible = true;

                textBox4.Visible = true;
                textBox5.Visible = true;
                textBox6.Visible = true;
            }
        }

        public void pay(int cash, int cost)
        {
            cash = cash - cost;
            usercash = cash.ToString(); ;
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=eshop;Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();

                //MessageBox.Show("provedeno");
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE `users` SET `cash`='" + cash + "' WHERE `pid`='" + userpid + "'";
                //command.CommandText.Replace("\"", "\"\"");    
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
                label4.Text = cash.ToString();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string item_properties = "";
            string id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            selected_id = id;
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select * from items WHERE id = \""+ id +"\"";;
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //string itemid = reader["id"].ToString();
                    label10.Text = reader["itemname"].ToString();
                    string iteminfo = reader["iteminfo"].ToString();


                    textBox3.Text = iteminfo;
                    string itemimg = reader["itemimg"].ToString();
                    label11.Text = reader["itemprice"].ToString();
                    label12.Text = reader["itemtype"].ToString();
                    item_properties = reader["inputs"].ToString();
                    execute_command = reader["command"].ToString();
                    download_image(itemimg, id);
                    pictureBox1.ImageLocation = globalAdresar + @"\images\" + id + ".jpg";
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
                string[] split = new string[] { "|" };

                string[] split1 = item_properties.Split(split, StringSplitOptions.None);
                int arraylengh = split1.Length;
                int i = 0;
                if (item_properties == "") arraylengh = 0;
                parametry(arraylengh);

                
                while (arraylengh != i)
                {
                    nazvy_parametru(i+1,split1[i]);

                    i = i + 1;
                }
               
                
        }

        

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(usercash);
            int b = Convert.ToInt32(label11.Text);
            if (a > b)
            {
                bool complete = false;
                string edited_command = execute_command;
                if (edited_command.Contains("var1")) edited_command = edited_command.Replace("var1", textBox4.Text);
                if (edited_command.Contains("var2")) edited_command = edited_command.Replace("var2", textBox5.Text);
                if (edited_command.Contains("var3")) edited_command = edited_command.Replace("var3", textBox6.Text);
                string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
                MySqlConnection conn = new MySqlConnection(connString);
                try
                {
                    conn.Open();


                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = edited_command;
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    complete = true;
                }
                conn.Close();
                
                if (complete) pay(a, b);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
