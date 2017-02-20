using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Beer
{
    /// <summary>
    /// Interaction logic for TenderWindow.xaml
    /// </summary>
    /// 
    public enum DialogResult { Success, Failure };
    public partial class KeypadWindow : MetroWindow
    {
        public DialogResult Result { get; private set; }
        public double Value { get; private set; }
        public KeypadWindow()
        {
            Result = Beer.DialogResult.Failure;
            InitializeComponent();
        }

        private void button_Copy10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Value = double.Parse(this.textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture);

                Result = Beer.DialogResult.Success;
                this.Close();
            }
            catch
            {

            }
        }

        private void button_Copy14_Click(object sender, RoutedEventArgs e)
        {
            Result = Beer.DialogResult.Failure;
            this.Close();
        }

        private void AddDigit(char s)
        {
            string text = this.textBox.Text;
            text += s;
            this.textBox.Text = text;
            this.textBox.Focus();
            this.textBox.Select(text.Length,0);
            
        }

        private void button_Copy6_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('1');
        }

        private void button_Copy7_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('2');
        }

        private void button_Copy8_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('3');
        }

        private void button_Copy3_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('4');
        }

        private void button_Copy4_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('5');
        }

        private void button_Copy5_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('6');
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('7');
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('8');
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('9');
        }

        private void button_Copy12_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('0');
        }

        private void button_Copy13_Click(object sender, RoutedEventArgs e)
        {
            AddDigit('.');
        }

        private void button_Copy9_Click(object sender, RoutedEventArgs e)
        {
            string text = this.textBox.Text;
            if(text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            this.textBox.Text = text;
            this.textBox.Focus();
            this.textBox.Select(text.Length, 0);
        }
    }
}
