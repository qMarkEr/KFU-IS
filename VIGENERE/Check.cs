using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vigenere_OS
{
    internal static class CheckField
    {
        static Func<char, bool> CheckisDigitOrLetter = (c) => !char.IsLetter(c) && !char.IsDigit(c);
        static Func<char, bool> CheckisLetter = (c) => char.IsLetter(c);
        static Func<char, string, bool> CheckAlphabet = (c, str) => str.Contains(c);
        private static bool MainCheck(string data, string type, Func<char, bool> checkType)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show($"Empty {type}");
                return false;
            }
            for (int i = 0; i < data.Length; i++)
            {
                if (checkType(data[i]))
                {
                    MessageBox.Show($"Invalid {type}");
                    return false;
                }
            }
            return true;
        }
        public static bool CheckText(string data, string alphabet, string type)
        {

            if (MainCheck(data, type, CheckisDigitOrLetter))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (!alphabet.Contains(data[i]))
                    {
                        MessageBox.Show($"Wrong alphabet");
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public static bool CheckLetters(string data, string type)
        {
            return MainCheck(data, type, CheckisLetter);
        }
        public static bool CheckEmpty(string data, string type)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show($"Empty {type}");
                return false;
            }
            return true;
        }
        public static bool CheckDigits(string data, string type)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show($"Empty {type}");
                return false;
            }
            BigInteger k;
            if (!BigInteger.TryParse(data, out k))
            {
                MessageBox.Show($"Invalid {type}");
                return false;
            }

            return true;
        }
    }
}