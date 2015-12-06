/*
 * Created by SharpDevelop.
 * User: adolf
 * Date: 23.10.2015
 * Time: 13:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TiRoRiN_Master_Server
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void changename (string addon)
		{
			MainForm.ActiveForm.Text = addon;
			
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			
			
		}
	}
}
