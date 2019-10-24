﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logic.XML.Check
{
    public partial class CheckHeader
    {
        [XmlElement]
        public List<CheckLine> CheckLine { get; set; }
        [XmlAttribute]
        public int Count { get; set; }
    }
}