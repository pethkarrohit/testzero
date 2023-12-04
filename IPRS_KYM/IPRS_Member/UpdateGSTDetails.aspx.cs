using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    public partial class UpdateGSTDetails : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                 
                    DisplayOperation();
                if (Convert.ToString(Session["ApplicationStatus"]) == "0")
                    GetApprovalData();
            }
        }
        protected void GetApprovalData()
        {

            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));

            DSIT_DataLayer DAL = new DSIT_DataLayer();
            DataTable DT = DAL.GetDataTable("App_Accounts_Authorization_CurrentStatus", parameters.ToArray());
            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {

                    if (DT.Rows[0]["App_Status"].ToString() == "R" && DT.Rows[0]["AuthorizationType"].ToString().ToUpper() == "R")
                    {
                        spComments.InnerText = DT.Rows[0]["Comment"].ToString();

                    }

                }



            }

        }
        protected void DisplayOperation()
        {
            #region "Populating GST Details"


            DSIT_DataLayer DAL = new DSIT_DataLayer();

            var parameters = new List<SqlParameter>();

            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));

            DataTable DT = DAL.GetDataTable("App_Accounts_List_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {



                #region "toggling GST"
                txtDetails1_gst.Text = DT.Rows[0]["Detail1"].ToString();
                if (txtDetails1_gst.Text != string.Empty)
                    rbtGstApl.SelectedValue = "AP";
                else
                    rbtGstApl.SelectedValue = "NAP";


                #endregion

                hdnRegType.Value = DT.Rows[0]["AccountRegType"].ToString();
                hdnAccountId.Value = DT.Rows[0]["AccountID"].ToString();

                hdnAccountUpdateField.Value = string.Empty;
                hdnAccountUpdateField.Value = "Detail1='" + txtDetails1_gst.Text.ToUpper() + "'";
                hdnReUpdateInfo.Value =  DT.Rows[0]["ReUpdateInfo"].ToString();
                rbtGstApl_SelectedIndexChanged(null, null);


            }
            #endregion
        }
        protected void rbtGstApl_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DocSubtype = "";
            divGst.Visible = false;
            if (rbtGstApl.SelectedValue.ToUpper() == "AP")
            {
                divGst.Visible = true;
                DocSubtype = DocSubtype + "GST";
            }
            //else
            //{
            //    txtDetails1_gst.Text = string.Empty;
            //    DocSubtype = DocSubtype + "0";
            //}


            UCDocUpload.UcAccountId = hdnAccountId.Value;



            UCDocUpload.DocSubtype = DocSubtype;
            UCDocUpload.DocDesc = "GST";
            UCDocUpload.loadGrdDocumentsPreApproval();
        }
        protected void btnGSTInfo_Click(object sender, EventArgs e)
        {
            HiddenField hdnfilecount = (HiddenField)UCDocUpload.FindControl("hdnfilecount");
            if (hdnfilecount != null)
            {
                if (hdnfilecount.Value == "0")
                {
                    objGeneralFunction.BootBoxAlert("Please upload Documents", Page);
                    return;
                }
            }
            string STRSQlUpdate = string.Empty;
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty;
            STRSQlUpdate = "Detail1='" + txtDetails1_gst.Text.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "ReUpdateInfo=',GST,'"+hdnReUpdateInfo.Value.Replace(",GST,","");


            try
            {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();

                var parameters = new List<SqlParameter>();

                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@strsqlAccounts", STRSQlUpdate, SqlDbType.NVarChar, 8000, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@strsqlArchive", hdnAccountUpdateField.Value, SqlDbType.NVarChar, 8000, ParameterDirection.Input));

                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));


                objDAL.ExecuteSP("App_Accounts_Archive_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);



                if (RecordId > 0)
                {

                    parameters = new List<SqlParameter>();

                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ApplicationStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

                    objDAL = new DSIT_DataLayer();

                    objDAL.ExecuteSP("App_Accounts_AppStatus_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);

                    if (RecordId > 0)
                    {
                        Int64 ReturnId = 0;
                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AuthorizationLevel", Session["AuthorizationLevel"].ToString(), SqlDbType.TinyInt, 0, ParameterDirection.Input));

                        DataTable DT = objDAL.GetDataTable("App_Accounts_List_Email", parameters.ToArray());

                        EmailConfig EmailConfig = new EmailConfig();

                        #region "Email Config Member"
                        EmailConfig = new EmailConfig();
                        EmailConfig.BookType = "AA";
                        EmailConfig.EmailType = "MUBC";
                        EmailConfig.EmailTo = DT.Rows[0]["AccountEmail"].ToString();
                        EmailConfig.DTTransaction = DT;
                        ReturnId = EmailConfig.CreateEmailLog();

                        #endregion

                        #region "Email Config Member"

                        EmailConfig = new EmailConfig();
                        EmailConfig.BookType = "AA";
                        EmailConfig.EmailType = "MRA1";
                        EmailConfig.DTTransaction = DT;
                        EmailConfig.EmailTo = DT.Rows[0]["Auth_EmailAddress"].ToString();
                        ReturnId = EmailConfig.CreateEmailLog();
                        #endregion
                        Session["ApplicationStatus"] = 1;
                        Response.Redirect("Information?");
                    }
                }
                else
                {
                    objGeneralFunction.BootBoxAlert("Updation Failed", Page);
                    return;
                }

            }
            catch (Exception ex)
            {


            }
        }

        protected void lnkRefresh_Click(object sender, EventArgs e)
        {
            DisplayOperation();
        }
    }
}