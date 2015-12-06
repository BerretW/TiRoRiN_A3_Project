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

namespace TiRoRiN_New_Launcher
{
    public partial class register : Form
    {
        string mysql_server = "tirorin.eu";
        string mysql_port = "3306";
        string mysql_db = "Launcher";
        string mysql_user = "core";
        string mysql_pass = "A4ac9380?";
        public register()
        {
            InitializeComponent();
        }

        private void register_Load(object sender, EventArgs e)
        {

        }
        


        public void register_user(string username, string pass, string email, string arma, string mc, string steam)
        {
            string pripojeni = "Server=" + mysql_server + ";Port=" + mysql_port + ";Database=" + mysql_db + ";Uid=" + mysql_user + ";password=" + mysql_pass + "";
            MySqlConnection conn = new MySqlConnection(pripojeni);

            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "INSERT INTO `users`(`User`, `Pass`, `email`,`Arma_ID`,`MC_Name`,`Steam_ID`) VALUES('" + username + "','" + pass + "','" + email + "','" + arma + "','" + mc + "','" + steam + "')";

           

            try
            {
                conn.Open();
            }

            catch
            {

            }
            command.ExecuteNonQuery();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "" & textBox2.Text != "" & textBox2.Text == textBox3.Text)
            {

                register_user(textBox1.Text, textBox2.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text);
                MessageBox.Show("Úspěšně zaregistrováno");
            }
            else MessageBox.Show("Hesla se neshodují nebo nejdou všechny dúležité položky vyplněny");
        }
    }
}
