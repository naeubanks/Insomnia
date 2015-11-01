using System.Windows;

namespace Insomnia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) 
        {
            App.icon.Visible = false;

            // TODO: Provide options on task bar to shut it down?
            App.Current.Shutdown();
        } 


        private void monitorOff_Click(object sender, RoutedEventArgs e)
        {
            PowerHelpers.TurnOffMonitor();
        }
    }
}
