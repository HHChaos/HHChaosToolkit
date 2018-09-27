using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace HHChaosToolkit.UWP.Services.Navigation
{
    public class NavigatedFromEventArgs
    {
        //
        // 摘要:
        //     获取目标页内容的根节点。
        //
        // 返回结果:
        //     目标页内容的根节点。
        public object Content { get; set; }
        //
        // 摘要:
        //     获取一个值，该值指示导航期间的移动方向。
        //
        // 返回结果:
        //     枚举的一个值。
        public NavigationMode NavigationMode { get; set; }
    }
}
