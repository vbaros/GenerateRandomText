using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GenerateRandomText.Generators
{
    class GenerateRandomWordFromDictionary : IRandomWordGenerator
    {
        private readonly string[] _dictonary;
        private readonly Random _random = new Random();

        private readonly HashSet<string> _dictionary2; 

        public GenerateRandomWordFromDictionary()
        {
            // Load the dictionary
            try
            {
                _dictonary = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"words.txt");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(
                    "Dictionary Generator requires words.txt file to be placed in the same directory as the executable.\n" +
                    "The file should contain one word per line.\n" +
                    "Please put the file and restart the application.", "Dictionary file missing", MessageBoxButton.OK,
                    MessageBoxImage.Error);

                IsEnabled = false;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

                IsEnabled = false;
                return;
            }

            IsEnabled = true;
        }

        public char[] GetWord()
        {
            return _dictonary[_random.Next(0, _dictonary.Length - 1)].ToCharArray();
        }

        public string Name => "Random Words from Dictionary";
        public bool IsEnabled { get; }
    }
}
