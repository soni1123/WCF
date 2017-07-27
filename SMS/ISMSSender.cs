using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.SMS
{
    public interface ISMSSender
    {
        int sendTSMS(string url, string user, string pwd, string sid, string mtype, string recipient, string message);
        int sendTSMS_InvoiceGenerated(string url, string user, string pwd, string sid, string mtype, string recipient, string name, string invoice_id, string amount);
        int sendTSMS_RecieptGenerated(string url, string user, string pwd, string sid, string mtype, string recipient, string reciept_id);
        int sendTSMS_LoginCreated(string url, string user, string pwd, string sid, string mtype, string recipient, string email, string userpass, string userurl);

        #region Missed Call Alert
        string getMissedCalls(string startfrom);
        #endregion
    }
}