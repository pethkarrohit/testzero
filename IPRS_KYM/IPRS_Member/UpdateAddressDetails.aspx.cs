using IPRS_Member.User_Controls;
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
    public partial class UpdateAddressDetails : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Convert.ToString(Session["ApplicationStatus"]) == "1" || Convert.ToString(Session["ApplicationStatus"]) == "0")
                //    Response.Redirect("Information");

                //else
                //{

                //if (Convert.ToString(Session["ApplicationStatus"]) == "0")
                //{

                //}
                ddlPincode_PM.PostButtonClick += ddlPincode_PM_Selected;
                ddlPincode_PR.PostButtonClick += ddlPincode_PR_Selected;


                #region "POPULATE Permanent Area"
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlPincode_PM.PopulateDropDown("Area_Populate", parameters, "Area");
                #endregion


                #region "POPULATE Present Area"
                parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlPincode_PR.PopulateDropDown("Area_Populate", parameters, "Area");
                #endregion

                DisplayOperation();
                if (Convert.ToString(Session["ApplicationStatus"]) == "0")
                    GetApprovalData();
            }
      
            // }
        }
        private void ddlPincode_PM_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlPincode_PM.SelectDropDown("0", "");
            ddlPincode_PM.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", "6", SqlDbType.TinyInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", e.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            DT = objDAL.GetDataTable("App_Geographical_Populate_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                ddlGeo_PM.SelectDropDown(DT.Rows[0]["GroupId"].ToString(), DT.Rows[0]["GeographicalGroup"].ToString());

            }
            #endregion




        }
        private void ddlPincode_PR_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlPincode_PR.SelectDropDown("0", "");
            ddlPincode_PR.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", "6", SqlDbType.TinyInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", e.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            DT = objDAL.GetDataTable("App_Geographical_Populate_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                ddlGeo_PR.SelectDropDown(DT.Rows[0]["GroupId"].ToString(), DT.Rows[0]["GeographicalGroup"].ToString());

            }
            #endregion




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
        protected void rbtnlPDCheckAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnlPDCheckAddress.SelectedValue == "N")
            {
                pnlPDPresentAddress.Visible = true;
            }
            else
            {
                pnlPDPresentAddress.Visible = false;
                txtAddress_PR.InnerText = "";
                ddlPincode_PR.SelectDropDown("0", "");
                ((TextBox)ddlGeo_PR.FindControl("txtDropDown")).Text = "";
            }

            if (hdnregType.Value == "I")
                UCDocUpload.DocIds = "4";
            else
                UCDocUpload.DocIds = "10";


            string DocSubtype = "";
            if (txtAddress_PR.InnerText != string.Empty)
            {
                DocSubtype = DocSubtype + "ADD";
            }
            UCDocUpload.DocSubtype = DocSubtype;
            UCDocUpload.DocDesc = "Address";
            UCDocUpload.loadGrdDocumentsPreApproval();
        }

        protected void btnUpdateAddress_Click(object sender, EventArgs e)
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
            Int64 RecordId = SaveApp_Accounts();
        }
        protected void lnkRefresh_Click(object sender, EventArgs e)
        {
            DisplayOperation();
        }
        protected Int64 SaveApp_Accounts()
        {
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty; string strRegDate = string.Empty; string Mobile = string.Empty;

            string CityId_PR = string.Empty; string GEO_PR = string.Empty; string GEO_PM = string.Empty;

            string CityId_pm = ((HiddenField)ddlGeo_PM.FindControl("hdnSelectedValue")).Value;

            GEO_PM = ((TextBox)ddlGeo_PM.FindControl("txtDropDown")).Text;
            GEO_PR = ((TextBox)ddlGeo_PR.FindControl("txtDropDown")).Text;

            string Pincode_PR = string.Empty;

            string Pincode_PM = ((TextBox)ddlPincode_PM.FindControl("txtDropDown")).Text;
            string PincodeId_PM = ((HiddenField)ddlPincode_PM.FindControl("hdnSelectedValue")).Value;

            if (Pincode_PM == "")
            {
                
                objGeneralFunction.BootBoxAlert("Select Permanent Pincode", Page);
                return 0;
            }

            CityId_pm = ((HiddenField)ddlGeo_PM.FindControl("hdnSelectedValue")).Value;
            if (CityId_pm == "0" || CityId_pm == "" || objGeneralFunction.IsNumeric(CityId_pm) == false)
            {
                
                objGeneralFunction.BootBoxAlert("Select Permanent City", Page);
                return 0;
            }
            if (txtCountryCode.Text.Trim() != string.Empty)
                Mobile = txtCountryCode.Text + "-" + txtMobile.Text.ToString();
            else
                Mobile = txtMobile.Text.ToString();

            if (rbtnlPDCheckAddress.SelectedValue == "N")
            {
                 Pincode_PR = ((TextBox)ddlPincode_PR.FindControl("txtDropDown")).Text;
                if (Pincode_PR == "")
                {
                    objGeneralFunction.BootBoxAlert("Select Present Pincode", Page);
                    return 0;
                }

                CityId_PR = ((HiddenField)ddlGeo_PR.FindControl("hdnSelectedValue")).Value;
                if (CityId_PR == "0" || CityId_PR == "" || objGeneralFunction.IsNumeric(CityId_PR) == false)
                {
                    objGeneralFunction.BootBoxAlert("Select Present City", Page);
                    return 0;
                }

            }
            else
            {
                CityId_PR = "0";
                ddlPincode_PR.SelectDropDown("0", "");
                //txtPincode_PR.Text = "";
                txtAddress_PR.InnerText = "";
            }


            string STRSQlUpdate = string.Empty;

            STRSQlUpdate = "AccountAddress='" + txtAddress_PM.InnerText.ToString().ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "GeographicalName='" + GEO_PM.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "GeographicalId='" + CityId_pm + "',";
            STRSQlUpdate += "Pincode='" + Pincode_PM.TrimEnd(',') + "',";

            if (rbtnlPDCheckAddress.SelectedValue == "N")
            {
                STRSQlUpdate += "AccountAddress_PR='" + txtAddress_PR.InnerText.ToString().ToUpper().TrimEnd(',') + "',";
                STRSQlUpdate += "GeographicalName_PR='" + GEO_PR.ToUpper().TrimEnd(',') + "',";
                STRSQlUpdate += "GeographicalId_PR='" + CityId_PR.TrimEnd(',') + "',";
                STRSQlUpdate += "Pincode_PR='" + Pincode_PR.TrimEnd(',') + "',";
            }
            else
            {
                STRSQlUpdate += "AccountAddress_PR='',";
                STRSQlUpdate += "GeographicalName_PR='',";
                STRSQlUpdate += "GeographicalId_PR=NULL,";
                STRSQlUpdate += "Pincode_PR='',";
            }

            STRSQlUpdate += "AccountPhone='" + txtTelephone.Text.TrimEnd(',') + "',";
            STRSQlUpdate += "AccountMobile='" + Mobile.TrimEnd(',') + "',";
            STRSQlUpdate += "AccountMobile_Alt='" + txtAlternateMobile.Text.TrimEnd(',') + "',";

            STRSQlUpdate += "ReUpdateInfo=',Address,'" + hdnReUpdateInfo.Value.Replace(",Address,", "");
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
                        EmailConfig EmailConfig = new EmailConfig();
                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AuthorizationLevel", Session["AuthorizationLevel"].ToString(), SqlDbType.TinyInt, 0, ParameterDirection.Input));

                        DataTable DT = objDAL.GetDataTable("App_Accounts_List_Email", parameters.ToArray());



                        #region "Email Config Member"
                        EmailConfig = new EmailConfig();
                        EmailConfig.BookType = "AA";
                        EmailConfig.EmailType = "MUA";
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
                        Response.Redirect("Information");
                    }
                }
                else
                {
                    objGeneralFunction.BootBoxAlert("Updation Failed", Page);
                    return 0;
                }

            }
            catch (Exception ex)
            {


            }
            return RecordId;

        }

        protected void DisplayOperation()
        {



            try
            {
                //HSTDOC = new Hashtable();
                //HSTDOC.Add(strGSTLookupId, "1");
                //HSTDOC.Add(strOSILookupId, "1");
                //HSTDOC.Add(strOSCLookupId, "1");

                #region Changing Permanent Compulsory

                #endregion

                DSIT_DataLayer DAL = new DSIT_DataLayer();

                var parameters = new List<SqlParameter>();

                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));

                DataSet DS = DAL.GetDataSet("App_Accounts_List_IPM", parameters.ToArray());
                if (DS.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = DS.Tables[0].Rows[0];




                    string AccountName = DR["AccountName"].ToString();
                    hdnregType.Value = DR["AccountRegType"].ToString();

                    UCDocUpload.UcAccountId = DR["AccountID"].ToString();

                    #region "Populating permanent Address"
                    txtAddress_PM.InnerText = DR["AccountAddress"].ToString();
                    ddlGeo_PM.SelectDropDown(DR["GeographicalId"].ToString(), DR["GeographicalName"].ToString());
                    ddlPincode_PM.SelectDropDown("0", DR["Pincode"].ToString());
                    #endregion

                    rbtnlPDCheckAddress.SelectedValue = "Y";
                    hdnReUpdateInfo.Value = DR["ReUpdateInfo"].ToString();
                    #region "Populating Present Address"

                    if (DR["AccountAddress_PR"].ToString() != string.Empty)
                    {
                        rbtnlPDCheckAddress.SelectedValue = "N";
                        txtAddress_PR.InnerText = DR["AccountAddress_PR"].ToString();
                        ddlPincode_PR.SelectDropDown("0", DR["Pincode_PR"].ToString());
                        ddlGeo_PR.SelectDropDown(DR["GeographicalId_PR"].ToString(), DR["GeographicalName_PR"].ToString());
                    }

                    #endregion

                    rbtnlPDCheckAddress_SelectedIndexChanged(null, null);




                    txtTelephone.Text = DR["AccountPhone"].ToString();
                    if (DR["AccountMobile"].ToString().Contains('-'))
                    {
                        string[] Mobile = DR["AccountMobile"].ToString().Split('-');

                        if (Mobile.Length == 2)
                        {
                            txtCountryCode.Text = Mobile[0];
                            txtMobile.Text = Mobile[1];
                        }
                    }
                    else
                        txtMobile.Text = DR["AccountMobile"].ToString();

                    txtAlternateMobile.Text = DR["AccountMobile_Alt"].ToString();



                    hdnAccountUpdateField.Value = string.Empty;

                    hdnAccountUpdateField.Value = "AccountAddress='" + DR["AccountAddress"].ToString() + "',";
                    hdnAccountUpdateField.Value += "GeographicalId='" + DR["GeographicalId"].ToString() + "',";
                    hdnAccountUpdateField.Value += "GeographicalName='" + DR["GeographicalName"].ToString() + "',";
                    hdnAccountUpdateField.Value += "Pincode='" + DR["Pincode"].ToString() + "',";
                    hdnAccountUpdateField.Value += "AccountPhone='" + DR["AccountPhone"].ToString() + "',";
                    hdnAccountUpdateField.Value += "AccountMobile='" + DR["AccountMobile"].ToString() + "',";
                    hdnAccountUpdateField.Value += "AccountMobile_Alt='" + DR["AccountMobile_Alt"].ToString() + "',";


                    if (rbtnlPDCheckAddress.SelectedValue == "N")
                    {
                        hdnAccountUpdateField.Value += "AccountAddress_PR='" + DR["AccountAddress_PR"].ToString() + "',";
                        hdnAccountUpdateField.Value += "GeographicalName_PR='" + DR["GeographicalName_PR"].ToString() + "',";
                        hdnAccountUpdateField.Value += "GeographicalId_PR='" + DR["GeographicalId_PR"].ToString() + "',";
                        hdnAccountUpdateField.Value += "Pincode_PR='" + DR["Pincode_PR"].ToString() + "'";
                    }
                    else
                    {
                        hdnAccountUpdateField.Value += "AccountAddress_PR='',";
                        hdnAccountUpdateField.Value += "GeographicalName_PR='',";
                        hdnAccountUpdateField.Value += "GeographicalId_PR=NULL,";
                        hdnAccountUpdateField.Value += "Pincode_PR=''";
                    }


                }
            }
            catch (Exception ex)
            {

                objGeneralFunction.BootBoxAlert(ex.Message, Page);
            }
        }
        protected void ToggleRegistrationType(object sender, EventArgs e)
        {


            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet DS = new DataSet();
            if (hdnregType.Value == "I")
            {


                divAltMobile.Visible = true;
                SpAddressYesNo.InnerText = "Is Present address same as the Permanent address ?";
                SpPrmnAddressType.InnerText = "Permanent Address";
                //divIndividualOverSeasInfo.Visible = true;
                // divGST.Visible = true;
            }
            else
            {

                SpAddressYesNo.InnerText = "Is Present address same as the Registerd address ?";


                SpPrmnAddressType.InnerText = "Registered Address";

            }




        }
    }


}