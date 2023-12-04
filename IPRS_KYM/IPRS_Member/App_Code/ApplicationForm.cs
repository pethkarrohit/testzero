using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

public class ApplicationForm
{

    private string _RecordKeyId;

    public string RecordKeyId
    {
        get { return _RecordKeyId; }
        set { _RecordKeyId = value; }
    }


    string FontName = "HELVETICA";
    static string hdnoutOfPage = "";

    Rectangle pageSize = PageSize.A4;

    float marginLeft = 15f;
    float marginRight = 15f;
    float marginTop = 15f;
    float marginBottom = 20f;
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

    protected void GeneratPDFReport()
    {
        DataSet myCompany = GetCompanyDetails();

        if (RecordKeyId == "")
            hdnoutOfPage = "2";
        else
            hdnoutOfPage = "3";

        DSIT_DataLayer objDAL = new DSIT_DataLayer();
        var parameters = new List<SqlParameter>();
        DataSet myDataset = new DataSet();
        parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyIds", RecordKeyId, SqlDbType.NVarChar, 100, ParameterDirection.Input));
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

        Document document = new Document(PageSize.A4, marginLeft, marginRight, marginTop, marginBottom);

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

                    CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);

                    //Create Footer

                    CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "1/" + hdnoutOfPage);


                    //Content
                    #region "Creating Personal Details" 
                    //SalesOrder Details
                    CreatePerosnalDetails(TableContent, DTAccounts.Rows[i]);

                    CreateAddressInfo(TableContent, DTAccounts.Rows[i]);

                    CreateOtherInfo(TableContent, DTAccounts.Rows[i]);

                    CreateBankInfo(TableContent, DTAccounts.Rows[i]);


                    document.Add(TableContent);
                    document.NewPage();
                    PageRowNo = PageRowNo + 1;

                    TableContent = new PdfPTable(1); ;

                    TableContent.TotalWidth = UsableWidth;
                    TableContent.SetWidths(new float[] { 100f });
                    TableContent.LockedWidth = true;
                    CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);

                    //Create Footer

                    CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "2/" + hdnoutOfPage);
                    if (RecordKeyId == "" || RecordKeyId == "0")
                        CreateWorkInfoBlank(TableContent);
                    else
                        CreateWorkInfo(TableContent, DTAccounts.Rows[i]);
                    document.Add(TableContent);

                    if (RecordKeyId != "" && RecordKeyId != "0")
                    {
                        document.NewPage();
                        TableContent = new PdfPTable(1); ;

                        TableContent.TotalWidth = UsableWidth;
                        TableContent.SetWidths(new float[] { 100f });
                        TableContent.LockedWidth = true;
                        CreateHeader_Reports(DS_Co, document, PageNo, out HeaderRowHeight);
                        CreateFooter(document, writer, out FooterRowHeight, DTAccounts.Rows[i], "3/" + hdnoutOfPage);
                        CreateDocInfo(TableContent, DTAccounts.Rows[i]);
                    }

                    document.Add(TableContent);
                    #endregion

                }
                catch (Exception ex)
                {

                    //objGeneralFunction.BootBoxAlert(ex.Message, Page);
                }

            }
            document.Close();
            byte[] bytes = memoryStream.ToArray();

            if (HttpContext.Current.Request.QueryString["Stype"] == "S")
            {

                string EmailAttachment = "IPRS_ApplicationForm_" + RecordKeyId + ".pdf";
                string strPath = HttpContext.Current.Server.MapPath("~/UploadEmails\\" + EmailAttachment);
                FileStream file = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
                memoryStream.WriteTo(file);
                file.Close();
                memoryStream.Close();
            }
            else
            {
                memoryStream.Close();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AddHeader("content-disposition", "inline;filename=Application.pdf");

                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.BinaryWrite(bytes);
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Close();
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
            if (RecordKeyId != "0" || RecordKeyId != "")
            {
                parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", RecordKeyId, SqlDbType.BigInt, 0, ParameterDirection.Input));

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


        string Photourl = DisplayMemberImage();
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(Photourl));
        // img.SetAbsolutePosition(25f, 25f);

        PdfPTable TablePersonal = new PdfPTable(5);

        TablePersonal.TotalWidth = UsableWidth;
        TablePersonal.SetWidths(new float[] { 20f, 20f, 20f, 20f, 20f, });
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

        TablePersonal.AddCell(CreateCells("Application Date :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells((DR["ApplicationDate"].ToString() != string.Empty ? objGeneralFunction.FormatDate(DR["ApplicationDate"].ToString(), "dd-MMM-yyyy", null) : ""), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        TablePersonal.AddCell(CreateCells("Application No :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["AccountCode"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        cell = new PdfPCell(img, true);
        cell.Padding = 2;
        cell.Border = Rectangle.NO_BORDER;
        cell.VerticalAlignment = Rectangle.ALIGN_TOP;
        if (StrRegType == "C")
        {
            cell.Rowspan = 8;
        }
        else
        {
            cell.Rowspan = 7;
        }
        TablePersonal.AddCell(cell);


        TablePersonal.AddCell(CreateCells("Pancard :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["Detail2"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        TablePersonal.AddCell(CreateCells("Aadhar No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["Detail3"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));


        if (StrRegType == "C")
        {
            TablePersonal.AddCell(CreateCells("Company Name :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountName"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.Colspan = 3;
            cell.Padding = 4;
            TablePersonal.AddCell(cell);
        }
        if (StrRegType == "C")
        {
            TablePersonal.AddCell(CreateCells("Establishment Date. :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["DOB"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.Colspan = 2;
            cell.Padding = 4;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("(DD-MM-YYYY)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));


        }
        if (StrRegType == "I")
        {
            cell = new PdfPCell(CreateCells("(First Name | Middle Name | Last Name )", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 4;
            TablePersonal.AddCell(cell);
        }
        if (StrRegType == "C")
        {
            cell = new PdfPCell(CreateCells("(First Name | Middle Name | Last Name ) Proprietor / Director / Partner / Company Representative", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 4;
            TablePersonal.AddCell(cell);
        }


        TablePersonal.AddCell(CreateCells("Full Name :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        if (StrRegType == "C")
            cell = new PdfPCell(CreateCells(COAccountName, 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        else
            cell = new PdfPCell(CreateCells(DR["AccountName"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.Colspan = 3;

        TablePersonal.AddCell(cell);


        TablePersonal.AddCell(CreateCells("Alias :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell = new PdfPCell(CreateCells(DR["AccountAlias"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.Colspan = 3;

        TablePersonal.AddCell(cell);


        TablePersonal.AddCell(CreateCells("GST :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell = new PdfPCell(CreateCells(DR["Detail1"].ToString() == "" ? "NA" : DR["Detail1"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.VerticalAlignment = Rectangle.ALIGN_TOP;

        if (StrRegType == "C")
            cell.Colspan = 3;
        TablePersonal.AddCell(cell);


        if (StrRegType == "I")
        {
            TablePersonal.AddCell(CreateCells("Nationality :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(DR["Nationality"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        }




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


        TablePersonal.AddCell(CreateCells("Place Of Birth :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["PlaceOfBirth"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        TablePersonal.AddCell(CreateCells("DOB :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells((DR["DOB"].ToString() != string.Empty ? objGeneralFunction.FormatNullableDate(DR["DOB"].ToString()) : ""), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        TablePersonal.AddCell(CreateCells("(DD-MM-YYYY)", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));


        TablePersonal.AddCell(CreateCells("Mobile No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        if (StrRegType == "C")
            TablePersonal.AddCell(CreateCells(COMobile, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        else
            TablePersonal.AddCell(CreateCells(DR["AccountMobile"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        TablePersonal.AddCell(CreateCells("Telephone No. :-", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell = new PdfPCell(CreateCells(DR["AccountPhone"].ToString().ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.Colspan = 2;
        TablePersonal.AddCell(cell);

        if (StrRegType == "I")
        {
            TablePersonal.AddCell(CreateCells("Alternate Mobile :-", 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(DR["AccountMobile_Alt"].ToString(), 9, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 4;
            TablePersonal.AddCell(cell);
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
    protected void CreateAddressInfo(PdfPTable TableContent, DataRow DR)
    {
        try
        {



            string[] CityStatePara_PR; string[] CityStatePara_PM;
            string City_PM = string.Empty; string State_PM = string.Empty; string Country_PM = string.Empty; ; string Pincode_PM = string.Empty; string Address_PM = string.Empty;

            string City_PR = string.Empty; string State_PR = string.Empty; string Country_PR = string.Empty; string Pincode_PR = string.Empty; string Address_PR = string.Empty;




            PdfPTable TablePersonal = new PdfPTable(4);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] { 20f, 30f, 15f, 35f });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

            if (DR != null)
            {
                if (DR["GeographicalName"].ToString() != string.Empty)
                {
                    CityStatePara_PM = DR["GeographicalName"].ToString().Split('/');
                    for (int i = 0; i < CityStatePara_PM.Length; i++)
                    {

                        if (i == 0)
                            City_PM = CityStatePara_PM[0];
                        if (i == 1)
                            State_PM = CityStatePara_PM[1];
                        if (i == 1)
                            Country_PM = CityStatePara_PM[2];



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
                            City_PR = CityStatePara_PR[0];
                        if (i == 1)
                            State_PR = CityStatePara_PR[1];
                        if (i == 1)
                            Country_PR = CityStatePara_PR[2];



                    }
                    Pincode_PR = DR["Pincode_PR"].ToString();
                }
                Address_PR = DR["AccountAddress_PR"].ToString();
                if (Address_PR == "")
                    Address_PR = "Same As Above";
            }



            PdfPCell cell = new PdfPCell(new Phrase("ADDRESS INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 3;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 2", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            TablePersonal.AddCell(cell);


            TablePersonal.AddCell(CreateCells("Permanent Address", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            cell = new PdfPCell(CreateCells(Address_PM, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 3;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(City_PM, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells("Pincode", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(Pincode_PM, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(State_PM, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(Country_PM, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

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
            cell = new PdfPCell(CreateCells(Address_PR, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            cell.Colspan = 3;
            TablePersonal.AddCell(cell);

            TablePersonal.AddCell(CreateCells("City", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(City_PR, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells("Pincode", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(Pincode_PR, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

            TablePersonal.AddCell(CreateCells("State", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(State_PR, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
            TablePersonal.AddCell(CreateCells("Country", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
            TablePersonal.AddCell(CreateCells(Country_PR, 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));



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

        cell = new PdfPCell(new Phrase("PART 3", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
        cell.Border = Rectangle.NO_BORDER;
        cell.Padding = 5;
        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
        cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
        TablePersonal.AddCell(cell);





        TablePersonal.AddCell(CreateCells("Role Type", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell = new PdfPCell(CreateCells(DR["RollTypeNames"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.Colspan = 3;
        TablePersonal.AddCell(cell);


        cell = new PdfPCell(CreateCells("Whether Member of Other Overseas Society?", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell.Colspan = 2;
        TablePersonal.AddCell(cell);
        if (RecordKeyId != "0" && RecordKeyId != "")
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
        cell = new PdfPCell(CreateCells(DR["OverseasSocietyName"].ToString() == string.Empty ? "NA" : DR["OverseasSocietyName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
        cell.Colspan = 2;
        TablePersonal.AddCell(cell);

        cell = new PdfPCell(CreateCells("Whether Member of any Association in INDIA", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        cell.Colspan = 2;
        TablePersonal.AddCell(cell);
        cell = new PdfPCell(CreateCells(DR["AssociationName_India"].ToString() == string.Empty ? "NA" : DR["AssociationName_India"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));
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

        cell = new PdfPCell(new Phrase("PART 4", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
        cell.Border = Rectangle.NO_BORDER;
        cell.Padding = 5;
        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
        cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
        TablePersonal.AddCell(cell);


        TablePersonal.AddCell(CreateCells("Bank Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["BankName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));


        TablePersonal.AddCell(CreateCells("Account No", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["BankAcNo"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        TablePersonal.AddCell(CreateCells("Branch Name", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["BankBranchName"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        TablePersonal.AddCell(CreateCells("IFSC Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["BankIFSCCode"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));

        TablePersonal.AddCell(CreateCells("MICR Code", 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, false, 6));
        TablePersonal.AddCell(CreateCells(DR["MicrCode"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, false, false, false, true, 6));



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

        cell = new PdfPCell(new Phrase("PART 5", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
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


            PdfPTable TablePersonal = new PdfPTable(10);

            TablePersonal.TotalWidth = UsableWidth;
            TablePersonal.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
            TablePersonal.LockedWidth = true;
            TablePersonal.DefaultCell.Border = Rectangle.NO_BORDER;

            var parameters = new List<SqlParameter>();
            DSIT_DataLayer DAL = new DSIT_DataLayer();

            parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyId", DR["AccountId"], SqlDbType.BigInt, 0, ParameterDirection.Input));
            DataTable DTWork = DAL.GetDataTable("App_Accounts_WorkRegistration_List_IPM", parameters.ToArray());


            PdfPCell cell = new PdfPCell(new Phrase("WORK NOTIFICATION INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 7;
            TablePersonal.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 5", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            cell.Colspan = 3;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
            TablePersonal.AddCell(cell);


            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 3f;
            cell.Colspan = 10;
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
                    TablePersonal.AddCell(CreateCells(DTWork.Rows[i]["Author_Lyricist"].ToString(), 10, Font.NORMAL, Rectangle.ALIGN_TOP, Rectangle.ALIGN_LEFT, true, true, true, true, 2));

                    anchor = new Anchor(new Chunk(DTWork.Rows[i]["DigitalLink"].ToString(), ancfont).SetUnderline(0.1f, -1f));
                    anchor.Reference = DTWork.Rows[i]["DigitalLink"].ToString();
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
        }
        catch (Exception)
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

            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", RecordKeyId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentLookupId", 0, SqlDbType.BigInt, 0, ParameterDirection.Input));
            DataTable DTWork = DAL.GetDataTable("App_Accounts_Doc_ListData_IPM", parameters.ToArray());


            PdfPCell cell = new PdfPCell(new Phrase("DOCUMENT INFO", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 5;
            TableHeader.AddCell(cell);

            cell = new PdfPCell(new Phrase("PART 6", WriteFont("", 10, Font.BOLD)));//dt.Columns[j].ToString()
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
                string URL = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
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
            string Email = DS_CO.Tables[0].Rows[0]["Email"].ToString();
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
                phrase_Adresss.Add(new Chunk(CompanyName.ToUpper(), WriteFont("", 11, Font.BOLD)));
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

                //objGeneralFunction.BootBoxAlert("Error While Printing Company Address ", Page);
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


            FilePath = HttpContext.Current.Server.MapPath("~/MemberPhoto/");
            string[] fileEntries = Directory.GetFiles(FilePath);
            foreach (string fileName in fileEntries)
            {
                if (fileName.ToUpper().Contains("MPU_" + RecordKeyId + "_") == true)
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
}
