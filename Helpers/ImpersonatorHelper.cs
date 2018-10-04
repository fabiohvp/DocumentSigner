using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace DocumentSigner.Helpers
{
    public class ImpersonatorHelper
    {
        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern int OpenProcessToken(
            IntPtr ProcessHandle, // handle to process
            int DesiredAccess, // desired access to process
            ref IntPtr TokenHandle // handle to open access token
        );

        [DllImport("kernel32", SetLastError = true),
        SuppressUnmanagedCodeSecurityAttribute]
        static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        public const int TOKEN_DUPLICATE = 2;
        public const int TOKEN_QUERY = 0X00000008;
        public const int TOKEN_IMPERSONATE = 0X00000004;

        public static WindowsIdentity GetToken()
        {
            //Console.WriteLine("The app is now running as {0}", WindowsIdentity.GetCurrent().Name);
            var hToken = IntPtr.Zero;
            var dupeTokenHandle = IntPtr.Zero;
            //I determine the process ID by checking explorer, from here we grab the user token


            var runningProcesses = Process.GetProcesses();
            var currentSessionID = Process.GetCurrentProcess().SessionId;
            var sameAsthisSession = (from c in runningProcesses where c.SessionId == currentSessionID select c).ToArray();
            var proc = sameAsthisSession[0];

            if (OpenProcessToken(proc.Handle, TOKEN_QUERY | TOKEN_IMPERSONATE | TOKEN_DUPLICATE, ref hToken) != 0)
            {
                var newId = new WindowsIdentity(hToken);
                //Console.WriteLine(newId.Owner);
                try
                {
                    const int SecurityImpersonation = 2;
                    dupeTokenHandle = DupeToken(hToken, SecurityImpersonation);

                    if (IntPtr.Zero == dupeTokenHandle)
                    {
                        var s = string.Format("Dup failed {0}, privilege not held",
                        Marshal.GetLastWin32Error());
                        throw new Exception(s);
                    }

                    return newId;
                }
                finally
                {
                    CloseHandle(hToken);
                }
            }
            else
            {
                var s = string.Format("OpenProcess Failed {0}, privilege not held", Marshal.GetLastWin32Error());
                throw new Exception(s);
            }
            //Console.WriteLine("The app is now running as {0}", WindowsIdentity.GetCurrent().Name);

        }
        static IntPtr DupeToken(IntPtr token, int Level)
        {
            var dupeTokenHandle = IntPtr.Zero;
            var retVal = DuplicateToken(token, Level, ref dupeTokenHandle);
            return dupeTokenHandle;
        }
    }
}