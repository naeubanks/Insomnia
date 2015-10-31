using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Insomnia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        PowerPlan initialPlan = null;

        public MainWindow()
        {
            InitializeComponent();

            initialPlan = PowerHelpers.GetCurrentPlan();

            // TODO: In order for this to work today, you must pre-configure a power plan
            // which is named "Insomnia" and is set to make it so that the device never
            // goes to sleep.
            // Use the Power Management APIs to create this kind of plan on the fly based on the
            // users current plan.
            List<PowerPlan> plans = PowerHelpers.GetPlans();
            for (int i = 0; i < plans.Count; ++i)
            {
                PowerPlan plan = plans.ElementAt(i);
                if (plan.name.Equals("Insomnia"))
                {
                    PowerHelpers.SetActive(plan);
                    break;
                }
            }
        }

        private void RestoreInitialPowerPlan()
        {
            PowerPlan currentPlan = PowerHelpers.GetCurrentPlan();
            if (!currentPlan.Equals(initialPlan))
            {
                PowerHelpers.SetActive(initialPlan);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            RestoreInitialPowerPlan();
        }

        private void monitorOff_Click(object sender, RoutedEventArgs e)
        {
            PowerHelpers.TurnOffMonitor();
        }
    }
}
