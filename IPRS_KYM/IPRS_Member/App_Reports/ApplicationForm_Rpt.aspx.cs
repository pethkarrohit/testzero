using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text.html.simpleparser;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using iTextSharp.text.pdf.draw;
using System;
using System.Net;
using IPRS.App_Code;
using System.Runtime.InteropServices;

namespace IPRS_Member.App_Reports
{
    public partial class ApplicationForm_Rpt : System.Web.UI.Page
    {
        // A4 Page Size Height=11.7f,Width =8.3f This is in inches
        //  11.7*72=842 in points -- height
        //  8.3*72=595 in points --- width
        string FontName = "HELVETICA";


        Rectangle pageSize = PageSize.A4;

        float marginLeft = 15f;
        float marginRight = 15f;
        float marginTop = 15f;
        float marginBottom = 40f;
        float pdfTableFont = 11f;
        Int32 intFontSize = 11;


        float UsableWidth;
        float UsableHeight;
        float TotalWidth = 595;
        //float TotalHeight = 842;

        float TotalHeight = 1404;



        //float TotalHeight = Document.GetPageSize(PageSize.A4);


        string recordsFrom = string.Empty, recordsTo = string.Empty, reportdate = string.Empty;
        string ReportDisplayDateTime = string.Empty;
        string ReportDisplayHeaderFooterNote = string.Empty;


        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["RID"] != null)
                {
                    hdnRecordId.Value = Convert.ToString(clsCryptography.Decrypt(Request.QueryString["RID"])).Replace('~', ',');
                    hdnoutOfPage.Value = "4";
                }
                else
                {
                    hdnoutOfPage.Value = "3";
                    hdnRecordId.Value = "";
                }
                // Renu 12-02-2021
            }
        }




        private DataSet GetCompanyDetails()
        {



            #region GET COMPANY DETAILS
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@CompanyLevel", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
            DataSet myCompany = new DataSet();
            myCompany = objDAL.GetDataSet("App_Company_List", parameters.ToArray());
            #endregion


            objDAL = null;
            return myCompany;
        }


        private string getRefAccountCode(Int64 AccountId)
        {

            string PAccountCode = "";



            DSIT_DataLayer DAL = new DSIT_DataLayer();

            var parameters = new List<SqlParameter>();

            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));

            DataSet DS = DAL.GetDataSet("App_RefAccounts_List_AccountCode", parameters.ToArray());

            if (DS.Tables[0].Rows.Count > 0)
            {

                DataRow DR = DS.Tables[0].Rows[0];
                PAccountCode = DR["RefAccountCode"].ToString();
                return PAccountCode;

            }
            else
            {

                return PAccountCode;
            }



        }

        protected void btnDisplayReport_Click(object sender, EventArgs e)
        {
            DataSet myCompany = GetCompanyDetails();


            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            DataSet myDataset = new DataSet();
            






            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyIds", hdnRecordId.Value, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            myDataset = objDAL.GetDataSet("App_Accounts_List_IPM_Rpt", parameters.ToArray());
            UsableHeight = TotalHeight - (marginTop + marginBottom);
            UsableWidth = TotalWidth - (marginRight + marginLeft);

            GenerateReport(myDataset, myCompany);


        }





        private Font WriteFont(string FontStyle, float Size, int fontstyle)
        {
            if (FontStyle == "")
                FontStyle = FontName;
            Font fntNormalText = FontFactory.GetFont(FontStyle, Size, fontstyle);

            return fntNormalText;
        }

        protected void GenerateReport(DataSet DSOrder, DataSet DS_Co)
        {

            Document document = new Document(PageSize.A4, marginLeft, marginRight, 130, marginBottom);

            // string source = Server.MapPath("/IERP_Roopa/App_Reports/Process/Po.pdf");
            ///* float source1 = document.PageSize()*/;
            // PdfReader reader = new PdfReader(source);
            // PdfCopy pdfCopy;

            //float totatls = reader.GetPageSize(0);
            //var page = pdfCopy.GetImportedPage(reader, 0);
            //Rectangle mediabox = reader.GetPageSize(0);


            Font NormalFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            float HeaderRowHeight = 0;
            float FooterRowHeight = 0;
            float ContentRowHeight = 0;
            float HeaderContentRowHeight = 0;

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();


                DataTable DTAccounts = DSOrder.Tables[0];

                int PageNo = 1;

                int PageRowNo = 0;

                

                for (int i = 0; i < DTAccounts.Rows.Count; i++)
                {


                    try
                    {
                        //Creating Header
                        if (i > 0)
                        {
                            document.NewPage();
                            PageNo = 1;
                        }
                        PdfPTable TableContent = new PdfPTable(1); ;

                        TableContent.TotalWidth = UsableWidth;
                        TableContent.SetWidths(new float[] { 100f });
                        TableContent.LockedWidth = true;


                        clsDocumentHeaderFooter obj = new clsDocumentHeaderFooter();

                        obj.marginLeft = marginLeft;
                        obj.marginRight = marginRight;
                        obj.marginTop = marginTop;
                        obj.marginBottom = marginBottom;
                        obj.UsableWidth = UsableWidth;
                        obj.DS_CO = DS_Co;
                        obj.DR = DTAccounts.Rows[i];
                        writer.PageEvent = obj;

                        //CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);

                        //Create Footer

                        //CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "1/" + hdnoutOfPage.Value);


                        //Content
                        #region "Creating Personal Details" 
                        //SalesOrder Details

                            DataRow DR = DTAccounts.Rows[i];
                            string StrRegType = DR["AccountRegType"].ToString();

                            CreatePerosnalDetails(TableContent, DTAccounts.Rows[i]);
                            
                            
                            if (StrRegType=="LH" || StrRegType=="LHN")
                            {
                                CreateDeceasedInfo(TableContent, DTAccounts.Rows[i]);
                            }

                            CreateAddressInfo(TableContent, DTAccounts.Rows[i]);

                            CreateOtherInfo(TableContent, DTAccounts.Rows[i]);

                            CreateBankInfo(TableContent, DTAccounts.Rows[i]);
                        

                        //COMMENTED BY HARIOM 20-02-2023

                            //if  (StrRegType=="I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
                            //{
                            //    CreateNominationDetails(TableContent, DTAccounts.Rows[i]);
                            //}

                        



                        document.Add(TableContent);
                        document.NewPage();


                        PageRowNo = PageRowNo + 1;

                       // CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);
                        CreateLastPageInfo(document, DTAccounts.Rows[i]);
                       // CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "2/" + hdnoutOfPage.Value);

                        document.NewPage();
                        PageRowNo = PageRowNo + 1;

                        TableContent = new PdfPTable(1);

                        TableContent.TotalWidth = UsableWidth;
                        TableContent.SetWidths(new float[] { 100f });
                        TableContent.LockedWidth = true;
                       // CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);

                        //Create Footer

                      //  CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "3/" + hdnoutOfPage.Value);
                        if (hdnRecordId.Value == "" || hdnRecordId.Value == "0")
                            CreateWorkInfoBlank(TableContent);
                        else
                            CreateWorkInfo(TableContent, DTAccounts.Rows[i]);
                        document.Add(TableContent);

                        PageRowNo = PageRowNo + 1;
                        if (hdnRecordId.Value != "" && hdnRecordId.Value != "0")
                        {
                            document.NewPage();
                            TableContent = new PdfPTable(1); ;

                            TableContent.TotalWidth = UsableWidth;
                            TableContent.SetWidths(new float[] { 100f });
                            TableContent.LockedWidth = true;
                           // CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);
                            CreateDocInfo(TableContent, DTAccounts.Rows[i]);
                           // CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "4/" + hdnoutOfPage.Value);
                            document.Add(TableContent);
                        }
                        document.NewPage();
                        #endregion

                    }
                    catch (Exception ex)
                    {

                        objGeneralFunction.BootBoxAlert(ex.Message, Page);
                    }

                }
                document.Close();
                byte[] bytes = memoryStream.ToArray();
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    PdfStamper stmper = new PdfStamper(reader, stream);

                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        PdfDictionary dict = reader.GetPageN(i);
                        PdfObject objPDF = dict.GetDirectObject(PdfName.CONTENTS);
                        if (objPDF.GetType() == typeof(PRStream))
                        {
                            PRStream strm = (PRStream)objPDF;
                            byte[] data = PdfReader.GetStreamBytes(strm);
                            //strm.SetData(System.Text.Encoding.UTF8.GetBytes(new string(System.Text.Encoding.UTF8.GetString(data).ToCharArray()).Replace("^", i.ToString()).Replace("%N", pages.ToString())));
                            strm.SetData(System.Text.Encoding.Default.GetBytes(new string(System.Text.Encoding.Default.GetString(data).ToCharArray()).Replace("^", i.ToString()).Replace("%N", pages.ToString())));
                        }
                    }
                    stmper.Close();
                    bytes = stream.ToArray();
                }
                if (Request.QueryString["Stype"] == "S")
                {

                    string EmailAttachment = "IPRS_ApplicationForm_" + hdnRecordId.Value + ".pdf";
                    string strPath = Server.MapPath("~/UploadEmails\\" + EmailAttachment);
                    FileStream file = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
                    memoryStream.WriteTo(file);
                    file.Close();
                    memoryStream.Close();
                }
                else
                {
                    memoryStream.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "inline;filename=Application.pdf");

                    Response.Buffer = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                    Response.Close();
                }
            }
        }




        protected void CreatePerosnalDetails(PdfPTable TableContent, DataRow DR)
        {
            string StrRegType = DR["AccountRegType"].ToString();

            DataSet DSAdC = null; string COAccountName = string.Empty; string COMobile = string.Empty;
            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            if (StrRegType == "C")
            {
                if (hdnRecordId.Value != "0" || hdnRecordId.Value != "")
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));

                    DSAdC = DAL.GetDataSet("App_Accounts_Address_Contact_List_IPM", parameters.ToArray());
                }

                if (DSAdC != null)
                {
                    if (DSAdC.Tables[0].Rows.Count > 0)
                    {
                        COAccountName = DSAdC.Tables[0].Rows[0]["Name"].ToString();
                        COMobile = DSAdC.Tables[0].Rows[0]["ContactMobile"].ToString();
                    }
                }
            }
            iTextSharp.text.Image img = null;string photomsg = "";
            try
            {

                string Photourl = DisplayMemberImage();
                img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(Photourl));
                // img.SetAbsolutePosition(25f, 25f);
            }
            catch (Exception ex)
            {
                photomsg = ex.Message.ToString();

            }
            PdfPTable TablePersonal = new PdfPTable(5);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] {20f, 16f, 24f, 20f, 20f, });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;




            PdfPCell cell = new PdfPCell(new Phrase("PERSONAL DETAILS (BLOCK LETTERS)", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 4;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 1", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("Role Type", 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            string _entityType = string.Empty;
            if (DR["RollTypeIds"] != null && DR["RollTypeIds"].ToString() != string.Empty)
            {
                if (DR["EntityType"] != null && DR["EntityType"].ToString() != string.Empty)
                {
                    switch (DR["EntityType"].ToString())
                    {
                        case "SP":
                            _entityType = " ( Sole Proprietary Concern )";
                            break;
                        case "PR":
                            _entityType = " ( Partnership )";
                            break;
                        case "CP":
                            _entityType = " (Corporate (Pvt Ltd/Ltd Company) )";
                            break;
                        default:
                            _entityType = string.Empty;
                            break;
                    }
                }
                cell = new PdfPCell(CreateCells(DR["RollTypeNames"].ToString().ToUpper()  + _entityType, 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            }
            else
            {
                cell = new PdfPCell(CreateCells("", 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            }
            cell.Colspan = 5;
            TablePersonal.AddCell(cell);

            string appDate = "";
            string payDate = "";
            if (DR["PaymentDate"].ToString() != "")
            {
                DateTime apDate = DateTime.Parse(DR["PaymentDate"].ToString());
                payDate = apDate.ToString("dd-MM-yyyy");
            }

            if (DR["ApplicationDate"].ToString() != string.Empty)
                appDate = objGeneralFunction.FormatDate(DR["ApplicationDate"].ToString().ToUpper() , "dd-MMM-yyyy", null);
            TablePersonal.AddCell(CreateCells("Application Date :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            //TablePersonal.AddCell(CreateCells((DR["PaymentDate"].ToString() != string.Empty ? objGeneralFunction.FormatDate(DR["ApplicationDate"].ToString(), "dd-MMM-yyyy", null) : ""), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells((DR["PaymentDate"].ToString().ToUpper()  != string.Empty ? payDate : appDate), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells("Application No :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["AccountCode"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            if(img!=null)
            cell = new PdfPCell(img, true);
            else
                cell = new PdfPCell(new Phrase(photomsg));
            cell.Padding = 2;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            if (StrRegType == "C" || StrRegType == "NC")
            {
                cell.Rowspan = 10;
            }
            else
            {
                cell.Rowspan = 9;
            }
            TablePersonal.AddCell(cell);

            if (StrRegType == "C" || StrRegType == "NC")
            {
                TablePersonal.AddCell(CreateCells("Company Name :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DR["AccountName"].ToString().ToUpper() , 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.Colspan = 3;
                cell.Padding = 4;
                TablePersonal.AddCell(cell);
            }
            if (StrRegType == "C" || StrRegType == "NC")
            {
                TablePersonal.AddCell(CreateCells("Establishment Date. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DR["DOB"].ToString().ToUpper()  == "" ? "" : objGeneralFunction.FormatNullableDate(DR["DOB"].ToString().ToUpper()), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.Colspan = 2;
                cell.Padding = 4;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            }

            //if (StrRegType == "I")
            //{
            //    cell = new PdfPCell(CreateCells("(First Name | Middle Name | Last Name )", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            //    cell.Colspan = 4;
            //    TablePersonal.AddCell(cell);
            //}
            //if (StrRegType == "C")
            //{
            //    cell = new PdfPCell(CreateCells("(First Name | Middle Name | Last Name ) Proprietor / Director / Partner / Company Representative", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            //    cell.Colspan = 4;
            //    TablePersonal.AddCell(cell);
            //}


            TablePersonal.AddCell(CreateCells("Full Name :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            if (StrRegType == "C" || StrRegType == "NC")
                cell = new PdfPCell(CreateCells(DR["FirstName"].ToString().ToUpper() + " " + DR["LastName"].ToString().ToUpper(), 9, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            else
                cell = new PdfPCell(CreateCells(DR["AccountName"].ToString().ToUpper(), 9, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 3;

            TablePersonal.AddCell(cell);


            TablePersonal.AddCell(CreateCells("Alias :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountAlias"].ToString().ToUpper() , 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 3;

            TablePersonal.AddCell(cell);

            //'C' added by Hariom 31-03-23
            if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN" || StrRegType == "C")
            { 
                TablePersonal.AddCell(CreateCells("Pan No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["Detail2"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            }

            //'C' added by Hariom 31-03-23
            if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN" || StrRegType == "C")
            { 
                TablePersonal.AddCell(CreateCells("Aadhaar No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["Detail3"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                //TablePersonal.AddCell(cell);
            }



            //cell.Colspan = 3; //changed by hariom 28-03-23
            TablePersonal.AddCell(CreateCells("GST :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["Detail1"].ToString().ToUpper()  == "" ? "NA" : DR["Detail1"].ToString().ToUpper() , 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 4));
            //cell.VerticalAlignment = Rectangle.ALIGN_TOP;

            //cell.Colspan = 3;
            //TablePersonal.AddCell(CreateCells("GST :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 7));           
            //cell = new PdfPCell(CreateCells(DR["Detail1"].ToString() == "" ? "NA" : DR["Detail1"].ToString(), 9, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 7));            
            //TablePersonal.AddCell(cell);


            if (StrRegType == "NI" || StrRegType == "NC" )
            {
                // update and added by Rohit 22-05-2023 on
                TablePersonal.AddCell(CreateCells("TRC No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["TRCNo"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            }




                if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
            {
                TablePersonal.AddCell(CreateCells("Nationality :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["Nationality"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //TablePersonal.AddCell(cell);
            }


            //if (StrRegType == "NI")
            //{
            //    //pending for dual nationality

            //}

            if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
            {
                TablePersonal.AddCell(CreateCells("Father name :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                //TablePersonal.AddCell(CreateCells(DR["FatherName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell = new PdfPCell(CreateCells(DR["FatherName"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);
            }

            if (StrRegType == "NI" )
            {
                TablePersonal.AddCell(CreateCells("Social Security No :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["SocialSecurityNo"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                
            }



            //TablePersonal.AddCell(CreateCells("Role Type", 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));

            //string _entityType = string.Empty;
            //if (DR["EntityType"] != null && DR["EntityType"].ToString() != string.Empty)
            //{
            //    switch (DR["EntityType"].ToString())
            //    {
            //        case "SP":
            //            _entityType = " ( Sole Proprietary Concern )";
            //            break;
            //        case "PR":
            //            _entityType = " ( Partnership )";
            //            break;
            //        case "CP":
            //            _entityType = " (Corporate (Pvt Ltd/Ltd Company) )";
            //            break;
            //        default:
            //            _entityType = string.Empty;
            //            break;
            //    }
            //}

            //cell = new PdfPCell(CreateCells(DR["RollTypeNames"].ToString() + _entityType, 10, Font.BOLD, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            //cell.Colspan = 5;
            //TablePersonal.AddCell(cell);


            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 10f;
            cell.Colspan = 4;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);





            //----------------------------------------------------------------------------------

            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 3f;
            cell.Colspan = 5;
            cell.Border = Rectangle.TOP_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);

            if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
            {
                TablePersonal.AddCell(CreateCells("Place Of Birth :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["PlaceOfBirth"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("DOB :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells((DR["DOB"].ToString().ToUpper()  != string.Empty ? objGeneralFunction.FormatNullableDate(DR["DOB"].ToString().ToUpper() ) : ""), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            }

            TablePersonal.AddCell(CreateCells("Mobile No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            if (StrRegType == "C")
                TablePersonal.AddCell(CreateCells(COMobile.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            else
                TablePersonal.AddCell(CreateCells(DR["AccountMobile"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Telephone No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountPhone"].ToString().ToUpper()  , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("Email Address :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountEmail"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            if (StrRegType == "I" )
                cell.Colspan = 2;
            else
                cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("Alternate Email Address :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["Alt_EmailId"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));                        
            TablePersonal.AddCell(cell);

            //if (StrRegType == "I")
            //{
            TablePersonal.AddCell(CreateCells("Alternate Mobile :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountMobile_Alt"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            if (StrRegType == "I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
            {
                TablePersonal.AddCell(CreateCells("Gender :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["Gender"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 2;
                //TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("Prefered Language :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["PreferredLanguage"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 2;
                //TablePersonal.AddCell(cell);
            }

            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 10f;
            cell.Colspan = 5;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);


            cell = new PdfPCell(TablePersonal);
            cell.Padding = 2;
            cell.Border = Rectangle.TOP_BORDER;
            cell.BorderWidthTop = 2f;
            cell.Colspan = 2;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;

            TableContent.AddCell(cell);


        }


        protected void CreateDeceasedInfo(PdfPTable TableContent, DataRow DR)
        {
            try
            {


                


                string StrRegType = DR["AccountRegType"].ToString();

                string[] CityStatePara_PR; string[] CityStatePara_PM;
                string City_PM = string.Empty; string State_PM = string.Empty; string Country_PM = string.Empty; ; string Pincode_PM = string.Empty; string Address_PM = string.Empty;
                string City_PR = string.Empty; string State_PR = string.Empty; string Country_PR = string.Empty; string Pincode_PR = string.Empty; string Address_PR = string.Empty;

                iTextSharp.text.Image img = null; string photomsg = "";

                PdfPTable TablePersonal = new PdfPTable(5);

                TablePersonal.TotalWidth = UsableWidth;
                TablePersonal.SetWidths(new float[] { 18f, 22f, 18f, 23f, 20f });
                TablePersonal.LockedWidth = true;
                TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

                var parameters = new List<SqlParameter>();
                DSIT_DataLayer DAL = new DSIT_DataLayer();

                parameters.Add(objGeneralFunction.GetSqlParameter("@RefAccountCode", DR["RefAccountCode"].ToString() , SqlDbType.NVarChar, 50, ParameterDirection.Input));
                DataTable DTDECEASED = DAL.GetDataTable("App_Accounts_Deceased_List", parameters.ToArray());



                if (DTDECEASED.Rows.Count > 0)
                {


                    hdnRefAccountId.Value = getRefAccountId(DR["RefAccountCode"].ToString());

                    
                    try
                    {

                        string Photourl = DisplayDeceasedImage();
                        img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(Photourl));
                        // img.SetAbsolutePosition(25f, 25f);
                    }
                    catch (Exception ex)
                    {
                        photomsg = ex.Message.ToString();

                    }

                    if (DTDECEASED.Rows[0]["GeographicalName"].ToString() != string.Empty)
                    {
                        CityStatePara_PM = DTDECEASED.Rows[0]["GeographicalName"].ToString().Split('/');

                        for (int i = 0; i < CityStatePara_PM.Length; i++)
                        {

                            if (i == 1)
                                City_PM = CityStatePara_PM[i];
                            if (i == 2)
                                State_PM = CityStatePara_PM[i];
                            if (i == 3)
                                Country_PM = CityStatePara_PM[i];


                        }

                    }
                    Address_PM = DTDECEASED.Rows[0]["AccountAddress"].ToString();
                    Pincode_PM = DTDECEASED.Rows[0]["Pincode"].ToString();
                

                if (DR != null)
                {
                    if (DTDECEASED.Rows[0]["GeographicalName_PR"].ToString() != string.Empty)
                    {
                        CityStatePara_PR = DTDECEASED.Rows[0]["GeographicalName_PR"].ToString().Split('/');
                        for (int i = 0; i < CityStatePara_PR.Length; i++)
                        {
                            if (i == 0)
                                City_PR = CityStatePara_PR[1];
                            if (i == 1)
                                State_PR = CityStatePara_PR[2];
                            if (i == 1)
                                Country_PR = CityStatePara_PR[3];

                        }
                        Pincode_PR = DTDECEASED.Rows[0]["Pincode_PR"].ToString();
                    }
                    else
                    {
                        City_PR = City_PM;
                        State_PR = State_PM;
                        Country_PR = Country_PM;
                        Pincode_PR = Pincode_PM;
                    }
                    Address_PR = DTDECEASED.Rows[0]["AccountAddress_PR"].ToString();
                    if (Address_PR == "")
                        Address_PR = Address_PM;
                

                
            }

                if (StrRegType == "NI" || StrRegType == "NC")
                {


                    if (DR != null)
                    {
                        if (DTDECEASED.Rows[0]["Accountaddress"].ToString() != string.Empty)
                        {


                            City_PM = DTDECEASED.Rows[0]["PerCity"].ToString();
                            State_PM = DTDECEASED.Rows[0]["PerState"].ToString();
                            Country_PM = DTDECEASED.Rows[0]["PerCountryName"].ToString();

                            Address_PM = DTDECEASED.Rows[0]["AccountAddress"].ToString();
                            Pincode_PM = DTDECEASED.Rows[0]["PerZipcode"].ToString();
                        }
                    }

                    if (DR != null)
                    {
                        if (DTDECEASED.Rows[0]["Accountaddress_PR"].ToString() != string.Empty)
                        {


                            City_PR = DTDECEASED.Rows[0]["PreCity"].ToString();
                            State_PR = DTDECEASED.Rows[0]["PreState"].ToString();
                            Country_PR = DTDECEASED.Rows[0]["PreCountryName"].ToString();
                            Pincode_PR = DTDECEASED.Rows[0]["PreZipCode"].ToString();
                        }
                        else
                        {
                            City_PR = City_PM;
                            State_PR = State_PM;
                            Country_PR = Country_PM;
                            Pincode_PR = Pincode_PM;
                        }
                        Address_PR = DTDECEASED.Rows[0]["AccountAddress_PR"].ToString();
                        if (Address_PR == "")
                            Address_PR = Address_PM;
                    }

                }

                }



                PdfPCell cell = new PdfPCell(new Phrase("DECEASED INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.Border = Rectangle.NO_BORDER;
                cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                cell.Padding = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 4;
                TablePersonal.AddCell(cell);

                cell = new PdfPCell(new Phrase("PART 2", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("Deceased Member Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DTDECEASED.Rows[0]["AccountName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("Relationship", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DTDECEASED.Rows[0]["Relationship"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                if (img != null)
                    cell = new PdfPCell(img, true);
                else
                    cell = new PdfPCell(new Phrase(photomsg));
                cell.Padding = 2;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                cell.Rowspan = 9;
                TablePersonal.AddCell(cell);
                
                TablePersonal.AddCell(CreateCells("Date of Birth", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(objGeneralFunction.FormatDate(DTDECEASED.Rows[0]["DOB"].ToString().ToUpper() , "dd-MM-yyyy", null) , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                


                TablePersonal.AddCell(CreateCells("Date of Death", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(objGeneralFunction.FormatDate(DTDECEASED.Rows[0]["DateOfDeath"].ToString().ToUpper() , "dd-MM-yyyy", null),  10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("Death Certificate No", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DTDECEASED.Rows[0]["DeathCertificateNo"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("Trader Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(DTDECEASED.Rows[0]["AccountAlias"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                //cell.Colspan = 3;
                TablePersonal.AddCell(cell);


                TablePersonal.AddCell(CreateCells("Permanent Address", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(Address_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(City_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Pin/Zip Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Pincode_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(State_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Country_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                //cell = new PdfPCell();
                //cell.Padding = 0;
                //cell.FixedHeight = 10f;
                //cell.Colspan = 5;
                //cell.Border = Rectangle.NO_BORDER;
                //cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                //TablePersonal.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Padding = 0;
                //cell.FixedHeight = 10f;
                //cell.Colspan = 5;
                //cell.Border = Rectangle.TOP_BORDER;
                //cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                //TablePersonal.AddCell(cell);



                TablePersonal.AddCell(CreateCells("Present Address", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(Address_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(City_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Pin/Zip Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Pincode_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(State_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Country_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));



                //cell = new PdfPCell();
                //cell.Padding = 0;
                //cell.FixedHeight = 10f;
                //cell.Colspan = 5;
                //cell.Border = Rectangle.NO_BORDER;
                //cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                //TablePersonal.AddCell(cell);                

                


                //if (img != null)
                //    cell = new PdfPCell(img, true);
                //else
                //    cell = new PdfPCell(new Phrase(photomsg));                
                //cell.Colspan = 2;
                //cell.Border = Rectangle.NO_BORDER;
                //cell.VerticalAlignment = Rectangle.ALIGN_TOP;                                                                     
                //TablePersonal.AddCell(cell);
                



                cell = new PdfPCell(TablePersonal);
                cell.Padding = 2;
                cell.Border = Rectangle.NO_BORDER;
                cell.Colspan = 3;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;

                TableContent.AddCell(cell);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void CreateAddressInfo(PdfPTable TableContent, DataRow DR)
        {
            try
            {

                string StrRegType = DR["AccountRegType"].ToString();


                string[] CityStatePara_PR; string[] CityStatePara_PM;
                string City_PM = string.Empty; string State_PM = string.Empty; string Country_PM = string.Empty; ; string Pincode_PM = string.Empty; string Address_PM = string.Empty;                
                string City_PR = string.Empty; string State_PR = string.Empty; string Country_PR = string.Empty; string Pincode_PR = string.Empty; string Address_PR = string.Empty;
                               





                PdfPTable TablePersonal = new PdfPTable(4);

                TablePersonal.TotalWidth = UsableWidth;
                TablePersonal.SetWidths(new float[] { 20f, 30f, 15f, 35f });
                TablePersonal.LockedWidth = true;
                TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

                if (StrRegType == "I" || StrRegType == "C" || StrRegType == "LH" || StrRegType == "LHN")
                {
                

                        if (DR != null)
                        {
                            if (DR["GeographicalName"].ToString() != string.Empty)
                            {
                                CityStatePara_PM = DR["GeographicalName"].ToString().Split('/');
                                //for (int i = 0; i < CityStatePara_PM.Length; i++)
                                //{

                                //    if (i == 0)
                                //        City_PM = CityStatePara_PM[1];
                                //    if (i == 1)
                                //        State_PM = CityStatePara_PM[2];
                                //    if (i == 1)
                                //        Country_PM = CityStatePara_PM[3];


                                //}

                                //renu
                                for (int i = 0; i < CityStatePara_PM.Length; i++)
                                {

                                    if (i == 1)
                                        City_PM = CityStatePara_PM[i];
                                    if (i == 2)
                                        State_PM = CityStatePara_PM[i];
                                    if (i == 3)
                                        Country_PM = CityStatePara_PM[i];


                                }

                            }
                            Address_PM = DR["AccountAddress"].ToString();
                            Pincode_PM = DR["Pincode"].ToString();
                        }

                        if (DR != null)
                        {
                            if (DR["GeographicalName_PR"].ToString() != string.Empty)
                            {
                                CityStatePara_PR = DR["GeographicalName_PR"].ToString().Split('/');
                                for (int i = 0; i < CityStatePara_PR.Length; i++)
                                {
                                    if (i == 0)
                                        City_PR = CityStatePara_PR[1];
                                    if (i == 1)
                                        State_PR = CityStatePara_PR[2];
                                    if (i == 1)
                                        Country_PR = CityStatePara_PR[3];

                                }
                                Pincode_PR = DR["Pincode_PR"].ToString();
                            }
                            else
                            {
                                City_PR = City_PM;
                                State_PR = State_PM;
                                Country_PR = Country_PM;
                                Pincode_PR = Pincode_PM;
                            }
                            Address_PR = DR["AccountAddress_PR"].ToString();
                            if (Address_PR == "")
                                Address_PR = Address_PM;
                        }

               }

                if (StrRegType == "NI" || StrRegType == "NC")
                {


                    if (DR != null)
                    {
                            if (DR["Accountaddress"].ToString() != string.Empty)
                            {

                            
                                City_PM = DR["PerCity"].ToString(); 
                                State_PM = DR["PerState"].ToString();
                                Country_PM = DR["PerCountryName"].ToString();                                                

                                Address_PM = DR["AccountAddress"].ToString();
                                Pincode_PM = DR["PerZipcode"].ToString();
                            }
                    }

                    if (DR != null)
                    {
                        if (DR["Accountaddress_PR"].ToString() != string.Empty)
                        {
                            

                            City_PR = DR["PreCity"].ToString();
                            State_PR = DR["PreState"].ToString();
                            Country_PR = DR["PreCountryName"].ToString();
                            Pincode_PR = DR["PreZipCode"].ToString();
                        }
                        else
                        {
                            City_PR = City_PM;
                            State_PR = State_PM;
                            Country_PR = Country_PM;
                            Pincode_PR = Pincode_PM;
                        }
                        Address_PR = DR["AccountAddress_PR"].ToString();
                        if (Address_PR == "")
                            Address_PR = Address_PM;
                    }

                }





                PdfPCell cell = new PdfPCell(new Phrase("ADDRESS INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.Border = Rectangle.NO_BORDER;
                cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                cell.Padding = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                cell = new PdfPCell(new Phrase("PART 3", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                TablePersonal.AddCell(cell);


                TablePersonal.AddCell(CreateCells("Permanent Address", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(Address_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(City_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Pin/Zip Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Pincode_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(State_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Country_PM.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 10f;
                cell.Colspan = 5;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);

                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 10f;
                cell.Colspan = 5;
                cell.Border = Rectangle.TOP_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);



                TablePersonal.AddCell(CreateCells("Present Address", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell = new PdfPCell(CreateCells(Address_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 3;
                TablePersonal.AddCell(cell);

                TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(City_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Pin/Zip Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Pincode_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

                TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(State_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(Country_PR.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));



                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 10f;
                cell.Colspan = 5;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);

                cell = new PdfPCell(TablePersonal);
                cell.Padding = 2;
                cell.Border = Rectangle.NO_BORDER;

                cell.Colspan = 2;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;

                TableContent.AddCell(cell);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void CreateOtherInfo(PdfPTable TableContent, DataRow DR)
        {
            string StrRegType = DR["AccountRegType"].ToString();

            DSIT_DataLayer DAL = new DSIT_DataLayer();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@RegistrationType", DR["AccountRegType"], SqlDbType.BigInt, 0, ParameterDirection.Input));




            PdfPTable TablePersonal = new PdfPTable(4);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] { 15f, 35f, 15f, 35f });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;




            PdfPCell cell = new PdfPCell(new Phrase("OTHER INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 3;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 4", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            TablePersonal.AddCell(cell);





            //TablePersonal.AddCell(CreateCells("Role Type", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            //cell = new PdfPCell(CreateCells(DR["RollTypeNames"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            //cell.Colspan = 3;
            //TablePersonal.AddCell(cell);

            cell = new PdfPCell(CreateCells("Territory Applied For", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            string TerAppFor = "";
            if (DR["TeritoryAppFor"].ToString() == "")
            {
                TerAppFor = "NA";
            }
            else if (DR["TeritoryAppFor"].ToString() == "0356")
            { 
                TerAppFor = "INDIA";
            }
            else if (DR["TeritoryAppFor"].ToString() == "2136")
            {
                TerAppFor = "WORLD";
            }

            cell = new PdfPCell(CreateCells(TerAppFor.ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);


            cell = new PdfPCell(CreateCells("Whether Member of Other Overseas Society?", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);
            if (hdnRecordId.Value != "0" && hdnRecordId.Value != "")
            {
                if (DR["OverseasSocietyName"].ToString() == string.Empty)
                    cell = new PdfPCell(CreateCells("NO", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                else
                    cell = new PdfPCell(CreateCells("YES", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            }
            else
                cell = new PdfPCell(CreateCells("YES / NO", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(CreateCells("Name of the Other Overseas Society", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);
            cell = new PdfPCell(CreateCells(DR["OverseasSocietyName"].ToString().ToUpper()  == string.Empty ? "NA" : DR["OverseasSocietyName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);


            cell = new PdfPCell(CreateCells("Internal Identification Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);
            cell = new PdfPCell(CreateCells(DR["InternalIdentificationName"].ToString().ToUpper()  == string.Empty ? "NA" : DR["InternalIdentificationName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);

            if (StrRegType=="I" || StrRegType == "NI" || StrRegType == "LH" || StrRegType == "LHN")
            { 
                cell = new PdfPCell(CreateCells("IPI Number", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                cell.Colspan = 2;
                TablePersonal.AddCell(cell);
                cell = new PdfPCell(CreateCells(DR["IPINumber"].ToString().ToUpper()  == string.Empty ? "NA" : DR["IPINumber"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
                cell.Colspan = 2;
                TablePersonal.AddCell(cell);
            }


            cell = new PdfPCell(CreateCells("Whether Member of any Association in INDIA", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);
            cell = new PdfPCell(CreateCells(DR["AssociationName_India"].ToString().ToUpper()  == string.Empty ? "NA" : DR["AssociationName_India"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 2;
            TablePersonal.AddCell(cell);



            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 10f;
            cell.Colspan = 5;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);








            cell = new PdfPCell(TablePersonal);
            cell.Padding = 2;
            cell.Border = Rectangle.NO_BORDER;
            cell.Colspan = 2;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TableContent.AddCell(cell);


        }

        protected void CreateBankInfo(PdfPTable TableContent, DataRow DR)
        {

            string StrRegType = DR["AccountRegType"].ToString();

            PdfPTable TablePersonal = new PdfPTable(4);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] { 15f, 35f, 15f, 35f });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;




            PdfPCell cell = new PdfPCell(new Phrase("BANK INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 3;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 5", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            TablePersonal.AddCell(cell);


            TablePersonal.AddCell(CreateCells("Bank Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["BankName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));


            TablePersonal.AddCell(CreateCells("Account No", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));

            

            TablePersonal.AddCell(CreateCells(DR["BankAcNo"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Branch Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["BankBranchName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("IFSC Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["BankIFSCCode"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("MICR Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["MicrCode"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            if (StrRegType == "NI" || StrRegType == "NC")
            {
                TablePersonal.AddCell(CreateCells("Swift Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["BankSwift"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            }

            TablePersonal.AddCell(CreateCells("Currency Type", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));            
            TablePersonal.AddCell(CreateCells(DR["CurrencyName"].ToString().ToUpper()  == string.Empty ? "NA" : DR["CurrencyName"].ToString().ToUpper() , 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            //Added by Hariom 17-02-2023

           // if (hdnRefAccountId.Value == "")
            //{

                TablePersonal.AddCell(CreateCells("Account Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
                TablePersonal.AddCell(CreateCells(DR["BankAccountName"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
           // }
            //else
            //{
            //    TablePersonal.AddCell(CreateCells("Account Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            //    TablePersonal.AddCell(CreateCells(DR["AccountName"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            //}
            
            //End


            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 3f;
            cell.Colspan = 5;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);








            cell = new PdfPCell(TablePersonal);
            cell.Padding = 2;
            cell.Border = Rectangle.NO_BORDER;

            cell.Colspan = 2;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;

            TableContent.AddCell(cell);


        }





        //COMMENTED BY HARIOM 20-02-2023

        //protected void CreateNominationDetails(PdfPTable TableContent, DataRow DR)
        //{


        //    string StrRegType = DR["AccountRegType"].ToString();

        //    PdfPTable TablePersonal = new PdfPTable(5);

        //    TablePersonal.TotalWidth = UsableWidth;
        //    TablePersonal.SetWidths(new float[] { 18f, 22f, 18f, 23f, 20f, });
        //    TablePersonal.LockedWidth = true;
        //    TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

        //    var parameters = new List<SqlParameter>();
        //    DSIT_DataLayer DAL = new DSIT_DataLayer();
        //    string hh = DR["AccountId"].ToString();
        //    parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", DR["AccountId"], SqlDbType.BigInt, 0, ParameterDirection.Input));
        //    DataTable DTNominee = DAL.GetDataTable("App_Accounts_Nominee_List", parameters.ToArray());


        //    PdfPCell cell = new PdfPCell(new Phrase("NOMINEE INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
        //    cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
        //    cell.Border = Rectangle.NO_BORDER;
        //    cell.Padding = 5;
        //    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
        //    cell.Colspan = 4;
        //    TablePersonal.AddCell(cell);

        //    cell = new PdfPCell(new Phrase("PART 6", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
        //    cell.Border = Rectangle.NO_BORDER;
        //    cell.Padding = 5;
        //    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
        //    cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
        //    TablePersonal.AddCell(cell);


        //    cell = new PdfPCell();
        //    cell.Padding = 0;
        //    cell.FixedHeight = 3f;
        //    cell.Colspan = 5;
        //    cell.Border = Rectangle.NO_BORDER;
        //    cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        //    TablePersonal.AddCell(cell);

        //    iTextSharp.text.Image img = null; string photomsg = "";

        //    if (DTNominee.Rows.Count > 0)
        //    {
        //        Anchor anchor; PdfPCell pcell;
        //        Font ancfont = WriteFont("", 10, Font.BOLD);
        //        ancfont.Color = BaseColor.BLUE;


        //        //TablePersonal.AddCell(CreateCells("Gender", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //        //TablePersonal.AddCell(CreateCells("Email Id", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //        //TablePersonal.AddCell(CreateCells("Mobile", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

        //        //TablePersonal.AddCell(CreateCells("Nominee Pan Card", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //        //TablePersonal.AddCell(CreateCells("Aadhar Card", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //        //TablePersonal.AddCell(CreateCells("View Image", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

        //        for (int i = 0; i < DTNominee.Rows.Count; i++)
        //        {


        //            try
        //            {

        //                string Photourl = DisplayNomineeImage();
        //                img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(Photourl));
        //                // img.SetAbsolutePosition(25f, 25f);
        //            }
        //            catch (Exception ex)
        //            {
        //                photomsg = ex.Message.ToString();

        //            }

        //            TablePersonal.AddCell(CreateCells("First Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["FirstName"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Last Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["LastName"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            if (img != null)
        //                cell = new PdfPCell(img, true);
        //            else
        //                cell = new PdfPCell(new Phrase(photomsg));
        //            cell.Padding = 2;
        //            cell.Border = Rectangle.NO_BORDER;
        //            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        //            cell.Rowspan = 9;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Relationship", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["Relationship"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Share", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["Share"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("DOB", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["DOB"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Gender", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["NomineeGender"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);


        //            TablePersonal.AddCell(CreateCells("Email Id", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["NomineeEmailId"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Mobile", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["NomineeMobile"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Pan No", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["PanNo"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);

        //            TablePersonal.AddCell(CreateCells("Aadhar No", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        //            cell = new PdfPCell(CreateCells(DTNominee.Rows[i]["AadharNo"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        //            //cell.Colspan = 3;
        //            TablePersonal.AddCell(cell);


        //            cell = new PdfPCell();
        //            cell.Padding = 0;
        //            cell.FixedHeight = 10f;
        //            cell.Colspan = 5;
        //            cell.Border = Rectangle.NO_BORDER;
        //            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        //            TablePersonal.AddCell(cell);

        //            cell = new PdfPCell();
        //            cell.Padding = 0;
        //            cell.FixedHeight = 10f;
        //            cell.Colspan = 5;
        //            cell.Border = Rectangle.TOP_BORDER;
        //            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        //            TablePersonal.AddCell(cell);


        //            //string[] flinks = DTNominee.Rows[i]["Nomineeimage"].ToString().Split('_');
        //            //string fname = flinks[flinks.Length - 1];
        //            //string URL = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
        //            //anchor = new Anchor(new Chunk(fname, ancfont).SetUnderline(0.1f, -1f));
        //            //anchor.Reference = URL + "/MemberRegWorkDocs/" + DTNominee.Rows[i]["Nomineeimage"].ToString();
        //            //pcell = new PdfPCell(anchor);
        //            //pcell.HorizontalAlignment = Rectangle.ALIGN_TOP;
        //            //pcell.VerticalAlignment = Rectangle.ALIGN_LEFT;
        //            //pcell.Padding = 2;
        //            //pcell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
        //            //TablePersonal.AddCell(pcell);








        //            //TablePersonal.AddCell(CreateCells("First Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));                    
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["FirstName"].ToString() == string.Empty ? "NA" : DTNominee.Rows[i]["FirstName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        //            //TablePersonal.AddCell(CreateCells("Last Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["LastName"].ToString() == string.Empty ? "NA" : DTNominee.Rows[i]["FirstName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        //            //TablePersonal.AddCell(CreateCells("Relationship", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["Relationship"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));


        //            //TablePersonal.AddCell(CreateCells("Share (%)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["Share"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));


        //            //TablePersonal.AddCell(CreateCells("Date Of Birth", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["DOB"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));






        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["FirstName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["LastName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["Relationship"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["Share"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["DOB"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["NomineeGender"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["NomineeEmailId"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["NomineeMobile"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["PanNo"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
        //            //TablePersonal.AddCell(CreateCells(DTNominee.Rows[i]["AadharNo"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));







        //        }

        //    }




        //    cell = new PdfPCell();
        //    cell.Padding = 0;
        //    cell.FixedHeight = 3f;
        //    cell.Colspan = 5;
        //    cell.Border = Rectangle.NO_BORDER;
        //    cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        //    TablePersonal.AddCell(cell);


        //    cell = new PdfPCell(TablePersonal);
        //    cell.Padding = 2;
        //    cell.Border = Rectangle.NO_BORDER;
        //    cell.Colspan = 2;
        //    cell.VerticalAlignment = Rectangle.ALIGN_TOP;

        //    TableContent.AddCell(cell);



        //}




        protected void CreateWorkInfoBlank(PdfPTable TableContent)
        {
            PdfPTable TablePersonal = new PdfPTable(2);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] { 25f, 75f });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;




            PdfPCell cell = new PdfPCell(new Phrase("WORK INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;

            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 7", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            TablePersonal.AddCell(cell);


            TablePersonal.AddCell(CreateCells("Song Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Film / Non Film Album Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Language", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Category", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            cell = new PdfPCell(CreateCells("please use(Semi colon(;) as a seperator between singers :))", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell.Padding = 2;


            TablePersonal.AddCell(CreateCells("Artist/Singer-For Multiple Singers", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Author(Music Composer / Composer)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Author (Lyricist)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Publisher", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Release Year", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("Digital Link", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));


            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 3f;
            cell.Colspan = 5;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TablePersonal.AddCell(cell);








            cell = new PdfPCell(TablePersonal);
            cell.Padding = 2;
            cell.Border = Rectangle.NO_BORDER;

            cell.Colspan = 2;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;

            TableContent.AddCell(cell);


        }
        protected void CreateWorkInfo(PdfPTable TableContent, DataRow DR)
        {
            try
            {


                PdfPTable TablePersonal = new PdfPTable(11);

                TablePersonal.TotalWidth = UsableWidth;
                TablePersonal.SetWidths(new float[] { 10f, 10f, 10f, 9f, 9f, 9f, 9f, 9f, 10f, 10f, 5f});
                TablePersonal.LockedWidth = true;
                TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

                var parameters = new List<SqlParameter>();
                DSIT_DataLayer DAL = new DSIT_DataLayer();
                string hh = DR["AccountId"].ToString();
                string RefAccountCode ="";
                string RefAccountId="";

                string StrRegType = DR["AccountRegType"].ToString();

                if (StrRegType=="LH")
                {
                    RefAccountCode = getRefAccountCode(Convert.ToInt64(hdnRecordId.Value));
                    RefAccountId = getRefAccountId(RefAccountCode);

                }




                if (StrRegType == "LH")
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", RefAccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
                }
                else 
                {
                    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", DR["AccountId"], SqlDbType.BigInt, 0, ParameterDirection.Input));
                }
                DataTable DTWork = DAL.GetDataTable("App_Accounts_WorkRegistration_List_IPM", parameters.ToArray());


                PdfPCell cell = new PdfPCell(new Phrase("WORK NOTIFICATION INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 8;
                TablePersonal.AddCell(cell);

                cell = new PdfPCell(new Phrase("PART 7", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                cell.Colspan = 3;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                TablePersonal.AddCell(cell);


                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 3f;
                cell.Colspan = 11;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);

                if (DTWork.Rows.Count > 0)
                {
                    Anchor anchor; PdfPCell pcell;
                    Font ancfont = WriteFont("", 10, Font.BOLD);
                    ancfont.Color = BaseColor.BLUE;
                    TablePersonal.AddCell(CreateCells("Song Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Film Album Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Language Names", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Category", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Artist Singers", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Author Composer", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Publisher", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Author Lyricist", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

                    TablePersonal.AddCell(CreateCells("Digital Link", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("File Link", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                    TablePersonal.AddCell(CreateCells("Release Year", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

                    for (int i = 0; i < DTWork.Rows.Count; i++)
                    {

                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["SongName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Film_AlbumName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["LanguageNames"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["WorkCategory"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Artist_Singers"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Author_Composer"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Publisher"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Author_Lyricist"].ToString().ToUpper(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

                        anchor = new Anchor(new Chunk(DTWork.Rows[i]["DigitalLink"].ToString(), ancfont).SetUnderline(0.1f, -1f));                     
                        anchor.Reference = DTWork.Rows[i]["DigitalLink"].ToString();
                        pcell = new PdfPCell(anchor);
                        pcell.HorizontalAlignment = Rectangle.ALIGN_TOP;
                        pcell.VerticalAlignment = Rectangle.ALIGN_LEFT;
                        pcell.Padding = 2;
                        pcell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                        TablePersonal.AddCell(pcell);
                                                
                        string[] flinks = DTWork.Rows[i]["DocLink"].ToString().Split('_');
                        string fname = flinks[flinks.Length - 1];
                        string URL = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                        anchor = new Anchor(new Chunk(fname, ancfont).SetUnderline(0.1f, -1f));
                        anchor.Reference = URL + "/MemberRegWorkDocs/" + DTWork.Rows[i]["DocLink"].ToString();
                        pcell = new PdfPCell(anchor);
                        pcell.HorizontalAlignment = Rectangle.ALIGN_TOP;
                        pcell.VerticalAlignment = Rectangle.ALIGN_LEFT;
                        pcell.Padding = 2;
                        pcell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                        TablePersonal.AddCell(pcell);                        
                        TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["ReleaseYear"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

                    }

                }




                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 3f;
                cell.Colspan = 5;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);


                cell = new PdfPCell(TablePersonal);
                cell.Padding = 2;
                cell.Border = Rectangle.NO_BORDER;
                cell.Colspan = 2;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;

                TableContent.AddCell(cell);

                PdfPTable TableDisclaimer = new PdfPTable(3);
                TableDisclaimer.TotalWidth = UsableWidth;
                TableDisclaimer.SetWidths(new float[] { 15f, 5f, 80f });
                TableDisclaimer.LockedWidth = true;
                TableDisclaimer.DefaultCell.Border = Rectangle.NO_BORDER;

                TableDisclaimer.AddCell(new PdfPCell(new Phrase("")) { Colspan = 3, FixedHeight = 100f, Border = Rectangle.NO_BORDER });



                TableDisclaimer.AddCell(new PdfPCell(CreateCells("Disclaimer", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2)) { Colspan = 3 });
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("1.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_RIGHT, false, false, false, false, 2)) { Colspan = 2 });
                TableDisclaimer.AddCell(CreateCells("I/We hereby declare the Works submitted here along with this form by me/us is/are owned / authored by me / us.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2));
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("2.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_RIGHT, false, false, false, false, 2)) { Colspan = 2 });
                TableDisclaimer.AddCell(CreateCells("This is the Form for the submission of Released Work only (subject to verification with the supporting for work).", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2));
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("3.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_RIGHT, false, false, false, false, 2)) { Colspan = 2 });
                TableDisclaimer.AddCell(CreateCells("Member/Applicant filling up this Form will be solely responsible for the details provided & hereby indemnify IPRS for any future inconsistency.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2));
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("4.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_RIGHT, false, false, false, false, 2)) { Colspan = 2 });
                TableDisclaimer.AddCell(CreateCells("In case of any discrepancy in the future observed IPRS has the right to HOLD & put the same in the DISPUTE list (unless resolved).", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2));
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("Signature of the Member", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2)) { Colspan = 2 });
                TableDisclaimer.AddCell(CreateCells("", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 2));
                TableDisclaimer.AddCell(new PdfPCell(CreateCells("(IN CASE OF SOLE ROPERITOR/PARTNERSHIP FIRM/COMPANY/TRUST – AUTHORISED SIGNATORY STAMP REQUIRED)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 2)) { Colspan = 3 });


                cell = new PdfPCell(TableDisclaimer);
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                TableContent.AddCell(cell);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        protected void CreateDocInfo(PdfPTable TableContent, DataRow DR)
        {
            try
            {
                PdfPTable TableHeader = new PdfPTable(2);
                TableHeader.TotalWidth = UsableWidth;
                TableHeader.SetWidths(new float[] { 50f, 50f });
                TableHeader.LockedWidth = true;
                TableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPTable TablePersonal = new PdfPTable(2);

                TablePersonal.TotalWidth = UsableWidth;
                TablePersonal.SetWidths(new float[] { 10f, 90f });
                TablePersonal.LockedWidth = true;
                TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

                var parameters = new List<SqlParameter>();
                DSIT_DataLayer DAL = new DSIT_DataLayer();

                int tFlag =4;
                if (DR["AccountRegType"].ToString() == "C")
                { 
                    if (DR["EntityType"].ToString() == "SP")
                    {
                        tFlag = 0;
                    }
                    else if (DR["EntityType"].ToString() == "PR")
                    {
                        tFlag = 1;
                    }
                    else if (DR["EntityType"].ToString() == "CP")
                    {
                        tFlag = 2;
                    }
                }
                

                parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", hdnRecordId.Value, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
                parameters.Add(objGeneralFunction.GetSqlParameter("@tFlag", tFlag , SqlDbType.TinyInt, 0, ParameterDirection.Input));
                DataTable DTWork = DAL.GetDataTable("App_Accounts_Doc_ListData_IPM", parameters.ToArray());


                PdfPCell cell = new PdfPCell(new Phrase("DOCUMENT INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                TableHeader.AddCell(cell);

                cell = new PdfPCell(new Phrase("PART 8", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
                cell.Border = Rectangle.NO_BORDER;
                cell.Padding = 5;
                cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                TableHeader.AddCell(cell);

                cell = new PdfPCell();
                cell.Colspan = 2;
                cell.Border = Rectangle.NO_BORDER;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.AddElement(TableHeader);
                TablePersonal.AddCell(cell);

                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 10f;
                cell.Colspan = 2;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);

                if (DTWork.Rows.Count > 0)

                {
                    TablePersonal.AddCell(CreateCells("Sr No.", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 4));
                    TablePersonal.AddCell(CreateCells("Document", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 4));

                    Anchor anchor; PdfPCell pcell;
                    Font ancfont = WriteFont("", 10, Font.BOLD);
                    ancfont.Color = BaseColor.BLUE;
                    string URL = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                    
                    for (int i = 0; i < DTWork.Rows.Count; i++)
                    {
                        anchor = new Anchor(new Chunk(DTWork.Rows[i]["DocumentName"].ToString(), ancfont).SetUnderline(0.1f, -1f));

                        anchor.Reference = URL + "/MemberRegDocs/" + DTWork.Rows[i]["DocFileName"].ToString();


                        pcell = new PdfPCell(anchor);

                        pcell.Padding = 2;
                        pcell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;

                        pcell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                        pcell.VerticalAlignment = Rectangle.ALIGN_TOP;


                        TablePersonal.AddCell(CreateCells((i + 1).ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 4));
                        TablePersonal.AddCell(pcell);

                    }

                }




                cell = new PdfPCell();
                cell.Padding = 0;
                cell.FixedHeight = 3f;
                cell.Colspan = 5;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TablePersonal.AddCell(cell);



                cell = new PdfPCell(TablePersonal);
                cell.Padding = 2;
                cell.Border = Rectangle.NO_BORDER;

                cell.Colspan = 2;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;

                TableContent.AddCell(cell);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private PdfPCell CreateCells(string Value, int fontSize, int fontstyle, int VAlignment, int HAlignment, bool leftborder, bool rightborder, bool topborder, bool bottomborder, int padding)
        {
            PdfPCell pcell;
            Phrase phrase_Value = new Phrase();

            if (Value.Contains('~'))
            {
                string[] val = Value.Split('~');
                phrase_Value.Add(new Chunk(val[0], WriteFont("", fontSize, fontstyle)));
                phrase_Value.Add(new Chunk(Environment.NewLine));
                phrase_Value.Add(new Chunk(val[1], WriteFont("", fontSize, fontstyle)));

                pcell = new PdfPCell(phrase_Value);
                pcell.SetLeading(10f, 0f);
            }
            else
            {
                phrase_Value.Add(new Chunk(Value, WriteFont("", fontSize, fontstyle)));

                pcell = new PdfPCell(phrase_Value);

            }
            pcell.Padding = padding;
            pcell.Border = (leftborder == true ? Rectangle.LEFT_BORDER : 0) | (rightborder == true ? Rectangle.RIGHT_BORDER : 0) | (topborder == true ? Rectangle.TOP_BORDER : 0) | (bottomborder == true ? Rectangle.BOTTOM_BORDER : 0);

            pcell.HorizontalAlignment = HAlignment;
            pcell.VerticalAlignment = VAlignment;

            return pcell;


            //outerTable.AddCell(pcell);
        }
        private void buildNestedTable_Adress(PdfPTable outerTable, DataSet DS_CO)
        {
            if (DS_CO.Tables[0].Rows.Count > 0)
            {
                string CompanyName = DS_CO.Tables[0].Rows[0]["CompanyName"].ToString();
                string Telephone = DS_CO.Tables[0].Rows[0]["Telephone"].ToString();
                string Fax = DS_CO.Tables[0].Rows[0]["Fax"].ToString();
                string Address = DS_CO.Tables[0].Rows[0]["CompanyAddress"].ToString();
                string State = DS_CO.Tables[0].Rows[0]["StateName"].ToString();
                string CityName = DS_CO.Tables[0].Rows[0]["CityName"].ToString();
                string Country = DS_CO.Tables[0].Rows[0]["CountryName"].ToString();
                string Pincode = DS_CO.Tables[0].Rows[0]["Pincode"].ToString();
                string Addresstext = Address + Environment.NewLine + CityName + "-" + Pincode + " , " + State + " , " + Country;
                string Email = "membership@iprsltd.com"; //DS_CO.Tables[0].Rows[0]["Email"].ToString();
                string Web = DS_CO.Tables[0].Rows[0]["Web"].ToString();
                string GSTNO = DS_CO.Tables[0].Rows[0]["Details1"].ToString();
                string CINNO = DS_CO.Tables[0].Rows[0]["Details2"].ToString();


                string Tel = string.Empty; string FAXNo = string.Empty;
                try
                {
                    if (Telephone.ToString() != "")
                        Tel = "Tel: " + Telephone;
                    else
                        Tel = string.Empty;

                    if (Fax.ToString() != "")
                        FAXNo = "Fax: " + Fax;      //Setting the Company Telphone text.
                    else
                        FAXNo = string.Empty;




                    Phrase phrase_Adresss = new Phrase();
                    phrase_Adresss.Add(new Chunk(CompanyName.ToUpper(), WriteFont("", 13, Font.BOLD)));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    phrase_Adresss.Add(new Chunk("A Registered Copyright Society under Sec 33(3) of the Copyright Act", WriteFont("", 8, Font.BOLD)));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    phrase_Adresss.Add(new Chunk(Addresstext, WriteFont("", 9, Font.NORMAL)));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    //phrase_Adresss.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    phrase_Adresss.Add(new Chunk(Tel + " | " + FAXNo, WriteFont("", 9, Font.NORMAL)));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    if (Email != string.Empty)
                        phrase_Adresss.Add(new Chunk("Email : " + Email, WriteFont("", 9, Font.NORMAL)));
                    if (Web != string.Empty)
                        phrase_Adresss.Add(new Chunk(" | Web : " + Web, WriteFont("", 9, Font.NORMAL)));
                    phrase_Adresss.Add(new Chunk(Environment.NewLine));
                    if (CINNO != string.Empty)
                        phrase_Adresss.Add(new Chunk("CIN : " + CINNO, WriteFont("", 9, Font.NORMAL)));
                    if (GSTNO != string.Empty)
                        phrase_Adresss.Add(new Chunk(" | GSTIN : " + GSTNO, WriteFont("", 9, Font.NORMAL)));

                    PdfPCell cell = new PdfPCell(new Phrase(phrase_Adresss));
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.SetLeading(13f, 0f);

                    cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                    cell.Border = PdfPCell.NO_BORDER;



                    //Phrase phrase_Tel = new Phrase();
                    //phrase_Tel.Add(new Chunk(Tel + "\n" + FAXNo, WriteFont("", 9, Font.NORMAL)));
                    //phrase_Tel.Add(new Chunk(Environment.NewLine));
                    //phrase_Tel.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));

                    //cell = new PdfPCell(phrase_Tel);

                    //cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    //cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                    //cell.Border = PdfPCell.NO_BORDER;
                    //cell.FixedHeight = 30f;
                    //innerTable1.AddCell(cell);

                    //cell = new PdfPCell(new Phrase(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1))));
                    //cell.Colspan = 2;
                    //cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                    //cell.Border = PdfPCell.NO_BORDER;
                    //innerTable1.AddCell(cell);

                    // Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    // innerTable1.AddCell(p);
                    outerTable.AddCell(cell);
                }
                catch (Exception ex)
                {

                    objGeneralFunction.BootBoxAlert("Error While Printing Company Address ", Page);
                }
            }
        }
        protected void CreateHeader_Reports(DataSet DS_Co, Document document, int pageNo, out float HeaderRowHeight)
        {
            HeaderRowHeight = 0;
            string logoURL = "~/CompanyLogo/IprsLogo.png";
            #region "Creating Header"
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(logoURL));

            PdfPTable TableHeader = new PdfPTable(2);


            //Setting  widths for columns

            PdfPCell cell;
            TableHeader.TotalWidth = UsableWidth;
            TableHeader.SetWidths(new float[] { 10f, 80f });
            TableHeader.LockedWidth = true;


            //Setting  Header Image

            cell = new PdfPCell(img, false);
            cell.Padding = 0;
            cell.Border = PdfPCell.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            //cell.FixedHeight = 40f;

            TableHeader.AddCell(cell);




            // Adding Address After Logo

            buildNestedTable_Adress(TableHeader, DS_Co);

            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 10f;
            cell.Colspan = 2;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            TableHeader.AddCell(cell);



            TableHeader.CompleteRow();
            document.Add(TableHeader);





            #endregion
        }
        protected void CreateFooter(Document document, PdfWriter writer, out float FooterRowHeight, DataRow DR, string PgNo)
        {
            FooterRowHeight = 0;
            #region "ADD TEXT TO FOOTER"
            #endregion



            #region "Adding table on page footer."
            PdfPTable footerTable = new PdfPTable(3);
            footerTable.LockedWidth = true;
            footerTable.TotalWidth = UsableWidth;//565


            //  footerTable.LockedWidth = true;
            footerTable.SetWidthPercentage(new float[] { 33f, 33f, 34f }, pageSize);// Gridview Cell Width

            PdfPCell footercell;
            footercell = new PdfPCell(new Phrase(DR["AccountName"].ToString(), WriteFont("", 11, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_LEFT;
            footerTable.AddCell(footercell);


            footercell = new PdfPCell(new Phrase(PgNo, WriteFont("", 11, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTable.AddCell(footercell);


            footercell = new PdfPCell(new Phrase(DR["AccountCode"].ToString(), WriteFont("", 11, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_RIGHT;

            footerTable.AddCell(footercell);


            FooterRowHeight = footerTable.GetRowHeight(0);
            //document.Add(footerTable);
            #endregion
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin,
                   document.BottomMargin,
writer.DirectContent);
        }
        protected string DisplayMemberImage()
        {

            string FilePath = string.Empty;
            string OutFileName = string.Empty;

            try
            {


                FilePath = Server.MapPath("~/MemberPhoto/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MPU_" + hdnRecordId.Value + "_") == true)
                    {
                        string filenm = Path.GetFileName(fileName);
                        OutFileName = "~/MemberPhoto/" + filenm;
                        break;
                    }
                }

            }
            catch (Exception)
            {


            }
            if (OutFileName == string.Empty)
            {
                OutFileName = "~/Images/photo.png";
            }
            return OutFileName;
        }

        //protected void DisplayMemberImageDeceased()
        //{

        //    string FilePath = string.Empty;
        //    int FileExist = 0;
        //    //btnphotoViewd.Visible = false;
        //    //ImgUserd.ImageUrl = "~/Images/user.png";
        //    //ImgUserLarged.ImageUrl = "~/Images/user.png";
        //    try
        //    {


        //        FilePath = Server.MapPath("~/MemberPhoto/");
        //        string[] fileEntries = Directory.GetFiles(FilePath);
        //        foreach (string fileName in fileEntries)
        //        {
        //            if (fileName.ToUpper().Contains("MPU_" + hdnRefAccountId.Value + "_") == true)
        //            {
        //                FileExist = 1;
        //                string filenm = Path.GetFileName(fileName);
        //                //btnphotoView.NavigateUrl = "~/MemberPhoto/" + filenm;
        //                ImgUserLarged.ImageUrl = "~/MemberPhoto/" + filenm;
        //                ImgUserd.ImageUrl = "~/MemberPhoto/" + filenm;
        //                hdnphotoImageNamed.Value = filenm;
        //                break;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}

        protected string DisplayDeceasedImage()
        {

            string FilePath = string.Empty;
            string OutFileName = string.Empty;

            try
            {


                FilePath = Server.MapPath("~/MemberPhoto/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MPU_" + hdnRefAccountId.Value + "_") == true)
                    {
                        string filenm = Path.GetFileName(fileName);
                        OutFileName = "~/MemberPhoto/" + filenm;
                        break;
                    }
                }

            }
            catch (Exception)
            {


            }
            if (OutFileName == string.Empty)
            {
                OutFileName = "~/Images/photo.png";
            }
            return OutFileName;
        }

        protected string DisplayNomineeImage()
        {

            string FilePath = string.Empty;
            string OutFileName = string.Empty;

            try
            {


                FilePath = Server.MapPath("~/MemberRegWorkDocs/");
                string[] fileEntries = Directory.GetFiles(FilePath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains("MWN_" + hdnRecordId.Value + "_") == true)
                    {
                        string filenm = Path.GetFileName(fileName);
                        OutFileName = "~/MemberRegWorkDocs/" + filenm;
                        break;
                    }
                }

            }
            catch (Exception)
            {


            }
            if (OutFileName == string.Empty)
            {
                OutFileName = "~/Images/photo.png";
            }
            return OutFileName;
        }

        private string getRefAccountId(string AccountCode)
        {

            string RefAccountId = "";

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

        protected void CreateLastPageInfo(Document document, DataRow DR)
        {
            PdfPTable TblInner = new PdfPTable(3);
            TblInner.TotalWidth = UsableWidth;
            TblInner.SetWidths(new float[] { 40f, 20f, 40f });
            TblInner.LockedWidth = true;
            TblInner.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell cell = new PdfPCell(new Phrase("", WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT; cell.Border = Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Padding = 5; ; cell.Colspan = 3; cell.FixedHeight = 50f;
            TblInner.AddCell(cell);

            //

            string payDate = "";
            if (DR["PaymentDate"].ToString() != "")
            {
                DateTime apDate = DateTime.Parse(DR["PaymentDate"].ToString());
                payDate = apDate.ToString("dd-MM-yyyy");
            }
            //
            string StrContent = string.Empty;

            StrContent = "I hereby apply Membership of the society as a " + DR["RollTypeNames"].ToString() + " ";
            StrContent += "and agree, if elected and admitted to the membership of the Company, to abide by the Memorandum and Articles ";
            StrContent += "of Association and the Rules of the Company for the time being in force or as amended from time to time and to ";
            StrContent += "execute an Assignment Deed to the Company of all the Public Performing and Mechanical Rights in the Literary & ";
            StrContent += "Musical Works of which I am the Composer / Song Writer / Publisher of these Works as OWNER (including all ";
            StrContent += "future works to the exclusion of all other persons including myself) and undertake to execute, if so and whenever ";
            StrContent += "required, additional Assignments from time to time in respect of any future Literary & Musical Works which I may ";
            StrContent += "compose, write or acquire the Copyrights therein. A list of the Literary & Musical Works with which I am the ";
            StrContent += "Author / my Company is / Partnership Firm is / Proprietorship is concerned is given in the Work Notification ";
            StrContent += "Form(s) submitted herewith. I state and confirm that I am an Author / Composer / Owner Publisher and/ or Owner ";
            StrContent += "of Rights and that all the details given herewith are true and correct. I also hereby accept, declare and confirm that I ";
            StrContent += "shall not do any acts which would be against the interest of the Company and shall also adhere to the Memorandum ";
            StrContent += "of Association and Articles of Association and the Rules made there under during the course of my membership of ";
            StrContent += "the Association.";

            cell = new PdfPCell(new Phrase(StrContent, WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT; cell.Colspan = 3;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.SetLeading(15f, 0f);
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            TblInner.AddCell(cell);

          //  cell = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToString("dd-MMM-yyyy"), WriteFont("", 10, Font.NORMAL)));
            cell = new PdfPCell(new Phrase("Date: " + payDate, WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT; cell.Border = Rectangle.NO_BORDER; cell.Padding = 5; ; cell.Colspan = 3;
            TblInner.AddCell(cell);

            cell = new PdfPCell(new Phrase("Place", WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT; cell.Border = Rectangle.NO_BORDER; cell.Padding = 5;
            TblInner.AddCell(cell);

            cell = new PdfPCell(new Phrase("Signature", WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT; cell.Border = Rectangle.NO_BORDER; cell.Padding = 5;
            TblInner.AddCell(cell);

            cell = new PdfPCell(new Phrase("", WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT; cell.Border = Rectangle.BOTTOM_BORDER; cell.Padding = 5;
            TblInner.AddCell(cell);


            cell = new PdfPCell(new Phrase("", WriteFont("", 10, Font.NORMAL)));
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT; cell.Border = Rectangle.NO_BORDER; cell.Padding = 5;
            TblInner.AddCell(cell);

            cell = new PdfPCell(new Phrase("(IN CASE OF SOLE ROPERITOR/PARTNERSHIP FIRM/COMPANY/TRUST – AUTHORISED SIGNATORY STAMP REQUIRED)",
                WriteFont("", 10, Font.NORMAL)));
            cell.Colspan = 2; cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT; cell.Border = Rectangle.NO_BORDER; cell.Padding = 5;
            TblInner.AddCell(cell);



            cell = new PdfPCell(new Phrase("Approving Director Signature", WriteFont("", 10, Font.BOLD)));
            cell.Colspan = 3;
            cell.FixedHeight = 100f;
            cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            TblInner.AddCell(cell);

            //28-01-2020 .... 
            bool isFileExist = false;
            string AuthUserDocPath = string.Empty; MemoryStream ms = null;

            if (DR["AUTHUSER"].ToString() != "0" && DR["AUTHUSER"].ToString() != string.Empty)//FIND APPLICATION APPROVED OR NOT
            {
                // AuthUserDocPath = "https://lic.iprs.org/Webservice/licservice.asmx/getemployeeImage?UserID=" + DR["AUTHUSER"].ToString();///System.Configuration.ConfigurationManager.AppSettings["AuthSignPath"] + DR["AUTHUSER"].ToString() + ".png";
                AuthUserDocPath = "~/EmployeeImages/" + DR["AUTHUSER"].ToString() + ".png";
                // try
                //{
                //    using (WebClient webClient = new WebClient())
                //    {
                //        var data = webClient.DownloadData(AuthUserDocPath);
                //        ms = new MemoryStream(data, 0, data.Length);
                //        isFileExist = true;
                //    }
                //}
                //catch (Exception ex)
                //{

                //   TblInner.AddCell(new PdfPCell(new Phrase(ex.Message, WriteFont("", 10, 0))) { Border = 0, Colspan = 3, HorizontalAlignment = 1, PaddingTop = 10f });
                //}


                try
                {
                    if (File.Exists(Server.MapPath(AuthUserDocPath)))
                    {
                        isFileExist = true;
                    }

                }
                catch (Exception ex)
                {
                    TblInner.AddCell(new PdfPCell(new Phrase(ex.Message, WriteFont("", 10, 0))) { Border = 0, Colspan = 3, HorizontalAlignment = 1, PaddingTop = 10f });
                }
            }
            if (isFileExist)
            {
                //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Server.MapPath(AuthUserDocPath));
                cell = new PdfPCell(img, false);
                //cell.MinimumHeight = 80f;
                cell.Colspan = 3;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TblInner.AddCell(cell);

                TblInner.AddCell(new PdfPCell(new Phrase("Approved Date: " + DR["APPROVEDATE"].ToString(), WriteFont("", 10, 0))) { Border = 0, Colspan = 3, HorizontalAlignment = 1, PaddingTop = 10f });
            }
            else
            {
                cell = new PdfPCell();
                cell.MinimumHeight = 80f;
                cell.Colspan = 3;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.NO_BORDER;
                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                TblInner.AddCell(cell);
            }




            document.Add(TblInner);
        }
    }
}