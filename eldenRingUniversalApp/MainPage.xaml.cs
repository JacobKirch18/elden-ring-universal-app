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
using Windows.Storage.Streams;
using System.Security.Cryptography;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace eldenRingUniversalApp
{
    public sealed partial class MainPage : Page
    {

        private static ObservableCollection<BossViewModel> defeatedBosses = new ObservableCollection<BossViewModel>();
        private ObservableCollection<BossViewModel> bossList;
        string noImageFound;
        public MainPage()
        {
            this.InitializeComponent();

            // Uncomment this to fix any bad settings
            //ApplicationData.Current.LocalSettings.Values.Clear();

            bossList = new ObservableCollection<BossViewModel>();
            noImageFound = @"Images\elden ring.jpg";
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Task.Delay(100); // Wait for OnNavigatedFrom in Compendium

            if (e.Parameter is string defeatedBossList && defeatedBossList != "")
            {
                var bosses = JsonConvert.DeserializeObject<ObservableCollection<BossViewModel>>(defeatedBossList);
                defeatedBosses = new ObservableCollection<BossViewModel>(bosses);
            }

            else
            {
                StorageFolder current = ApplicationData.Current.LocalFolder;
                try
                {
                    // Uncomment this to clear any bad data from file
                    // StorageFile file = await current.CreateFileAsync("bosses.txt", CreationCollisionOption.ReplaceExisting);
                    
                    StorageFile file = await current.GetFileAsync("bosses.txt"); // Change file name
                    string textFromFile = await ReadFromFile(file); // should be Json string
                    if (textFromFile != null && textFromFile != "")
                    {
                        defeatedBosses = JsonConvert.DeserializeObject<ObservableCollection<BossViewModel>>(textFromFile);
                    }
                }
                catch (FileNotFoundException)
                {
                    // Ignore
                }
            }

            try
            {   
                await GetListBosses();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unhandled exception in OnNavigatedTo: {ex.Message}");
            }
        }
        private async Task<string> ReadFromFile(StorageFile storageFile)
        {
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                using (DataReader dataReader = new DataReader(stream))
                {
                    uint length = (uint)stream.Size;
                    await dataReader.LoadAsync(length);
                    return dataReader.ReadString(length);
                }
            }
        }
        // I prompted ChatGpt "if I have an api that i'm using,
        // how could I loop through all the objects in the api and add them to a list"
        // To get some help with implementing the looping, then to get past an 
        // error from deserializing, ChatGpt gave me the idea of creating a 
        // wrapper class in Boss.cs (changed to BossViewModel)
        public async Task<ObservableCollection<BossViewModel>> GetListBosses()
        {
            List<BossViewModel> apiBossList = new List<BossViewModel>();

            const string requestUrl = "https://eldenring.fanapis.com/api/bosses?limit=100";

            LoadingIndicator.IsActive = true;
            await Task.Delay(750);

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
                        Content = ex.Message + "\nCheck your internet connection",
                        CloseButtonText = "OK",
                    };

                    await errorDialog.ShowAsync();
                }
                finally
                {
                    LoadingIndicator.IsActive = false;
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
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            await OnNavigatedFromHelper();
        }
        private async Task OnNavigatedFromHelper()
        {
            // Save text
            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile file = await current.CreateFileAsync("bosses.txt", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(file, JsonConvert.SerializeObject(defeatedBosses));
        }
        private async Task SaveToFile(StorageFile storageFile, string contents)
        {
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter dataWriter = new DataWriter(stream))
                {
                    dataWriter.WriteString(contents);
                    await dataWriter.StoreAsync();
                }
            }
        }
        private void defeatButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                BossViewModel defeatedBoss = clickedButton.DataContext as BossViewModel;
                AddDefeatedBoss(defeatedBoss, false);
            }
        }
        private void allDefeatedButton_Click(object sender, RoutedEventArgs e)
        {
            if (defeatedBosses.Count == 100)
            {
                defeatedText.Text = "All bosses already defeated";
            }
            else if (defeatedBosses.Count == 0)
            {
                defeatedText.Text = "All bosses added to Compendium";
            }
            else
            {
                defeatedText.Text = "Remaining bosses added to Compendium";
            }
            foreach (var defeatedBoss in bossList)
            {
                AddDefeatedBoss(defeatedBoss, true);
            }
            Popup();
        }
        private void AddDefeatedBoss(BossViewModel defeatedBoss, bool allBosses)
        {
            if (!allBosses)
            {
                if (defeatedBoss != null && !defeatedBosses.Contains(defeatedBoss))
                {
                    defeatedBosses.Add(defeatedBoss);
                    defeatedText.Text = defeatedBoss.Name + " added to Copmendium";
                    Popup();
                }
                else
                {
                    defeatedText.Text = defeatedBoss.Name + " already in Compendium";
                    Popup();
                }
            }
            else
            {
                foreach (var boss in bossList)
                {
                    if (boss != null && !defeatedBosses.Contains(boss))
                    {
                        defeatedBosses.Add(boss);
                    }
                }
            }
        }
        private void Popup()
        {
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
        private void compendiumButton_Click(object sender, RoutedEventArgs e)
        {
            string defeated = JsonConvert.SerializeObject(defeatedBosses);
            this.Frame.Navigate(typeof(CompendiumPage), defeated);
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
                // Got the automatic focus of text block from SnyderCoder 
                // https://stackoverflow.com/questions/42243817/set-focus-to-a-textbox-in-uwp
                
                searchTextBox.Visibility = Visibility.Visible;
                searchTextBox.Focus(FocusState.Programmatic);
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
