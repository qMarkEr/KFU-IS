using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RSA_DECRYPTION
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static BigInteger B = 13;
        static List<BigInteger> P = new() { 2, 3, 5 };
        private const string alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя";
        StringBuilder log = new();
        private int number = 1;
        static BigInteger mod;
        static BigInteger p;
       
        private int Iterations = 0;
        static BigInteger q;
        public MainWindow()
        {
            InitializeComponent();
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
        private readonly Func<BigInteger, BigInteger> g = x => x * x + 1;
        private readonly Func<BigInteger, BigInteger> g1 = x => x * x + x + 1;
        private readonly Func<BigInteger, BigInteger> g2 = x => x * x + 2 * x;
        static BigInteger test(BigInteger n)
        {


            List<BigInteger> Q = new();


            int tries = 0;
            while (true)
            {
                BigInteger M = 1;


                if (tries == 0)
                {
                    divisors();
                    for (int i = 0; i < P.Count; i++)
                        M *= BigInteger.Pow(P[i], (int)Math.Floor(Math.Log10((double)B) / Math.Log10((double)P[i])));
                    Q = Primes(B, (B * B));

                }
                BigInteger res = StageOne(n, M, Q);
                if (res != -1)
                    return res;

                tries++;
                // System.Console.WriteLine(tries);
                if (tries == 10000)
                {
                    B *= 2;
                    tries = 0;
                    System.Console.WriteLine(B);
                }


            }
        }
        private void Rho(BigInteger n, Func<BigInteger, BigInteger> g, CancellationTokenSource token) {
            BigInteger startX = 2;
            BigInteger x = startX;
            BigInteger y = 1; int i = 0; int stage = 2;
            while ((BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n) == 1 || 
                BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n) == n))
            {
                if (i == stage)
                {
                    y = x;
                    stage *= 2;
                }
                x = g(x) % n;
                if (x == startX)
                {
                    x += 1;
                    startX = x;
                }
                i++;
            }
            Iterations = i;
            token.Cancel();
            p = BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n);
            q = (n / BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n));
        }
        private void RhoM(BigInteger n, Func<BigInteger, BigInteger> g, CancellationTokenSource token)
        {
            BigInteger startX = 2;
            BigInteger x = 2;
            BigInteger y = 1; int i = 0; int stage = 2;
            StringBuilder stat = new StringBuilder("Please, stand by");
            while ((BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n) == 1 ||
                BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n) == n))
            {
                if (i == stage)
                {
                    y = x;
                    stage *= 2;
                }
                if (i % 500000 == 0)
                {
                    if ((i / 500000) % 3 == 0)
                    {
                        stat.Clear();
                        stat.Append("Please, stand by");
                    }
                    stat.Append(".");
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        Status.Content = stat.ToString();
                    }
                    );
                }
                x = g(x) % n;
                if (x == startX)
                {
                    x += 1;
                    startX = x;
                }
                i++;
            }
            Iterations = i;
            token.Cancel();
            p = BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n);
            q = (n / BigInteger.GreatestCommonDivisor(BigInteger.Abs(y - x), n));
        }

       /* private string RSA() {
            BigInteger phi = (p - 1) * (q - 1);
            BigInteger d = 0, y = 0;
            BigInteger g = gcdex(publicKey, phi, ref d, ref y);
            d = (d % phi + phi) % phi;
            log.AppendLine("\tPhi(N): " + phi);
            BigInteger SecretWord = BigInteger.Parse(sw.Text);
            BigInteger res = BigInteger.ModPow(SecretWord, d, mod);
            log.AppendLine("\tPrivate key: " + d);
            StringBuilder sb = new();
            while (res != 0)
            {
                sb.Insert(0, alphabet[(int)(res % 100 - 16)]);
                res /= 100;
            }
            return sb.ToString();
        }*/
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

        private static BigInteger plrd(BigInteger n)
        {
            BigInteger a = 2;
            while (true)
            {
                for (BigInteger j = 2; j < B + 1; j++)
                {
                    a = BigInteger.ModPow(a, j, n);
                    BigInteger d = BigInteger.GreatestCommonDivisor(a - 1, n);
                    if (1 < d && d < n)
                    {
                        p = d;
                        q = n / p;
                        return d;
                    }
                    if (j == B)
                    {
                        break;
                    }
                    if (j % 100 == 0)
                    {
                        a = BigInteger.ModPow(a, BigInteger.GreatestCommonDivisor(j, B), n);
                    }

                }
                for (int r = 2; r < Sqrt(B) + 1; r++)
                {
                    if (B % r == 0)
                    {
                        double e = BigInteger.Log(B, r);
                        BigInteger ap = BigInteger.ModPow(a, BigInteger.Pow(r, (int)e - 1), n);
                        BigInteger d = BigInteger.GreatestCommonDivisor(ap - 1, n);
                        if (1 < d && d < n)
                        {
                            p = d;
                            q = n / p;
                            return d;
                        }
                    }
                }
                B *= B;
                if (B >= n) return -1;
            }
        }
        static void divisors()
        {
            for (BigInteger i = P[P.Count - 1] + 2; i <= B; i += 2)
            {
                if (MillerTest(15, i))
                    P.Add(i);
            }
        }

        static List<BigInteger> Primes(BigInteger a, BigInteger b)
        {
            List<BigInteger> Q = new();
            for (BigInteger i = a; i <= b; i++)
                if (MillerTest(15, i))
                    Q.Add(i);

            return Q;
        }
       

        private static BigInteger StageOne(BigInteger n, BigInteger M, List<BigInteger> Q)
        {
            Random rnd = new();
            BigInteger a = next(2, n);
            while (BigInteger.GreatestCommonDivisor(n, a) != 1)
                a = rnd.Next();
            if (BigInteger.GreatestCommonDivisor(BigInteger.ModPow(a, M, n) - 1, n) != 1)
            {
                System.Console.WriteLine(BigInteger.GreatestCommonDivisor(BigInteger.ModPow(a, M, n) - 1, n));
                p = BigInteger.GreatestCommonDivisor(BigInteger.ModPow(a, M, n) - 1, n);
                q = n / p;
                return p;
            }
            else
                return StageTwo(a, M, n, Q);

        }
        private static BigInteger StageTwo(BigInteger a, BigInteger M, BigInteger n, List<BigInteger> Q)
        {
            BigInteger b = BigInteger.ModPow(a, M, n);
            BigInteger d = 1;
            int iterator = 0;
            while (iterator < Q.Count)
            {
                BigInteger c = BigInteger.ModPow(b, Q[iterator], n);
                d = BigInteger.GreatestCommonDivisor(n, c - 1);
                if (d == 1)
                    iterator++;
                else
                {
                    p = d;
                    q = n / p;
                    return d;
                }
            }
            return -1;
        }
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
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBox res = rho_multi_res;
            try
            {
                Button btn = (Button)sender;
                var cts = new CancellationTokenSource();
                log.AppendLine("WORD " + number + "{\n");
                Stopwatch stopwatch = new();
                stopwatch.Start();
                if (btn == rho_multi)
                {
                    mod = BigInteger.Parse(rho_multi_n.Text);
                    Task[] tsks =
                    {
                    Task.Run(() => RhoM(mod, g, cts)),
                    Task.Run(() => Rho(mod, g1, cts)),
                    Task.Run(() => Rho(mod, g2, cts))
                    };
                    await Task.WhenAny(tsks);
                } 
                if (btn == rho_single)
                {
                    mod = BigInteger.Parse(rho_single_n.Text);
                    res = rho_single_res;
                    
                    await Task.Run(() => RhoM(mod, g, cts));
                }
                if (btn == p_button)
                {
                    mod = BigInteger.Parse(p_n.Text);
                    res = p_res;

                    await Task.Run(() => plrd(mod));
                }
                stopwatch.Stop();
                log.Append("\tq:" + q);
                log.Append("\n\tp:" + p);
                log.Append("\n\tTime: " + stopwatch.Elapsed + "\n}");
                res.AppendText(log.ToString() + "\n");

            } catch
            {
                res.AppendText("WORD" + number + " {\n\tERROR\n}\n");
            }
            number++;
            log.Clear();
            Status.Content = "";
        }
    }
}