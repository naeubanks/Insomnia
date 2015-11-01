using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Insomnia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NotifyIcon icon;

        protected override void OnStartup(StartupEventArgs e)
        {
            App.icon = new NotifyIcon();
            icon.Click += new EventHandler(icon_Click);
            icon.Icon = Insomnia.Properties.Resources.Owl;
            icon.Visible = true;

            base.OnStartup(e);

            // Prevent the system from sleeping
            PowerHelpers.EnableInsomnia();
        }

        private void icon_Click(Object sender, EventArgs e)
        {
            // TODO
        }
    }
}
