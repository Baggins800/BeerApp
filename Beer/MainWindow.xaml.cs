﻿using System;
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

            foreach (var cat in categories)
            {
                Button button = new Button();
                button.Width = 150;
                button.Height = 50;
                Label label = new Label() { Content = cat };
                button.Content = label;
                button.Click += Button_Click;

                //var i = this.catStackPanel;
                this.catStackPanel.Children.Add(button);

            }
            SetInactivity(0);
        }

        // Category select button click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedCategory = ((sender as Button).Content as Label).Content.ToString();
            // Get beers
            string[] drinks = csv.GetDrinks(selectedCategory);
            SetInactivity(1);

            this.selectStackPanel.Children.Clear();

            foreach (var drink in drinks)
            {
                Button button = new Button();
                button.Width = 160;
                button.Height = 50;
                Label label = new Label() { Content = drink };
                button.Content = label;
                button.Click += Button_Click1;

                //var i = this.catStackPanel;
                this.selectStackPanel.Children.Add(button);
            }

        }

        // Beer select category click
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            string drinkName = ((sender as Button).Content as Label).Content.ToString();
            AddSelectedDrink(drinkName);
            displaySelectedDrinks();
        }

        private void displaySelectedDrinks()
        {
            listBox.Items.Clear();
            foreach(var k in selectedDrinks.Keys)
            {
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                Label name = new Label() { Content = k };
                Label qty = new Label() { Content = selectedDrinks[k] < 10 ? "0" + selectedDrinks[k].ToString() : selectedDrinks[k].ToString() };
                Button removeButton = new Button() { Content = "-" };
                Button plusButton = new Button() { Content = "+" };
                removeButton.Tag = k;
                removeButton.Click += RemoveButton_Click;
                plusButton.Tag = k;
                plusButton.Click += PlusButton_Click;
                stackPanel.Children.Add(qty);
                stackPanel.Children.Add(plusButton);
                stackPanel.Children.Add(removeButton);
                stackPanel.Children.Add(name);
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

        void SetInactivity(int index)
        {
            this.tabControl.SelectedIndex = index;
            foreach(TabItem panel in this.tabControl.Items)
            {
                panel.IsEnabled = false;
            }
            for(int i = 0; i < this.tabControl.SelectedIndex; i++)
                (this.tabControl.Items[i] as TabItem).IsEnabled = true;
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
    }
}
