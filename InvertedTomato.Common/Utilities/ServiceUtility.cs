using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace InvertedTomato {
    public static class ServiceUtility {
        [DllImport("advapi32.dll")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
        [DllImport("Advapi32.dll")]
        private static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName, int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName, string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        private static extern void CloseServiceHandle(IntPtr SCHANDLE);
        [DllImport("advapi32.dll")]
        private static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
        [DllImport("advapi32.dll")]
        private static extern int DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        private const int SC_MANAGER_CREATE_SERVICE = 0x0002;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        //private const int SERVICE_DEMAND_START = 0x00000003;
        private const int SERVICE_ERROR_NORMAL = 0x00000001;
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int SERVICE_QUERY_CONFIG = 0x0001;
        private const int SERVICE_CHANGE_CONFIG = 0x0002;
        private const int SERVICE_QUERY_STATUS = 0x0004;
        private const int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        private const int SERVICE_START = 0x0010;
        private const int SERVICE_STOP = 0x0020;
        private const int SERVICE_PAUSE_CONTINUE = 0x0040;
        private const int SERVICE_INTERROGATE = 0x0080;
        private const int SERVICE_USER_DEFINED_CONTROL = 0x0100;
        private const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL);
        private const int SERVICE_AUTO_START = 0x00000002;
        private const int GENERIC_WRITE = 0x40000000;
        private const int DELETE = 0x10000;

        /// <summary>
        /// This method installs and runs the service in the service control manager.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="serviceDisplayName">Display name of the service.</param>
        /// <returns>True if the process went thro successfully. False if there was any error.</returns>
        public static void InstallService(string serviceDisplayName) {
            Contract.Requires(null != serviceDisplayName);

            var serviceName = System.Reflection.Assembly.GetEntryAssembly().FullName;
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;

            var serviceManagerHandle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
            if (serviceManagerHandle.ToInt64() == 0) {
                throw new SystemException("Failed to open service manager.");
            }

            try {
                var serviceHandle = CreateService(serviceManagerHandle, serviceName, serviceDisplayName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, path, null, 0, null, null, null);
                if (serviceHandle.ToInt64() == 0) {
                    throw new SystemException("Failed to create service.");
                }

                // Now trying to start the service
                if (StartService(serviceHandle, 0, null) == 0) {
                    // If the value i is zero, then there was an error starting the service.
                    // note: error may arise if the service is already running or some other problem.
                    throw new SystemException("Failed to start service.");
                }
            } finally {
                CloseServiceHandle(serviceManagerHandle);
            }
        }

        /// <summary>
        /// This method uninstalls the service from the service control manager.
        /// </summary>
        /// <param name="serviceName">Name of the service to uninstall.</param>
        public static void UninstallService() {
            var serviceName = System.Reflection.Assembly.GetEntryAssembly().FullName;

            var serviceManagerHandle = OpenSCManager(null, null, GENERIC_WRITE);
            if (serviceManagerHandle.ToInt64() == 0) {
                throw new SystemException("Failed to open service manager.");
            }

            var serviceHandle = OpenService(serviceManagerHandle, serviceName, DELETE);
            try {
                if (serviceHandle.ToInt64() == 0) {
                    throw new SystemException("Failed to open service handle.");
                }

                if (DeleteService(serviceHandle) == 0) {
                    throw new SystemException("Failed to delete service.");
                }
            } finally {
                CloseServiceHandle(serviceManagerHandle);
            }
        }
    }
}
