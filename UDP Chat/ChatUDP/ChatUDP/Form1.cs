using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;



namespace ChatUDP
{
    public partial class Form1 : Form
    {
        Socket socket;
        EndPoint endpoint_host;
        EndPoint endpoint_remote;

        struct udpPacket
        {
            public byte[] message;
        }
        udpPacket udppacket;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            foreach (IPAddress ipaddr in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                comboBox1.Items.Add(ipaddr.ToString());
            }
            comboBox1.Items.Add("127.0.0.1");

            this.Text = "Unconnected";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

                byte[] msg = new byte[256];
                msg = enc.GetBytes(textBox1.Text);
                socket.Send(msg);
                listBox1.Items.Add("You: " + textBox1.Text);
                textBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                endpoint_host = new IPEndPoint(IPAddress.Parse(textBox2.Text),
                            Convert.ToInt32(textBox3.Text));
            socket.Bind(endpoint_host);

            endpoint_remote = new IPEndPoint(IPAddress.Parse(textBox4.Text),
                                    Convert.ToInt32(textBox5.Text));
            socket.Connect(endpoint_remote);

            udppacket.message = new byte[256];

            socket.BeginReceiveFrom(udppacket.message, 0, udppacket.message.Length,
                                     SocketFlags.None, ref endpoint_remote,
                            new AsyncCallback(HandleSocket), udppacket.message);

            this.Text = "Connected to: " + endpoint_host.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HandleSocket(IAsyncResult iares)
        {
            try
            {
                int size = socket.EndReceiveFrom(iares, ref endpoint_remote);
                if (size > 0)
                {
                    byte[] aux = new byte[256];
                    aux = (byte[])iares.AsyncState;
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string msg = enc.GetString(aux);
 
                    listBox1.Items.Add(": " + msg);                   
                }

                udppacket.message = new byte[256];
                socket.BeginReceiveFrom(udppacket.message, 0, udppacket.message.Length, SocketFlags.None, ref endpoint_remote, new AsyncCallback(HandleSocket), udppacket.message);
            }
            catch (Exception exp)
            {
                label5.Text = exp.ToString();
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = comboBox1.Items[comboBox1.SelectedIndex].ToString();
        }

    }
}
