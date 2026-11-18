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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public class Game
    {

        public async Task<string> CreateSession(string type, int startTime, int location, int _reserve) {
            Process[] pros = Process.GetProcessesByName("theHunter");
            if (pros.Length > 0) {
                MessageBox.Show("theHunter game is currently running.\nIf you wish to start a new hun,\nend your current session then try again.", "Hunter Reborn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }

            HttpClient client = new HttpClient();

            var data = new Dictionary<string, string>
            {
                ["start"] = location.ToString(),
                ["loctype"] = type,
                ["reserve"] = _reserve.ToString(),
                ["time"] = startTime.ToString(),
                ["loadout_id"]="0"
            };

            try
            {
                var cookies = await Launcher.GetBrowser().CoreWebView2.CookieManager.GetCookiesAsync("https://api-hr.crecraftstudios.com");
                var cookie = cookies.FirstOrDefault(c => c.Name == "hr_ses_t");

                if (cookie == null)
                    throw new InvalidOperationException("No auth cookie found");

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api-hr.crecraftstudios.com/v1/Launch/start");
                request.Content = new FormUrlEncodedContent(data);
                request.Headers.Add("Cookie", $"hr_ses_t={cookie.Value}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                StartGameResult result = await response.Content.ReadFromJsonAsync<StartGameResult>() ?? throw new InvalidOperationException("Empty server response");
                result.success = true;
                return JsonSerializer.Serialize(result);
            }
            catch (Exception err) {
                return JsonSerializer.Serialize(new StartGameResult
                {
                    success = false
                });
            }
        }

        public async Task<bool> LaunchGame(string access_token, string refresh_token, string session)
        {
            try
            { 
                Process thehunter = new Process();
                thehunter.StartInfo.FileName = Program.HUNTER_EXE_DIR;
                thehunter.StartInfo.WorkingDirectory = Program.HUNTER_WORK_DIR;
                thehunter.StartInfo.ArgumentList.Add($"/connect:hunt://gameserver.thehunter.com/{session}/lang/en_US/access_token/{access_token}/refresh_token/{refresh_token}");
                thehunter.StartInfo.ArgumentList.Add("/param:41");
                thehunter.StartInfo.UseShellExecute = false;
                thehunter.Start();
                return true;
            }
            catch (Exception err) {
                return false;
            }
        }
    }

    public class StartGameResult() { 
        public string ACCESS_TOKEN { get; set; }
        public string REFRESH_TOKEN { get; set; }
        
        public string SESSION { get; set; }
        public string STATUS { get; set; }
        public bool success { get; set; }

    }
}