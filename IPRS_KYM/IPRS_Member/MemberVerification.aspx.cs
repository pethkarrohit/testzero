using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;

namespace IPRS_Member
{
    public partial class MemberVerification : System.Web.UI.Page
    {
        #region global decleration
        GeneralFunction objGeneralFunction = new GeneralFunction();
        #endregion

        #region Nothing Any Code Line Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            // Renu 12-02-2021
        }
        #endregion

        /// <summary>For Timer Verification Tick
        /// Fisrt Check Registration ID using Store Procedure(App_Accounts_Temp_List) and Get Information Related Member from Table(App_Accounts_Temp) 
        /// and store in DataSet Form.
        /// after that insert data in to table(App_Accounts) using Store Procedure(App_Accounts_Manage_MV_IPM). if Account Registration Type is C then insert 
        /// address record in to table(App_Accounts_Address_Contact) by using Store Procedure(App_Accounts_Address_Contact_Manage_IPM). 
        /// after this process done delete specific record from Table(App_Accounts_Temp) by using Store Procedure(App_Accounts_Temp_Delete).
        /// Send Verification success Email to the Member EmailID by using function(CreateEmailLog)
        /// here we set value for RecordStatus= 1, ApplicationStatus= Null And AdmissionStatus = 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Verification
        protected void tmrVerificationMessage_Tick(object sender, EventArgs e)
        {
            #region "Variable declaration section."
            SqlParameter[] oSQLP = null;
            DSIT_DataLayer DAL = null;
            string ReturnMessage = string.Empty;
            Int64 RecordId = 0;
            #endregion

            try
            {
                #region "Validating the Registration ID."
                if (clsCryptography.Decrypt(Request.QueryString["RID"]) == null)
                {
                    divRejected.Visible = true;
                    divPrograss.Visible = false;
                    divActivated.Visible = false;
                    return;
                }
                hdnRecordKeyId.Value = clsCryptography.Decrypt(Request.QueryString["RID"]);
                #endregion

                tmrVerificationMessage.Enabled = false;

                #region "Getting the Temp Account"
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordKeyId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                DAL = new DSIT_DataLayer();
                DataSet DS = DAL.GetDataSet("App_Accounts_Temp_List", parameters.ToArray());
                #endregion

                if (DS.Tables.Count == 0)
                {
                    divRejected.Visible = true;
                    divPrograss.Visible = false;
                    divActivated.Visible = false;
                    Page.Response.Redirect("MemberLogin", false);
                    return;
                }
                if (DS.Tables[0].Rows.Count == 0)
                {
                    divRejected.Visible = true;
                    divPrograss.Visible = false;
                    divActivated.Visible = false;
                    Page.Response.Redirect("MemberLogin", false);
                    return;
                }
                DataRow DR = DS.Tables[0].Rows[0];
                string FirstName="";
                string Lastname="";

                #region "Activating the  Account."
                string Mobile = ""; string AccountName = string.Empty; string Email = string.Empty;
                if (DR["AccountRegType"].ToString().ToUpper() == "I" || DR["AccountRegType"].ToString().ToUpper() == "NI" || DR["AccountRegType"].ToString().ToUpper() == "SC" || DR["AccountRegType"].ToString().ToUpper() == "NSC" || DR["AccountRegType"].ToString().ToUpper() == "LH" || DR["AccountRegType"].ToString().ToUpper() == "LHN")
                {
                    Mobile = DR["Mobile"].ToString();
                    AccountName = DR["AccountName"].ToString();
                }
                if (DR["AccountRegType"].ToString().ToUpper() == "C" || DR["AccountRegType"].ToString().ToUpper() == "NC")
                {
                    AccountName = DR["CompanyName"].ToString();
                    Mobile = DR["Mobile"].ToString();
                    string AccountNamet = DR["AccountName"].ToString();
                    string[] strAccountName = AccountNamet.Split(' ');
                    FirstName = strAccountName[0];
                    Lastname = strAccountName[1];
                }
                Email = DR["AccoutnLogin"].ToString();
                try
                {
                    spError.InnerText = string.Empty;
                    parameters.Clear();
                    SqlDateTime sqldatenull;
                    sqldatenull = SqlDateTime.Null;
                    parameters = new List<SqlParameter>();
                    parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RefAccountCode", DR["AccountCode"].ToString(), SqlDbType.NVarChar, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountType", "C", SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", DR["AccountRegType"].ToString(), SqlDbType.NVarChar, 5, ParameterDirection.Input));                    
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 39, SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded
                    if (DR["Bookid"].ToString()=="")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", DR["Bookid"].ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded
                    }                                
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", AccountName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", FirstName, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", Lastname, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", "", SqlDbType.NVarChar, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RelationShip", DR["RelationShip"].ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@DOB", sqldatenull, SqlDbType.DateTime, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@DateOfDeath", sqldatenull, SqlDbType.DateTime, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@DeathCertNo", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress", DR["AccountAddress"].ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", DR["AccountAlias"].ToString(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
                    if (DR["AccountRegType"].ToString()=="NI" || DR["AccountRegType"].ToString() == "NC")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId",0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", DR["GeographicalId"].ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", DR["GeographicalName"].ToString(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", DR["Pincode"].ToString(), SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    if (DR["AccountRegType"].ToString() == "NI" || DR["AccountRegType"].ToString() == "NC")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", DR["PincodeId"].ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPhone", DR["Telephone"].ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile", Mobile, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountEmail", DR["AccoutnLogin"].ToString().ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Alt_EmailId", DR["Alt_EmailId"].ToString().ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPassword", DR["AccountPassword"].ToString().ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountWeb", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail1", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail2", DR["PanNo"].ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail3", DR["AadharNo"].ToString().Trim().ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail4", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail5", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail6", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail7", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail8", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail9", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail10", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail11", "", SqlDbType.NVarChar, 110, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail12", "", SqlDbType.NVarChar, 120, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RollTypeIds", DR["RollTypeIds"].ToString().Trim().ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationDate", objGeneralFunction.TranslateDateTime(DR["AccountRegDate"].ToString().Trim().ToString()), SqlDbType.DateTime, 25, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", "Administrator", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                    DAL = new DSIT_DataLayer();
                    //   DAL.DataBeginTran();
                    DAL.ExecuteSP("App_Accounts_Manage_MV_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                    if (RecordId == 0)
                    {
                        spError.InnerText = ReturnMessage;
                        divRejected.Visible = true;
                        divPrograss.Visible = false;
                        divActivated.Visible = false;
                        // DAL.DataRollback();
                        return;
                    }
                    #endregion

                    if (DR["AccountRegType"].ToString().ToUpper() == "C")
                    {
                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ContactId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", RecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AddressId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Name", DR["AccountName"].ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Designation", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Department", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ContactPhone", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ContactMobile", DR["Mobile"].ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ContactEmail", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", "Administrator", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                        DAL.ExecuteSP("App_Accounts_Address_Contact_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                    }
                    spError.InnerText = ReturnMessage;
                    if (RecordId > 0 && hdnRecordKeyId.Value != string.Empty)
                    {
                        DAL = new DSIT_DataLayer();
                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@TempAccountId", hdnRecordKeyId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                        DAL.ExecuteSP("App_Accounts_Temp_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);

                        #region "Email Config"
                        EmailConfig EmailConfig = new EmailConfig();
                        EmailConfig.BookType = "AA";
                        EmailConfig.EmailType = "MV";
                        EmailConfig.Name = AccountName;
                        EmailConfig.EmailTo = Email;
                        Int64 ReturnId = EmailConfig.CreateEmailLog();
                        #endregion

                    }
                    //DAL.DataCommitTran();
                    divPrograss.Visible = false;
                    divActivated.Visible = true;
                    divRejected.Visible = false;
                }
                catch (Exception ex)
                {
                    spError.InnerText = ex.Message;
                    // DAL.DataRollback();
                    divRejected.Visible = true;
                    divPrograss.Visible = false;
                    divActivated.Visible = false;
                }

            }
            catch (Exception ex)
            {
                #region "Catch section."
                // genFunc.AlertUser(genFunc.ReplaceASC(ex.Message), this.Page);
                divRejected.Visible = true;
                divPrograss.Visible = false;
                divActivated.Visible = false;
                #endregion
            }
        }
        #endregion
    }
}