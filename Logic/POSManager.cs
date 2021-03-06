﻿using ExceptionManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlStructureComplat;

namespace Logic
{
    public class POSManager
    {
        #region fields
        private static string url = @"http://public.softclub.by:8010/mdom_pos/online.request";
        //private string TerminalId = "KKM1";
        //private string Version = "1";
        #endregion
        #region Prop
        public MDOM_POS Request { get; set; }
        public MDOM_POS Response { get; set; }
        #endregion

        #region public methods
        public MDOM_POS GetCancelVOIRequest(PS_ERIP reqArg)
        {
            string KioskReceipt = reqArg.ResponseReq.KioskReceipt;
            string PC_ID = reqArg.ResponseReq.PayRecord?.FirstOrDefault()?.PC_ID;
            return GetCancelVOIRequest(PC_ID, KioskReceipt);
        }
        public MDOM_POS GetCancelVOIRequest(string argPCID, string argKioskReceipt)
        {
            SetInitialParams();
            Request.EnumType = PosQAType.VOIRequest;
            Request.ResponseReq.PC_ID = argPCID;
            Request.ResponseReq.KioskReceipt = argKioskReceipt;
            return Request;
        }
        public MDOM_POS CreatePayPosRequest(PayRecord payrecArg)
        {
            SetInitialParams();
            Request.EnumType = PosQAType.PURRequest;
            if (payrecArg == null) { Ex.Throw<ArgumentNullException>($"{nameof(CreatePayPosRequest)}(): argument {nameof(PayRecord)}=null"); }
            if (string.IsNullOrEmpty(payrecArg.Summa)) { Ex.Throw($"{nameof(CreatePayPosRequest)}(): argument PayRecord.Summa is null or empty"); }
            decimal summa = 0;
            decimal fine = 0;
            decimal commission = 0;
            Ex.Throw($"{nameof(CreatePayPosRequest)}(): не удалось конвертировать PayRecord.Summa='{payrecArg.Summa}' в цифровое значение"
                , () => summa = StringToDecimal(payrecArg.Summa));
            if (payrecArg.Fine != null)
            {
                Ex.Throw($"{nameof(CreatePayPosRequest)}(): не удалось конвертировать PayRecord.Fine='{payrecArg.Summa}' в цифровое значение"
                    , () => fine = StringToDecimal(payrecArg.Fine));
            }
            if (payrecArg.PayCommission != null)
            {
                Ex.Throw($"{nameof(CreatePayPosRequest)}(): не удалось конвертировать PayRecord.PayCommission='{payrecArg.Summa}' в цифровое значение"
                  , () => commission = StringToDecimal(payrecArg.PayCommission));
            }
            decimal PayAllPOS = summa + fine + commission;
            Request.ResponseReq.PaySumma = PayAllPOS.ToString().Replace(',', '.');
            return Request;
        }
        private static decimal StringToDecimal(string argString)
        {
            return decimal.Parse(argString.Replace(',', '.'), NumberStyles.Currency, CultureInfo.InvariantCulture);
        }
        #endregion

        private MDOM_POS SetInitialParams()
        {
            Request = new MDOM_POS();
            Request.ResponseReq.TerminalId = StaticMain.Settings.Terminal_MdomPOS.TerminalID;
            Request.ResponseReq.Version = StaticMain.Settings.Terminal_MdomPOS.version;
            return Request;
        }
    }
}
