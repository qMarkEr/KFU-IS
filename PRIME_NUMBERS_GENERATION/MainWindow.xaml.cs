using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Numerics;



namespace Prime_numbers_generation
{
    public partial class MainWindow : Window
    {
        private static BigInteger power(BigInteger x, BigInteger y, BigInteger p)
        {
            BigInteger res = 1;
            while (y > 0)
            {
                if ((y & 1) != 0)
                    res = (res * x) % p;
                y >>= 1;
                x = (x * x) % p;
            }
            return res;
        }

        private static BigInteger gcd(BigInteger a, BigInteger b)
        {
            BigInteger c;
            while (b != 0)
            {
                c = a % b;
                a = b;
                b = c;
            }
            return a;
        }
        
        private Func<int, BigInteger, bool> FermaTest = delegate (int bit, BigInteger n)
        {
            for (int i = 2; i < 2 + bit; i++)
                if ((gcd(n, i) != 1 || power(i, n - 1, n) != 1))
                    return false;
            return true;
        };

        private static Func<int, BigInteger, bool> SolowayTest = delegate (int bit, BigInteger n)
        {
            for (int i = 0; i < bit; i++)
            {
                var a = next(2, n - 1);
                int x = qr(a, n);
                if (x == 0 || power(a, (n - 1) / 2, n) != (x + n) % n)
                {
                    return false;
                }
            }
            return true;
        };

        private static Func<int, BigInteger, bool> MillerTest = delegate (int bit, BigInteger n)
        {
            BigInteger t = n - 1;
            int s = 0;
            while ((t & 1) != 1)
            {
                t = t >> 1;
                s++;
            }

            for (int k = 0; k < bit; k++)
            {
                BigInteger y = new BigInteger();
                BigInteger a = next(2, n - 2);
                BigInteger x = power(a, t, n);
                for (int i = 0; i < s; i++)
                {
                    y = power(x, 2, n);
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


        private static int qr(BigInteger a, BigInteger n)
        {
            int t = 1;
            while (a != 0)
            {
                while (a % 2 == 0)
                {
                    a = a >> 1;
                    BigInteger r = n % 8;
                    if (r == 3 || r == 5)
                        t = -t;
                }
                BigInteger temp = a;
                a = n;
                n = temp;
                if (a % 4 == n % 4 && n % 4 == 3)
                    t = -t;
                a %= n;
            }
            if (n == 1)
                return t;
            else
                return 0;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Frm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Func<int, BigInteger, bool> Test = FermaTest;
                Button btn = (Button)sender;
                TextBox txt = new TextBox();
                if (btn == Frm)
                {
                    Test = FermaTest;
                    txt = Ferma;
                }
                if (btn == Slw)
                {
                    Test = SolowayTest;
                    txt = Soloway;
                }
                if (btn == Mlr)
                {
                    Test = MillerTest;
                    txt = Miller;
                }
                int bit = Convert.ToInt32(Bit.Text);
                var b = next(bit);
                if ((b & 1) != 1)
                    b++;
                while (!Test((int)Math.Log(bit, 2), b))
                    b += 2;
                txt.Text = b.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Smth went wrong");
            }

        }


        private void FrmSrch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int lim = Convert.ToInt32(limit.Text);
                Func<int, BigInteger, bool> Test = FermaTest;
                Button btn = (Button)sender;
                TextBox txt = new TextBox();
                Label x = new Label();
                if (btn == FrmSrch)
                {
                    Test = FermaTest;
                    txt = FermaSearch;
                    x = F;
                }
                if (btn == SlwSrch)
                {
                    Test = SolowayTest;
                    txt = SolowaySearch;
                    x = S;
                }
                if (btn == MlrSrch)
                {
                    Test = MillerTest;
                    txt = MillerSearch;
                    x = M;
                }
                int quantity = 2;
                StringBuilder sb = new StringBuilder();
                sb.Append("2 3 ");
                for (int i = 5; i <= lim; i += 2)
                {
                    if (Test((int)Math.Log(i, 2), i))
                    {
                        sb.Append($"{i} ");
                        quantity++;
                    }
                }
                x.Content = quantity.ToString();
                txt.Text = sb.ToString();
            }
            catch (Exception ex) {
                MessageBox.Show("Smth went wrong");
            }
        }

        private void FSS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                TextBox first = new TextBox();
                TextBox second = new TextBox();
                TextBox res = new TextBox();
                if (btn == FSS)
                {
                    first = FermaSearch;
                    second = SolowaySearch;
                    res = FSSdiff;
                }
                if (btn == SSMR)
                {
                    first = SolowaySearch;
                    second = MillerSearch;
                    res = SSMRdiff;
                }
                List<int> fst = new List<int>();
                List<int> snd = new List<int>();
                fst = first.Text.Trim(' ').Split(' ').Select(t => Convert.ToInt32(t)).ToList();
                snd = second.Text.Trim(' ').Split(' ').Select(t => Convert.ToInt32(t)).ToList();
                StringBuilder sb = new StringBuilder();
                IEnumerable<int> resLst = fst.Except(snd);
                foreach (var item in resLst)
                {
                    sb.Append(String.Format(item + " "));
                }
                res.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Smth went wrong");
            }
        }
    }
}
