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
using System.IO;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public partial class Setup : Form
    {
        Action<JsonObject> callback;

        public Setup(Action<JsonObject> callback)
        {
            InitializeComponent();
            this.callback = callback;
            this.ShowDialog();
        }

        public string GetLocation()
        {
            return this.txtExeLocation.Text;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new()
            {
                Filter = "theHunter (*.exe)|*.exe",
                Title = "Location for theHunter.exe"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                this.txtExeLocation.Text = dialog.FileName;
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            string filePath = this.txtExeLocation.Text;

            if (!File.Exists(filePath)) {
                MessageBox.Show($"{filePath} can't be found", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            JsonObject json = new JsonObject
            {
                ["workingDir"] = Path.GetDirectoryName(filePath),
                ["exe"] = filePath
            };

            string versionPath = Path.Combine(Path.GetDirectoryName(filePath), "version");
            if (File.Exists(versionPath))
            {

                string version = File.ReadAllText(versionPath);
                JsonObject j = JsonNode.Parse(version)!.AsObject();

                if (j.ContainsKey("game_version"))
                {
                    if ((int)j["game_version"] > 2026081804)
                    {
                        MessageBox.Show("Oh No!\nIt appears you're using a newer version that what Hunter Reborn currently supports.\nPlease read the setup guide on how to download the required version from steam", "Version not supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else if ((int)j["game_version"] < 2026081804)
                    {
                        MessageBox.Show("Oh No!\nIt appears you're using an older version that what Hunter Reborn currently supports.\nPlease read the setup guide on how to download the required version from steam", "Version not supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            this.callback(json);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
