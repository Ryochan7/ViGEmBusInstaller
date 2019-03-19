using System;
using System.IO;
using System.Reflection;

namespace ViGEmBusInstaller
{
    class Util
    {
        public static Guid sysGuid = new Guid("{4D36E97D-E325-11CE-BFC1-08002BE10318}");
        public static string exepath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        public static string arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
        internal const string vigemBusDevicePath = @"Root\ViGEmBus";

        public static bool IsViGEmInstalled()
        {
            return CheckForSysDevice(vigemBusDevicePath);
        }

        private static bool CheckForSysDevice(string searchHardwareId)
        {
            bool result = false;
            Guid sysGuid = Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}");
            NativeMethods.SP_DEVINFO_DATA deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);
            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            //var type = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref sysGuid, null, 0, 0);
            for (int i = 0; !result && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_HardwareIds, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                //if (NativeMethods.SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, NativeMethods.SPDRP_DEVICEDESC, ref type,
                //    dataBuffer, dataBuffer.Length, ref requiredSize))
                {
                    string hardwareId = dataBuffer.ToUTF16String();
                    if (hardwareId.Equals(searchHardwareId))
                        result = true;

                    //Console.WriteLine(dataBuffer.ToUTF8String());
                    //Console.WriteLine(hardwareId);
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return result;
        }

        public static string ViGEmBusInstanceId()
        {
            return ObtainSysDeviceInstanceId(vigemBusDevicePath);
        }

        public static string ViGEmBusVersion()
        {
            return DeviceVersionNumber(vigemBusDevicePath);
        }

        public static string ViGEmBusDevProp(NativeMethods.DEVPROPKEY prop)
        {
            return GetDriverProperty(vigemBusDevicePath, prop);
        }

        private static string ObtainSysDeviceInstanceId(string searchHardwareId)
        {
            string result = "";
            bool devmatch = false;
            Guid sysGuid = Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}");
            NativeMethods.SP_DEVINFO_DATA deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);
            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            //var type = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref sysGuid, null, 0, 0);
            for (int i = 0; !devmatch && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_HardwareIds, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string hardwareId = dataBuffer.ToUTF16String();
                    if (hardwareId.Equals(searchHardwareId))
                        devmatch = true;
                }
            }

            if (devmatch)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_InstanceId, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    result = dataBuffer.ToUTF16String();
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return result;
        }

        private static string DeviceVersionNumber(string searchHardwareId)
        {
            string result = "";
            bool devmatch = false;
            Guid sysGuid = Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}");
            NativeMethods.SP_DEVINFO_DATA deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);
            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            //var type = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref sysGuid, null, 0, 0);
            for (int i = 0; !devmatch && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_HardwareIds, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string hardwareId = dataBuffer.ToUTF16String();
                    if (hardwareId.Equals(searchHardwareId))
                        devmatch = true;
                }
            }

            if (devmatch)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_DriverVersion, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    result = dataBuffer.ToUTF16String();
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return result;
        }

        private static string GetDriverProperty(string searchHardwareId,
            NativeMethods.DEVPROPKEY prop)
        {
            string result = "";
            bool devmatch = false;
            Guid sysGuid = Guid.Parse("{4d36e97d-e325-11ce-bfc1-08002be10318}");
            NativeMethods.SP_DEVINFO_DATA deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);
            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            //var type = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref sysGuid, null, 0, 0);
            for (int i = 0; !devmatch && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref NativeMethods.DEVPKEY_Device_HardwareIds, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string hardwareId = dataBuffer.ToUTF16String();
                    if (hardwareId.Equals(searchHardwareId))
                        devmatch = true;
                }
            }

            if (devmatch)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref prop, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    result = dataBuffer.ToUTF16String();
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return result;
        }
    }
}
