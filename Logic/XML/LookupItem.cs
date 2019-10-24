﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logic.XML
{
    [XmlRoot]
    public class LookupItem
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
