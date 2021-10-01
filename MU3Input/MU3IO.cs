using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MU3Input
{
    public class MU3IO
    {
        private static SerialIO _io;
        private static SerialTest _test;

        [DllExport(ExportName = "mu3_io_get_api_version")]
        public static ushort GetVersion()
        {
            return 0x0102;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_init")]
        public static uint Init()
        {
            if (Process.GetCurrentProcess().ProcessName != "amdaemon" &&
                Process.GetCurrentProcess().ProcessName != "Debug" &&
                Process.GetCurrentProcess().ProcessName != "Test")
                return 0;

            _io = new SerialIO();
            _test = new SerialTest(_io);

            Task.Run(() => _test.ShowDialog());
            return 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_poll")]
        public static uint Poll()
        {
            if (_io == null)
                return 0;

            if (!_io.IsConnected)
            {
                _io.Connect();
            }
            else
            {
                _io.PullData();
            }

            _test.UpdateData();
            return 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_opbtns")]
        public static void GetOpButtons(out byte opbtn)
        {
            opbtn = 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_gamebtns")]
        public static void GetGameButtons(out byte left, out byte right)
        {
            if (_io == null || !_io.IsConnected)
            {
                left = 0;
                right = 0;
                return;
            }

            left = _io.LeftButton;
            right = _io.RightButton;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_lever")]
        public static void GetLever(out short pos)
        {
            if (_io == null || !_io.IsConnected)
            {
                pos = 0;
                return;
            }

            pos = _io.Lever;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_set_led")]
        public static void SetLed(uint data)
        {
            _test.SetColor(data);
            _io.LEDBrightness = _test.LEDBrightness;
            _io.SetLed(data);
        }
    }
}
