using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace STM32Control
{
    public class StartUp
    {
        static Mutex m;
        [STAThread]
        public static void Main()
        {
            bool isrun;
            m = new Mutex(true, "STM32Control", out isrun);
            if (!isrun)
            {
                
            }
            SingleApp sa = new SingleApp();
            string[] s = { "STM32Control" };
            sa.Run(s);
        }
    }

    public class SingleApp : WindowsFormsApplicationBase
    {
        public SingleApp()
        {
            base.IsSingleInstance = true;
        }

        private App app;
        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            app = new App();
            app.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            app.TryActive();
        }

    }


}
