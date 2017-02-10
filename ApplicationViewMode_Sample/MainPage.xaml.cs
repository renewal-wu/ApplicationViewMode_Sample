using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ApplicationViewMode_Sample
{
    public sealed partial class MainPage : Page
    {
        private ApplicationView LyricsView { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ShowCompactButton_Click(object sender, RoutedEventArgs e)
        {
            if (LyricsView != null)
            {
                return;
            }

            var mainWindow = CoreWindow.GetForCurrentThread();
            var mainViewId = ApplicationView.GetApplicationViewIdForWindow(mainWindow);

            #region Create a new View ,named LyricsView
            var currentView = CoreApplication.CreateNewView();
            await currentView.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                LyricsView = ApplicationView.GetForCurrentView();
                LyricsView.Consolidated += LyricsView_Consolidated;

                #region Set the content of Window in LyricsView
                var rootContainer = new Grid()
                {
                    Background = new SolidColorBrush()
                    {
                        Color = Colors.Black
                    }
                };
                var lyricTextBlock = new TextBlock()
                {
                    Foreground = new SolidColorBrush()
                    {
                        Color = Colors.White
                    },
                    Text = "I'm lyrics window.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 20
                };
                rootContainer.Children.Add(lyricTextBlock);
                Window.Current.Content = rootContainer;
                Window.Current.Activate();
                #endregion

                #region Show the LyricsView as standalone
                var currentWindow = CoreWindow.GetForCurrentThread();
                var viewId = ApplicationView.GetApplicationViewIdForWindow(currentWindow);
                var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewId, ViewSizePreference.Default, mainViewId, ViewSizePreference.Default);
                #endregion

                #region Set the LyricsView as CompactOverlay mode
                await LyricsView.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                #endregion
            });
            #endregion
        }

        private async void HideCompactButton_Click(object sender, RoutedEventArgs e)
        {
            if (LyricsView == null)
            {
                return;
            }

            await LyricsView.TryConsolidateAsync();
        }

        private void LyricsView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            if (LyricsView == null)
            {
                return;
            }

            LyricsView.Consolidated -= LyricsView_Consolidated;
            LyricsView = null;
        }
    }
}
