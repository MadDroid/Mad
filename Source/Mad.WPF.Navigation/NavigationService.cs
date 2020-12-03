using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Mad.WPF.Navigation
{
    public static class NavigationService
    {
        private sealed class NavigationData
        {
            public NavigationData(Page navigationPage, NavigationEventArgs navigationArgs)
            {
                NavigationPage = navigationPage ?? throw new ArgumentNullException(nameof(navigationPage));
                NavigationArgs = navigationArgs;
            }

            public Page NavigationPage { get; }
            public NavigationEventArgs NavigationArgs { get; }
        }

        #region Fields

        private static Frame? _frame;
        private static readonly object _navigationLock = new object();
        private static NavigationData? _navigationData;

        #endregion

        #region Properties

        public static Frame? Frame
        {
            get => _frame;
            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public static Page? CurrentPage => Frame?.Content as Page;

        public static bool CaGoBack => Frame?.CanGoBack ?? false;
        public static bool CaGoFoward => Frame?.CanGoForward ?? false;

        #endregion

        #region Events

        public static event NavigatedEventHandler? Navigated;
        public static event NavigationFailedEventHandler? NavigationFailed;

        #endregion

        #region Public Methods

        public static void GoBack()
        {
            if (Frame is null || !Frame.CanGoBack)
                return;

            Frame.GoBack();
        }

        public static void GoFoward()
        {
            if (Frame is null || !Frame.CanGoForward)
                return;

            Frame.GoForward();
        }

        public static bool Navigate<T>()
            where T : Page, new()
            => Navigate<T>(null);

        public static bool Navigate<T>(object? parameter)
            where T : Page, new()
        {
            if (Frame is null)
                return false;

            return Frame.Navigate(new T(), parameter);
        }

        #endregion

        #region Helpers

        private static void RegisterFrameEvents()
        {
            if (Frame is null)
                return;

            Frame.Navigated += Frame_Navigated;
            Frame.NavigationFailed += Frame_NavigationFailed;
            Frame.Navigating += Frame_Navigating;
        }

        private static void UnregisterFrameEvents()
        {
            if (Frame is null)
                return;

            Frame.Navigated -= Frame_Navigated;
            Frame.NavigationFailed -= Frame_NavigationFailed;
            Frame.Navigating -= Frame_Navigating;
        }

        #endregion

        #region Callbacks

        private static void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var page = (Page)e.Content;
            var navigationMode = e.NavigationMode switch
            {
                System.Windows.Navigation.NavigationMode.New => NavigationMode.New,
                System.Windows.Navigation.NavigationMode.Forward => NavigationMode.Forward,
                System.Windows.Navigation.NavigationMode.Back => NavigationMode.Back,
                System.Windows.Navigation.NavigationMode.Refresh => NavigationMode.Refresh,
                _ => throw new NotImplementedException()
            };

            lock (_navigationLock)
            {
                _navigationData = new(page, new NavigationEventArgs(navigationMode, e.Content, page.GetType(), e.ExtraData));
            }
        }

        private static void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            lock (_navigationLock)
            {
                if (_navigationData is not null)
                {
                    _navigationData.NavigationPage.OnNavigatedTo(_navigationData.NavigationArgs);
                }

                Navigated?.Invoke(sender, _navigationData?.NavigationArgs);

                _navigationData = null;
            }

        }

        private static void Frame_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            if (NavigationFailed is not null)
            {
                var args = new NavigationFailedEventArgs(e.Exception, CurrentPage?.GetType())
                {
                    Handled = e.Handled
                };

                NavigationFailed(sender, args);

                e.Handled = args.Handled;
            }
        }

        #endregion
    }
}
