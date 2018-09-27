using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HHChaosToolkit.UWP.Services.Navigation
{
    public class NavigationServiceList
    {
        public static readonly string DefaultNavigationServiceKey = "Default";
        private static NavigationServiceList _instance;

        private readonly Dictionary<string, NavigationService> _services = new Dictionary<string, NavigationService>();

        private NavigationServiceList()
        {
            Register(DefaultNavigationServiceKey, new NavigationService());
        }

        public static NavigationServiceList Instance => _instance ?? (_instance = new NavigationServiceList());

        public NavigationService Default => _services[DefaultNavigationServiceKey];

        public NavigationService this[string key] => _services[key];

        public void Register(string key, NavigationService service)
        {
            if (!IsRegistered(key))
                _services.Add(key, service);
        }

        public void RegisterOrUpdateFrame(string key, Frame frame)
        {
            if (!IsRegistered(key))
                _services.Add(key, new NavigationService { Frame = frame });
            else
                _services[key].Frame = frame;
        }

        public void Unregister(string key)
        {
            if (IsRegistered(key))
                _services.Remove(key);
        }

        public bool IsRegistered(string key)
        {
            return _services.ContainsKey(key);
        }
    }
}
