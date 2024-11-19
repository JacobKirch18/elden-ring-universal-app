using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace eldenRingUniversalApp
{
    public sealed partial class MainPage : Page
    {

        private static ObservableCollection<BossViewModel> defeatedBosses = new ObservableCollection<BossViewModel>();

        // All of these are tests and should be removed 
        private ObservableCollection<BossViewModel> bossList;
        string[] drops = { "30,000 runes" };
        string noImageFound;
        public MainPage()
        {
            this.InitializeComponent();

            bossList = new ObservableCollection<BossViewModel>();
            noImageFound = @"Images\elden ring.jpg";

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ApplicationData.Current.LocalSettings.Values["defeated"] = JsonConvert.SerializeObject(defeatedBosses);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("defeated")) // To make sure it exists
            {
                defeatedBosses = JsonConvert.DeserializeObject<ObservableCollection<BossViewModel>>(
                    ApplicationData.Current.LocalSettings.Values["defeated"] as string);
            }

            if (e.Parameter is string defeatedBossList && defeatedBossList != "")
            {
                var bosses = JsonConvert.DeserializeObject<ObservableCollection<BossViewModel>>(defeatedBossList);
                defeatedBosses = new ObservableCollection<BossViewModel>(bosses);
            }

            try
            {   
                await GetBosses();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unhandled exception in OnNavigatedTo: {ex.Message}");
            }
        }

        // I prompted ChatGpt "if I have an api that i'm using,
        // how could I loop through all the objects in the api and add them to a list"
        // To get some help with implementing the looping, then to get past an 
        // error from deserializing, ChatGpt gave me the idea of creating a 
        // wrapper class in Boss.cs
        public async Task<ObservableCollection<BossViewModel>> GetBosses()
        {
            List<BossViewModel> apiBossList = new List<BossViewModel>();

            const string requestUrl = "https://eldenring.fanapis.com/api/bosses?limit=100";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var json = await httpClient.GetStringAsync(requestUrl);
                    var bossWrapper = JsonConvert.DeserializeObject<BossWrapper>(json);
                    
                    if (bossWrapper == null || bossWrapper.Data == null)
                    {
                        throw new Exception("Invalid or empty response from the API.");
                    }

                    apiBossList = bossWrapper?.Data ?? new List<BossViewModel>();

                }
                catch (HttpRequestException ex)
                {
                    var errorDialog = new ContentDialog()
                    {
                        Title = "HttpClient error",
                        Content = ex.Message,
                        CloseButtonText = "OK",
                    };

                    await errorDialog.ShowAsync();
                }
            }

            foreach (var boss in apiBossList)
            {
                if (boss.Image == null)
                {
                    boss.Image = noImageFound;
                }
                bossList.Add(boss);
            }

            return bossList;
        }

        private void compendiumButton_Click(object sender, RoutedEventArgs e)
        {
            string defeated = JsonConvert.SerializeObject(defeatedBosses);
            this.Frame.Navigate(typeof(CompendiumPage), defeated);
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
                BossViewModel defeatedBoss = clickedButton.DataContext as BossViewModel;
                if (defeatedBoss != null && !defeatedBosses.Contains(defeatedBoss))
                {

                    defeatedBosses.Add(defeatedBoss);
                    defeatedText.Text = defeatedBoss.Name + " added to Compendium";

                    // I asked ChatGPT "how could I flash a little notification pop up in my page?" to learn about Popup
                    // I also asked ChatGPT "I'm trying to have it centered, how could I do that" and got the following code

                    // Get the size of the current window
                    var windowBounds = Window.Current.Bounds;

                    // Get the Popup's content size
                    NotificationPopup.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var popupSize = NotificationPopup.Child.DesiredSize;

                    // Calculate the center position
                    double horizontalOffset = (windowBounds.Width - popupSize.Width) / 2;
                    double verticalOffset = (windowBounds.Height - popupSize.Height) / 2;

                    // Set the Popup's offset
                    NotificationPopup.HorizontalOffset = horizontalOffset;
                    NotificationPopup.VerticalOffset = verticalOffset;
                    NotificationPopup.IsOpen = true;

                    // Automatically close the popup after 3 seconds
                    Task task = Task.Delay(3000).ContinueWith(t =>
                    {
                        _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            NotificationPopup.IsOpen = false;
                        });
                    });
                    // Remove boss from Main or show that he was defeated in Main
                }
                else if (defeatedBosses.Contains(defeatedBoss))
                {
                    defeatedText.Text = defeatedBoss.Name + " already in compendium";

                    // I asked ChatGPT "how could I flash a little notification pop up in my page?" to learn about Popup
                    // I also asked ChatGPT "I'm trying to have it centered, how could I do that" and got the following code

                    // Get the size of the current window
                    var windowBounds = Window.Current.Bounds;

                    // Get the Popup's content size
                    NotificationPopup.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var popupSize = NotificationPopup.Child.DesiredSize;

                    // Calculate the center position
                    double horizontalOffset = (windowBounds.Width - popupSize.Width) / 2;
                    double verticalOffset = (windowBounds.Height - popupSize.Height) / 2;

                    // Set the Popup's offset
                    NotificationPopup.HorizontalOffset = horizontalOffset;
                    NotificationPopup.VerticalOffset = verticalOffset;
                    NotificationPopup.IsOpen = true;

                    // Automatically close the popup after 3 seconds
                    Task task = Task.Delay(3000).ContinueWith(t =>
                    {
                        _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            NotificationPopup.IsOpen = false;
                        });
                    });
                }

            }
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
            if (searchTextBox.Text ==  string.Empty)
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

            var results = bossList.Where(boss => boss.Name.ToLower().Contains(searchText)).ToList();

            bossListView.ItemsSource = results;

            if (searchTextBox.Text == string.Empty || searchTextBox.Text == "Type to search...")
            {
                bossListView.ItemsSource = bossList;
            }
        }
    }
}
