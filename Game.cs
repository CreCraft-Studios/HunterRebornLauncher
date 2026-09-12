using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            HttpClient client = new HttpClient();

            var data = new Dictionary<string, string>
            {
                ["oauth_access_token"] = "abc",
                ["start"] = location.ToString(),
                ["loctype"] = type,
                ["reserve"] = _reserve.ToString(),
                ["time"] = startTime.ToString(),
                ["loadout_id"]="0"
            };

            try
            {
                var response = await client.PostAsync("http://localhost:82/v1/Launch/start", new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                MessageBox.Show($"Response: {await response.Content.ReadAsStringAsync()}");
                StartGameResult result = await response.Content.ReadFromJsonAsync<StartGameResult>();
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
                thehunter.StartInfo.FileName = @"D:\SteamLibrary\steamapps\common\theHunter\game\theHunter.exe";
                thehunter.StartInfo.WorkingDirectory = @"D:\SteamLibrary\steamapps\common\theHunter\game";
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