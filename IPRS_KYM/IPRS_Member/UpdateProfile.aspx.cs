using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using IPRS_Member.User_Controls;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using static IPRS_Member.User_Controls.ucDropDown;
using Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.text;
using ListItem = System.Web.UI.WebControls.ListItem;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Remoting.Messaging;
using System.Web.Security;
using System.Reflection;
using System.Net.Sockets;

namespace IPRS_Member
{
    public partial class UpdateProfile : System.Web.UI.Page
    {
        /// <summary>
        /// here we create object for GeneralFunction and User Control(ucNomineeDetails)
        /// Commented By Rohit
        /// </summary>
        GeneralFunction objGeneralFunction = new GeneralFunction();
        ucNomineeDetails ucNgrdview = new ucNomineeDetails();
        string MessageReturn = string.Empty;
        string zeroworkid= string.Empty;


        //static string strGSTLookupId = "22";
        //static string strOSILookupId = "6";
        //static string strOSCLookupId = "12";
        //static string strCompanyId_SP = "14";
        //static string strCompanyId_PR = "15,16";
        //static string strCompanyId_CP = "17,18,19,20";
        //static string strAdressLookupId = "4";

        /// <summary>
        /// For SetStepHighlight
        /// Here we Check Selected Wizard Steps and Disabled Wizard Step
        /// Commented By Rohit
        /// </summary>
        #region Check And HighLight Wizard Setps
        protected void SetStepHighlight()
        {
            #region "SET STEPS HIGHLIGHT"
            try
            {
                for (int i = 0; i < wzMain.WizardSteps.Count - 2; i++)
                {
                    HtmlAnchor aStep = (HtmlAnchor)Master.FindControl("ContentPlaceHolder1").FindControl("aStep" + (i + 1).ToString());
                    if (wzMain.ActiveStepIndex == i)
                        aStep.Attributes.Add("class", "selected");
                    else
                        aStep.Attributes.Add("class", "disabled");
                }
            }
            catch (Exception ex)
            {
                objGeneralFunction.GenerateError(Page, "wzMain_ActiveStepChanged", ex.Message);
            }
            #endregion "SET STEPS HIGHLIGHT"
        }
        #endregion

        /// <summary> 
        /// For Page_Load
        /// Here we first Step4(Nominee Detail Information) visible false, and Cheak Session["AccountId"] is Null or not.
        /// Assign Value hdnRecordId and hdnRegistrationType,if Registration Type LH OR LHN then related Control and Divison are visible or not.
        /// get geograaphical data using Store Procedure(Area_Populate) and asign value to dropdown(ddlPincode_PM and ddlPincode_PR).
        /// get Language Information by using Store Procedure(App_Language_List) and asign value to dropdown(ddlMotherLang).
        /// get Currency Information by using Store Procedure(App_Currency_List) and asign value to dropdown(ddlMotherLang).
        /// get Country List by using Store Procedure(App_Country_List) and asign value to dropdown(ddlCountry and DDLPerCountry).
        /// get Society List by using Store Procedure(App_Society_List) and asign value to dropdown(ddlOverseasSocietyName).
        /// Populate Song different data List from Table(App_WorkCategory) Using parameter @TypeCode TypeCode in table(App_WorkCategory) is 
        /// for WK = Work Category, WSK = Work Sub Category, IP = Intended Purpose, MR = Music Relationship, BLTVR = BLTVR by using 
        /// store procedure(App_SongDetails_List)
        /// Populate Song Category List only for those Catgory having Typecode is WK from Table(App_WorkCategory) by using store procedure(App_Category_List)
        /// Populate society list from Table(App_Society_Master) using parameter @RecordKeyId,here @RecordKeyId = 0 means get list of all society without 
        /// Non Member Society and @RecordKeyId = 1 means get Non Member Society value by using store procedure(App_PerformSociety_List)
        /// Assign Value to related Controls By Using Function(DisplayOperation). Finally check Application Status = 1 then active Step 6.
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.TextBox.DisabledCssClass = "";
            if (Session["AccountId"] == null)
                Response.Redirect("MemberLogin", false);//Temproary Basis
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            ddlPincode_PM.PostButtonClick += ddlPincode_PM_Selected;
            ddlPincode_PR.PostButtonClick += ddlPincode_PR_Selected;
            // ddlWorkCategory.PostButtonClick += ddlWorkCategory_Selected;
            aStep4.Visible = false; //added by Hariom 28-03-23 Nominee information tab hide

            if (!IsPostBack)
            {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                hdnRecordId.Value = Session["AccountId"].ToString();
                hdnRegistrationType.Value = Session["AccountRegType"].ToString();
                if (hdnRegistrationType.Value == "LH")
                {
                    hdnRefAccountCode.Value = Session["RefAccountCode"].ToString();
                    hdnRefAccountId.Value = getRefAccountId(hdnRefAccountCode.Value.ToString());
                    divLegDetail.Visible = true;
                    divTrader.Visible = true;
                    DisplayExistingDetails();
                    btnPhotoUploadd.Visible = false;
                    FPuploadPhotod.Visible = false;
                    btnAddWork.Visible = false;
                    divAlias.Visible = false;
                    divCompany.Visible = false;
                    txtdmembername.Enabled = false;
                    txtdreationship.Enabled = false;
                    txtddatebirth.Enabled = false;
                    txtddeathofdate.Enabled = false;
                    txtddeathofcert.Enabled = false;
                    txtdadd.Disabled = true;
                    txtdpin.Enabled = false;
                    txtdgeo.Enabled = false;
                    txtDAccountAlias.Enabled = false;
                    cbxRollTyped.Enabled = false;
                    txtIPI.Enabled = true; //changed by Hariom 20-02-23
                }
                if (hdnRegistrationType.Value == "LHN")
                {
                    hdnRefAccountCode.Value = Session["RefAccountCode"].ToString();
                    hdnRefAccountId.Value = getRefAccountId(hdnRefAccountCode.Value.ToString());
                    divLegDetail.Visible = true;
                    divTrader.Visible = true;
                    DisplayExistingDetails();
                    btnPhotoUploadd.Visible = true;
                    FPuploadPhotod.Visible = true;
                    btnAddWork.Visible = true;
                    divAlias.Visible = false;
                    divCompany.Visible = false;
                    cbxRollTyped.Enabled = false;
                }

                #region "POPULATE Permanent Area"                
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlPincode_PM.PopulateDropDown("Area_Populate", parameters, "Area");
                #endregion

                #region "POPULATE Present Area"
                parameters.Clear();
                parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlPincode_PR.PopulateDropDown("Area_Populate", parameters, "Area");
                #endregion

                #region "POPULATE MOTHER TOUNGE LANGUAGE"
                var parame = new List<SqlParameter>();
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@Status", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlMotherLang.PopulateDropDown("App_Language_List", parame, "Language");
                #endregion

                #region "POPULATE CURRENCY"
                parameters.Clear();
                parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                DDLCurrency.PopulateDropDown("App_Currency_List", parameters, "Currency");
                DDLCurrency.SelectDropDown("1", "Indian Rupees");
                #endregion

                #region "POPULATE PreCountry"
                parameters.Clear();
                parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlCountry.PopulateDropDown("App_Country_List", parameters, "Country");
                #endregion

                #region "POPULATE PerCountry"
                parameters.Clear();
                parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                DDLPerCountry.PopulateDropDown("App_Country_List", parameters, "Country");
                #endregion

                #region "POPULATE Society"            
                var param = new List<SqlParameter>();
                param.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlOverseasSocietyName.PopulateDropDown("App_Society_List", param, "Society");
                #endregion

                #region Zero work
                divPublisher.Visible = false;
                ddlWorkSubCategory.Enabled = false;
                ddlPerformingSociety.Enabled = false;
                txtACPerformanceShare.Text = "00";
                txtACMechanicalShare.Text = "00";
                txtACPerformanceShare.Enabled = false;
                txtACMechanicalShare.Enabled = false;
                txtACIPInumber.Enabled = false;

                btnCmpSongDetail.Enabled = false;

                #region ISRC No
                if (hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NC")
                {
                    txtISRCNo.Enabled = true;
                }
                else
                {
                    txtISRCNo.Enabled = false;
                }
                #endregion

                #region Populate Intended Purpose
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "IP", SqlDbType.NVarChar, 10, ParameterDirection.Input));
                objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlIntendedPurpose, "Intended Purpose");
                #endregion

                #region Song Language
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@Status", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                ddlLanguage.PopulateDropDown("App_Language_List", parame, "Song Language");
                #endregion

                #region Populate Version
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "VR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
                objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlVersion, "Version");
                #endregion

                #region Populate Music Relation
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "MR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
                objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlMusicRelation, "Music Relation");
                #endregion

                #region Populate BlTVR
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "BLTVR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
                objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlBLTVR, "BlTVR");
                #endregion

                #region Populate Category
                objDAL.FillDropDown("App_Category_List", ddlWorkCategory, "Work Category");
                #endregion

                #region Populate Perform Society
                parame.Clear();
                parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
                ddlPerformingSociety.SelectedIndex = 1;
                #endregion

                #endregion

                //rbtRegistrationType_SelectedIndexChanged(null, null);
                if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                {
                    EnaDisDIVd();
                    divroletyped.Visible = true;
                    divroletype.Visible = false;
                }
                else
                {
                    EnaDisDIV();
                    divroletyped.Visible = false;
                    divroletyped.Visible = true;
                }
                DisplayOperation();
                if (hdnApplicationStatus.Value == "1")
                {
                    wzMain.ActiveStepIndex = 6;
                }
            }
            if (Convert.ToInt16(Session["ApprovalLogcount"]) > 0)
            {
                sp1.Attributes["onclick"] = "RedirectTowizradIndex(this);";
                sp2.Attributes["onclick"] = "RedirectTowizradIndex(this);";
                sp3.Attributes["onclick"] = "RedirectTowizradIndex(this);";
                sp4.Attributes["onclick"] = "RedirectTowizradIndex(this);";
                sp5.Attributes["onclick"] = "RedirectTowizradIndex(this);";
                sp6.Attributes["onclick"] = "RedirectTowizradIndex(this);";
            }
            else
            {
                sp1.Attributes["onclick"] = "javascript:void(0);";
                sp2.Attributes["onclick"] = "javascript:void(0);";
                sp3.Attributes["onclick"] = "javascript:void(0);";
                sp4.Attributes["onclick"] = "javascript:void(0);";
                sp5.Attributes["onclick"] = "javascript:void(0);;";
                sp6.Attributes["onclick"] = "javascript:void(0);;";
            }
        }
        #endregion

        /// <summary>
        /// For DisplayExistingDetails
        /// In this Function Get Deceased Member Information using Store Procedure(App_Accounts_List_Deceased) and asign value to related controls
        /// Commented By Rohit
        /// </summary>
        #region Display Existing Member Details
        protected void DisplayExistingDetails()
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@Accountcode", hdnRefAccountCode.Value, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_Deceased", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];
                txtdmembername.Text  = DR["AccountName"].ToString();
                //hdnDAccountId.Value = DR["AccountId"].ToString();
                txtdmembername.ReadOnly = true;
                if (DR["Relationship"].ToString() != "")
                {
                    txtdreationship.Text   = DR["Relationship"].ToString();
                }
                txtdreationship.ReadOnly = true;
                txtddatebirth.Text = String.Format("{0:dd/MMM/yyyy}", DR["DOB"]).ToString();
                txtddatebirth.ReadOnly = true;                
                txtddeathofdate.Text = String.Format("{0:dd/MMM/yyyy}", DR["DateOfDeath"]).ToString();
                txtddeathofdate.ReadOnly = true;                
                txtddeathofcert.Text = DR["DeathCertificateNo"].ToString();
                txtddeathofcert.ReadOnly = true;
                //txtAddress_PM.InnerText = DR["AccountAddress"].ToString();
                //txtAddress_PM.Disabled = true;
                txtdadd.InnerText = DR["AccountAddress_PR"].ToString();
                txtdadd.Disabled = true;
                //txtpincode.Text = DR["Pincode"].ToString();
                //txtpincode.ReadOnly = true;
                txtdpin.Text = DR["Pincode_PR"].ToString();
                txtdpin.ReadOnly = true;
                txtdgeo.Text = DR["GeographicalName_PR"].ToString();
                txtdgeo.ReadOnly = true;
                txtDAccountAlias.Text = DR["AccountAlias"].ToString();
                txtDAccountAlias.ReadOnly = true;
                //Added by Hariom 20-02-2023
                txtIPI.Text = DR["IPINumber"].ToString();
                txtIPI.ReadOnly = true;
                //end
                //txtgeo.Text = DR["GeographicalName"].ToString();
                //tgeo.ReadOnly = true;
                //ddlGeo_PM.SelectDropDown(DR["GeographicalId"].ToString(), DR["GeographicalName"].ToString());
                //ddlGeo_PM.blEnabled = false;
                //ddlPincode_PM.SelectDropDown(DR["PincodeId"].ToString(), DR["Pincode"].ToString());
                //ddlPincode_PM.blEnabled = false;                
                DisplayMemberImageDeceased();
            }
        }
        #endregion

        /// <summary>
        /// For DisplayMemberImageDeceased
        /// In this Function Check Deceased Member Image is Exists in server MemberPhoto folder if exists then image asgin to Hyperlink(btnphotoViewd)
        /// Commented By Rohit
        /// </summary>
        #region Display Deceased Member Image
        protected void DisplayMemberImageDeceased()
        {
            string FilePath = string.Empty;
            int FileExist = 0;
            btnphotoViewd.Visible = false;
            ImgUserd.ImageUrl = "~/Images/user.png";
            ImgUserLarged.ImageUrl = "~/Images/user.png";
            try
            {
                FilePath = Server.MapPath("~/MemberPhoto/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MPU_" + hdnRefAccountId.Value + "_") == true)
                    {
                        FileExist = 1;
                        string filenm = Path.GetFileName(fileName);
                        //btnphotoView.NavigateUrl = "~/MemberPhoto/" + filenm;
                        ImgUserLarged.ImageUrl = "~/MemberPhoto/" + filenm;
                        ImgUserd.ImageUrl = "~/MemberPhoto/" + filenm;
                        hdnphotoImageNamed.Value = filenm;
                        break;
                    }
                }
                if (FileExist == 1)
                {
                    btnphotoViewd.Visible = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Code Not In Use
        //private void ddlOverseasSocietyName_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        //{
        //    DataTable DT = new DataTable();
        //    #region "POPULATE GEOGRPHICAL LOCATION"
        //    ddlOverseasSocietyName.SelectDropDown("0", "");
        //    ddlOverseasSocietyName.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
        //    hdnsocietyName.Value = e.SelectedText.Split('-').Last().TrimStart().TrimEnd();
        //    var parameters = new List<SqlParameter>();
        //    //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalLevel", "6", SqlDbType.TinyInt, 0, ParameterDirection.Input));
        //    //parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", e.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));
        //    DSIT_DataLayer objDAL = new DSIT_DataLayer();
        //    DT = objDAL.GetDataTable("App_Society_List", parameters.ToArray());
        //    if (DT.Rows.Count > 0)
        //    {
        //        ddlOverseasSocietyName.SelectDropDown(DT.Rows[0]["SocietyId"].ToString(), DT.Rows[0]["OverseasSocietyName"].ToString());
        //    }
        //    #endregion
        //}
        #endregion

        /// <summary>
        /// For ddlPincode_PR_Selected
        /// In this event get geographical data using Store Procedure(App_Geographical_Populate_IPM) on DropDown(ddlPincode_PR) Selected Value
        /// and asgin value to Dorpdown(ddlGeo_PR) Using User Control(ucDropDown).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Geographical Data For Present Address
        private void ddlPincode_PR_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlPincode_PR.SelectDropDown("0", "");
            ddlPincode_PR.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            //changed by Renu
            //ddlPincode_PR.SelectDropDown(e.SelectedValue, e.SelectedText.TrimStart().TrimEnd());
            //Added by Renu on 15/12/2020
            hdnpincode.Value = e.SelectedText.Split('-').Last().TrimStart().TrimEnd();
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
        #endregion

        /// <summary>
        /// For ddlPincode_PM_Selected
        /// In this event get geographical data using Store Procedure(App_Geographical_Populate_IPM) on DropDown(ddlPincode_PR) Selected Value
        /// and asgin value to Dorpdown(ddlGeo_PR) Using User Control(ucDropDown)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Geographical Data For Permenant Adress
        private void ddlPincode_PM_Selected(Object sender, ucDropDown.PostButtonClickArg e)
        {
            DataTable DT = new DataTable();
            #region "POPULATE GEOGRPHICAL LOCATION"
            ddlPincode_PM.SelectDropDown("0", "");
            ddlPincode_PM.SelectDropDown(e.SelectedValue, e.SelectedText.Split('-')[1].TrimStart().TrimEnd());
            //changed by Renu
            //ddlPincode_PM.SelectDropDown(e.SelectedValue, e.SelectedText.TrimStart().TrimEnd());
            //Added by Renu on 15/12/2020
            hdnpincodePM.Value = e.SelectedText.Split('-').Last().TrimStart().TrimEnd();
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

        /// <summary>
        /// For btnStepNext_Click
        /// On this button click wizard(wzMain) step redirect from current step to next step 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Wizard Next Step Cahnage
        protected void btnStepNext_Click(object sender, EventArgs e)
        {
            wzMain.ActiveStepIndex = wzMain.ActiveStepIndex + 1;
        }
        #endregion

        /// <summary>
        /// For btnStepPrevious_Click
        /// On this button click wizard(wzMain) step redirect from current step to previous step 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Wizard Previous Step Cahnage
        protected void btnStepPrevious_Click(object sender, EventArgs e)
        {
            wzMain.ActiveStepIndex = wzMain.ActiveStepIndex - 1;
        }
        #endregion

        /// <summary>
        /// For BooSocietyFound
        /// get society information using store procedure(App_Accounts_List_SocietyName) and for that we use paramter(ddlOverseasSocietyName)
        /// Commented By Rohit
        /// </summary>
        /// <returns> true or False</returns>
        #region get society name
        private Boolean BooSocietyFound()
        {         
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            string SocietyName = ((TextBox)ddlOverseasSocietyName.FindControl("txtDropDown")).Text;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@SocietyName", SocietyName, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_SocietyName", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// For wzMain_ActiveStepChanged
        /// here check which wizard(wzMain) step is active and asper that get information from different function and assign value to related controls
        /// if ActiveStepIndex=0 (Personal Information) then get inforamtion using function(DisplayOperation) and assign value to related controls
        ///if ActiveStepIndex=1 (Bank Details) then get inforamtion using function(PopulateBankInfo) and assign value to related controls
        ///if ActiveStepIndex=2 (Work Notification) then get inforamtion using function(DisplayWorkDetails) and assign value to related controls
        ///if ActiveStepIndex=4 (Document Upload) then get inforamtion using function(loadGrdDocumentsPreApproval) and assign value to related controls
        ///if ActiveStepIndex=5 (Submit Application) check information asper the validation from using function(loadGrdDocumentsPreApproval) and open payment button using function(TogglePaymentButton)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Assign Value Active Step
        protected void wzMain_ActiveStepChanged(object sender, EventArgs e)
        {
            if (wzMain.ActiveStepIndex != 6)
            {
                SetStepHighlight();
            }
            Button btnStepNext = wzMain.FindControl("StepNavigationTemplateContainerID$btnStepNext") as Button;
            Button btnContinue = wzMain.FindControl("StepNavigationTemplateContainerID$btnContinue") as Button;
            btnStepNext.Visible = true;
            btnContinue.Visible = false;
            
            //PLEASE DONT REMOVE THIS
            #region This method is called again as some times bank data is getting updated in session out
            if (wzMain.ActiveStepIndex == 0)
            {
                DisplayOperation();
            }
            #endregion

            #region ActiveStepIndex == 1
            if (wzMain.ActiveStepIndex == 1)
            {
                txtBankName.Focus();

                #region This method is called again as some times blank data is getting updated in session out
                if (txtBankName.Text == string.Empty)
                {
                    PopulateBankInfo(null);
                }
                #endregion

                if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC")
                {
                    txtIFSC.Enabled = false;
                    btnvalidate.Enabled = false;
                    txtBankName.Enabled = true;
                    txtBankName.ReadOnly = false;
                    txtBankBranchName.Enabled = true;
                    txtBankBranchName.ReadOnly = false;
                    txtMICR.Enabled = true;
                    txtMICR.ReadOnly = false;
                }
            }
            #endregion

            #region ActiveStepIndex == 2
            if (wzMain.ActiveStepIndex == 2)
            {
                btnStepNext.Visible = false;
                btnContinue.Visible = true;
                txtSongName.Focus();
                DisplayWorkDetails();
                //DisplaySongDetails();
            }
            #endregion

            #region ActiveStepIndex == 4
            if (wzMain.ActiveStepIndex == 4)
            {
                loadGrdDocumentsPreApproval();
            }
            #endregion

            #region ActiveStepIndex == 5
            if (wzMain.ActiveStepIndex == 5)
            {
                if (hdnRegistrationType.Value != "LH")
                {
                    if (gvWork.Rows.Count == 0)
                    {
                        objGeneralFunction.BootBoxAlert("Atleast 1 Work Notification Required", Page);
                        return;
                    }
                }

                loadGrdDocumentsPreApproval();
                DataTable DT = (DataTable)ViewState["DocLookUp"];
                if (DT != null)
                {
                    object result;
                    result = DT.Compute("SUM(Uploaded)", "IsCompulsary=0");
                    DataRow[] DR = DT.Select("IsCompulsary=0");
                    if (Convert.ToDouble(result) < DR.Length)
                    {
                        wzMain.ActiveStepIndex = 4;
                        objGeneralFunction.BootBoxAlert("Please Upload the List of Douments", Page);
                        return;
                    }
                }
                TogglePaymentButton();
                btnStepNext.Visible = false;
            }
            #endregion
        }

        #endregion

        /// <summary>
        /// For rbtnlMemberoverseas_SelectedIndexChanged
        /// If Member Select Radio Button(rbtnlMemberoverseas_SelectedIndexChanged) Value is overseas 
        /// then Related Division And Controls Are Visible
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Overseas Member
        protected void rbtnlMemberoverseas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnlMemberoverseas.SelectedValue == "Y")
            {
                pnlMemberOverseas.Visible = true;
                divIPINumber.Visible = true;
                divInternalNumber.Visible = true;
                txtInternalName.Text = "";
                DDLTerAppFor.SelectedIndex = 1;
                DDLTerAppFor.Enabled = true;
            }
            else
            {
                pnlMemberOverseas.Visible = false;
                divIPINumber.Visible = false;
                divInternalNumber.Visible = false;
                DDLTerAppFor.SelectedIndex = 2;
                DDLTerAppFor.Enabled = false;
                ((TextBox)ddlOverseasSocietyName.FindControl("txtDropDown")).Text = "";
                ((HiddenField)ddlOverseasSocietyName.FindControl("hdnSelectedValue")).Value = "0";
            }
        }
        #endregion

        /// <summary>
        /// For rbtnlPDCheckAddress_SelectedIndexChanged
        /// If member Select radio Button(rbtnlPDCheckAddress) is Yes then related Panle is Visible
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Permenant Address
        protected void rbtnlPDCheckAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnlPDCheckAddress.SelectedValue == "N")
            {
                pnlPDPermanentAddress.Visible = true;
            }
            else
            {
                pnlPDPermanentAddress.Visible = false;
                //txtAddress_PM.InnerText ="";
                ////DDLPerCountry
                //txtPerState.Text = "";
                //txtPerCity.Text = "";
                //txtperZipCode.Text = "";
            }
        }
        #endregion

        /// <summary>
        /// For loadGrdDocumentsPreApproval
        /// check validation asper the input value and registration type for get related document list by using store 
        /// procedure(Doc_LookUp_ListData_IPM) and assign/Bind dataset value to viewstate(ViewState["DocLookUp"]) and 
        /// Gridview(grdDocumentsPreApproval).
        /// Commented By Rohit
        /// </summary>
        #region Get Requierd Document List
        protected void loadGrdDocumentsPreApproval()
        {
            DataSet ds = null;
            string DocSubtype = ",";

            #region Changing GST Compulsory
            if (txtDetails1_gst.Text != string.Empty)
            { DocSubtype = DocSubtype + "GST,"; }
            #endregion

            #region Changing Overseas Compulsory
            string SocietyName = ((TextBox)ddlOverseasSocietyName.FindControl("txtDropDown")).Text;
            if (SocietyName.ToString()  != string.Empty)
            {
                { DocSubtype = DocSubtype + "OVR,"; }
            }
            #endregion

            #region Changing Permanent Compulsory
            if (txtAddress_PR.InnerText != string.Empty)
            {
                { DocSubtype = DocSubtype + "ADD,"; }

            }
            #endregion

            #region Changing Company Docs
            if (hdnRegistrationType.Value == "C")
            {
                { DocSubtype = DocSubtype + rbtEntityType.SelectedValue + ","; }
            }
            #endregion

            try
            {
                string DocType = string.Empty;
                if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "SC")
                    DocType = "MUPI";
                if (hdnRegistrationType.Value == "C")
                { 
                    if  (rbtEntityType.SelectedValue == "SP")
                    {
                        DocType = "MUPC";                     
                    }
                    else if (rbtEntityType.SelectedValue == "PR")
                    {
                        DocType = "MUPC1";                     
                    }
                    else if (rbtEntityType.SelectedValue == "CP")
                    {
                        DocType = "MUPC2";                     
                    }
                }
                if (hdnRegistrationType.Value == "NI"  || hdnRegistrationType.Value == "NSC")
                { 
                    DocType = "MUPNI";
                }
                if (hdnRegistrationType.Value == "NC")
                { 
                    DocType = "MUPNC";
                }
                if (hdnRegistrationType.Value == "LH")
                { 
                    DocType = "MUPLH";
                }
                if (hdnRegistrationType.Value == "LHN" )
                { 
                    DocType = "MUPLHN";
                }

                #region "Retrieving the Documnet List data from database."
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentName", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentType", DocType, SqlDbType.NVarChar, 10, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocSubType", DocSubtype, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                ds = objDAL.GetDataSet("Doc_LookUp_ListData_IPM", parameters.ToArray());
                #endregion
                //added by Hariom 29-04-2023
                if (hdnRegistrationType.Value == "NC")
                {
                    ds.Tables[0].Rows.RemoveAt(10);
                    ds.Tables[0].Rows.RemoveAt(9);
                }
                else if (hdnRegistrationType.Value == "NI")
                {
                    ds.Tables[0].Rows.RemoveAt(8);
                    ds.Tables[0].Rows.RemoveAt(7);
                }
                //end

                ds.Tables[0].AcceptChanges();
                ViewState["DocLookUp"] = ds.Tables[0];

                grdDocumentsPreApproval.DataSource = ds;
                grdDocumentsPreApproval.DataBind();
            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(objGeneralFunction.ReplaceASC(ex.Message), this.Page);
            }
            finally
            {
                #region "Finally section."
                ds = null;
                #endregion
            }
        }

        #endregion

        #region This Function Not in Use
        protected void ChangeDocVal(DataTable DT, string Ids)
        {
            string[] Para = Ids.Split(',');
            for (int i = 0; i < Para.Length; i++)
            {
                DataRow[] DR = DT.Select("DocumentLookupId=" + Para[i]);
                if (DR.Length > 0)
                {
                    DR[0]["Iscompulsary"] = 0;
                }
            }
        }
        #endregion

        /// <summary>
        /// For EnaDisDIV
        /// In this Function we Firstly All Division are visibly False and Item of Check Box(cbxRollType) are clear.
        /// get Registration Type by using Store Procedure(MemberRoleType_LookUp_Populate_IPM) and asign value to Check Box(cbxRollType).
        /// And asper the Registration Type related Divison and Control are visibly true by using Function(DivEnaDis).
        /// Commented By Rohit
        /// </summary>
        #region Enable Disable Division
        protected void EnaDisDIV()
        {
            cbxRollType.Items.Clear();
            divCompanyNote.Visible = false;
            divAadhar.Visible = false;
            divBirthInfo.Visible = false;
            divBirthPlace.Visible = false;
            divNatinality.Visible = false;
            //divIndividualOverSeasInfo.Visible = true;
            divCompany.Visible = false;
            divDesignation.Visible = false;
            divFather.Visible = false;
            divGender.Visible = false;
            divlang.Visible = false;
            //divIPINumber.Visible = false;
            divchannel.Visible = false;
            divSocial.Visible = false;
            divPanNo.Visible = false;
            divTRCNo.Visible = false;
            divFForm.Visible = false;
            divDNationality.Visible = false;
            divPincode.Visible = false;
            divZipcode.Visible = false; 
            divPerPincode.Visible = false;
            divPerZip.Visible = false;
            divswift.Visible = false;
            divMotherTounge.Visible = false; 

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet DS = new DataSet();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnRegistrationType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
            objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRollType, out DS, true);
            parameters.Clear();
            if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "NSC" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
            {
                DataRow DR = DS.Tables[0].Select("RegistrationType='" + hdnRegistrationType.Value.ToString() + "'and MemberRoleType like 'publisher%'").FirstOrDefault();
                if (DR != null)
                {
                    if (cbxRollType.Items.Count > 0)
                    {
                        System.Web.UI.WebControls.ListItem item = cbxRollType.Items.FindByValue(DR["MemberRoleTypeId"].ToString());
                        if (item != null)
                            item.Enabled = false;
                    }
                }
                if (DS != null)
                { 
                    ViewState["DtRollType"] = DS.Tables[0];
                }
                spnAlias.InnerText = "Alias";
                divAadhar.Visible = true;
                divBirthInfo.Visible = true;
                divBirthPlace.Visible = true;
                divNatinality.Visible = true;
                SpAddressYesNo.InnerText = "Is Permanent address same as the Present address ?";
                SpPrmnAddressType.InnerText = "Present Address";
                spnUpload.InnerText = "Upload Photo*";
                //divIndividualOverSeasInfo.Visible = true;
                // divGST.Visible = true;
            }
            else
            {
                divDesignation.Visible = true;
                divCompany.Visible = true;
                //SectionTitle.InnerText = "Company Info";
                divCompanyNote.Visible = true;
                SpAddressYesNo.InnerText = "Is Permanent address same as the Registerd address ?";
                txtFname.Attributes.Add("placeholder", "Enter Representative First Name");
                txtLname.Attributes.Add("placeholder", "Enter Representative Last Name");
                //parameters = new List<SqlParameter>();
                //parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnRegistrationType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                //objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRollType, out DS);
                for (int i = 0; i <= cbxRollType.Items.Count - 1; i++)
                {
                    if (cbxRollType.Items[i].Text == "Publisher")
                    {
                        cbxRollType.Items[i].Selected = true;
                    }
                }
                spnAlias.InnerText = "Alias/ Trader Name*";
                SpPrmnAddressType.InnerText = "Registered Address";
                spnUpload.InnerText = "Upload Representative Photo/Company Logo*";
            }
            DivEnaDis();
        }
        #endregion

        /// <summary>
        /// For EnaDisDIVd
        /// In this Function we Firstly All Division are visibly False and Item of Check Box(cbxRollTyped) are clear.
        /// get Registration Type by using Store Procedure(MemberRoleType_LookUp_Populate_IPM) and asign value to Check Box(cbxRollTyped).
        /// And asper the Registration Type related Divison and Control are visibly true by using Function(DivEnaDis).
        /// Commented By Rohit
        /// </summary>
        #region Enable Disable Division For Deceased Member
        protected void EnaDisDIVd()
        {
            cbxRollTyped.Items.Clear();
            divCompanyNote.Visible = false;
            divAadhar.Visible = false;
            divBirthInfo.Visible = false;
            divBirthPlace.Visible = false;
            divNatinality.Visible = false;
            //divIndividualOverSeasInfo.Visible = true;
            divCompany.Visible = false;
            divDesignation.Visible = false;
            divFather.Visible = false;
            divGender.Visible = false;
            divlang.Visible = false;
            divIPINumber.Visible = true; //uncommnted by Hariom 20-02-23
            divchannel.Visible = false;
            divSocial.Visible = false;
            divPanNo.Visible = false;
            divTRCNo.Visible = false;
            divFForm.Visible = false;
            divDNationality.Visible = false;
            divPincode.Visible = false;
            divZipcode.Visible = false;
            divPerPincode.Visible = false;
            divPerZip.Visible = false;
            divswift.Visible = false;
            divMotherTounge.Visible = false;

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet DS = new DataSet();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnRegistrationType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
            objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRollTyped, out DS, true);
            parameters.Clear();
            if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "NSC" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
            {
                DataRow DR = DS.Tables[0].Select("RegistrationType='" + hdnRegistrationType.Value.ToString() + "'and MemberRoleType like 'publisher%'").FirstOrDefault();
                if (DR != null)
                {
                    if (cbxRollTyped.Items.Count > 0)
                    {
                        ListItem item = cbxRollTyped.Items.FindByValue(DR["MemberRoleTypeId"].ToString());
                        if (item != null)
                            item.Enabled = false;
                    }
                }
                if (DS != null)
                {
                    ViewState["DtRollType"] = DS.Tables[0];
                }

                spnAlias.InnerText = "Alias";
                divAadhar.Visible = true;
                divBirthInfo.Visible = true;
                divBirthPlace.Visible = true;
                divNatinality.Visible = true;
                SpAddressYesNo.InnerText = "Is Permanent address same as the Present address ?";
                SpPrmnAddressType.InnerText = "Present Address";
                spnUpload.InnerText = "Upload Photo*";
                //divIndividualOverSeasInfo.Visible = true;
                // divGST.Visible = true;
            }
            else
            {
                divDesignation.Visible = true;
                divCompany.Visible = true;
                //SectionTitle.InnerText = "Company Info";
                divCompanyNote.Visible = true;
                SpAddressYesNo.InnerText = "Is Permanent address same as the Registerd address ?";
                txtFname.Attributes.Add("placeholder", "Enter Representative First Name");
                txtLname.Attributes.Add("placeholder", "Enter Representative Last Name");
                //parameters = new List<SqlParameter>();
                //parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", hdnRegistrationType.Value, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                //objDAL.FillListControl("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray(), cbxRollType, out DS);
                for (int i = 0; i <= cbxRollTyped.Items.Count - 1; i++)
                {
                    if (cbxRollTyped.Items[i].Text == "Publisher")
                    {
                        cbxRollTyped.Items[i].Selected = true;
                    }
                }
                spnAlias.InnerText = "Alias/ Trader Name*";
                SpPrmnAddressType.InnerText = "Registered Address";
                spnUpload.InnerText = "Upload Representative Photo/Company Logo*";
            }
            DivEnaDis();
        }
        #endregion

        /// <summary>
        /// for DivEnaDis
        /// In this Function Enable Disable Division asper Registration Type(hdnRegistrationType) Value
        /// Commented By Rohit
        /// </summary>
        #region Enable Disable Division asper Registration Type
        private void DivEnaDis()
        {
            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN")
            {
                divFather.Visible = true;
                divGender.Visible = true;
                //divlang.Visible = true;
                divIPINumber.Visible = true;
                divMotherTounge.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "NC" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC")
            {
                divchannel.Visible = true;
            }
            divAadhar.Visible = false;
            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN")
            {
                divAadhar.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC")
            {
                divSocial.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN")
            {
                divPanNo.Visible = true;
                divPincode.Visible = true;
                divPerPincode.Visible = true;  
            }
            if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC" || hdnRegistrationType.Value.ToString() == "NSC")
            {
                divTRCNo.Visible = true;
                divFForm.Visible = true;
                divZipcode.Visible = true;
                divPerZip.Visible = true;  
            }
            if (hdnRegistrationType.Value.ToString() == "NI")
            {
                divDNationality.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN")
            {
                divPincode.Visible = true;
                divPerPincode.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC" || hdnRegistrationType.Value.ToString() == "NSC")
            {
                divswift.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "NC")   
            {
                divDtofEst.Visible = true;
            }
            if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC" )
            {
                divPincode.Visible = false;
                //divZipcode.Visible = false;
                divgeopre.Visible = false;
                divPerPincode.Visible = false;                
                divGeoPer.Visible = false;  
                divPreCountry.Visible = true;
                divPreState.Visible = true;
                divPreCity.Visible = true;
                divPerCountry.Visible = true;
                divPerState.Visible = true;
                divPerCity.Visible = true; 
            }
            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "C" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN")
            {
                divPincode.Visible = true;
                //divZipcode.Visible = true;
                divgeopre.Visible = true;
                divPerPincode.Visible = true;
                divGeoPer.Visible = true;
                divPreCountry.Visible = false;
                divPreState.Visible = false;
                divPreCity.Visible = false;
                divPerCountry.Visible = false;
                divPerState.Visible = false;
                divPerCity.Visible = false;
            }
        }
        #endregion

        /// <summary>
        /// For rbtGstApl_SelectedIndexChanged
        /// In This Event If member is Company then GST Divison is visible
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region GST Divison visible
        protected void rbtGstApl_SelectedIndexChanged(object sender, EventArgs e)
        {
            divGst.Visible = false;
            if (rbtGstApl.SelectedValue.ToUpper() == "AP")
            {
                divGst.Visible = true;
            }
            else
            {
                divGst.Visible = false;
            }
        }
        #endregion

        /// <summary>
        /// For getRefAccountId
        /// Get Account ID by using store procedure(App_Accounts_List_RefAccountId) porviding AccountCode in this function 
        /// Commented By Rohit
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <returns>RefAccountId</returns>
        #region Get Account ID
        private string getRefAccountId(string AccountCode)
        {
            string RefAccountId="";
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RefAccountCode", AccountCode, SqlDbType.NVarChar, 0, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_RefAccountId", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];
                RefAccountId = DR["AccountId"].ToString();
            }
            return RefAccountId;
        }

        #endregion

        /// <summary>
        /// For DisplayOperation
        /// In this Function Get Account Information using Store Procedure(App_Accounts_List_IPM) and asgin value to related Control.
        /// in this function we use User Control(ucDropDown) and asign value to DropDown(DropDownLang,ddlMotherLang,ddlCountry,DDLPerCountry,
        /// ddlPincode_PR,ddlGeo_PR,ddlOverseasSocietyName). If Registration Type is C or NC then RadionButton(rbtGstApl) visible. Panel(pnlPDPermanentAddress)is visible asper radio Button(rbtnlPDCheckAddress)
        /// selected Value.Display member Image by using Function(DisplayMemberImage). Get Member Bank Infromation Using Function(PopulateBankInfo)
        /// Get Member Contact Infromation Using Store Procedure(App_Accounts_Address_Contact_List_IPM)
        /// Commented By Rohit
        /// </summary>
        #region Get Account Information And Asign Value TO related Controls
        protected void DisplayOperation()
        {
            try
            {
                txtIFSC.Text = "";
                DSIT_DataLayer DAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                DataSet DS = DAL.GetDataSet("App_Accounts_List_IPM", parameters.ToArray());
                if (DS.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = DS.Tables[0].Rows[0];
                    hdnAccountStatus.Value = DR["RecordStatus"].ToString();
                    hdnApplicationStatus.Value = DR["ApplicationStatus"].ToString();
                    hdnRegistrationType.Value = DR["AccountRegType"].ToString();
                    hdnRecordId.Value = DR["AccountId"].ToString();
                    if (DR["RegistrationDate"].ToString() != string.Empty)
                        hdnRegistrationDate.Value = DR["RegistrationDate"].ToString();
                    hdnAccountCode.Value = DR["AccountCode"].ToString();
                    //rbtRegistrationType.SelectedValue = DR["AccountRegType"].ToString();
                    //rbtRegistrationType_SelectedIndexChanged(null, null);
                    string AccountName = DR["AccountName"].ToString();
                    hdnBookId.Value = DR["BookId"].ToString();
                    if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "NSC")
                    {
                        string[] strAccountName = AccountName.Split(' ');
                        if (strAccountName.Length > 0)
                        {
                            if (strAccountName.Length == 1)
                                txtFname.Text = strAccountName[0];
                            if (strAccountName.Length == 2)
                            {
                                //changed by deven father nme'
                                txtFname.Text = strAccountName[0];
                                txtLname.Text = strAccountName[1];
                            }
                            if (strAccountName.Length == 3)
                            {
                                txtFname.Text = strAccountName[0] + " " + strAccountName[1];
                                //txtFatherName.Text = strAccountName[1];
                                txtLname.Text = strAccountName[2];
                            }
                            if (strAccountName.Length > 3)
                            {
                                txtFname.Text = strAccountName[0] + " " + strAccountName[1];
                                //txtFatherName.Text = strAccountName[1];
                                txtLname.Text = strAccountName[2];
                            }
                        }
                    }
                    else
                    {
                        //string[] strAccountName = AccountName.Split(' ');
                        //if (strAccountName.Length == 2)
                        //{
                        //    //changed by deven father nme'
                        //    txtFname.Text = strAccountName[0];
                        //    txtLname.Text = strAccountName[1];
                        //}
                        txtcompanyName.Text = DR["AccountName"].ToString();
                        if (DR["FirstName"].ToString() != "")
                        {
                            txtFname.Text = DR["FirstName"].ToString();
                        }
                        if (DR["LastName"].ToString() != "")
                        {
                            txtLname.Text = DR["LastName"].ToString();
                        }
                        //rbtRegistrationType.Enabled = false;
                    }
                    txtAccountAlias.Text = DR["AccountAlias"].ToString();
                    txtEmail.Text = DR["AccountEmail"].ToString();
                    txtAltEmail.Text = DR["Alt_EmailId"].ToString();
                    txtFatherName.Text = DR["Fathername"].ToString();
                    rbtnGender.SelectedValue = DR["Gender"].ToString();
                    DropDownLang.SelectedValue = DR["PreferredLanguage"].ToString();
                    ddlMotherLang.SelectDropDown(DR["LanguageId"].ToString(), DR["LanguageName"].ToString());
                    txtInternalName.Text = DR["InternalidentificationName"].ToString();
                    txtIPINumber.Text = DR["IPINumber"].ToString();
                    //txtIPI.Text = DR["IPINumber"].ToString(); // added by Hariom 20-02-23
                    txtchannel.InnerText = DR["ChanlDesc"].ToString();
                    txtSocialNo.Text = DR["SocialSecurityNo"].ToString();
                    txtSocialNo.Text = DR["SocialSecurityNo"].ToString();
                    txtTrcNo.Text = DR["TRCNo"].ToString();
                    txtfForm.Text = DR["TenFForm"].ToString();
                    RBLNationality.SelectedValue = DR["DualNationality"].ToString();
                    ddlCountry.SelectDropDown(DR["PreCountryId"].ToString(), DR["PreCountryName"].ToString());
                    DDLPerCountry.SelectDropDown(DR["PerCountryId"].ToString(), DR["PerCountryname"].ToString());
                    if (DR["TeritoryAppFor"].ToString() != "")
                    {
                        DDLTerAppFor.SelectedValue = DR["TeritoryAppFor"].ToString();
                    }
                    
                    #region "toggling GST"
                    txtDetails1_gst.Text = DR["Detail1"].ToString();
                    if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "NC" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "NSC" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    {
                        if (txtDetails1_gst.Text != string.Empty)
                            rbtGstApl.SelectedValue = "AP";
                        else
                            rbtGstApl.SelectedValue = "NAP";
                    }
                    else
                    {
                        rbtGstApl.SelectedValue = "AP";
                        //rbtGstApl.Enabled = false;
                    }
                    rbtGstApl_SelectedIndexChanged(null, null);
                    #endregion

                    if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    {
                        divLegDetail.Visible = true;
                    }
                    else
                    {
                        divLegDetail.Visible = false;
                    }
                    txtDetails2_Pan.Text = DR["Detail2"].ToString();
                    txtDetails3_Aadh.Text = DR["Detail3"].ToString();
                    rbtnlPDCheckAddress.SelectedValue = "Y";

                    #region "Populating permanent Address"
                    if (DR["AccountAddress"].ToString() != string.Empty)
                    {
                        rbtnlPDCheckAddress.SelectedValue = "N";
                        txtAddress_PM.InnerText = DR["AccountAddress"].ToString();
                        if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                        {
                            ddlGeo_PM.SelectDropDown(DR["GeographicalId"].ToString(), DR["GeographicalName"].ToString());
                            ddlPincode_PM.SelectDropDown(DR["PincodeId"].ToString(), DR["Pincode"].ToString());
                        }
                        if (hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "NC" || hdnRegistrationType.Value == "NSC")
                        {
                            txtperZipCode.Text = DR["PerZipCode"].ToString();
                            DDLPerCountry.SelectDropDown(DR["PerCountryId"].ToString(), DR["PerCountryname"].ToString());
                            txtPerCity.Text = DR["PerCity"].ToString();
                            txtPerState.Text = DR["PerState"].ToString();
                        }
                        //Added by Renu on 15/12/2020
                        hdnpincodePM.Value = DR["Pincode"].ToString();
                    }
                    #endregion

                    #region "Populating Present Address"
                    txtAddress_PR.InnerText = DR["AccountAddress_PR"].ToString();
                    if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    {
                        ddlPincode_PR.SelectDropDown(DR["PincodeId_PR"].ToString(), DR["Pincode_PR"].ToString());
                        ddlGeo_PR.SelectDropDown(DR["GeographicalId_PR"].ToString(), DR["GeographicalName_PR"].ToString());
                    }
                    if (hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "NC" || hdnRegistrationType.Value == "NSC")
                    {
                        txtzipcode.Text = DR["PreZipCode"].ToString();
                        ddlCountry.SelectDropDown(DR["PreCountryId"].ToString(), DR["PreCountryname"].ToString());
                        txtCity.Text = DR["PreCity"].ToString();
                        txtState.Text = DR["PreState"].ToString();
                    }
                    //Added by Renu on 15/12/2020
                    hdnpincode.Value = DR["Pincode_PR"].ToString();
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
                    txtDOB.Text = objGeneralFunction.FormatNullableDate(DR["DOB"].ToString());
                    if (hdnRegistrationType.Value == "I")
                    {
                        //txtDOB.Text = objGeneralFunction.FormatNullableDate(DR["DOB"].ToString());
                    }
                    else
                        txtDateofEstablishment.Text = objGeneralFunction.FormatNullableDate(DR["DOB"].ToString());

                    txtPOB.Text = DR["PlaceOfBirth"].ToString();
                    txtNationality.Text = DR["Nationality"].ToString() == "" ? "INDIAN" : DR["Nationality"].ToString();

                    #region "Populating Bank Details"
                    PopulateBankInfo(DS.Tables[0]);
                    #endregion

                    if (DR["RollTypeIds"].ToString() != string.Empty)
                    {
                        hdnRoleTypeIds.Value = DR["RollTypeIds"].ToString();
                        hdnRoleTypes.Value = string.Empty;
                        string[] strRolltype = DR["RollTypeIds"].ToString().Split(',');
                        if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                        {
                            for (int i = 0; i < strRolltype.Length; i++)
                            {
                                for (int j = 0; j < cbxRollTyped.Items.Count; j++)
                                {
                                    if (strRolltype[i] == cbxRollTyped.Items[j].Value.ToString())
                                    {
                                        cbxRollTyped.Items[j].Selected = true;
                                        hdnRoleTypes.Value += cbxRollTyped.Items[j].Attributes["dval"] + "@" + cbxRollTyped.Items[j].Value + ",";
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < strRolltype.Length; i++)
                            {
                                for (int j = 0; j < cbxRollType.Items.Count; j++)
                                {
                                    if (strRolltype[i] == cbxRollType.Items[j].Value.ToString())
                                    {
                                        cbxRollType.Items[j].Selected = true;
                                        hdnRoleTypes.Value += cbxRollType.Items[j].Attributes["dval"] + "@" + cbxRollType.Items[j].Value + ",";
                                    }
                                }
                            }
                        }
                        hdnRoleTypes.Value = hdnRoleTypes.Value.TrimEnd(',');
                    }

                    if (DR["OverseasSocietyName"].ToString() == string.Empty)
                        rbtnlMemberoverseas.SelectedValue = "N";
                    else
                    {
                        //txtOverseasSocietyName.Text = DR["OverseasSocietyName"].ToString();
                        ddlOverseasSocietyName.SelectDropDown(DR["SocietyId"].ToString(), DR["OverseasSocietyName"].ToString());
                        rbtnlMemberoverseas.SelectedValue = "Y";
                    }
                    txtAssociationMember.Text = DR["AssociationName_India"].ToString();
                    rbtnlMemberoverseas_SelectedIndexChanged(null, null);
                    if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    { 
                        cbxRollTyped.Enabled = true;
                    }   
                    else
                    {
                        cbxRollType.Enabled = true;
                    }
                    hdnPaymentRecieptId.Value = DR["PaymentRecieptId"].ToString();
                    if (hdnPaymentRecieptId.Value == "")
                        hdnPaymentRecieptId.Value = "0";
                    //Change by RENU 0n 15/12/2020 on IPRS team request
                    //if (Convert.ToInt64(hdnPaymentRecieptId.Value) > 0)
                    //{
                    //    cbxRollType.Enabled = false;
                    //}
                    if (txtMobile.Text != "" )
                    {
                        txtMobile.ReadOnly = true;
                        txtCountryCode.ReadOnly = true;
                    }
                    if (txtEmail.Text != "")
                    {
                        txtEmail.ReadOnly = true;
                    }
                    if (hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NC")
                    {
                        if (DR["EntityType"].ToString() != string.Empty)
                            rbtEntityType.SelectedValue = DR["EntityType"].ToString();

                        parameters = new List<SqlParameter>();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
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
                                    txtLname.Text = strName[1] + strName[2];
                                }
                                if (strName.Length > 3)
                                {
                                    txtFname.Text = strName[0];
                                    txtLname.Text = strName[1] + " " + strName[2];
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
                    if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    {
                        cbxRollTyped.Enabled = false;
                    }
                    DisplayMemberImage();
                    //txtFatherName.Text = DR["FatherName"].ToString();
                    //txtFname.Text = DR["FirstName"].ToString();
                    //txtLastName.Text = DR["LastName"].ToString();
                    //var item = rbtnGender.Items.Cast<ListItem>().FirstOrDefault(i => i.Text.Equals(DR["Gender"].ToString(), StringComparison.InvariantCultureIgnoreCase));
                    //if (item != null)
                    //    rbtnGender.SelectedValue = item.Value;
                    //ddlLanguage.SelectDropDown(DR["PreferredLanguageId"].ToString(), DR["PreferredLanguage"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(objGeneralFunction.ReplaceASC(ex.Message), Page);
            }
        }
        #endregion

        /// <summary>
        /// For PopulateBankInfo
        /// In this Function Get Member bank Information By using Store Procedure(App_Accounts_List_Bank_IPM) and asign values to 
        /// related Controls. If Data is Null then Button(btnvalidate_Click) is active 
        /// Commented By Rohit
        /// </summary>
        /// <param name="DTAccount"></param>
        #region Get Bank Detail
        protected void PopulateBankInfo(DataTable DTAccount)
        {
            DataTable DT = new DataTable();
            if (DTAccount == null)
            {
                DSIT_DataLayer DAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                DT = DAL.GetDataTable("App_Accounts_List_Bank_IPM", parameters.ToArray());
            }
            else
            { DT = DTAccount; }
            if (DT.Rows.Count > 0)
            {
                #region "Populating Bank Details"
                txtBankName.Text = DT.Rows[0]["BankName"].ToString();
                txtBankName.ReadOnly = true; 
                txtBankAcNo.Text = DT.Rows[0]["BankAcNo"].ToString();                
                txtBankBranchName.Text = DT.Rows[0]["BankBranchName"].ToString();
                txtBankBranchName.ReadOnly = true;
                txtIFSC.Text = DT.Rows[0]["BankIFSCCode"].ToString();
                txtMICR.Text = DT.Rows[0]["MicrCode"].ToString();
                txtMICR.ReadOnly = true;
                if (DT.Rows[0]["Currency_Id"].ToString() != "")
                { 
                    DDLCurrency.SelectDropDown(DT.Rows[0]["Currency_Id"].ToString(), DT.Rows[0]["CurrencyName"].ToString());
                }
                txtswiftcode.Text = DT.Rows[0]["BankSwift"].ToString();
                if (txtIFSC.Text.Trim() != "")
                    btnvalidate_Click(null, null);
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// For wzMain_NextButtonClick
        /// this event for go to next wizard(wzMain) step, here we check Account holder name in Session["AccountName1"] if yes then go forward 
        /// otherwise redirect to login screeen.
        /// if ActiveStepIndex=0 (Personal Information) then get inforamtion hidden field and assign value to related controls
        ///if ActiveStepIndex=1 (Bank Details) then get inforamtion hidden field and assign value to related controls
        ///if ActiveStepIndex=4 (Document Upload) then get inforamtion by viewstate(ViewState["DocLookUp"]) if viewstate is null then dispaly the massage
        ///Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Next Button Click
        protected void wzMain_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            string strCheck = objGeneralFunction.CheckScripValidation(this, typeof(TextBox));
            Session["AccountName1"] = ddlAccountHoldername.SelectedValue.ToString();
            if (strCheck != "")
            {
                e.Cancel = true;
                objGeneralFunction.BootBoxAlert(strCheck, Page);
                return;
            }
            strCheck = objGeneralFunction.CheckScripValidation(this, typeof(HtmlTextArea));
            if (strCheck != "")
            {
                e.Cancel = true;
                objGeneralFunction.BootBoxAlert(strCheck, Page);
                return;
            }
            if (wzMain.ActiveStepIndex == 0)
            {
                string strGSTValid = string.Empty;
                #region "Saving Basic Details"
                // for GST auto not applicable - Renu
                if (txtDetails1_gst.Text.ToString() == "")
                {
                    rbtGstApl.SelectedValue = "NAP";
                    divGst.Visible = false;
                }
                // for GST auto not applicable 
                if (rbtEntityType.Visible && (rbtEntityType.SelectedValue == "" || rbtEntityType.SelectedValue == string.Empty))
                {
                    e.Cancel = true;
                    objGeneralFunction.BootBoxAlert("SELECT ENTITY TYPE TO PROCEED.", Page);
                    return;
                }
                //if (rbtRegistrationType.SelectedValue == "C" && txtcompanyName.Text.Length > 45)
                //{
                //    e.Cancel = true;
                //    objGeneralFunction.BootBoxAlert("ENTER A SHORT NAME FOR COMPANY", Page);
                //    return;
                //}
                string CityId_pm = "0";
                string CityId_PR = "0"; string Mobile = ""; string DOB = ""; string DOE = "";
                //if (txtMname.Text != string.Empty)
                //    hdnAccountName.Value = txtFname.Text.Trim() + " " + txtMname.Text.Trim() + " " + txtLname.Text.Trim();
                //else
                //    hdnAccountName.Value = txtFname.Text.Trim() + " " + txtLname.Text.Trim();
                hdnAccountName.Value = txtFname.Text.Trim() + " " + txtLname.Text.Trim();
                hdnRoleTypeIds.Value = string.Empty;
                hdnRoleTypes.Value = string.Empty;
                DataTable dtRoleType = (DataTable)ViewState["DtRollType"];
                string rType = string.Empty;
                DataRow DR;
                if (hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                { 
                    if (cbxRollTyped.Items.Count > 0)
                    {
                        for (int i = 0; i < cbxRollTyped.Items.Count; i++)
                        {
                            if (cbxRollTyped.Items[i].Selected == true)
                            {
                                hdnRoleTypeIds.Value += cbxRollTyped.Items[i].Value + ",";

                                if (dtRoleType != null)
                                {
                                    DR = dtRoleType.Select("MemberRoleTypeId = " + cbxRollTyped.Items[i].Value).FirstOrDefault();
                                    hdnRoleTypes.Value += DR["RoleType"].ToString() + "@" + cbxRollTyped.Items[i].Value + ",";
                                }
                            }
                        }
                        hdnRoleTypeIds.Value = hdnRoleTypeIds.Value.TrimEnd(',');
                        hdnRoleTypes.Value = hdnRoleTypes.Value.TrimEnd(',');
                        hdnRoleTypeIds.Value = objGeneralFunction.SplitCheckListBoxID(cbxRollTyped);
                        if (hdnRoleTypeIds.Value == string.Empty)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Select Role Type", Page);
                            return;
                        }
                    }
            }
            else
            {
                if (cbxRollType.Items.Count > 0)
                {
                    for (int i = 0; i < cbxRollType.Items.Count; i++)
                    {
                        if (cbxRollType.Items[i].Selected == true)
                        {
                            hdnRoleTypeIds.Value += cbxRollType.Items[i].Value + ",";

                            if (dtRoleType != null)
                            {
                                DR = dtRoleType.Select("MemberRoleTypeId = " + cbxRollType.Items[i].Value).FirstOrDefault();
                                hdnRoleTypes.Value += DR["RoleType"].ToString() + "@" + cbxRollType.Items[i].Value + ",";
                            }
                        }
                    }
                    hdnRoleTypeIds.Value = hdnRoleTypeIds.Value.TrimEnd(',');
                    hdnRoleTypes.Value = hdnRoleTypes.Value.TrimEnd(',');
                    hdnRoleTypeIds.Value = objGeneralFunction.SplitCheckListBoxID(cbxRollType);
                    if (hdnRoleTypeIds.Value == string.Empty)
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Select Role Type", Page);
                        return;
                    }
                }
            }
                //if (ddlLanguage.GetSelectedValue() == "0" && ddlLanguage.GetSelectedValue() == "")
                //{
                //    e.Cancel = true;
                //    objGeneralFunction.BootBoxAlert("Select Language from Dropdown", Page);
                //    return;
                //}
                //Int32 diffdays = Convert.ToInt32(DateTime.Today.Subtract(Convert.ToDateTime(txtDOB.Text)).TotalDays);
                if (hdnRegistrationType.Value =="I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                { 
                    if (txtDOB.Text !="")
                    { 
                        DateTime dob = Convert.ToDateTime(txtDOB.Text);
                        int days = (DateTime.Today -dob).Days;
                        Int32 diffYears = (days / 365);

                        if (diffYears < 10)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Applicant Date of Birth should not be less than 10 Years.", Page);
                            return;
                        }
                    }
                }
                string ddlgeoname = ((TextBox)ddlGeo_PR.FindControl("txtDropDown")).Text;
                string Pincode_PR = ((TextBox)ddlPincode_PR.FindControl("txtDropDown")).Text;
                string PincodeId_PR = ((HiddenField)ddlPincode_PR.FindControl("hdnSelectedValue")).Value;
                string Pincode_PM = ((TextBox)ddlPincode_PM.FindControl("txtDropDown")).Text;
                string PincodeId_PM = ((HiddenField)ddlPincode_PM.FindControl("hdnSelectedValue")).Value;
                string Areacode = ""; string AreaName = "";
                if (ddlPincode_PR.Visible ==true)
                { 
                    if (Pincode_PR == "")
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Select Present Pincode", Page);
                        return;
                    }
                    if (PincodeId_PR == "0" || PincodeId_PR == "" || Pincode_PR.Length == 0)
                    {
                        e.Cancel = true;
                        ddlPincode_PR.SelectDropDown("0", "");
                        objGeneralFunction.BootBoxAlert("Select Area with Pincode from dropdown", Page);
                        return;
                    }
                    objGeneralFunction.ValidatePincode(PincodeId_PR, out Areacode, out AreaName);
                    if (Pincode_PR != Areacode.Trim())
                    {
                        e.Cancel = true;
                        ddlPincode_PR.SelectDropDown("0", "");
                        objGeneralFunction.BootBoxAlert("Select Area with Pincode from dropdown", Page);
                        return;
                    }
                    CityId_PR = ((HiddenField)ddlGeo_PR.FindControl("hdnSelectedValue")).Value;
                    if (CityId_PR == "0" || CityId_PR == "" || objGeneralFunction.IsNumeric(CityId_PR) == false)
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Select Present City", Page);
                        return;
                    }
                }
                if (rbtnlPDCheckAddress.SelectedValue == "N")
                {
                    if (ddlPincode_PM.Visible == true)
                    {
                        if (Pincode_PM == "")
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Select Permanent Pincode", Page);
                            return;
                        }
                        if (PincodeId_PM == "0" || PincodeId_PM == "" || Pincode_PM.Length == 0)
                        {
                            e.Cancel = true;
                            ddlPincode_PM.SelectDropDown("0", "");
                            objGeneralFunction.BootBoxAlert("Select Permanent Area with Pincode from dropdown", Page);
                            return;
                        }
                        Areacode = ""; AreaName = "";
                        objGeneralFunction.ValidatePincode(PincodeId_PM, out Areacode, out AreaName);
                        if (Pincode_PM != Areacode.Trim())
                        {
                            e.Cancel = true;
                            ddlPincode_PM.SelectDropDown("0", "");
                            objGeneralFunction.BootBoxAlert("Select Permanent Area with Pincode from dropdown", Page);
                            return;
                        }
                        CityId_pm = ((HiddenField)ddlGeo_PM.FindControl("hdnSelectedValue")).Value;
                        if (CityId_pm == "0" || CityId_pm == "" || objGeneralFunction.IsNumeric(CityId_pm) == false)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Select Permanent City", Page);
                            return;
                        }
                    }
                }
                else
                {
                    if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                    {
                        CityId_pm = CityId_PR;
                        ddlPincode_PM.SelectDropDown(PincodeId_PR, Pincode_PR);
                        ddlGeo_PM.SelectDropDown(CityId_PR, ddlgeoname);
                        txtAddress_PM.InnerText = txtAddress_PR.InnerText;
                    }
                    else
                    {
                        txtPerCity.Text  = txtCity.Text;
                        txtPerState.Text = txtState.Text; 
                        txtperZipCode.Text = txtzipcode.Text;
                        ddlgeoname = ((TextBox)ddlCountry.FindControl("txtDropDown")).Text;
                        CityId_PR = ((HiddenField)ddlCountry.FindControl("hdnSelectedValue")).Value;
                        DDLPerCountry.SelectDropDown(CityId_PR, ddlgeoname);
                        txtAddress_PM.InnerText = txtAddress_PR.InnerText;
                    }
                }
                string strfilename = "";
                int photouploaded = FileExist("MPU_" + hdnRecordId.Value + "_", Server.MapPath("~/MemberPhoto/"), out strfilename);
                if (photouploaded == 0)
                {
                    e.Cancel = true;
                    objGeneralFunction.BootBoxAlert("Please upload Photo", Page);
                    return;
                }
                if (txtCountryCode.Text.Trim() != string.Empty)
                    Mobile = txtCountryCode.Text + "-" + txtMobile.Text.ToString();
                else
                    Mobile = txtMobile.Text.ToString();

                if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN" || hdnRegistrationType.Value == "SC" || hdnRegistrationType.Value == "NSC")
                {
                    if (txtDOB.Text != string.Empty)
                    {
                        DOB = objGeneralFunction.FormatDate(txtDOB.Text, "yyyy-MM-dd", Page);
                        if (Convert.ToDateTime(DOB) > DateTime.Today)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("ENTER VALID DATE OF BIRTH.", Page);
                            return;
                        }
                    }
                }
                else
                {
                    if (txtDateofEstablishment.Text != string.Empty)
                    {
                        DOB = objGeneralFunction.FormatDate(txtDateofEstablishment.Text, "yyyy-MM-dd", Page);
                        if (Convert.ToDateTime(DOB) >= DateTime.Today)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("ENTER VALID DATE OF ESTABLISHMENT.", Page);
                            return;
                        }
                    }
                }
                if (txtEmail.Text.ToString().Trim() == txtAltEmail.Text.ToString().Trim())
                {
                    e.Cancel = true;
                    objGeneralFunction.BootBoxAlert("Email Id and Alternate Email Id cannot be same.", Page);
                    return;
                }
                if (divGst.Visible == true)
                {
                    strGSTValid = txtDetails1_gst.Text.ToString().ToUpper();
                    strGSTValid = strGSTValid.Remove(strGSTValid.Length - 3);
                    strGSTValid = strGSTValid.Substring(2);
                    if (strGSTValid != txtDetails2_Pan.Text.ToUpper())
                    {
                        e.Cancel = true;
                        objGeneralFunction.BootBoxAlert("Enter GST No. in correct format, with request to Pan No.", Page);
                        return;
                    }
                }
                else
                {
                    txtDetails1_gst.Text = string.Empty;
                }
                    if (rbtnlMemberoverseas.SelectedValue == "Y")
                    {
                        if (BooSocietyFound() == false)
                        {
                            e.Cancel = true;
                            objGeneralFunction.BootBoxAlert("Please select overseas society name from drop down list", Page);
                            return;
                        }
                    }

                #region "Saving BAsic Infor"
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                Int64 RecordId = 0;
                //string ReturnMessage = "";
                RecordId = SaveApp_Accounts(objDAL, hdnAccountName.Value, CityId_pm, Mobile, DOB, hdnRoleTypeIds.Value, CityId_PR, strfilename);
                if (RecordId == 0)
                {
                    objGeneralFunction.BootBoxAlert(MessageReturn.ToUpper(), Page);
                    e.Cancel = true;
                    return;
                }
                else
                {
                    divLegDetail.Visible = false;
                }
                hdnRecordId.Value = RecordId.ToString();
                #endregion

                #region "Account Holder Name"
                if (hdnRegistrationType.Value == "C")
                {
                    ddlAccountHoldername.Items.Insert(0,  txtcompanyName.Text.ToUpper());
                    ddlAccountHoldername.Items.Insert(1,  txtFname.Text + " " + txtLname.Text.ToUpper());
                }
                else
                {
                    //ddlAccountHoldername.Text = hdnAccountName.Value;
                    ddlAccountHoldername.Items.Insert(0, hdnAccountName.Value);
                }
                #endregion

                #region "IF Company Saving the Proprietor Info"
                if ((hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NC") && Convert.ToInt64(hdnRecordId.Value) > 0)
                {
                    Int64 RecordId_AddCt = SaveAccountAddress_Contact(hdnAccountName.Value, Mobile);
                }
                #endregion
                #endregion
            }
            //deven change
            if (wzMain.ActiveStepIndex == 1)
            {
                #region "Saving Bank Infor"
                if (txtBankName.Text == string.Empty)
                {
                    objGeneralFunction.BootBoxAlert("Enter Bank Name", Page);
                    e.Cancel = true;
                    return;
                }
                if (hdnRegistrationType.Value.ToString() != "NI" && hdnRegistrationType.Value.ToString() != "NC")
                {
                    if (txtBankAcNo.Text.Trim() == string.Empty || txtIFSC.Text.Trim() == string.Empty || txtBankBranchName.Text.Trim() == string.Empty)
                    {
                        objGeneralFunction.BootBoxAlert("Enter Bank Details ", Page);
                        e.Cancel = true;
                        return;
                    }
                    if (hdnIFSC_Val.Value == "")
                    {
                        objGeneralFunction.BootBoxAlert("Validate IFSC Code.", Page);
                        e.Cancel = true;
                        return;
                    }
                }
                if (hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "NC")
                {
                    if (txtBankAcNo.Text.Trim() == string.Empty ||  txtBankBranchName.Text.Trim() == string.Empty)
                    {
                        objGeneralFunction.BootBoxAlert("Enter Bank Details ", Page);
                        e.Cancel = true;
                        return;
                    }
                }
                Int64 RecordId_Bank = SaveBankIfo(Convert.ToInt64(hdnRecordId.Value));
                if (RecordId_Bank == 0)
                {
                    objGeneralFunction.BootBoxAlert("Failed To Save Bank Details. Try Again Or Contact Adminstrator !!", Page);
                    e.Cancel = true;
                    return;
                }
                #endregion
            }

            #region code not in use
            //if (wzMain.ActiveStepIndex == 3)
            //{
            //    decimal Share=0;
            //    GridView gr = (GridView)ucNomineeDetails.FindControl("gvWork");
            //    if (gr != null)
            //    {                    
            //        for (int i = 0; i < gr.Rows.Count ; i++)
            //        {                            
            //            Share = Share + Convert.ToDecimal(gr.Rows[i].Cells[12].Text.ToString());
            //        }                                        
            //    }
            //    if (Share > 100 || Share < 100)
            //    {
            //        objGeneralFunction.BootBoxAlert("Nominee total share should be 100%", Page);
            //        e.Cancel = true;
            //        return;
            //    }
            //}
            #endregion

            if (wzMain.ActiveStepIndex == 4)
            {
                DataTable DT = (DataTable)ViewState["DocLookUp"];
                if (DT != null)
                {
                    object result;
                    result = DT.Compute("SUM(Uploaded)", "IsCompulsary=0");
                    DataRow[] DR = DT.Select("IsCompulsary=0");
                    if (Convert.ToDouble(result) < DR.Length)
                    {
                        objGeneralFunction.BootBoxAlert("Please Upload the List of Douments", Page);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// For SaveApp_Accounts
        /// here we save and update personal information in to Table(App_Accounts) using store procedure(App_Accounts_Manage_IPM)
        /// if hdnRecordId value is 0 then record insert otherwise reocred updated.
        /// Commented By Rohit
        /// </summary>
        /// <param name="objDAL"></param>
        /// <param name="AccountName"></param>
        /// <param name="CityId_pm"></param>
        /// <param name="Mobile"></param>
        /// <param name="DOB"></param>
        /// <param name="RollTypeIds"></param>
        /// <param name="CityId_PR"></param>
        /// <param name="strfilename"></param>
        /// <returns>RecordId</returns>
        #region Save Personal Information
        protected Int64 SaveApp_Accounts(DSIT_DataLayer objDAL, string AccountName, string CityId_pm, string Mobile, string DOB, string RollTypeIds, string CityId_PR, string strfilename)
        {
            Int64 RecordId = 0;
            Int64 LanguageId = 0;
            string  CountryId = "";
            string PerCountryId= "";
            string ReturnMessage = string.Empty; string strRegDate = string.Empty;
            string ddlgeoname_PM = ((TextBox)ddlGeo_PM.FindControl("txtDropDown")).Text;
            string ddlgeoname_PR = ((TextBox)ddlGeo_PR.FindControl("txtDropDown")).Text;
            string Pincode_PM = ((TextBox)ddlPincode_PM.FindControl("txtDropDown")).Text;
            string PincodeId_PM = ((HiddenField)ddlPincode_PM.FindControl("hdnSelectedValue")).Value;
            string Pincode_PR = ((TextBox)ddlPincode_PR.FindControl("txtDropDown")).Text;
            string PincodeId_PR = ((HiddenField)ddlPincode_PR.FindControl("hdnSelectedValue")).Value;
            string SocietyName = ((TextBox)ddlOverseasSocietyName.FindControl("txtDropDown")).Text;
            string SocietyId = ((HiddenField)ddlOverseasSocietyName.FindControl("hdnSelectedValue")).Value;
            if (PincodeId_PM == "")
            {
                PincodeId_PM = "0";
            }
            if (PincodeId_PR == "")
            {
                PincodeId_PR = "0";
            }
            string Language = ((TextBox)ddlMotherLang.FindControl("txtDropDown")).Text;
            if (hdnRegistrationType.Value=="I" || hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
            { 
                if (Language.Trim() !="" )
                {
                    LanguageId = Convert.ToInt64(((HiddenField)ddlMotherLang.FindControl("hdnSelectedValue")).Value);
                }
            }
            string Country="";
            string PerCountry = "";
            if (hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "NC")
            { 
                Country = ((TextBox)ddlCountry.FindControl("txtDropDown")).Text;
                if (Country.Trim()  != "")
                {
                     //CountryId = Convert.ToInt64(((HiddenField)ddlCountry.FindControl("hdnSelectedValue")).Value);
                }            
            }
            if (hdnRegistrationType.Value == "NI" || hdnRegistrationType.Value == "NC")
            {
                PerCountry = ((TextBox)DDLPerCountry.FindControl("txtDropDown")).Text;
                if (PerCountry.Trim() != "")
                {
                    //PerCountryId = Convert.ToInt64(((HiddenField)DDLPerCountry.FindControl("hdnSelectedValue")).Value);
                }
            }
            try
            {
                //if (txtDateofEstablishment.Text != string.Empty)
                //{
                //    strRegDate = objGeneralFunction.FormatDate(txtDateofEstablishment.Text, "yyyy-MM-dd", Page);
                //}
                if (rbtnlMemberoverseas.SelectedValue.ToUpper() == "N")
                {
                    //txtOverseasSocietyName.Text = string.Empty;
                    //ddlOverseasSocietyName.ToString() = "";  
                }
                if (rbtGstApl.SelectedValue.ToUpper() == "N")
                {
                    txtDetails1_gst.Text = string.Empty;
                }
                //if (txtAccountAlias.Text.Trim() == string.Empty)
                //{
                //    if(rbtRegistrationType.SelectedValue=="I")
                //        txtAccountAlias.Text = AccountName;
                //    else
                //        txtAccountAlias.Text = txtcompanyName.Text;
                //}
                if (txtAccountAlias.Text.Contains(","))
                {
                    var result = string.Join(",", txtAccountAlias.Text.Split(',').Select(s => s.Trim()).ToArray());
                    txtAccountAlias.Text = result;
                }
                if (txtAccountAlias.Text.Contains(";"))
                {
                    var result = string.Join(",", txtAccountAlias.Text.Split(';').Select(s => s.Trim()).ToArray());
                    txtAccountAlias.Text = result;
                }
                // Added by RENU on 14/12/2020               
                //string p_PM = ddlPincode_PM.GetSelectedValue();
                //string p_PR = ddlPincode_PR.GetSelectedValue();
                //if ((ddlPincode_PR.GetSelectedText() != hdnpincode.Value.ToString()) && (p_PR == "0"))
                //{
                //    objGeneralFunction.BootBoxAlert("Please select pincode and city from dropdown list.", Page);
                //    return RecordId;
                //}
                //if ((rbtnlPDCheckAddress.SelectedValue == "N") && (ddlPincode_PM.GetSelectedText() != hdnpincodePM.Value.ToString()) && (p_PM == "0"))
                //{
                //    objGeneralFunction.BootBoxAlert("Please select pincode and city from dropdown list of permanent address.", Page);
                //    return RecordId;
                //}
                //
                var parameters = new List<SqlParameter>();
                //renu 04/11/2020
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", rbtRegistrationType.SelectedValue, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                //Update by RENU on 14/12/2020
                if (hdnRegistrationType.Value == "I" || hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "LH" || hdnRegistrationType.Value == "LHN")
                {               
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", hdnRegistrationType.Value.ToString(), SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", CityId_pm, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId_PR", CityId_PR, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                    objDAL.ExecuteSP("App_Accounts_BookID_Validate", parameters.ToArray(), out ReturnMessage, out RecordId);
                    if (RecordId == 0)
                    {                    
                       objGeneralFunction.BootBoxAlert("Please select pincode and city.", Page);
                       return 0;
                    }
                }
                    RecordId = 0;
                    ReturnMessage = "";
                    parameters = new List<SqlParameter>();
                    parameters.Clear(); 
                    parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", hdnAccountCode.Value, SqlDbType.NVarChar, 20, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountType", "C", SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    //  parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", rbtRegistrationType.SelectedValue, SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    //Update by RENU on 14/12/2020
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountRegType", hdnRegistrationType.Value.ToString(), SqlDbType.NVarChar, 5, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@GroupId", 39, SqlDbType.BigInt, 0, ParameterDirection.Input));//hardcoded
                    // if (rbtRegistrationType.SelectedValue == "I")
                    if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "LH" || hdnRegistrationType.Value.ToString() == "LHN" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC")
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", AccountName.Trim(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    else
                        parameters.Add(objGeneralFunction.GetSqlParameter("@AccountName", txtcompanyName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));

                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAlias", txtAccountAlias.Text.ToString(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail1", txtDetails1_gst.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));//gst
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail2", txtDetails2_Pan.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));//panno
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail3", txtDetails3_Aadh.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail4", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail5", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail6", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail7", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail8", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail9", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail10", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail11", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Detail12", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress", txtAddress_PM.InnerText.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    if (CityId_pm=="" || CityId_pm == "0")
                    { 
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", CityId_pm, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    if (PincodeId_PM == "" || PincodeId_PM == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", 0, SqlDbType.BigInt, 25, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId", Convert.ToInt64(PincodeId_PM), SqlDbType.BigInt, 25, ParameterDirection.Input));
                    }
                    if (Pincode_PM == "" || Pincode_PM == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", "", SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", Pincode_PM, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreZipCode", txtzipcode.Text, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountPhone", txtTelephone.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile", Mobile, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountMobile_Alt", txtAlternateMobile.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@DOB", DOB, SqlDbType.DateTime, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PlaceOfBirth", txtPOB.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Nationality", txtNationality.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RollTypeIds", RollTypeIds, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", hdnAccountStatus.Value, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@EntityType", rbtEntityType.SelectedValue, SqlDbType.NVarChar, 10, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@OverseasSocietyName", SocietyName, SqlDbType.NVarChar, 150, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SocietyId", Convert.ToInt64(SocietyId), SqlDbType.BigInt, 150, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AssociationName_India", txtAssociationMember.Text, SqlDbType.NVarChar, 150, ParameterDirection.Input));
                    if (ddlgeoname_PM == "" || ddlgeoname_PM == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", null, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    else
                    { 
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName", ddlgeoname_PM, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountAddress_PR", txtAddress_PR.InnerText.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    if (CityId_pm == "" || CityId_pm == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId_PR", 0    , SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId_PR", CityId_PR, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    if (ddlgeoname_PR == "" || ddlgeoname_PR == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName_PR", "", SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    }
                    else
                    { 
                        parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalName_PR", ddlgeoname_PR, SqlDbType.NVarChar, 255, ParameterDirection.Input));
                    }
                    if (PincodeId_PR == "" || PincodeId_PR == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId_PR", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    { 
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PincodeId_PR",Convert.ToInt64(PincodeId_PR), SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    if (Pincode_PR == "" || Pincode_PR == "0")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode_PR", "", SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode_PR", Pincode_PR, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PerZipCode", txtperZipCode.Text, SqlDbType.NVarChar, 25, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountEmail", txtEmail.Text.ToString().Trim(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Alt_EmailId", txtAltEmail.Text.ToString().Trim(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountImage", "MemberPhoto/" + strfilename, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@FatherName", txtFatherName.Text.Trim(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", txtFname.Text, SqlDbType.NVarChar, 30, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", txtLname.Text, SqlDbType.NVarChar, 35, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Gender", rbtnGender.SelectedItem.Text, SqlDbType.NVarChar, 10, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@LanguageId", Convert.ToInt64(LanguageId), SqlDbType.BigInt, 10, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@Languagename", Language, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreferredLanguage", DropDownLang.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreferredLanguageId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@InternalIdentificationName", txtInternalName.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@IPINumber", txtIPINumber.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ChanlDesc", txtchannel.InnerText, SqlDbType.NVarChar, 1000, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SocialSecurityNo", txtSocialNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@TRCNo", txtTrcNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@TenFForm", txtfForm.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@DualNationality", RBLNationality.SelectedValue, SqlDbType.TinyInt, 50, ParameterDirection.Input));
                    if (CountryId == "0" || CountryId == "")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PreCountryId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PreCountryId", CountryId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreCountryName", Country, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreState", txtState.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PreCity", txtCity.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    if (PerCountryId == "0" || PerCountryId == "")
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PerCountryId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@PerCountryId", PerCountryId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PerCountryName", PerCountry, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PerState", txtPerState.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PerCity", txtPerCity.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PresentZipCode", txtTrcNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    //parameters.Add(objGeneralFunction.GetSqlParameter("@PermanentZipCode", txtTrcNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    if (DDLTerAppFor.SelectedIndex == 0)
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@TeritoryAppFor", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    else
                    {
                        parameters.Add(objGeneralFunction.GetSqlParameter("@TeritoryAppFor", DDLTerAppFor.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    }
                    parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                    objDAL.ExecuteSP("App_Accounts_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                    MessageReturn = ReturnMessage;
            }
            catch (Exception ex)
            {
            }
            return RecordId;
        }

        #endregion

        #region function not in use
        protected Int64 SaveAccountAddress(string CityId_PR, string Pincode_PR)
        {
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty;
            try
            {
                var parameters = new List<SqlParameter>();
                string AdressType = "Present Personal Address";
                //if (rbtRegistrationType.SelectedValue == "C")
                //changed by RENU
                if (hdnRegistrationType.Value.ToString() == "C")
                    AdressType = "Present Office Address";

                parameters.Add(objGeneralFunction.GetSqlParameter("@AddressId", hdnAddressId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountCode", hdnAccountCode.Value.ToString(), SqlDbType.NVarChar, 50, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AddressType", AdressType, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Address", txtAddress_PR.InnerText.ToString(), SqlDbType.NVarChar, 255, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@GeographicalId", CityId_PR, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Pincode", Pincode_PR, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Phone", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Email", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Web", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@Fax", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

                DSIT_DataLayer objDAL = new DSIT_DataLayer();


                objDAL.ExecuteSP("App_Accounts_Address_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    hdnAddressId.Value = RecordId.ToString();
                }
            }
            catch (Exception)
            {


            }

            return RecordId;
        }

        #endregion

        /// <summary>
        /// For SaveAccountAddress_Contact
        /// In this fuction save member/company contact address in Table(App_Accounts_Address_Contact) by using
        /// store procedure(App_Accounts_Address_Contact_Manage_IPM)
        /// Commented By Rohit
        /// </summary>
        /// <param name="AccountName"></param>
        /// <param name="Mobile"></param>
        /// <returns>RecordId</returns>
        #region Insert Contact Address
        protected Int64 SaveAccountAddress_Contact(string AccountName, string Mobile)
        {
            Int64 RecordId = 0;
            string ReturnMessage = string.Empty;
            try
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@ContactId", hdnContactId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AddressId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
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
                objDAL.ExecuteSP("App_Accounts_Address_Contact_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    hdnContactId.Value = RecordId.ToString();
                }
            }
            catch (Exception)
            { }
            return RecordId;
        }

        #endregion

        /// <summary>
        /// For SaveBankIfo
        /// here update member Bank Information in Table(App_Accounts) by using store procedure(App_Accounts_Manage_Bank_IPM)
        /// Commented By Rohit
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns>RecordId</returns>
        #region Save Bank Inforamtion
        protected Int64 SaveBankIfo(Int64 AccountId)
        {
            Int64 RecordId = 0; 
            //RecordId = AccountId; //CHANGES BY HARIOM 14-12-22 
            string ReturnMessage = string.Empty;
            string Currency = ((TextBox)DDLCurrency.FindControl("txtDropDown")).Text;
            Int64 CurrencyId = Convert.ToInt64(((HiddenField)DDLCurrency.FindControl("hdnSelectedValue")).Value);
            try
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@hdnRecordKeyId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankName", txtBankName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankAcNo", txtBankAcNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankIFSCCode", txtIFSC.Text.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankBranchName", txtBankBranchName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@MicrCode", txtMICR.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@CurrencyId", CurrencyId, SqlDbType.BigInt, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@CurrencyName", Currency, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankSwift", txtswiftcode.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@BankAccountName", ddlAccountHoldername.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                objDAL.ExecuteSP("App_Accounts_Manage_Bank_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
            }
            catch (Exception)
            { }
            return RecordId;
        }

        #endregion

        /// <summary>
        /// For DisplayWorkDetails
        /// get member work information by using store procedure(App_Accounts_WorkRegistration_List_IPM) and these value to view state (ViewState["DTWorkNotification"])
        /// and bind gridview(gvWork).
        /// get incomplete song infromation by using store procedure(App_Accounts_WorkRegistration_List_IC) if get dataset is null then asper 
        /// that assign value to related controls by using Function(DisplayZeroWorkDetails) otherwise  assign value to related controls and get 
        /// the song deatil by Function(DisplaySongDetails)
        /// Commented By Rohit
        /// </summary>
        #region Display Work Deatil with Song Detail
        private void DisplayWorkDetails()
        {
            // string[] Strarry_Alias = null;
            //txtSongName.Text = "Song1";
            //txtFilm_AlbumName.Text = "Song1";
            //txtLanguageNames.Text = "Song1";
            //ddlWorkCategory.SelectedIndex = 1;
            //txtArtistorMultipleSingers.InnerText = "Artist1,Artist11";
            //txtRelYear.Text = "2019";
            //txtDigitalLink.Text = "google.com";
            //updated by Renu on 03/02/2021

            string Alias = txtAccountAlias.Text;
            if (Alias.Trim() == string.Empty)
            {
                if (hdnRegistrationType.Value == "I")
                {
                    Alias = hdnAccountName.Value;
                    txtAccountAlias.Text = Alias;
                }
                else
                {
                    Alias = txtcompanyName.Text;
                    txtAccountAlias.Text = Alias;
                }
            }
            else
            {
                Alias = Alias.TrimEnd(',') + "," + (hdnRegistrationType.Value == "I" ? hdnAccountName.Value : txtcompanyName.Text);
            }
            //  Strarry_Alias = Alias.TrimEnd(',').TrimStart(',').Split(',');
            if (hdnRegistrationType.Value == "LHN" || hdnRegistrationType.Value == "LH")
            {
                foreach (ListItem item in cbxRollTyped.Items)
                {
                    if (item.Selected == true)
                    {
                        if (item.Text.ToUpper().Contains("MUSIC"))
                        {
                            //txtAuthorMusicComposer.Attributes["placeholder"] = Alias; for zerowrok
                        }
                        if (item.Text.ToUpper().Contains("LYRIC"))
                        {
                            ////txtAuthorLyricist.Attributes["placeholder"] = Alias;for zerowrok
                            // txtAuthorLyricist.Text = Alias;
                        }
                        if (item.Text.ToUpper().Contains("PUBLISHER"))
                        {
                            //// txtPublisher.Attributes["placeholder"] = Alias;for zerowrok
                            // txtPublisher.Text = Alias;
                        }
                    }
                }
            }
            else
            {
                foreach (System.Web.UI.WebControls.ListItem item in cbxRollType.Items)
                {
                    if (item.Selected == true)
                    {
                        if (item.Text.ToUpper().Contains("MUSIC"))
                        {
                            //txtAuthorMusicComposer.Attributes["placeholder"] = Alias;for zerowrok
                        }
                        if (item.Text.ToUpper().Contains("LYRIC"))
                        {
                            //txtAuthorLyricist.Attributes["placeholder"] = Alias;for zerowrok
                            // txtAuthorLyricist.Text = Alias;
                        }
                        if (item.Text.ToUpper().Contains("PUBLISHER"))
                        {
                            // txtPublisher.Attributes["placeholder"] = Alias;for zerowrok
                            // txtPublisher.Text = Alias;
                        }
                    }
                }
            }

            #region Bind Grid WorkNotification
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            if (hdnRegistrationType.Value == "LH")
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRefAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            else
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            DataSet MyDataSet = new DataSet();
            MyDataSet = objDAL.GetDataSet("App_Accounts_WorkRegistration_List_IPM", parameters.ToArray());
            if (MyDataSet == null)
            {
                objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                return;
            }
            else
            {
                if (MyDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = MyDataSet.Tables[0].Rows[0];
                    HdnZeroworkID.Value = DR["WorkNotificationId"].ToString();
                }
                    ViewState["DTWorkNotification"] = MyDataSet.Tables[0];
                    HiddenField hdnScreenWidth = (HiddenField)Page.Master.FindControl("hdnScreenWidth");
                    objGeneralFunction.SetGridWidth(gvWork, hdnScreenWidth.Value, DivWork);
                    gvWork.DataSource = MyDataSet.Tables[0];
                    gvWork.DataBind();

                #region code no n use
                //objDAL = null;
                Int64 RecordId = 0;
                    string ReturnMessage = string.Empty;
                //parameters.Clear();
                //parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                //objDAL.ExecuteSP("App_Accounts_SongRegistration_list", parameters.ToArray(), out ReturnMessage, out RecordId);

                //if (RecordId == 0)
                //{
                //    btnAddWork.Enabled = true;
                //    btnAddWork.Text = "Add Song Details";
                //}
                //else
                //{
                //    btnAddWork.Enabled = false;
                //    btnAddWork.Text = "Add New Song Details";
                //}
                #endregion

                MyDataSet.Dispose();
            }
            #endregion Bind Grid WorkNotification

            #region Display incomplete Song Details

            parameters.Clear();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            DataSet MyWorkDataSet = new DataSet();
            MyWorkDataSet=objDAL.GetDataSet("App_Accounts_WorkRegistration_List_IC", parameters.ToArray());
            if (MyWorkDataSet.Tables[0].Rows.Count == 0)
            {
                //objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                DisplayZeroWorkDetails();
                return;
            }
            else
            {
                if (MyWorkDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = MyWorkDataSet.Tables[0].Rows[0];
                    txtISRCNo.Text = DR["ISRCNo"].ToString();
                    txtSongName.Text = DR["SongName"].ToString();
                    txtAltrSongName.Text = DR["AltSongTitle"].ToString();
                    txtFilm_AlbumName.Text = DR["Film_AlbumName"].ToString();
                    ddlIntendedPurpose.SelectedItem.Text = DR["WorkCategory"].ToString();
                    ((TextBox)ddlLanguage.FindControl("txtDropDown")).Text = DR["LanguageNames"].ToString();
                    txtDuration.Text = DR["SongDuration"].ToString();
                    ddlVersion.SelectedItem.Text = DR["SongVersion"].ToString();
                    ddlMusicRelation.SelectedItem.Text = DR["MusicRelationship"].ToString();
                    ddlBLTVR.SelectedItem.Text = DR["BLTVR"].ToString();
                    ddlWorkCategory.SelectedItem.Text = DR["SongCategory"].ToString();
                    if (DR["SongSubCategory"].ToString() == "")
                    {
                        ddlWorkSubCategory.Items.Clear();
                        ddlWorkSubCategory.Items.Insert(0, new ListItem("Null", "Null"));
                        ddlWorkSubCategory.Enabled = false;
                    }
                    else
                    {
                        parameters.Clear();
                        DataSet swds = new DataSet();
                        parameters.Add(objGeneralFunction.GetSqlParameter("@CategoryGroupId", ddlWorkCategory.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                        objDAL.FillDropDown("App_SubCategory_List", parameters.ToArray(), ddlWorkSubCategory, "Work Sub Category", out swds);
                        ddlWorkSubCategory.SelectedItem.Text = DR["SongSubCategory"].ToString();
                    }
                    txtArtistorMultipleSingers.InnerText = DR["Artist_Singers"].ToString();
                    txtDigitalLink.Text = DR["DigitalLink"].ToString();
                    txtRelYear.Text = DR["ReleaseYear"].ToString();
                    btnAddWork.Enabled = false;
                    btnAddWork.Text = "Add New Song Details";
                    DisplaySongDetails();
                }
                else
                {
                    btnAddWork.Enabled = true;
                    btnAddWork.Text = "Add New Song Details";
                    // DisplayZeroWorkDetails();
                }
            }
            MyWorkDataSet.Dispose();
            #endregion
        }

        #endregion

        /// <summary>
        /// For DisplayZeroWorkDetails
        /// In this Function we get work inforamtion by using store procedure(App_Accounts_WorkRegistration_List_IPM) and bind to gridview(gvWork) 
        /// assign value to related controls for the new work registration.
        /// Commented By Rohit
        /// </summary>
        #region Zero work Detail
        private void DisplayZeroWorkDetails()
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parame = new List<SqlParameter>();
            
            #region ISRC No
            if (hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NC")
            {
                txtISRCNo.Enabled = true;
            }
            else
            {
                txtISRCNo.Enabled = false;
            }
            #endregion
            
            txtSongName.Text = "";
            txtAltrSongName.Text = "";
            txtFilm_AlbumName.Text = "";

            #region Populate Intended Purpose
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "IP", SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlIntendedPurpose, "Intended Purpose");
            #endregion

            #region Song Language
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@Status", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            ddlLanguage.PopulateDropDown("App_Language_List", parame, "Song Language");
            #endregion

            #region Populate Version
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "VR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlVersion, "Version");
            #endregion

            #region Populate Music Relation
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "MR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlMusicRelation, "Music Relation");
            #endregion

            #region Populate BlTVR
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@TypeCode", "BLTVR", SqlDbType.NVarChar, 10, ParameterDirection.Input));
            objDAL.FillDropDown("App_SongDetails_List", parame.ToArray(), ddlBLTVR, "BlTVR");
            #endregion

            #region Populate Category
            objDAL.FillDropDown("App_Category_List", ddlWorkCategory, "Work Category");
            #endregion

            #region Populate Perform Society
            parame.Clear();
            parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
            objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
            ddlPerformingSociety.SelectedIndex = 1;
            #endregion

            txtArtistorMultipleSingers.InnerText = "";
            txtDigitalLink.Text = "";
            txtRelYear.Text = "";
            btnAddWork.Enabled = true;
            btnAddWork.Text = "Add New Song Details";

            #region Bind Grid WorkNotification
            var parameters = new List<SqlParameter>();
            if (hdnRegistrationType.Value == "LH")
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRefAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            else
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            DataSet MyDataSet = new DataSet();
            MyDataSet = objDAL.GetDataSet("App_Accounts_WorkRegistration_List_IPM", parameters.ToArray());
            if (MyDataSet == null)
            {
                objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                return;
            }
            else
            {
                if (MyDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = MyDataSet.Tables[0].Rows[0];
                    HdnZeroworkID.Value = DR["WorkNotificationId"].ToString();
                }
                ViewState["DTWorkNotification"] = MyDataSet.Tables[0];
                HiddenField hdnScreenWidth = (HiddenField)Page.Master.FindControl("hdnScreenWidth");
                objGeneralFunction.SetGridWidth(gvWork, hdnScreenWidth.Value, DivWork);
                gvWork.DataSource = MyDataSet.Tables[0];
                gvWork.DataBind();
                MyDataSet.Dispose();
                gvAuthComp.Dispose();
                gvAuthComp.DataBind();
            }
            #endregion Bind Grid WorkNotification
        }

        #endregion

        /// <summary>
        /// For DisplaySongDetails
        /// in this function get song detail to related work registraion by using store procedure(App_Accounts_SongRegistration_list) by prameter
        /// (WorkNotificationId) and bind the gridview(gvAuthComp) and assign table to viewstate(ViewState["DTSongDetails"])
        /// Commented By Rohit
        /// </summary>
        #region Display Song Details
        private void DisplaySongDetails()
        {
            #region Bind Grid WorkNotification

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();

            if (hdnRegistrationType.Value == "LH")
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRefAccountId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            else
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            }

            DataSet MyDataSet = new DataSet();
            MyDataSet = objDAL.GetDataSet("App_Accounts_SongRegistration_list", parameters.ToArray());

            if (MyDataSet == null)
            {
                objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                return;
            }
            else
            {
                //ViewState["DTSongDetails"] = MyDataSet.Tables[0];
                //HiddenField hdnScreenWidth = (HiddenField)Page.Master.FindControl("hdnScreenWidth");
                //objGeneralFunction.SetGridWidth(gvAuthComp, hdnScreenWidth.Value, DivAuthComp);
                //DataSet ds = new DataSet(); 
                //gvAuthComp.DataSource = MyDataSet.Tables[0];
                //gvAuthComp.DataBind();
                //MyDataSet.Dispose();
                DataTable DTBSongNotification = new DataTable();
                DTBSongNotification.Columns.Add(new DataColumn("SongRegistrationID", typeof(int)));
                DTBSongNotification.Columns.Add(new DataColumn("WorkNotificationId", typeof(int)));
                DTBSongNotification.Columns.Add(new DataColumn("AccountId", typeof(int)));
                DTBSongNotification.Columns.Add(new DataColumn("RoleType", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("MemberOfSoc", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("PerformingSociety", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("IPINumber", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("FirstName", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("LastName", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("PublisherName", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("DP", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("PerformanceShare", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("MechanicalShare", typeof(string)));
                DTBSongNotification.Columns.Add(new DataColumn("SyncShare", typeof(string)));


                for (int i=0; i < MyDataSet.Tables[0].Rows.Count ; i++)
                {
                    DataRow myDataRow = DTBSongNotification.NewRow();
                    myDataRow["SongRegistrationID"] = MyDataSet.Tables[0].Rows[i]["SongRegistrationID"].ToString();
                    myDataRow["WorkNotificationId"] = HdnZeroworkID.Value;
                    myDataRow["AccountId"] = hdnRecordId.Value;

                    if (MyDataSet.Tables[0].Rows[i]["RoleType"].ToString() == "A")
                    {
                        myDataRow["RoleType"] = "AUTHOR";
                    }
                    else if (MyDataSet.Tables[0].Rows[i]["RoleType"].ToString() == "C")
                    {
                        myDataRow["RoleType"] = "COMPOSER";
                    }
                    else if (MyDataSet.Tables[0].Rows[i]["RoleType"].ToString() == "CA")
                    {
                        myDataRow["RoleType"] = "COMPOSER/AUTHOR";
                    }
                    else
                    {
                        myDataRow["RoleType"] = "PUBLISHER";
                    }

                    if (MyDataSet.Tables[0].Rows[i]["MemberOfSoc"].ToString() == "Y")
                    {
                        myDataRow["MemberOfSoc"] = "YES";
                    }
                    else
                    {
                        myDataRow["MemberOfSoc"] = "NO";
                    }
                    myDataRow["PerformingSociety"] = MyDataSet.Tables[0].Rows[i]["PerformingSociety"].ToString();
                    myDataRow["IPINumber"] = MyDataSet.Tables[0].Rows[i]["IPINumber"].ToString();
                    myDataRow["FirstName"] = MyDataSet.Tables[0].Rows[i]["FirstName"].ToString();
                    myDataRow["LastName"] = MyDataSet.Tables[0].Rows[i]["LastName"].ToString();
                    myDataRow["PublisherName"] = MyDataSet.Tables[0].Rows[i]["PublisherName"].ToString();
                    if (MyDataSet.Tables[0].Rows[i]["DP"].ToString() == "Y")
                    {
                        myDataRow["DP"] = "YES";
                    }
                    else
                    {
                        myDataRow["DP"] = "NO";
                    }
                    myDataRow["PerformanceShare"] = MyDataSet.Tables[0].Rows[i]["PerformanceShare"].ToString();
                    myDataRow["MechanicalShare"] = MyDataSet.Tables[0].Rows[i]["MechanicalShare"].ToString();
                    if (MyDataSet.Tables[0].Rows[i]["SyncShare"].ToString() == "")
                    {
                        myDataRow["SyncShare"] = "0.00";
                    }
                    else
                    {
                        myDataRow["SyncShare"] = MyDataSet.Tables[0].Rows[i]["SyncShare"].ToString();
                    }
                    DTBSongNotification.Rows.Add(myDataRow);
                }
                DTBSongNotification.AcceptChanges();

                DataView DVview = DTBSongNotification.DefaultView;
                DVview.Sort = "RoleType";
                gvAuthComp.DataSource = DVview;
                gvAuthComp.DataBind();

                gvAuthComp.Visible = true;

                ViewState["DTSongDetails"] = DTBSongNotification;
                DTBSongNotification.Dispose();
            }
            #endregion Bind Grid WorkNotification
        }

        #endregion

        #region Event Not in use
        protected void wzMain_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //if (wzMain.ActiveStepIndex == 1)
            //{
            //    DisplayOperation();
            //}
            //if (wzMain.ActiveStepIndex == 3)
            //{
            //    DisplayWorkDetails();
            //}
        }

        #endregion 

        /// <summary>
        /// For btnWorkSelect_Click
        /// in this event get selected work detail from gridview(gvWork)  by using store procedure(App_Accounts_WorkRegistration) 
        /// and assign value to related controls and aslo using function(DisplaySongDetails) for getrelated song details 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Select work From Gridview(gvWork)
        protected void btnWorkSelect_Click(object sender, EventArgs e)
        {
            if (btnAddWork.Enabled == true)
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                string WorknotificationId = ((HiddenField)gvr.FindControl("hdnWorkNotificationId")).Value;
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", Convert.ToInt64(WorknotificationId.ToString()), SqlDbType.BigInt, 0, ParameterDirection.Input));
                DataSet MySelectDataSet = new DataSet();
                MySelectDataSet = objDAL.GetDataSet("App_Accounts_WorkRegistration_Select_List_IPM", parameters.ToArray());
                if (MySelectDataSet == null)
                {
                    objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                    return;
                }
                else
                {
                    DataRow DR = MySelectDataSet.Tables[0].Rows[0];
                    HdnZeroworkID.Value = DR["WorkNotificationId"].ToString();
                    txtISRCNo.Text = DR["ISRCNo"].ToString();
                    txtSongName.Text = DR["SongName"].ToString();
                    txtAltrSongName.Text = DR["AltSongTitle"].ToString();
                    txtFilm_AlbumName.Text = DR["Film_AlbumName"].ToString();
                    ddlIntendedPurpose.ClearSelection();
                    ddlIntendedPurpose.Items.FindByText(DR["WorkCategory"].ToString()).Selected = true;
                    ddlLanguage.SelectDropDown(DR["LanguageID"].ToString(), DR["LanguageNames"].ToString().Trim());
                    ddlVersion.ClearSelection();
                    ddlVersion.Items.FindByText(DR["SongVersion"].ToString()).Selected = true;
                    ddlMusicRelation.ClearSelection();
                    ddlMusicRelation.Items.FindByText(DR["MusicRelationship"].ToString()).Selected = true;
                    ddlBLTVR.ClearSelection();
                    ddlBLTVR.Items.FindByText(DR["BLTVR"].ToString()).Selected = true;
                    ddlWorkCategory.ClearSelection();
                    ddlWorkCategory.Items.FindByText(DR["SongCategory"].ToString()).Selected = true;
                    string catid = ddlWorkCategory.SelectedValue;
                    parameters.Clear();
                    DataSet swds = new DataSet();
                    parameters.Add(objGeneralFunction.GetSqlParameter("@CategoryGroupId", ddlWorkCategory.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    objDAL.FillDropDown("App_SubCategory_List", parameters.ToArray(), ddlWorkSubCategory, "Work Sub Category", out swds);
                    if (swds.Tables[0].Rows.Count > 1)
                    {
                        ddlWorkSubCategory.Enabled = true;
                        ddlWorkSubCategory.ClearSelection();
                        ddlWorkSubCategory.Items.FindByText(DR["SongSubCategory"].ToString()).Selected = true;
                    }
                    else
                    {
                        ddlWorkSubCategory.Items.Clear();
                        ddlWorkSubCategory.Items.Insert(0, new ListItem("Null", "Null"));
                        ddlWorkSubCategory.Enabled = false;
                    }
                    txtDuration.Text = DR["SongDuration"].ToString();
                    txtArtistorMultipleSingers.InnerText = DR["Artist_Singers"].ToString();
                    txtRelYear.Text = DR["ReleaseYear"].ToString();
                    txtDigitalLink.Text = DR["DigitalLink"].ToString();
                    DisplaySongDetails();
                    btnAddWork.Text = "Clear Song Selection";
                }
            }
        }

        #endregion

        /// <summary>
        /// For btnWorkDelete_Click
        /// this event use for delete work record using store procedure(App_Accounts_WorkRegistration_Delete)
        /// and delete releted workids song details using store procedure(App_Accounts_SongRegistration_Delete)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Delete Work
        protected void btnWorkDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            string WorknotificationId = ((HiddenField)gvr.FindControl("hdnWorkNotificationId")).Value;
            ///* Code to Delete Employee Record */
            Int64 RecordId;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", Convert.ToInt64(WorknotificationId.ToString()), SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;
            objDAL.ExecuteSP("App_Accounts_WorkRegistration_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);
            parameters.Clear();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", Convert.ToInt64(WorknotificationId.ToString()), SqlDbType.BigInt, 0, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            objDAL.ExecuteSP("App_Accounts_SongRegistration_Delete", parameters.ToArray());
            if (ReturnMessage == string.Empty)
                objGeneralFunction.BootBoxAlert("ERROR IN DELETING DATA. PLEASE CONTACT ADMINISTRATOR", this.Page);
            else
            {
                DataTable DTBWorkNotification = new DataTable();
                if (ViewState["DTWorkNotification"] != null)
                    DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];
                if (RecordId > 0)
                {
                    DataRow myDataRow = DTBWorkNotification.Select("WorkNotificationId=" + WorknotificationId).FirstOrDefault(); // finds all rows with id==2 and selects first or null if haven't found any
                    if (myDataRow != null)
                    {
                        myDataRow.Delete();
                        string FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                        int fileDel = FileDelete("MWN_" + hdnRecordId.Value + "_" + WorknotificationId + "_", FilePath);
                    }
                }

                WorkControlClear();
                btnAddWork.Enabled = true;
                //btnAddWork.Text = "Add Song Details";

                DTBWorkNotification.AcceptChanges();


                if (DTBWorkNotification != null)
                {
                    gvWork.DataSource = DTBWorkNotification;
                    gvWork.DataBind();
                }


                ViewState["DTWorkNotification"] = DTBWorkNotification;
                DTBWorkNotification.Dispose();


                objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
            }
            objDAL = null;
        }
        #endregion
        

        protected void btnSongDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            string SongRegistrationID = ((HiddenField)gvr.FindControl("hdnSongRegistrationID")).Value;
            Int64 RecordId; string ReturnMessage = string.Empty;

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet MySelectDataSet = new DataSet();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", SongRegistrationID, SqlDbType.BigInt, 0, ParameterDirection.Input));
            MySelectDataSet = objDAL.GetDataSet("App_Accounts_SongRegistration_Select_List_IPM", parameters.ToArray());

            GetDeletepercentage(int.Parse(SongRegistrationID));

            parameters.Clear();
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongRegistrationID", SongRegistrationID, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            objDAL.ExecuteSP("App_Accounts_SongDetail_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);

            WorkUpdateSongShare(long.Parse(MySelectDataSet.Tables[0].Rows[0]["WorkNotificationId"].ToString()));

            DisplaySongDetails();


        }

        /// <summary>
        /// For btnSongSelect_Click
        /// in this event get selected work detail from gridview(gvAuthComp)  by using store procedure(App_Accounts_SongRegistration_Select_List_IPM) 
        /// and assign value to related controls.
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Select Song From Gridview(gvAuthComp) 
        protected void btnSongSelect_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            string SongRegistrationID = ((HiddenField)gvr.FindControl("hdnSongRegistrationID")).Value;

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", Convert.ToInt64(SongRegistrationID.ToString()), SqlDbType.BigInt, 0, ParameterDirection.Input));

            DataSet MySelectDataSet = new DataSet();
            MySelectDataSet = objDAL.GetDataSet("App_Accounts_SongRegistration_Select_List_IPM", parameters.ToArray());
            if (MySelectDataSet == null)
            {
                objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                return;
            }
            else
            {
                DataRow DR = MySelectDataSet.Tables[0].Rows[0];
                if (DR["FirstName"].ToString() == "N/A")
                {
                    divPublisher.Visible = true;
                    divAutCom.Visible = false;
                }
                else
                {
                    divPublisher.Visible = false;
                    divAutCom.Visible = true;
                }
                hdnSongRegistrationID.Value= DR["SongRegistrationID"].ToString();
                HdnZeroworkID.Value = DR["WorkNotificationId"].ToString();
                ddlRegisterType.SelectedValue = DR["RoleType"].ToString();
                rbtnlACOtherSoc.SelectedValue= DR["MemberOfSoc"].ToString();
                txtACIPInumber.Text= DR["IPINumber"].ToString();
                txtACFirstName.Text = DR["FirstName"].ToString();
                txtACLastName.Text = DR["LastName"].ToString();
                txtPublisherName.Text = DR["PublisherName"].ToString();
                rdbnldp.SelectedValue = DR["DP"].ToString();
                txtACPerformanceShare.Text = DR["PerformanceShare"].ToString();
                txtACMechanicalShare.Text = DR["MechanicalShare"].ToString();
                txtACSyncShare.Text = DR["SyncShare"].ToString();
                var parame = new List<SqlParameter>();
                if (rbtnlACOtherSoc.SelectedValue == "Y")
                {
                    objDAL.GetDataReader("SELECT [dbo].[GetSocityID] ('IPRS')");
                    string SocityNameCode = DR["PerformingSociety"].ToString();
                    string SocityName = SocityNameCode.Substring(0, SocityNameCode.IndexOf('(')).Trim();
                    //string[] parts = SocityNameCode.Split(')');
                    //string SocityCode = "";

                    //for (int i = 0; i < parts.Length; i++)
                    //{
                    //    if (parts[i].Contains("("))
                    //    {
                    //        string[] parts2 = parts[i].Split('(');
                    //        SocityCode = parts2[1];
                    //    }
                    //}

                    ddlPerformingSociety.Enabled = true;
                    txtACIPInumber.Enabled = true;
                    parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 2, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parame.Add(objGeneralFunction.GetSqlParameter("@Perform", SocityName, SqlDbType.NVarChar,50, ParameterDirection.Input));
                    objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
                    ddlPerformingSociety.SelectedItem.Text = DR["PerformingSociety"].ToString(); 
                }
                else
                {
                    parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
                    ddlPerformingSociety.SelectedItem.Text = DR["PerformingSociety"].ToString();
                    ddlPerformingSociety.Enabled = false;
                    txtACIPInumber.Enabled = false;
                }
                //btnAddAuthComp.Text = "Clear Selection";
            }
        }

        #endregion

        /// <summary>
        /// for btnCmpSongDetail_Click
        /// In this event first calculate total share percentage against worknotification using store procedure(App_Accounts_Song_Share_CL)
        /// if share percentage below under 100% then display massage otherwise update RecordStatus = 1 in Table(App_Accounts_WorkRegistration)
        /// using store procedure(App_Accounts_WorkRegistration_Update). using Function(DisplayWorkDetails) to get work information.
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Complete Song Details
        protected void btnCmpSongDetail_Click(object sender, EventArgs e)
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            string ReturnMessage = string.Empty;
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            DataSet SDS = new DataSet();
            SDS = objDAL.GetDataSet("App_Accounts_Song_Share_CL", parameters.ToArray());
            double share = double.Parse(SDS.Tables[0].Rows[0]["Share"].ToString());
            if (share < 100)
            {
                objGeneralFunction.BootBoxAlert("Please Complete 100%  of Performance and Mechanical Share 100% ", this.Page);
                return;
            }
            else 
            {
                parameters.Clear();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.ExecuteScalar("App_Accounts_WorkRegistration_Update", parameters.ToArray());

                DisplayWorkDetails();
                btnAddAuthComp.Enabled = true;
                btnCmpSongDetail.Enabled = true;
                //gvAuthComp.DataSource=null;
                gvAuthComp.DataBind();
            }
        }

        #endregion

        /// <summary>
        /// For WorkNotificationSave
        /// get information by differnt control and assing those value to related parameters if worknotification = 0 then insert record in
        /// to Table(App_Accounts_WorkRegistration) otherwise update records in to table(App_Accounts_WorkRegistration) using Store Procedure
        /// (App_Accounts_WorkRegistration_Mange_IPM). create datatable and bind with gridview(gvWork) and Viewstate(DTWorkNotification).
        /// Commented By Rohit
        /// </summary>
        /// <param name="intRecordId"></param>
        /// <returns>RecordId</returns>
        #region Save Work Notification
        protected Int64 WorkNotificationSave(Int64 intRecordId)
        {
            string strMode = string.Empty;
            Int64 RecordId = 0;

            #region Save Work Details

            #region Insert Work Detail in To table
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ISRCNo", txtISRCNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongName", txtSongName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AltSongTitle", txtAltrSongName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@Film_AlbumName", txtFilm_AlbumName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            string IntendedPurpose = ddlIntendedPurpose.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkCategory", IntendedPurpose, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            string SonglanguageID = ddlLanguage.GetSelectedValue().ToString();
            string Songlangu = ddlLanguage.GetSelectedText().ToString();
            string Songlanguage = Songlangu.Substring(0, Songlangu.IndexOf("("));
            parameters.Add(objGeneralFunction.GetSqlParameter("@LanguageNames", Songlanguage, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@LanguageID", SonglanguageID, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongDuration", txtDuration.Text, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            string Version = ddlVersion.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongVersion", Version, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            string MusicRelation = ddlMusicRelation.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@MusicRelationship", MusicRelation, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            string BLTVR = ddlBLTVR.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@BLTVR", BLTVR, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            string SongWorkCategory = ddlWorkCategory.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongCategory", SongWorkCategory, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            string WorkSubCategory = ddlWorkSubCategory.SelectedItem.ToString();
            parameters.Add(objGeneralFunction.GetSqlParameter("@SongSubCategory", WorkSubCategory, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@Artist_Singers", txtArtistorMultipleSingers.InnerText, SqlDbType.NVarChar, 500, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DigitalLink", txtDigitalLink.Text.Trim().ToString(), SqlDbType.NVarChar, 500, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocLink", "", SqlDbType.NVarChar, 500, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReleaseYear", txtRelYear.Text.Trim().ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;
            objDAL.ExecuteSP("App_Accounts_WorkRegistration_Mange_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
            #endregion

            #region "SET ADD BUTTON STATUS BASED ON FORM REQUIREMENT"
            if (RecordId == 0 || ReturnMessage.Contains("RECORD ALREADY EXIST"))
            {
                 return RecordId;
            }
            string[] savedfile = SaveWorkFile(FPWork, RecordId);
            //string savedfile = SaveWorkFile(FPWork, RecordId);
            //if (savedfile[1] == string.Empty)
            //{
            //    FalseWork_delete(RecordId);
            //    RecordId = 0;
            //    return RecordId;
            //}
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", RecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocLink", savedfile[1], SqlDbType.NVarChar, 500, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            objDAL.ExecuteSP("App_Accounts_WorkRegistration_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);

            DataTable DTBWorkNotification = new DataTable();
            if (ViewState["DTWorkNotification"] != null)
                DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];

            #region "CREATING NEW CONTACT"
            DataRow myDataRow = DTBWorkNotification.NewRow();
            myDataRow["WorkNotificationId"] = RecordId;
            myDataRow["AccountId"] = hdnRecordId.Value;
            myDataRow["ISRCNo"] = txtISRCNo.Text.ToString();
            myDataRow["SongName"] = txtSongName.Text.ToString();
            myDataRow["AltSongTitle"] = txtAltrSongName.Text.ToString();
            myDataRow["Film_AlbumName"] = txtFilm_AlbumName.Text.ToString();
            myDataRow["WorkCategory"] = ddlIntendedPurpose.SelectedItem.ToString();
            myDataRow["LanguageNames"] = ddlLanguage.GetSelectedText().ToString();
            myDataRow["SongDuration"] = txtDuration.Text.ToString();
            myDataRow["SongVersion"] = ddlVersion.SelectedItem.ToString();
            myDataRow["MusicRelationship"] = ddlMusicRelation.SelectedItem.ToString();
            myDataRow["BLTVR"] = ddlBLTVR.SelectedItem.ToString();
            myDataRow["SongCategory"] = ddlWorkCategory.SelectedItem.ToString();
            myDataRow["SongSubCategory"] = ddlWorkSubCategory.SelectedItem.ToString();
            myDataRow["Artist_Singers"] = txtArtistorMultipleSingers.InnerText.ToString();
            myDataRow["ReleaseYear"] = txtRelYear.Text.ToString();
            myDataRow["DigitalLink"] = txtDigitalLink.Text.ToString();
            if (savedfile[0] == "")
                myDataRow["Workfile"] = "";
            else
                myDataRow["Workfile"] = savedfile[0];

            DTBWorkNotification.Rows.Add(myDataRow);
            #endregion
            DTBWorkNotification.AcceptChanges();
            DataView DVview = DTBWorkNotification.DefaultView;
            DVview.Sort = "SongName";
            gvWork.DataSource = DVview;
            gvWork.DataBind();
            gvWork.Visible = true;
            ViewState["DTWorkNotification"] = DTBWorkNotification;
            DTBWorkNotification.Dispose();
            #endregion "SET ADD BUTTON STATUS BASED ON FORM REQUIREMENT"
            #region "This done as per client Requiremt"
            DisplayWorkDetails();
            #endregion
            return RecordId;
            #endregion
        }

        #endregion

        /// <summary>
        /// For WorkControlClear
        /// In this Function clear all work relate controls value
        /// Commented By Rohit
        /// </summary>
        #region Clear Controls
        protected void WorkControlClear()
        {
            #region set control value
            txtISRCNo.Text = string.Empty;
            txtSongName.Text = string.Empty;
            txtAltrSongName.Text = string.Empty;
            txtFilm_AlbumName.Text = string.Empty;
            ddlIntendedPurpose.SelectedValue = "0";
            ((TextBox)ddlLanguage.FindControl("txtDropDown")).Text = string.Empty;
            txtDuration.Text = "04:00";
            ddlVersion.SelectedValue = "0";
            ddlMusicRelation.SelectedValue = "0";
            ddlBLTVR.SelectedValue = "0";
            ddlWorkCategory.SelectedValue = "0";
            ddlWorkSubCategory.Items.Clear();
            ddlWorkSubCategory.Enabled = false;
            txtArtistorMultipleSingers.InnerText = string.Empty;
            txtDigitalLink.Text = string.Empty;
            txtRelYear.Text = string.Empty;
            txtSongName.Focus();
            #endregion
        }

        #endregion

        /// <summary>
        /// For WorkSongSave
        /// get information by differnt control and assing those value to related parameters if SongRegistrationID = 0 then insert record in
        /// to Table(App_Accounts_SongRegistration) otherwise update records in to table(App_Accounts_SongRegistration) using Store Procedure
        /// (App_Accounts_SongRegistration_Mange_IPM). Update record share asper role type using Function(WorkUpdateSongShare). Display song records in gridview(gvAuthComp)
        /// using Function(DisplaySongDetails)
        /// Commented By Rohit
        /// </summary>
        /// <param name="intRecordId"></param>
        /// <returns></returns>
        #region Save Song Detail
        protected Int64 WorkSongSave(Int64 intRecordId)
        {
            Int64 RecordId = 0;
            string strMode = string.Empty;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;
            DataTable dataexists = new DataTable();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@RoleType", ddlRegisterType.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", txtACFirstName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", txtACLastName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@PublisherName", txtPublisherName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            
            dataexists = objDAL.GetDataTable("App_Accounts_SongRegistration_list_RT", parameters.ToArray());
            if (dataexists.Rows.Count > 0)
            {
                objGeneralFunction.BootBoxAlert("RECORD AlREADY EXISTS", Page);
                return RecordId;
            }
            else
            {
                #region Auther/Composer/Publisher
                parameters.Clear();
                if (hdnSongRegistrationID.Value == "")
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SongRegistrationID", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SongRegistrationID", hdnSongRegistrationID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                }
                parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@RoleType", ddlRegisterType.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@MemberOfSoc", rbtnlACOtherSoc.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@PerformingSociety", ddlPerformingSociety.SelectedItem.ToString(), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@IPINumber", txtACIPInumber.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));

                if (txtACFirstName.Visible == false)
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", "N/A", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", txtACFirstName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }

                if (txtACLastName.Visible == false)
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", "N/A", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", txtACLastName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }

                if (txtPublisherName.Visible == false)
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PublisherName", "N/A", SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@PublisherName", txtPublisherName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                }
                parameters.Add(objGeneralFunction.GetSqlParameter("@DP", rdbnldp.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@PerformanceShare", Convert.ToDouble(txtACPerformanceShare.Text), SqlDbType.Float, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@MechanicalShare", Convert.ToDouble(txtACMechanicalShare.Text), SqlDbType.Float, 0, ParameterDirection.Input));
                if (txtACSyncShare.Text == "")
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SyncShare", 0.00, SqlDbType.Float, 0, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@SyncShare", Convert.ToDouble(txtACSyncShare.Text), SqlDbType.Float, 0, ParameterDirection.Input));
                }
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                objDAL.ExecuteSP("App_Accounts_SongRegistration_Mange_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                #endregion

                #region  Update Song Share
                WorkUpdateSongShare(intRecordId);
                lblZAShare.Text = "";
                lblZCShare.Text = "";
                lblZCAShare.Text = "";
                lblZECLShare.Text = "";
                // parameters.Clear();
                // parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@RoleType", ddlRegisterType.SelectedValue, SqlDbType.NVarChar, 100, ParameterDirection.Input));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@PerformanceShare", Convert.ToDouble(txtACPerformanceShare.Text), SqlDbType.Float, 0, ParameterDirection.Input));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@MechanicalShare", Convert.ToDouble(txtACMechanicalShare.Text), SqlDbType.Float, 0, ParameterDirection.Input));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                // parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                // ReturnMessage = string.Empty;
                //objDAL.ExecuteSP("App_Accounts_SongRegistration_Share_Update", parameters.ToArray(), out ReturnMessage, out RecordId);
                #endregion


                #region Bind Grid
                //DataTable DTBSongNotification = new DataTable();
                //if (ViewState["DTSongDetails"] != null)
                //    DTBSongNotification = (DataTable)ViewState["DTSongDetails"];
                //#region "CREATING NEW CONTACT"
                //DataRow myDataRow = DTBSongNotification.NewRow();
                //myDataRow["SongRegistrationID"] = RecordId;
                //myDataRow["WorkNotificationId"] = intRecordId;
                //myDataRow["AccountId"] = hdnRecordId.Value;
                //if (ddlRegisterType.SelectedValue == "A")
                //{
                //    myDataRow["RoleType"] = "AUTHOR";
                //}
                //else if (ddlRegisterType.SelectedValue == "C")
                //{
                //    myDataRow["RoleType"] = "COMPOSER";
                //}
                //else if (ddlRegisterType.SelectedValue == "CA")
                //{
                //    myDataRow["RoleType"] = "COMPOSER/AUTHOR";
                //}
                //else
                //{
                //    myDataRow["RoleType"] = "PUBLISHER";
                //}
                ////myDataRow["RoleType"] = ddlRegisterType.SelectedValue.ToString();
                //if (rbtnlACOtherSoc.SelectedValue == "Y")
                //{
                //    myDataRow["MemberOfSoc"] = "YES";
                //}
                //else
                //{
                //    myDataRow["MemberOfSoc"] = "NO";
                //}
                ////myDataRow["MemberOfSoc"] = rbtnlACOtherSoc.SelectedValue.ToString();
                //myDataRow["PerformingSociety"] = ddlPerformingSociety.SelectedItem.ToString();
                //myDataRow["IPINumber"] = txtACIPInumber.Text.ToString();
                //myDataRow["FirstName"] = txtACFirstName.Text.ToString().ToUpper();
                //myDataRow["LastName"] = txtACLastName.Text.ToString().ToUpper();
                //myDataRow["PublisherName"] = txtPublisherName.Text.ToString().ToUpper();
                //if (rdbnldp.SelectedValue == "Y")
                //{
                //    myDataRow["DP"] = "YES";
                //}
                //else
                //{
                //    myDataRow["DP"] = "NO";
                //}
                ////myDataRow["DP"] = rdbnldp.SelectedValue.ToString();
                //myDataRow["PerformanceShare"] = txtACPerformanceShare.Text.ToString();
                //myDataRow["MechanicalShare"] = txtACMechanicalShare.Text.ToString();
                //if (txtACSyncShare.Text == "")
                //{
                //    myDataRow["SyncShare"] = "0.00";
                //}
                //else
                //{
                //    myDataRow["SyncShare"] = txtACSyncShare.Text.ToString();
                //}
                //DTBSongNotification.Rows.Add(myDataRow);
                //DTBSongNotification.AcceptChanges();
                //DataView DVview = DTBSongNotification.DefaultView;
                //DVview.Sort = "RoleType";
                //gvAuthComp.DataSource = DVview;
                //gvAuthComp.DataBind();
                //gvAuthComp.Visible = true;
                //ViewState["DTSongDetails"] = DTBSongNotification;
                //DTBSongNotification.Dispose();
                #endregion

                DisplaySongDetails();
                ddlRegisterType.SelectedIndex = 0;
                rbtnlACOtherSoc.SelectedItem.Text = "No";

                parameters.Clear();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.FillDropDown("App_PerformSociety_List", parameters.ToArray(), ddlPerformingSociety, "Performing Society");
                ddlPerformingSociety.SelectedIndex = 1;
                ddlPerformingSociety.Enabled = false;
                txtACIPInumber.Text = "";
                txtACIPInumber.Enabled = false;
                txtACFirstName.Text = "";
                txtACLastName.Text = "";
                txtPublisherName.Text = "";
                txtACPerformanceShare.Text = "";
                txtACMechanicalShare.Text = "";
                txtACSyncShare.Text = "";

                return RecordId;
            }
        }

        #endregion

        /// <summary>
        /// For WorkUpdateSongShare
        /// In this Function get share detail from Viewstate(ViewState["DBShareCal"]) asper that update roletype wise
        /// share percentage using Function(WorkUpdateShare) 
        /// Commented By Rohit
        /// </summary>
        /// <param name="intRecordId"></param>
        /// <returns>RecordId</returns>
        #region Get Song Share 
        protected Int64 WorkUpdateSongShare(Int64 intRecordId)
        {
            Int64 RecordId = 0;
            DataTable SDS = new DataTable();
            if (ViewState["DBShareCal"] != null)
                SDS = (DataTable)ViewState["DBShareCal"];

            for (int i = 0; i < SDS.Rows.Count; i++)
            {
                if (SDS.Rows[i]["RoleType"].ToString() == "A")
                {
                    if (lblZAShare.Text != "")
                    {
                        WorkUpdateShare(intRecordId, SDS.Rows[i]["RoleType"].ToString(), double.Parse(lblZAShare.Text));
                    }
                }
                else if (SDS.Rows[i]["RoleType"].ToString() == "C")
                {
                    if (lblZCShare.Text != "")
                    {
                        WorkUpdateShare(intRecordId, SDS.Rows[i]["RoleType"].ToString(), double.Parse(lblZCShare.Text));
                    }
                }
                else if (SDS.Rows[i]["RoleType"].ToString() == "CA")
                {
                    if (lblZCAShare.Text != "")
                    {
                        WorkUpdateShare(intRecordId, SDS.Rows[i]["RoleType"].ToString(), double.Parse(lblZCAShare.Text));
                    }
                }
                else
                {
                    if (lblZECLShare.Text != "")
                    {
                        WorkUpdateShare(intRecordId, SDS.Rows[i]["RoleType"].ToString(), double.Parse(lblZECLShare.Text));
                    }
                }
            }
            return RecordId;
        }

        #endregion

        /// <summary>
        /// For WorkUpdateShare
        /// In this function update Song share percentage in to Table(App_Accounts_SongRegistration) using 
        /// store procedure(App_Accounts_SongRegistration_Share_Update)
        /// Commented By Rohit
        /// </summary>
        /// <param name="intRecordId"></param>
        /// <param name="Role"></param>
        /// <param name="Share"></param>
        #region Update Song Share
        protected void WorkUpdateShare(Int64 intRecordId, string Role, double Share)
        {
            Int64 RecordId = 0;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@RoleType", Role, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@PerformanceShare",Share, SqlDbType.Float, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@MechanicalShare", Share, SqlDbType.Float, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            objDAL.ExecuteSP("App_Accounts_SongRegistration_Share_Update", parameters.ToArray(), out ReturnMessage, out RecordId);
        }

        #endregion

        /// <summary>
        /// For FalseWork_delete
        /// In this Function delete work registration using store procedure(App_Accounts_WorkRegistration_Delete) by prameter(WorknotificationId)
        /// and delete work file from server folder(MemberRegWorkDocs). bind data to Gridview(gvWork)
        /// Commented By Rohit
        /// </summary>
        /// <param name="WorknotificationId"></param>
        #region Delete Work Registration
        protected void FalseWork_delete(Int64 WorknotificationId)
        {
            ///* Code to Delete Employee Record */
           #region "EXECUTING DELETE PROCEDURE"
            Int64 RecordId;
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", WorknotificationId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;
            objDAL.ExecuteSP("App_Accounts_WorkRegistration_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);
            if (ReturnMessage == string.Empty)
            {
                // objGeneralFunction.BootBoxAlert("ERROR IN DELETING DATA. PLEASE CONTACT ADMINISTRATOR", this.Page);
            }
            else
            {
                DataTable DTBWorkNotification = new DataTable();
                if (ViewState["DTWorkNotification"] != null)
                    DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];
                if (RecordId > 0)
                {
                    DataRow myDataRow = DTBWorkNotification.Select("WorkNotificationId=" + WorknotificationId).FirstOrDefault(); // finds all rows with id==2 and selects first or null if haven't found any
                    if (myDataRow != null)
                    {
                        myDataRow.Delete();
                        string FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                        int fileDel = FileDelete("MWN_" + hdnRecordId.Value + "_" + WorknotificationId + "_", FilePath);
                    }
                }
                DTBWorkNotification.AcceptChanges();
                if (DTBWorkNotification != null)
                {
                    gvWork.DataSource = DTBWorkNotification;
                    gvWork.DataBind();
                }
                ViewState["DTWorkNotification"] = DTBWorkNotification;
                DTBWorkNotification.Dispose();
                objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
            }
            objDAL = null;
            #endregion "EXECUTING DELETE PROCEDURE"
        }

        #endregion

        /// <summary>
        /// For btnContinue_Click
        /// In this event cheak atleast one work registration and assign value to datatable(DTBWorkNotification) from ViewState(ViewState["DTWorkNotification"])
        /// and check validation asper roletype
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Continue Button Click
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            DataTable DTBWorkNotification = new DataTable();
            int intComposerCount = 0;
            int intLyricistCount = 0;
            int intPublisherCount = 0;
            string[] MemberType = hdnRoleTypes.Value.Split(',');
            if (gvWork.Rows.Count == 0 && hdnRegistrationType.Value!="LH")
            {
                objGeneralFunction.BootBoxAlert("Atleast 1 Work Notification Required", Page);
                return;
            }
            if (ViewState["DTWorkNotification"] != null)
            {
                DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];
            }
            if (hdnRegistrationType.Value != "LH")
            {
                if (DTBWorkNotification.Rows.Count > 0)
                {
                    for (int i = 0; i <= DTBWorkNotification.Rows.Count - 1; i++)
                    {
                        if (Array.Find(MemberType, element => element.Contains("M")) != null)
                        {
                            if (WorkMatches(DTBWorkNotification.Rows[i]["Author_Composer"].ToString(), hdnAccountName.Value, MemberType) != 0 || WorkMatches(DTBWorkNotification.Rows[i]["Author_Composer"].ToString(), txtAccountAlias.Text, MemberType) != 0)
                            {
                                intComposerCount++;
                            }
                        }
                        if (Array.Find(MemberType, element => element.Contains("L")) != null)
                        {
                            if (WorkMatches(DTBWorkNotification.Rows[i]["Author_Lyricist"].ToString(), hdnAccountName.Value, MemberType) != 0 || WorkMatches(DTBWorkNotification.Rows[i]["Author_Lyricist"].ToString(), txtAccountAlias.Text, MemberType) != 0)
                            {
                                intLyricistCount++;
                            }
                        }
                        if (Array.Find(MemberType, element => element.Contains("P")) != null)
                        {
                            if (WorkMatches(DTBWorkNotification.Rows[i]["Publisher"].ToString(), hdnAccountName.Value, MemberType) != 0 || WorkMatches(DTBWorkNotification.Rows[i]["Publisher"].ToString(), txtAccountAlias.Text, MemberType) != 0)
                            {
                                intPublisherCount++;
                            }
                        }
                    }
                }
                if (DTBWorkNotification.Rows.Count > 0)
                {
                    if (hdnRoleTypes.Value.Contains("M"))
                    {
                        if (intComposerCount == 0)
                        {
                            objGeneralFunction.BootBoxAlert("Atleast one Composer Should get matched with Alias Name ", this.Page);
                            return;
                        }
                    }
                    if (hdnRoleTypes.Value.Contains("L"))
                    {
                        if (intLyricistCount == 0)
                        {
                            objGeneralFunction.BootBoxAlert("Atleast one Lyricist Should get matched with Alias Name  ", this.Page);
                            return;
                        }
                    }
                    if (hdnRoleTypes.Value.Contains("P"))
                    {
                        if (intPublisherCount == 0)
                        {
                            objGeneralFunction.BootBoxAlert("Atleast one Publisher Name Should get matched with Alias  ", this.Page);
                            return;
                        }
                    }
                }
            }
            if (hdnRegistrationType.Value == "C" || hdnRegistrationType.Value == "NC")
            {
                wzMain.ActiveStepIndex = wzMain.ActiveStepIndex + 2;
            }
            else 
            { 
                wzMain.ActiveStepIndex = wzMain.ActiveStepIndex + 1+1;
            }
        }

        #endregion

        /// <summary>
        /// For btnAddWork_Click
        /// In this event check required control validation for work registration and Save work registration using Function(WorkNotificationSave)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region
        protected void btnAddWork_Click(object sender, EventArgs e)
        {
            bool flag = true;
            if (btnAddWork.Text == "Clear Song Selection")
            {
                DisplayZeroWorkDetails();
            }
            else
            {
                #region "Saving Work Notification"

                #region Control Validation

                if (hdnRecordId.Value == "0")
                {
                    objGeneralFunction.BootBoxAlert("UNABLE TO TRACK MASTER RECORD ID. PLEASE GO BACK AND START AGAIN", Page);
                    return;
                }
                DataTable DTBWorkNotification = new DataTable();
                if (ViewState["DTWorkNotification"] != null)
                {
                    DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];
                    if (DTBWorkNotification.Rows.Count > 10)
                    {
                        objGeneralFunction.BootBoxAlert("MINIMUM 10 WORKNOTIFICATION ALLOWED", this.Page);
                        return;

                    }
                }
                if (txtISRCNo.Enabled == true)
                {
                    if (txtISRCNo.Text == "")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE ENTER ISRC NO.", Page);
                        return;
                    }
                }
                if (txtSongName.Text == "")
                {
                    objGeneralFunction.BootBoxAlert("PLEASE ENTER SONG NAME", Page);
                    return;
                }
                if (txtFilm_AlbumName.Text == "")
                {
                    objGeneralFunction.BootBoxAlert("PLEASE ENTER FILM/ALBUM NAME", Page);
                    return;
                }
                if (ddlIntendedPurpose.SelectedIndex == 0)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT INTENDED PURPOSE", Page);
                    return;
                }

                string lang = ddlLanguage.GetSelectedText();
                if (lang.Length > 2)
                {
                    if (lang == "" || lang == "0")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE SELECT LANGUAGE", Page);
                        return;
                    }
                }
                else
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT LANGUAGE", Page);
                    return;
                }
                if (ddlVersion.SelectedIndex == 0)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT SONG VERSION", Page);
                    return;
                }
                if (ddlMusicRelation.SelectedIndex == 0)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT SONG MUSIC RELATION", Page);
                    return;
                }
                if (ddlWorkCategory.SelectedIndex == 0)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT SONG CATEGORY", Page);
                    return;
                }
                else
                {
                    if (ddlWorkSubCategory.Enabled == true)
                    {
                        if (ddlWorkSubCategory.SelectedIndex == 0)
                        {
                            objGeneralFunction.BootBoxAlert("PLEASE SELECT SONG CATEGORY  ( " + ddlWorkCategory.SelectedItem.ToString().ToUpper() + " )  SUB-CATEGORY", Page);
                            return;
                        }
                    }
                }
                if (txtArtistorMultipleSingers.InnerText == "")
                {
                    objGeneralFunction.BootBoxAlert("PLEASE ENTER SINGER NAME", Page);
                    return;
                }
                if (txtRelYear.Text == "")
                {
                    objGeneralFunction.BootBoxAlert("PLEASE ENTER SONG RELEASE YEAR", Page);
                    return;
                }
                if (txtDigitalLink.Text == "" && FPWork.HasFile == false)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE ENTER SONG DIGITAL LINK OR UPLOAD WORK FILE", Page);
                    return;
                }

                #region Code Not In Use
                //if (ddlWorkCategory.SelectedIndex == 0)
                //{
                //    objGeneralFunction.BootBoxAlert("PLEASE SELECT CATEGORY", this.Page);
                //    return;
                //}
                //string Type = string.Empty;
                //foreach (ListItem item in cbxRollType.Items)
                //{
                //    if (item.Selected == true)
                //    {
                //        if (item.Text.ToUpper().Contains("MUSIC"))
                //        {
                //            Type += ",M";
                //        }
                //        if (item.Text.ToUpper().Contains("LYRIC"))
                //        {
                //            Type += ",L";
                //        }
                //        if (item.Text.ToUpper().Contains("PUBLISHER"))
                //        {
                //            Type += ",P";
                //        }
                //    }
                //}
                //Type = Type.TrimEnd(',').TrimStart(',');
                #endregion

                string[] MemberType = hdnRoleTypes.Value.Split(',');
                if (MemberType.Length == 3)
                {
                    //for zerowrok
                    //if (WorkMatches(txtAuthorMusicComposer.Text, txtAccountAlias.Text, MemberType) == 0
                    //        && WorkMatches(txtAuthorLyricist.Text, txtAccountAlias.Text, MemberType) == 0
                    //         && WorkMatches(txtPublisher.Text, txtAccountAlias.Text, MemberType) == 0
                    //        )
                    //{
                    //    objGeneralFunction.BootBoxAlert("Account Alias Name Should get matched with either Composer or Lyricist or Publisher ", this.Page);
                    //    return;
                    //}
                }
                else if (MemberType.Length == 2)
                {
                    string value = Array.Find(MemberType, element => element.EndsWith("M", StringComparison.Ordinal));
                    if (Array.Find(MemberType, element => element.Contains("M")) != null && Array.Find(MemberType, element => element.Contains("L")) != null)
                    {
                        //for zerowrok
                        //if (WorkMatches(txtAuthorMusicComposer.Text, txtAccountAlias.Text, MemberType) == 0
                        //    && WorkMatches(txtAuthorLyricist.Text, txtAccountAlias.Text, MemberType) == 0
                        //    )
                        //{
                        //    objGeneralFunction.BootBoxAlert("Composer - Lyricist any one should get matched with the Alias name ", this.Page);
                        //    return;
                        //}
                    }
                    if (Array.Find(MemberType, element => element.Contains("M")) != null && Array.Find(MemberType, element => element.Contains("P")) != null)
                    {
                        //for zerowrok
                        //if (WorkMatches(txtAuthorMusicComposer.Text, txtAccountAlias.Text, MemberType) == 0
                        //   || WorkMatches(txtPublisher.Text, txtAccountAlias.Text, MemberType) == 0
                        //   )
                        //{
                        //    objGeneralFunction.BootBoxAlert("Composer or Publisher  one of them should get matched with the Alias name ", this.Page);
                        //    return;
                        //}
                    }
                    if (Array.Find(MemberType, element => element.Contains("L")) != null && Array.Find(MemberType, element => element.Contains("P")) != null)
                    {
                        //for zerowrok
                        //if (WorkMatches(txtAuthorLyricist.Text, txtAccountAlias.Text, MemberType) == 0
                        //  && WorkMatches(txtPublisher.Text, txtAccountAlias.Text, MemberType) == 0
                        //  )
                        //{
                        //    objGeneralFunction.BootBoxAlert("Lyricist or Publisher  one of them should get matched with the Alias name", this.Page);
                        //    return;
                        //}
                    }
                }
                else if (MemberType.Length == 1)
                {
                    if (MemberType[0].Contains("M"))
                    {
                        //for zerowrok
                        //if (WorkMatches(txtAuthorMusicComposer.Text, txtAccountAlias.Text, MemberType) == 0)
                        //{
                        //    objGeneralFunction.BootBoxAlert("Composer Should get matched with  Alias Name ", this.Page);
                        //    return;
                        //}
                    }
                    if (MemberType[0].Contains("L"))
                    {
                        //for zerowrok
                        //if (WorkMatches(txtAuthorLyricist.Text, txtAccountAlias.Text, MemberType) == 0)
                        //{
                        //    objGeneralFunction.BootBoxAlert("Lyricist Should get matched with Alias Name  ", this.Page);
                        //    return;
                        //}
                    }
                    if (MemberType[0].Contains("P"))
                    {
                        //for zerowrok
                        //if (WorkMatches(txtPublisher.Text, txtAccountAlias.Text, MemberType) == 0)
                        //{
                        //    objGeneralFunction.BootBoxAlert("Publisher Name Should get matched with Alias  ", this.Page);
                        //    return;
                        //}
                    }
                }

                if (FPWork.HasFile)
                {
                    HttpPostedFile PFile = FPWork.PostedFile;
                    if (!(PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                            || PFile.ContentType == "application/pdf"
                            || PFile.ContentType == "image/gif"))
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png, pdf", this.Page);
                        return;
                    }

                    if (CheckFileSizeLimit(FPWork, 5000) == 1)
                    {
                        objGeneralFunction.BootBoxAlert("FILE SIZE SHOULD BE LESS THAN 5MB", Page);
                        return;
                    }
                }

                #endregion

                Int64 RecordId_Work = WorkNotificationSave(Convert.ToInt64(hdnRecordId.Value));

                if (RecordId_Work == 0)
                {
                    objGeneralFunction.BootBoxAlert("Failed To Save Work Notification. Try Again Or Contact Adminstrator !!", Page);
                    return;
                }
                else
                {
                    btnAddWork.Enabled = false;
                    btnAddWork.Text = "Add New Song Details";
                }

                #endregion
            }
        }

        #endregion

        /// <summary>
        /// For btnAddAuthComp_Click
        ///  In this event check required control validation for Song registration and Save Song registration using Function(WorkSongSave)
        ///  using store procedure(App_Accounts_Song_Share_CL) calulate Share percentage if share percentage is equal to 100 then 
        ///  update recordstatus Table(App_Accounts_SongRegistration) using store procedure(App_Accounts_WorkRegistration_Update)
        ///  Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Button Add Author/Composer/Publisher
        protected void btnAddAuthComp_Click(object sender, EventArgs e)
        {
            if (btnAddAuthComp.Text == "Add Auther/Composer")
            {
                bool flag = true;
                #region "Saving Zero Work Notification"

                #region Control Validation

                if (hdnRecordId.Value == "0")
                {
                    objGeneralFunction.BootBoxAlert("UNABLE TO TRACK MASTER RECORD ID. PLEASE GO BACK AND START AGAIN", Page);
                    return;
                }
                if (ddlRegisterType.SelectedIndex == 0)
                {
                    objGeneralFunction.BootBoxAlert("PLEASE SELECT REGISTERATION TYPE", Page);
                    return;
                }
                if (rbtnlACOtherSoc.SelectedValue == "Y")
                {
                    if (ddlPerformingSociety.SelectedIndex == 0)
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE SELECT PERFORMING SOCIETY NAME", Page);
                        return;
                    }
                    if (txtACIPInumber.Text == "")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE ENTER IPI NUMBER", Page);
                        return;
                    }
                    else
                    {
                        string IPILEN = txtACIPInumber.Text;
                        if (IPILEN.Length != 11)
                        {
                            objGeneralFunction.BootBoxAlert("IPI Number must be equal to 11 Number Digit", Page);
                            return;
                        }
                    }
                }
                if (txtACFirstName.Visible == true)
                {
                    if (txtACFirstName.Text == "")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE ENTER FIRST NAME", Page);
                        return;
                    }
                }
                if (txtACLastName.Visible == true)
                {
                    if (txtACLastName.Text == "")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE ENTER LAST NAME", Page);
                        return;
                    }
                }
                if (txtPublisherName.Visible == true)
                {
                    if (txtPublisherName.Text == "")
                    {
                        objGeneralFunction.BootBoxAlert("PLEASE ENTER PUBLISHER NAME", Page);
                        return;
                    }
                }

                #endregion

                Int64 RecordId_Work = WorkSongSave(Convert.ToInt64(HdnZeroworkID.Value));

                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                string ReturnMessage = string.Empty;
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                DataSet SDS = new DataSet();
                SDS = objDAL.GetDataSet("App_Accounts_Song_Share_CL", parameters.ToArray());
                double share = double.Parse(SDS.Tables[0].Rows[0]["Share"].ToString());
                if (share >= 100)
                {
                    parameters.Clear(); 
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                    objDAL.ExecuteScalar("App_Accounts_WorkRegistration_Update", parameters.ToArray());
                    btnAddAuthComp.Enabled = true;
                    btnCmpSongDetail.Enabled = true;
                }
                #endregion
            }
            else
            {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.FillDropDown("App_PerformSociety_List", parameters.ToArray(), ddlPerformingSociety, "Performing Society");
                ddlPerformingSociety.SelectedIndex = 1;
                ddlPerformingSociety.Enabled = false;
                txtACIPInumber.Text = "";
                txtACIPInumber.Enabled = false;
                txtACFirstName.Text = "";
                txtACLastName.Text = "";
                txtPublisherName.Text = "";
                txtACPerformanceShare.Text = "";
                txtACMechanicalShare.Text = "";
                txtACSyncShare.Text = "";
                btnAddAuthComp.Text = "Add Auther/Composer";
            }
        }

        #endregion

        /// <summary>
        /// For WorkMatches
        /// this function check member every registered work with his/her own record as per role type.
        /// member must be registerd with every work if is it not then return 0  
        /// Commented By Rohit
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ALias"></param>
        /// <param name="Type"></param>
        /// <returns>returnCount</returns>
        #region Work Matcheing
        protected int WorkMatches(string text, string ALias, string[] Type)
        {
            string fullName = string.Empty;
            if (hdnRegistrationType.Value == "C")
                fullName = txtcompanyName.Text;
            else
            {
                fullName = hdnAccountName.Value;
            }
            text = text.ToUpper();
            ALias = ALias.ToUpper();
            if (ALias.Contains(";"))
            {
                ALias = ALias.Replace(";", ",") + "," + fullName;
            }
            else if (ALias.Contains(","))
            {
                ALias = ALias + "," + fullName;
            }
            else
            {
                if (ALias == string.Empty)
                    ALias = fullName;
                else
                    ALias = ALias + "," + fullName;
            }
            int returnCount = 0;
            if (text.Contains(","))
            {
                string[] para = text.Split(',');
                string[] Alias = ALias.Split(',');
                returnCount = para.Intersect(Alias).Count();
            }
            else
            {
                if (text != "")
                {
                    if (ALias.Contains(text))
                        returnCount = 1;
                }
            }
            return returnCount;
        }

        #endregion

        /// <summary>
        /// For gvWork_RowDataBound
        /// bind the control to gridview(gvWork) for creating rows for member work details.
        /// the controls are as LinkButton(lnk2(btnWorkDelete),lnk3(btnWorkSelect)), ImageButton(img2(imgtWorkfile))
        /// and HyperLink(hypworkfile(hypworkfile)).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region
        protected void gvWork_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hypworkfile = (HyperLink)e.Row.FindControl("hypworkfile");
                HiddenField hdnWorkNotificationId = (HiddenField)e.Row.FindControl("hdnWorkNotificationId");
                LinkButton lnk2 = (LinkButton)e.Row.FindControl("btnWorkDelete");
                LinkButton lnk3 = (LinkButton)e.Row.FindControl("btnWorkSelect");
                ImageButton img2 = (ImageButton)e.Row.FindControl("imgtWorkfile");
                if (hdnRegistrationType.Value == "LH")
                {
                    lnk2.Enabled = false;
                    lnk2.Attributes.Add("disabled", "true");
                    img2.Enabled = false;
                    if (lnk2.OnClientClick != null)
                    {
                        lnk2.OnClientClick = null;
                    }
                }
                if (txtSongName.Text != "")
                {
                    lnk3.Enabled = false;
                    lnk3.Attributes.Add("disabled", "true");
                    if (lnk3.OnClientClick != null)
                    {
                        lnk3.OnClientClick = null;
                    }
                }
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].ToolTip = gvWork.HeaderRow.Cells[i].Text;
                }
                string filename = "";
                filename = Getfile_Folder("MWN_" + hdnRecordId.Value + "_" + hdnWorkNotificationId.Value + "_", "~/MemberRegWorkDocs/");
                hypworkfile.NavigateUrl = "~/MemberRegWorkDocs/" + filename;
                if (filename == string.Empty)
                    hypworkfile.ToolTip = "File Not Uploaded";
            }
            //ToolTip = '<%#Eval("Workfile").ToString()==""?"File Not Uploaded":"" %>' Target = "_blank" NavigateUrl = '<%#Eval("Workfile") %>'
        }

        #endregion

        #region Event Not in use
        protected void gvAuthComp_RowDataBound(object sender, GridViewRowEventArgs e)
        { 

        }

        #endregion

        /// <summary>
        /// For grdDocumentsPreApproval_RowDataBound
        /// In this event bound document list and compulsory document having * for mandatory to gridview(grdDocumentsPreApproval)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Create Document List
        protected void grdDocumentsPreApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDocumentName = (Label)e.Row.FindControl("lblDocumentName");
                HiddenField hdnIsCompulsary = (HiddenField)e.Row.FindControl("hdnIsCompulsary");
                if (hdnIsCompulsary.Value == "0")
                {
                    lblDocumentName.Text = lblDocumentName.Text + "*";
                    //lblDocumentName.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        #endregion

        /// <summary>
        /// For btnfileupload_Click
        /// In this event define controls as Button(btnUpload),FileUpload(FileUploadControl(FileUpload1)),Label(LblFilecount(lblSrno),
        /// lblfooterErrorMsg(lblfooterErrorMsg),lblSrno(lblSrno)). After check File size is Less than 5 MB or not, validate filename with Extension
        /// and length. save finle in Server Folder(MemberRegDocs) with file name("MRU_" + hdnRecordId.Value). insert/update record by using Function(SaveDocs_DB).
        /// bind dataset to ViewState(DocLookUp) and count the document list which is uploded. bind document list using Function(Display_AccountsDoc_ListData)
        /// 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region document upload and bind to gridview
        protected void btnfileupload_Click(object sender, EventArgs e)
        {
            Button btnUpload = (Button)sender;
            GridViewRow gvr = (GridViewRow)btnUpload.NamingContainer;
            FileUpload FileUploadControl = (FileUpload)gvr.FindControl("FileUpload1");
            HiddenField hdnDocLookupId = (HiddenField)gvr.FindControl("hdnDocLookupId");
            // HiddenField hdnUploadedCount = (HiddenField)gvr.FindControl("hdnUploadedCount");
            Label LblFilecount = (Label)gvr.FindControl("lblSrno");
            DataTable DT = new DataTable();
            Label lblfooterErrorMsg = (Label)grdDocumentsPreApproval.FooterRow.FindControl("lblfooterErrorMsg");
            Label lblSrno = (Label)gvr.FindControl("lblSrno");
            if (lblfooterErrorMsg != null)
                lblfooterErrorMsg.Text = "";
            //int uploadCount = 0;
            //if (hdnUploadedCount.Value != "")
            //    uploadCount = Convert.ToInt16(hdnUploadedCount.Value);
            if (FileUploadControl.HasFiles)
            {
                int i = 0;
                foreach (HttpPostedFile uploadedFile in FileUploadControl.PostedFiles)
                {
                    HttpPostedFile PFile = uploadedFile;
                    int filesize = PFile.ContentLength; // Get File size
                    if (filesize / 1024 > 5000) // Check whether file size is less than 5MB
                    {
                        lblfooterErrorMsg.ForeColor = Color.Red;
                        lblfooterErrorMsg.Text += "SrNo : " + lblSrno.Text + ", " + "File Size should be less than 5MB. </br> ";
                        break;
                    }
                    try
                    {
                        string strFileName = string.Empty;
                        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        string fileExt = Path.GetExtension(PFile.FileName);
                        //Added by RENU on 11/12/2020                        
                        filename = Regex.Replace(filename, @"\s+", string.Empty);
                        filename = RemoveSpecialCharacters(filename);
                        Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                        bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                        if (!isValidateFilename)
                        {
                            break;
                        }
                        if (filename.Length > 40)
                        {
                            filename = filename.Substring(0, 40);
                        }
                        // 
                        string SaveAsFileName = "MRU_" + hdnRecordId.Value + "_" + hdnDocLookupId.Value + "_N" + (i + 1).ToString() + "_" + filename + fileExt;
                        strFileName = Server.MapPath("~/MemberRegDocs/" + SaveAsFileName);
                        uploadedFile.SaveAs(strFileName);
                        Int64 ReturnId = SaveDocs_DB(0, hdnDocLookupId.Value, filename, SaveAsFileName);
                        DataTable DTLookUp = (DataTable)ViewState["DocLookUp"];
                        if (DTLookUp != null && ReturnId > 0)
                        {
                            DataRow[] DocRow = DTLookUp.Select("DocumentLookupId = " + hdnDocLookupId.Value + "");
                            if (DocRow.Length > 0)
                            {
                                if (DocRow[0]["Uploaded"].ToString() != "")
                                {
                                    DocRow[0]["Uploaded"] = 1;
                                    DocRow[0]["UploadedCount"] = DocRow[0]["UploadedCount"].ToString() != "" ? Convert.ToDouble(DocRow[0]["UploadedCount"].ToString()) + 1 : 0;
                                    LblFilecount.Text = DocRow[0]["UploadedCount"].ToString();
                                }
                            }
                        }
                        objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                        //}
                        //else
                        //{
                        //    objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png, pdf", this.Page);
                        //}
                    }
                    catch (Exception ex)
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
                    }
                    i++;
                }
                #region Code not in use
                //for (int i = 0; i < FileUploadControl.PostedFiles.Count; i++)
                //{
                //    HttpPostedFile PFile = FileUploadControl.PostedFiles[i];
                //    int filesize = PFile.ContentLength; // Get File size
                //    if (filesize / 1024 > 5000) // Check whether file size is less than 5MB
                //    {
                //        lblfooterErrorMsg.ForeColor = Color.Red;
                //        lblfooterErrorMsg.Text += "SrNo : " + lblSrno.Text + ", " + "File Size should be less than 5MB. </br> ";
                //        break;
                //    }
                //    try
                //    {
                //        string strFileName = string.Empty;
                //        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                //        string fileExt = Path.GetExtension(PFile.FileName);
                //        //Added by RENU on 11/12/2020                        
                //        filename = Regex.Replace(filename, @"\s+", string.Empty);
                //        filename = RemoveSpecialCharacters(filename);
                //        Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                //        bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                //        if (!isValidateFilename)
                //        {
                //            break;
                //        }
                //        if (filename.Length > 40)
                //        {
                //            filename = filename.Substring(0, 40);
                //        }
                //        // 
                //        string SaveAsFileName = "MRU_" + hdnRecordId.Value + "_" + hdnDocLookupId.Value + "_N" + (i + 1).ToString() + "_" + filename + fileExt;
                //        strFileName = Server.MapPath("~/MemberRegDocs/" + SaveAsFileName);
                //        FileUploadControl.SaveAs(strFileName);
                //        Int64 ReturnId = SaveDocs_DB(0, hdnDocLookupId.Value, filename, SaveAsFileName);
                //        DataTable DTLookUp = (DataTable)ViewState["DocLookUp"];
                //        if (DTLookUp != null && ReturnId > 0)
                //        {
                //            DataRow[] DocRow = DTLookUp.Select("DocumentLookupId = " + hdnDocLookupId.Value + "");
                //            if (DocRow.Length > 0)
                //            {
                //                if (DocRow[0]["Uploaded"].ToString() != "")
                //                {
                //                    DocRow[0]["Uploaded"] = 1;
                //                    DocRow[0]["UploadedCount"] = DocRow[0]["UploadedCount"].ToString() != "" ? Convert.ToDouble(DocRow[0]["UploadedCount"].ToString()) + 1 : 0;
                //                    LblFilecount.Text = DocRow[0]["UploadedCount"].ToString();
                //                }
                //            }
                //        }
                //        objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                //        //}
                //        //else
                //        //{
                //        //    objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png, pdf", this.Page);
                //        //}
                //    }
                //    catch (Exception ex)
                //    {
                //        objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
                //    }
                //}
                #endregion
            }
            else
            {
                objGeneralFunction.BootBoxAlert("Please Upload  file", this.Page);
            }
            Display_AccountsDoc_ListData(hdnDocLookupId.Value);
        }

        #endregion

        /// <summary>
        /// For ibtFileDelete_Click
        /// In this event delete document record using store procedure(App_Accounts_Doc_Delete_IPM) and delete file using Function(FileDelete)
        /// display and validate document list using Function(Display_AccountsDoc_ListData,loadGrdDocumentsPreApproval).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region delete document 
        protected void ibtFileDelete_Click(object sender, EventArgs e)
        {
            LinkButton ibtFileDelete = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)ibtFileDelete.NamingContainer;
            GridView GVfiles = (GridView)gvr.Parent.NamingContainer;
            HiddenField hdnAccountDocId = (HiddenField)gvr.FindControl("hdnAccountDocId");
            HiddenField hdnFileName = (HiddenField)gvr.FindControl("hdnFileName");
            HiddenField hdnDocumentLookupId = (HiddenField)gvr.FindControl("hdnDocumentLookupId");
            string ReturnMessage = string.Empty;
            Int64 DelRecordId = 0;
            try
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountDocId", hdnAccountDocId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                objDAL.ExecuteSP("App_Accounts_Doc_Delete_IPM", parameters.ToArray(), out ReturnMessage, out DelRecordId);
                if (DelRecordId > 0)
                {
                    string FilePath = Server.MapPath("~/MemberRegDocs/");
                    int fileDel = FileDelete(hdnFileName.Value, FilePath);
                    Display_AccountsDoc_ListData(hdnDocumentLookupId.Value);
                    loadGrdDocumentsPreApproval();
                }
            }
            catch (Exception ex)
            {
            }
            modViewFiles.Show();
        }
        #endregion

        /// <summary>
        /// For FileDelete
        /// In this Function Check File with(MPU_AccountID_)  In folder(MemberPhoto)
        /// if file exists then delete it and return(FileExistStatus) to event(btnPhotoUpload_Click)
        /// Commented By Rohit
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <returns>FileExistStatus</returns>
        #region Exists File Delete
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

        /// <summary>
        /// For FileExist
        /// in this function validate file exits in folder(MemberPhoto) or not if file does not exist then dispaly massage
        /// Commented By Rohit
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <param name="StrFileName"></param>
        /// <returns></returns>
        #region Check Account Image is exists 
        protected int FileExist(string FileName, string FilePath, out string StrFileName)
        {
            StrFileName = "";
            int FileExistStatus = 0;
            try
            {
                var query = from o in Directory.GetFiles(FilePath, "*.*")
                            let x = new FileInfo(o)
                            where x.FullName.ToUpper().Contains(FileName.ToUpper())
                            select o;

                foreach (var item in query)
                {
                    StrFileName = Path.GetFileName(item);
                    FileExistStatus = 1;
                }
            }
            catch (Exception ex)
            {
            }
            return FileExistStatus;

            //foreach (var item in query)
            //{
            //    if (File.Exists(Filename))
            //    {
            //        data = data + "update App_Accounts Set AccountImage='" + item.Replace("D:\\Sharing\\IPRS_Member\\IPRS_Member\\IPRS_Member\\MemberPhoto\\", "") + "' where Accountid=" + item.Replace("D:\\Sharing\\IPRS_Member\\IPRS_Member\\IPRS_Member\\MemberPhoto\\", "").Split('_')[1] + Environment.NewLine;

            //    }

            //    StrFileName = Path.GetFileName(item);
            //    FileExistStatus = 1;
            //}
        }

        #endregion

        /// <summary>
        /// For SaveDocs_DB
        ///  Insert/Update document record into Table(App_Accounts_Doc). here @AccountDocId = 0 then record Inserted
        ///  other wise update record using store procedure(App_Accounts_Doc_Manage_IPM) and return Document ID
        ///  Commented By Rohit
        /// </summary>
        /// <param name="DocId"></param>
        /// <param name="DocLookUpId"></param>
        /// <param name="DocCaption"></param>
        /// <param name="DocFileName"></param>
        /// <returns>RecordId</returns>
        #region Save Document Details
        protected Int64 SaveDocs_DB(Int64 DocId, string DocLookUpId, string DocCaption, string DocFileName)
        {
            var parameters = new List<SqlParameter>();
            string ReturnMessage = string.Empty;
            Int64 RecordId = 0;
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountDocId", DocId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", DocLookUpId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ApprovalType", "A", SqlDbType.NVarChar, 5, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocStatus", 0, SqlDbType.TinyInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentCaption", DocCaption, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocFileName", DocFileName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocUploadDesc", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            objDAL.ExecuteSP("App_Accounts_Doc_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
            if (RecordId == 0 || ReturnMessage.Contains("RECORD ALREADY EXIST"))
            {
                RecordId = 0; ;
                return RecordId;
            }
            return RecordId;
        }

        #endregion

        /// <summary>
        /// for Display_AccountsDoc_ListData
        /// get document list using store procedure(App_Accounts_Doc_ListData_IPM) and bind data to GridView(grdViewUploadedFiles)
        /// Commented By Rohit
        /// </summary>
        /// <param name="DocId"></param>
        #region Bind Document list to Girdview
        private void Display_AccountsDoc_ListData(string DocId)
        {
            #region Bind Grid Documents
            try
            {
                int tFlag = 4;
                if (hdnRegistrationType.Value == "C")
                {
                    if (rbtEntityType.SelectedValue == "SP")
                    {
                        tFlag = 0;
                    }
                    else if (rbtEntityType.SelectedValue == "PR")
                    {
                        tFlag = 1;
                    }
                    else if (rbtEntityType.SelectedValue  == "CP")
                    {
                        tFlag = 2;
                    }
                }
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", DocId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@tFlag", tFlag, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                DataSet MyDataSet = new DataSet();
                MyDataSet = objDAL.GetDataSet("App_Accounts_Doc_ListData_IPM", parameters.ToArray());
                if (MyDataSet != null)
                {
                    grdViewUploadedFiles.DataSource = MyDataSet.Tables[0];
                    grdViewUploadedFiles.DataBind();
                }
                objDAL = null;
                MyDataSet.Dispose();
                #endregion Bind Grid WorkNotification
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        /// <summary>
        /// For btnViewUploadedFiles_Click
        /// get document list using Function(Display_AccountsDoc_ListData) and display Document list using Ajax Control(modViewFiles) 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region View Uploaded Doument
        protected void btnViewUploadedFiles_Click(object sender, EventArgs e)
        {
            lblHeadingofFiles.Text = "Uploaded Files";
            LinkButton btnViewUploadedFiles = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btnViewUploadedFiles.NamingContainer;
            HiddenField hdnDocLookupId = (HiddenField)gvr.FindControl("hdnDocLookupId");
            Display_AccountsDoc_ListData(hdnDocLookupId.Value);
            modViewFiles.Show();
        }

        #endregion

        /// <summary>
        /// For SaveWorkFile
        /// In this function save members work file in server folder(MemberRegWorkDocs) with file name("MWN_" + hdnRecordId.Value)
        /// Commented By Rohit
        /// </summary>
        /// <param name="FPWork"></param>
        /// <param name="WorkId"></param>
        /// <returns></returns>
        #region Save Work File 
        protected string[] SaveWorkFile(FileUpload FPWork, Int64 WorkId)
        {
            HttpPostedFile PFile = FPWork.PostedFile;
            //string strFileName = string.Empty;
            string[] strFileName = new string[2];
            try
            {
                if (FPWork.HasFile == true)
                {
                    //if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                    //    || PFile.ContentType == "application/pdf" || PFile.ContentType == "application/vnd.ms-excel"
                    //    || PFile.ContentType == "image/gif" || PFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    //{
                    string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                    string fileExt = Path.GetExtension(PFile.FileName);
                    //Added by RENU on 11/12/2020
                    filename = Regex.Replace(filename, @"\s+", string.Empty);
                    filename = RemoveSpecialCharacters(filename);
                    Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                    bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                    if (!isValidateFilename)
                    {
                        strFileName[0] = string.Empty;
                        strFileName[1] = string.Empty;
                        objGeneralFunction.BootBoxAlert("Upload status: Error getting file", this.Page);
                        return strFileName;
                    }
                    if (filename.Length > 40)
                    {
                        filename = filename.Substring(0, 40);
                    }
                    string SaveAsFileName = "MWN_" + hdnRecordId.Value + "_" + WorkId + "_" + filename + fileExt;
                    strFileName[1] = SaveAsFileName;
                    FileDelete("MWN_" + hdnRecordId.Value + "_" + WorkId + "_", Server.MapPath("~/MemberRegWorkDocs/"));
                    strFileName[0] = Server.MapPath("~/MemberRegWorkDocs/" + SaveAsFileName);
                    PFile.SaveAs(strFileName[0]);  //D:\DreamSoft Renu\SVN\IPRS_KYM\IPRS_KYM\IPRS_Member\MemberRegWorkDocs\MWN_30894_10029_MWN_3165_2736_She can on soundcloud.pdf
                    objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);

                    //}
                    //else
                    //{
                    //    strFileName = string.Empty;
                    //    objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png, pdf", this.Page);
                    //}
                }
                else
                {
                    strFileName[0] = string.Empty;
                    strFileName[1] = string.Empty;
                    //objGeneralFunction.BootBoxAlert("Upload status: Error getting file", this.Page);
                }
            }
            catch (Exception ex)
            {
                strFileName[0] = string.Empty;
                strFileName[1] = string.Empty;
                objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
            }
            return strFileName;
        }

        #endregion

        /// <summary>
        /// For RemoveSpecialCharacters
        /// In this Function check any special character in Filename and retrun(sb) to event(btnPhotoUpload_Click)
        /// Commented By Rohit
        /// </summary>
        /// <param name="str"></param>
        /// <returns>sb</returns>
        #region Remove Special Character
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
        #endregion

        /// <summary>
        /// For CheckFileSizeLimit
        /// In this function Check File Size and comapair to allow to maximum size.
        /// Commented By Rohit
        /// </summary>
        /// <param name="FPWork"></param>
        /// <param name="MaxSizeKB"></param>
        /// <returns></returns>
        #region Check File Size
        protected Int64 CheckFileSizeLimit(FileUpload FPWork, int MaxSizeKB)
        {
            try
            {
                decimal fileSize = 0;
                fileSize = (FPWork.PostedFile.InputStream).Length / 1024;
                if (fileSize > MaxSizeKB)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        /// <summary>
        /// For Getfile_Folder
        /// using this function from server folder(MemberRegWorkDocs) and return file name
        /// Commented By Rohit
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <returns>MemberRegWorkDocs</returns>
        #region Get file
        protected string Getfile_Folder(string FileName, string FilePath)
        {
            string outputFileName = "";
            try
            {
                FilePath = Server.MapPath("~/MemberRegWorkDocs/");

                var query = from o in Directory.GetFiles(FilePath, "*.*")
                            let x = new FileInfo(o)
                            where x.FullName.ToUpper().Contains(FileName.ToUpper())
                            select o;

                foreach (var item in query)
                {
                    outputFileName = Path.GetFileName(item.ToString());
                }
            }
            catch (Exception ex)
            {
            }
            return outputFileName;
        }

        #endregion

        /// <summary>
        /// For btnSubmitApplication_Click
        /// In this event firstly check all previous step are done or not by using store procedure(App_Accounts_ValidateSubmisson_IPM) and check 
        /// application status is 0 or null. if its null then update application status = 1 till which payment not done successfully. if payment
        /// done successfully then update application status =0 by using store procedure(App_Accounts_AppStatus_Update_IPM). 
        /// after get previous payment list from using store procedure(App_Accounts_RegPayment_List_IPM) if Record = 0 then
        /// Insert/Update payment detail by using Function(SavePaymentLog) and get datarow of TransactionNo and PaymentRecieptId.
        /// complete insert/update then create Hash table for and asign value which required for payment procedure. if Textbox(txtMemberShipAmt) have value more than 0
        /// then redirect to Page(paymentrequest.aspx) otherwise config mail using Function(UpdatePaymentSubmission_CreateEmailLog) and send to Member EmailId
        /// and redirect to Page(Response.aspx) otherwise Insert/Update payment detail by using Function(SavePaymentLog) and get pervious payment 
        /// status using Function(VerifyPayment) after that changes in datatable asper new data and asign to viewstate(DT_Payment) if payment status is success 
        /// then button(bntMakePayment) visible false otherwise display messgae like Your Last Transaction Was Not Successful and button(bntMakePayment) visible True
        /// Commented By Rohit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Submit Application 
        protected void btnSubmitApplication_Click(object sender, ImageClickEventArgs e)
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters1 = new List<SqlParameter>();
            DataTable DTValid = new DataTable();
            if (hdnRegistrationType.Value =="LH")
            {
                parameters1.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRefAccountId.Value  , SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            else
            { 
                parameters1.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            }
            DTValid = objDAL.GetDataTable("App_Accounts_ValidateSubmisson_IPM", parameters1.ToArray());
            if (DTValid.Rows.Count > 0)
            {
                string ValidMsg = "";
                DataRow[] DR = DTValid.Select("Rec_Count=0");
                if (DR.Length > 0)
                {
                    ValidMsg = "";
                    for (int i = 0; i < DR.Length; i++)
                    {
                        ValidMsg += DR[i]["Name"].ToString() + ",";
                    }
                }
                ValidMsg = ValidMsg.TrimEnd(',');
                if (ValidMsg != string.Empty)
                {
                    objGeneralFunction.BootBoxAlert("Some Details Are missing in " + ValidMsg, this.Page);
                    return;
                }
            }
            if (hdnApplicationStatus.Value == "0")
            {
                var parameters = new List<SqlParameter>();
                string ReturnMessage = string.Empty;
                Int64 RecordId = 0;
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ApplicationStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));
                objDAL.ExecuteSP("App_Accounts_AppStatus_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    Session["ApplicationStatus"] = 1;
                    CreateEmailLog(hdnRecordId.Value);
                    Response.Redirect("Information");
                    return;
                }
            }
            if (hdnApplicationStatus.Value == "")
            {
                bntMakePayment.Visible = true;
                bntMakePayment.Enabled = true;
                string firstname = Convert.ToString(Session["AccountName"]).Trim();
                string email = Convert.ToString(Session["LoginName"]).Trim();
                DataTable DT = new DataTable();
                DT = objDAL.GetDataTableSql("App_Accounts_RegPayment_List_IPM " + hdnRecordId.Value);
                if (DT.Rows.Count == 0)
                {
                    if (txtMemberShipAmt.Text == string.Empty)
                    {
                        objGeneralFunction.BootBoxAlert("Error fetching Amount please contact Administrator", this.Page);
                        return;
                    }
                    hdnPaymentRecieptId.Value = "0";
                    DataRow DR = DT.NewRow();
                    DR["PaymentRecieptId"] = 0;
                    DR["AccountId"] = hdnRecordId.Value;
                    DR["TransactionNo"] = DateTime.Now.ToString("yyMMddHHmmss");
                    DR["PaymentAmount"] = Convert.ToDouble(txtMemberShipAmt.Text);
                    DR["PaidAmount"] = "0";
                    DR["PaymentStatus"] = "1";
                    DR["PaymentGatewayResponse"] = "";
                    DR["ResponseNo"] = "";
                    DR["ResponseString"] = "";
                    DR["LastTransactionNo"] = "";
                    DR["LastResponseNo"] = "";
                    DR["LastResponseString"] = "";
                    DR["PaymentBankName"] = "";
                    if (Convert.ToDouble(txtMemberShipAmt.Text) == 0)
                    {
                        DR["PaymentStatus"] = "0";
                        DR["PaymentGatewayResponse"] = "Free Membership on account of covid";
                        DR["ResponseString"] = "";
                    }
                    Int64 RecordId = 0;
                    string ReturnMessage = "";

                    //added by Hariom 19-04-2023
                    //if (DT.Rows[4]["PaymentStatus"].ToString() == "1")
                    //{
                    //    objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                    //}
                    ////end

                    objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                    if (RecordId == 0)
                    {
                        objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
                    }
                    else
                    {
                        Hashtable HST_Pay = new Hashtable();
                        HST_Pay["Amt"] = txtMemberShipAmt.Text;
                        HST_Pay["rid"] = RecordId.ToString();
                        HST_Pay["fname"] = firstname;
                        HST_Pay["email"] = email;
                        HST_Pay["phone"] = txtMobile.Text;
                        HST_Pay["TransactionNo"] = DR["TransactionNo"].ToString();
                        HST_Pay["PaymentRecieptId"] = DR["PaymentRecieptId"].ToString();
                        HST_Pay["AccountId"] = hdnRecordId.Value;
                        Session["PayValues"] = HST_Pay;
                        if (Convert.ToDouble(txtMemberShipAmt.Text) > 0)
                            Response.Redirect("paymentrequest.aspx");
                        else
                        {
                            RecordId = objGeneralFunction.UpdatePaymentSubmission_CreateEmailLog(Convert.ToInt64(hdnRecordId.Value), hdnAccountName.Value);
                            Response.Redirect("Response.aspx");
                        }
                    }
                }
                else 
                {
                    //hariom 19-04-2023
                    DT = objDAL.GetDataTableSql("App_Accounts_RegPayment_List_IPM " + hdnRecordId.Value);
                    if (DT.Rows[0]["PaymentStatus"].ToString() == "1")
                    {
                        hdnPaymentRecieptId.Value = "0";
                        DataRow DR1 = DT.NewRow();
                        DR1["PaymentRecieptId"] = 0;
                        DR1["AccountId"] = hdnRecordId.Value;
                        DR1["TransactionNo"] = DateTime.Now.ToString("yyMMddHHmmss");
                        DR1["PaymentAmount"] = Convert.ToDouble(txtMemberShipAmt.Text);
                        DR1["PaidAmount"] = "0";
                        DR1["PaymentStatus"] = "1";
                        DR1["PaymentGatewayResponse"] = "";
                        DR1["ResponseNo"] = "";
                        DR1["ResponseString"] = "";
                        DR1["LastTransactionNo"] = "";
                        DR1["LastResponseNo"] = "";
                        DR1["LastResponseString"] = "";
                        DR1["PaymentBankName"] = "";
                        Int64 RecordId = 0;
                        string ReturnMessage = "";
                        objGeneralFunction.SavePaymentLog(DR1, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                    }
                    //end
                        DataRow DR = DT.Rows[0];
                    string transaction_details = ""; string PayStatus = "";
                    //DataRow[] DR = DT.Select("PaymentStatus=0");
                    //btnContinueToPay.Enabled = false;
                    PayStatus = VerifyPayment(DT.Rows[0]["TransactionNo"].ToString(), out transaction_details);
                    JObject jobj = JObject.Parse(transaction_details);
                    var postTitles = from p in jobj["transaction_details"][DT.Rows[0]["TransactionNo"].ToString()] select p;
                    string strDt = string.Empty;
                    string Amt = "";
                    string StrReason = ""; string StrRspNo = "";
                    string strName = ""; string strValue = "";
                    foreach (var item in postTitles)
                    {
                        strName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToUpper();
                        strValue = ((Newtonsoft.Json.Linq.JProperty)item).Value.ToString();
                        if (strName == "AMT")
                            Amt = strValue;
                        if (strName == "ADDEDON")
                            strDt = strValue;
                        if (strName == "FIELD9")
                            StrReason = strValue;
                        if (strName == "MIHPAYID")
                            StrRspNo = strValue;
                    }
                    DR["LastResponseNo"] = DR["ResponseNo"].ToString();
                    DR["LastResponseString"] = DR["ResponseString"].ToString();
                    DR["PaymentGatewayResponse"] = PayStatus;
                    DR["ResponseNo"] = StrRspNo.ToString();
                    DR["ResponseString"] = transaction_details;
                    DT.AcceptChanges();
                    ViewState["DT_Payment"] = DT;

                    //if (Convert.ToDouble(txtMemberShipAmt.Text) == 0)
                    //{
                    //    DR["PaymentStatus"] = "0";
                    //    DR["PaymentGatewayResponse"] = "Free Membership on account of covid";
                    //    DR["ResponseString"] = "";
                    //    string ReturnMessage = ""; Int64 RecordId = 0;
                    //    objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                    //    if (RecordId > 0)
                    //        RecordId = objGeneralFunction.UpdatePaymentSubmission_CreateEmailLog(Convert.ToInt64(hdnRecordId.Value), hdnAccountName.Value);
                    //    Response.Redirect("Response.aspx");
                    //    return;
                    //}

                    if (PayStatus.ToUpper() == "SUCCESS" && DT.Rows[0]["PaymentStatus"].ToString() == "1")
                    {
                        string ReturnMessage = ""; Int64 RecordId = 0;
                        objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                        if (RecordId > 0)
                            RecordId = objGeneralFunction.UpdatePaymentSubmission_CreateEmailLog(Convert.ToInt64(hdnRecordId.Value), hdnAccountName.Value);
                        //spPayMsg.InnerHtml = " <h4>Thank you for the payment.</h4>";
                        //spPayMsg.InnerHtml += "<br/>";
                        //spPayMsg.InnerHtml += "<h4> We have sent you an email for successfully registering at our website.</h4>";
                        //spPayMsg.InnerHtml += " <br/>";
                        //spPayMsg.InnerHtml += "<h4 >< a href = 'Default.aspx' > Continue To Home Page </ a ></h4>";
                        bntMakePayment.Visible = false;
                        wzMain.ActiveStepIndex =6;
                    }
                    else
                    {
                        spPayMsg.InnerHtml = " <h4>Your Last Transaction Was Not Successful</h4>";
                        spPayMsg.InnerHtml += "<br/>";
                        spPayMsg.InnerHtml += "<h4><u>Transaction Details</u> <br> Status  " + PayStatus + ", Date:" + strDt + ", <br>Reason:" + StrReason + "</h4>";
                        DateTime DTPaymentTime;
                        try
                        {
                            DTPaymentTime = DateTime.Parse(strDt);
                        }
                        catch (Exception ex)
                        {
                            DTPaymentTime = DateTime.Parse(DT.Rows[0]["ModifedDate"].ToString());
                            bntMakePayment.Visible = true;
                            modPayment.Show();
                            return;
                        }

                        //TimeSpan TS = DateTime.Now.Subtract(DTPaymentTime);
                        //if (TS.TotalHours < 4)
                        //{
                        //    bntMakePayment.Visible = false;  ////'its faklse'
                        //    spPayMsg.InnerHtml += "<br/>";
                        //    spPayMsg.InnerHtml += "<h4><b>Please Try after " + DTPaymentTime.AddHours(5).ToString("dd-MMM-yyyy h:mm tt") + " </b></h4>";
                        //}
                        modPayment.Show();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// For VerifyPayment
        /// In this Function using Tansaction to get response string. for get payment response string we create first hash value 
        /// using Function(Generatehash512) by parameters(StrKey(MERCHANT_KEY),StrSalt(SALT),method(verify_payment),varR1(strTransNo)).
        /// after using post methd get url(VRFYURL) and asign the hash value to url and get payment staus sucess or not.
        /// Commented By Rohit
        /// </summary>
        /// <param name="strTransNo"></param>
        /// <param name="transaction_details"></param>
        /// <returns>strstatus</returns>
        #region Verify Payment
        protected string VerifyPayment(string strTransNo, out string transaction_details)
        {
            string strstatus = ""; transaction_details = "";
            try
            {
                //using (var myWebRequest = new WebRequest())
                //{
                string StrKey = Convert.ToString(ConfigurationManager.AppSettings["MERCHANT_KEY"]);
                string StrSalt = Convert.ToString(ConfigurationManager.AppSettings["SALT"]);
                string method = "verify_payment";
                string varR1 = strTransNo;
                string strhash = Generatehash512(StrKey + "|" + "verify_payment" + "|" + varR1 + "|" + StrSalt);
                string Url = Convert.ToString(ConfigurationManager.AppSettings["VRFYURL"]);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string postString = "key=" + StrKey +
                   "&command=" + method +
                   "&hash=" + strhash +
                   "&var1=" + varR1;
                WebRequest myWebRequest = WebRequest.Create(Url);
                myWebRequest.Method = "POST";
                myWebRequest.ContentType = "application/x-www-form-urlencoded";
                myWebRequest.Timeout = 180000;
                StreamWriter requestWriter = new StreamWriter(myWebRequest.GetRequestStream());
                requestWriter.Write(postString);
                requestWriter.Close();
                StreamReader responseReader = new StreamReader(myWebRequest.GetResponse().GetResponseStream());
                WebResponse myWebResponse = myWebRequest.GetResponse();
                Stream ReceiveStream = myWebResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                string response = readStream.ReadToEnd();
                JObject account = JObject.Parse(response);
                String status = (string)account.SelectToken("transaction_details." + varR1 + ".status");
                strstatus = status;
                transaction_details = account.ToString();
            }
            catch (Exception)
            {
                strstatus = "";
            }
            return strstatus;
        }

        #endregion

        /// <summary>
        /// For CreateEmailLog
        /// Asper Code this function not in use but if in future required then we can use it
        /// Commented By Rohit
        /// </summary>
        /// <param name="AccountId"></param>
        #region Create Email log
        protected void CreateEmailLog(string AccountId)
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            EmailConfig EmailConfig = new EmailConfig();
            Int64 ReturnId = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AuthorizationLevel", Session["AuthorizationLevel"].ToString(), SqlDbType.TinyInt, 0, ParameterDirection.Input));
            DataTable DT = DAL.GetDataTable("App_Accounts_List_Email", parameters.ToArray());

            ////-------------------------- GET BOOK NAME------------------
            //parameters = new List<SqlParameter>();
            //parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", hdnBookId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@BookType", "AA", SqlDbType.NVarChar, 4, ParameterDirection.Input));
            //DataTable DTBookName = DAL.GetDataTable("App_Get_BookName", parameters.ToArray());

            List<KeyValuePair<string, object>> Paralist = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("RecordKeyId", AccountId),
                };

            #region "Email Config Member"
            EmailConfig = new EmailConfig();
            EmailConfig.BookType = "AA";
            EmailConfig.EmailType = "MS";
            EmailConfig.EmailTo = DT.Rows[0]["AccountEmail"].ToString();
            EmailConfig.ParaList = Paralist;
            EmailConfig.DTTransaction = DT;
            EmailConfig.RollType = DT.Rows[0]["MemberRoleType"].ToString();
            EmailConfig.BookName = DT.Rows[0]["BookName"].ToString();
            ReturnId = EmailConfig.CreateEmailLog();
            #endregion
            if (DT.Rows.Count > 0)
            {
                #region "Email Config "
                EmailConfig = new EmailConfig();
                EmailConfig.BookType = "AA";
                EmailConfig.ParaList = Paralist;
                EmailConfig.EmailType = "MA1";
                EmailConfig.DTTransaction = DT;
                EmailConfig.EmailTo = DT.Rows[0]["Auth_EmailAddress"].ToString();
                EmailConfig.RollType = DT.Rows[0]["MemberRoleType"].ToString();
                EmailConfig.BookName = DT.Rows[0]["BookName"].ToString();
                ReturnId = EmailConfig.CreateEmailLog();
                #endregion
            }
        }

        #endregion

        /// <summary>
        /// For btnworkfileupload_Click
        /// in this event save work notification file usingFunction(SaveWorkFile)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Save Work Notification File
        protected void btnworkfileupload_Click(object sender, EventArgs e)
        {
            //updated by Renu
            string[] strfilename = SaveWorkFile(fpworkfileupload, Convert.ToInt64(hdnWorkNotificationId.Value));
        }

        #endregion

        /// <summary>
        /// For imgtWorkfile_Click
        /// Upload Work File from GridView(gvWork) ImageButton(imgtWorkfile) Click using this function 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Upload Work File From GridView
        protected void imgtWorkfile_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgtWorkfile = (ImageButton)sender;
            GridViewRow gvr = (GridViewRow)imgtWorkfile.NamingContainer;
            HiddenField hdnWorkId = (HiddenField)gvr.FindControl("hdnWorkNotificationId");
            hdnWorkNotificationId.Value = hdnWorkId.Value;
            moduploadFiles.Show();
        }

        #endregion

        /// <summary>
        /// For TogglePaymentButton
        /// In this Function if Payment Reciept Id =0 then get membership fee amount using Store procedure(MemberRoleType_LookUp_CalFees_IPM) and those value to 
        /// Textbox(txtMemberShipAmt) and asign Image(credit-debit-card.png) to Imagebutton(btnSubmitApplication) other wise 
        /// asign Image(credit-debit-card.png) to Imagebutton(btnSubmitApplication).
        /// Commented By Rohit
        /// </summary>
        #region Show Payment Button
        protected void TogglePaymentButton()
        {
            divPayment.Visible = false;
            if (hdnPaymentRecieptId.Value != "0" && hdnPaymentRecieptId.Value != "")
            {
                if (Convert.ToInt64(hdnPaymentRecieptId.Value) > 0)
                {
                    btnSubmitApplication.ImageUrl = "~/Images/submit-application.png";
                    //btnDDChQ.Visible = false;
                }
            }
            else
            {
                DSIT_DataLayer DAL = new DSIT_DataLayer();
                divPayment.Visible = true;
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@MemberRoleTypeIds", hdnRoleTypeIds.Value, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                string StrAmount = DAL.ExecuteScalar("MemberRoleType_LookUp_CalFees_IPM", parameters.ToArray());
                txtMemberShipAmt.Text = StrAmount;
                if (txtMemberShipAmt.Text!="")
                { 
                    txtMemberShipAmt.Text = Convert.ToDouble(txtMemberShipAmt.Text).ToString("0.00");
                }
                btnSubmitApplication.ImageUrl = "~/Images/credit-debit-card.png";
                // btnDDChQ.Visible = true;
            }
        }

        #endregion

        #region Function Not In Use
        public static string[] GetStringInBetween(string strSource, string strBegin, string strEnd, bool includeBegin, bool includeEnd)
        {
            string[] result = { "", "" };
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired
                if (includeBegin)
                {
                    iIndexOfBegin -= strBegin.Length;
                }
                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {  // include the End string if desired
                    if (includeEnd)
                    { iEnd += strEnd.Length; }
                    result[0] = strSource.Substring(0, iEnd);
                    // advance beyond this segment
                    if (iEnd + strEnd.Length < strSource.Length)
                    { result[1] = strSource.Substring(iEnd + strEnd.Length); }
                }
            }
            else
            // stay where we are
            { result[1] = strSource; }
            return result;
        }//String function end
        
        private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\" runat=\"server\"" +
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

        #endregion

        /// <summary>
        /// For Generatehash512
        /// In this Function create hash value Verify the payment return hash value
        /// Commented By Rohit
        /// </summary>
        /// <param name="text"></param>
        /// <returns>hex</returns>
        #region Generate HAsh
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

        #endregion

        /// <summary>
        /// For btnReviewApplication_Click
        /// In this event redirect to page(ApplicationForm_Rpt.aspx) with AccountID for review Application
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Review Application
        protected void btnReviewApplication_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "windowKey", "window.open('App_Reports/ApplicationForm_Rpt.aspx?RID=" + clsCryptography.Encrypt(hdnRecordId.Value) + "');", true);
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "windowKey", "window.open('../Reports/PdfCall.aspx?RCID=" + APIId + "&HDR=Appheader&RPT=StudentApplication&FTR=Appfooter" + "');", true);
            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(objGeneralFunction.ReplaceASC(ex.Message), this.Page);
            }
        }

        #endregion

        /// <summary>
        /// For btnPhotoUpload_Click
        /// In this Event first check control(FileUpload) has file or not. after that check the file name and file extension(jpeg,png,gif). 
        /// if file has not valid extension then show the massage. next check any special character in file name using function(RemoveSpecialCharacters)
        /// if it has then show the massage. next save file name in specific format like(MPU_AccountID_filename with extension). check if file is already
        /// exists in folder(MemberPhoto) then delete file using function(FileDelete) after that save/replace file in folder(MemberPhoto). asign the value
        /// to related controls(hdnphotoImageName,ImgUserLarge,ImgUser,btnphotoView).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Photo Upload In folder(MemberPhoto)
        protected void btnPhotoUpload_Click(object sender, EventArgs e)
        {
            if (FPuploadPhoto.HasFiles)
            {
                HttpPostedFile PFile = FPuploadPhoto.PostedFile;
                try
                {
                    string strFileName = string.Empty;
                    if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                        || PFile.ContentType == "image/gif")
                    {
                        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        string fileExt = Path.GetExtension(PFile.FileName);
                        //Added by RENU on 11/12/2020
                        filename = Regex.Replace(filename, @"\s+", string.Empty);
                        filename = RemoveSpecialCharacters(filename);
                        Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                        bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                        if (!isValidateFilename)
                        {
                            objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded as filename have some special character", this.Page);
                            return;
                        }
                        if (filename.Length > 40)
                        {
                            filename = filename.Substring(0, 40);
                        }
                        //
                        string SaveAsFileName = "MPU_" + hdnRecordId.Value + "_" + filename + fileExt;
                        if (hdnphotoImageName.Value != string.Empty)
                        {
                            FileDelete("MPU_" + hdnRecordId.Value + "_", Server.MapPath("~/MemberPhoto/"));
                            //try
                            //{
                            //    File.Delete(Server.MapPath("~/MemberPhoto/" + hdnphotoImageName.Value));
                            //}
                            //catch (Exception) { }
                        }
                        strFileName = "~/MemberPhoto/" + "_temp_" + SaveAsFileName;
                        FPuploadPhoto.SaveAs(Server.MapPath(strFileName));
                        Int64 ReturnId = objGeneralFunction.ResizeImage(Server.MapPath("~/MemberPhoto/"), "_temp_" + SaveAsFileName, Server.MapPath("~/MemberPhoto/"), SaveAsFileName, 150, 150, true, true);
                        if (ReturnId > 0)
                        {
                            btnphotoView.Visible = true;
                            // btnphotoView.NavigateUrl = strFileName.Replace("_temp_", "");
                            hdnphotoImageName.Value = strFileName.Replace("_temp_", "");
                            ImgUserLarge.ImageUrl = strFileName.Replace("_temp_", "");
                            ImgUser.ImageUrl = strFileName.Replace("_temp_", "");
                            objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                        }
                        else
                        {
                            objGeneralFunction.BootBoxAlert("File Upload Failed", this.Page);
                        }
                    }
                    else
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png", this.Page);
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
        #endregion

        /// <summary>
        /// For btnPhotoUploadd_Click (Deceased Member)
        /// In this Event first check control(FileUpload) has file or not. after that check the file name and file extension(jpeg,png,gif). 
        /// if file has not valid extension then show the massage. next check any special character in file name using function(RemoveSpecialCharacters)
        /// if it has then show the massage. next save file name in specific format like(MPU_AccountID_filename with extension). check if file is already
        /// exists in folder(MemberPhoto) then delete file using function(FileDelete) after that save/replace file in folder(MemberPhoto). asign the value
        /// to related controls(hdnphotoImageName,ImgUserLarge,ImgUser,btnphotoView).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Deceased Member Photo Upload
        protected void btnPhotoUploadd_Click(object sender, EventArgs e)
        {
            if (FPuploadPhotod.HasFiles)
            {
                HttpPostedFile PFile = FPuploadPhotod.PostedFile;
                try
                {
                    string strFileName = string.Empty;
                    if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                        || PFile.ContentType == "image/gif")
                    {
                        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        string fileExt = Path.GetExtension(PFile.FileName);
                        //Added by RENU on 11/12/2020
                        filename = Regex.Replace(filename, @"\s+", string.Empty);
                        filename = RemoveSpecialCharacters(filename);
                        Regex sampleRegex = new Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$"); //Regex(@"^[\-+*\w]+\.[A-Za-z]{3,4}$");
                        bool isValidateFilename = sampleRegex.IsMatch(filename + fileExt);
                        if (!isValidateFilename)
                        {
                            objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded as filename have some special character", this.Page);
                            return;
                        }
                        if (filename.Length > 40)
                        {
                            filename = filename.Substring(0, 40);
                        }
                        //
                        string SaveAsFileName = "MPU_" + hdnRefAccountId.Value + "_" + filename + fileExt;
                        if (hdnphotoImageName.Value != string.Empty)
                        {
                            FileDelete("MPU_" + hdnRefAccountId.Value + "_", Server.MapPath("~/MemberPhoto/"));
                            //try
                            //{
                            //    File.Delete(Server.MapPath("~/MemberPhoto/" + hdnphotoImageName.Value));
                            //}
                            //catch (Exception) { }
                        }
                        strFileName = "~/MemberPhoto/" + "_temp_" + SaveAsFileName;
                        FPuploadPhotod.SaveAs(Server.MapPath(strFileName));
                        Int64 ReturnId = objGeneralFunction.ResizeImage(Server.MapPath("~/MemberPhoto/"), "_temp_" + SaveAsFileName, Server.MapPath("~/MemberPhoto/"), SaveAsFileName, 150, 150, true, true);
                        if (ReturnId > 0)
                        {
                            btnphotoViewd.Visible = true;
                            // btnphotoView.NavigateUrl = strFileName.Replace("_temp_", "");
                            hdnphotoImageNamed.Value = strFileName.Replace("_temp_", "");
                            ImgUserLarged.ImageUrl = strFileName.Replace("_temp_", "");
                            ImgUserd.ImageUrl = strFileName.Replace("_temp_", "");
                            objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                        }
                        else
                        {
                            objGeneralFunction.BootBoxAlert("File Upload Failed", this.Page);
                        }
                    }
                    else
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file Should be either  jpeg, jpg , gif, png", this.Page);
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

        #endregion

        /// <summary>
        /// For DisplayMemberImage
        /// In this Function Check Member Image is Exists in server MemberPhoto folder if exists then image asgin to Hyperlink(btnphotoView)
        /// Commented By Rohit
        /// </summary>
        #region Display Member Image 
        protected void DisplayMemberImage()
        {
            string FilePath = string.Empty;
            int FileExist = 0;
            btnphotoView.Visible = false;
            ImgUser.ImageUrl = "~/Images/user.png";
            ImgUserLarge.ImageUrl = "~/Images/user.png";
            try
            {
                FilePath = Server.MapPath("~/MemberPhoto/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MPU_" + hdnRecordId.Value + "_") == true)
                    {
                        FileExist = 1;
                        string filenm = Path.GetFileName(fileName);
                        //btnphotoView.NavigateUrl = "~/MemberPhoto/" + filenm;
                        ImgUserLarge.ImageUrl = "~/MemberPhoto/" + filenm;
                        ImgUser.ImageUrl = "~/MemberPhoto/" + filenm;
                        hdnphotoImageName.Value = filenm;
                        break;
                    }
                }
                if (FileExist == 1)
                {
                    btnphotoView.Visible = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Event not in use
        protected void btnDDChQ_Click(object sender, ImageClickEventArgs e)
        {
            if (wzMain.ActiveStepIndex == 5)
                wzMain.ActiveStepIndex = 6;
        }
        #endregion

        /// <summary>
        /// For txtDetails1_gst_TextChanged
        /// In this event check gst no. length validation if its correct then check gst no is correct or not asper that dispaly massge.
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region check Gst No.
        protected void txtDetails1_gst_TextChanged(object sender, EventArgs e)
        {
            if (txtDetails1_gst.Text.Length == 15)
            {
                string NewPAN = txtDetails1_gst.Text.Substring(2, 10).ToUpper();
                if (NewPAN != txtDetails2_Pan.Text.ToUpper())
                {
                    if (txtDetails2_Pan.Text.Length > 0)
                    {
                        objGeneralFunction.BootBoxAlert("PREVIOUS PAN DETAILS DOES NOT MATCH WITH CURRENT PAN DETAILS ACCORDING TO GSTIN ENTERED _______ PREVIOUS PAN = " + txtDetails2_Pan.Text.ToUpper() + " | CURRENT PAN = " + NewPAN, Page);
                    }
                    txtDetails2_Pan.Text = txtDetails1_gst.Text.Substring(2, 10).ToUpper();
                }
            }
            else
            {
                objGeneralFunction.BootBoxAlert("INVALID GSTIN ENTER", Page);
            }
        }

        #endregion

        /// <summary>
        /// For lnkBtnRedirectToWizrd_Click
        /// Get Active Index and set its previous Index to active
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Change Wizard Index
        protected void lnkBtnRedirectToWizrd_Click(object sender, EventArgs e)
        {
            wzMain.ActiveStepIndex = Convert.ToInt32(hdnWizardIndex.Value) - 1;
        }

        #endregion

        #region Event Not In Use
        protected void btnContinueToPay_Click(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();
            DT = (DataTable)ViewState["DT_Payment"];
            if (DT != null)
            {
                string ReturnMessage = ""; Int64 RecordId = 0;
                string firstname = Convert.ToString(Session["AccountName"]);
                string email = Convert.ToString(Session["LoginName"]);
                DataRow DR = DT.Rows[0];
                DR["PaidAmount"] = "0";
                DR["PaymentStatus"] = "1";
                DR["PaymentGatewayResponse"] = "";
                DR["ResponseNo"] = "";
                DR["ResponseString"] = "";
                DR["LastTransactionNo"] = "";
                DR["LastResponseNo"] = "";
                DR["LastResponseString"] = "";
                DR["PaymentBankName"] = "";
                objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    Hashtable HST_Pay = new Hashtable();
                    HST_Pay["Amt"] = txtMemberShipAmt.Text;
                    HST_Pay["rid"] = RecordId.ToString();
                    HST_Pay["fname"] = firstname;
                    HST_Pay["email"] = email;
                    HST_Pay["phone"] = txtMobile.Text;
                    HST_Pay["TransactionNo"] = DR["TransactionNo"].ToString();
                    HST_Pay["PaymentRecieptId"] = DR["PaymentRecieptId"].ToString();
                    HST_Pay["AccountId"] = hdnRecordId.Value;
                    Session["PayValues"] = HST_Pay;
                    Response.Redirect("paymentrequest.aspx");
                }
                else
                {
                    objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
                }
            }
        }

        #endregion

        #region This Event Not in Use
        protected void bntMakePaymentold_Click(object sender, EventArgs e)
        {
            //Poornima
            DataTable DT = new DataTable();
            if (ViewState["DT_Payment"] != null)
            // if (DT != null)
            {
                DT = ((DataTable)ViewState["DT_Payment"]).Clone();
                string ReturnMessage = ""; Int64 RecordId = 0;
                string firstname = Convert.ToString(Session["AccountName"]).Trim();
                string email = Convert.ToString(Session["LoginName"]).Trim();
                DataRow DR = DT.NewRow();
                DR["PaymentRecieptId"] = 0;
                DR["AccountId"] = hdnRecordId.Value;
                DR["TransactionNo"] = DateTime.Now.ToString("yyMMddHHmmss");
                DR["PaymentAmount"] = Convert.ToDouble(txtMemberShipAmt.Text);
                DR["PaidAmount"] = "0";
                DR["PaymentStatus"] = "1";
                DR["PaymentGatewayResponse"] = "";
                DR["ResponseNo"] = "";
                DR["ResponseString"] = "";
                DR["LastTransactionNo"] = "";
                DR["LastResponseNo"] = "";
                DR["LastResponseString"] = "";
                DR["PaymentBankName"] = "";
                
                //DR["PaidAmount"] = "0";
                //DR["PaymentStatus"] = "1";
                //DR["PaymentGatewayResponse"] = "";
                //DR["LastTransactionNo"] = DR["TransactionNo"].ToString();
                //DR["PaymentBankName"] = "";
                //DR["ResponseNo"] = "";
                //DR["ResponseString"] = "";
                //DR["TransactionNo"] = DR["TransactionNo"].ToString().Split('_')[0] + "_" + DateTime.Now.ToString("yyMMddHHmmss");

                objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    Hashtable HST_Pay = new Hashtable();
                    HST_Pay["Amt"] = txtMemberShipAmt.Text;
                    HST_Pay["rid"] = RecordId.ToString();
                    HST_Pay["fname"] = firstname;
                    HST_Pay["email"] = email;
                    HST_Pay["phone"] = txtMobile.Text;
                    HST_Pay["TransactionNo"] = DR["TransactionNo"].ToString();
                    HST_Pay["PaymentRecieptId"] = DR["PaymentRecieptId"].ToString();
                    HST_Pay["AccountId"] = hdnRecordId.Value;
                    Session["PayValues"] = HST_Pay;
                    Response.Redirect("paymentrequest.aspx");
                }
                else
                {
                    objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
                }
            }
        }

        #endregion

        /// <summary>
        /// For bntMakePayment_Click
        /// In this event get payment data from ViewState(DT_Payment) and asign value to related controls.
        /// if record found more 0 then create hashtable(HST_Pay) and bind those value. redirect ot page(paymentrequest.aspx)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region make Payment
        protected void bntMakePayment_Click(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();
            DT = (DataTable)ViewState["DT_Payment"];
            if (DT != null)
            {
                string ReturnMessage = ""; Int64 RecordId = 0;
                string firstname = Convert.ToString(Session["AccountName"]).Trim();
                string email = Convert.ToString(Session["LoginName"]).Trim();
                DataRow DR = DT.Rows[0];
                DR["PaidAmount"] = "0";
                DR["PaymentStatus"] = "1";
                DR["PaymentGatewayResponse"] = "";
                DR["LastTransactionNo"] = DR["TransactionNo"].ToString();
                DR["PaymentBankName"] = "";
                DR["ResponseNo"] = "";
                DR["ResponseString"] = "";
                DR["TransactionNo"] = DR["TransactionNo"].ToString().Split('_')[0] + "_" + DateTime.Now.ToString("yyMMddHHmmss");
                objGeneralFunction.SavePaymentLog(DR, "ADMINISTRATOR", out ReturnMessage, out RecordId);
                if (RecordId > 0)
                {
                    Hashtable HST_Pay = new Hashtable();
                    HST_Pay["Amt"] = txtMemberShipAmt.Text;
                    HST_Pay["rid"] = RecordId.ToString();
                    HST_Pay["fname"] = firstname;
                    HST_Pay["email"] = email;
                    HST_Pay["phone"] = txtMobile.Text;
                    HST_Pay["TransactionNo"] = DR["TransactionNo"].ToString();
                    HST_Pay["PaymentRecieptId"] = DR["PaymentRecieptId"].ToString();
                    HST_Pay["AccountId"] = hdnRecordId.Value;
                    Session["PayValues"] = HST_Pay;
                    Response.Redirect("paymentrequest.aspx");
                }
                else
                {
                    objGeneralFunction.BootBoxAlert(ReturnMessage, this.Page);
                }
            }
        }

        #endregion

        /// <summary>
        /// For btnvalidate_Click
        /// In this event Get Url From Web.config for calling API to validate the IFSC Code and get bank information asper the IFSC Code
        /// for this we use Function(CallAPI). using Json DeserializeObject we get bank infromation from IFSC_Data.cs after that asgin 
        /// these value to related controls. if IFSC Code is valid then asign vaule to lblInfo(VALID IFSC) and hdnIFSC_Val(Valid).
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Ifsc Validate
        protected void btnvalidate_Click(object sender, EventArgs e)
        {
            try
            {
                hdnIFSC_Val.Value = "";
                lblInfo.InnerHtml = "INVALID";
                txtBankName.Text = "";
                txtBankBranchName.Text = "";
                txtMICR.Text = "";
                string url = ConfigurationManager.AppSettings["IFSC_url"].ToString();
                var str = CallAPI(url + txtIFSC.Text.Trim(), "GET");
                if (str == null || str.ToString() == "" || str.ToString() == "Not Found")
                {
                    lblInfo.InnerText = "NO DATA FOUND";
                    return;
                }
                IFSC_Data IFSC_Info = new IFSC_Data();
                IFSC_Info = JsonConvert.DeserializeObject<IFSC_Data>(str.ToString());
                string info = "BANK: " + IFSC_Info.BANK +
                        "<br />BRANCH: " + IFSC_Info.BRANCH + "<br />MICR: " + IFSC_Info.MICR;
                txtBankName.Text = IFSC_Info.BANK;
                txtBankBranchName.Text = IFSC_Info.BRANCH;
                txtMICR.Text = IFSC_Info.MICR;
                lblInfo.InnerHtml = "VALID IFSC";
                hdnIFSC_Val.Value = "Valid";
            }
            catch (Exception ex)
            {
                lblInfo.InnerText = ex.ToString();
            }
        }
        #endregion

        #region code not in use
        //protected void imgValidate_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        hdnIFSC_Val.Value = "";
        //        lblInfo.InnerHtml = "INVALID";
        //        txtBankName.Text = "";
        //        txtBankBranchName.Text = "";
        //        txtMICR.Text = "";
        //        string url = ConfigurationManager.AppSettings["IFSC_url"].ToString();
        //        var str = CallAPI(url + txtIFSC.Text.Trim(), "GET");

        //        if (str == null || str.ToString() == "" || str.ToString() == "Not Found")
        //        {
        //            lblInfo.InnerText = "NO DATA FOUND";
        //            return;
        //        }

        //        IFSC_Data IFSC_Info = new IFSC_Data();
        //        IFSC_Info = JsonConvert.DeserializeObject<IFSC_Data>(str.ToString());

        //        string info = "BANK: " + IFSC_Info.BANK +
        //                "<br />BRANCH: " + IFSC_Info.BRANCH + "<br />MICR: " + IFSC_Info.MICR;

        //        txtBankName.Text = IFSC_Info.BANK;
        //        txtBankBranchName.Text = IFSC_Info.BRANCH;
        //        txtMICR.Text = IFSC_Info.MICR;

        //        lblInfo.InnerHtml = "VALID IFSC";
        //        hdnIFSC_Val.Value = "Valid";

        //    }
        //    catch (Exception ex)
        //    {
        //        lblInfo.InnerText = ex.ToString();

        //    }
        //}
        #endregion

        /// <summary>
        /// For CallAPI
        /// In this Function we pass parameter (like strURL=https://ifsc.razorpay.com/SBIN0007531,Method=GET) to this function GET the 
        /// Bank Information from Using IFSC code. here we use API from Razorpay services. for geting inforamtion using API we define 
        /// SecurityProtocol for encryption and authentication the data for this we define SecurityProtocolType 
        /// like Tls(Transport Layer Security (TLS) 1.0),Tls11(Transport Layer Security (TLS) 1.1),Tls12(Transport Layer Security (TLS) 1.2).
        /// after that using web services we get output in Reslut and its return to Event (btnvalidate_Click).
        /// Commented By Rohit
        /// </summary>
        /// <param name="strURL"></param>
        /// <param name="Method"></param>
        /// <param name="urlParameters"></param>
        /// <returns>Result</returns>
        #region Call Razor API For IFSC Code Validation
        private object CallAPI(string strURL, string Method, string urlParameters = "")
        {
            try
            {
                string URL = string.Format(strURL);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebRequest reqObj = WebRequest.Create(URL);
                reqObj.Method = Method;
                HttpWebResponse resposeObj = null;
                resposeObj = (HttpWebResponse)reqObj.GetResponse();
                string Result = "";
                using (Stream stream = resposeObj.GetResponseStream())
                {
                    StreamReader Sr = new StreamReader(stream);
                    Result = Sr.ReadToEnd();
                }
                return Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        
        public DataTable JsonToDataTable(object JsonResponse)
        {
            try
            {
                //DataTable DT = (DataTable)JsonConvert.DeserializeObject(JsonResponse.ToString(), (typeof(DataTable)));
                DataTable DT = (DataTable)JsonConvert.DeserializeObject<DataTable>(JsonResponse.ToString());
                return DT;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// For ddlRegisterType_SelectedIndexChanged
        /// Here enable or disable the division asper Register Type selection and assign value to related textbox
        /// (like txtACPerformanceShare and txtACMechanicalShare)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Registaratin Type
        protected void ddlRegisterType_SelectedIndexChanged(object sender, EventArgs e)
        {
                Getsharepercentage();
        }
        #endregion

        /// <summary>
        /// For Getsharepercentage
        /// In this function get song share details asper role type from using store procedure(App_Accounts_Song_Share_CL).
        /// aspr data calculate share asper role type like(total Author/Composer share is not more 25, total Publisher share is 
        /// not more 50) afte those data bind to ViewState(DBShareCal)
        /// Commented By Rohit
        /// </summary>
        #region Get & calculate Share
        protected void Getsharepercentage()
        {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                string ReturnMessage = string.Empty;
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
                DataSet SDS = new DataSet();
                SDS = objDAL.GetDataSet("App_Accounts_Song_Share_CL", parameters.ToArray());

                int ARoles = 0; int CRoles = 0; int CARoles = 0; int ERoles = 0;
                double AShare = 25; double CShare = 25; double CAShare = 50; double EShare = 50; double CAAShare = 25; double CACShare = 25;
                double AClShare = 0; double CCLShare = 0; double CACLShare = 0; double ECLShare = 0; double ClShare = 0;
                double DAShare = 0; double DCShare = 0; double DCAShare = 0; double DEShare = 0;

            if (SDS.Tables[0].Rows.Count == 0)
                {
                    if (ddlRegisterType.SelectedValue == "E")
                    {
                        divPublisher.Visible = true;
                        divAutCom.Visible = false;
                        txtACPerformanceShare.Text = "50.00";
                        txtACMechanicalShare.Text = "50.00";
                        txtISRCNo.Enabled = true;
                    }
                    else if (ddlRegisterType.SelectedValue == "CA")
                    {
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = "50.00";
                        txtACMechanicalShare.Text = "50.00";
                        txtISRCNo.Enabled = false;
                    }
                    else if (ddlRegisterType.SelectedValue == "A" || ddlRegisterType.SelectedValue == "C")
                    {
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = "25.00";
                        txtACMechanicalShare.Text = "25.00";
                        txtISRCNo.Enabled = false;
                    }
                    else
                    {
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = "00.00";
                        txtACMechanicalShare.Text = "00.00";
                        txtISRCNo.Enabled = false;
                    }
                }
                else
                {
                for (int i = 0; i < SDS.Tables[0].Rows.Count; i++)
                {
                    if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "A")
                    {
                        ARoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DAShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                    else if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "C")
                    {
                        CRoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DCShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                    else if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "CA")
                    {
                        CARoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DCAShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                    else
                    {
                        ERoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DEShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                }

                if (ddlRegisterType.SelectedValue == "E")
                {
                    if (ERoles != 0)
                    {
                        ECLShare = EShare / (ERoles + 1);
                        divPublisher.Visible = true;
                        divAutCom.Visible = false;
                        txtACPerformanceShare.Text = ECLShare.ToString("0.00");
                        txtACMechanicalShare.Text = ECLShare.ToString("0.00");
                        txtISRCNo.Enabled = true;
                        lblZECLShare.Text = ECLShare.ToString("0.00");
                    }
                    else
                    {
                        divPublisher.Visible = true;
                        divAutCom.Visible = false;
                        txtACPerformanceShare.Text = EShare.ToString("0.00");
                        txtACMechanicalShare.Text = EShare.ToString("0.00");
                        txtISRCNo.Enabled = true;
                    }
                }
                else if (ddlRegisterType.SelectedValue == "CA")
                {
                    if (ARoles != 0 && CRoles == 0 && CARoles == 0)
                    {
                        AClShare = AShare / (ARoles + 1);
                        CCLShare = CShare - AClShare;

                        //CAShare = AClShare + CShare;
                        CACLShare = CAShare - CCLShare;
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = CACLShare.ToString("0.00");
                        txtACMechanicalShare.Text = CACLShare.ToString("0.00");
                        txtISRCNo.Enabled = false;
                        lblZAShare.Text = AClShare.ToString("0.00");
                        //lblZCShare.Text = CCLShare.ToString("0.00");
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                    }
                    else if (CRoles != 0 && ARoles == 0 && CARoles == 0)
                    {
                        CCLShare = CShare / (CRoles + 1);
                        AClShare = AShare - CCLShare;
                        //CAShare = CCLShare + AShare;
                        CACLShare = CAShare - AClShare;
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = CACLShare.ToString("0.00");
                        txtACMechanicalShare.Text = CACLShare.ToString("0.00");
                        txtISRCNo.Enabled = false;
                        // lblZAShare.Text = AClShare.ToString("0.00");
                        lblZCShare.Text = CCLShare.ToString("0.00");
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                    }
                    else if (CRoles != 0 && ARoles != 0 && CARoles == 0)
                    {
                        AClShare = AShare / (ARoles + 1);
                        lblZAShare.Text = AClShare.ToString("0.00");
                        CCLShare = CShare / (CRoles + 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");

                        AClShare = AClShare * ((ARoles + 1) - 1);
                        CCLShare = CCLShare * ((CRoles + 1) - 1);
                        CACLShare = CAShare - (AClShare + CCLShare);
                        lblZCAShare.Text = CCLShare.ToString("0.00");
                        txtACPerformanceShare.Text = CACLShare.ToString("0.00");
                        txtACMechanicalShare.Text = CACLShare.ToString("0.00");

                    }
                    else if (ARoles != 0 && CRoles == 0 && CARoles != 0)
                    {
                        AClShare = AShare / (ARoles + CARoles + 1);
                        lblZAShare.Text = AClShare.ToString("0.00");
                        CACLShare = CAAShare / (CARoles + 1);
                        ECLShare = CACLShare + AClShare;
                        lblZCAShare.Text = ECLShare.ToString("0.00");
                        txtACPerformanceShare.Text = ECLShare.ToString("0.00");
                        txtACMechanicalShare.Text = ECLShare.ToString("0.00");

                    }
                    else if (CRoles != 0 && ARoles == 0 && CARoles != 0)
                    {
                        CCLShare = CShare / (CRoles + CARoles + 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");
                        CACLShare = CAAShare / (CARoles + 1);
                        ECLShare = CACLShare + CCLShare;
                        lblZCAShare.Text = ECLShare.ToString("0.00");
                        txtACPerformanceShare.Text = ECLShare.ToString("0.00");
                        txtACMechanicalShare.Text = ECLShare.ToString("0.00");
                    }
                    else if (CRoles != 0 && ARoles != 0 && CARoles != 0)
                    {
                        AClShare = AShare / (ARoles + CARoles + 1);
                        lblZAShare.Text = AClShare.ToString("0.00");
                        
                        CCLShare = CShare / (CRoles + CARoles + 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");
                        
                        CACLShare = CAShare / (CARoles + 1+1);
                        //ECLShare = CACLShare + CCLShare + AClShare;
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                        txtACPerformanceShare.Text = CACLShare.ToString("0.00");
                        txtACMechanicalShare.Text = CACLShare.ToString("0.00");
                    }
                    else
                    {
                        CACLShare = CAShare / (CARoles + 1);
                        divPublisher.Visible = false;
                        divAutCom.Visible = true;
                        txtACPerformanceShare.Text = CACLShare.ToString("0.00");
                        txtACMechanicalShare.Text = CACLShare.ToString("0.00");
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                        txtISRCNo.Enabled = false;
                    }
                }
                else if (ddlRegisterType.SelectedValue == "A" || ddlRegisterType.SelectedValue == "C")
                {
                    if (ddlRegisterType.SelectedValue == "A")
                    {
                        if (CARoles != 0 && CRoles == 0)
                        {
                            AClShare = AShare / (ARoles + CARoles +1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                            txtACPerformanceShare.Text = AClShare.ToString("0.00");
                            txtACMechanicalShare.Text = AClShare.ToString("0.00");
                            CACLShare = AShare/ (CARoles);
                            CCLShare = CACLShare + AClShare;

                            lblZCAShare.Text = CCLShare.ToString("0.00");
                        }
                        else if (CARoles != 0 && CRoles != 0)
                        {
                            AClShare = AShare / (ARoles + CARoles + 1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                            txtACPerformanceShare.Text = AClShare.ToString("0.00");
                            txtACMechanicalShare.Text = AClShare.ToString("0.00");
                            CCLShare = CShare / (CRoles + CARoles);

                            CACLShare = AClShare + CCLShare; //= CAShare - (AClShare * (ARoles + CARoles + 1));
                            //CCLShare = CShare / (CRoles + CARoles);
                            //CACLShare = CAShare / (ARoles + CRoles + CARoles + 1);
                            //CACLShare = CAShare - (AClShare + CCLShare) ;
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                        }
                        else
                        {
                            ClShare = AShare / (ARoles + 1);
                            lblZAShare.Text = ClShare.ToString("0.00");
                            txtACPerformanceShare.Text = ClShare.ToString("0.00");
                            txtACMechanicalShare.Text = ClShare.ToString("0.00");
                        }

                    }
                    else if (ddlRegisterType.SelectedValue == "C")
                    {
                        if (CARoles != 0 && ARoles == 0)
                        {
                            CCLShare = CShare / (CRoles + CARoles + 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");
                            txtACPerformanceShare.Text = CCLShare.ToString("0.00");
                            txtACMechanicalShare.Text = CCLShare.ToString("0.00");
                            CACLShare = CShare / (CARoles);
                            CCLShare = CACLShare + CCLShare;

                            lblZCAShare.Text = CCLShare.ToString("0.00");
                         
                        }
                        else if (CARoles != 0 && ARoles != 0)
                        {
                            CCLShare = CShare / (CRoles + CARoles + 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");
                            txtACPerformanceShare.Text = CCLShare.ToString("0.00");
                            txtACMechanicalShare.Text = CCLShare.ToString("0.00");
                            
                            AClShare = AShare / (ARoles + CARoles );
                            //CCLShare = CCLShare * ((CRoles + 2) - 1);
                            CACLShare = CCLShare + AClShare; //CAShare - (CCLShare + AClShare);
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                           
                        }
                        else
                        {
                            ClShare = CShare / (CRoles + 1);
                            lblZCShare.Text = ClShare.ToString("0.00");
                            txtACPerformanceShare.Text = ClShare.ToString("0.00");
                            txtACMechanicalShare.Text = ClShare.ToString("0.00");
                        }

                    }
                    else
                    {
                        txtACPerformanceShare.Text = CShare.ToString("0.00");
                        txtACMechanicalShare.Text = CShare.ToString("0.00");
                    }
                    divPublisher.Visible = false;
                    divAutCom.Visible = true;
                    txtISRCNo.Enabled = false;
                }
                else
                {
                    divPublisher.Visible = false;
                    divAutCom.Visible = true;
                    txtACPerformanceShare.Text = "00.00";
                    txtACMechanicalShare.Text = "00.00";
                    txtISRCNo.Enabled = false;
                }

                    ViewState["DBShareCal"] = SDS.Tables[0];
                }
        }

        #endregion

        protected void GetDeletepercentage(int SongRegistrationID)
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            string ReturnMessage = string.Empty;
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@WorkNotificationId", HdnZeroworkID.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            DataSet SDS = new DataSet();
            SDS = objDAL.GetDataSet("App_Accounts_Song_Share_CL", parameters.ToArray());

            int ARoles = 0; int CRoles = 0; int CARoles = 0; int ERoles = 0;
            double AShare = 25; double CShare = 25; double CAShare = 50; double EShare = 50; double CAAShare = 25; double CACShare = 25;
            double AClShare = 0; double CCLShare = 0; double CACLShare = 0; double ECLShare = 0; double ClShare = 0;
            double DAShare = 0; double DCShare = 0; double DCAShare = 0; double DEShare = 0;

            if (SDS.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < SDS.Tables[0].Rows.Count; i++)
                {
                    if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "A")
                    {
                        ARoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DAShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                       
                    }
                    else if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "C")
                    {
                        CRoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DCShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                    else if (SDS.Tables[0].Rows[i]["RoleType"].ToString() == "CA")
                    {
                        CARoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DCAShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                    else
                    {
                        ERoles = int.Parse(SDS.Tables[0].Rows[i]["Record"].ToString());
                        DEShare = double.Parse(SDS.Tables[0].Rows[i]["Share"].ToString());
                    }
                }

                parameters.Clear();
                DataSet MySelectDataSet = new DataSet();
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", SongRegistrationID, SqlDbType.BigInt, 0, ParameterDirection.Input));
                MySelectDataSet = objDAL.GetDataSet("App_Accounts_SongRegistration_Select_List_IPM", parameters.ToArray());

                if (MySelectDataSet.Tables[0].Rows[0]["RoleType"].ToString() == "A")
                {
                    if (ARoles != 0 && CARoles == 0)
                    {
                        AClShare = AShare / (ARoles - 1);
                       
                        lblZAShare.Text = AClShare.ToString("0.00");
                    }
                    else if (ARoles != 0 && CARoles != 0 && CRoles == 0)
                    {
                        AClShare = AShare / (ARoles + CARoles - 1);
                        lblZAShare.Text = AClShare.ToString("0.00");
                       
                        ClShare = AShare / (CARoles);
                        CACLShare = AClShare + ClShare;
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                    }
                    else if (ARoles != 0 && CARoles != 0 && CRoles != 0)
                    {
                        AClShare = AShare / (ARoles + CARoles - 1);
                        lblZAShare.Text = AClShare.ToString("0.00");
                        CCLShare = CShare / (CRoles + CARoles);
                        if (ARoles > 1 )
                        {
                            ClShare = AShare / (CARoles + 1);
                            CACLShare = CCLShare + AClShare;
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                        }
                        else
                        {
                            ClShare = AShare / (CARoles);
                            CACLShare = ClShare + CCLShare;
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                        }

                    }
                    else { }
                }
                else if (MySelectDataSet.Tables[0].Rows[0]["RoleType"].ToString() == "C")
                {
                    if (CRoles != 0 && CARoles == 0)
                    {
                        CCLShare = CShare / (CRoles - 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");
                    }
                    else if (CRoles != 0 && CARoles != 0 && ARoles == 0)
                    {
                        CCLShare = CShare / (CRoles + CARoles - 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");

                        ClShare = CShare / (CARoles);
                        CACLShare = ClShare + CCLShare;
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                    }
                    else if (CRoles != 0 && CARoles != 0 && ARoles != 0)
                    {
                        CCLShare = CShare / (CRoles + CARoles - 1);
                        lblZCShare.Text = CCLShare.ToString("0.00");
                        AClShare = AShare / (ARoles + CARoles);
                        if (CRoles > 1)
                        {
                            ClShare = CShare / (CARoles + 1);
                            CACLShare = AClShare + CCLShare;
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                        }
                        else
                        {
                            ClShare = CShare / (CARoles);
                            CACLShare = ClShare + AClShare;
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                        }
                    }
                    else { }

                }
                else if (MySelectDataSet.Tables[0].Rows[0]["RoleType"].ToString() == "CA")
                {

                    
                    if (ARoles != 0 && CRoles == 0 && CARoles != 0)
                    {
                        if (CARoles > 1)
                        {
                            AClShare = AShare / (ARoles + CARoles - 1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                            CACLShare = CShare / (CARoles - 1); 
                            ClShare = ClShare + AClShare;
                            lblZCAShare.Text = ClShare.ToString("0.00");
                        }
                        else
                        {
                            CACLShare = CAShare / (CARoles - 1);
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                            AClShare = AShare / (ARoles + CARoles - 1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                        }
                    }
                    else if (CRoles != 0 && ARoles == 0 && CARoles != 0)
                    {
                        if (CARoles > 1)
                        {
                            CCLShare = CShare / (CRoles + CARoles - 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");
                            CACLShare = AShare / (CARoles - 1);
                            ClShare = CACLShare + CCLShare;
                            lblZCAShare.Text = ClShare.ToString("0.00");
                        }
                        else
                        {
                            CACLShare = CAShare / (CARoles - 1);
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                            CCLShare = CShare / (CRoles + CARoles - 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");
                        }
                    }
                    else if (CRoles != 0 && ARoles != 0 && CARoles != 0)
                    {
                        if (CARoles > 1)
                        {
                            AClShare = AShare / (ARoles + CARoles - 1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                            CCLShare = CShare / (CRoles + CARoles - 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");

                            CACLShare = AShare / (CARoles - 1);
                            ECLShare = CShare / (CARoles - 1);

                            ClShare = CCLShare+  AClShare;
                            lblZCAShare.Text = ClShare.ToString("0.00");
                        }
                        else
                        {
                            CACLShare = CAShare / (CARoles - 1);
                            lblZCAShare.Text = CACLShare.ToString("0.00");
                            CCLShare = CShare / (CRoles + CARoles - 1);
                            lblZCShare.Text = CCLShare.ToString("0.00");
                            AClShare = AShare / (ARoles + CARoles - 1);
                            lblZAShare.Text = AClShare.ToString("0.00");
                            ECLShare = CCLShare + AClShare;
                            lblZCAShare.Text = ECLShare.ToString("0.00");
                        }
                    }
                    else 
                    {
                        CACLShare = CAShare / (CARoles - 1);
                        lblZCAShare.Text = CACLShare.ToString("0.00");
                    }
                }
                else
                {
                    ECLShare = EShare/ (ERoles - 1);
                    lblZECLShare.Text = ECLShare.ToString("0.00");
                }
                ViewState["DBShareCal"] = SDS.Tables[0];
            }

        }

        /// <summary>
        /// For ddlWorkCategory_SelectedIndexChanged
        /// here populate work sub-category using Store Procedure(App_SubCategory_List) with parameter (@CategoryGroupId)
        /// and assign value to dropdownlist(ddlWorkSubCategory). if dataset return null data then assign value 
        /// Null to dropdownlist(ddlWorkSubCategory)
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Work Sub Category
        protected void ddlWorkCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>();
            DataSet swds = new DataSet(); 
            parameters.Add(objGeneralFunction.GetSqlParameter("@CategoryGroupId", ddlWorkCategory.SelectedValue, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordStatus", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            objDAL.FillDropDown("App_SubCategory_List", parameters.ToArray(), ddlWorkSubCategory, "Work Sub Category",out swds);
            if (swds.Tables[0].Rows.Count > 1)
            {
                ddlWorkSubCategory.Enabled = true;
            }
            else 
            {
                ddlWorkSubCategory.Items.Clear();
                ddlWorkSubCategory.Items.Insert(0, new ListItem("Null", "Null"));
                ddlWorkSubCategory.Enabled = false;
            }
        }
        #endregion

        /// <summary>
        /// For rbtnlACOtherSoc_SelectedIndexChanged
        /// here we populate Performing Society information using Store Procedure(App_PerformSociety_List) asper the 
        /// information enable the dropdownlist(ddlPerformingSociety). here paramter @RecordKeyId=0 means get all soceity list
        /// and IPI no. is compulsary and assign value to dropdownlist(ddlPerformingSociety) and enable it.
        /// @RecordKeyId = 1 means assign only Non-Member Society and dropdownlist(ddlPerformingSociety) and disable it.
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Get Other Performing Society List
        protected void rbtnlACOtherSoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parame = new List<SqlParameter>();

            if (rbtnlACOtherSoc.SelectedValue == "Y")
            {
                ddlPerformingSociety.Enabled = true;
                txtACIPInumber.Enabled = true;
                parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId",0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
            }
            else 
            {
                
                parame.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId",1, SqlDbType.BigInt, 0, ParameterDirection.Input));
                objDAL.FillDropDown("App_PerformSociety_List", parame.ToArray(), ddlPerformingSociety, "Performing Society");
                ddlPerformingSociety.SelectedIndex = 1;
                ddlPerformingSociety.Enabled = false;
                txtACIPInumber.Enabled = false;
            }
        }
        #endregion

        /// <summary>
        /// For rdbnldp_SelectedIndexChanged
        /// in this evnet controls visible are true or false asper  radiobuttonlist(rdbnldp_SelectedIndexChanged) index change
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Change for Song on Public Domain
        protected void rdbnldp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdbnldp.SelectedValue == "Y")
            {
                txtACFirstName.Text = "DP";
                txtACIPInumber.Text = "00039657154";
                txtACFirstName.Enabled = false;
                txtACLastName.Enabled = false;
                txtACIPInumber.Enabled=false;
            }
            else 
            {
                txtACFirstName.Text = "";
                txtACIPInumber.Text = "";
                txtACFirstName.Enabled = true;
                txtACLastName.Enabled = true;
                txtACIPInumber.Enabled= false;
            }
        }

        #endregion

        /// <summary>
        /// For txtACIPInumber_TextChanged
        /// Validate IPI number is Numeric and have 11 digit
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Validate IPI Number
        protected void txtACIPInumber_TextChanged(object sender, EventArgs e)
        {
            string IPILEN = txtACIPInumber.Text;

            if (IPILEN.Length != 11)
            {
                objGeneralFunction.BootBoxAlert("IPI Number must be equal to 11 Number Digit", Page);
                return;
            }
        }

        #endregion

        /// <summary>
        /// for txtRelYear_TextChanged
        /// here we check release year validation with current year and length
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Release Year
        protected void txtRelYear_TextChanged(object sender, EventArgs e)
        {
            string RELLEN = txtRelYear.Text;
            if (RELLEN.Length != 4)
            {
                objGeneralFunction.BootBoxAlert("Release Year must be equal to 4 Number Digit", Page);
                return;
            }
            else
            {
                string currentyear = DateTime.Now.Year.ToString();
                string relyear = txtRelYear.Text;
                if (float.Parse(relyear.ToString()) <= float.Parse(currentyear.ToString()))
                {
                }
                else
                {
                    objGeneralFunction.BootBoxAlert("Release Year must be equal or less than Current Year", Page);
                    return;
                }
            }
        }
        #endregion
    }

}