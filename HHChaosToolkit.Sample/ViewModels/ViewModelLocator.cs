﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.Sample.Views;
using HHChaosToolkit.Sample.Views.TestPages;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            if (!ServiceLocator.IsLocationProviderSet)
            {
                InitViewModelLocator();
            }
        }
        public ObjectPickerService ObjectPickerService => ServiceLocator.Current.GetInstance<ObjectPickerService>();
        public SubWindowsService SubWindowsService => ServiceLocator.Current.GetInstance<SubWindowsService>();

        public ShellViewModel ShellViewModel => ServiceLocator.Current.GetInstance<ShellViewModel>();
        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public NavigationServiceViewModel NavigationServiceViewModel => ServiceLocator.Current.GetInstance<NavigationServiceViewModel>();
        public PickerServiceViewModel PickerServiceViewModel => ServiceLocator.Current.GetInstance<PickerServiceViewModel>();
        public SubWindowsServiceViewModel SubWindowsServiceViewModel => ServiceLocator.Current.GetInstance<SubWindowsServiceViewModel>();
        public ToastSampleViewModel ToastSampleViewModel => ServiceLocator.Current.GetInstance<ToastSampleViewModel>();

        public TestNavigationViewModel1 TestNavigationViewModel1 => ServiceLocator.Current.GetInstance<TestNavigationViewModel1>();
        public TestNavigationViewModel2 TestNavigationViewModel2 => ServiceLocator.Current.GetInstance<TestNavigationViewModel2>();
        public TestColorPickerViewModel TestColorPickerViewModel => ServiceLocator.Current.GetInstance<TestColorPickerViewModel>();
        public TestInputDialogViewModel TestInputDialogViewModel => ServiceLocator.Current.GetInstance<TestInputDialogViewModel>();

        public TestSampleSubWindowViewModel TestSampleSubWindowViewModel => SimpleIoc.Default.GetInstanceWithoutCaching<TestSampleSubWindowViewModel>();

        public void InitViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register(() => new ObjectPickerService());
            SimpleIoc.Default.Register(() => new SubWindowsService());

            SimpleIoc.Default.Register<ShellViewModel>();
            RegisterNavigationService<MainViewModel, MainPage>(ShellViewModel.ContentNavigationServiceKey);
            RegisterNavigationService<NavigationServiceViewModel, NavigationServicePage>(ShellViewModel.ContentNavigationServiceKey);
            RegisterNavigationService<PickerServiceViewModel, PickerServicePage>(ShellViewModel.ContentNavigationServiceKey);
            RegisterNavigationService<SubWindowsServiceViewModel, SubWindowsServicePage>(ShellViewModel.ContentNavigationServiceKey);
            RegisterNavigationService<ToastSampleViewModel, ToastSamplePage>(ShellViewModel.ContentNavigationServiceKey);

            RegisterNavigationService<TestNavigationViewModel1, TestNavigationPage1>(NavigationServiceViewModel.ContentNavigationServiceKey);
            RegisterNavigationService<TestNavigationViewModel2, TestNavigationPage2>(NavigationServiceViewModel.ContentNavigationServiceKey);

            RegisterObjectPicker<Color, TestColorPickerViewModel, TestColorPickerPage>();
            RegisterObjectPicker<string, TestInputDialogViewModel, TestInputDialogPage>();

            RegisterSubWindow<TestSampleSubWindowViewModel, TestSampleSubWindowPage>();
        }
        public void RegisterNavigationService<VM, V>(string nsKey)
            where VM : ViewModelBase
        {
            SimpleIoc.Default.Register<VM>();
            if (!NavigationServiceList.Instance.IsRegistered(nsKey))
                NavigationServiceList.Instance.Register(nsKey, new NavigationService());
            var contentService = NavigationServiceList.Instance[nsKey];
            contentService.Configure(typeof(VM).FullName, typeof(V));
        }
        public void RegisterObjectPicker<T, VM, V>()
            where VM : ObjectPickerBase<T>
        {
            SimpleIoc.Default.Register<VM>();
            ObjectPickerService.Configure(typeof(T).FullName, typeof(VM).FullName, typeof(V));
        }
        public void RegisterSubWindow<VM, V>()
            where VM : SubWindowBase
        {
            SimpleIoc.Default.Register<VM>();
            SubWindowsService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
