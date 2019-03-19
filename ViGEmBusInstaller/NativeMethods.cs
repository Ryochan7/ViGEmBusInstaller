using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ViGEmBusInstaller
{
    class NativeMethods
    {
        internal const int INVALID_HANDLE_VALUE = -1;
        internal const short DIGCF_PRESENT = 0x2;
        internal const short DIGCF_DEVICEINTERFACE = 0x10;
        internal const int DIGCF_ALLCLASSES = 0x4;
        internal const short DIGCF_DEFAULT = 0x1;

        internal const int SPDRP_DEVICEDESC = 0;
        internal const int SPDRP_HARDWAREID = 1;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVINFO_DATA
        {
            internal int cbSize;
            internal Guid ClassGuid;
            internal int DevInst;
            internal IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEVPROPKEY
        {
            public Guid fmtid;
            public ulong pid;
        }

        internal static DEVPROPKEY DEVPKEY_Device_HardwareIds =
            new DEVPROPKEY { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 3 };

        internal static DEVPROPKEY DEVPKEY_Device_InstanceId =
            new DEVPROPKEY { fmtid = new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), pid = 256 };

        internal static DEVPROPKEY DEVPKEY_Device_DriverVersion =
            new DEVPROPKEY { fmtid = new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), pid = 3 };

        internal static DEVPROPKEY DEVPKEY_Device_DriverInfPath =
            new DEVPROPKEY { fmtid = new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), pid = 5 };

        [DllImport("setupapi.dll", EntryPoint = "InstallHinfSection", CallingConvention = CallingConvention.StdCall)]
        public static extern void InstallHinfSection(
            [In] IntPtr hwnd,
            [In] IntPtr ModuleHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)] string CmdLineBuffer,
            int nCmdShow);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, int hwndParent, int flags);

        //[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        //static internal extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, int hwndParent, int flags);

        [DllImport("setupapi.dll")]
        static internal extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

        [DllImport("setupapi.dll")]
        static internal extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll")]
        public static extern bool SetupUninstallOEMInfW(
            [In, MarshalAs(UnmanagedType.LPWStr)] string CmdLineBuffer,
            uint flags,
            IntPtr reserved
        );
    }
}
