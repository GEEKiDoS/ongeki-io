using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MU3Input
{
    public class IOData
    {
        [JsonProperty("left")]
        public int[] Left { get; set; }
        [JsonProperty("right")]
        public int[] Right { get; set; }
        [JsonProperty("lever")]
        public int Lever { get; set; }
    }

    public class SerialIO
    {
        private SerialPort _serialPort;

        public IOData Data { get; private set; }

        public byte LeftButton
        {
            get
            {
                if (Data == null) return 0;

                return (byte)(Data.Left[2] << 0
                            | Data.Left[1] << 1
                            | Data.Left[0] << 2
                            | Data.Left[3] << 3
                            | Data.Left[4] << 4);
            }
        }

        public byte RightButton
        {
            get
            {
                if (Data == null) return 0;

                return (byte)(Data.Right[2] << 0
                            | Data.Right[1] << 1
                            | Data.Right[0] << 2
                            | Data.Right[3] << 3
                            | Data.Right[4] << 4);
            }
        }

        public short Lever
        {
            get
            {
                if (Data == null) return 0;

                double value = Math.Pow(Data.Lever / 1024.0, 0.4545) - 0.5;
                return (short)(value * 32766);
            }
        }

        public byte LEDBrightness { get; set; }

        public bool IsConnected
        {
            get
            {
                return _serialPort.IsOpen;
            }
        }

        public string Port
        {
            get => _serialPort.PortName;
            set => _serialPort.PortName = value;
        }

        public SerialIO()
        {
            _serialPort = new SerialPort("COM3", 38400, Parity.None, 8, StopBits.One);
            _serialPort.DtrEnable = true;
            _serialPort.Handshake = Handshake.None;

            try
            {
                _serialPort.Open();
            }
            catch
            {

            }
        }

        internal void Disconnect()
        {
            _serialPort.Close();
        }

        public void Connect()
        {
            try
            {
                _serialPort.Open();
            }
            catch
            {
            }
        }

        private static byte[] getDataCmd = { 68 };

        public void PullData()
        {
            if (!IsConnected)
                return;

            try
            {
                _serialPort.Write(getDataCmd, 0, 1);
                Data = JsonConvert.DeserializeObject<IOData>(_serialPort.ReadLine());
            }
            catch
            {
            }
        }

        private static int[] bitPosMap =
        {
            23, 19, 22, 20, 21, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6
        };

        public void SetLed(uint data)
        {
            byte[] buffer = new byte[20];
            buffer[0] = 76; // 'L'
            buffer[1] = LEDBrightness; // brightness;

            for (int i = 0; i < 18; i++)
            {
                buffer[2 + i] = (byte)(((data >> bitPosMap[i]) & 1) * 255);
            }

            _serialPort.Write(buffer, 0, 20);
        }
    }
}
