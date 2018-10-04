using DocumentSigner.Helpers;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace DocumentSigner
{
    [RunInstaller(true)]
    public class CustomInstaller : Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(IDictionary savedState)
        {
            var targetDir = Context.Parameters["targetdir"];
            Run(targetDir, "ImportCertificate.bat", null, false);
            Run(targetDir, "BindCertificate.bat", null, false);

            var pathCertificado = Path.Combine(targetDir, "localhost.pfx");
            var certificado = new X509Certificate2(pathCertificado, "123456");

            Run(targetDir, "ConfigureBrowsers.bat", Convert.ToBase64String(certificado.GetRawCertData()), false);

            if (!EventLog.SourceExists(LogHelper.SourceName))
            {
                EventLog.CreateEventSource(LogHelper.SourceName, LogHelper.LogName);
            }

            base.Commit(savedState);
            //Run(targetDir, "StartProcess.bat", null, true);
        }

        private void Run(string targetDir, string fileName, string parameters, bool loadUserProfile)
        {
            fileName = Path.Combine(targetDir, fileName);
            parameters = (parameters ?? string.Empty).Replace("\"", "\"\"");

            parameters = "\"" + parameters + "\" > \"" + fileName + ".log\"";

            var processInfo = new ProcessStartInfo(fileName, parameters);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = false;
            processInfo.RedirectStandardOutput = false;
            processInfo.LoadUserProfile = loadUserProfile;

            var process = Process.Start(processInfo);
            process.WaitForExit();

            if (process.ExitCode > 0)
            {
                throw new Exception("Houve um erro ao instalar os certificados");
            }

            process.Close();
        }
    }
}
