using MahApps.Metro.Controls;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace Beer
{
    /// <summary>
    /// Interaction logic for PayWindow.xaml
    /// </summary>
    public partial class PayWindow : MetroWindow
    {
        double total;
        double tendered;
        double change;
        public int Result = 0;

        public PayWindow(double total)
        {
            this.total = total;
            InitializeComponent();

            foreach (TabItem panel in this.tabControl.Items)
            {
                panel.IsEnabled = false;
            }
            (this.tabControl.Items[0] as TabItem).IsEnabled = true;

            textTotalMain.Text = Helper.FormatPrice(total);
        }

        // Cash
        private void button_Click(object sender, RoutedEventArgs e)
        {
            GetTendered();            
        }

        async void GetTendered()
        {
            var controller = await this.ShowInputAsync("Cash", "Enter amount tendered");
            try
            {
                tendered = double.Parse(controller.ToString());
                if (tendered >= total)
                {
                    change = tendered - total;
                    ShowChange();
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Incorrect amount tendered");
                }
            }
            catch
            {

            }
            
        }

        //Card
        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            tendered = total;
            change = 0;
            ShowChange();
        }

        private void ShowChange()
        {
            (this.tabControl.Items[1] as TabItem).IsEnabled = true;
            this.tabControl.SelectedIndex = 1;
            this.textBlockChange.Text = Helper.FormatPrice(change);
            this.textBlockTotal.Text = Helper.FormatPrice(total);
            this.textBlockTendered.Text = Helper.FormatPrice(tendered);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Result = 0;
            this.Close(); 
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Result = 1;
            this.Close();
        }
    }
}
