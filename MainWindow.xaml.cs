using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using GenerateRandomText.Generators;
using Microsoft.Win32;

namespace GenerateRandomText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<IRandomWordGenerator> _randomWordListGenerators = new ObservableCollection<IRandomWordGenerator>();

        public MainWindow()
        {
            // Initialize Generators
            _randomWordListGenerators.Add(new GenerateRandomWord(3, 10));
            _randomWordListGenerators.Add(new GenerateRandomWordFromDictionary());

            InitializeComponent();

            FileSizeinKB.Text = "1024";

            // Initilize List of Generators
            GeneratorsListBox.ItemsSource = _randomWordListGenerators;
            GeneratorsListBox.SelectedIndex = 0;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            int KBs;
            string fileName;

            // Get size of the file
            try
            {
                KBs = int.Parse(FileSizeinKB.Text, NumberStyles.None);

                if (KBs < 1)
                {
                    MessageBox.Show("Please enter a valid positive number.", "File size not valid", MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter a valid positive number.", "File size not valid", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Ask for a file location
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (saveDialog.ShowDialog() == true)
                fileName = saveDialog.FileName;
            else
                return;

            // Get selected generator
            IRandomWordGenerator generator = GeneratorsListBox.SelectionBoxItem as IRandomWordGenerator;

            // Generate a list of words for given size and save
            if (generator != null)
            {
                FileWriter fileWriter = new FileWriter(generator, fileName, KBs);

                // subscribe to finished event
                fileWriter.GenerateFinished += elapsed =>
                {
                    // enable button on the UI thread
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // enable buttons
                        Generate.IsEnabled = true;
                        Close.IsEnabled = true;

                        // set status bar
                        Status.Text = "Generate finished in: " + elapsed + " milisec.";
                    }));
                };

                // disable buttons
                Generate.IsEnabled = false;
                Close.IsEnabled = false;

                Status.Text = "Generating...";
                await fileWriter.GenerateFile();
            }
        }

        private void FileSizeinMB_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // workaround for a ComboBox issue. It doesn't disable items
            GeneratorsListBox.IsDropDownOpen = true;
            GeneratorsListBox.IsDropDownOpen = false;
        }
    }
}
