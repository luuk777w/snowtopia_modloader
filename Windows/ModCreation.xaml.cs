using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnowTopia_ModLoader
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ModCreation : Window
    {
        public ModCreation()
        {
            InitializeComponent();
        }
        
        private void BrowseOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                AssemblyLocationLabel.Content = openFileDialog.FileName;
            }
        }
        private void SelectOutputOnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Snowtopia Mod file (*.stmod)|*.stmod";
            if (saveFileDialog.ShowDialog() == true)
            {
                OutputLocationLabel.Content = saveFileDialog.FileName;
            }
        }

        private void CreateOnClick(object sender, RoutedEventArgs e)
        {
            if(AssemblyLocationLabel.Content == null)
            {
                MessageBox.Show("Assembly path not set!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OutputLocationLabel.Content == null)
            {
                MessageBox.Show("Mod output path not set!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var AssemblyLocation = AssemblyLocationLabel.Content.ToString();
            var OutputLocation = OutputLocationLabel.Content.ToString();

            if(!File.Exists(AssemblyLocation))
            {
                MessageBox.Show("Assembly could not be found!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                File.Create(OutputLocation).Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Mod file could not be created!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var modCreator = new ModCreator(AssemblyLocation, OutputLocation);

            modCreator.CreateMod();
        }
    }
}
