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

namespace IPRS_Member.User_Controls
{
    public partial class ucNomineeDetails : System.Web.UI.UserControl
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
        string MessageReturn = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.TextBox.DisabledCssClass = "";
            if (Session["AccountId"] == null)
                Response.Redirect("MemberLogin", false);//Temproary Basis

            if (!IsPostBack)
            {


                ViewState["NomeneeId"] = "0";
                hdnRecordId.Value = Session["AccountId"].ToString();
                hdnRegistrationType.Value = Session["AccountRegType"].ToString();


                //rbtRegistrationType_SelectedIndexChanged(null, null);
                //rbtRegistrationType.SelectedValue = "I";
                //rbtRegistrationType_SelectedIndexChanged(null, null);
                RBLMinor_SelectedIndexChanged(null, null);
                RBLMinor.SelectedValue = "0";


                DisplayWorkDetails();



            }
        }


        private void DivEnaDis()
        {

            if (hdnRegistrationType.Value.ToString() == "I" || hdnRegistrationType.Value.ToString() == "NI" || hdnRegistrationType.Value.ToString() == "SC" || hdnRegistrationType.Value.ToString() == "NSC")
            {
                divFirstName.Visible = true;
                divLastName.Visible = true;
                divDOB.Visible = true;
                divRelationship.Visible = true;
                divGender.Visible = true;
                //divUploadPhoto.Visible = true;
                divEmail.Visible = true;
                divMobile.Visible = true;
                divshare.Visible = true;
            }


        }

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

        protected Int64 NomineeDetailSave(Int64 intRecordId, string strNomineeId)
        {
            string strMode = string.Empty;
            Int64 RecordId = 0;

            var parameters = new List<SqlParameter>();


            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeId", strNomineeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", intRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeName", txtFirstName.Text + " " + TxtLastName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@RelationShip", DDLRelationship.SelectedValue, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DOB", txtDOB.Text, SqlDbType.DateTime, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@Minor", RBLMinor.SelectedValue, SqlDbType.TinyInt, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@GuardianName", txtGuardianName.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@GuardianMobile", txtGuardMobileNo.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@PanNo", txtpanno.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AadharNo", txtAadhar.Text, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeGender", rbtnGender.SelectedValue, SqlDbType.NVarChar, 20, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeImage", "", SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeEmailId", txtEmail.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeMobile", txtMobile.Text, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@Share", txtshare.Text, SqlDbType.Float, 3, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@cokudUserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;

            objDAL.ExecuteSP("App_Accounts_Nominee_Manage", parameters.ToArray(), out ReturnMessage, out RecordId);


            if (RecordId == 0 || ReturnMessage.Contains("RECORD ALREADY EXIST"))
            {
                return RecordId;
            }

            string[] savedfile = SaveWorkFile(FPWork, RecordId);

            if (savedfile[1] == string.Empty)
            {
                FalseWork_delete(RecordId);
                RecordId = 0;
                return RecordId;
            }
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeId", RecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeImage", savedfile[1], SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            objDAL.ExecuteSP("App_Accounts_Nominee_Update", parameters.ToArray(), out ReturnMessage, out RecordId);

            DisplayWorkDetails();

            txtFirstName.Text = string.Empty;
            TxtLastName.Text = string.Empty;
            DDLRelationship.SelectedIndex = 0;
            txtDOB.Text = string.Empty;

            rbtnGender.SelectedIndex = 0;
            RBLMinor.SelectedIndex = 0;
            txtGuardianName.Text = string.Empty;
            txtGuardMobileNo.Text = string.Empty;
            txtAadhar.Text = string.Empty;
            txtpanno.Text = string.Empty;

            txtEmail.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtshare.Text = string.Empty;
            ViewState["NomeneeId"] = "0";
            txtFirstName.Focus();

            return RecordId;
        }



        private void DisplayWorkDetails(string strRecordId = "0")
        {



            #region Bind Grid Notification

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", strRecordId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));


            DataSet MyDataSet = new DataSet();

            MyDataSet = objDAL.GetDataSet("App_Accounts_Nominee_List", parameters.ToArray());



            if (MyDataSet == null)
            {
                objGeneralFunction.BootBoxAlert("ERROR EXECUTING RANGE DETAILS RECORDS. PLEASE CONTACT ADMINISTRATOR", Page);
                return;
            }

            ViewState["DTWorkNotification"] = MyDataSet.Tables[0];


            gvWork.DataSource = MyDataSet.Tables[0];
            gvWork.DataBind();


            objDAL = null;
            MyDataSet.Dispose();
            #endregion Bind Grid WorkNotification
        }

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
                    objGeneralFunction.BootBoxAlert("Upload status: Error getting file", this.Page);
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



        protected void FalseWork_delete(Int64 NomineeId)
        {
            ///* Code to Delete Employee Record */
            Int64 RecordId;


            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeId", NomineeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            #region "EXECUTING DELETE PROCEDURE"
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;


            objDAL.ExecuteSP("App_Accounts_Nominee_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);



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
                    DataRow myDataRow = DTBWorkNotification.Select("NomineeId=" + NomineeId).FirstOrDefault();
                    if (myDataRow != null)
                    {
                        myDataRow.Delete();
                        string FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                        int fileDel = FileDelete("MWN_" + hdnRecordId.Value + "_" + NomineeId + "_", FilePath);
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
        protected void btnWorkDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            string NomineeId = ((HiddenField)gvr.FindControl("hdnNomineeId")).Value;


            Int64 RecordId;


            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@NomineeId", NomineeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            #region "EXECUTING DELETE PROCEDURE"
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            string ReturnMessage = string.Empty;


            objDAL.ExecuteSP("App_Accounts_Nominee_Delete", parameters.ToArray(), out ReturnMessage, out RecordId);



            if (ReturnMessage == string.Empty)
                objGeneralFunction.BootBoxAlert("ERROR IN DELETING DATA. PLEASE CONTACT ADMINISTRATOR", this.Page);
            else
            {
                DataTable DTBWorkNotification = new DataTable();
                if (ViewState["DTWorkNotification"] != null)
                    DTBWorkNotification = (DataTable)ViewState["DTWorkNotification"];
                if (RecordId > 0)
                {
                    DataRow myDataRow = DTBWorkNotification.Select("NomineeId=" + NomineeId).FirstOrDefault(); // finds all rows with id==2 and selects first or null if haven't found any
                    if (myDataRow != null)
                    {
                        myDataRow.Delete();
                        string FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                        int fileDel = FileDelete("MWN_" + hdnRecordId.Value + "_" + NomineeId + "_", FilePath);
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





        protected void rbtRegistrationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            divFirstName.Visible = false;
            divLastName.Visible = false;
            divDOB.Visible = false;
            divRelationship.Visible = false;
            divGender.Visible = false;
            //divUploadPhoto.Visible = false;            
            divEmail.Visible = false;
            divMobile.Visible = false;
            divshare.Visible = false;

            DivEnaDis();

        }





        protected void btnAddWork_Click(object sender, EventArgs e)
        {
            bool flag = true;
            #region "Saving Work Notification"
            if (hdnRecordId.Value == "0")
            {
                objGeneralFunction.BootBoxAlert("UNABLE TO TRACK MASTER RECORD ID. PLEASE GO BACK AND START AGAIN", Page);
                return;
            }
            if (RBLMinor.SelectedValue == "1" && txtGuardianName.Text.Trim() == string.Empty)
            {
                objGeneralFunction.BootBoxAlert("ENTER GUARDIAN NAME", Page);
                return;
            }


            string strMsg = "";
            if (ValidateData(out strMsg) == false)
            {
                objGeneralFunction.BootBoxAlert(strMsg, Page);
                return;
            }

            //if (booPerGreaterhundered() == true )
            //{
            //    objGeneralFunction.BootBoxAlert("TOTAL PERCENTAGE OF NOMINEE SHOULD NOT GREATER THEN 100%", Page);
            //    return;
            //}



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

                if (ViewState["NomeneeId"].ToString() != "0")
                {
                    string FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                    int fileDel = FileDelete("MWN_" + hdnRecordId.Value + "_" + ViewState["NomeneeId"].ToString() + "_", FilePath);
                }

                Int64 RecordId_Nominee = NomineeDetailSave(Convert.ToInt64(hdnRecordId.Value), ViewState["NomeneeId"].ToString());
                if (RecordId_Nominee == 0)
                {
                    objGeneralFunction.BootBoxAlert("Failed To Save Nominee. Try Again Or Contact Adminstrator !!", Page);
                    return;
                }
            }

            else
            {
                objGeneralFunction.BootBoxAlert("PLEASE SELECT IMAGE FILE TO UPLOAD", Page);

            }


            #endregion
        }

        private bool ValidateData(out string strMsg)
        {
            double TotalPer;
            HiddenField hdValue;
            HiddenField hdNominneId; HiddenField hdhdnPan; HiddenField hdnAadharNo;
            TotalPer = 0; strMsg = "";


            TotalPer = txtshare.Text == "" ? 0 : Convert.ToDouble(txtshare.Text);

            foreach (GridViewRow row in gvWork.Rows)
            {
                hdValue = (HiddenField)row.FindControl("hdnshare");
                hdNominneId = (HiddenField)row.FindControl("hdnNomineeId");
                hdhdnPan = (HiddenField)row.FindControl("hdnPan");
                hdnAadharNo = (HiddenField)row.FindControl("hdnAadharNo");

                if (hdNominneId.Value.ToString() != ViewState["NomeneeId"].ToString())
                {
                    TotalPer = TotalPer + Convert.ToDouble(hdValue.Value);

                    if (hdhdnPan.Value == txtpanno.Text.Trim())
                    {
                        strMsg = "PAN NO ALREADY EXIST.";
                        return false;
                    }
                    if (hdnAadharNo.Value != string.Empty && txtAadhar.Text.Trim() != string.Empty)
                    {
                        if (hdnAadharNo.Value == txtAadhar.Text.Trim())
                        {
                            strMsg = "AADHAR ALREADY EXIST.";
                            return false;
                        }
                    }
                }
            }

            if (TotalPer > 100)
            {
                strMsg = "TOTAL PERCENTAGE OF NOMINEE SHOULD NOT GREATER THEN 100%";
                return false;
            }

            return true;


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








        protected void gvWork_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hypworkfile = (HyperLink)e.Row.FindControl("hypworkfile");
                HiddenField hdnNomineeId = (HiddenField)e.Row.FindControl("hdnNomineeId");


                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].ToolTip = gvWork.HeaderRow.Cells[i].Text;
                }
                string filename = "";

                filename = Getfile_Folder("MWN_" + hdnRecordId.Value + "_" + hdnNomineeId.Value + "_", "~/MemberRegWorkDocs/");

                hypworkfile.NavigateUrl = "~/MemberRegWorkDocs/" + filename;
                if (filename == string.Empty)
                    hypworkfile.ToolTip = "File Not Uploaded";
            }

            //ToolTip = '<%#Eval("Workfile").ToString()==""?"File Not Uploaded":"" %>' Target = "_blank" NavigateUrl = '<%#Eval("Workfile") %>'
        }



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

        protected void RBLMinor_SelectedIndexChanged(object sender, EventArgs e)
        {

            divGuardian.Visible = false;
            divGuardMobile.Visible = false;

            if (RBLMinor.SelectedValue == "0")
            {
                divGuardian.Visible = false;
                divGuardMobile.Visible = false;
                txtGuardianName.Text = "";
                txtGuardMobileNo.Text = "";


            }
            else if (RBLMinor.SelectedValue == "1")
            {
                divGuardian.Visible = true;
                divGuardMobile.Visible = true;
            }
        }


        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            string NomineeId = ((HiddenField)gvr.FindControl("hdnNomineeId")).Value;

            ViewState["NomeneeId"] = NomineeId;

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", NomineeId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));

            DataTable MyDatatable = new DataTable();

            MyDatatable = objDAL.GetDataTable("App_Accounts_Nominee_List", parameters.ToArray());
            if (MyDatatable.Rows.Count > 0)
            {
                DisplayData(MyDatatable.Rows[0]);
            }
        }

        public TextBox DOB
        {
            get { return txtDOB; }
            set { txtDOB = value; }
        }

        public RadioButtonList RMinor
        {
            get { return RBLMinor; }
            set { RBLMinor = value; }
        }

        private void DisplayData(DataRow DR)
        {
            try
            {

                txtFirstName.Text = DR["FirstName"].ToString();
                TxtLastName.Text = DR["LastName"].ToString();
                DDLRelationship.SelectedValue = DR["RelationShip"].ToString();
                txtDOB.Text = DR["DOB"].ToString();
                RBLMinor.SelectedValue = DR["Minorvalue"].ToString();
                RBLMinor_SelectedIndexChanged(null, null);
                //if (DR["Minor"].ToString() == "No")
                //{
                //    RBLMinor.SelectedValue = "0";
                //    divGuardian.Visible = false;
                //    divGuardMobile.Visible = false;
                //}
                //else if (DR["Minor"].ToString() == "Yes")
                //{
                //    RBLMinor.SelectedValue = "1";
                //    divGuardian.Visible = true;
                //    divGuardMobile.Visible = true;

                //}

                txtGuardianName.Text = DR["GuardianName"].ToString();
                txtGuardMobileNo.Text = DR["GuardianMobile"].ToString();
                txtpanno.Text = DR["PanNo"].ToString();
                txtAadhar.Text = DR["AadharNo"].ToString();
                rbtnGender.SelectedValue = DR["NomineeGender"].ToString();
                txtMobile.Text = DR["NomineeMobile"].ToString();
                txtEmail.Text = DR["NomineeEmailId"].ToString();
                txtshare.Text = DR["Share"].ToString();




            }
            catch (Exception e)
            {

            }
        }
    }
}
