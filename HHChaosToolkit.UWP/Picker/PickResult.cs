﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHChaosToolkit.UWP.Picker
{
    public class PickResult<T>
    {
        public bool Canceled { get; set; }
        public T Result { get; set; }
    }
}
