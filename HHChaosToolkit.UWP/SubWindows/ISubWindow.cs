using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHChaosToolkit.UWP.SubWindows
{
    public interface ISubWindow
    {
        event EventHandler WindowClosed;
        void RaiseWindowClosedEvent();
        string SubWinKey { get; set; }
    }
}
