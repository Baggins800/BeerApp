using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Beer
{
    public static class Helper
    {
        public static string FormatPrice(double d)
        {
            return string.Format("R {0:#.00}", Convert.ToDecimal(d));
        }
    }
    public class Bar
    {
        private string[] lines;
        public Bar(string file)
        {
            string csvFile = file;
            lines = File.ReadAllLines(csvFile);
        }
        public void ReadCSV(string file)
        {
            string csvFile = file;
            lines = File.ReadAllLines(csvFile);
        }
        public string[] GetCategories()
        {
            var values = lines.Select(l => new { ID = l.Split(',').First(), Type = l.Split(',')[1], Name = l.Split(',')[2] });
            var category = values.GroupBy(test => test.Type).Select(grp => grp.First());
            string[] results = new string[category.Count()];
            int i = 0;
            foreach (var value in category)
            {
                results[i++] = string.Format("{0}", value.Type);
            }
            return results;
        }

        public string[] GetDrinks(string category)
        {
            var values = lines.Select(l => new { ID = l.Split(',').First(), Type = l.Split(',')[1], Name = l.Split(',')[2] });
            var drinks = values.Where(p => p.Type == category);
            string[] results = new string[drinks.Count()];
            int i = 0;
            foreach (var value in drinks)
            {
                results[i++] = string.Format("{0}", value.Name);
            }
            return results;
        }

        public string GetID(string drink)
        {
            var values = lines.Select(l => new { ID = l.Split(',').First(), Type = l.Split(',')[1], Name = l.Split(',')[2] });
            var drinkid = values.Where(p => p.Name == drink);
            string result = "";
            foreach (var value in drinkid)
            {
                result = value.ID;
            }
            return result;
        }
        public double GetPrice(string drinkname)
        {
            var values = lines.Select(l => new { ID = l.Split(',').First(), Type = l.Split(',')[1], Name = l.Split(',')[2], Price = l.Split(',')[3] });
            var drink = values.Where(p => p.Name == drinkname);
            var result = drink.First().Price;
            result = result.Replace("R", "").Trim();

            double outValue;
            if (double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out outValue))
                return outValue;
            else throw new Exception("What number is this");
        }

    }
    public class Invoice
    {
        private Bar b;
        private Dictionary<string, int> items = new Dictionary<string, int>();
        public Invoice(string file)
        {
            b = new Bar(file);
        }
        public void AddItem(string item)
        {
            if (items.ContainsKey(item))
            {
                items[item]++;
            }
            else
            {
                items.Add(item, 1);
            }
        }

        public void RemoveItem(string item)
        {
            if (items.ContainsKey(item))
            {
                items[item]--;
   
                if (items[item] <= 0)
                {
                    items.Remove(item);
                }
            }
        }

        public double CalculateTotal()
        {
            double total = 0.0;
            foreach (var v in items)
            {
                total += b.GetPrice(v.Key) * v.Value;
            }
            return total;
        }

        public Dictionary<string, int> GetItems()
        {
            return items;
        }
    }
    public class Stock
    {
        private Bar B;
        private Dictionary<string, int> items = new Dictionary<string, int>();
        public Stock(string file)
        {
            B = new Bar(file);
            foreach (var b in B.GetCategories())
            {
                foreach (var l in B.GetDrinks(b))
                {
                    items.Add(l, 0);
                }
            }
        }
        public void AddInvoice(Invoice invoice)
        {
            foreach (var k in invoice.GetItems())
            {
                items[k.Key] += k.Value;
            }
        }
        Dictionary<string, int> GetStock()
        {
            return items;
        }

        public void WriteToCSV(string filename)
        {
            string filename_new = filename;
            string csv = "";
            if (File.Exists(filename))
            {
                var lines = File.ReadAllLines(filename);
                var values = lines.Select(l => new { ID = l.Split(',').First(), Count = l.Split(',')[1] });
                foreach (var i in values)
                {
                    items[i.ID] += Convert.ToInt32(i.Count);
                }
            }

            foreach (var i in items)
            {
                csv += string.Format("{0},{1}\n", i.Key, i.Value);
            }
            File.WriteAllText(filename_new, csv);

        }

        public void WriteToCSVID(string filename)
        {
            string filename_new = filename;
            string csv = "";
            if (File.Exists(filename))
            {
                filename_new += ".new";
            }
            foreach (var i in items)
            {
                csv += string.Format("{0},{1},{2}\n", B.GetID(i.Key), i.Key, i.Value);
            }
            File.WriteAllText(filename_new, csv);
        }

    }
   /* class Program
    {
        static void Main(string[] args)
        {
            Bar b = new Bar("bier.csv");
            Invoice i = new Invoice("bier.csv");
            i.AddItem(b.GetDrinks("Beer").First());
            i.AddItem(b.GetDrinks("Beer").First());
            List<Invoice> kk = new List<Invoice>();
            kk.Add(i);
            Stock a = new Stock("bier.csv");
            a.AddInvoices(kk);
            a.WriteToCSV("piesang.csv");
            foreach (var v in b.GetCategories())
            {
                Console.WriteLine(v);
            }

            foreach (var v in b.GetDrinks("Beer"))
            {
                Console.WriteLine("{0} - {1}", v, b.GetPrice(v));

            }
            Console.WriteLine(i.CalculateTotal());

            Console.ReadLine();
        }
    }*/
}