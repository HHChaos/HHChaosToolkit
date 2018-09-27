using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.UWP.Mvvm
{
    public abstract class ViewModelBase : BindableBase, INavigable
    {
        public virtual void OnNavigatedFrom(NavigatedFromEventArgs e)
        {
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }
    }
}