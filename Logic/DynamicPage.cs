﻿using Logic.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlStructureComplat;

namespace Logic
{
    public class DynamicPage
    {
        public PS_ERIP Response { get; set; }
        public PS_ERIP Request { get; set; }
        public bool IsResponced { get => Response != null; }
    }
}
