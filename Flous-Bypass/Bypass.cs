namespace Bypass
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    public static class ProcessAccessFlags
    {
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
    }

    class Bypass : IDisposable
    {


        private IntPtr m_hProcess = IntPtr.Zero;
        private uint dwPid = 0;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);


        public Bypass(string processName) //Used to pass on processName to Attach function.
        {
            Attach(processName);
        }

        private void Attach(string processName) //Gets processName handle with read/write perms and stores it in m_hProcess. 
        {
            IntPtr window = FindWindow(null, processName);
            GetWindowThreadProcessId(window, out dwPid);

            m_hProcess = OpenProcess(ProcessAccessFlags.PROCESS_VM_OPERATION | ProcessAccessFlags.PROCESS_VM_READ | ProcessAccessFlags.PROCESS_VM_WRITE, false, dwPid);
        }

        //Readers
        //The function name describes what each one does

        public byte[] ReadBytes(IntPtr lpBaseAddress, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(m_hProcess, lpBaseAddress, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public byte[] ReadBytes(IntPtr lpBaseAddress, int offset, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(m_hProcess, lpBaseAddress + offset, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public int ReadInt(IntPtr address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4));
        }

        public int ReadInt(IntPtr address, int offset)
        {
            return BitConverter.ToInt32(ReadBytes(address + offset, 4));
        }

        public short ReadShort(IntPtr address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2));
        }

        public short ReadShort(IntPtr address, int offset)
        {
            return BitConverter.ToInt16(ReadBytes(address + offset, 2));
        }

        public ushort ReadUShort(IntPtr address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2));
        }

        public ushort ReadUShort(IntPtr address, int offset)
        {
            return BitConverter.ToUInt16(ReadBytes(address + offset, 2));
        }

        public uint ReadUInt(IntPtr address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4));
        }

        public uint ReadUInt(IntPtr address, int offset)
        {
            return BitConverter.ToUInt32(ReadBytes(address + offset, 4));
        }

        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4));
        }

        public float ReadFloat(IntPtr address, int offset)
        {
            return BitConverter.ToSingle(ReadBytes(address + offset, 4));
        }

        public double ReadDouble(IntPtr address)
        {
            return BitConverter.ToDouble(ReadBytes(address, 8));
        }

        public double ReadDouble(IntPtr address, int offset)
        {
            return BitConverter.ToDouble(ReadBytes(address + offset, 8));
        }

        // Writers

        public bool WriteBytes(IntPtr lpBaseAddress, byte[] writeBytes)
        {
            return WriteProcessMemory(m_hProcess, lpBaseAddress, writeBytes, writeBytes.Length, IntPtr.Zero);
        }

        public bool WriteBytes(IntPtr lpBaseAddress, byte[] writeBytes, int offset)
        {
            return WriteProcessMemory(m_hProcess, lpBaseAddress + offset, writeBytes, writeBytes.Length, IntPtr.Zero);
        }

        public bool WriteInt(IntPtr lpBaseAddress, int value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteInt(IntPtr lpBaseAddress, int value, int offset)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr lpBaseAddress, short value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr lpBaseAddress, int offset, short value)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr lpBaseAddress, ushort value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr lpBaseAddress, int offset, ushort value)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr lpBaseAddress, uint value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr lpBaseAddress, int offset, uint value)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public bool WriteFloat(IntPtr lpBaseAddress, float value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteFloat(IntPtr lpBaseAddress, int offset, float value)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public bool WriteDouble(IntPtr lpBaseAddress, double value)
        {
            return WriteBytes(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public bool WriteDouble(IntPtr lpBaseAddress, int offset, double value)
        {
            return WriteBytes(lpBaseAddress + offset, BitConverter.GetBytes(value));
        }

        public void Dispose() //Closes handle, is called automatically once exiting the using statement. 
        {
            if (m_hProcess != IntPtr.Zero)
            {
                CloseHandle(m_hProcess);
            }
        }
    }
}

