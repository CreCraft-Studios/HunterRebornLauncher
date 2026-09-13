using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;
using ValveKeyValue;

namespace CreCraftStudios.HunterReborn.Launcher
{
    internal class Install
    {
        public static JObject? Run() {
            using RegistryKey? regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");

            string? path = regKey?.GetValue("InstallPath")?.ToString();

            if (path == null)
                return OpenDialog();

            string libraryPath = Path.Combine(path, "steamapps", "libraryfolders.vdf");

            using FileStream stream = File.OpenRead(libraryPath);

            KVSerializer kVSerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

            KVDocument kvDoc = kVSerializer.Deserialize(stream);

            string hunterPath = string.Empty;
            string hunterManPath = string.Empty;

            foreach (var lib in kvDoc.Root) {
                KVObject obj = lib.Value;

                if (obj["apps"].ContainsKey("253710")) {
                    hunterPath = Path.Combine((string)obj["path"], "steamapps");
                    hunterManPath = Path.Combine(hunterPath, "appmanifest_253710.acf");
                    break;
                }
            }

            if (string.IsNullOrEmpty(hunterPath) || !File.Exists(hunterManPath))
                return OpenDialog();


            using FileStream hunterManStream = File.OpenRead(hunterManPath);
            KVDocument docHunter = kVSerializer.Deserialize(hunterManStream);
            string installDir = Path.Combine(hunterPath, "common", (string)docHunter.Root["installdir"], "game");

            if (!Directory.Exists(installDir))
                return OpenDialog();

            JObject jObject = new JObject
            {
                ["workingDir"]=installDir,
                ["exe"] = Path.Combine(installDir, "theHunter.exe")
            };

            return jObject;
        }

        private static JObject? OpenDialog() {
            using FolderBrowserDialog dialog = new() { 
                Description = "Select your game installation folder. Should be located .../theHunter/game"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                return new JObject
                {
                    ["workingDir"] = dialog.SelectedPath,
                    ["exe"] = Path.Combine(dialog.SelectedPath, "theHunter.exe")
                };

            return null;
        }
    }
}