using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TextCopy;

namespace REO
{
    public class Startup
    {
        public static string projectRootFolder;
        public static string IPAddress;
        BrowserWindow browserWindow;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }
        IServerAddressesFeature serverAddressesFeature = null;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
             serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            IPAddress = serverAddressesFeature.Addresses.First();
            projectRootFolder = env.ContentRootPath;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "settings",
                    pattern: "settings",
                defaults: new { controller = "Settings", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=RealmEye}/{action=Index}");
            });

            if (HybridSupport.IsElectronActive)
            {
                SetupElectron();
            }
        }
        public async void SetupElectron()
        {
            WebPreferences wp = new WebPreferences
            {
                WebviewTag = true,
            };
            BuildMenu();
            lastPing = DateTime.Now;
            browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = int.Parse(Settings.GetSetting("BrowserWidth")),
                Height = int.Parse(Settings.GetSetting("BrowserHeight")),
                WebPreferences = wp,
                Show = false,
                AlwaysOnTop = true,
            });
            await browserWindow.WebContents.Session.ClearCacheAsync();//clear cache so local CSS and Js files are always up to date
            Electron.GlobalShortcut.UnregisterAll();
            SetToggleOverlayAccelerator(Settings.GetSetting("ToggleOverlay"));
            SetResetPasswordAccelerator(Settings.GetSetting("ResetPassword"));
            browserWindow.OnClosed += () => { Electron.GlobalShortcut.UnregisterAll(); Electron.App.Quit();  };
            browserWindow.OnResize += BrowserWindow_OnResize;
            browserWindow.OnReadyToShow += BrowserWindow_OnReadyToShow1;
            Electron.IpcMain.On("Ping", (obj) => { lastPing = DateTime.Now; });
            Electron.IpcMain.On("SetRealmEyeStyle", (style) => { Settings.SetSetting("RealmEyeStyle", (string)style); });
            Electron.IpcMain.On("SetSettingsStyle", (style) => { Settings.SetSetting("SettingsStyle", (string)style); });
            Electron.IpcMain.On("SetToggleOverlay", (accelerators) => { SetToggleOverlayAccelerator((string)accelerators); });
            Electron.IpcMain.On("SetResetPassword", (accelerators) => { SetResetPasswordAccelerator((string)accelerators); });
            Electron.IpcMain.On("SetChatMessageMethod", (ChatMessageMethod) => { Settings.SetSetting("ChatMessageMethod", (string)ChatMessageMethod); });
            Electron.IpcMain.On("SendInGameMessage",  (args) => {
                dynamic expand = args;
                ChatMessage($"/tell {expand.username} {expand.message}");
            });
        }

        private void BrowserWindow_OnReadyToShow1()
        {
            browserWindow.WebContents.Session.Cookies.OnChanged += Cookies_OnChanged;
            browserWindow.SetAlwaysOnTop(true, OnTopLevel.screenSaver, 1);
            browserWindow.SetVisibleOnAllWorkspaces(true);
            browserWindow.Show();
            Task.Run(() => { CheckIPCConnection(); });
        }

        DateTime lastPing = DateTime.Now; 
        bool builtMenu = false;
         async void CheckIPCConnection()
        {
            while (true)
            {
                try
                {
                    if((DateTime.Now - lastPing).TotalSeconds > 30)
                    {
                        Electron.Dialog.ShowErrorBox("An Electron.Net bug has caused the program to stop working properly. Please restart the program to use all of the features.", "App Restart Required!");
                        break;
                    }
                }
                catch(Exception e)
                {

                }
                await Task.Delay(5000);
            }
        }
        private void Cookies_OnChanged(Cookie cookie, CookieChangedCause arg2, bool removed)/* In Electron cookies are only kept during the session and are lost on restart. This sets the timeout of cookies so you stay logged in after restarting app. */
        {
            if (!removed && cookie.Domain.Contains("realmeye"))
            {
                if(cookie.Domain[0] == '.')//Cookies.SetAsync fails if the domain starts with a .
                {
                    cookie.Domain = cookie.Domain.Substring(1);
                }

                var url = $"{ (cookie.Secure ? "https" : "http")}://{cookie.Domain}{cookie.Path}";
                browserWindow.WebContents.Session.Cookies.SetAsync(new CookieDetails()
                {
                    Url = url, 
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Secure = cookie.Secure,
                    HttpOnly = cookie.HttpOnly,
                    ExpirationDate = (long)new TimeSpan(DateTime.UtcNow.AddDays(365).Ticks).TotalSeconds,
                    
                }) ;
            }
        }

        private async void BrowserWindow_OnResize()
        {
            var rect = await browserWindow.GetBoundsAsync();
            Settings.SetSettings(new Dictionary<string, string>() { { "BrowserWidth", rect.Width.ToString() }, { "BrowserHeight", rect.Height.ToString() } });
        }

        public void SetToggleOverlayAccelerator(string accelerators)
        {
            Electron.GlobalShortcut.Unregister(Settings.GetSetting("ToggleOverlay"));
            Settings.SetSetting("ToggleOverlay", accelerators);
            Electron.GlobalShortcut.Register(accelerators, async () => { if (await browserWindow.IsVisibleAsync() && !await browserWindow.IsMinimizedAsync()) { browserWindow.Minimize(); } else { browserWindow.Show(); } });

        }
        public void SetResetPasswordAccelerator(string accelerators)
        {
            Electron.GlobalShortcut.Unregister(Settings.GetSetting("ResetPassword"));
            Settings.SetSetting("ResetPassword", accelerators);
            Electron.GlobalShortcut.Register(accelerators, () => {
                ChatMessage("/tell MrEyeBall password");
            });

        }
        async void ChatMessage(string msg)
        {
            if (OperatingSystem.IsWindows() && (ChatMessageMethod)Enum.Parse(typeof(ChatMessageMethod), Settings.GetSetting("ChatMessageMethod"),true) == ChatMessageMethod.PostMessage)
            {
                Electron.WindowManager.BrowserWindows.First().Minimize();
                var processes = Process.GetProcessesByName("RotMG Exalt");
                if (processes.Count() > 0)
                {
                    var MainWindowHandle = processes[0].MainWindowHandle;
                    Keyboard.SendEnterKey(MainWindowHandle);
                    await Task.Delay(500);
                    Keyboard.SendString(MainWindowHandle, msg);
                }
            }
            else
            {
                ClipboardService.SetText(msg);
            }
        }
        private List<MenuItem> menuItems = new List<MenuItem>();
        private void BuildMenu()
        {
            var menu = new MenuItem[] {
            new MenuItem { Label = "RealmEye", Type = MenuType.normal, Click = () => { browserWindow.LoadURL(IPAddress); } },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Label = "Settings", Type = MenuType.normal, Click = () => { browserWindow.LoadURL(IPAddress + "/Settings"); } },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Label = "Open Developer Tools", Type = MenuType.normal, Click = () => browserWindow.WebContents.OpenDevTools() },
            };
            Electron.Menu.SetApplicationMenu(menu);
        }

    }
}
