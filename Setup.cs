using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public partial class Setup : Form
    {
        Action<JObject> callback;

        public Setup(Action<JObject> callback)
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


            JObject json = new JObject
            {
                ["workingDir"] = Path.GetDirectoryName(filePath),
                ["exe"] = filePath
            };

            string version = Path.Combine(Path.GetDirectoryName(filePath), "version");
            JObject j = JObject.Parse(version);

            if (j.ContainsKey("game_version")) {
                if ((int)j["game_version"] > 2026081804)
                {
                    MessageBox.Show("Oh No!\nIt appears you're using a newer version that what Hunter Reborn currently supports.\nPlease read the setup guide on how to download the required version from steam", "Version not supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if ((int)j["game_version"] < 2026081804) {
                    MessageBox.Show("Oh No!\nIt appears you're using an older version that what Hunter Reborn currently supports.\nPlease read the setup guide on how to download the required version from steam", "Version not supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            this.callback(json);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
