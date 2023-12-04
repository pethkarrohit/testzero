using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace IPRS_Member
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            //using (var myWebRequest = new WebRequest())
            //{
            string StrKey = "xSCR0I";
            string StrSalt = "ccXiDgBZ";
            string method = "verify_payment";
            string varR1 = txtTransno.Text;
            string strhash = Generatehash512(StrKey + "|" + "verify_payment" + "|" + varR1 + "|" + StrSalt);
            string Url = "https://info.payu.in/merchant/postservice.php?form=2";
            PayDetail pDetail = new PayDetail { key = StrKey, command = "verify_payment", var1 = varR1, hash = strhash };
            //  client.BaseAddress = new Uri("https://info.payu.in/merchant/postservice.php?form=2");
            //  System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //var response = client.PutAsJsonAsync("api/person", p).Result;
            //        var response = client.PostAsync("api/verify_payment", new StringContent(new JavaScriptSerializer().Serialize(pDetail), Encoding.UTF8, "application/json"));
            //        response.Wait();
            //        var result = response.Result;


            //        if (result.IsSuccessStatusCode)
            //        {
            //            var customerJsonString = result.Content.ReadAsStringAsync();
            //            customerJsonString.Wait();
            //            var students = customerJsonString.Result;
            //            Console.Write("Success");
            //        }
            //        else
            //            Console.Write("Error");
            ////    }
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string postString = "key=" + StrKey +
               "&command=" + method +
               "&hash=" + strhash +
               "&var1=" + varR1;
            WebRequest myWebRequest = WebRequest.Create(Url);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            myWebRequest.Timeout = 180000;
            StreamWriter requestWriter = new StreamWriter(myWebRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(myWebRequest.GetResponse().GetResponseStream());
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, encode);

            string response = readStream.ReadToEnd();
            JObject account = JObject.Parse(response);
            String status = (string)account.SelectToken("transaction_details." + varR1 + ".status");
            divudf1.InnerText = (string)account.SelectToken("transaction_details." + varR1 + ".udf1");
            divResponseString.InnerText = account.ToString();

        }
        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }
        public class PayDetail
        {
            string _var1 = string.Empty;
            public string var1
            {
                get { return _var1; }
                set { _var1 = value; }
            }
            string _key = string.Empty;
            public string key
            {
                get { return _key; }
                set { _key = value; }
            }
            string _hash = string.Empty;
            public string hash
            {
                get { return _hash; }
                set { _hash = value; }
            }
            string _command = string.Empty;
            public string command
            {
                get { return _command; }
                set { _command = value; }
            }
        }

        protected void btnWorkRegFolder_Click(object sender, EventArgs e)
        {
            GetFile_Member();
        }

        public void GetFile_Member()
        {


            Hashtable HSTFile = new Hashtable();



            //HSTFile.Add("MemberPhoto", "MPU_" + MemberId.ToString() + "_");
            //HSTFile.Add("MemberRegDocs", "MRU_" + MemberId.ToString() + "_");
            HSTFile.Add("MemberRegWorkDocs", "MWN_");

            try
            {
                string FileNm = string.Empty;
                foreach (DictionaryEntry item in HSTFile)
                {


                    var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath(item.Key.ToString()), "*.*")
                                let x = new FileInfo(o)
                                where x.FullName.ToUpper().Contains(item.Value.ToString().ToUpper())
                                select o;
                    
                    foreach (var Queryitem in query)
                    {
                        FileNm = Path.GetFileName(Queryitem);

                        divwork.InnerHtml += "Update App_Accounts_WorkRegistration set DocLink='" + FileNm + "' where WorkNotificationId=" + FileNm.Split('_')[2]+"<br>";
                    }
                }
            }
            catch (Exception ex)
            {


            }



        }
    }
}