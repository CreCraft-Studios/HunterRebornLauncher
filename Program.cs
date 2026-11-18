// Copyright (C) 2026 CreCraft Studios
//
// This file is part of Hunter Reborn Launcher.
//
// Hunter Reborn Launcher is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// This program is distributed WITHOUT ANY WARRANTY.
// See the LICENSE file for details.

using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using CreCraftStudios.HunterReborn.Launcher.Security.Certificate;
using Microsoft.Web.WebView2.Core;

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

            if (!IsWebview2RuntimeInstalled())
            {
                MessageBox.Show("Hunter Reborn Launcher startup failed.", "Failed to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var root = RootCA.GetOrCreate();

            var certs = Certificate.Generate(root);
            Certificate.Write(certs);

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            JsonObject? json=null;

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
            else json = JsonObject.Parse(File.ReadAllText(configPath))!.AsObject();

            HUNTER_WORK_DIR = json["workingDir"]!.GetValue<string>();
            HUNTER_EXE_DIR = json["exe"]!.GetValue<string>();

            Application.Run(new Launcher());
        }

        private static bool IsWebview2RuntimeInstalled()
        {
            try
            {
                CoreWebView2Environment.GetAvailableBrowserVersionString();
                return true;
            }
            catch (WebView2RuntimeNotFoundException) {
                if (MessageBox.Show("It appears WebView2 Runtime isn't installed on this computer.\nThe WebView2 Runtime is required for Hunter Reborn Launcher to function.\n\nWould you like Hunter Reborn Launcher to automatically install it?", "Missing WebView2 Runtime", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    return false;

                string installerLocation = Path.Combine(AppContext.BaseDirectory, "Executables", "MicrosoftEdgeWebview2Setup.exe");

                if (!File.Exists(installerLocation))
                    return false;

                try
                {
                    using Process pro = Process.Start(new ProcessStartInfo
                    {
                        FileName = installerLocation,
                        Arguments = "/silent /install",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    if (pro == null)
                        return false;

                    pro.WaitForExit();

                    CoreWebView2Environment.GetAvailableBrowserVersionString();

                    return true;
                }
                catch (Exception err) {
                    MessageBox.Show("An error has occurred when installing WebView2.\nThe WebView2 runtime is required for Hunter Reborn Launcher to work.\n\nIf the launcher can't install it you can install the WebView2 Runtime manually from developer.microsoft.com/en-gb/microsoft-edge/webview2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
            }
        }
    }
}