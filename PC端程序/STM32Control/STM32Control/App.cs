using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace STM32Control
{
    public class App :System.Windows.Application
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoSet(uint action, uint uiParam, uint vparam, uint init);

        public App()
        {
            SystemParametersInfoSet(0x001C /*SPI_SETMENUDROPALIGNMENT*/, 0, 0, 0);
            System.Uri resourceLocater = new System.Uri("/STM32Control;component/AppRes.xaml", System.UriKind.Relative);
            App.LoadComponent(this, resourceLocater);
            this.SetLanguageDictionary();
        }

        public static App ApplicationInstance { get { return instance; } }
        static App instance;
        public static bool ismv = false;
        private static NotifyIcon trayIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RemoveTrayIcon();
            AddTrayIcon();
            int rendertier = RenderCapability.Tier >> 16;

            instance = this;

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
           
            MainWindow mw = new MainWindow();
            ismv = true;
            this.MainWindow = mw;
            mw.Show();
        }

        internal void TryActive()
        {
            MainWindow mw = GetMainWindow();
            if (mw != null)
            {
                mw.WindowState = WindowState.Minimized;
                mw.WindowState = WindowState.Normal;
                //mw.Activate();

                trayIcon_Click(null, null);
                
            }
        }

        public MainWindow GetMainWindow()
        {
            if (this.MainWindow is MainWindow)
                return (MainWindow)this.MainWindow;
            else
                return null;
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "zh-CN":
                    dict.Source = new Uri("..\\Resource\\Languages\\Chinese_Simplified.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resource\\Languages\\English.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionReportView exception = new ExceptionReportView(e.Exception);
            if (MainWindow.Visibility == Visibility.Visible)
            {
                exception.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                exception.Owner = MainWindow;
            }
            exception.ShowDialog();
            e.Handled = true;
        }

        private void AddTrayIcon()
        {
            if (trayIcon != null)
            {
                return;
            }
            trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("logo.ico"),
                //Icon = new System.Drawing.Icon("STM32Control;component/Images/logo.ico"),
                Text = "STM32Control"
            };
            trayIcon.Click += trayIcon_Click;
            trayIcon.Visible = true;

            ContextMenu menu = new ContextMenu();

            MenuItem closeItem = new MenuItem();
            closeItem.Text = "退出";
            closeItem.Click += new EventHandler(delegate { this.Shutdown(); });
            MenuItem showItem = new MenuItem();
            showItem.Text = "显示界面";
            showItem.Click += new EventHandler(delegate { this.trayIcon_Click(null, null); });
            menu.MenuItems.Add(showItem);
            menu.MenuItems.Add(closeItem);

            trayIcon.ContextMenu = menu;    //设置NotifyIcon的右键弹出菜单
        }

        void trayIcon_Click(object sender, EventArgs e)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Focus();
        }

        private void RemoveTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            RemoveTrayIcon();
        }

    }
}
