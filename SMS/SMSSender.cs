using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
namespace tys361.WCF.SMS
{
    public class SMSSender : ISMSSender
    {
        public int sendTSMS(string url, string user, string pwd, string sid, string mtype, string recipient, string message)
        {
            mtype = "N";
            string SmsStatusMsg = string.Empty;
            try
            {
                //Sending SMS To User
                WebClient client = new WebClient();
                string URL = url + "?User=" + user + "&passwd=" + pwd + "&mobilenumber=" + recipient + "&message=" + message + "&sid=" + sid + "&mtype=" + mtype + "";

                SmsStatusMsg = client.DownloadString(URL);
                if (SmsStatusMsg.Contains("<br>"))
                {
                    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                }
                return 1;
            }
            catch (WebException e1)
            {
                SmsStatusMsg = e1.Message;
                return 0;
            }
            catch (Exception e2)
            {
                SmsStatusMsg = e2.Message;
                return 0;
            }
        }
        public int sendTSMS_InvoiceGenerated(string url, string user, string pwd, string sid, string mtype, string recipient, string name, string invoice_id, string amount)
        {
            mtype = "N";
            string SmsStatusMsg = string.Empty;
            try
            {
                //Sending SMS To User
                WebClient client = new WebClient();
                string URL = url + "?User=" + user + "&passwd=" + pwd + "&mobilenumber=" + recipient + "&message=XIPHIAS ATSI Dear " + name + ", NEW INVOICE " + invoice_id + " with Rs " + amount + " has generated. Login to view and download your invoice. Thanks for paying with us.&sid=" + sid + "&mtype=" + mtype + "";

                SmsStatusMsg = client.DownloadString(URL);
                if (SmsStatusMsg.Contains("<br>"))
                {
                    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                }
                return 1;
            }
            catch (WebException e1)
            {
                SmsStatusMsg = e1.Message;
                return 0;
            }
            catch (Exception e2)
            {
                SmsStatusMsg = e2.Message;
                return 0;
            }
        }

        public int sendTSMS_RecieptGenerated(string url, string user, string pwd, string sid, string mtype, string recipient, string reciept_id)
        {
            mtype = "N";
            string SmsStatusMsg = string.Empty;
            try
            {
                //Sending SMS To User
                WebClient client = new WebClient();
                string URL = url + "?User=" + user + "&passwd=" + pwd + "&mobilenumber=" + recipient + "&message=XIPHIAS ATSI : RECEIPT " + reciept_id + " for registration has generated.Login to view and download.&sid=" + sid + "&mtype=" + mtype + "";

                SmsStatusMsg = client.DownloadString(URL);
                if (SmsStatusMsg.Contains("<br>"))
                {
                    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                }
                return 1;
            }
            catch (WebException e1)
            {
                SmsStatusMsg = e1.Message;
                return 0;
            }
            catch (Exception e2)
            {
                SmsStatusMsg = e2.Message;
                return 0;
            }
        }

        public int sendTSMS_LoginCreated(string url, string user, string pwd, string sid, string mtype, string recipient, string email, string userpass, string userurl)
        {
            mtype = "N";
            string SmsStatusMsg = string.Empty;
            try
            {
                //Sending SMS To User
                WebClient client = new WebClient();
                string URL = url + "?User=" + user + "&passwd=" + pwd + "&mobilenumber=" + recipient + "&message=Welcome to XIPHIAS ATSI, Your online Credentials EMAIL " + email + " PASSWORD " + userpass + " URL " + userurl + "&sid=" + sid + "&mtype=" + mtype + "";

                SmsStatusMsg = client.DownloadString(URL);
                if (SmsStatusMsg.Contains("<br>"))
                {
                    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                }
                return 1;
            }
            catch (WebException e1)
            {
                SmsStatusMsg = e1.Message;
                return 0;
            }
            catch (Exception e2)
            {
                SmsStatusMsg = e2.Message;
                return 0;
            }
        }

        #region Missed Call Alerts
        public string getMissedCalls(string startfrom)
        {
            try
            {
                WebClient client = new WebClient();
                string URL = "http://manage.sarvsms.com/api/api_get_misscall_did_no_data.php?username=u5665&msg_token=sFvwLd&did_no=9699990026&start_from=" + startfrom + "";
                string response = client.DownloadString(URL);
                return response;
            }
            catch (WebException e1)
            {
                return e1.Message;
            }
            catch (Exception e2)
            {
                return e2.Message;
            }
        }
        #endregion
    }
}