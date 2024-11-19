using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
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
    public sealed partial class CompendiumPage : Page
    {

        ObservableCollection<BossViewModel> defeatedBosses;

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

            if (e.Parameter is ObservableCollection<BossViewModel> bossList)
            {
                defeatedBosses = new ObservableCollection<BossViewModel>(bossList);
            }

            string radioButtonName = "rb0";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("checkedRadioButton"))
            {
                radioButtonName = ApplicationData.Current.LocalSettings.Values["checkedRadioButton"] as string;
            }

            if (radioButtonName == "rb0")
            {
                rb0.IsChecked = true;
            }
            else if (radioButtonName == "rb1")
            {
                rb1.IsChecked = true;
            }
            else if (radioButtonName == "rb2")
            {
                rb2.IsChecked = true;
            }
            else if (radioButtonName == "rb3")
            {
                rb3.IsChecked = true;
            }
            else if (radioButtonName == "rb4")
            {
                rb4.IsChecked = true;
            }
            else if (radioButtonName == "rb5")
            {
                rb5.IsChecked = true;
            }
            else if (radioButtonName == "rb6")
            {
                rb6.IsChecked = true;
            }
            else if (radioButtonName == "rb7")
            {
                rb7.IsChecked = true;
            }

            //updateHealthValues();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ApplicationData.Current.LocalSettings.Values["checkedRadioButton"] = findCheckedRadioButton();
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), defeatedBosses);
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

        private void removeBossButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                BossViewModel defeatedBoss = clickedButton.DataContext as BossViewModel;
                if (defeatedBoss != null)
                {
                    defeatedBosses.Remove(defeatedBoss);
                }
            }
        }

        private void editNicknameButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                // asked GitHub Copilot "I want a dialog popup to enter a boss nickname"
                var dialog = new ContentDialog
                {
                    Title = "Edit Nickname",
                    Content = new TextBox(),
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel"
                };

                dialog.PrimaryButtonClick += (s, args) =>
                {
                    var textBox = dialog.Content as TextBox;
                    if (textBox != null)
                    {
                        string newNickname = textBox.Text;
                        BossViewModel boss = clickedButton.DataContext as BossViewModel;
                        if (boss != null)
                        {
                            boss.Nickname = newNickname;
                        }
                    }
                };

                _ = dialog.ShowAsync();
            }
        }

        // these would not work because of the way the health string comes in, some would not parse correctly

        //private void radioButton_Click(object sender, RoutedEventArgs e) 
        //{
        //    updateHealthValues();
        //}

        //private void updateHealthValues()
        //{
        //    int mul = getMultiplier();
        //    foreach (BossViewModel boss in defeatedBosses)
        //    {
        //        if (boss.HealthPoints != "???")
        //        {
        //            // from https://learn.microsoft.com/en-us/dotnet/api/system.int32.tryparse?view=net-9.0
        //            if (int.TryParse(boss.HealthPoints, out int health))
        //            {
        //                health = health * mul;
        //                boss.HealthPoints = health.ToString();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Returns the number used in health calculations for different New Game+ Levels 
        /// </summary>
        //private int getMultiplier()
        //{
        //    string radioButtonName = findCheckedRadioButton();

        //    if (radioButtonName == "rb0")
        //    {
        //        return 0;
        //    }
        //    else if (radioButtonName == "rb1")
        //    {
        //        return 1;
        //    }
        //    else if (radioButtonName == "rb2")
        //    {
        //        return 2;
        //    }
        //    else if (radioButtonName == "rb3")
        //    {
        //        return 3;
        //    }
        //    else if (radioButtonName == "rb4")
        //    {
        //        return 4;
        //    }
        //    else if (radioButtonName == "rb5")
        //    {
        //        return 5;
        //    }
        //    else if (radioButtonName == "rb6")
        //    {
        //        return 6;
        //    }
        //    else if (radioButtonName == "rb7")
        //    {
        //        return 7;
        //    }
        //    else return 0;
        //}

        public string findCheckedRadioButton()
        {
            if (rb0.IsChecked == true)
            {
                return "rb0";
            }
            if (rb1.IsChecked == true)
            {
                return "rb1";
            }
            if (rb2.IsChecked == true)
            {
                return "rb2";
            }
            if (rb3.IsChecked == true)
            {
                return "rb3";
            }
            if (rb4.IsChecked == true)
            {
                return "rb4";
            }
            if (rb5.IsChecked == true)
            {
                return "rb5";
            }
            if (rb6.IsChecked == true)
            {
                return "rb6";
            }
            if (rb7.IsChecked == true)
            {
                return "rb7";
            }
            return "rb0";
        }

    }
}
