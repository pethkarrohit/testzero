using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member.User_Controls
{
    public partial class UCDocUpload : System.Web.UI.UserControl
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
 
        public string UcAccountId = "";
        public string DocSubtype = "";
        public string DocIds = "";
        public string DocDesc = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnUcRecordId.Value = UcAccountId;
                hdnDocSubtype.Value = DocSubtype;

            }

        }
        protected void grdDocumentsPreApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDocumentName = (Label)e.Row.FindControl("lblDocumentName");
                HiddenField hdnIsCompulsary = (HiddenField)e.Row.FindControl("hdnIsCompulsary");
                if (hdnIsCompulsary.Value == "0")
                {

                    lblDocumentName.ForeColor = System.Drawing.Color.Red;

                }



            }
        }
        public void loadGrdDocumentsPreApproval()
        {

            hdnUcRecordId.Value = UcAccountId;
            hdnDocSubtype.Value = DocSubtype;
            #region "Variable declaration section."           
            DataSet ds = null;
            #endregion




            try
            {
                string DocType = string.Empty; string RegType = string.Empty;

                RegType = Convert.ToString(Session["AccountRegType"]);
                if (RegType == "I")
                    DocType = "MUPI";
                if (RegType == "C")
                    DocType = "MUPC";

                #region "Retrieving the Documnet List data from database."
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", DocIds, SqlDbType.NVarChar, 50, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentType", DocType, SqlDbType.NVarChar, 10, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocSubType", hdnDocSubtype.Value, SqlDbType.NVarChar, 50, ParameterDirection.Input));

                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                ds = objDAL.GetDataSet("Doc_LookUp_ListData_Update_IPM", parameters.ToArray());

                #endregion
                // DataRow[] DR;

                //DR = ds.Tables[0].Select("DocumentLookupId=" + strGSTLookupId);
                //if (DR.Length > 0)
                //{
                //    DR[0]["Iscompulsary"] = GetDocIdsVal(strGSTLookupId);
                //}
                //DR = ds.Tables[0].Select("DocumentLookupId=" + strOSILookupId);
                //if (DR.Length > 0)
                //{
                //    DR[0]["Iscompulsary"] = GetDocIdsVal(strOSILookupId);
                //}
                //DR = ds.Tables[0].Select("DocumentLookupId=" + strOSCLookupId);
                //if (DR.Length > 0)
                //{
                //    DR[0]["Iscompulsary"] = GetDocIdsVal(strOSCLookupId);
                //}

                //#region Changing GST Compulsory

                //if (txtDetails1_gst.Text != string.Empty)
                //{ ChangeDocVal(ds.Tables[0], strGSTLookupId); }

                //#endregion

                //#region Changing Overseas Compulsory
                //if (txtOverseasSocietyName.Text != string.Empty)
                //{
                //    if (hdnRegistrationType.Value == "I")
                //        ChangeDocVal(ds.Tables[0], strOSILookupId);
                //    else
                //        ChangeDocVal(ds.Tables[0], strOSCLookupId);
                //}
                //#endregion
                //#region Changing Permanent Compulsory
                //if (txtAddress_PR.InnerText != string.Empty)
                //{
                //    ChangeDocVal(ds.Tables[0], strAdressLookupId);

                //}
                //#endregion
                //#region Changing Company Docs

                //if (rbtEntityType.SelectedValue == "SP")
                //    ChangeDocVal(ds.Tables[0], strCompanyId_SP);
                //if (rbtEntityType.SelectedValue == "PR")
                //    ChangeDocVal(ds.Tables[0], strCompanyId_PR);
                //if (rbtEntityType.SelectedValue == "CL")
                //    ChangeDocVal(ds.Tables[0], strCompanyId_CP);

                //#endregion


                ds.Tables[0].AcceptChanges();
                ViewState["DocLookUp"] = ds.Tables[0];

                grdDocumentsPreApproval.DataSource = ds;
                grdDocumentsPreApproval.DataBind();
                if (ds.Tables[0].Rows.Count == 0)
                    hdnfilecount.Value = "-1";
                else
                    hdnfilecount.Value = "0";
               

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

        protected void btnfileupload_Click(object sender, EventArgs e)
        {
            Button btnUpload = (Button)sender;
            GridViewRow gvr = (GridViewRow)btnUpload.NamingContainer;
            FileUpload FileUploadControl = (FileUpload)gvr.FindControl("FileUpload1");
            HiddenField hdnDocLookupId = (HiddenField)gvr.FindControl("hdnDocLookupId");
            HiddenField hdnUploadedCount = (HiddenField)gvr.FindControl("hdnUploadedCount");
            Label LblFilecount = (Label)gvr.FindControl("LblFilecount");
            DataTable DT = new DataTable();
            int uploadCount = 0;
            if (hdnUploadedCount.Value != "")
                uploadCount = Convert.ToInt16(hdnUploadedCount.Value);
            if (FileUploadControl.HasFiles)
            {
                for (int i = 0; i < FileUploadControl.PostedFiles.Count; i++)
                {
                    HttpPostedFile PFile = FileUploadControl.PostedFiles[i];

                    try
                    {
                        string strFileName = string.Empty;
                        //if (PFile.ContentType == "image/jpeg" || PFile.ContentType == "image/png"
                        //    || PFile.ContentType == "application/pdf" || PFile.ContentType == "application/vnd.ms-excel"
                        //    || PFile.ContentType == "image/gif")
                        //{

                        string filename = Path.GetFileNameWithoutExtension(PFile.FileName);
                        string fileExt = Path.GetExtension(PFile.FileName);

                        string SaveAsFileName = "MRU_" + hdnUcRecordId.Value + "_" + hdnDocLookupId.Value + "_N" + (i + 1).ToString() + "_" + filename + fileExt;

                        strFileName = Server.MapPath("~/MemberRegDocs/" + SaveAsFileName);

                        Int64 ReturnId = SaveDocs_DB(0, hdnDocLookupId.Value, filename, SaveAsFileName);
                        DataTable DTLookUp = (DataTable)ViewState["DocLookUp"];
                        DataRow[] DocRowVal = null;
                        if (DTLookUp != null && ReturnId > 0)
                        {
                            FileUploadControl.SaveAs(strFileName);
                            DataRow[] DocRow = DTLookUp.Select("DocumentLookupId = " + hdnDocLookupId.Value + "");
                            DocRowVal = DTLookUp.Select("Uploaded=0");
                            if (DocRow.Length > 0)
                            {

                                if (DocRow[0]["Uploaded"].ToString() != "")
                                {
                                    DocRow[0]["Uploaded"] = 1;
                                    DocRow[0]["UploadedCount"] = Convert.ToDouble(DocRow[0]["UploadedCount"]) + 1;
                                    LblFilecount.Text = (DocRow[0]["UploadedCount"]).ToString();
                                }
                            }
                        }
                        if (ReturnId > 0)
                            objGeneralFunction.BootBoxAlert("File Uploaded Successfully", this.Page);
                        else
                        {
                            objGeneralFunction.BootBoxAlert("File already Exist change file name and upload", this.Page);
                        }
                        if (DocRowVal != null)
                            if (DocRowVal.Length > 0)
                            {
                                hdnfilecount.Value = "1";
                            }

                    }
                    catch (Exception ex)
                    {
                        objGeneralFunction.BootBoxAlert("Upload status: The file could not be uploaded.The following error occured: " + ex.Message, this.Page);
                    }
                }
            }
            else
            {
                objGeneralFunction.BootBoxAlert("Please Upload  file", this.Page);
            }





        }
        protected Int64 SaveDocs_DB(Int64 DocId, string DocLookUpId, string DocCaption, string DocFileName)
        {

            var parameters = new List<SqlParameter>();
            string ReturnMessage = string.Empty;
            Int64 RecordId = 0;
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountDocId", DocId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnUcRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", DocLookUpId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ApprovalType", "R", SqlDbType.NVarChar, 5, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocStatus", 0, SqlDbType.TinyInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentCaption", DocCaption, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocFileName", DocFileName, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocUploadDesc", DocDesc, SqlDbType.NVarChar, 100, ParameterDirection.Input));
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
        private void Display_AccountsDoc_ListData(string DocId)
        {

            #region Bind Grid Documents
            try
            {


                DSIT_DataLayer objDAL = new DSIT_DataLayer();
                var parameters = new List<SqlParameter>();
                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnUcRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", DocId, SqlDbType.BigInt, 0, ParameterDirection.Input));

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

        protected void btnViewUploadedFiles_Click(object sender, EventArgs e)
        {
            lblHeadingofFiles.Text = "Uploaded Files";

            LinkButton btnViewUploadedFiles = (LinkButton)sender;


            GridViewRow gvr = (GridViewRow)btnViewUploadedFiles.NamingContainer;

            HiddenField hdnDocLookupId = (HiddenField)gvr.FindControl("hdnDocLookupId");

            Display_AccountsDoc_ListData(hdnDocLookupId.Value);



            modViewFiles.Show();

        }
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
    }


}