using System;
using System.Windows.Forms;
using CreCraftStudios.HunterReborn.Launcher.Security.Certificate;

namespace CreCraftStudios.HunterReborn.Launcher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var root = RootCA.GetOrCreate();

            var certs = Certificate.Generate(root);
            Certificate.Write(certs);

            Application.Run(new Launcher());
        }
    }
}