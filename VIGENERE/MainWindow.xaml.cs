using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace Vigenere_OS
{
    public class Alphabet
    {
        public bool alphabetMode = false;
        private static string Latin = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static string Cyrillic = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";

        public string Language
        {
            get => alphabetMode ? Cyrillic : Latin;
        }
        public int Power
        {
            get => alphabetMode ? Cyrillic.Length : Latin.Length;
        }
    }
    public partial class MainWindow : Window
    {
        private static Alphabet alphabet = new();
        public MainWindow()
        {
            InitializeComponent();
        }
        Func<char, char, string> shift = (char key, char data) =>
            alphabet.Language[(alphabet.Language.IndexOf(data) + alphabet.Language.IndexOf(key)) % alphabet.Power].ToString();

        Func<char, char, string> shiftReverse = (char key, char data) =>
            alphabet.Language[(alphabet.Language.IndexOf(data) - alphabet.Language.IndexOf(key) + alphabet.Power) % alphabet.Power].ToString();
        private void Encrypt(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox BaseText;
                TextBox ResultText;
                var btn = (Button)sender;
                Func<char, char, string> shft;
                if (btn == EncryptBtn)
                {
                    shft = shift;
                    BaseText = input;
                    ResultText = encrypted;
                }
                else
                {
                    shft = shiftReverse;
                    BaseText = encrypted;
                    ResultText = deciphered;
                }
                if (CheckField.CheckText(key.Text, alphabet.Language, "key"))
                {
                    string shift_key = key.Text;
                    if (CheckField.CheckText(BaseText.Text.ToLower(), alphabet.Language, "Input"))
                    {
                        StringBuilder sb = new StringBuilder(String.Empty);
                        for (int i = 0; i < BaseText.Text.Length; i++)
                        {
                            sb.Append(shft(Char.ToLower(shift_key[i % shift_key.Length]), Char.ToLower(BaseText.Text[i])));
                        }
                        ResultText.Text = sb.ToString();
                    }
                    else
                    {
                        ResultText.Text = String.Empty;
                    }
                }
                else
                {
                    ResultText.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong");
            }
        }
        private void changeAlphabet(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            Brush active = new SolidColorBrush(System.Windows.Media.Color.FromRgb(130, 183, 199));
            Brush nonActive = new SolidColorBrush(System.Windows.Media.Color.FromRgb(26, 138, 152));
            button.Background = active;
            if (button == RU)
            {
                EN.Background = nonActive;
                alphabet.alphabetMode = true;
            }
            else
            {
                RU.Background = nonActive;
                alphabet.alphabetMode = false;
            }
        }
    }
}
