using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace eldenRingUniversalApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // All of these are tests and should be removed 
        private ObservableCollection<Boss> bossList;
        string[] drops = { "30,000 runes" };
        string margitImagePath = @"Images\margit.jfif"; 
        public MainPage()
        {
            this.InitializeComponent();

            bossList = new ObservableCollection<Boss>()
            {
                new Boss { Id = "0", Description = "Test0", Drops = drops,
                    HealthPoints = "10", Location = "Limgrave", Name = "Margit1",
                    Image = margitImagePath
                },
                new Boss { Id = "1", Description = "Test1", Drops = drops,
                    HealthPoints = "10", Location = "Limgrave", Name = "Margit The Fell: Omen",
                    Image = margitImagePath
                },
                new Boss { Id = "3", Description = "Test3", Drops = drops,
                    HealthPoints = "10", Location = "Limgrave", Name = "Margit3",
                    Image = margitImagePath
                }
            };
        }

        private void compendiumButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CompendiumPage));
        }

        private void aboutGameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void defeatButton_Click(object sender, RoutedEventArgs e)
        {
            // When user defeats a boss
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                Boss defeatedBoss = clickedButton.DataContext as Boss;
                if (defeatedBoss != null)
                {
                    // Send defeatedBoss to Compendium
                    // Alert the user maybe?
                    // Remove boss from Main or show that he was defeated in Main
                }
            }
        }
    }
}
