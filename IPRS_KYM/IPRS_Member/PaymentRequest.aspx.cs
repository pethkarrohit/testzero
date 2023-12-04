using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;

namespace IPRS_Member
{
    public partial class PaymentRequest : System.Web.UI.Page
    {
        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;
        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_Load(object sender, EventArgs e)
        {

            //if (Request.QueryString["Amt"] != null)
            //    amount.Value = clsCryptography.Decrypt(Request.QueryString["Amt"].ToString());
            //if (Request.QueryString["rid"] != null)
            //    udf1.Value = clsCryptography.Decrypt(Request.QueryString["rid"].ToString());
            //if (Request.QueryString["fname"] != null)
            //    firstname.Value = clsCryptography.Decrypt(Request.QueryString["fname"].ToString());
            //if (Request.QueryString["email"] != null)
            //    email.Value = clsCryptography.Decrypt(Request.QueryString["email"].ToString());
            //if (Request.QueryString["phone"] != null)
            //    phone.Value = clsCryptography.Decrypt(Request.QueryString["phone"].ToString());
            if (Session["PayValues"] == null)
            {
                objGeneralFunction.BootBoxAlert("Error Sending Request", this.Page);
                return;
            }

            Hashtable HST_Pay = new Hashtable();
            HST_Pay = (Hashtable)Session["PayValues"];

            if (HST_Pay["PaymentRecieptId"].ToString() == "" || HST_Pay["PaymentRecieptId"].ToString() == "0")
            {
                objGeneralFunction.BootBoxAlert("Error Sending Request", this.Page);
                return;
            }

            amount.Value = HST_Pay["Amt"].ToString();
            udf1.Value = HST_Pay["AccountId"].ToString();
            udf2.Value = HST_Pay["PaymentRecieptId"].ToString();
            firstname.Value = HST_Pay["fname"].ToString();
            email.Value = HST_Pay["email"].ToString();
            phone.Value = HST_Pay["phone"].ToString();
            hdnTxnId.Value = HST_Pay["TransactionNo"].ToString();
            productinfo.Value = "IPRS Member Registration";
            Session.Clear();
            Session.Abandon();
            surl.Value = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/PaymentResponse.aspx";
            furl.Value = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/PaymentResponse.aspx";

        }

        protected void collectData()
        {


            try
            {
                key.Value = ConfigurationManager.AppSettings["MERCHANT_KEY"];
                string[] hashVarsSeq;
                string hash_string = string.Empty;


                //if (string.IsNullOrEmpty(Request.Form["txnid"])) // generating txnid
                //{
                //    Random rnd = new Random();
                //    string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                //    txnid1 = strHash.ToString().Substring(0, 20);

                //}
                //else
                //{
                //    txnid1 = Request.Form["txnid"];
                //}

                txnid1 = hdnTxnId.Value;

                if (string.IsNullOrEmpty(Request.Form["hash"])) // generating hash value
                {
                    if (
                        string.IsNullOrEmpty(ConfigurationManager.AppSettings["MERCHANT_KEY"]) ||
                        string.IsNullOrEmpty(txnid1) ||
                        string.IsNullOrEmpty(Request.Form["amount"]) ||
                        string.IsNullOrEmpty(Request.Form["firstname"]) ||
                        string.IsNullOrEmpty(Request.Form["email"]) ||
                        string.IsNullOrEmpty(Request.Form["phone"]) ||
                        string.IsNullOrEmpty(Request.Form["productinfo"]) ||
                        string.IsNullOrEmpty(Request.Form["surl"]) ||
                        string.IsNullOrEmpty(Request.Form["furl"])
                        )
                    {
                        //error

                        return;
                    }

                    else
                    {
                        hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
                        hash_string = "";
                        foreach (string hash_var in hashVarsSeq)
                        {
                            if (hash_var == "key")
                            {
                                hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                                hash_string = hash_string + '|';
                            }
                            else if (hash_var == "txnid")
                            {
                                hash_string = hash_string + txnid1;
                                hash_string = hash_string + '|';
                            }
                            else if (hash_var == "amount")
                            {
                                hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                                hash_string = hash_string + '|';
                            }
                            else
                            {

                                hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                                hash_string = hash_string + '|';
                            }
                        }

                        hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

                        hash1 = Generatehash512(hash_string).ToLower();         //generating hash
                        action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";// setting URL

                    }


                }

                else if (!string.IsNullOrEmpty(Request.Form["hash"]))
                {
                    hash1 = Request.Form["hash"];
                    action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";

                }




                if (!string.IsNullOrEmpty(hash1))
                {
                    hash.Value = hash1;
                    txnid.Value = txnid1;

                    System.Collections.Hashtable data = new System.Collections.Hashtable(); // adding values in gash table for data post
                    data.Add("hash", hash.Value);
                    data.Add("txnid", txnid.Value);
                    data.Add("key", key.Value);
                    string AmountForm = Convert.ToDecimal(amount.Value.Trim()).ToString("g29");// eliminating trailing zeros
                    amount.Value = AmountForm;
                    data.Add("amount", AmountForm);
                    data.Add("firstname", firstname.Value.Trim());
                    data.Add("email", email.Value.Trim());
                    data.Add("phone", phone.Value.Trim());
                    data.Add("productinfo", productinfo.Value.Trim());
                    data.Add("surl", surl.Value.Trim());
                    data.Add("furl", furl.Value.Trim());
                    data.Add("lastname", lastname.Value.Trim());
                    data.Add("curl", curl.Value.Trim());
                    data.Add("address1", address1.Value.Trim());
                    data.Add("address2", address2.Value.Trim());
                    data.Add("city", city.Value.Trim());
                    data.Add("state", state.Value.Trim());
                    data.Add("country", country.Value.Trim());
                    data.Add("zipcode", zipcode.Value.Trim());
                    data.Add("udf1", udf1.Value.Trim());
                    data.Add("udf2", udf2.Value.Trim());
                    data.Add("udf3", udf3.Value.Trim());
                    data.Add("udf4", udf4.Value.Trim());
                    data.Add("udf5", udf5.Value.Trim());
                    data.Add("pg", pg.Value.Trim());


                    string strForm = PreparePOSTForm(action1, data);
                    Page.Controls.Add(new LiteralControl(strForm));

                }

                else
                {
                    //no hash

                }

            }

            catch (Exception ex)

            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }
        }
        private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (System.Collections.DictionaryEntry key in data)
            {

                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }


            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
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









        protected void btnsubmit_Click(object sender, EventArgs e)
        {

            //string Amount=string.Empty; string rid= string.Empty; string firstname= string.Empty; string email= string.Empty;
            //if (Request.QueryString["Amt"] != null)
            //     Amount = clsCryptography.Decrypt(Request.QueryString["Amt"].ToString());
            //if (Request.QueryString["rid"] != null)
            //     rid = clsCryptography.Decrypt(Request.QueryString["rid"].ToString());
            //if (Request.QueryString["fname"] != null)
            //    firstname = clsCryptography.Decrypt(Request.QueryString["fname"].ToString());
            //if (Request.QueryString["email"] != null)
            //    email = clsCryptography.Decrypt(Request.QueryString["email"].ToString());

            //string productinfo = "IPRS Membership";



            collectData();
        }

    }
}