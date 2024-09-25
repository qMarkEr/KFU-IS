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
using System.Windows.Shapes;using System.Windows.Threading;

namespace RSA
{
    public partial class MainWindow : Window
    {
        private const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789 ";
        public BigInteger publicKey = new(); 
        private BigInteger privateKey = new(); 
        private BigInteger mod = new();
        private bool keys = false;
        private bool isGen = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {  
            try {
                
                string text = input.Text.ToLower();
                if (text != String.Empty && keys == true)
                {
                    bool characterCheck = true;
                    StringBuilder sb = new();
                    for (int i = 0; i < text.Length && characterCheck; i++)
                    {
                        int index = alphabet.IndexOf(text[i]);
                        sb.Append(BigInteger.ModPow(index, publicKey, mod) + " ");
                        if (index == -1) characterCheck = false;
                    }
                    if (characterCheck)
                        cryptogram.Text = sb.ToString();
                    else
                        cryptogram.Text = "Error";
                } else
                    cryptogram.Text = "Error";
            }
            catch
            {
                cryptogram.Text = "Error";
            }
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                if (isGen == false)
                {
                    isGen = true;
                    int bit = Convert.ToInt32(bitLength.Text);
                    pOut.Text = String.Empty;
                    qOut.Text = String.Empty;
                    nOut.Text = String.Empty;
                    eOut.Text = String.Empty;
                    dOut.Text = String.Empty;
                    phiOut.Text = String.Empty;
                    var p = SomeLongRunningTaskAsync(pOut, bit);
                    var q = SomeLongRunningTaskAsync(qOut, bit);
                    keys = true;
                    RSA(await p, await q);
                    isGen = false;
                }
                }
            catch
            {
                bitLength.Text = "Error";
            }

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var text = cryptogram.Text.Trim(' ').Split(' ');
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                    sb.Append(alphabet[(int)(BigInteger.ModPow(BigInteger.Parse(text[i]), privateKey, mod) % alphabet.Length)]);

                output.Text = sb.ToString();
            }
            catch {
                output.Text = "Error";
            }
        }
        private void RSA(BigInteger p, BigInteger q)
        {
            BigInteger n = p * q;
            nOut.Text = n.ToString();

            BigInteger phi = (p - 1) * (q - 1);
            phiOut.Text = phi.ToString();

            BigInteger e = getE(phi);
            eOut.Text = e.ToString();

            BigInteger d = 0, y = 0;
            BigInteger g = gcdex(e, phi, ref d, ref y);
            d = (d % phi + phi) % phi;
            dOut.Text = d.ToString();

            privateKey = d;
            publicKey = e;
            mod = n;
            
        }
        public static BigInteger gcdex(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            BigInteger x1 = 1, y1 = 1;
            BigInteger gcd = gcdex(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }
        private BigInteger getE(BigInteger phi)
        {
            BigInteger e = next(1, phi);
            while (gcd(e, phi) != 1)
            {
                if (e > 2)
                {
                    e -= 1;
                } else
                {
                    e = next(1, phi);
                }
            }
            return e;
        }
        private Task<BigInteger> SomeLongRunningTaskAsync(TextBox txb, int bit)
        {
            return Task.Run(() => action(txb, bit));
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
                BigInteger x = BigInteger.ModPow(a, t, n);
                for (int i = 0; i < s; i++)
                {
                    y = BigInteger.ModPow(x, 2, n);
                    if (y == 1 && x != 1 && x != n - 1)
                        return false;
                    x = y;
                }
                if (y != 1)
                    return false;

            }
            return true;
        };

        public static BigInteger next(int bit)
        {
            const int chunk = 8;
            StringBuilder sb = new StringBuilder();
            Random r = new();
            if (bit % chunk != 0)
                for (int i = 0; i < chunk - bit % chunk; i++)
                    sb.Append('0');
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

        BigInteger action(TextBox txb, object obj)
        {
            int bit = (int)obj;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    txb.Text = "Generation. Please wait...";
                }
            );
            BigInteger b = next(bit);
            if ((b & 1) == 0)
                b++;

            while (!MillerTest((int)Math.Log(bit, 2), b))
            while (!MillerTest((int)Math.Log(bit, 2), b))
            {
                if (b < (int)Math.Pow(2, bit) - 2)
                    b += 2;
                else
                    b = next(bit);
            }


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    txb.Text = b.ToString();
                }
            );
            return b;
        }
    }
}
