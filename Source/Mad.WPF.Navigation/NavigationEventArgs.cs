using System;

namespace Mad.WPF.Navigation
{
    public delegate void NavigatedEventHandler(object sender, NavigationEventArgs? e);

    public sealed class NavigationEventArgs
    {
        public NavigationEventArgs(NavigationMode navigationMode, object content, Type sourcePageType, object? parameter)
        {
            NavigationMode = navigationMode;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            SourcePageType = sourcePageType ?? throw new ArgumentNullException(nameof(sourcePageType));
            Parameter = parameter;
        }

        public NavigationEventArgs(NavigationMode navigationMode, object content, Type sourcePageType)
           : this(navigationMode, content, sourcePageType, null) { }


        public object? Parameter { get; }
        public NavigationMode NavigationMode { get; }
        public object Content { get; }
        public Type SourcePageType { get; }
    }
}