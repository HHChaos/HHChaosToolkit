using HHChaosToolkit.UWP.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HHChaosToolkit.Sample.Helpers
{
    public static class ToastHelper
    {
        public static void SendToast(string content, TimeSpan? duration = null)
        {
            var toast = new Toast(content);
            if (duration.HasValue)
            {
                toast.Duration = duration.Value;
            }
            toast.Show();
        }
        public static void SendCustomToast(string content, TimeSpan? duration = null)
        {
            var toast = new Toast(content);
            toast.Style = App.Current.Resources["CustomToastStyle"] as Style;
            if (duration.HasValue)
            {
                toast.Duration = duration.Value;
            }
            toast.Show();
        }
    }
}
