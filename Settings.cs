using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public class Settings
    {
        public bool fullscreen;
        public bool vertical_sync;
        public bool context_help;
        public bool audio_memory;
        public List<DisplayModes.Resolution> resolutions = new();
        public DisplayModes.Resolution selectedResolution;

        public Enums.QUALITY scene_complexity;
        public Enums.TEXTURE_QUALITY texture_quality;
        public Enums.QUALITY shadow_quality;
        public Enums.ANTI_ALIASING anti_aliasing;
        public Enums.ANI_FILTERING anisotropic_filtering;
        private readonly string filePath;

        protected static Settings instance;

        public Settings() {
            instance = this;

            this.filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TheHunterSettings_live.cfg");

            if (!File.Exists(filePath)) {
                string steamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TheHunterSettings_steam_live.cfg");
                if (File.Exists(steamPath))
                    File.Copy(steamPath, this.filePath);
                else
                {
                    this.fullscreen = false;
                    this.vertical_sync = false;
                    this.context_help = false;
                    this.audio_memory = false;
                    this.scene_complexity = Enums.QUALITY.LOW;
                    this.texture_quality = Enums.TEXTURE_QUALITY.LOW;
                    this.shadow_quality = Enums.QUALITY.LOW;
                    this.anisotropic_filtering = Enums.ANI_FILTERING.OFF;
                    this.anti_aliasing = Enums.ANTI_ALIASING.X1;

                    this.resolutions = DisplayModes.GetSupported();
                    this.selectedResolution = this.resolutions[0];
                    return;
                }
            }

            this.resolutions = DisplayModes.GetSupported();
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++) {
                switch (i) {
                    case 1:
                        int.TryParse(lines[i], out int full);
                        this.fullscreen = full == 1 ? true : false;
                        break;
                    case 2:
                        string[] t = lines[i].Split(" ");
                        int.TryParse(t[0], out int width);
                        int.TryParse(t[1], out int height);
                        int.TryParse(t[2], out int refresh);
                        this.selectedResolution= new(width, height, refresh);
                        break;
                    case 3:
                        int.TryParse(lines[i], out int scene);
                        this.scene_complexity = (Enums.QUALITY)scene;
                        break;
                    case 4:
                        int.TryParse(lines[i], out int texture);
                        this.texture_quality = (Enums.TEXTURE_QUALITY)texture;
                        break;
                    case 5:
                        int.TryParse(lines[i], out int shadow);
                        this.shadow_quality = (Enums.QUALITY)shadow;
                        break;
                    case 6:
                        int.TryParse(lines[i], out int anti);
                        this.anti_aliasing = (Enums.ANTI_ALIASING)anti;
                        break;
                    case 7:
                        int.TryParse(lines[i], out int ani);
                        this.anisotropic_filtering = (Enums.ANI_FILTERING)ani;
                        break;
                    case 8:
                        int.TryParse(lines[i], out int sync);
                        this.vertical_sync = sync == 1 ? true : false;
                        break;
                    case 9:
                        int.TryParse(lines[i], out int ctx);
                        this.context_help = ctx==1 ? true : false;
                        break;
                    case 10:
                        int.TryParse(lines[i], out int audio);
                        this.audio_memory = audio == 1 ? true : false;
                        break;
                }
            }
        }

        public static Settings Get() {
            return instance;
        }

        public void Save()
        {
            string[] data=new string[12];

            data[1] = this.fullscreen == true ? "1" : "0";
            data[2] = $"{this.selectedResolution.Width} {this.selectedResolution.Height} {this.selectedResolution.Hz}";
            data[3] = ((int)this.scene_complexity).ToString();
            data[4] = ((int)this.texture_quality).ToString();
            data[5] = ((int)this.shadow_quality).ToString();
            data[6] = ((int)this.anti_aliasing).ToString();
            data[7] = ((int)this.anisotropic_filtering).ToString();
            data[8] = this.vertical_sync == true ? "1" : "0";
            data[9] = this.context_help == true ? "1" : "0";
            data[10] = this.audio_memory == true ? "1" : "0";

            data[0] = "129";
            data[11] = "0";

            File.WriteAllLines(this.filePath, data);
        }

        public string ToJson() {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { IncludeFields = true });
        }

        public class Enums { 
            public enum QUALITY { 
                LOW=0,
                MEDIUM=1,
                HIGH=2,
                VERY_HIGH=3
            }

            public enum TEXTURE_QUALITY { 
                LOW=0,
                MEDIUM=1,
                HIGH=2
            }

            public enum ANTI_ALIASING { 
                X1=0,
                X2=1,
                X4=2,
                X8=3
            }

            public enum ANI_FILTERING
            {
                OFF=0,
                X1 = 1,
                X2 = 2,
                X4 = 3,
                X8 = 4,
                X16=5
            }
        }


        public class JS {
            public async Task<string> Get() {
                Settings set = Settings.Get();

                JsonArray a = new();
                foreach (var item in set.resolutions)
                {
                    a.Add(new JsonObject
                    {
                        ["width"] = item.Width,
                        ["height"] = item.Height,
                        ["rate"] = item.Hz
                    });
                }

                JsonObject json = new()
                {
                    ["fullscreen"] = set.fullscreen,
                    ["vertical_sync"]=set.vertical_sync,
                    ["context_help"]=set.context_help,
                    ["audio_memory"]=set.audio_memory,
                    ["scene_complexity"]=(int)set.scene_complexity,
                    ["texture_quality"]=(int)set.texture_quality,
                    ["shadow_quality"]=(int)set.shadow_quality,
                    ["anti_aliasing"]=(int)set.anti_aliasing,
                    ["anisotropic_filtering"]=(int)set.anisotropic_filtering,
                    ["selected_resolution"] = new JsonObject {
                        ["width"]=set.selectedResolution.Width,
                        ["height"]=set.selectedResolution.Height,
                        ["rate"]=set.selectedResolution.Hz
                    },
                    ["resolutions"]=a
                };

                return JsonSerializer.Serialize(json);
            }

            public async Task Save(string json) {
                JsonObject j = JsonNode.Parse(json).AsObject();

                Settings set = Settings.Get();
                set.fullscreen = j["fullscreen"]!.GetValue<bool>();
                set.vertical_sync = j["vertical_sync"]!.GetValue<bool>();
                set.context_help = j["context_help"]!.GetValue<bool>();
                set.audio_memory = j["audio_memory"]!.GetValue<bool>();

                set.scene_complexity = (Enums.QUALITY)j["scene_complexity"]!.GetValue<int>();
                set.texture_quality = (Enums.TEXTURE_QUALITY)j["texture_quality"]!.GetValue<int>();
                set.shadow_quality = (Enums.QUALITY)j["shadow_quality"]!.GetValue<int>();
                set.anti_aliasing = (Enums.ANTI_ALIASING)j["anti_aliasing"]!.GetValue<int>();
                set.anisotropic_filtering = (Enums.ANI_FILTERING)j["anisotropic_filtering"]!.GetValue<int>();

                JsonObject res = j["selected_resolution"]!.AsObject();
                set.selectedResolution = new DisplayModes.Resolution(res["width"]!.GetValue<int>(), res["height"]!.GetValue<int>(), res["rate"]!.GetValue<int>());

                set.Save();
            }
        }
    }
}