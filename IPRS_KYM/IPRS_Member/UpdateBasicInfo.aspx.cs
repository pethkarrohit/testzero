using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace IPRS_Member
{
    public partial class UpdateBasicInfo : System.Web.UI.Page
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
            #region "Populating  Details"
            //txtAccountHoldername.Text = Convert.ToString(Session["AccountName"]);
            try
            {



                DSIT_DataLayer DAL = new DSIT_DataLayer();

                var parameters = new List<SqlParameter>();

                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));

                DataTable DT = DAL.GetDataTable("App_Accounts_List_IPM", parameters.ToArray());
                if (DT.Rows.Count > 0)
                {


                    DataRow DR = DT.Rows[0];

                    hdnRegistrationType.Value = DR["AccountRegType"].ToString();
                    hdnAccountId.Value = DT.Rows[0]["AccountID"].ToString();

                    txtDateofEstablishment.Text = objGeneralFunction.FormatNullableDate(DR["DOB"].ToString());
                    //DateTime.ParseExact(objGeneralFunction.FormatNullableDate(DR["RegistrationDate"].ToString()), "dd/MM/yyyy", null).ToString("yyyy/MM/dd")   objGeneralFunction.FormatNullableDate( DR["RegistrationDate"].ToString());


                    string AccountName = DR["AccountName"].ToString();

                    RegistrationType_Changed();


                    if (hdnRegistrationType.Value == "I")
                    {
                        //txtAccountHoldername.Text = AccountName;
                        string[] strAccountName = AccountName.Split(' ');
                        if (strAccountName.Length > 0)
                        {
                            if (strAccountName.Length == 1)
                                txtFname.Text = strAccountName[0];

                            if (strAccountName.Length == 2)
                            {
                                txtFname.Text = strAccountName[0];
                                txtLname.Text = strAccountName[1];
                            }
                            if (strAccountName.Length == 3)
                            {
                                txtFname.Text = strAccountName[0];
                                txtMname.Text = strAccountName[1];
                                txtLname.Text = strAccountName[2];
                            }
                        }

                    }
                    else
                    {
                        // txtAccountHoldername.Text = DR["AccountName"].ToString();
                        txtcompanyName.Text = DR["AccountName"].ToString();
                    }
                    txtAccountAlias.Text = DR["AccountAlias"].ToString();
                    hdnReUpdateInfo.Value = DR["ReUpdateInfo"].ToString();






                    txtDetails2_Pan.Text = DR["Detail2"].ToString();

                    txtDetails3_Aadh.Text = DR["Detail3"].ToString();









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




                    if (hdnRegistrationType.Value == "C")
                    {
                        if (DR["EntityType"].ToString() != string.Empty)
                            rbtEntityType.SelectedValue = DR["EntityType"].ToString();

                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));

                        DataSet DSAdC = DAL.GetDataSet("App_Accounts_Address_Contact_List_IPM", parameters.ToArray());
                        if (DSAdC.Tables[0].Rows.Count > 0)
                        {

                            DataRow DRADC = DSAdC.Tables[0].Rows[0];
                            txtDesignation.Text = DRADC["Designation"].ToString();
                            hdnContactId.Value = DRADC["ContactId"].ToString();
                            string[] strName = DRADC["Name"].ToString().Split(' ');
                            if (strName.Length > 0)
                            {
                                if (strName.Length == 1)
                                    txtFname.Text = strName[0];

                                if (strName.Length == 2)
                                {
                                    txtFname.Text = strName[0];
                                    txtLname.Text = strName[1];
                                }
                                if (strName.Length == 3)
                                {
                                    txtFname.Text = strName[0];
                                    txtMname.Text = strName[1];
                                    txtLname.Text = strName[2];
                                }
                            }

                            if (DRADC["ContactMobile"].ToString().Contains('-'))
                            {
                                string[] Mobile = DRADC["ContactMobile"].ToString().Split('-');

                                if (Mobile.Length == 2)
                                {
                                    txtCountryCode.Text = Mobile[0];
                                    txtMobile.Text = Mobile[1];
                                }
                            }
                            else
                            {
                                txtMobile.Text = DRADC["ContactMobile"].ToString();
                            }
                        }
                    }

                    string strRegDate = string.Empty;
                    if (txtDateofEstablishment.Text != string.Empty)
                    {
                        strRegDate = objGeneralFunction.FormatDate(txtDateofEstablishment.Text, "yyyy-MM-dd", Page);
                    }

                    if (hdnRegistrationType.Value == "I")
                        hdnAccountUpdateField.Value = "AccountName='" + AccountName.ToUpper() + "',";
                    else
                        hdnAccountUpdateField.Value = "AccountName='" + txtcompanyName.Text.ToUpper() + "',";
                    if (strRegDate == string.Empty)
                        hdnAccountUpdateField.Value += "DOB='" + strRegDate + "',";
                    else
                        hdnAccountUpdateField.Value += "DOB=NULL,";

                    hdnAccountUpdateField.Value += "EntityType='" + rbtEntityType.SelectedValue.ToUpper() + "',";
                    hdnAccountUpdateField.Value += "AccountAlias='" + txtAccountAlias.Text.ToUpper() + "',";
                    hdnAccountUpdateField.Value += "Detail2='" + txtDetails2_Pan.Text.ToUpper() + "',";
                    hdnAccountUpdateField.Value += "Detail3='" + txtDetails3_Aadh.Text.ToUpper() + "',";
                    hdnAccountUpdateField.Value += "AccountMobile='" + txtCountryCode.Text + "-" + txtMobile.Text + "',";
                    hdnAccountUpdateField.Value += "AccountPhone='" + txtTelephone.Text + "'";





                    UCDocUpload.UcAccountId = DT.Rows[0]["AccountID"].ToString();
                    string DocSubtype = "";
                    #region Changing Company Docs
                    if (hdnRegistrationType.Value == "C")
                    {
                        { DocSubtype = DocSubtype + rbtEntityType.SelectedValue + ","; }
                    }

                    #endregion



                    UCDocUpload.DocSubtype = DocSubtype;
                    UCDocUpload.DocDesc = "Basic";
                    UCDocUpload.loadGrdDocumentsPreApproval();
                    DisplayMemberImage();



                }
                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void DisplayMemberImage()
        {

            string FilePath = string.Empty;
            int FileExist = 0;
            btnphotoView.Visible = false;

            try
            {


                FilePath = Server.MapPath("~/MemberPhoto/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MPU_" + hdnAccountId.Value + "_") == true)
                    {
                        FileExist = 1;
                        string filenm = Path.GetFileName(fileName);
                        btnphotoView.NavigateUrl = "~/MemberPhoto/" + filenm;
                        hdnphotoImageName.Value = filenm;
                        break;
                    }
                }
                if (FileExist == 1)
                {

                    btnphotoView.Visible = true;
                }
            }
            catch (Exception)
            {


            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
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



            string strRegDate = string.Empty; string AccountName = string.Empty; string Mobile = string.Empty;


            if (txtCountryCode.Text.Trim() != string.Empty)
                Mobile = txtCountryCode.Text + "-" + txtMobile.Text.ToString();
            else
                Mobile = txtMobile.Text.ToString();

            if (txtMname.Text != string.Empty)
                AccountName = txtFname.Text.Trim() + " " + txtMname.Text.Trim() + " " + txtLname.Text.Trim();
            else
                AccountName = txtFname.Text.Trim() + " " + txtLname.Text.Trim();



            if (txtDateofEstablishment.Text != string.Empty)
            {
                strRegDate = objGeneralFunction.FormatDate(txtDateofEstablishment.Text, "yyyy-MM-dd", Page);
            }


            if (txtAccountAlias.Text.Trim() == string.Empty)
                txtAccountAlias.Text = AccountName;

            if (txtAccountAlias.Text.Contains(","))
            {
                var result = string.Join(",", txtAccountAlias.Text.Split(',').Select(s => s.Trim()).ToArray());
                txtAccountAlias.Text = result;
            }
            if (txtAccountAlias.Text.Contains(";"))
            {
                var result = string.Join(";", txtAccountAlias.Text.Split(',').Select(s => s.Trim()).ToArray());
                txtAccountAlias.Text = result;
            }


            string STRSQlUpdate = string.Empty;
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty;

            if (hdnRegistrationType.Value == "I")
                STRSQlUpdate = "AccountName='" + AccountName.ToUpper().TrimEnd(',') + "',";
            else
                STRSQlUpdate = "AccountName='" + txtcompanyName.Text.ToUpper().TrimEnd(',') + "',";

            STRSQlUpdate += "DOB='" + strRegDate + "',";
            STRSQlUpdate += "EntityType='" + rbtEntityType.SelectedValue.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "AccountAlias='" + txtAccountAlias.Text.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "Detail2='" + txtDetails2_Pan.Text.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "Detail3='" + txtDetails3_Aadh.Text.ToUpper().TrimEnd(',') + "',";
            STRSQlUpdate += "AccountMobile='" + Mobile.TrimEnd(',') + "',";
            STRSQlUpdate += "AccountPhone='" + txtTelephone.Text.TrimEnd(',') + "',";
            
            STRSQlUpdate += "ReUpdateInfo=',Basic,'" + hdnReUpdateInfo.Value.Replace(",Basic,", "");

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





            }
            catch (Exception ex)
            {


            }



            try
            {


                #region "IF Company Saving the Proprietor Info"
                if (hdnRegistrationType.Value == "C" && Convert.ToInt64(RecordId) > 0)
                {

                    Int64 RecordId_AddCt = SaveAccountAddress_Contact(AccountName, Mobile, RecordId);
                }
                #endregion

                if (RecordId > 0)
                {

                    var parameters = new List<SqlParameter>();

                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ApplicationStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

                    DSIT_DataLayer objDAL = new DSIT_DataLayer();

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
            if (RecordId > 0)
            {
                objGeneralFunction.BootBoxAlert("GST Updated Successfully", Page);
                DisplayOperation();
            }
            else
            {
                objGeneralFunction.BootBoxAlert("Updation Failed", Page);
            }
        }
        protected void RegistrationType_Changed()
        {


            divCompanyNote.Visible = false;

            divAadhar.Visible = false;

            //divIndividualOverSeasInfo.Visible = true;
            divCompany.Visible = false;
            divDesignation.Visible = false;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet DS = new DataSet();
            if (hdnRegistrationType.Value == "I")
            {




                spnAlias.InnerText = "Alias";
                divAadhar.Visible = true;


                //divIndividualOverSeasInfo.Visible = true;
                // divGST.Visible = true;
            }
            else
            {
                divDesignation.Visible = true;
                divCompany.Visible = true;
                //SectionTitle.InnerText = "Company Info";
                divCompanyNote.Visible = true;




            }




        }

        protected void btnPhotoUpload_Click(object sender, EventArgs e)
        {
            if (FPuploadPhoto.HasFiles)
            {

                HttpPostedFile PFile = FPuploadPhoto.PostedFile;

                try
                {
                    string strFileName = string.Empty;
                    if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                        || PFile.ContentType == "application/pdf"
                        || PFile.ContentType == "image/gif")
                    {

                        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        string fileExt = Path.GetExtension(PFile.FileName);

                        string SaveAsFileName = "MPU_" + hdnAccountId.Value + "_" + filename + fileExt;
                        if (hdnphotoImageName.Value != string.Empty)
                        {
                            try
                            {
                                File.Delete(Server.MapPath("~/MemberPhoto/" + hdnphotoImageName.Value));
                            }
                            catch (Exception) { }

                        }
                        strFileName = "~/MemberPhoto/" + "_temp_" + SaveAsFileName;
                        FPuploadPhoto.SaveAs(Server.MapPath(strFileName));
                        objGeneralFunction.ResizeImage(Server.MapPath("~/MemberPhoto/"), "_temp_" + SaveAsFileName, Server.MapPath("~/MemberPhoto/"), SaveAsFileName, 150, 150, false, true);
                        btnphotoView.Visible = true;
                        btnphotoView.NavigateUrl = strFileName.Replace("_temp_", "");
                        hdnphotoImageName.Value = strFileName.Replace("_temp_", "");
                        objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);

                    }
                    else
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png, pdf", this.Page);
                    }

                }
                catch (Exception ex)
                {
                    objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
                }

            }
            else
            {
                objGeneralFunction.BootBoxAlert("Please Upload  file", this.Page);
            }


        }
        protected Int64 SaveApp_Accounts(DSIT_DataLayer objDAL, string AccountName, string CityId_pm, string Mobile, string DOB, string RollTypeIds, string CityId_PR)
        {
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty; string strRegDate = string.Empty;

            try
            {
                if (txtDateofEstablishment.Text != string.Empty)
                {
                    strRegDate = objGeneralFunction.FormatDate(txtDateofEstablishment.Text, "yyyy-MM-dd", Page);
                }
                if (strRegDate == string.Empty)
                    // strRegDate = objGeneralFunction.FormatDate(System.DateTime.Now.ToString(), "yyyy-MM-dd HH:mm:ss", Page);
                    // strRegDate = DbNull.Value;

                    //if (rbtnlMemberoverseas.SelectedValue.ToUpper() == "N")
                    //{
                    //    txtOverseasSocietyName.Text = string.Empty;
                    //    txtAssociationMember.Text = string.Empty;
                    //}


                    if (txtAccountAlias.Text.Trim() == string.Empty)
                        txtAccountAlias.Text = AccountName;

                if (txtAccountAlias.Text.Contains(","))
                {
                    var result = string.Join(",", txtAccountAlias.Text.Split(',').Select(s => s.Trim()).ToArray());
                    txtAccountAlias.Text = result;
                }
                if (txtAccountAlias.Text.Contains(";"))
                {
                    var result = string.Join(";", txtAccountAlias.Text.Split(',').Select(s => s.Trim()).ToArray());
                    txtAccountAlias.Text = result;
                }




                //var parameters = new List<SqlParameter>();

                //parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", hdnAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@CompanyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", hdnAccountCode.Value, SqlDbType.NVarChar, 20, ParameterDirection.Input));

                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountType", "C", SqlDbType.NVarChar, 5, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", rbtRegistrationType.SelectedValue, SqlDbType.NVarChar, 5, ParameterDirection.Input));

                //parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 39, SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded

                //if (hdnRegistrationType.Value == "I")
                //    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", AccountName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //else
                //    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", txtcompanyName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));

                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", txtAccountAlias.Text.ToString(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail1", txtDetails1_gst.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));//gst
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail2", txtDetails2_Pan.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));//panno
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail3", txtDetails3_Aadh.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail4", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail5", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail6", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail7", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail8", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail9", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail10", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail11", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Detail12", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress", txtAddress_PM.InnerText.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", CityId_pm, SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", ddlgeoname, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", txtPincode_PM.Text, SqlDbType.NVarChar, 25, ParameterDirection.Input));

                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress_PR", txtAddress_PR.InnerText.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName_PR", ddlgeoname_PR, SqlDbType.NVarChar, 255, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode_PR", txtPincode_PR.Text, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId_PR", CityId_PR, SqlDbType.BigInt, 0, ParameterDirection.Input));



                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPhone", txtTelephone.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile", Mobile, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile_Alt", txtAlternateMobile.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationDate", strRegDate, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@DOB", DOB, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@PlaceOfBirth", txtPOB.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@Nationality", txtNationality.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@RollTypeIds", RollTypeIds, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", hdnAccountStatus.Value, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@EntityType", rbtEntityType.SelectedValue, SqlDbType.NVarChar, 10, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@OverseasSocietyName", txtOverseasSocietyName.Text, SqlDbType.NVarChar, 150, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AssociationName_India", txtAssociationMember.Text, SqlDbType.NVarChar, 150, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));




                //objDAL.ExecuteSP("App_Accounts_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
            }
            catch (Exception ex)
            {


            }
            return RecordId;

        }
        protected Int64 SaveAccountAddress_Contact(string AccountName, string Mobile, Int64 ARchiveRecordId)
        {
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty;
            try
            {

                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@ContactId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AddressId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Ar_AccountId", ARchiveRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Name", AccountName.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Designation", txtDesignation.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Department", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ContactPhone", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ContactMobile", Mobile, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ContactEmail", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));


                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                objDAL.ExecuteSP("App_Accounts_Address_Contact_Arch_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    hdnContactId.Value = RecordId.ToString();

                }

            }

            catch (Exception)
            {


            }

            return RecordId;
        }
    }
}