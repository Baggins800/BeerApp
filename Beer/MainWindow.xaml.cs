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
using MahApps.Metro.Controls;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Globalization;

namespace Beer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string[] categories;

        Dictionary<string, int> selectedDrinks
        {
            get
            {
                return invoice.GetItems();
            }
        }

        Bar csv;
        Invoice invoice;
        Stock stock;

        string defaultFileName = "bier.csv";

        public MainWindow()
        {
            InitializeComponent();



            string defaultFileName = "bier.csv";
            if (!File.Exists(defaultFileName)){

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = ".csv";

                if (openFileDialog.ShowDialog() == true)
                    File.Copy(openFileDialog.FileName, defaultFileName);
            }
            csv = new Bar(defaultFileName);
            invoice = new Invoice(defaultFileName);
            stock = new Stock(defaultFileName);

            categories = csv.GetCategories();

            this.catStackPanel.Items.Clear();
            foreach (var cat in categories)
            {

                ListBoxItem item = new ListBoxItem();
                item.Width = 150;
                item.Height = 80;

                Label label = new Label() { Content = cat };
                label.FontSize = 14;
                label.FontWeight = FontWeights.Bold;

                item.Content = label;

                //var i = this.catStackPanel;
                this.catStackPanel.Items.Add(item);
            }
        }

        // Category select button click
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void displaySelectedDrinks()
        {
            listBox.Items.Clear();
            foreach(var k in selectedDrinks.Keys)
            {
                const int fontsize = 24;
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                Label name = new Label() { Content = k };
                name.Width = 200;
                name.FontSize = fontsize;
                Label qty = new Label() { Content = selectedDrinks[k] < 10 ? "0" + selectedDrinks[k].ToString() : selectedDrinks[k].ToString() };
                qty.FontSize = fontsize;
                Label price = new Label() { Content = "@ (R "+ csv.GetPrice(k).ToString("F")  + ")"};
                price.FontSize = fontsize;
                double price_ = csv.GetPrice(k) * selectedDrinks[k];
                Label pricetotal = new Label() { Content = "R " + price_.ToString("F") };
                pricetotal.FontSize = fontsize;
                Button removeButton = new Button() { Content = "-" };
                removeButton.FontSize = fontsize;
                Button plusButton = new Button() { Content = "+" };
                plusButton.FontSize = fontsize;
                removeButton.Width = 50;
                removeButton.Height = 50;
                plusButton.Width = 50;
                plusButton.Height = 50;
                removeButton.Tag = k;
                removeButton.Click += RemoveButton_Click;
                plusButton.Tag = k;
                plusButton.Click += PlusButton_Click;
                stackPanel.Children.Add(qty);
                stackPanel.Children.Add(plusButton);
                stackPanel.Children.Add(removeButton);
                stackPanel.Children.Add(name);
                stackPanel.Children.Add(price);
                stackPanel.Children.Add(pricetotal);
                listBox.Items.Add(stackPanel);
            }

            double total = calculateTotal();
            this.totalCostBlock.Text = Helper.FormatPrice(total);

        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Tag.ToString();
            AddSelectedDrink(name);
            displaySelectedDrinks();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Tag.ToString();
            RemoveSelectedDrink(name);
            displaySelectedDrinks();

        }

        private void RemoveSelectedDrink(string name)
        {
            // TODO RUAN
            invoice.RemoveItem(name);
            /*selectedDrinks[name]--;
            if (selectedDrinks[name] <= 0) selectedDrinks.Remove(name);*/
        }

        private void AddSelectedDrink(string name)
        {
            // TODO RUAN
            invoice.AddItem(name);
            /*if (!selectedDrinks.ContainsKey(name))
                selectedDrinks[name] = 1;
            else
                selectedDrinks[name]++;*/
        }

        private double calculateTotal()
        {
            // TODO RUAN
            return invoice.CalculateTotal();
            /*double total = 0;
            foreach (string k in selectedDrinks.Keys)
                total += 100 * selectedDrinks[k];
            return total;*/                                         
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            double total = calculateTotal();
            var payWindow = new PayWindow(total);
            //381,361
            payWindow.Left = this.Left + this.Width / 2 - 381 / 2;
            payWindow.Top = this.Top + this.Height / 2 - 381 / 2;
            payWindow.ShowDialog();

            if(payWindow.Result == 1) // Next customer, clear stuff
            {
                // Done
                stock.AddInvoice(invoice);

                invoice = new Invoice(defaultFileName);

                selectedDrinks.Clear();
                displaySelectedDrinks();
            }  
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO STOCK TAKING AND SAVE!!!
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("de-DE");
            string stockFileName = "stock_" + localDate.ToString(culture);
            stockFileName = stockFileName.Replace(".", "_");
            stockFileName = stockFileName.Substring(0, stockFileName.IndexOf(" "));
            stockFileName += ".csv";
            stock.WriteToCSVID(stockFileName);
        }

        // Beer select category click
        private void Button_Click1(object sender, RoutedEventArgs e)
        {

        }


        // Beer select category click
        private void catStackPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (catStackPanel.SelectedItem == null) return;
            string selectedCategory = ((catStackPanel.SelectedItem as ListBoxItem).Content as Label).Content.ToString();

            // Get beers
            string[] drinks = csv.GetDrinks(selectedCategory);

            this.selectStackPanel.Items.Clear();

            foreach (var drink in drinks)
            {
                Button button = new Button();
                //button.Style = Resources["AccentedSquareButtonStyle"] as Style;

                Label label = new Label() { Content = drink };
                label.FontSize = 18;
                label.FontWeight = FontWeights.Bold;

                label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(@"drink.png", UriKind.RelativeOrAbsolute));
                image.Width = 48;
                image.Height = 48;

                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Vertical;
                panel.Children.Add(image);
                panel.Children.Add(label);
                

                ListBoxItem item = new ListBoxItem();
                item.Content = panel;
                item.Width = 160;
                item.Height = 160;
                item.Tag = drink;

                //var i = this.catStackPanel;
                this.selectStackPanel.Items.Add(item);
            }

        }

        private void selectStackPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectStackPanel.SelectedItem == null) return;
            string drinkName = (string)(selectStackPanel.SelectedItem as ListBoxItem).Tag;
            selectStackPanel.SelectedIndex = -1;
            AddSelectedDrink(drinkName);
            displaySelectedDrinks();
        }
    }
}
