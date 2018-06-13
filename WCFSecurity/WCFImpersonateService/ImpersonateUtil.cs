
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace WCFImpersonateService{
    /// <summary>
    /// This class contains the methods for impersonation 
    /// as well as undoing the impersonation
    /// </summary>
    internal class ImpersonateUtil {


        #region Public Methods
        /// <summary>
        /// Method to impersonate as logged in user.
        /// </summary>
        /// <returns>Impersonation context is returned, where disposing this will
        /// revert to self i.e. SYSTEM account</returns>
        [
        SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Any failures in doing impersonation should fail gracefully")
        ]
        internal static WindowsImpersonationContext ImpersonateLoggedOnUser() {
            IntPtr loggedOnUserToken = IntPtr.Zero;
            try {
                if (WindowsIdentity.GetCurrent().IsSystem) {
                    if (GetLogonUserToken(out loggedOnUserToken, true)) {
                        return new WindowsIdentity(loggedOnUserToken).Impersonate();
                    }
                } else {
                    return null;
                }
            } catch (Exception exception) {
                // impersonation fails 
            } finally {
                if (loggedOnUserToken != IntPtr.Zero) { 
                    SafeNativeMethods.CloseHandle(loggedOnUserToken);
                }
            }
            return null;
        }
      
        /// <summary>
        /// Check logged in context is already set to another user
        /// </summary>
        /// <returns></returns>
        internal static bool IsImpersonatedRequired() {
            var identity = WindowsIdentity.GetCurrent();
            return identity!=null && identity.IsSystem;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the logged on user token
        /// </summary>
        /// <param name="hToken"></param>
        /// <param name="retryOnFailure"></param>
        /// <returns></returns>
        private static bool GetLogonUserToken(out IntPtr hToken, bool retryOnFailure) {
            bool success = false;
            int retryCount = 5;

            do {
                //Get the active session id and then
                //Get the token of logged on user
                success = SafeNativeMethods.WTSQueryUserToken(
                    SafeNativeMethods.WTSGetActiveConsoleSessionId(), out hToken);
                // On failure retry 5 times at an interval of 2 seconds.
                if (!success && retryOnFailure) {
                    Thread.Sleep(2000);
                    retryCount--;
                } else {
                    break;
                }
            } while (retryCount > 0);

            return success;
        }

        private static bool IsFilePathOnNetwork(string imageFile) {
            if (
                !string.IsNullOrEmpty(imageFile) &&
                !imageFile.StartsWith(@"/", StringComparison.OrdinalIgnoreCase) &&
                !imageFile.StartsWith(@"\", StringComparison.OrdinalIgnoreCase)) {
                var type = new DriveInfo(Path.GetPathRoot(imageFile)).DriveType;
                return type == DriveType.Network || type == DriveType.NoRootDirectory;
            }
            return true;
        }

        #endregion

        [SuppressMessage("Microsoft.Globalization",
            "CA2101:SpecifyMarshalingForPInvokeStringArguments")]
        [SuppressMessage("Microsoft.Design",
            "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "Need everything together in this class for better maintainability.")]
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
       
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods {

        [DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true, 
            CharSet = CharSet.Unicode)]
        internal static extern int DeviceIoControl(
            IntPtr hDevice,
            uint ioControlCode,
            string inBuffer,
            uint inBufferSize,
            IntPtr outBufferPtr,
            uint outBufferSize,
            ref uint bytesReturned,
            IntPtr overlappedPtr
        );

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            uint flags,
            IntPtr template
        );

        /// <summary>
        /// The WTSGetActiveConsoleSessionId function retrieves the Remote Desktop Services 
        /// session that is currently attached to the physical console. 
        /// The physical console is the monitor, keyboard, and mouse.
        /// Note that it is not necessary that Remote Desktop Services be running for 
        /// this function to succeed.
        /// </summary>
        /// <returns>The session identifier of the session that is attached 
        /// to the physical console. If there is no session attached to the physical console, 
        /// (for example, if the physical console session is in the process
        /// of being attached or detached), this function returns 0xFFFFFFFF.</returns>
        [DllImport("kernel32.dll")]
        internal static extern uint WTSGetActiveConsoleSessionId();

        /// <summary>
        /// Queries the current logged in user token
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        [DllImport("wtsapi32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        /// <summary>
        /// Closes open handes returned by LogonUser
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);
    }

}
