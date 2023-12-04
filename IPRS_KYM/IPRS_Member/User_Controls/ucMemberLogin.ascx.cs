using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Routing;
using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Microsoft.AspNet.FriendlyUrls;
namespace IPRS_Member.User_Controls
{


    public partial class ucMemberLogin : System.Web.UI.UserControl
    {
        /// page_load line no 42-69 , btnLogin_Click line no 86-189,UpdateLoginStatus line no 204-213, RememberMe line no 220-256

        /// <summary>
        /// assign Y value to strDisplayRemembermer for region divRememberMe visible or not
        /// assign N value to strDisplayForgotPassword for region divForgotPassword visible or not
        /// assign N value to strDisplayRegistration for region divRegistration visible or not
        /// we need some comman code like for validation, calculation, data in entire project,
        /// so in this file(GeneralFunction.cs) we create comman code and use in where its neccessary 
        /// : Comment By Rohit
        /// </summary>
        #region Declare Public Variable and create object for app class file
        public string strDisplayRemembermer = "Y";
        public string strDisplayForgotPassword = "N";
        public string strDisplayRegistration = "N";
        GeneralFunction objGeneralFunction = new GeneralFunction();
        #endregion

        /// <summary>
        /// in the page load event if page is not postback  
        /// if there is member previously selected remember me option then we get information from cookies for auto login
        /// : Comment By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Page Load event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strDisplayRemembermer == "N")
                divRememberMe.Visible = false;

            if (strDisplayForgotPassword == "N")
                divForgotPassword.Visible = false;

            if (strDisplayRegistration == "N")
                divRegistration.Visible = false;

            if (!IsPostBack)
            {
                #region GETTING COOKIES INFORMATION FOR AUTO LOGIN
                if (Request.Cookies["MemberInfo"] != null)
                {
                    HttpCookie objCookie = Request.Cookies.Get("MemberInfo");
                    txtLoginName.Text = objCookie.Values["LoginName"];
                    txtPassword.Attributes.Add("value", Convert.ToString(objCookie.Values["Password"]));
                    chkRememberMe.Checked = true;
                }
                #endregion GETTING COOKIES INFORMATION FOR AUTO LOGIN

                dvMessage.Visible = false;
            }
        }

        #endregion

        /// <summary>
        /// in the btnLogin event
        /// firstly we check login id is exist in database or not for that we get value from txtLoginName(TextBox) and 
        /// pass those value to the App_Accounts_Login_IPM(Store procedure) as parameter. if record exist in database then we check passowrd is it 
        /// correct or not. if member choose the remeber option then call RememberMe() function. 
        /// after this we asgin value to the different Sessions which will be required 
        /// for further procedure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Button Login Click Event
        protected void btnLogin_Click(object sender, EventArgs e)
        {

            //RegisterRoutes(RouteTable.Routes);
            //Page.Response.Redirect("UpdateProfile", false);

            string str = clsCryptography.Decrypt("7rH6TQCRVCCCDyJ3SGdIZQ%3d%3d");
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@LoginName", txtLoginName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            DataSet myDataSet = new DataSet();
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            myDataSet = objDAL.GetDataSet("App_Accounts_Login_IPM", parameters.ToArray());

            if (myDataSet.Tables[0].Rows.Count == 0)
            {
                dvMessage.Visible = true;
                lblMessage.Text = "INVALID LOGIN NAME";
                return;
            }
            else
            {
                #region CHECK ACCOUNT STATUS
                //if (myDataSet.Tables[0].Rows[0]["RecordStatus"].ToString() == "1")
                //{
                //    dvMessage.Visible = true;
                //    lblMessage.Text = "ACCOUNT NOT ACTIVE. PLEASE CONTACT ADMINISTRATOR";
                //    return;
                //}
                #endregion CHECK ACCOUNT STATUS

                #region CHECK LOGIN PASSWORD
                if (myDataSet.Tables[0].Rows[0]["AccountPassword"].ToString() != clsCryptography.Encrypt(txtPassword.Text))
                {
                    dvMessage.Visible = true;
                    lblMessage.Text = "INVALID PASSWORD";

                    return;
                }
                #endregion CHECK LOGIN PASSWORD

            }//Else for if (myDataSet.Tables[0].Rows.Count==0)

            RememberMe();

            #region SETTING SESSION VARIABLES
            Session["AccountId"] = myDataSet.Tables[0].Rows[0]["AccountId"].ToString();
            Session["LoginName"] = myDataSet.Tables[0].Rows[0]["AccountEmail"].ToString().Trim();
            Session["AccountName"] = myDataSet.Tables[0].Rows[0]["AccountName"].ToString().Trim();
            Session["AccountAppStatus"] = myDataSet.Tables[0].Rows[0]["ApplicationStatus"].ToString();
            Session["AccountRegType"] = myDataSet.Tables[0].Rows[0]["AccountRegType"].ToString();
            Session["AccountStatus"] = myDataSet.Tables[0].Rows[0]["RecordStatus"].ToString();
            Session["AccountCode"] = myDataSet.Tables[0].Rows[0]["AccountCode"].ToString();
            Session["RefAccountCode"]= myDataSet.Tables[0].Rows[0]["RefAccountCode"].ToString();
            Session["ApplicationStatus"] = myDataSet.Tables[0].Rows[0]["ApplicationStatus"].ToString();
            Session["BusinessUnitId"] = myDataSet.Tables[0].Rows[0]["BusinessUnitId"].ToString();
            Session["BranchId"] = myDataSet.Tables[0].Rows[0]["BranchId"].ToString();
            Session["BookId"] = myDataSet.Tables[0].Rows[0]["BookId"].ToString();
            Session["BookType"] = myDataSet.Tables[0].Rows[0]["BookType"].ToString();
            Session["AuthorizationLevel"] = "1";
            if (myDataSet.Tables[0].Columns.Contains("ApprovalLogcount"))
                Session["ApprovalLogcount"] = myDataSet.Tables[0].Rows[0]["ApprovalLogcount"].ToString();
            if (File.Exists("~/DSIT/" + Session["AccountId"].ToString() + ".xml") == false)
            {
                try
                {
                    File.Delete(Server.MapPath("~/DSIT/" + Session["AccountId"].ToString() + ".xml"));
                }
                catch
                {
                    
                }
            }
            #endregion SETTING SESSION VARIABLES

            myDataSet.Dispose();
            //RegisterRoutes(RouteTable.Routes);
            #region "When Total Approval is 4 then allow to remove style and left menu just show the Update Profile form"

            if (Convert.ToString(Session["AccountStatus"]) == "1")
            {
                if (Convert.ToString(Session["ApplicationStatus"]) == "")
                    Page.Response.Redirect("UpdateProfile", false);
                else
                    Page.Response.Redirect("Information", false);//till authorization level completes
            }
            else
            {
                if (Convert.ToString(Session["ApplicationStatus"]) == "0")
                    Page.Response.Redirect("Information", false);
                else
                    Page.Response.Redirect("Home", false);
            }
            #endregion

        }

        #endregion

        /// <summary>
        /// in this function we check how many times member enter wrong password. 
        /// if member continuously enter wrong password in 3 times that account was lock.
        /// So, for that here we use App_Users_Update_Login_Status(store procedure) and also using App_Users_AuditTrail_Manage (store procedure)
        /// for maintain log evnt for every use. 
        /// </summary>
        /// <param name="objDAL"></param>
        /// <param name="strUserName"></param>
        /// <param name="strLoginType"></param>
        /// <param name="strAuditTrail"></param>
        #region Update Login Status
        protected void UpdateLoginStatus(DSIT_DataLayer objDAL, string strUserName, string strLoginType, string strAuditTrail)
        {
            #region INVALID LOGIN UPDATE LOCK COUNT
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@txtLoginName", clsCryptography.Encrypt(txtLoginName.Text), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@txtUserName", strUserName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@LoginType", strLoginType, SqlDbType.NVarChar, 5, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AuditTrail", strAuditTrail, SqlDbType.TinyInt, 0, ParameterDirection.Input));
            objDAL.ExecuteSP("App_Users_Update_Login_Status", parameters.ToArray());
            #endregion INVALID LOGIN UPDATE LOCK COUNT
        }

        #endregion

        /// <summary>
        /// In this function we check first the user selected checkbox (chkRememberMe) or not.
        /// If chkRememberMe are selected then we store login name and password value in Cookies
        /// </summary>
        #region Function Remember Me
        protected void RememberMe()
        {
            #region REMEMBER ME

            if (chkRememberMe.Checked == true)
            {
                if (Request.Cookies["MemberInfo"] != null)
                {
                    HttpCookie objCookie = Request.Cookies["MemberInfo"];
                    objCookie.Values.Remove("UserName");
                    objCookie.Values.Remove("Password");
                    objCookie.Values["LoginName"] = txtLoginName.Text.Trim();
                    objCookie.Values["Password"] = txtPassword.Text.Trim();
                    objCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(objCookie);
                }
                else
                {
                    HttpCookie objCookie = new HttpCookie("MemberInfo");
                    objCookie.Values["LoginName"] = txtLoginName.Text.Trim();
                    objCookie.Values["Password"] = txtPassword.Text.Trim();
                    objCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(objCookie);
                }
            }
            else
            {
                if (Request.Cookies["MemberInfo"] != null)
                {
                    HttpCookie objCookie = Request.Cookies["MemberInfo"];
                    objCookie.Values.Remove("LoginName");
                    objCookie.Values.Remove("Password");
                    Response.Cookies.Add(objCookie);
                }
            }
            #endregion REMEMBER ME
        }

        #endregion

        #region Function not in use
        private void RegisterRoutes(RouteCollection routes)
        {

            //routes.Clear();
            //routes.EnableFriendlyUrls();
            //routes.MapPageRoute("Login", "Login", "~/MemberLogin.aspx");
            //routes.MapPageRoute("Home", "Home", "~/Home.aspx");
            //routes.MapPageRoute("UpdateProfile", "UpdateProfile", "~/UpdateProfile.aspx");
            //routes.MapPageRoute("MemberRegistration", "MemberRegistration", "~/MemberRegistration.aspx");
            //routes.MapPageRoute("ForgotPassword", "ForgotPassword", "~/ForgotPassword.aspx");
            //routes.MapPageRoute("MemberLogout", "MemberLogout", "~/MemberLogout.aspx");
            //routes.MapPageRoute("MemberChangePassword", "MemberChangePassword", "~/MemberChangePassword.aspx");
            //routes.MapPageRoute("LogOut", "LogOut", "~/LogOut.aspx");


        }

        #endregion
    }


}