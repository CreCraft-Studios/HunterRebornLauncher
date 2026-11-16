using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public partial class Launcher : Form
    {
        private static WebView2 browserInstance;

        private readonly Uri LANDING_URL = new Uri("https://hunterreborn.crecraftstudios.com");

        private readonly static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nginx");
        private readonly string nginxPath = Path.Combine(path, "nginx.exe");
        private readonly string configPath = Path.Combine(path, "conf", "nginx.conf");

        private readonly Settings settings;

        public Launcher()
        {
            InitializeComponent();

            this.settings = new();

            this.InitializeWebView();
            this.InitializeNginx();
            browserInstance = this.browser;
        }

        public static WebView2 GetBrowser() {
            return browserInstance;
        }

        private void onNavigating(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri))
            {
                e.Cancel = true;
                return;
            }

            if (uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                return;

            var auth = uri.GetLeftPart(UriPartial.Authority);
        }

        private async void InitializeWebView()
        {
            await this.browser.EnsureCoreWebView2Async();
            this.browser.CoreWebView2.Settings.IsStatusBarEnabled = false;

            this.browser.CoreWebView2.Navigate(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "splash.html")).AbsoluteUri);

            Game game = new Game();
            Settings.JS settingsJS = new();
            this.browser.CoreWebView2.AddHostObjectToScript("GameSession", game);
            this.browser.CoreWebView2.AddHostObjectToScript("GameSettings", settingsJS);

            this.browser.CoreWebView2.AddWebResourceRequestedFilter("*launcher.js", CoreWebView2WebResourceContext.Script);

            this.browser.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
            this.browser.CoreWebView2.NavigationStarting += onNavigating;
            this.browser.CoreWebView2.Navigate(this.LANDING_URL.ToString());
        }

        private void InitializeNginx()
        {
            Process nginx = new Process();
            nginx.StartInfo.FileName = this.nginxPath;
            nginx.StartInfo.WorkingDirectory = path;
            nginx.StartInfo.UseShellExecute = false;
            nginx.StartInfo.CreateNoWindow = true;

            nginx.StartInfo.Arguments = $"-c \"{this.configPath}\"";

            try
            {
                nginx.Start();

                nginx.EnableRaisingEvents = true;
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error has occurred in launching Hunter Reborn Launcher\nPlease relaunch the launcher\n\nError Message: {err.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task StopNginx() {
            Process stop = new Process();

            stop.StartInfo.FileName = nginxPath;
            stop.StartInfo.WorkingDirectory = path;
            stop.StartInfo.UseShellExecute = false;
            stop.StartInfo.CreateNoWindow = true;

            stop.StartInfo.Arguments = $"-c \"{configPath}\" -s stop";

            try
            {
                stop.Start();
                await stop.WaitForExitAsync();
            }
            catch (Exception err) {
                Debug.WriteLine($"Error when shutting down Nginx.\n\nMessage: {err.Message}");
            }
        }

        protected async override void OnFormClosed(FormClosedEventArgs e) { 
            await this.StopNginx();

            base.OnFormClosed(e);
        }

        private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (!e.Request.Uri.EndsWith("/launcher.js", StringComparison.OrdinalIgnoreCase))
                return;

            string javascript = """
                   LAUNCHER = (function(){

                    function openHuntMenu() {
                        const selected = document.querySelector(".menu-selected");
                        if (selected.id == "menu-hunt")
                            return;


                        selected.classList.remove("menu-selected");
                        selected.style.display="none";

                        const hunt = document.getElementById("menu-hunt");
                        hunt.classList.add("menu-selected");
                        hunt.style.display="block";
                    }

                    function isLauncher() {
                        return true;
                    }

                    document.getElementById("btn-hunt").addEventListener("click", openHuntMenu);
                    document.getElementById("btn-start-game").addEventListener("click", async function() {
                        if (!isLauncher())
                            return;

                            const json = await chrome.webview.hostObjects.GameSession.CreateSession(HUNT.game.type, HUNT.game.start_time, HUNT.game.location, HUNT.game.reserve);

                            if (json=="")
                                return;
                     
                            const result = JSON.parse(json);
                            if (!result.success) {
                                alert('There was an error with the server. Please try again later');
                                console.log(json);
                                return;
                            }

                            const didRun = await chrome.webview.hostObjects.GameSession.LaunchGame(result.ACCESS_TOKEN, result.REFRESH_TOKEN, result.SESSION);
                            if (!didRun) {
                                alert('Failed to launch your game executable');
                                return;
                            }
                            });

                    return {
                        openHuntMenu,
                        isLauncher
                    };

                })();
                """;

            byte[] bytes = Encoding.UTF8.GetBytes(javascript);
            var stream = new MemoryStream(bytes);

            var response = this.browser.CoreWebView2.Environment.CreateWebResourceResponse(
                stream,
                200,
                "OK",
                "Content-Type: application/javascript; charset=utf-8");

            e.Response = response;
        }
    }
}