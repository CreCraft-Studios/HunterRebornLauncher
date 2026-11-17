using System;
using System.IO;
using System.Windows.Forms;
using CreCraftStudios.HunterReborn.Launcher.Security.Certificate;
using Newtonsoft.Json.Linq;

namespace CreCraftStudios.HunterReborn.Launcher
{
    internal static class Program
    {
        public static string HUNTER_WORK_DIR;
        public static string HUNTER_EXE_DIR;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var root = RootCA.GetOrCreate();

            var certs = Certificate.Generate(root);
            Certificate.Write(certs);

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            JObject? json=null;

            if (!File.Exists(configPath))
            {
                new Setup(r => { json = r; });

                if (json == null) {
                    MessageBox.Show("Failed to locate or load theHunter installed directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), json.ToString());
            }
            else json = JObject.Parse(File.ReadAllText(configPath));

            HUNTER_WORK_DIR = json["workingDir"]!.Value<string>();
            HUNTER_EXE_DIR = json["exe"]!.Value<string>();

            Application.Run(new Launcher());
        }
    }
}