using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.Sample.ViewModels;
using HHChaosToolkit.UWP.Services.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HHChaosToolkit.Sample.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel => DataContext as ShellViewModel;
        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel.Initialize(NavigationView, ShellFrame);
            NavigationView.ItemInvoked += NavigationView_ItemInvoked;
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel?.ItemInvokedCommand?.Execute(args);
        }
    }
}
