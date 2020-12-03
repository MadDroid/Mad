using System;

namespace Mad.WPF.Navigation
{
    public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    public class NavigationFailedEventArgs
    {
        public NavigationFailedEventArgs(Exception exception, Type? sourcePageType)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            SourcePageType = sourcePageType ?? throw new ArgumentNullException(nameof(sourcePageType));
        }

        public Exception Exception { get; }
        public Type? SourcePageType { get; }
        public bool Handled { get; set; }
    }
}