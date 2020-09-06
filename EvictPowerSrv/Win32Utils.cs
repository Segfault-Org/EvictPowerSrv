using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EvictPowerSrv
{
    sealed class Win32Utils
    {
        public const uint EWX_HYBRID_SHUTDOWN = 0x00400000;
        public const uint EWX_SHUTDOWN = 0x00000001;
        public const uint EWX_FORCE = 0x00000004;

        public const ulong SHTDN_REASON_MAJOR_POWER = 0x00060000;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ExitWindowsEx(uint uFlags, ulong dwReason);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
        phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool ExitWindowsEx(int flg, int rea);

        public const int SE_PRIVILEGE_ENABLED = 0x00000002;
        public const int TOKEN_QUERY = 0x00000008;
        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        public static bool ExitWindows()
        {
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            if (!OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
            {
                return false;
            }
            tp.Count = 1;
            tp.Luid = 0;
            if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid))
            {
                return false;
            }
            tp.Attr = SE_PRIVILEGE_ENABLED;
            if (!AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                return false;
            }
            if (!ExitWindowsEx(EWX_HYBRID_SHUTDOWN | EWX_FORCE | EWX_SHUTDOWN,
                    SHTDN_REASON_MAJOR_POWER))
            {
                return false;
            }
            return true;
        }

        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
    }
}
