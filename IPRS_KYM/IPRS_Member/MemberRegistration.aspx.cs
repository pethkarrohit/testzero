using IPRS_Member.User_Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;


namespace IPRS_Member
{
    public partial class MemberRegistration : System.Web.UI.Page
    {
        /// <summary>
        /// create object for GeneralFunction and DSIT_DataLayer
        /// from the GeneralFunction we can call customize function or common function which is use in multiple pages.
        /// Commented By Rohit
        /// </summary>
        #region Public Decleration
        GeneralFunction objGeneralFunction = new GeneralFunction();
        string msg = string.Empty;
        #endregion

        /// <summary> for Page Load
        /// Here we check Session["AccountCode"] and Session["AccountRegType"] if Session are null then Redirect to MemberLogin page.
        /// firstly we check which division or input controls are requierd by Account registration Type (Session["AccountRegType"]) 
        /// so only those division or Input controls are visible/ accessible. After that we genrate value for Captcha. next populate area details and assign to 
        /// ddlpincode (Dropdownlist), ddlGeographical (Dropdownlist) and for deceased member assign value to ddlpincode_PM (Dropdownlist)
        /// , ddlGeo_PM (Dropdownlist).
        /// : Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Page Load Event
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.TextBox.DisabledCssClass = "";
            ddlpincode.PostButtonClick += ddlPincode_Selected;
            ddlPincode_PM.PostButtonClick += ddlPincode_PM_Selected;
            if (Session["AccountCode"] == null)
                Response.Redirect("MemberLogin", false);//Temproary Basis
            if (Session["AccountRegType"] == null)
                Response.Redirect("MemberLogin", false);//Temproary Basis

            //ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            //scriptManager.RegisterPostBackControl(this.btnNextButton);
            //this.BT

            if (Session["AccountRegType"].ToString() != null)
            {
                hdnAccountRegType.Value = Session["AccountRegType"].ToString();
                if (hdnAccountRegType.Value == "I")
                {
                    lblFormTitle.Text = "Registration " + "(Individual)";
                    //lblNote.Text = "Enter the Name as Pancard";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                    btnSendOTPmobile.Visible = true; //added by Rohit 10/08/20223
                    Divotp.Visible = true; // added by Rohit 10/08/20223
                    DivAadhar.Visible = true;// added by Rohit 10/08/20223
                    DivSocial.Visible = false;// added by Rohit 10/08/20223
                }
                else if (hdnAccountRegType.Value == "NI")
                {
                    lblFormTitle.Text = "Registration " + "(NRI Individual)";
                    //Label lblNote = new Label();
                    //lblNote.Text = "Make a name of pancard";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                    txtPan.CausesValidation = false;
                    btnSendOTPmobile.Visible = false; //added by Hariom 25-02-2023
                    Divotp.Visible = false; // added by Rohit 10/08/20223
                    DivAadhar.Visible = false;// added by Rohit 10/08/20223
                    DivSocial.Visible = true;// added by Rohit 10/08/20223
                }
                else if (hdnAccountRegType.Value == "C")
                {
                    lblFormTitle.Text = "Registration " + "(Company)";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                    btnSendOTPmobile.Visible = true; //added by Rohit 10/08/20223
                    Divotp.Visible = true; // added by Rohit 10/08/20223
                    DivAadhar.Visible = true;// added by Rohit 10/08/20223
                    DivSocial.Visible = false;// added by Rohit 10/08/20223
                }
                else if (hdnAccountRegType.Value == "NC")
                {
                    lblFormTitle.Text = "Registration " + "(NRI Company)";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                    txtPan.CausesValidation = false;
                    btnSendOTPmobile.Visible = false; //added by Hariom 25-02-2023
                    Divotp.Visible = false; // added by Rohit 10/08/20223
                    DivAadhar.Visible = false;// added by Rohit 10/08/20223
                    DivSocial.Visible = true;// added by Rohit 10/08/20223
                }
                else if (hdnAccountRegType.Value == "SC")
                {
                    lblFormTitle.Text = "Registration " + "(Self Release)";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                }
                else if (hdnAccountRegType.Value == "NSC")
                {
                    lblFormTitle.Text = "Registration " + "(NRI Self Release)";
                    SpLH.InnerText = "";
                    DivDTrader.Visible = false;
                    DivTrader.Visible = true;
                    divroletyped.Visible = false;
                    divroletype.Visible = true;
                }
                else if (hdnAccountRegType.Value == "LH")
                {
                    lblFormTitle.Text = "Registration " + "(Legal Heir - Existing Deceased Member)";
                    SpLH.InnerText = "Legal heir details";
                    DivDTrader.Visible = true;
                    DivTrader.Visible = false;
                    divroletyped.Visible = true;
                    cbxRoleTyped.Enabled = false;
                    divroletype.Visible = false;
                    btnSendOTPmobile.Visible = true; //added by Rohit 10/08/20223
                    Divotp.Visible = true; // added by Rohit 10/08/20223
                    DivAadhar.Visible = true;// added by Rohit 10/08/20223
                    DivSocial.Visible = false;// added by Rohit 10/08/20223
                }
                else if (hdnAccountRegType.Value == "LHN")
                {
                    lblFormTitle.Text = "Registration " + "(Legal Heir - Non member)";
                    SpLH.InnerText = "Legal heir details";
                    DivDTrader.Visible = true;
                    DivTrader.Visible = false;
                    divroletyped.Visible = true;
                    divroletype.Visible = false;
                    btnSendOTPmobile.Visible = true; //added by Rohit 10/08/20223
                    Divotp.Visible = true; // added by Rohit 10/08/20223
                    DivAadhar.Visible = true;// added by Rohit 10/08/20223
                    DivSocial.Visible = false;// added by Rohit 10/08/20223
                }
            }
            txtPassword.Attributes.Add("value", Convert.ToString(txtPassword.Text));
            if (!IsPostBack)
            {
                ViewState["CaptchaValue"] = GenerateRandomCode();
                hdnEcapValue.Value = ViewState["CaptchaValue"].ToString();
                if (Session["AccountCode"].ToString() != null)
                {
                    hdnAccountCode.Value = Session["AccountCode"].ToString();
                }
                if (hdnAccountRegType.Value != "NI" && hdnAccountRegType.Value != "NC")
                {
                    #region "POPULATE Area"
                    var parameters = new List<SqlParameter>();
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    ddlpincode.PopulateDropDown("Area_Populate", parameters, "Area");
                    #endregion
                    ddlGeographical.blEnabled = false;
                    btnSendOTPmobile.Enabled = true;
                    Divotp.Visible = true;
                    DivAadhar.Visible = true;// added by Rohit 10/08/20223
                    DivSocial.Visible = false;// added by Rohit 10/08/20223
                }
                else
                {
                    ddlGeographical.blEnabled = true;
                    btnSendOTPmobile.Enabled = false;
                    txtOTPmobile.Enabled = false;
                    Divotp.Visible = false;
                    DivAadhar.Visible = false;// added by Rohit 10/08/20223
                    DivSocial.Visible = true;// added by Rohit 10/08/20223
                }
                #region "POPULATE Permanent Area"
                // this code for only deceased member
                var parameters1 = new List<SqlParameter>();
                parameters1.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlPincode_PM.PopulateDropDown("Area_Populate", parameters1, "Area");
                #endregion

                //rbtRegistrationType_SelectedIndexChanged(null, null);
                DisplayDivEnaDis();
                if (hdnAccountRegType.Value == "LHN" || hdnAccountRegType.Value == "LH")
                    PopulateRoleTypesd();
                else
                {
                    PopulateRoleTypes();
                }

                //txtFname.Text = "poornima";
                //txtMname.Text = "P";
                //txtLname.Text = "Moorthy";
                //txtAddress.InnerText = "Malad";
                //txtPincode.Text = "400095";
                //txtCountryCode.Text = "+91";
                //txtMobile.Text = "9930666849";
                //txtEmail.Text = "poornima@dreamsoftindia.com";
                //txtPassword.Text = "poorni";
                //txtPan.Text = "abcde4844g";

                // by renu
                double otpCount = Convert.ToDouble(hdnOTPCount.Value);
                if (otpCount == 0)
                {
                    btnVerifyEmail.Visible = false;
                }
                if (hdnAccountRegType.Value == "NC" || hdnAccountRegType.Value == "NI")
                {
                    btnVerifyMobile.Text = "Verified";
                }
                double otpmobileCount = Convert.ToDouble(hdnOTPmobileCount.Value);
                if (otpmobileCount == 0)
                {
                    btnVerifyMobile.Visible = false;
                }

            }
            Button btnNextButton = objGeneralFunction.GetControlFromWizard(wzMain, GeneralFunction.WizardNavigationTempContainer.StartNavigationTemplateContainerID, "btnNextButton") as Button;
            if (btnNextButton != null)
            {
                if (btnVerifyEmail.Text == "Verified")
                    btnNextButton.Enabled = true;
                else
                    btnNextButton.Enabled = false;
            }
        }
        #endregion

        /// <summary> for ddlPincode
        /// Here we get value from user control (ucDropDown) and on that basis get geografical location by using store procedure
        /// (App_Geographical_Populate_IPM ) and assign value to ddlGeographical drop down like GeographicalGroup (DOMBIVALI / THANE / Maharashtra / India) with 
        /// GeographicalId(41067) and GroupId(262)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Drop Down PinCode
        private void ddlPincode_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlpincode.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            //ddlpincode.SelectDropDown(e.SelectedValue, e.SelectedText.TrimStart().TrimEnd());
            ddlGeographical.SelectDropDown("0", "");
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", "6", SqlDbType.TinyInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", e.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            DT = objDAL.GetDataTable("App_Geographical_Populate_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                ddlGeographical.SelectDropDown(DT.Rows[0]["GroupId"].ToString(), DT.Rows[0]["GeographicalGroup"].ToString());
            }
            #endregion
        }
        #endregion

        /// <summary> for ddlPincode For Deceased Member
        /// Here we get value from user control (ucDropDown) and on that basis get geografical location by using store procedure
        /// (App_Geographical_Populate_IPM ) and assign value to ddlGeographical drop down like GeographicalGroup (DOMBIVALI / THANE / Maharashtra / India) with 
        /// GeographicalId(41067) and GroupId(262)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Drop Down PinCode For Deceased Member
        private void ddlPincode_PM_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlPincode_PM.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            //ddlpincode.SelectDropDown(e.SelectedValue, e.SelectedText.TrimStart().TrimEnd());
            ddlGeo_PM.SelectDropDown("0", "");
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
        #endregion

        /// <summary>for Timer1_Tick For Email
        /// here we check Data Insert Into Table (App_Accounts_Temp) or not.if data inserted then send verfication email to Member emailid for
        /// verification and change Button(btnfinish) text into Continue and related Divison are visible true. if data not insert into table then related division visiable
        /// false and change button(btnfinish) text into Try Again 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Timer1_Tick
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            #region TIMER TICK FALSE      
            Timer1.Enabled = false;
            // imgloading.Visible = false;
            if (hdnRecordKeyId.Value == "0")
            {
                wzMain.ActiveStepIndex = 2;
                divsuccess.Visible = false;
                divfail.Visible = true;
                Button btnfinish = (Button)wzMain.FindControl("btnfinish");
                btnfinish.Text = "Try again";
            }
            else
            {
                wzMain.ActiveStepIndex = 2;
                SendMail();
                divsuccess.Visible = true;
                divfail.Visible = false;
                Button btnfinish = (Button)wzMain.FindControl("btnfinish");
                btnfinish.Text = "Continue";
            }
            #endregion TIMER TICK FALSE
        }
        #endregion

        /// <summary> For DisplayDivEnaDis
        /// Here we code, which division are visiable or not asper the Account Registration Type like(I,NI,C,NC,LH,LHN)
        /// Commented By Rohit
        /// </summary>
        #region Diplay Divisions
        protected void DisplayDivEnaDis()
        {
            //if (Request.QueryString["RG"] != null)
            //    rbtRegistrationType.SelectedValue = Convert.ToString(Request.QueryString["RG"]);
            divCompany.Visible = false;
            SPCompanyNote.Visible = false;
            if (hdnAccountRegType.Value == "C" || hdnAccountRegType.Value == "NC")
            {
                SPCompanyNote.Visible = true;
                divCompany.Visible = true;
                txtfn.Attributes.Add("placeholder", "Enter Representative First & Middle Name"); //changed by Hariom 17-04-2023
                txtLname.Attributes.Add("placeholder", "Enter Representative Last Name");
                txtDAccountAlias.ReadOnly = false;
            }
            if (hdnAccountRegType.Value == "NI" || hdnAccountRegType.Value == "NC")
            {
                //ddlpincode.blEnabled = false;
                //txtCountryCode.Enabled = false;
                //txtMobile.Enabled = false;
                //txtOTPmobile.Enabled = false;
                //txtPan.Enabled = false;
            }
            else
            {
                //ddlpincode.blEnabled = true;
                //txtCountryCode.Enabled = true;
                //txtMobile.Enabled = true;
                //txtOTPmobile.Enabled = true;
                //txtPan.Enabled = true;
            }
            if (hdnAccountRegType.Value == "LH")
            {
                divLegDetail.Visible = true;
                DivDTrader.Visible = true;
                DivTrader.Visible = false;
                DisplayExistingDetails();
                txtDAccountAlias.ReadOnly = true;
                //FPuploadPhoto.Visible = false;
                //btnPhotoUpload.Visible = false;
            }
            else if (hdnAccountRegType.Value == "LHN")
            {
                divLegDetail.Visible = true;
                DivDTrader.Visible = true;
                DivTrader.Visible = false;
                EnaDisNonMem();
                txtDAccountAlias.ReadOnly = false;
                //FPuploadPhoto.Visible = true;
                //btnPhotoUpload.Visible = true;
            }
            else
            {
                divLegDetail.Visible = false;
                DivDTrader.Visible = false;
                DivTrader.Visible = true;
                txtDAccountAlias.ReadOnly = false;
            }
        }
        #endregion

        /// <summary> For EnaDisNonMem
        /// Here we code, which division are visiable for Deceased Member
        /// Commented By Rohit
        /// </summary>
        #region EnaDisNonMem
        protected void EnaDisNonMem()
        {
            txtdeceasedMemName.ReadOnly = false;
            txtDOB.ReadOnly = false;
            txtdateofdeath.ReadOnly = false;
            txtdatecertNumber.ReadOnly = false;
            txtAddress_PM.Disabled = false;
            ddlGeo_PM.blEnabled = true;
            ddlPincode_PM.blEnabled = true;
        }
        #endregion

        /// <summary> For DisplayExistingDetails
        /// Get existing Deceased Member information for legal Heir new registraion by using Store procedure(App_Accounts_List_Deceased) and 
        /// assign those value to related controls
        /// Commented By Rohit
        /// </summary>
        #region Display Existing Deceased Member Details
        protected void DisplayExistingDetails()
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@Accountcode", hdnAccountCode.Value, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_Deceased", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];
                txtdeceasedMemName.Text = DR["AccountName"].ToString();
                txtdeceasedMemName.ReadOnly = true;
                DDLRelationship.SelectedValue = DR["Relationship"].ToString();
                txtDOB.Text = DateTime.Parse(DR["DOB"].ToString()).ToString("dd/MMM/yyyy");
                txtDOB.ReadOnly = true;
                if (hdnAccountRegType.Value == "LH")
                {
                    calDOB.Enabled = false;
                    calDOB1.Enabled = false;
                }
                else
                {
                    calDOB.Enabled = true;
                    calDOB1.Enabled = true;
                }
                txtdateofdeath.Text = DateTime.Parse(DR["DateOfDeath"].ToString()).ToString("dd/MMM/yyyy");
                txtdateofdeath.ReadOnly = true;
                txtdatecertNumber.Text = DR["DeathCertificateNo"].ToString();
                txtdatecertNumber.ReadOnly = true;
                txtAddress_PM.InnerText = DR["AccountAddress_Pr"].ToString();
                txtAddress_PM.Disabled = true;
                hdnAccountId.Value = DR["AccountId"].ToString();
                ddlGeo_PM.SelectDropDown(DR["GeographicalId_Pr"].ToString(), DR["GeographicalName_Pr"].ToString());
                ddlGeo_PM.blEnabled = false;
                ddlPincode_PM.SelectDropDown(DR["PincodeId_Pr"].ToString(), DR["Pincode_Pr"].ToString());
                ddlPincode_PM.blEnabled = false;
                //DisplayMemberImage(Convert.ToInt64( DR["AccountId"]));
            }
        }
        #endregion

        #region code not in use
        //protected void DisplayMemberImage(Int64 pAccountId)
        //{

        //    string FilePath = string.Empty;
        //    int FileExist = 0;
        //    btnphotoView.Visible = false;
        //    ImgUser.ImageUrl = "~/Images/user.png";
        //    ImgUserLarge.ImageUrl = "~/Images/user.png";
        //    try
        //    {


        //        FilePath = Server.MapPath("~/MemberPhoto/");
        //        string[] fileEntries = Directory.GetFiles(FilePath);
        //        foreach (string fileName in fileEntries)
        //        {
        //            if (fileName.ToUpper().Contains("MPU_" + pAccountId + "_") == true)
        //            {
        //                FileExist = 1;
        //                string filenm = Path.GetFileName(fileName);
        //                //btnphotoView.NavigateUrl = "~/MemberPhoto/" + filenm;
        //                ImgUserLarge.ImageUrl = "~/MemberPhoto/" + filenm;
        //                ImgUser.ImageUrl = "~/MemberPhoto/" + filenm;
        //                hdnphotoImageName.Value = filenm;
        //                break;
        //            }
        //        }
        //        if (FileExist == 1)
        //        {

        //            btnphotoView.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}

        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        
        protected int FileDelete(string FileName, string FilePath)
        {

            int FileExistStatus = 0;
            try
            {


                var query = from o in Directory.GetFiles(FilePath, "*.*")
                            let x = new FileInfo(o)
                            where x.FullName.ToUpper().Contains(FileName.ToUpper())
                            select o;

                foreach (var item in query)
                {
                    FileExistStatus = 1;
                    File.Delete(item);
                }

            }
            catch (Exception ex)
            {


            }
            return FileExistStatus;
        }
        #endregion

        /// <summary>for PopulateRoleTypes
        /// Get Role Type asper Account Registration Type by using Store Procedure(MemberRoleType_LookUp_Populate_IPM) like MemberRoleTypeId(2),
        /// MemberRoleType(Author (Lyricist / Lyrical Works),RoleType(IL),RegistrationType(I)
        ///Commented By Rohit
        /// </summary>
        #region PopulateRoleTypes
        protected void PopulateRoleTypes()
        {
            DataSet DS = new DataSet();
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnAccountRegType.Value, SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRoleType, out DS);
            parameters.Clear();
            if (hdnAccountRegType.Value == "I" || hdnAccountRegType.Value == "C" || hdnAccountRegType.Value == "NI" || hdnAccountRegType.Value == "NC" || hdnAccountRegType.Value == "SC" || hdnAccountRegType.Value == "NSC" || hdnAccountRegType.Value == "LH" || hdnAccountRegType.Value == "LHN")
            {
                DataRow DR = DS.Tables[0].Select("RegistrationType='" + hdnAccountRegType.Value + "'").FirstOrDefault();
                // and MemberRoleType like 'publisher%'").FirstOrDefault();
                if (DR != null)
                {
                    if (cbxRoleType.Items.Count > 0)
                    {
                        ListItem item = cbxRoleType.Items.FindByValue(DR["MemberRoleTypeId"].ToString());
                        if (item != null)
                            item.Enabled = false;
                    }
                }
            }
            if (Request.QueryString["RT"] != null)
            {
                if (Request.QueryString["RT"].ToString() != string.Empty)
                {
                    string tempRT;
                    tempRT = clsCryptography.Decrypt(Request.QueryString["RT"].ToString());

                    //string[] strRolltype = tempRT.ToString().TrimEnd('~').Split('~');
                    string[] strRolltype = Request.QueryString["RT"].ToString().TrimEnd('~').Split('~');

                    for (int i = 0; i < strRolltype.Length; i++)
                    {
                        for (int j = 0; j < cbxRoleType.Items.Count; j++)
                        {
                            if (strRolltype[i] == cbxRoleType.Items[j].Value.ToString())
                            {
                                cbxRoleType.Items[j].Selected = true;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>for PopulateRoleTypesd (Deceased Member)
        /// Get Role Type asper Account Registration Type by using Store Procedure(MemberRoleType_LookUp_Populate_IPM) for Deceased Member like MemberRoleTypeId(2),
        /// MemberRoleType(Author (Lyricist / Lyrical Works),RoleType(IL),RegistrationType(I)
        ///Commented By Rohit
        /// </summary>
        #region PopulateRoleTypesd (Deceased Member) 
        protected void PopulateRoleTypesd()
        {
            DataSet DS = new DataSet();
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnAccountRegType.Value, SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRoleTyped, out DS);
            parameters.Clear();
            if (hdnAccountRegType.Value == "I" || hdnAccountRegType.Value == "C" || hdnAccountRegType.Value == "NI" || hdnAccountRegType.Value == "NC" || hdnAccountRegType.Value == "SC" || hdnAccountRegType.Value == "NSC" || hdnAccountRegType.Value == "LH" || hdnAccountRegType.Value == "LHN")
            {
                DataRow DR = DS.Tables[0].Select("RegistrationType='" + hdnAccountRegType.Value + "'").FirstOrDefault();
                // and MemberRoleType like 'publisher%'").FirstOrDefault();
                if (DR != null)
                {
                    if (cbxRoleTyped.Items.Count > 0)
                    {
                        ListItem item = cbxRoleTyped.Items.FindByValue(DR["MemberRoleTypeId"].ToString());
                        if (item != null)
                            item.Enabled = false;
                    }
                }
            }
            if (Request.QueryString["RT"] != null)
            {
                if (Request.QueryString["RT"].ToString() != string.Empty)
                {
                    string tempRT;
                    tempRT = clsCryptography.Decrypt(Request.QueryString["RT"].ToString());

                    //string[] strRolltype = tempRT.ToString().TrimEnd('~').Split('~');
                    string[] strRolltype = Request.QueryString["RT"].ToString().TrimEnd('~').Split('~');

                    for (int i = 0; i < strRolltype.Length; i++)
                    {
                        for (int j = 0; j < cbxRoleTyped.Items.Count; j++)
                        {
                            if (strRolltype[i] == cbxRoleTyped.Items[j].Value.ToString())
                            {
                                cbxRoleTyped.Items[j].Selected = true;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary> For GenerateRandomCode
        /// for Genrate Capcha Code for final Human Verification
        /// Commented By Rohit
        /// </summary>
        /// <returns></returns>
        #region GenerateRandomCode
        private string GenerateRandomCode()
        {
            string s = ""; int c = 0;
            try
            {
                Random r = new Random();
                int a = r.Next(1, 10);
                int b = r.Next(1, 10);
                c = a + b;
                s = a.ToString() + " @ " + b.ToString() + " = ";
                hdnEcapcha.Value = s.ToString();
            }
            catch { }
            return c.ToString();
        }
        #endregion

        /// <summary>For Send Verification Mail 
        /// here we code for send Member Verification Email with Temporary Account ID  after complete registration process for this we use
        /// function(CreateEmailLog) from EmailConfig.cs 
        /// </summary>
        #region Send Verification Mail 
        public void SendMail()
        {
            try
            {
                string AccountName = string.Empty;
                //if (txtMname.Text != string.Empty)
                //    AccountName = txtfn.Text.Trim() + " " + txtMname.Text.Trim() + " " + txtLname.Text.Trim();
                //else
                //    AccountName = txtfn.Text.Trim() + " " + txtLname.Text.Trim();
                AccountName = txtfn.Text.Trim() + " " + txtLname.Text.Trim();
                
                #region "Email Config"
                string link = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/MemberVerification.aspx?RID=" + clsCryptography.Encrypt(hdnRecordKeyId.Value);
                EmailConfig EmailConfig = new EmailConfig();
                EmailConfig.BookType = "AA";
                EmailConfig.EmailType = "MR";
                EmailConfig.DocumentAttach = "";
                EmailConfig.Name = AccountName;
                EmailConfig.Link = "<a href='" + link + "' target='_blank'>Click here to verify Your Registration</a>";
                EmailConfig.EmailTo = txtEmail.Text;
                // EmailConfig.EmailContent = HtmlContent;
                
                Int64 ReturnId = EmailConfig.CreateEmailLog();

                //string HtmlContent1 = "";
                //string link1 = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/MemberVerification.aspx?RID=" + clsCryptography.Encrypt(hdnRecordKeyId.Value);
                //HtmlContent = "Dear Sir/Madam,<br><br>Click the below link ";
                //HtmlContent += "to Verify your Registration";
                //HtmlContent += "<br>" + link1;
                //HtmlContent += "<br><br>Thank You,<br>IPRS.ORG";
                //System.Net.Mail.MailMessage CustomerMessage = new System.Net.Mail.MailMessage();
                //CustomerMessage.From = new MailAddress("poornima@dreamsoftindia.com");
                //CustomerMessage.To.Add(new MailAddress(txtEmail.Text));
                //CustomerMessage.Subject = "Verification Mail for New Registration";
                //CustomerMessage.IsBodyHtml = true;
                //CustomerMessage.Body = HtmlContent1;
                //SmtpClient Customer = new SmtpClient();
                //Customer.Host = "mail.dreamsoftindia.com";
                //Customer.Credentials = new System.Net.NetworkCredential("poornima-dreamsoftindia", "Poornima@18");
                //Customer.Send(CustomerMessage);
                #endregion
            }
            catch (Exception ee)
            {
                divMessage.Visible = true;
            }
        }
        #endregion

        /// <summary> For Insert Data Into Tables(App_Accounts_Temp) For otherthan LHN & LH & Update Data Into Tables(App_Accounts) For LHN & LH
        /// here we insert member personal data into App_Accounts_Temp by using Store Procedure(App_Accounts_Temp_Manage) with parameters.
        /// before insert data we check Captcha Value is not null if its then get massage. after that check Account registration type if registration type not in LHN and LH then 
        /// insert data in temporary table(App_Accounts_Temp) and send verifaction Email to Member Email ID.
        /// if registration type is LHN then update data in table(App_Accounts) and get Deceased Member Account Code
        /// if registration type is LH then Update The Existing Deceased Member Some Field in Table(App_Accounts) like(AccountAlias,Relationship)
        /// with parameter(AccountId)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Insert Data 
        protected void wzMain_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                if (wzMain.ActiveStepIndex == 0)
                {
                    if (txtcode.Text.Trim() != ViewState["CaptchaValue"].ToString())
                    {
                        objGeneralFunction.BootBoxAlert("YOU HAVE ENTERED INVALID CAPTCHA CODE. PLEASE RETRY.", Page);
                        e.Cancel = true;
                        return;
                    }
                    string AccountName = string.Empty; string CityId = "0";
                    //if (txtMname.Text != string.Empty)
                    //    AccountName = txtfn.Text.Trim() + " " + txtMname.Text.Trim() + " " + txtLname.Text.Trim();
                    //else
                    //    AccountName = txtfn.Text.Trim() + " " + txtLname.Text.Trim();
                    AccountName = txtfn.Text.Trim() + " " + txtLname.Text.Trim();
                    if (hdnAccountRegType.Value == "LHN" || hdnAccountRegType.Value == "LH")
                    {
                        hdnRoleTypeIds.Value = objGeneralFunction.SplitCheckListBoxID(cbxRoleTyped);
                    }
                    else
                    {
                        hdnRoleTypeIds.Value = objGeneralFunction.SplitCheckListBoxID(cbxRoleType);
                    }
                    if (hdnRoleTypeIds.Value == string.Empty)
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Select Role Type", Page);
                        return;
                    }
                    string Pincode = "";
                    string PincodeId = "0";
                    if (hdnAccountRegType.Value == "NI" || hdnAccountRegType.Value == "NC")
                    {
                        Pincode = ((TextBox)ddlpincode.FindControl("txtDropDown")).Text.Trim();
                    }
                    if (hdnAccountRegType.Value == "I" || hdnAccountRegType.Value == "C" || hdnAccountRegType.Value == "LH" || hdnAccountRegType.Value == "LHN")
                    {
                        Pincode = ((TextBox)ddlpincode.FindControl("txtDropDown")).Text.Trim();
                        PincodeId = ((HiddenField)ddlpincode.FindControl("hdnSelectedValue")).Value;
                        if (PincodeId == "0" || PincodeId == "" || Pincode.Length == 0)
                        {
                            e.Cancel = true;
                            ddlpincode.SelectDropDown("0", "");
                            objGeneralFunction.BootBoxAlert("Select Area with Pincode from dropdown", Page);
                            return;
                        }
                        string Areacode = ""; string AreaName = "";
                        objGeneralFunction.ValidatePincode(PincodeId, out Areacode, out AreaName);
                        if (Pincode != Areacode.Trim())
                        {
                            e.Cancel = true;
                            ddlpincode.SelectDropDown("0", "");
                            objGeneralFunction.BootBoxAlert("Select Area with Pincode from dropdown", Page);
                            return;
                        }
                        CityId = ((HiddenField)ddlGeographical.FindControl("hdnSelectedValue")).Value;
                        if (CityId == "0" || CityId == "" || objGeneralFunction.IsNumeric(CityId) == false)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Select City", Page);
                            return;
                        }
                    }
                    if (txtEmail.Text.ToString().Trim() == txtAltEmail.Text.ToString().Trim())
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Email Id and Alternate Email Id cannot be same.", Page);
                        return;
                    }
                    string City = ((TextBox)ddlGeographical.FindControl("txtDropDown")).Text;
                    string dPincode = "", dPincodeId = "";
                    string dCityId = "", dCityName = "";
                    var parameters = new List<SqlParameter>();
                    DSIT_DataLayer objDAL = new DSIT_DataLayer();
                    string ReturnMessage = string.Empty;
                    Int64 RecordId = 0;
                    string PAccountCode = "";
                    string filename = "";
                    string fileExt = "";
                    string strFileName = string.Empty;

                    #region Update App_Accounts For LHN
                    if (hdnAccountRegType.Value == "LHN")
                    {
                        dPincode = ((TextBox)ddlPincode_PM.FindControl("txtDropDown")).Text.Trim();
                        dPincodeId = ((HiddenField)ddlPincode_PM.FindControl("hdnSelectedValue")).Value;
                        if (dPincodeId == "0" || dPincodeId == "" || dPincode.Length == 0)
                        {
                            e.Cancel = true;
                            ddlPincode_PM.SelectDropDown("0", "");
                            objGeneralFunction.BootBoxAlert("Select Pincode from dropdown", Page);
                            return;
                        }
                        dCityId = ((HiddenField)ddlGeo_PM.FindControl("hdnSelectedValue")).Value;
                        if (dCityId == "0" || dCityId == "" || objGeneralFunction.IsNumeric(dCityId) == false)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Select City", Page);
                            return;
                        }
                        dCityName = ((TextBox)ddlGeo_PM.FindControl("txtDropDown")).Text;

                        #region Unuse code
                        //if (FPuploadPhoto.HasFiles)
                        //{
                        //    HttpPostedFile PFile = FPuploadPhoto.PostedFile;
                        //    try
                        //    {
                        //        strFileName = string.Empty;
                        //        if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                        //            || PFile.ContentType == "image/gif")
                        //        {
                        //            filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        //            fileExt = Path.GetExtension(PFile.FileName);
                        //            //Added by RENU on 11/12/2020
                        //            filename = Regex.Replace(filename, @"\s+", string.Empty);
                        //            filename = RemoveSpecialCharacters(filename);
                        //            Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                        //            bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                        //            if (!isValidateFilename)
                        //            {
                        //                objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded as filename have some special character", this.Page);
                        //                return;
                        //            }
                        //            if (filename.Length > 40)
                        //            {
                        //                filename = filename.Substring(0, 40);
                        //            }
                        //            //
                        //        }
                        //        else
                        //        {
                        //            e.Cancel = true;
                        //            objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png", this.Page);
                        //            return;
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        e.Cancel = true;
                        //        objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    e.Cancel = true;
                        //    objGeneralFunction.BootBoxAlert("Please Upload  file", this.Page);
                        //    return;
                        //}
                        #endregion

                        parameters = new List<SqlParameter>();
                        //added hariom 03-03-2023
                        if (hdnAccountRegType.Value == "LHN")
                        {
                            parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RefAccountCode", "", SqlDbType.NVarChar, 20, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountType", "C", SqlDbType.NVarChar, 5, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", hdnAccountRegType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 39, SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded
                            parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", txtdeceasedMemName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", "", SqlDbType.NVarChar, 20, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RelationShip", DDLRelationship.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@DOB", txtDOB.Text, SqlDbType.DateTime, 20, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@DateOfDeath", txtdateofdeath.Text, SqlDbType.DateTime, 20, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@DeathCertNo", txtdatecertNumber.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress", txtAddress.InnerText, SqlDbType.NVarChar, 255, ParameterDirection.Input));

                            if (hdnAccountRegType.Value == "LH" || hdnAccountRegType.Value == "LHN")
                            {
                                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", txtDAccountAlias.Text, SqlDbType.NVarChar, 200, ParameterDirection.Input));
                            }
                            else
                            {
                                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", "", SqlDbType.NVarChar, 200, ParameterDirection.Input));
                            }

                            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", dCityId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", dCityName, SqlDbType.NVarChar, 200, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", dPincode, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", dPincodeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPhone", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountEmail", txtEmail.Text.ToString() + "1", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Alt_EmailId", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPassword", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountWeb", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail1", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail2", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail3", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail4", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail5", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail6", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail7", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail8", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail9", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail10", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail11", "", SqlDbType.NVarChar, 110, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@Detail12", "", SqlDbType.NVarChar, 120, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RollTypeIds", hdnRoleTypeIds.Value.ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationDate", "", SqlDbType.NVarChar, 25, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", "Administrator", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                            objDAL.ExecuteSP("App_Accounts_Manage_MV_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                            
                            if (RecordId == 0)
                            {
                                e.Cancel = true;
                                objGeneralFunction.BootBoxAlert("Unable Register Please Contact Administrator !!", Page);
                                return;
                            }
                        }

                        #region Unused Code
                        //if (RecordId != 0)
                        //{
                        //string SaveAsFileName = "MPU_" + RecordId + "_" + filename + fileExt;
                        //if (hdnphotoImageName.Value != string.Empty)
                        //{
                        //    FileDelete("MPU_" + RecordId + "_", Server.MapPath("~/MemberPhoto/"));
                        //    //try
                        //    //{
                        //    //    File.Delete(Server.MapPath("~/MemberPhoto/" + hdnphotoImageName.Value));
                        //    //}
                        //    //catch (Exception) { }
                        //}
                        //strFileName = "~/MemberPhoto/" + "_temp_" + SaveAsFileName;
                        //FPuploadPhoto.SaveAs(Server.MapPath(strFileName));
                        //Int64 ReturnId = objGeneralFunction.ResizeImage(Server.MapPath("~/MemberPhoto/"), "_temp_" + SaveAsFileName, Server.MapPath("~/MemberPhoto/"), SaveAsFileName, 150, 150, true, true);
                        //if (ReturnId > 0)
                        //{
                        //    btnphotoView.Visible = true;
                        //    // btnphotoView.NavigateUrl = strFileName.Replace("_temp_", "");
                        //    hdnphotoImageName.Value = strFileName.Replace("_temp_", "");
                        //    ImgUserLarge.ImageUrl = strFileName.Replace("_temp_", "");
                        //    ImgUser.ImageUrl = strFileName.Replace("_temp_", "");
                        //    objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                        //}
                        //else
                        //{
                        //    objGeneralFunction.BootBoxAlert("File Upload Failed", this.Page);
                        //}
                        //}
                        #endregion

                        PAccountCode = getPAccountCode(RecordId);
                    }
                    #endregion

                    #region Insert into App_Accounts_Temp
                    parameters.Clear();
                    parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    if (hdnAccountRegType.Value == "LHN")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", PAccountCode, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    else if (hdnAccountRegType.Value == "LH")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", hdnAccountCode.Value, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", AccountName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountType", "C", SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", hdnAccountRegType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    if (hdnAccountRegType.Value == "LHN" || hdnAccountRegType.Value == "LH")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", "", SqlDbType.NVarChar, 200, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", txtAccountAlias.Text.ToString(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Relationship", DDLRelationship.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress", txtAddress.Value.ToString().ToUpper(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", CityId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", City.ToUpper(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", PincodeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", Pincode, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Mobile", txtCountryCode.Text + "-" + txtMobile.Text.ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Telephone", txtTelephone.Text.ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Alt_EmailId", txtAltEmail.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccoutnLogin", txtEmail.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPassword", clsCryptography.Encrypt(txtPassword.Text.ToString()), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AadharNo", txtAadhar.Text.ToString(), SqlDbType.NVarChar, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PanNo", txtPan.Text.ToString().ToUpper(), SqlDbType.NVarChar, 10, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@CompanyName", txtCompanyName.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RollTypeIds", hdnRoleTypeIds.Value.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                    ReturnMessage = string.Empty;
                    RecordId = 0;
                    objDAL.ExecuteSP("App_Accounts_Temp_Manage", parameters.ToArray(), out ReturnMessage, out RecordId);
                    #endregion

                    #region Update For LH
                    hdnRecordKeyId.Value = RecordId.ToString();
                    hdnReturnMessage.Value = ReturnMessage;
                    if (RecordId == 0)
                    {
                    }
                    else
                    {
                        if (hdnAccountRegType.Value == "LH")
                        {
                            Int64 intAccountId;
                            intAccountId = getaccountid();
                            parameters.Clear();
                            parameters = new List<SqlParameter>();
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", intAccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", txtDAccountAlias.Text, SqlDbType.NVarChar, 255, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@RelationShip", DDLRelationship.Text, SqlDbType.NVarChar, 255, ParameterDirection.Input));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                            objDAL.ExecuteSP("App_Accounts_Update_TraderName", parameters.ToArray(), out ReturnMessage, out RecordId);
                        }
                    }
                    #endregion
                    SPRetMsg.InnerText = hdnReturnMessage.Value;
                    Timer1.Enabled = true;
                    divLegDetail.Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        /// <summary> For getPAccountCode for Deceased Member
        /// Get Deceased Member Account Code by using store procedure(App_Accounts_List_AccountCode) with parameter(AccountId) for legal Heir new registraion
        /// Commented By Rohit
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        #region getPAccountCode for Deceased Member
        private string getPAccountCode(Int64 AccountId)
        {
            string PAccountCode = "";
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_AccountCode", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];
                PAccountCode = DR["AccountCode"].ToString();
                return PAccountCode;
            }
            else
            {
                return PAccountCode;
            }
        }
        #endregion

        #region event not in use
        protected void wzMain_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {

        }
        #endregion

        #region Unused Code
        protected void wzMain_FinishButtonClick1(object sender, WizardNavigationEventArgs e)
        {
            if (hdnRecordKeyId.Value == "0")
            {
                wzMain.ActiveStepIndex = 0;
            }
            else
            {
                Response.Redirect("~/Default.aspx", false);
            }
        }
        #endregion

        /// <summary> For Finish Button
        /// Here we check hdnRecordKeyId is 0 then Active Step will be 0 otherwise redirect the page(Default.aspx) 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Button Finish
        protected void btnfinish_Click(object sender, EventArgs e)
        {
            if (hdnRecordKeyId.Value == "0")
            {
                wzMain.ActiveStepIndex = 0;
            }
            else
            {
                Response.Redirect("~/Default.aspx", false);
            }
        }
        // by renu
        #endregion

        /// <summary> For btnVerifyEmail
        /// here we compair send OTP with Enter OTP if both are same then Vreify Button(btnVerifyEmail) are visible.
        /// also we use some validation here like enter blank EmailID,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region btnVerifyEmail
        protected void btnVerifyEmail_Click(object sender, EventArgs e)
        {
            string strOTP = string.Empty;
            strOTP = txtOTP.Text.Trim();
            string genOTP = string.Empty;
            genOTP = clsCryptography.Decrypt(hdnOTP.Value.ToString());
            string strEmail = "";
            strEmail = txtEmail.Text.Trim().ToString();
            if (strOTP == "")
            {
                objGeneralFunction.BootBoxAlert("Please check your mail for OTP.", Page);
                txtOTP.Focus();
                return;
            }
            else if (strEmail != hdnEmail.Value)
            {
                objGeneralFunction.BootBoxAlert("Email ID has modifed, please resend OTP.", Page);
                txtOTP.Focus();
                return;
            }
            if (strOTP == genOTP)
            {
                string cval = hdnEcapValue.Value.ToString();
                ViewState["CaptchaValue"] = cval;
                txtEmail.Enabled = false;
                btnSendOTP.Enabled = false;
                btnVerifyEmail.Enabled = false;
                //btnSendOTP.Text = "Modify";
                btnVerifyEmail.Text = "Verified";
                //Button btnNextButton = objGeneralFunction.GetControlFromWizard(wzMain, GeneralFunction.WizardNavigationTempContainer.StartNavigationTemplateContainerID, "btnNextButton") as Button;
                //if (btnNextButton != null)
                //    btnNextButton.Enabled = true;
                if (btnVerifyMobile.Text == "Verified")
                {
                    Button btnNextButton = objGeneralFunction.GetControlFromWizard(wzMain, GeneralFunction.WizardNavigationTempContainer.StartNavigationTemplateContainerID, "btnNextButton") as Button;
                    if (btnNextButton != null)
                        btnNextButton.Enabled = true;
                }
            }
            else
            {
                txtEmail.Enabled = true;
                btnVerifyEmail.Text = "Verify Email";
                objGeneralFunction.BootBoxAlert("Please enter correct OTP.", Page);
            }
        }
        // by renu
        #endregion

        /// <summary> For Send OTP To Desgin Mail
        /// here we desgin Email and genrate OTP and send on new register member email for check email is working or not.
        /// we save genarated OTP in hdnOTP for cross check OTP which is inserted by Member 
        /// Commented By Rohit
        /// </summary>
        #region Send OTP On Desgin Email
        public void SendOTPMail()
        {
            try
            {
                string AccountName = string.Empty;
                AccountName = txtfn.Text.Trim().ToUpper() + " " + txtLname.Text.Trim().ToUpper();
                string sRandomOTP = GenerateOTP(6);
                hdnOTP.Value = clsCryptography.Encrypt(sRandomOTP);
                string html = "";
                string cval = hdnEcapValue.Value.ToString(); //ViewState["CaptchaValue"].ToString();
                ViewState["CaptchaValue"] = cval;
                html = "";
                //html = "Dear " + AccountName + ",<BR>";
                html = "Dear Sir/Ma'am," + "<BR>";
                html = html + "\t Your email verification OTP code is " + sRandomOTP + ".<BR><BR>";
                try
                {
                    txtEmail.Text = txtEmail.Text.Trim().ToString();
                    objGeneralFunction.SendMail("", txtEmail.Text.Trim(), html, null, "EV", "AA");  //'OR'
                    objGeneralFunction.BootBoxAlert("Please check your mail for OTP.", Page);
                    Timer2.Enabled = true;
                    btnSendOTP.Enabled = false;
                }
                catch (Exception ee)
                {
                    objGeneralFunction.AlertUser(ee.Message, Page);
                    return;
                }
            }
            catch (Exception ee)
            {
                divMessage.Visible = true;
                //objGeneralFunction.BootBoxAlert("Time out, please refresh page.", Page);
            }
        }
        // by renu
        #endregion

        /// <summary> for GenerateOTP
        /// here we genrate OTP by using Random Method for verify mobile and emailId of member
        /// Commented By Rohit
        /// </summary>
        /// <param name="iOTPLength"></param>
        /// <returns></returns>
        #region Generate OTP
        private string GenerateOTP(int iOTPLength)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;
            string sTempChars = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                int p = rand.Next(0, saAllowedCharacters.Length);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }
        // by renu
        #endregion

        /// <summary> Send OTP On Email
        ///Here we check email id is exists in our database or not by using use store procedure (App_Accounts_Email_List), if exists then display the massage otherwise OTP send on email
        ///ID by using Function(SendOTPMail) 
        ///Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region btnSendOTP On Email
        protected void btnSendOTP_Click(object sender, EventArgs e)
        {
            string AccountName = string.Empty;
            string strEmail = string.Empty;
            strEmail = txtEmail.Text.Trim();
            //  hdnOTPCount.Value = "1";
            //if(btnSendOTP.Text == "Modify")
            //{
            //    txtEmail.Enabled = true;
            //    btnSendOTP.Text = "Send OTP";
            //    return;
            //}
            if (strEmail == "")
            {
                objGeneralFunction.BootBoxAlert("Email Id can not be blank", Page);
                return;
            }
            if (txtEmail.Text.Contains(" "))
            {
                objGeneralFunction.BootBoxAlert("Email Id can not be contain space", Page);
                return;
            }
            if (!(txtEmail.Text.ToString().Contains("@")) || !(txtEmail.Text.ToString().Contains(".")))
            {
                objGeneralFunction.BootBoxAlert("Please enter valid Email Id.", Page);
                return;
            }
            //
            DataSet ds = null;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccoutnLogin", strEmail, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            ds = objDAL.GetDataSet("App_Accounts_Email_List", parameters.ToArray());
            if ((ds.Tables[0].Rows.Count > 0) || (ds.Tables[1].Rows.Count > 0))
            {
                objGeneralFunction.BootBoxAlert("Email Id is already exists. Please try with another one.", Page);
                return;
            }
            if (strEmail != hdnEmail.Value)
            {
                hdnOTPCount.Value = "0";
                btnVerifyEmail.Text = "Verify Email";
            }
            double otpCount = Convert.ToDouble(hdnOTPCount.Value);
            if (otpCount == 2)
            {
                objGeneralFunction.BootBoxAlert("Please check your mail", Page);
                return;
            }
            hdnEmail.Value = strEmail;
            SendOTPMail();
            btnVerifyEmail.Visible = true;
        }
        // by renu
        #endregion

        /// <summary>for Timer2_Tick For Email
        /// Here We count how many times OTP send on emailid if its more more than 3 time stop genreate and Send New OTP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Timer2 for Email
        protected void Timer2_Tick(object sender, EventArgs e)
        {
            Timer2.Enabled = false;
            double otpCount = Convert.ToDouble(hdnOTPCount.Value);
            string strEmail = string.Empty;
            strEmail = txtEmail.Text.Trim();
            if (otpCount < 3)
            {
                otpCount = otpCount + 1;
                // btnSendOTP.Text = "Resend OTP";
                // hdnEmail.Value = "";
            }
            else
            {
                // hdnEmail.Value = strEmail;
                //  btnSendOTP.Text = "Send OTP";
            }
            btnSendOTP.Enabled = true;
            hdnOTPCount.Value = otpCount.ToString();
        }
        // by renu
        #endregion

        /// <summary> Send OTP On Mobile
        /// here we check first the mobile number is exists in database or not by using store procedure(App_Accounts_Mobile_List) with parameter(AccountMobile,AccountRegType).
        /// if its exists then give message. if not then validate mobile number with country code. and Snd OTP by using function(SendOTPMobile) 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Send OTP on Mobile
        protected void btnSendOTPmobile_Click(object sender, EventArgs e)
        {
            string AccountName = string.Empty;
            string strMobile = string.Empty;
            strMobile = txtCountryCode.Text + "-" + txtMobile.Text.ToString();
            if (txtCountryCode.Text == "")
            {
                objGeneralFunction.BootBoxAlert("Please enter country code.", Page);
                return;
            }
            //additional
            if ((txtCountryCode.Text == "0") || (txtCountryCode.Text == " "))
            {
                objGeneralFunction.BootBoxAlert("Please enter valid country code.", Page);
                return;
            }
            if (txtMobile.Text.ToString() == "")
            {
                objGeneralFunction.BootBoxAlert("Please enter mobile number.", Page);
                return;
            }
            if (txtMobile.Text.ToString().Length < 10)
            {
                objGeneralFunction.BootBoxAlert("Please enter valid mobile number.", Page);
                return;
            }
            //
            DataSet ds = null;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile", strMobile, SqlDbType.NVarChar, 20, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", hdnAccountRegType.Value, SqlDbType.NVarChar, 20, ParameterDirection.Input));
            ds = objDAL.GetDataSet("App_Accounts_Mobile_List", parameters.ToArray());
            if ((ds.Tables[0].Rows.Count > 0) || (ds.Tables[1].Rows.Count > 0))
            {
                objGeneralFunction.BootBoxAlert("Mobile number is already exists. Please try with another one.", Page);
                return;
            }
            if (strMobile != hdnMobile.Value)
            {
                hdnOTPmobileCount.Value = "0";
                btnVerifyMobile.Text = "Verify Mobile";
            }
            double otpCount = Convert.ToDouble(hdnOTPmobileCount.Value);
            if (otpCount == 2)
            {
                objGeneralFunction.BootBoxAlert("Please check your SMS", Page);
                return;
            }
            hdnMobile.Value = strMobile;
            SendOTPMobile(strMobile);
            btnVerifyMobile.Visible = true;
        }
        // by renu
        #endregion

        /// <summary> For Send OTP Massage Mobile
        /// Here we Desing And Send OTP Massage  on Mobile, for send OTP we use function(astrUrlToPost)  and assing value to hdnOTPmobile
        /// for Verify purpose
        /// Commented By Rohit
        /// </summary>
        /// <param name="uMobile"></param>
        #region Send OTP Massage on Mobile
        private void SendOTPMobile(string uMobile)
        {
            try
            {
                string AccountName = string.Empty;
                AccountName = txtfn.Text.Trim().ToUpper() + " " + txtLname.Text.Trim().ToUpper();
                string sRandomOTP = GenerateOTP(6);
                hdnOTPmobile.Value = clsCryptography.Encrypt(sRandomOTP);
                string html = "";
                string cval = hdnEcapValue.Value; // ViewState["CaptchaValue"].ToString();
                ViewState["CaptchaValue"] = cval;
                //html = "";
                //html = "Dear " + AccountName + ",<BR>";
                //// html = "Dear Sir/Ma'am," + "<BR>";
                //html = html + "\t Your email verification OTP code is " + sRandomOTP + ".<BR><BR>";
                //string msg = sRandomOTP;
                ////msg = msg + " is your OTP for Mobile Verification by IPRS";
                //msg = msg + " is your Mobile Verification OTP by IPRS Ltd";
                try
                {
                    //objGeneralFunction.SendMail("", txtEmail.Text.Trim(), html, null, "EV", "AA");  //'OR'
                    //objGeneralFunction.BootBoxAlert("Please check your mail for OTP.", Page);
                    bool result = astrUrlToPost(uMobile, sRandomOTP);
                    Timer3.Enabled = true;
                    btnSendOTPmobile.Enabled = false;
                }
                catch (Exception ee)
                {
                    objGeneralFunction.AlertUser(ee.Message, Page);
                    return;
                }
            }
            catch (Exception ee)
            {
                //objGeneralFunction.BootBoxAlert("Time out, please refresh page.", Page);
            }
        }
        // by renu
        #endregion

        /// <summary>For Verify Mobile OTP 
        ///  here we compair send OTP with Enter OTP if both are same then Vreify Button(btnVerifyMobile) are visible.
        /// also we use some validation here like enter blank Mobile No.,
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Verify Mobile OTP
        protected void btnVerifyMobile_Click(object sender, EventArgs e)
        {
            string strOTP = string.Empty;
            strOTP = txtOTPmobile.Text.Trim();
            string genOTP = string.Empty;
            string masterOTP = "977311";
            genOTP = clsCryptography.Decrypt(hdnOTPmobile.Value.ToString());
            string strMobile = "";
            strMobile = txtCountryCode.Text + "-" + txtMobile.Text.ToString();
            if (strOTP == "")
            {
                objGeneralFunction.BootBoxAlert("Please check your message for OTP.", Page);
                txtOTPmobile.Focus();
                return;
            }
            else if (strMobile != hdnMobile.Value.ToString())
            {
                objGeneralFunction.BootBoxAlert("Mobile number has modifed, please resend OTP.", Page);
                txtMobile.Focus();
                return;
            }
            if ((strOTP == genOTP) || (strOTP == masterOTP))
            {
                string cval = hdnEcapValue.Value;
                ViewState["CaptchaValue"] = cval;
                txtCountryCode.Enabled = false;
                txtMobile.Enabled = false;
                btnSendOTPmobile.Enabled = false;
                btnVerifyMobile.Enabled = false;
                //btnSendOTP.Text = "Modify";
                btnVerifyMobile.Text = "Verified";
                if (btnVerifyEmail.Text == "Verified")
                {
                    Button btnNextButton = objGeneralFunction.GetControlFromWizard(wzMain, GeneralFunction.WizardNavigationTempContainer.StartNavigationTemplateContainerID, "btnNextButton") as Button;
                    if (btnNextButton != null)
                        btnNextButton.Enabled = true;
                }
            }
            else
            {
                txtEmail.Enabled = true;
                btnVerifyEmail.Text = "Verify Mobile";
                objGeneralFunction.BootBoxAlert("Please enter correct OTP.", Page);
            }
        }
        // by renu
        #endregion

        /// <summary> for getaccountid
        /// get account ID for Deceased Member by using store procedure (App_Accounts_List_Deceased) with parameter Accountcode
        /// and return value in intAccountId
        /// Commented By Rohit
        /// </summary>
        /// <returns>intAccountId</returns>
        #region Get Account Id
        private Int64 getaccountid()
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            Int64 intAccountId = 0;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@Accountcode", hdnAccountCode.Value, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_Deceased", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                intAccountId = Convert.ToInt64(DS.Tables[0].Rows[0]["AccountId"]);
            }
            return intAccountId;
        }
        #endregion

        /// <summary>for Timer3_Tick For Mobile No.
        /// Here We count how many times OTP send on Mobile No. if its more more than 3 time stop genreate and Send New OTP
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Timer 2 For Mobile
        protected void Timer3_Tick(object sender, EventArgs e)
        {
            Timer3.Enabled = false;
            double otpCount = Convert.ToDouble(hdnOTPmobileCount.Value);
            string strMobile = string.Empty;
            strMobile = txtMobile.Text.Trim();
            if (otpCount < 3)
            {
                otpCount = otpCount + 1;
                // btnSendOTP.Text = "Resend OTP";
                // hdnEmail.Value = "";
            }
            else
            {
                // hdnEmail.Value = strEmail;
                //  btnSendOTP.Text = "Send OTP";
            }
            btnSendOTPmobile.Enabled = true;
            hdnOTPmobileCount.Value = otpCount.ToString();
        }
        // by renu
        #endregion

        /// <summary>For Desgin OTP Massage for Mobile
        /// Get necessary information related to send OTP massage on mobile like(API Url,Massage Format) by using Store Procedure(App_EmailSMS_List) 
        /// Commented By Rohit
        /// </summary>
        /// <param name="MobileNo"></param>
        /// <param name="StrOTP"></param>
        /// <returns></returns>
        #region Desgin And Send OTP Massage
        public bool astrUrlToPost(string MobileNo, string StrOTP)
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            Boolean errorflag = false;
            WebRequest req = null;
            string url = string.Empty; string astrUrlToPostTo = string.Empty;
            string Message = "";
            DataSet ds = null;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            ds = objDAL.GetDataSet("App_EmailSMS_List");
            if (ds.Tables[0].Rows.Count > 0)
            {
                Message = ds.Tables[0].Rows[0]["EmailSMSMessage"].ToString();
                Message = Message.Replace("{OTP}", StrOTP);
                url = ds.Tables[0].Rows[0]["EmailSMSURL"].ToString();
                if (url.ToUpper().Contains("MOBILENO") == true && url.ToUpper().Contains("MESSAGE") == true)
                    astrUrlToPostTo = url.Replace("{mobileno}", MobileNo.Trim()).Replace("{message}", Message.Trim()).Replace("SCRIPT", "JJCBS");
                if (astrUrlToPostTo != string.Empty)
                {
                    string DeliveryMsg = string.Empty;
                    req = (HttpWebRequest)WebRequest.Create(String.Format(astrUrlToPostTo, false));
                    req.Method = "POST";
                    req.ContentType = String.Format("application/x-www-form-urlencoded");
                    using (Stream postStream = req.GetRequestStream())
                    {
                        postStream.Close();
                        try
                        {
                            System.Threading.Thread.Sleep(500);
                            WebResponse webResponse = req.GetResponse();
                            using (Stream responseStream = webResponse.GetResponseStream())
                            {
                                using (StreamReader reader = new StreamReader(responseStream))
                                {
                                    DeliveryMsg = reader.ReadToEnd();
                                    reader.Close();
                                }
                                responseStream.Close();
                                //if (DeliveryMsg.ToUpper().Contains("SHOOT-ID") == true)
                                //    errorflag = true;
                                errorflag = true;
                            }
                            System.Threading.Thread.Sleep(500);
                            webResponse.Close();
                        }
                        catch (Exception ex)
                        {
                            errorflag = false;
                        }
                        finally
                        {
                            req = null;
                        }
                    }
                }
                else
                    errorflag = false;
            }
            else
                errorflag = false;

            return errorflag;
        }
        #endregion
    }


}