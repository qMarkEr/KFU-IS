using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

namespace prime_numbers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int max = 10000;
        public MainWindow()
        {
            InitializeComponent();
        }

        private static BigInteger power(BigInteger x, BigInteger y, BigInteger p)
        {
            BigInteger res = 1;
            while (y > 0)
            {
                if ((y & 1) != 0)
                    res = (res * x) % p;
                y = y >> 1;
                x = (x * x) % p;
            }
            return res;
        }
        private static int gcd(int a, int b)
        {
            int c;
            while (b != 0)
            {
                c = a % b;
                a = b;
                b = c;
            }
            return a;
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            List<int> ES = Sieve();
            List<int> MR = Miller();

            StringBuilder sb = new StringBuilder();
            IEnumerable<int> res = MR.Except(ES);
            foreach (var item in res)
            {
                sb.Append(String.Format(item + " "));
            }
            pseudo.Text = sb.ToString();
        }

        private List<int> Miller()
        {
            List<int> ints = new List<int>();
            int a = 2;
            for (int i = 2; i < max; i++)
                if (power(a, i - 1, i) == 1)
                    if (gcd(a, i) == 1)
                        ints.Add(i);
            return ints;
        }
        private List<int> Sieve()
        {
            List<int> ints = new List<int>();
            bool[] prime = new bool[max + 1];

            for (int i = 0; i <= max; i++)
                prime[i] = true;

            for (int p = 2; p * p <= max; p++)
                if (prime[p] == true)
                    for (int i = p * p; i <= max; i += p)
                        prime[i] = false;

            for (int i = 2; i <= max; i++)
                if (prime[i] == true)
                    ints.Add(i);

            return ints;
        }

        private void calculate_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (max != 0) { 
            List<int> ES = Sieve();
            StringBuilder sieve = new StringBuilder();
            foreach (int i in ES)
            {
                sieve.Append(String.Format(i + " "));
            }
            Eratosfen.Text = sieve.ToString();
            }
            else
            {
                MessageBox.Show("Wrong input!");
            }
        }

        private void calculate_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (max != 0)
            {
                List<int> MR = Miller();
                StringBuilder rabin = new StringBuilder();
                foreach (int i in MR)
                {
                    rabin.Append(String.Format(i + " "));
                }
                Rabin.Text = rabin.ToString();
            } else
            {
                MessageBox.Show("Wrong input!");
            }
        }

        private void pseudo_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {
            int a;
            if (int.TryParse(pseudo_Copy.Text, out a) && a <= 1000000)
                max = a;
            else
                max = 0;
        }

    }
}
