using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Globalization;
namespace IPRS_Member
{
    public partial class MemberWelcome : System.Web.UI.Page
    {
        /// <summary>
        /// create object for GeneralFunction and DSIT_DataLayer
        /// from the GeneralFunction we can call customize function or common function which is use in multiple pages.
        /// from the DSIT_DataLayer we can call store proceduer, dataset, datatable and Sql Connection.
        /// Commented By Rohit
        /// </summary>
        GeneralFunction objGeneralFunction = new GeneralFunction();
        DSIT_DataLayer objDAL = new DSIT_DataLayer();

        /// <summary> for Page Load
        /// here we get Member Role Type from using MemberRoles_Lookup_Populate (store procedure)
        /// and assgin those value to DropDownTypeEntity.
        /// Defualt value of DropDownTypeEntity is equal to 0
        /// Asper Role Type related Division are visibles
        ///  Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Commented By Rohit
                //get MemberRoleName (RoleType), MemberRoleCode (RoleTypeID) value from MemberRoles_Lookup Table
                //using MemberRoles_Lookup_Populate Store Procedure
                //and Assgin those value to DropDownTypeEntity SelectedText is RoleType and SelectedValue = RoleTypeID
                objDAL.FillDropDown("MemberRoles_Lookup_Populate", DropDownTypeEntity, "Type of Entity");
                Session["AccountRegType"] = DropDownTypeEntity.SelectedValue;

                if (DropDownTypeEntity.SelectedValue == "0" || DropDownTypeEntity.SelectedValue == "")
                {
                    divEntityType.Visible = false;
                    //divLegelHire.Visible = false;
                    divLegelHireAccount.Visible = false;
                    divLegelHireIPINumber.Visible = false;
                    divradio.Visible = false;
                    divvalidate.Visible = false;
                    //divLegDetail.Visible = false;
                    Session["AccountCode"] = "";
                }
                else if (DropDownTypeEntity.SelectedValue == "LH")
                {

                    divEntityType.Visible = true;
                    //divLegelHire.Visible = true;
                    divradio.Visible = true;
                    if (rbtValOption.SelectedValue == "A")
                    {
                        divLegelHireAccount.Visible = true;
                        divLegelHireIPINumber.Visible = false;
                    }
                    else if (rbtValOption.SelectedValue == "I")
                    {
                        divLegelHireAccount.Visible = false;
                        divLegelHireIPINumber.Visible = true;

                    }
                    divvalidate.Visible = true;
                    DLRoleType_I.Enabled = false;
                    setEntityType(DropDownTypeEntity.SelectedValue);
                    lblType.Text = DropDownTypeEntity.SelectedItem.ToString();
                    //btnSubmit_I.Enabled = false;                                          
                }
                else
                {
                    divEntityType.Visible = true;
                    DLRoleType_I.Enabled = true;
                    setEntityType(DropDownTypeEntity.SelectedValue);
                    lblType.Text = DropDownTypeEntity.SelectedItem.ToString();
                    //divLegelHire.Visible = false;
                    divLegelHireAccount.Visible = false;
                    divLegelHireIPINumber.Visible = false;
                    divvalidate.Visible = false;
                    divradio.Visible = false;
                    Session["AccountCode"] = "";
                }
            }
        }

        #endregion

        /// <summary> For Set Entity Type
        /// In this function we get data from database asper selected Role type and assign value to different Controls like 
        /// DLRoleType_I(Datalist),Get docType, Get value asper docType and set value DLDocuments_I(Repeater)and Get membership Fee Detail and assign to lblfees_I(Label)
        /// Commented By Rohit
        /// </summary>
        /// <param name="EType"></param>

        #region Set Entity Type
        private void setEntityType(string EType)
        {

            // In this code we find the Role Type by using Registration Type
            // for this here we can use MemberRoleType_LookUp_Populate_IPM (Store Procedure)
            //after we get data from the database we assign those value to DLRoleType_I (DataList)
            //for this  MemberRoleType_LookUp_Populate_IPM (Store Procedure) we use table like MemberRoleType_LookUp
           // Commented By Rohit
            #region Find the Role Type asper Registration Type
            DataTable DT;
            var parameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", DropDownTypeEntity.SelectedValue, SqlDbType.NVarChar, 5, ParameterDirection.Input));
            DataRow[] DR;
            DT = new DataTable();
            DT = objDAL.GetDataTable("MemberRoleType_LookUp_Populate_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                DR = DT.Select("RegistrationType='" + DropDownTypeEntity.SelectedValue.ToString() + "'");
                if (DR.Length > 0)
                {
                    DLRoleType_I.DataSource = DR.CopyToDataTable();
                    DLRoleType_I.DataBind();
                }
                //DR = DT.Select("RegistrationType='C'");
                //if (DR.Length > 0)
                //{
                //    DLRoleType_C.DataSource = DR.CopyToDataTable();
                //    DLRoleType_C.DataBind();
                //}
            }
            parameters.Clear();
            #endregion

            //Firstly, here we find Registration Type and after that we assign the doctype value which is
            //we fix for different value for different Registration Type. 
            //now we can find the data from data asper doctype by using Doc_LookUp_ListData_IPM (Store Procedure)
            // and assign the list to DLDocuments_I(Repeater)
            //For the Doc_LookUp_ListData_IPM (Store Procedure) we use table Doc_LookUp
            //Commented By Rohit
            #region "Retrieving the Documnet List data from database."

            string docType = "";

            if (DropDownTypeEntity.SelectedValue == "I" || DropDownTypeEntity.SelectedValue == "SC")
            {
                docType = "MUPI";
            }
            else if (DropDownTypeEntity.SelectedValue == "C")
            {
                docType = "MUPC";
            }
            else if (DropDownTypeEntity.SelectedValue == "NI"  || DropDownTypeEntity.SelectedValue == "NSC")
            {
                docType = "MUPNI";
            }
            else if (DropDownTypeEntity.SelectedValue == "NC" )
            {
                docType = "MUPNC";
            }
            else if (DropDownTypeEntity.SelectedValue == "LH")
            {
                docType = "MUPLH";
            }
            else if (DropDownTypeEntity.SelectedValue == "LHN")
            {
                docType = "MUPLHN";
            }

            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentName", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentType", docType, SqlDbType.NVarChar, 10, ParameterDirection.Input));

            objDAL = new DSIT_DataLayer();
            DT = new DataTable();
            DT = objDAL.GetDataTable("Doc_LookUp_ListData_IPM", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                DLDocuments_I.DataSource = DT.Select("DocumentType='" + docType + "'").AsEnumerable().Take(11).CopyToDataTable();
                DLDocuments_I.DataBind();

                //DLDocuments_C.DataSource = DT.Select("DocumentType='MUPC'").AsEnumerable().Take(8).CopyToDataTable();
                //DLDocuments_C.DataBind();

            }

            #endregion

            //Firstly, here we find  member fee deatil asper Role Type by using MemberRoleType_LookUp_Fees_List_IPM (Store Procedure) 
            // and assign the those value to  to lblfees_I(label)
            //For the MemberRoleType_LookUp_Fees_List_IPM (Store Procedure) we use table MemberRoleType_LookUp_Fees
            //Commented By Rohit
            #region Retrieve Membership Fee Asper Role Type

            objDAL = new DSIT_DataLayer();
            DT = new DataTable();
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RoleType", "", SqlDbType.NVarChar, 10, ParameterDirection.Input));

            DT = objDAL.GetDataTable("MemberRoleType_LookUp_Fees_List_IPM", parameters.ToArray());
            string RoleType = string.Empty;
            for (int i = 0; i < DLRoleType_I.Items.Count; i++)
            {
                HiddenField hdnRoleTypeId = (HiddenField)DLRoleType_I.Items[i].FindControl("hdnRoleTypeId");
                HiddenField hdnRoleType = (HiddenField)DLRoleType_I.Items[i].FindControl("hdnRoleType");
                CheckBox cbxSelect = (CheckBox)DLRoleType_I.Items[i].FindControl("cbxSelect");
                if (cbxSelect != null)
                {
                    hdnRoleTypeIds_I.Value = hdnRoleTypeId.Value;
                    RoleType = hdnRoleType.Value;
                    cbxSelect.Checked = true;
                    break;
                }

            }
            if (DT.Rows.Count > 0)
            {
                DR = DT.Select("RoleType='" + RoleType + "'");
                if (DR.Length > 0)
                {
                    lblfees_I.Text = DR[0]["RoleTypeFee"].ToString();

                    if (lblfees_I.Text != string.Empty)
                        lblfees_I.Text = Convert.ToDouble(lblfees_I.Text).ToString("0.00");
                }
            }

            #endregion

            #region Unuse Code
            //for (int i = 0; i < DLRoleType_C.Items.Count; i++)
            //{
            //    HiddenField hdnRoleTypeId = (HiddenField)DLRoleType_C.Items[i].FindControl("hdnRoleTypeId");
            //    HiddenField hdnRoleType = (HiddenField)DLRoleType_C.Items[i].FindControl("hdnRoleType");
            //    CheckBox cbxSelect = (CheckBox)DLRoleType_C.Items[i].FindControl("cbxSelect");
            //    if (cbxSelect != null)
            //    {
            //        RoleType = hdnRoleType.Value;
            //        hdnRoleTypeIds_C.Value = hdnRoleTypeId.Value;
            //        cbxSelect.Checked = true;
            //        break;
            //    }

            //}

            //if (DT.Rows.Count > 0)
            //{
            //    DR = DT.Select("RoleType='" + RoleType + "'");
            //    if (DR.Length > 0)
            //    {
            //        lblfees_C.Text = DR[0]["RoleTypeFee"].ToString();

            //        if (lblfees_C.Text != string.Empty)
            //            lblfees_C.Text = Convert.ToDouble(lblfees_C.Text).ToString("0.00");
            //    }
            //}
            #endregion

        }

        #endregion

        /// <summary> For Role type Selection
        /// here we get RoleType and RoleTypeId from DLRoleType_I(Datalist) and on this we get Membership Fee by using query
        /// and assign those value to lblfees_I(Label)
        /// for the query we use MemberRoleType_LookUp_Fees table 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region Role type Selection
        protected void cbxSelect_I_CheckedChanged(object sender, EventArgs e)
        {
            hdnRoleTypeIds_I.Value = string.Empty;
            string RoleType = string.Empty;
            for (int i = 0; i < DLRoleType_I.Items.Count; i++)
            {
                HiddenField hdnRoleTypeId = (HiddenField)DLRoleType_I.Items[i].FindControl("hdnRoleTypeId");
                HiddenField hdnRoleType = (HiddenField)DLRoleType_I.Items[i].FindControl("hdnRoleType");
                CheckBox cbxSelect = (CheckBox)DLRoleType_I.Items[i].FindControl("cbxSelect");
                if (cbxSelect.Checked == true)
                {
                    hdnRoleTypeIds_I.Value = hdnRoleTypeIds_I.Value + hdnRoleTypeId.Value + "~";
                    RoleType = RoleType + hdnRoleType.Value;
                }

            }
            if (RoleType != string.Empty)
            {
                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@TableName", "MemberRoleType_LookUp_Fees", SqlDbType.VarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@ColumnName", "RoleTypeFee", SqlDbType.VarChar, 100, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@WhereClause", "Roletype='" + RoleType + "'", SqlDbType.VarChar, 100, ParameterDirection.Input));

                lblfees_I.Text = objDAL.ExecuteScalar("App_ExecuteScalar", parameters.ToArray());

                if (lblfees_I.Text != string.Empty)
                    lblfees_I.Text = Convert.ToDouble(lblfees_I.Text).ToString("0.00");
            }
            else
            {
                lblfees_I.Text = "0.00";
            }
        }
        #endregion

        #region Code Not in Use
        //protected void cbxSelect_C_CheckedChanged(object sender, EventArgs e)
        //{
        //    hdnRoleTypeIds_C.Value = string.Empty;
        //    string RoleType = string.Empty;
        //    for (int i = 0; i < DLRoleType_C.Items.Count; i++)
        //    {
        //        HiddenField hdnRoleTypeId = (HiddenField)DLRoleType_I.Items[i].FindControl("hdnRoleTypeId");
        //        HiddenField hdnRoleType = (HiddenField)DLRoleType_C.Items[i].FindControl("hdnRoleType");
        //        CheckBox cbxSelect = (CheckBox)DLRoleType_C.Items[i].FindControl("cbxSelect");
        //        if (cbxSelect.Checked == true)
        //        {
        //            hdnRoleTypeIds_C.Value = hdnRoleTypeIds_C.Value + hdnRoleTypeId.Value + "~";
        //            RoleType = RoleType + hdnRoleType.Value;
        //        }

        //    }
        //    if (RoleType != string.Empty)
        //    {
        //        DSIT_DataLayer objDAL = new DSIT_DataLayer();
        //        var parameters = new List<SqlParameter>();
        //        parameters.Add(objGeneralFunction.GetSqlParameter("@TableName", "MemberRoleType_LookUp_Fees", SqlDbType.VarChar, 100, ParameterDirection.Input));
        //        parameters.Add(objGeneralFunction.GetSqlParameter("@ColumnName", "RoleTypeFee", SqlDbType.VarChar, 100, ParameterDirection.Input));
        //        parameters.Add(objGeneralFunction.GetSqlParameter("@WhereClause", "Roletype='" + RoleType + "'", SqlDbType.VarChar, 100, ParameterDirection.Input));

        //        lblfees_C.Text = objDAL.ExecuteScalar("App_ExecuteScalar", parameters.ToArray());

        //        if (lblfees_C.Text != string.Empty)
        //            lblfees_C.Text = Convert.ToDouble(lblfees_C.Text).ToString("0.00");
        //    }
        //    else
        //    {
        //        lblfees_C.Text = "0.00";
        //    }
        //}

        //protected void btnSubmit_C_Click(object sender, EventArgs e)
        //{
        //    if (lblfees_C.Text == string.Empty)
        //        lblfees_C.Text = "0";

        //    if (Convert.ToDouble(lblfees_C.Text) == 0)
        //    {

        //        objGeneralFunction.BootBoxAlert("Select Role Type For Company", Page);
        //    }
        //    if (cbx_C.Checked == false)
        //    {
        //        objGeneralFunction.BootBoxAlert("Please agree to Terms and Conditions For Company", Page);
        //        cbx_C.Focus();
        //        return;
        //    }
        //    Response.Redirect("MemberRegistration.aspx?RT=" + hdnRoleTypeIds_C.Value + "&RG=C", false);
        //}
        #endregion

        /// <summary>For btnSubmit_I_Click
        /// Here First Check Member check cbx_I(Check Button) are check or not after that pass the value (Role type Id and Regitration Type)
        /// in querry string to MemberRegistration.aspx and redirect it
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region Send Information to Next
        protected void btnSubmit_I_Click(object sender, EventArgs e)
        {
            Session["AccountRegType"] = DropDownTypeEntity.SelectedValue;

            if (lblfees_I.Text == string.Empty)
                lblfees_I.Text = "0";
            //if (Convert.ToDouble(lblfees_I.Text) == 0)
            //{

            //    objGeneralFunction.BootBoxAlert("Select Role Type For Individual", Page);
            //    return;
            //}

            if (cbx_I.Checked == false)
            {
                objGeneralFunction.BootBoxAlert("Please agree to Terms and Conditions For Individual", Page);
                cbx_I.Focus();
                return;
            }
            Response.Redirect("MemberRegistration.aspx?RT=" + hdnRoleTypeIds_I.Value + "&RG=I", false);
        }

        #endregion

        /// <summary> for DropDownTypeEntity SelectedIndexChanged
        /// Here we check which role type selected by Member As per that required Division will be visible
        /// /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region DropDownTypeEntity SelectedIndexChanged
        protected void DropDownTypeEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownTypeEntity.SelectedValue == "0" || DropDownTypeEntity.SelectedValue == "")
            {
                divEntityType.Visible = false;
                //DropDownTypeEntity.Visible = false;
                //divLegelHire.Visible = false;
                divLegelHireAccount.Visible = false;
                divLegelHireIPINumber.Visible = false;
                divvalidate.Visible = false;
                divradio.Visible = false;

                btnSubmit_I.Enabled = true;
                Session["AccountCode"] = "";
            }
            else if (DropDownTypeEntity.SelectedValue == "LH")
            {
                divEntityType.Visible = true;
                setEntityType(DropDownTypeEntity.SelectedValue);
                lblType.Text = DropDownTypeEntity.SelectedItem.ToString();
                //divLegelHire.Visible = true;
                divradio.Visible = true;

                if (rbtValOption.SelectedValue == "A")
                {
                    divLegelHireAccount.Visible = true;
                    divLegelHireIPINumber.Visible = false;
                }
                else if (rbtValOption.SelectedValue == "I")
                {
                    divLegelHireAccount.Visible = false;
                    divLegelHireIPINumber.Visible = true;

                }

                DLRoleType_I.Enabled = false;
                divvalidate.Visible = true;
                btnSubmit_I.Enabled = false;
            }
            else
            {
                divEntityType.Visible = true;
                setEntityType(DropDownTypeEntity.SelectedValue);
                lblType.Text = DropDownTypeEntity.SelectedItem.ToString();
                //divLegelHire.Visible = false;
                DLRoleType_I.Enabled = true;
                divradio.Visible = false;
                divLegelHireAccount.Visible = false;
                divLegelHireIPINumber.Visible = false;
                divvalidate.Visible = false;
                btnSubmit_I.Enabled = true;
                Session["AccountCode"] = "";


            }

        }

        #endregion

        /// <summary> For rbtValOption_SelectedIndexChanged

        ///  This event Use in only when member Select Legal Heir - Existing Deceased Member
        ///  here we check if member serch by Name or IPI no. set value to Name is A and IPI no is I
        ///  Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region  Check Radion Button Value
        protected void rbtValOption_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (rbtValOption.SelectedValue == "A")
            {
                divLegelHireAccount.Visible = true;
                divLegelHireIPINumber.Visible = false;
            }
            else if (rbtValOption.SelectedValue == "I")
            {
                divLegelHireAccount.Visible = false;
                divLegelHireIPINumber.Visible = true;

            }

        }

        #endregion

        /// <summary> For btnValidate_Click ,Existing Deceased Member
        /// Here we check Deceased Member are exists in IPRS or not with parameter like IPI No. or Date of Death. If Deceased Member are exists then this event get member detail and 
        /// assign value with respective field. If Deceased Member are not exists then display massage to contact Administrator. 
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region  Validate Existing Deceased Member
        protected void btnValidate_Click(object sender, EventArgs e)
        {
            if (booValidateData() == true)
            {
                if (isnulldateofdeath() == true)
                {
                    objGeneralFunction.BootBoxAlert("Date of death Not Found", Page);
                    return;
                }
                btnSubmit_I.Enabled = true;
                //divLegelHire.Visible = false;  
                divLegelHireAccount.Visible = false;
                divLegelHireIPINumber.Visible = false; 
                divvalidate.Visible = false;
                divradio.Visible = false;
                divLegDetail.Visible = true;
                DisplayExistingDetails();
            }
            else
            {
                btnSubmit_I.Enabled = false;
                //divLegelHire.Visible = true;
                
                if (rbtValOption.SelectedValue =="A")
                {
                    divLegelHireIPINumber.Visible = false;
                    divLegelHireAccount.Visible = true;
                }
                else if (rbtValOption.SelectedValue == "I")
                {
                    divLegelHireIPINumber.Visible = true;
                    divLegelHireAccount.Visible = false;
                }
                objGeneralFunction.BootBoxAlert("Member details not found! Please write to us at membership@iprs.org or call us on 7700004372 from 10.00AM to 07.00PM (Monday to Friday)", Page); //added by Hariom 20-02-2023
                divLegDetail.Visible = false;
            }

        }

        #endregion

        /// <summary> For DisplayExistingDetails
        /// If Member exits in Our datatbase then we find and get details of member and assign to respective controls.
        /// for find the data we use App_Accounts_List_Deceased (Store Procedure). in this proceduer we use App_Accounts table
        /// Commented By Rohit
        /// </summary>

        #region Display Existing Deceased Member
        protected void DisplayExistingDetails()
        {
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@Accountcode", Session["AccountCode"], SqlDbType.NVarChar, 50, ParameterDirection.Input));
            DataSet DS = DAL.GetDataSet("App_Accounts_List_Deceased", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];
                txtdeceasedMemName.Text = DR["AccountName"].ToString();
                hdnDAccountId.Value = DR["AccountId"].ToString();
                txtdeceasedMemName.ReadOnly = true;
                if (DR["Relationship"].ToString() != "")
                {
                    txtrelationship.Text = DR["Relationship"].ToString();
                }
                txtrelationship.ReadOnly = true;
                txtDOB.Text = DateTime.Parse(DR["DOB"].ToString()).ToString("dd/MMM/yyyy");
                txtDOB.ReadOnly = true;
                if (DR["DateOfDeath"].ToString() != "" || DR["DateOfDeath"].ToString() != null)
                {
                    txtdateofdeath.Text = DateTime.Parse(DR["DateOfDeath"].ToString()).ToString("dd/MMM/yyyy");
                }
                txtdateofdeath.ReadOnly = true;
                txtdatecertNumber.Text = DR["DeathCertificateNo"].ToString();
                txtdatecertNumber.ReadOnly = true;
                txtAddress_PM.InnerText = DR["AccountAddress_Pr"].ToString();
                txtAddress_PM.Disabled = true;
                //Added by Hariom 25-02-2023
                txtIPINo.Text = DR["IPINumber"].ToString();
                txtIPINo.ReadOnly = true;
                //    end
                txtpincode.Text = DR["Pincode_Pr"].ToString();
                txtpincode.ReadOnly = true;
                txtgeo.Text = DR["GeographicalName_Pr"].ToString();
                txtgeo.ReadOnly = true;
                //ddlGeo_PM.SelectDropDown(DR["GeographicalId"].ToString(), DR["GeographicalName"].ToString());
                //ddlGeo_PM.blEnabled = false;
                //ddlPincode_PM.SelectDropDown(DR["PincodeId"].ToString(), DR["Pincode"].ToString());
                //ddlPincode_PM.blEnabled = false;                
                DisplayMemberImage();
            }
        }

        #endregion

        /// <summary> For DisplayMemberImage
        /// Here we find Member photo is exists in server mapth folder(MemberPhoto) or not.
        /// if yes then we attch ImageUrl to ImgUserLarge (Image)
        /// Commented By Rohit
        /// </summary>

        #region Display Deceased Member 
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
                    if (fileName.ToUpper().Contains("MPU_" + hdnDAccountId.Value + "_") == true)
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

        /// <summary> For booValidateData
        /// here we check Member detail by Name or IPI Number for this we use App_Accounts_ValidateLegelHire(Store Procedure). 
        /// this procedure use App_Accounts table. parameter @tVal is for Name(A) Or IPI(I). if data found assign account code value to Session["AccountCode"] 
        /// Commented By Rohit
        /// </summary>
        /// <returns>True or False</returns>

        #region  Check Existing Deceased Member are exists
        private Boolean booValidateData()
        {
            DataTable DT = new DataTable();

            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@FirstName", txtfirstname.Text, SqlDbType.NVarChar, 30, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@LastName", txtlastname.Text, SqlDbType.NVarChar, 45, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@IPINumber", txtIPINumber.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));

            ////added by Hariom 25-02-2023

            //parameters.Add(objGeneralFunction.GetSqlParameter("@IPINumber", txtIPINo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));

            ////end

            parameters.Add(objGeneralFunction.GetSqlParameter("@tVal", rbtValOption.SelectedValue, SqlDbType.NVarChar, 5, ParameterDirection.Input));
            DSIT_DataLayer objDAL = new DSIT_DataLayer();

            DT = objDAL.GetDataTable("App_Accounts_ValidateLegelHire", parameters.ToArray());
            if (DT.Rows.Count > 0)
            {
                Session["AccountCode"] = DT.Rows[0]["AccountCode"].ToString();
                return true;
            }
            else
            {
                Session["AccountCode"] = "";
                return false;
            }

        }

        #endregion

        /// <summary>For isnulldateofdeath
        ///here we find Deceased Member Date Of death  in our data base. if it is not in exists then we show the message 
        /// Commented By Rohit
        /// </summary>
        /// <returns>Ture or False</returns>

        #region Check Date of death
        private Boolean isnulldateofdeath()
        {

            DSIT_DataLayer DAL = new DSIT_DataLayer();

            Boolean tisNullDate = false;

            var parameters = new List<SqlParameter>();

            parameters.Add(objGeneralFunction.GetSqlParameter("@Accountcode", Session["AccountCode"], SqlDbType.NVarChar, 50, ParameterDirection.Input));

            DataSet DS = DAL.GetDataSet("App_Accounts_List_Deceased", parameters.ToArray());
            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow DR = DS.Tables[0].Rows[0];

                if (DR["DateOfDeath"].ToString() == "" || DR["DateOfDeath"].ToString() == null)
                {
                    tisNullDate = true;
                }
                else
                {
                    tisNullDate = false;
                }

                //Added by Hariom 20-02-2023

                //if (DR["IPINumber"].ToString() == "" || DR["DateOfDeath"].ToString() == null)
                //{
                //    tisNullDate = true;
                //}
                //else
                //{
                //    tisNullDate = false;
                //}

                //end

            }

            return tisNullDate;
        }

        #endregion
    }
}