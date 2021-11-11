using System;
using System.Drawing;
using System.Windows.Forms;

namespace MU3Input
{
    public partial class IOTest : Form
    {
        private HidIO _io;

        private CheckBox[] _left;
        private CheckBox[] _right;

        public IOTest(HidIO io)
        {
            InitializeComponent();

            _left = new[] {
                lA,
                lB,
                lC,
                lS,
                lM,
            };

            _right = new[] {
                rA,
                rB,
                rC,
                rS,
                rM,
            };

            _io = io;
        }
        
        public static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        internal void UpdateData()
        {
            if (!Enabled && Handle == IntPtr.Zero) return;
            if(_io.Data.Buttons == null) return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    lblStatus.Text = _io.IsConnected ? "Connected" : "Disconnected";

                    if (!_io.IsConnected) return;

                    if(testDown) _io.TestButton = 3;
                    else _io.TestButton = 0;
      
                    for (var i = 0; i < 5; i++)
                    {
                        _left[i].Checked = Convert.ToBoolean(_io.Data.Buttons[i]);
                        _right[i].Checked = Convert.ToBoolean(_io.Data.Buttons[i + 5]);
                    }

                    trackBar1.Value = _io.Lever;

                    if (_io.Scan)
                    {
                        textAimiId.Text = BitConverter.ToString(_io.AimiId).Replace("-", "");
                    }
                }));
            }
            catch
            {
                // ignored
            }
        }
        
        public void SetColor(uint data)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    _left[0].BackColor = Color.FromArgb(
                        (int)((data >> 23) & 1) * 255,
                        (int)((data >> 19) & 1) * 255,
                        (int)((data >> 22) & 1) * 255
                    );
                    _left[1].BackColor = Color.FromArgb(
                        (int)((data >> 20) & 1) * 255,
                        (int)((data >> 21) & 1) * 255,
                        (int)((data >> 18) & 1) * 255
                    );
                    _left[2].BackColor = Color.FromArgb(
                        (int)((data >> 17) & 1) * 255,
                        (int)((data >> 16) & 1) * 255,
                        (int)((data >> 15) & 1) * 255
                    );
                    _right[0].BackColor = Color.FromArgb(
                        (int)((data >> 14) & 1) * 255,
                        (int)((data >> 13) & 1) * 255,
                        (int)((data >> 12) & 1) * 255
                    );
                    _right[1].BackColor = Color.FromArgb(
                        (int)((data >> 11) & 1) * 255,
                        (int)((data >> 10) & 1) * 255,
                        (int)((data >> 9) & 1) * 255
                    );
                    _right[2].BackColor = Color.FromArgb(
                        (int)((data >> 8) & 1) * 255,
                        (int)((data >> 7) & 1) * 255,
                        (int)((data >> 6) & 1) * 255
                    );
                }));
            }
            catch
            {
                // ignored
            }
        }

        private void btnSetOption_Click(object sender, EventArgs e)
        {
            byte[] aimiId;
            
            try
            {
                aimiId = StringToByteArray(textAimiId.Text);
            }
            catch
            {
                MessageBox.Show("Invaild id, Id need to be a hex dump of 10 byte data.", "Error");
                return;
            }
            
            if (aimiId.Length != 10)
            {
                MessageBox.Show("Invaild id, Id need to be a hex dump of 10 byte data.");
                return;
            }
            
            _io.SetAimiId(aimiId);
        }

        bool testDown = false;

        private void TestDown(object sender, MouseEventArgs e)
        {
            testDown = true;
        }

        private void TestUp(object sender, MouseEventArgs e)
        {
            testDown = false;
        }
    }
}
