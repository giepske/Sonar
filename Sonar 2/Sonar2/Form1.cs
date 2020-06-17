using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
namespace Sonar2
{
    public partial class Form1 : Form
    {
        TcpClient tcpWhois;
        NetworkStream nsWhois;
        BufferedStream bfWhois;
        StreamWriter strmSend;
        StreamReader strmRecive;
        public Form1()
        {
            InitializeComponent();
            cmbServer.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tcpWhois = new TcpClient(cmbServer.SelectedItem.ToString(), 43);

            nsWhois = tcpWhois.GetStream();
            bfWhois = new BufferedStream(nsWhois);
            strmSend = new StreamWriter(bfWhois);
            strmSend.WriteLine(txtHostName.Text);

            strmSend.Flush();
            txtxResponse.Clear();

            try
            {
                strmRecive = new StreamReader(bfWhois);
                string response;
                while ((response = strmRecive.ReadLine()) != null)
                {
                    txtxResponse.Text += response + "\r\n";
                   
                }
            }
            catch
            { MessageBox.Show("Whois Server :x", "Error"); }
            finally
            {
                try
                {
                    tcpWhois.Close();
                }
                catch
                {
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbServer.SelectedIndex = 0;
        }
    }
}
