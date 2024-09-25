using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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

namespace Protocol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int aBits = 128;
        private const int pBits = 32;
        public MainWindow()
        {
            InitializeComponent();
        }
        public static BigInteger next(int bit)
        {
            const int chunk = 8;
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            if (bit % 8 != 0)
                for (int i = 0; i < 8 - bit % 8; i++)
                    sb.Append("0");
            sb.Append("1");

            for (int i = 1; i < bit; i++)
                sb.Append((r.Next() % 2).ToString());

            string str = sb.ToString();
            var x = Enumerable.Range(0, str.Length / chunk).Select(i => str.Substring(i * chunk, chunk)).ToArray();
            byte[] bytes = new byte[x.Length + 1];
            bytes[0] = 0;

            for (int i = 0; i < x.Length; i++)
                bytes[x.Length - i - 1] = Convert.ToByte(x[i], 2);
            return new BigInteger(bytes);
        }
        private static int bits(BigInteger num)
        {
            int i = 0;
            while (num != 0)
            {
                num >>= 1;
                i++;
            }
            return i;
        }
        private static BigInteger next(BigInteger start, BigInteger end)
        {
            int sBits = bits(start);
            int eBits = bits(end);
            Random r = new Random();
            int bit = r.Next(sBits, eBits);
            var res = next(bit);
            while (res < start || res > end)
            {
                bit = r.Next(sBits, eBits);
                res = next(bit);
            }
            return res;
        }

        private static Func<int, BigInteger, bool> MillerTest = delegate (int bit, BigInteger n)
        {
            BigInteger t = n - 1;
            int s = 0;
            while ((t & 1) != 1)
            {
                t >>= 1;
                s++;
            }

            for (int k = 0; k < bit; k++)
            {
                BigInteger y = new BigInteger();
                BigInteger a = next(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, t, n);
                for (int i = 0; i < s; i++)
                {
                    y = BigInteger.ModPow(x, 2, n);
                    if (y == 1 && x != 1 && x != n - 1)
                        return false;
                    x = y;
                }
                if (y != 1)
                {
                    return false;
                }
            }
            return true;
        };

        public static BigInteger Sqrt(BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }
        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        private List<BigInteger> PrimeFactors(BigInteger n)
        {
            List<BigInteger> facts = new();
            while (n % 2 == 0)
            {
                facts.Add(2);
                n /= 2;
            }

            for (int i = 3; i <= Sqrt(n); i += 2)
                while (n % i == 0)
                {
                    facts.Add(i);
                    n /= i;
                }
            
            if (n > 2)
                facts.Add(n);
            return facts;

        }


        private bool findPrimitive(BigInteger g, BigInteger p)
        {
            if (MillerTest(100, p) == false) return false;

            BigInteger phi = p - 1;

            var facts = PrimeFactors(phi);

            bool ok = true;
            for (int i = 0; i < facts.Count() && ok; ++i)
                ok &= BigInteger.ModPow(g, phi / facts[i], p) != 1;
            return ok;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BigInteger pn = next(pBits);

            if (pn % 2 == 0) pn++;

            while(!MillerTest(100, pn) || !MillerTest(100, (pn - 1) / 2)) pn += 2;
            pt.Text = pn.ToString();

            BigInteger gn = next(16);

            while (!findPrimitive(gn, pn)) gn = next(16);
            gt.Text = gn.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger pn = BigInteger.Parse(pt.Text); 
                BigInteger an = BigInteger.Parse(at.Text);
                BigInteger gn = BigInteger.Parse(gt.Text);
                BigInteger An = BigInteger.ModPow(gn, an, pn);
                gt.Text = gn.ToString();
                At.Text = An.ToString();
            } catch
            {
                MessageBox.Show("There is no required data(");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BigInteger an = next(aBits);
            at.Text = an.ToString();
        }
    }
}