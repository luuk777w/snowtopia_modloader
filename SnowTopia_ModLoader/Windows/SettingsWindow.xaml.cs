using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            AssemblyTB.Text = Settings.Default.AssemblyLocation;
            ModsFolderTB.Text = Settings.Default.ModFolderLocation;
        }

        private void BrowseOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                AssemblyTB.Text = openFileDialog.FileName;
            }
        }

        private void BrowseFolderOnClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ModsFolderTB.Text = dialog.FileName;
            }

        }

        private void SaveOnClick(object sender, RoutedEventArgs e)
        {
            if(File.Exists(AssemblyTB.Text))
            {
                Settings.Default.AssemblyLocation = AssemblyTB.Text;
                Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Assembly Path is not correct!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if(Directory.Exists(ModsFolderTB.Text))
            {
                Settings.Default.ModFolderLocation = ModsFolderTB.Text;
                Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Mod Folder does not exists!", "Snowtopia Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}
