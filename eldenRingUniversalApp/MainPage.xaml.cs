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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace eldenRingUniversalApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private List<Boss> defeatedBosses = new List<Boss>();

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
                    defeatedBosses.Add(defeatedBoss);

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
            }
        }
    }
}
