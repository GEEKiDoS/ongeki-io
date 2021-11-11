using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MU3Input
{
    public static class Mu3Io
    {
        internal static HidIO Io;
        private static IOTest _test;

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

            Io = new HidIO();
            _test = new IOTest(Io);

            Task.Run(() => _test.ShowDialog());
            return 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_poll")]
        public static uint Poll()
        {
            if (Io == null)
                return 0;

            if (!Io.IsConnected)
            {
                Io.Reconnect();
            }

            // Io.TestButton = 0;
            _test.UpdateData();
            return 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_opbtns")]
        public static void GetOpButtons(out byte opbtn)
        {
            opbtn = Io.TestButton;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_gamebtns")]
        public static void GetGameButtons(out byte left, out byte right)
        {
            if (Io == null || !Io.IsConnected)
            {
                left = 0;
                right = 0;
                return;
            }

            left = Io.LeftButton;
            right = Io.RightButton;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_lever")]
        public static void GetLever(out short pos)
        {
            if (Io == null || !Io.IsConnected)
            {
                pos = 0;
                return;
            }

            pos = Io.Lever;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_set_led")]
        public static void SetLed(uint data)
        {
            _test.SetColor(data);
            Io.SetLed(data);
        }
    }
}
