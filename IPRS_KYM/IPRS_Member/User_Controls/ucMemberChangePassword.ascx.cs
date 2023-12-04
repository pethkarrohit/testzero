using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace IPRS_Member.User_Controls
{
    public partial class ucMemberChangePassword : System.Web.UI.UserControl
    {
        public string strStrongPassword = "N";
        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_Load(object sender, EventArgs e)
        {

          

            if (!IsPostBack)
            {
                dvMessage.Visible = false;

                if (Session["AccountId"] != null)
                {
                    txtLoginName.Text = Session["LoginName"].ToString();
                    hdnUserName.Value = Session["LoginName"].ToString();
                    Session.Clear();
                    HttpContext.Current.Response.Cache.SetNoStore();
                }
                else
                {
                    objGeneralFunction.BootBoxAlert("SESSION EXPIRED. PLEASE LOGIN AGAIN", Page);
                }
            }

        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {



            if (txtPassword.Text == txtNewPassword.Text)
            {
                dvMessage.Visible = true;
                lblMessage.Text = "NEW PASSWORD HAS TO BE DIFFERENT FROM EXSITING PASSWORD";
                return;
            }

            if (txtNewPassword.Text.ToString() != txtConfirmPassword.Text.ToString())
            {
                dvMessage.Visible = true;
                lblMessage.Text = "NEW PASSWORD AND CONFIRM PASSWORD NEEDS TO MATCH";
                return;
            }

            //if (strStrongPassword == "Y")
            //{
            //    Regex sampleRegex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{8,10}$");
            //    bool isStrongPassword = sampleRegex.IsMatch(txtNewPassword.Text);

            //    if (isStrongPassword==false)
            //    {
            //        dvMessage.Visible = true;
            //        lblMessage.Text = "Password Should Alpha Numeric With Special Characters";
            //        return;
            //    }

            //}
            string Returnmsg = string.Empty;
            Int64 returnId = 0;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountEmail", txtLoginName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPassword", clsCryptography.Encrypt(txtPassword.Text), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", hdnUserName.Value.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountNewPassword", clsCryptography.Encrypt(txtNewPassword.Text), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.TinyInt, 0, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            objDAL.ExecuteSP("App_Accounts_Password_Update_IPM", parameters.ToArray(),out Returnmsg,out returnId);
            objGeneralFunction.BootBoxAlert(objGeneralFunction.ReplaceASC(Returnmsg), Page);

        }



    }


}