using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace IPRS_Member
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        /// <summary>
        /// create object for GeneralFunction
        /// Commented By Rohit
        /// </summary>
        GeneralFunction objGeneralFunction = new GeneralFunction();

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #endregion

        /// <summary>
        /// get password using Function(GetAccountData) and create design email send To Member EmailId 
        /// Commented By Rohit
        /// </summary>
        /// <param name="DT"></param>
        #region Send password on EmailID
        private void MailSend(DataTable DT)
        {
            try
            {
                string html = "";
                if (DT.Rows[0]["AccountRegType"].ToString() == "C")
                    html = "M/S ";
                else
                    html = "Dear ";
                html = html + DT.Rows[0]["AccountName"].ToString() + ",<br>Your UserName : <b>" + DT.Rows[0]["AccountEmail"].ToString() + "</b><br>";
                html = html + "Your Password : <b>" + clsCryptography.Decrypt(DT.Rows[0]["AccountPassword"].ToString()) + "</b>";
                string link = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Default.aspx";
                html += "<br>" + link;
                html += "<br><br>Thank You,<br>IPRS.ORG";

                System.Net.Mail.MailMessage CustomerMessage = new System.Net.Mail.MailMessage();
                CustomerMessage.From = new MailAddress("poornima@dreamsoftindia.com");
                CustomerMessage.To.Add(new MailAddress(txtLoginName.Text));
                CustomerMessage.Subject = "Forgot Password Mail from IPRS.ORG";
                CustomerMessage.IsBodyHtml = true;
                CustomerMessage.Body = html;
                SmtpClient Customer = new SmtpClient();
                Customer.Host = "mail.dreamsoftindia.com";
                Customer.Credentials = new System.Net.NetworkCredential("poornima-dreamsoftindia", "Poornima@18");
                Customer.Send(CustomerMessage);
                objGeneralFunction.BootBoxAlert("Your Password has been sent on your E-Mail ID !!", Page);
            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(ex.Message.ToString(), Page);
                return;
            }
        }
        #endregion

        /// <summary>
        /// Get Member Login Details Using Store Procedure(App_Accounts_Login_IPM)
        /// Commented By Rohit
        /// </summary>
        /// <returns></returns>
        #region Get Account Details
        public DataTable GetAccountData()
        {
            DataTable DT = new DataTable();
            try
            {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@LoginName", txtLoginName.Text, SqlDbType.VarChar, 100, ParameterDirection.Input));
                DT = objDAL.GetDataTable("App_Accounts_Login_IPM", parameters.ToArray());
                return DT;
            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(ex.Message.ToString(), Page);
                return DT;
            }
        }
        #endregion

        /// <summary>
        /// Get Account Data Using Function(GetAccountData) And Send Email To members EmailID by using Function(CreateEmailLog)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Send Email To Members EmailID
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            if (txtLoginName.Text == string.Empty)
            {
                objGeneralFunction.BootBoxAlert("Enter Email ID", Page);
                return;
            }
            try
            {
                DataTable DT = GetAccountData();
                if (DT == null)
                {
                    objGeneralFunction.BootBoxAlert("Invalid Email ID", Page);
                    return;
                }
                if (DT.Rows.Count == 0)
                {
                    objGeneralFunction.BootBoxAlert("Invalid Email ID", Page);
                    return;
                }
                else
                {
                    string link = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Default.aspx";
                    CreateEmailLog("MFP", link, DT);
                }
            }
            catch (Exception ee)
            {
                objGeneralFunction.BootBoxAlert(ee.Message.ToString(), Page);
            }
        }
        #endregion 

        /// <summary>
        /// Send Data For Class File(EmailConfig) to send Email To Member EmailID
        /// Commented By Rohit
        /// </summary>
        /// <param name="EmailType"></param>
        /// <param name="Link"></param>
        /// <param name="DT"></param>
        #region Create And Design Email 
        protected void CreateEmailLog(string EmailType, string Link, DataTable DT)
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            EmailConfig EmailConfig = new EmailConfig();
            Int64 ReturnId = 0;
            try
            {
                List<KeyValuePair<string, object>> Paralist = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("RecordKeyId", DT.Rows[0]["AccountId"].ToString()),
                };

                #region "Email Config Member"
                EmailConfig = new EmailConfig();
                EmailConfig.BookType = "AA";
                EmailConfig.EmailType = EmailType;
                EmailConfig.DTTransaction = DT;
                EmailConfig.Link = "<a href='" + Link + "' target='_blank'>Click here to Login</a>"; ;
                EmailConfig.ParaList = Paralist;
                EmailConfig.EmailTo = DT.Rows[0]["AccountEmail"].ToString();
                ReturnId = EmailConfig.CreateEmailLog();
                if (ReturnId > 0)
                {
                    objGeneralFunction.BootBoxAlert("Please check you mail box for password", Page);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}