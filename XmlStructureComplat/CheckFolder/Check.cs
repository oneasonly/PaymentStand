﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlStructureComplat.CheckFolder
{
    public partial class Check
    {
        [XmlElement]
        public CheckHeader CheckHeader { get; set; }

        [XmlElement]
        public CheckFooter CheckFooter { get; set; }
    }
}