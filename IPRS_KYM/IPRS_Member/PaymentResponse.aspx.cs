using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    public partial class PaymentResponse : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessPostPayment();

                tmrVerification.Enabled = true;
            }

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

        protected void ProcessPostPayment()
        {
            string Fname = string.Empty;
            string txn_id = string.Empty;
            hdnDataStatus.Value = "1";


            if (Request.Form["txnid"] == null || Request.Form["status"] == null)
            {
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "1";
                hdnErrorMsg.Value = "txnid Not Retrieved";
                return;

            }
            if (Request.Form["txnid"].ToString() == "")
            {
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "1";
                hdnErrorMsg.Value = "txnid Not Retrieved";
                return;

            }
            if (Request.Form["mihpayid"] == null)
            {
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "1";
                hdnErrorMsg.Value = "Problem in Retrieving Payu ID ";
                return;

            }
            if (Request.Form["mihpayid"].ToString() == "")
            {
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "1";
                hdnErrorMsg.Value = "Payu ID Not Retrieved";
                return;

            }
            if (Request.Form["status"].ToString() == "")
            {
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "1";
                hdnErrorMsg.Value = "Status Empty";
                return;
            }
            txn_id = Request.Form["txnid"];
            hdntxnid.Value = txn_id;
            try
            {
                string Salt = ConfigurationManager.AppSettings["SALT"];
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];
                hdnstatus.Value = Request.Form["status"].ToString();
                hdnResponseString.Value = Request.Form.ToString();
                hdnmhPayId.Value = Request.Form["mihpayid"].ToString();
                hdnstatus.Value = Request.Form["status"].ToString();
                hdnPaidAmount.Value = Request.Form["net_amount_debit"];
                hdnudf1.Value = Request.Form["UDF1"];
                hdnudf2.Value = Request.Form["UDF2"];
                try
                {
                    hdnAmount.Value = Request.Form["AMOUNT"];
                }
                catch (Exception) { }

                if (hdnstatus.Value.ToUpper() == "SUCCESS")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + Request.Form["status"];

                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        if (merc_hash_var.ToUpper() == "UDF1")
                        {
                            hdnudf1.Value = (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "");
                        }
                        if (merc_hash_var.ToUpper() == "AMOUNT")
                        {
                            hdnAmount.Value = (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "0");
                        }
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "");

                    }
                    merc_hash = Generatehash512(merc_hash_string).ToLower();
                    hdnmerc_hash_var.Value = merc_hash;

                    Fname = Request.Form["firstname"];
                    hdnfirstname.Value = Fname;
                    hdnmerc_hash.Value = Request.Form["hash"].ToString();
                    if (merc_hash != Request.Form["hash"])
                    {
                        divfail.Visible = true;
                        divsuccess.Visible = false;

                        hdnDataStatus.Value = "0";
                        hdnErrorMsg.Value = "Status=" + hdnstatus.Value + "--" + "Hash value did not matched";

                    }
                    else
                    {
                        // divmsg.InnerText = "value matched";
                        hdnDataStatus.Value = "0";
                        hdnErrorMsg.Value = "Status=" + hdnstatus.Value + "--" + "Hash value  matched";
                        divsuccess.Visible = true;
                        divfail.Visible = false;

                    }

                }

                else
                {
                    //  divmsg.InnerText = "status not recieved";
                    divfail.Visible = true;
                    divsuccess.Visible = false;
                    hdnDataStatus.Value = "1";
                    hdnErrorMsg.Value = "Status=" + hdnstatus.Value;



                }


            }

            catch (Exception ex)
            {
                // divmsg.InnerText = "EX : " + ex.ToString();
                divfail.Visible = true;
                divsuccess.Visible = false;
                hdnDataStatus.Value = "2";
                hdnErrorMsg.Value = "EX :" + ex.ToString();
                tmrVerification.Enabled = false;
            }

        }

        protected void tmrVerification_Tick(object sender, EventArgs e)
        {

            divPrograss.Visible = false;
            tmrVerification.Enabled = false;
            SavePaymentReciept(hdnDataStatus.Value, hdnErrorMsg.Value, hdnfirstname.Value);


        }
        protected void SavePaymentReciept(string status, string ErrorMsg, string Fname)
        {
            try
            {
                string AccountId = string.Empty;
                AccountId = hdnudf1.Value;
                string Amount = hdnAmount.Value;
                if (AccountId == "")
                    AccountId = "0";
                if (Amount == "")
                    Amount = "0";
                if (Amount == "0" && status == "0")
                {
                    status = "1";
                    ErrorMsg = ErrorMsg + "/" + "Invalid Amount";
                }

                string firstname = Convert.ToString(Session["AccountName"]);
                string email = Convert.ToString(Session["LoginName"]);
                DSIT_DataLayer DAL = new DSIT_DataLayer();
                DataTable DT = new DataTable();
                DT = DAL.GetDataTableSql("App_Accounts_RegPayment_List_IPM " + hdnudf1.Value + "," + hdnudf2.Value);
                if (DT.Rows.Count > 0)
                {

                    DataRow DR = DT.Rows[0];
                    DR["TransactionNo"] = hdntxnid.Value;
                    DR["PaidAmount"] = hdnPaidAmount.Value;
                    DR["PaymentStatus"] = status;
                    DR["PaymentGatewayResponse"] = ErrorMsg;
                    DR["ResponseNo"] = hdnmhPayId.Value;
                    DR["ResponseString"] = hdnResponseString.Value;
                    Int64 RecordId = 0;
                    string ReturnMessage = "";

                    objGeneralFunction.SavePaymentLog(DR, Fname, out ReturnMessage, out RecordId);


                    //var parameters = new List<SqlParameter>();
                    //string ReturnMessage = string.Empty;
                    //Int64 RecordId = 0;

                    //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@TransactionId", hdntxnid.Value, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PaymentAmount", Amount, SqlDbType.Money, 0, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PaidAmount", hdnPaidAmount.Value, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PaymentStatus", status, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PaymentGatewayResponse", ErrorMsg, SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@ResponseNo", hdnmhPayId.Value, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@ResponseString", hdnResponseString.Value, SqlDbType.NVarChar, 2000, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PaymentBankName", "", SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Fname, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

                    //DSIT_DataLayer objDAL = new DSIT_DataLayer();

                    //objDAL.ExecuteSP("App_Accounts_RegPayment_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);

                    if (status == "0" && RecordId > 0)
                    {
                        RecordId = objGeneralFunction.UpdatePaymentSubmission_CreateEmailLog(Convert.ToInt64(AccountId), Fname);
                        //DSIT_DataLayer objDAL = new DSIT_DataLayer();
                        //var parameters = new List<SqlParameter>();
                        //ReturnMessage = string.Empty;
                        //RecordId = 0;

                        //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        //parameters.Add(objGeneralFunction.GetSqlParameter("@ApplicationStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                        //parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Fname, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                        //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

                        //objDAL = new DSIT_DataLayer();

                        //objDAL.ExecuteSP("App_Accounts_AppStatus_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);

                        //CreateEmailLog(AccountId);
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }

        protected void CreateEmailLog(string AccountId)
        {

            DSIT_DataLayer DAL = new DSIT_DataLayer();
            EmailConfig EmailConfig = new EmailConfig();
            Int64 ReturnId = 0;
            try
            {


                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AuthorizationLevel", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));

                DataTable DT = DAL.GetDataTable("App_Accounts_List_Email", parameters.ToArray());

                List<KeyValuePair<string, object>> Paralist = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("RecordKeyId", AccountId),

                };
                ////-------------------------- GET BOOK NAME------------------

                //parameters = new List<SqlParameter>();
                //parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", DT.Rows[0]["BookId"].ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@BookType", "AA", SqlDbType.NVarChar, 4, ParameterDirection.Input));

                //DataTable DTBookName = DAL.GetDataTable("App_Get_BookName", parameters.ToArray());

                #region "Email Config Member"
                EmailConfig = new EmailConfig();
                EmailConfig.BookType = "AA";
                EmailConfig.EmailType = "MPS";
                EmailConfig.DTTransaction = DT;
                EmailConfig.ParaList = Paralist;
                EmailConfig.EmailTo = DT.Rows[0]["AccountEmail"].ToString();
                ReturnId = EmailConfig.CreateEmailLog();

                #endregion

                if (DT.Rows.Count > 0)
                {
                    #region "Email Config Member"

                    EmailConfig = new EmailConfig();
                    EmailConfig.BookType = "AA";
                    EmailConfig.EmailType = "MA1";
                    EmailConfig.DTTransaction = DT;
                    EmailConfig.EmailTo = DT.Rows[0]["Auth_EmailAddress"].ToString();
                    EmailConfig.RollType = DT.Rows[0]["MemberRoleType"].ToString();
                    EmailConfig.BookName = DT.Rows[0]["BookName"].ToString();
                    ReturnId = EmailConfig.CreateEmailLog();
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}