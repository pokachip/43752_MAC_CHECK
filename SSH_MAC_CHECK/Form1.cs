using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading;


/// <summary>
/// https://stackoverflow.com/questions/41555597/how-to-run-commands-by-sudo-and-enter-password-by-ssh-net-c-sharp
/// </summary>

namespace SSH_MAC_CHECK
{
    public partial class F1CS43752_MAC_CHECK : Form
    {
        public F1CS43752_MAC_CHECK()
        {
            InitializeComponent();
            ////////////////////////////////////////////////////////////////////////////////// read IP in txt file

            
            string input = "";
            string[] lines = System.IO.File.ReadAllLines(@"C:\43752IP.txt", Encoding.Default);
            foreach (string show in lines)
            {
                if (input == "")
                {
                    input = show;
                }
                else
                {
                    input = input + "\r\n" + show;
                }
            }
            
            //TextBox에 넣기
            textBox1.Text = input;
            //textBox1.Text = "192.168.10.130";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void button_PowerOff(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
                MessageBox.Show("Host Ip를 입력해주세요");
                
            else
            {
                SshClient cSSH = new SshClient(textBox1.Text, 22, "f1media", "f1media");
                cSSH.Connect();
                //cSSH.KeepAliveInterval = TimeSpan.FromSeconds(5);
                //cSSH.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                if (cSSH.IsConnected)
                {

                    //SshCommand cmd_PWROFF = 
                    cSSH.RunCommand("echo -e 'f1media\n' |  sudo -S shutdown -P now");
                    //SshCommand cmd_PWROFF = cSSH.RunCommand("./Desktop/F1CS43752/work/SSH_Shutdown.sh");
                    //cSSH.Disconnect();
                    //cSSH.Dispose();
                    //richTextBox1.AppendText("SSH Disconnected\n");
                    Application.ExitThread();
                    Application.Exit();
                }
                else
                {
                    Application.ExitThread();
                    Application.Exit();
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_MACcheck(object sender, EventArgs e)
        {
            string Macaddr = "00:90:4c:12:d0:01";
            string Initmac = "00:90:4c:12:d0:01\n";
            if (textBox1.Text == "") 
            {
                MessageBox.Show("Host Ip를 입력해주세요");              
            }
            else
            {
                SshClient cSSH = new SshClient(textBox1.Text, 22, "f1media", "f1media");
                cSSH.Connect();

                if (cSSH.IsConnected)
                {
                    richTextBox1.Clear();
                    text_MAC.Text = "";

                    //Scan PCIe slot
                    richTextBox1.AppendText("Scan PCIe Slot\n");
                    SshCommand cmd_rescan = cSSH.RunCommand("echo -e 'f1media\n' | sudo -S ./Desktop/F1CS43752/work/rscan.sh");
                    richTextBox1.SelectionColor = System.Drawing.Color.Blue;
                    richTextBox1.AppendText(cmd_rescan.Result + "\n");

                    // Load Drive
                    richTextBox1.AppendText("Load driver\n");
                    SshCommand cmd_load = cSSH.RunCommand("./Desktop/F1CS43752/work/SSHload.sh");
                    richTextBox1.AppendText(cmd_load.Result + "\n");

                    SshCommand tes2t = cSSH.RunCommand("ifconfig | grep wlan0");
                    richTextBox1.AppendText(tes2t.Result);

                    // Display MAC address
                    SshCommand test = cSSH.RunCommand("ifconfig wlan0 | grep ether | grep -o -E '([[:xdigit:]]{1,2}:){5}[[:xdigit:]]{1,2}'");
                    Macaddr = test.Result;
                    if (Macaddr.Equals(Initmac))
                    {
                        text_MAC.ForeColor = Color.Red;
                        text_MAC.Text = "Failed";
                    } else{
                        text_MAC.ForeColor = Color.Blue;
                        text_MAC.Text = Macaddr; //test.Result;
                    }
                    
                    
                    // remove pcie
                    SshCommand cmd_remove = cSSH.RunCommand("echo -e 'f1media\n' | sudo -S ./Desktop/F1CS43752/work/rmove.sh");
                    richTextBox1.AppendText(cmd_remove.Result + "\n");
                    richTextBox1.AppendText("PCIe removed\n");
                    richTextBox1.AppendText("\n");
                    // Disconnect SSH
                    cSSH.Disconnect();
                    cSSH.Dispose();
                    richTextBox1.AppendText("SSH Disconnected\n");
                  
                }
            }
        }
    }
}
