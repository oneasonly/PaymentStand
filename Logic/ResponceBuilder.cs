﻿using Logic.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logic
{
    public static class ResponceBuilder
    {
        static ResponceBuilder()
        {
            //PostGetHTTP.XmlReceived += PostGetHTTP_XmlReceived;
        }

        public static void PostGetHTTP_XmlReceived(XDocument inc)
        {
            var x = inc.Root.Descendants("PayRecord");
            var z = inc.Root.Descendants("PayRecord");
            //throw new NotImplementedException();
        }

        static void Main(string[] args)
        {

        }

        public static object ResponseToRequest(PS_ERIP body, object arg2)
        {
            return null;
        }

        public static void NextPage( object select)
        { }
        
    }
}
