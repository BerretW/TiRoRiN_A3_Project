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
using System.Security.Cryptography;
using System.Globalization;

namespace TiRoRiN_E_shop
{
    public partial class register : Form
    {
        string DBServer = "tirorin.eu";
        string DBPort = "3306";
        string DB = "eshop";
        string DBUser = "eshop";
        string DBPass = "a1d3";
        public register()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public bool check_username(string name)
        {
            return true;
        }

        static bool check_useremail(string email)
        {
            return true;
        }
        static bool check_userpid(string pid)
        {
            return true;
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

        public void send_user(string name, string pass, string pid, string email)
        {
            string connString = "Server=" + DBServer + ";Port=" + DBPort + ";Database=" + DB + ";Uid=" + DBUser + ";password=" + DBPass;
            MySqlConnection conn = new MySqlConnection(connString);
            try
            {
                conn.Open();
                if (check_useremail(textBox5.Text) & check_username(textBox1.Text) & check_userpid(textBox4.Text))
                {
                    //MessageBox.Show("provedeno");
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO `users`(`user`, `pass`, `pid`, `email`, `cash`) VALUES('" + name + "','" + pass + "','" + pid + "','" + email + "','50')";
                    command.ExecuteNonQuery();
                }
                else MessageBox.Show("User Name, email or Player ID is already used in database");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   

        private void button1_Click(object sender, EventArgs e)
        {
            bool emailok = false;
            emailok = textBox5.Text.Contains("@");
            if (emailok)
            {
                //MessageBox.Show("Email je spravny");
                if (textBox1.Text != "" & textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "")
                {
                    //MessageBox.Show("Všecho vyplneno");
                    if (textBox2.Text == textBox3.Text)
                    {
                        MessageBox.Show(hash_pass(textBox3.Text,textBox1.Text));
                        send_user(textBox1.Text, hash_pass(textBox3.Text, textBox1.Text), textBox4.Text, textBox5.Text);
                    }
                    else MessageBox.Show("Password does not match");

                }
                else MessageBox.Show("Please fill in all fields.");
            }
            else MessageBox.Show("Please enter valid email adress.");

            
        }
    }
}
