using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Windows.Storage.Streams;
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
        ObservableCollection<BossViewModel> newGameDefeated;
        
        string noImageFound;
        List<string> bossNames; // Save this to local settings

        public CompendiumPage()
        {
            this.InitializeComponent();
            newGameDefeated = new ObservableCollection<BossViewModel>();
            
            noImageFound = @"Images\elden ring.jpg";
            List<string> bossNames = new List<string>();
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

        public async Task<ObservableCollection<BossViewModel>> GetBosses(string requestUrl)
        {
            List<BossViewModel> apiBossList = new List<BossViewModel>();

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
            }

            foreach (var boss in apiBossList)
            {
                if (boss.Image == null)
                {
                    boss.Image = noImageFound;
                }
                defeatedBosses.Add(boss);
            }

            return defeatedBosses;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // I gave ChatGpt a big rundown on how OnNavigatedTo was being called before
            // OnNavigatedFrom was finished and it gave me a couple solutions but the only
            // one I could really figure out how to do was creating a task delay so the
            // OnNavigatedFrom had time to finish
            await Task.Delay(100);

            StorageFolder current = ApplicationData.Current.LocalFolder;
            try
            {
                // file.Path is complete path
                StorageFile file = await current.GetFileAsync("test.txt");
                string textFromFile = await ReadFromFile(file); // should be Json bossNames
                if (textFromFile != null && textFromFile != "")
                {
                    bossNames = JsonConvert.DeserializeObject<List<string>>(textFromFile);
                }
            }
            catch (FileNotFoundException)
            {
                // Ignore
            }

            /*if (ApplicationData.Current.LocalSettings.Values.ContainsKey("defeated")) // To make sure it exists
            {
                bossNames = JsonConvert.DeserializeObject<List<string>>(
                    ApplicationData.Current.LocalSettings.Values["defeated"] as string);
            }*/

            if (e.Parameter is string bossList)
            {
                var bosses = JsonConvert.DeserializeObject<ObservableCollection<BossViewModel>>(bossList);
                defeatedBosses = new ObservableCollection<BossViewModel>(bosses);
            }

            string radioButtonName = "rb0";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("checkedRadioButton"))
            {
                radioButtonName = ApplicationData.Current.LocalSettings.Values["checkedRadioButton"] as string;
            }

            if (radioButtonName == "rb0")
            {
                rb0.IsChecked = true;
                newGameDefeated = defeatedBosses;
            }
            else if (radioButtonName == "rb1")
            {
                rb1.IsChecked = true;
                newGameDefeated = defeatedBosses; // NewGame+(2) and beyond multiply value
            }
            else if (radioButtonName == "rb2")
            {
                rb2.IsChecked = true;
                CreateNewGameValues(2);
            }
            else if (radioButtonName == "rb3")
            {
                rb3.IsChecked = true;
                CreateNewGameValues(3);
            }
            else if (radioButtonName == "rb4")
            {
                rb4.IsChecked = true;
                CreateNewGameValues(4);
            }
            else if (radioButtonName == "rb5")
            {
                rb5.IsChecked = true;
                CreateNewGameValues(5);
            }
            else if (radioButtonName == "rb6")
            {
                rb6.IsChecked = true;
                CreateNewGameValues(6);
            }
            else if (radioButtonName == "rb7")
            {
                rb7.IsChecked = true;
                CreateNewGameValues(7);
            }

            //updateHealthValues();
            countText.Text = $"Total Bosses: {defeatedBosses.Count()} | Total Names: {bossNames.Count}";

            ApplicationData.Current.LocalSettings.Values.Clear();

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

        // Got Math.Pow for exponents in C# from
        // https://education.launchcode.org/intro-to-programming-csharp/chapters/data-and-variables/operations.html#:~:text=The%20symbols%20%2B%20and%20%2D%20%2C%20and,all%20do%20what%20you%20expect.
        // Got the whole formula from ChatGpt, I gave the newGame+ values and the expected results,
        // and it created a formula for me, I translated it into code

        private void CreateNewGameValues(int newGame)
        {
            if (defeatedBosses != null)
            {
                newGameDefeated = new ObservableCollection<BossViewModel>();
                foreach (var boss in defeatedBosses)
                {
                    BossViewModel newBoss = new BossViewModel();
                    CopyBoss(boss, newBoss);
                    string healthPoints = newBoss.HealthPoints;
                    if (healthPoints != "???") // Maybe unecessary 
                    {
                        if (healthPoints != null && healthPoints[0] == '≈') 
                        {
                            healthPoints = healthPoints.Remove(0, 1);
                        }
                        if (int.TryParse(healthPoints, out int health))
                        {
                            double healthMultiplier = 1.025 + 0.0125 * (newGame - 2) + 0.0125 * Math.Pow((newGame - 2), 2);
                            newBoss.HealthPoints = (health * healthMultiplier).ToString();
                        }
                    }
                    newGameDefeated.Add(newBoss);
                    bossListView.ItemsSource = newGameDefeated;
                }
            }
        }

        private BossViewModel CopyBoss(BossViewModel original, BossViewModel newB)
        {
            newB.Id = original.Id;
            newB.Name = original.Name;
            newB.Nickname = original.Nickname;
            newB.Image = original.Image;
            newB.Description = original.Description;
            newB.Location = original.Location;
            newB.Drops = original.Drops;
            newB.HealthPoints = original.HealthPoints;
            return newB;
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ApplicationData.Current.LocalSettings.Values["checkedRadioButton"] = findCheckedRadioButton();
            await OnNavigatedFromHelper();
            /*List<string> bossNames = new List<string>();
            foreach (var boss in defeatedBosses)
            {
                bossNames.Add(boss.Name);
            }
            //ApplicationData.Current.LocalSettings.Values["defeated"] = JsonConvert.SerializeObject(bossNames);
            // ApplicationData.Current.LocalSettings.Values["defeated"] = JsonConvert.SerializeObject(defeatedBosses);

            // Save text
            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile file = await current.CreateFileAsync("test.txt", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(file, JsonConvert.SerializeObject(bossNames));
            string test = await ReadFromFile(file);*/
        }
        private async Task OnNavigatedFromHelper()
        {
            List<string> bossNames = new List<string>();
            foreach (var boss in defeatedBosses)
            {
                bossNames.Add(boss.Name);
            }
            //ApplicationData.Current.LocalSettings.Values["defeated"] = JsonConvert.SerializeObject(bossNames);
            // ApplicationData.Current.LocalSettings.Values["defeated"] = JsonConvert.SerializeObject(defeatedBosses);

            // Save text
            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile file = await current.CreateFileAsync("test.txt", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(file, JsonConvert.SerializeObject(bossNames));
            string test = await ReadFromFile(file);
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

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            string defeated = JsonConvert.SerializeObject(defeatedBosses);
            this.Frame.Navigate(typeof(MainPage), defeated);
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
                    newGameDefeated.Remove(defeatedBoss);
                    bossNames.Remove(defeatedBoss.Name);
                    countText.Text = $"Total Bosses: {defeatedBosses.Count()} | Total Names: {bossNames.Count}";
                }
            }
        }

        private void removeAllButton_Click(object sender, RoutedEventArgs e)
        {
            defeatedBosses.Clear();
            bossNames.Clear();
            newGameDefeated.Clear();
            countText.Text = $"Total Bosses: {defeatedBosses.Count()} | Total Names: {bossNames.Count}";
        }

        // Editing NickName does not work because I'm using a new initialized observable collection for the health points
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

        private void radio_Checked(object sender, RoutedEventArgs e)
        {
            string _checked = findCheckedRadioButton(); // Find out what button
            // Fix error, not correct values
            int newGame = _checked[2] - '0'; // Get int value from it
            if (newGame > 1)
            {
                CreateNewGameValues(newGame); // Reload page with new newGame value
            }
            else
            {
                if (defeatedBosses != null)
                {
                    bossListView.ItemsSource = defeatedBosses;
                }
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


    }
}
