using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MU3Input
{
    public partial class SerialTest : Form
    {
        private SerialIO _io;

        private CheckBox[] _left;
        private CheckBox[] _right;

        public SerialTest(SerialIO io)
        {
            InitializeComponent();

            _left = new[] {
                checkBox3,
                checkBox2,
                checkBox1,
                checkBox4,
                checkBox5,
            };

            _right = new[] {
                checkBox8,
                checkBox9,
                checkBox10,
                checkBox7,
                checkBox6,
            };

            _io = io;
        }

        string oldCom;

        internal void UpdateData()
        {
            if (!Enabled && Handle == IntPtr.Zero) return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (oldCom != _io.Port)
                    {
                        textBox1.Text = oldCom = _io.Port;
                    }

                    label6.Text = _io.IsConnected ? "Connected" : "Disconnected";

                    if (!_io.IsConnected || _io.Data == null) return;

                    var data = _io.Data;

                    for (int i = 0; i < 5; i++)
                    {
                        _left[i].Checked = Convert.ToBoolean(data.Left[i]);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        _right[i].Checked = Convert.ToBoolean(data.Right[i]);
                    }

                    trackBar1.Value = _io.Lever;
                }));
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _io.Disconnect();
            _io.Port = textBox1.Text;
        }

        private void SerialTest_Load(object sender, EventArgs e)
        {

        }

        public byte LEDBrightness => (byte)trackBar2.Value;

        public void SetColor(uint data)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    _left[2].BackColor = Color.FromArgb(
                        (int)((data >> 23) & 1) * 255,
                        (int)((data >> 19) & 1) * 255,
                        (int)((data >> 22) & 1) * 255
                    );
                    _left[1].BackColor = Color.FromArgb(
                        (int)((data >> 20) & 1) * 255,
                        (int)((data >> 21) & 1) * 255,
                        (int)((data >> 18) & 1) * 255
                    );
                    _left[0].BackColor = Color.FromArgb(
                        (int)((data >> 17) & 1) * 255,
                        (int)((data >> 16) & 1) * 255,
                        (int)((data >> 15) & 1) * 255
                    );
                    _right[2].BackColor = Color.FromArgb(
                        (int)((data >> 14) & 1) * 255,
                        (int)((data >> 13) & 1) * 255,
                        (int)((data >> 12) & 1) * 255
                    );
                    _right[1].BackColor = Color.FromArgb(
                        (int)((data >> 11) & 1) * 255,
                        (int)((data >> 10) & 1) * 255,
                        (int)((data >> 9) & 1) * 255
                    );
                    _right[0].BackColor = Color.FromArgb(
                        (int)((data >> 8) & 1) * 255,
                        (int)((data >> 7) & 1) * 255,
                        (int)((data >> 6) & 1) * 255
                    );
                }));
            }
            catch { }
        }
    }
}
