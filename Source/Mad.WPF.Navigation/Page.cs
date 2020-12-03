using WinPage = System.Windows.Controls.Page;

namespace Mad.WPF.Navigation
{
    public class Page : WinPage
    {
        public virtual void OnNavigatedTo(NavigationEventArgs e) { }
    }
}
