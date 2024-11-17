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
using static System.Net.Mime.MediaTypeNames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace eldenRingUniversalApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompendiumPage : Page
    {

        ObservableCollection<Boss> defeatedBosses;

        public CompendiumPage()
        {
            this.InitializeComponent();
            //Boss AncestorSpirit = new Boss()
            //{
            //    Id = "17f69590896l0i1ul0hnmor8iyf9xd",
            //    Name = "Ancestor Spirit",
            //    Image = "https://eldenring.fanapis.com/images/bosses/17f69590896l0i1ul0hnmor8iyf9xd.png",
            //    Description = "A glowing spirit that takes the form of a large deer, making it a menacing threat when charging at targets.",
            //    Location = "Siofra River",
            //    Drops = new string[]
            //    {
            //        "13.000 Runes",
            //        "Ancestor Follower Ashes"
            //    },
            //    HealthPoints = "???"
            //};
            //defeatedBosses.Add(AncestorSpirit);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is List<Boss> bossList)
            {
                // Access the bossList data and perform any necessary operations
                // For example, you can bind it to a ListView or display it in the UI
               defeatedBosses = new ObservableCollection<Boss>(bossList);
            }
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void aboutGameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Visibility == Visibility.Visible)
            {
                searchTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                searchTextBox.Visibility = Visibility.Visible;
            }

        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == "Type to search...")
            {
                searchTextBox.Text = string.Empty;
            }
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == string.Empty)
            {
                searchTextBox.Text = "Type to search...";
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            // Prompted ChatGPT "i used a button to try to search through
            // a list view and display based on what the user types into a box,
            // how could I do this" and got the the next three lines of code to help me

            string searchText = searchTextBox.Text.ToLower();

            var results = defeatedBosses.Where(boss => boss.Name.ToLower().Contains(searchText)).ToList();

            bossListView.ItemsSource = results;

            if (searchTextBox.Text == string.Empty || searchTextBox.Text == "Type to search...")
            {
                bossListView.ItemsSource = defeatedBosses;
            }
        }
    }
}
