﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp.Helpers
{
    public interface iItemsSource
    {
        IEnumerable ItemsSource { get; set; }
    }
}