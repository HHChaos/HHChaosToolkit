using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class ShellViewModel:ViewModelBase
    {
        public static readonly string ContentNavigationServiceKey = "HomeShellViewModel_Content";
        private NavigationView _navigationView;

        public NavigationService ContentService => NavigationServiceList.Instance.IsRegistered(ContentNavigationServiceKey) ? NavigationServiceList.Instance[ContentNavigationServiceKey] : null;

        public void Initialize(NavigationView navigationView,Frame frame)
        {
            _navigationView = navigationView;
            NavigationServiceList.Instance.RegisterOrUpdateFrame(ContentNavigationServiceKey, frame);
            _navigationView.SelectedItem = _navigationView.MenuItems.First();
            ContentService?.Navigate(typeof(MainViewModel).FullName);
        }
        private ICommand _itemInvokedCommand;
        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                return;
            }

            var item = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .First(menuItem => (string) menuItem.Content == (string) args.InvokedItem);
            var viewModelFullName = "HHChaosToolkit.Sample.ViewModels." + item.Tag;
            var viewModelType = Type.GetType(viewModelFullName);
            if (viewModelType != null)
                ContentService?.Navigate(viewModelType.FullName);
        }
    }
}
