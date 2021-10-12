using System;
using System.Runtime.InteropServices;
using System.Threading;
using SimpleHID.Raw;

namespace MU3Input
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public struct OutputData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U1)]
        public byte[] Buttons;

        public ushort Lever;
        [MarshalAs(UnmanagedType.U1)] public bool Scan;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] AimiId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public unsafe struct SetLedInput
    {
        public byte Type;
        public byte LedBrightness;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public fixed byte LedColors[3 * 10];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 64)]
    public unsafe struct SetOptionInput
    {
        public byte Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public fixed byte AimiId[10];
    }

    // ReSharper disable once InconsistentNaming
    public class HidIO
    {
        private readonly SimpleRawHID _hid = new SimpleRawHID();
        private const ushort VID = 0x2341;
        private const ushort PID = 0x8036;
        private byte[] _inBuffer = new byte[64];

        private int _openCount = 0;
        private OutputData _data;

        public OutputData Data => _data;
        
        public bool IsConnected => _openCount > 0;

        public byte LeftButton =>
            (byte) (_data.Buttons[0] << 0
                    | _data.Buttons[1] << 1
                    | _data.Buttons[2] << 2
                    | _data.Buttons[3] << 3
                    | _data.Buttons[4] << 4);

        public byte RightButton =>
            (byte) (_data.Buttons[5] << 0
                    | _data.Buttons[6] << 1
                    | _data.Buttons[7] << 2
                    | _data.Buttons[8] << 3
                    | _data.Buttons[9] << 4);

        public short Lever
        {
            get
            {
                var value = Math.Pow(_data.Lever / 1023.0, 0.4545) - 0.5;
                return (short) (value * 32766);
            }
        }

        public bool Scan => _data.Scan;
        public byte[] AimiId => _data.AimiId;

        public HidIO()
        {
            Reconnect();
            new Thread(PollThread).Start();
        }

        public void Reconnect()
        {
            if (IsConnected)
                _hid.Close();

            _openCount = _hid.Open(1, VID, PID);
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private void PollThread()
        {
            while (true)
            {
                if (!IsConnected)
                    continue;

                var len = _hid.Receive(0, ref _inBuffer, 64, 1000);
                if (len < 0)
                {
                    _openCount = 0;
                    _hid.Close();
                    continue;
                }

                _data = ByteArrayToStructure<OutputData>(_inBuffer);
            }
        }
        
        private static int[] bitPosMap =
        {
            23, 19, 22, 20, 21, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6
        };

        public unsafe void SetLed(uint data)
        {
            if (!IsConnected)
                return;
            
            SetLedInput led;
            led.Type = 0;
            led.LedBrightness = 40;

            for (var i = 0; i < 9; i++)
            {
                led.LedColors[i] = (byte) (((data >> bitPosMap[i]) & 1) * 255);
                led.LedColors[i + 15] = (byte) (((data >> bitPosMap[i + 9]) & 1) * 255);
            }
            
            var outBuffer = new byte[64];
            fixed (void* d = outBuffer)
                CopyMemory(d, &led, 64);

            _hid.Send(0, outBuffer, 64, 1000);
        }
        
        public unsafe void SetAimiId(byte[] id)
        {
            if (!IsConnected)
                return;

            SetOptionInput input;
            input.Type = 1;
            
            fixed(void* src = id)
                CopyMemory(input.AimiId, src, 10);
            
            var outBuffer = new byte[64];
            fixed (void* d = outBuffer)
                CopyMemory(d, &input, 64);

            _hid.Send(0, outBuffer, 64, 1000);
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern unsafe void CopyMemory(void *dest, void *src, int count);
    }
}