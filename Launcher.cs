using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public partial class Launcher : Form
    {
        private readonly Uri URL = new Uri("http://localhost:81/");
        Process nginx = new Process();

        public Launcher()
        {
            InitializeComponent();

            this.InitializeWebView();
            this.InitializeNginx();
        }

        private void onNavigating(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri)) {
                e.Cancel = true;
                return;
            }

            if (uri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                return;

            if (!uri.Host.Equals(this.URL.Host, StringComparison.OrdinalIgnoreCase))
                e.Cancel = true;
        }

        private async void InitializeWebView()
        {
            await this.browser.EnsureCoreWebView2Async();
            this.browser.CoreWebView2.Settings.IsStatusBarEnabled = false;

            this.browser.CoreWebView2.Navigate(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "splash.html")).AbsoluteUri);

            Game game = new Game();
            this.browser.CoreWebView2.AddHostObjectToScript("GameSession", game);

            this.browser.CoreWebView2.AddWebResourceRequestedFilter("*launcher.js", CoreWebView2WebResourceContext.Script);

            this.browser.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
            this.browser.CoreWebView2.NavigationStarting += onNavigating;
            this.browser.CoreWebView2.Navigate(this.URL.ToString());
        }

        private void InitializeNginx() {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nginx");

            this.nginx.StartInfo.FileName = Path.Combine(path, "nginx.exe");
            this.nginx.StartInfo.WorkingDirectory = path;
            this.nginx.StartInfo.UseShellExecute = false;
            this.nginx.StartInfo.CreateNoWindow = true;

            this.nginx.StartInfo.Arguments = $"-c {Path.Combine(path, "conf", "nginx.conf")}";

            this.nginx.Start();
        }

        private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (!e.Request.Uri.EndsWith("/launcher.js", StringComparison.OrdinalIgnoreCase))
                return;

            string javascript = """
                   $LAUNCHER = (function(){

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

                            const json = await chrome.webview.hostObjects.GameSession.CreateSession($HUNT.game.type, $HUNT.game.start_time, $HUNT.game.location, $HUNT.game.reserve);
                     
                            const result = JSON.parse(json);
                            if (!result.success) {
                                alert('There was an error with the server. Please try again later');
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