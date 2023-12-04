using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
namespace IPRS.App_Code
{
    public class clsDocumentHeaderFooter : PdfPageEventHelper
    {
        public Document document { get; set; }
        public PdfWriter writer { get; set; }
        public PdfPTable tblHeader { get; set; }
        public PdfPTable tblFooter { get; set; }
        public float marginLeft { get; set; }
        public float marginRight { get; set; }
        public float marginTop { get; set; }
        public float marginBottom { get; set; }
        public float UsableWidth { get; set; }
        public float UsableHeight { get; set; }


        public string HeaderNote { get; set; }
        public string FooterNote { get; set; }

        public DataSet _DS_CO;

        public DataSet DS_CO
        {
            get { return _DS_CO; }
            set { _DS_CO = value; }
        }

        public DataRow _DR;

        public DataRow DR
        {
            get { return _DR; }
            set { _DR = value; }
        }

        private void CreateHeader(PdfWriter writer, Document document)
        {



            string logoURL = "~/CompanyLogo/IprsLogo.png";

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(logoURL));


            // float[] HeaderCols = new float[] { 10f, 80f };
            //tblHeader = new PdfPTable(HeaderCols);
            //tblHeader.TotalWidth = UsableWidth;
            //tblHeader.LockedWidth = true;

            //tblHeader.AddCell(new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });

            //Setting  widths for columns
            tblHeader = new PdfPTable(2);
            PdfPCell cell;
            tblHeader.TotalWidth = UsableWidth;
            tblHeader.SetWidths(new float[] { 10f, 80f });
            tblHeader.LockedWidth = true;


            //Setting  Header Image

            cell = new PdfPCell(img, false);
            cell.Padding = 0;
            cell.Border = PdfPCell.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            //cell.FixedHeight = 40f;

            tblHeader.AddCell(cell);




            // Adding Address After Logo

            tblHeader.AddCell(buildNestedTable_Adress());

            cell = new PdfPCell();
            cell.Padding = 0;
            cell.FixedHeight = 10f;
            cell.Colspan = 2;
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Rectangle.ALIGN_TOP;
            tblHeader.AddCell(cell);



        //    tblHeader.CompleteRow();


            //tblHeader.AddCell(new PdfPCell(tblHeader) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP });
        }
        private void CreateFooter(PdfWriter writer, Document document)
        {
            //tblFooter = new PdfPTable(new float[] { 60, 40 });
            //tblFooter.TotalWidth = UsableWidth;
            //tblFooter.LockedWidth = true;



            //tblFooter.AddCell(new PdfPCell() { Border = 0, FixedHeight = 130, Colspan = tblFooter.NumberOfColumns });
            //tblFooter.AddCell(new PdfPCell() { Border = 1, Colspan = tblFooter.NumberOfColumns, Padding = 2 });
            //tblFooter.AddCell(new PdfPCell(new Phrase("Page - " + writer.PageNumber.ToString() + " of ^", SetFont("", 10, Font.NORMAL))) { Border = 0, HorizontalAlignment = 2, Colspan = tblFooter.NumberOfColumns });


            #region "ADD TEXT TO FOOTER"
            #endregion



            tblFooter = new PdfPTable(3);
            tblFooter.LockedWidth = true;
            tblFooter.TotalWidth = UsableWidth;//565


            //  footerTable.LockedWidth = true;
            tblFooter.SetWidthPercentage(new float[] { 33f, 33f, 34f }, PageSize.A4);// Gridview Cell Width

            PdfPCell footercell;
            footercell = new PdfPCell(new Phrase(DR["AccountName"].ToString(), WriteFont("", 10, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_LEFT;
            tblFooter.AddCell(footercell);


            footercell = new PdfPCell(new Phrase("^/%N", WriteFont("", 10, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_CENTER;
            tblFooter.AddCell(footercell);


            footercell = new PdfPCell(new Phrase(DR["AccountCode"].ToString(), WriteFont("", 10, Font.NORMAL)));//dt.Columns[j].ToString()
            footercell.Border = Rectangle.NO_BORDER;//Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            footercell.Padding = 2;
            footercell.HorizontalAlignment = Element.ALIGN_RIGHT;

            tblFooter.AddCell(footercell);



        }



        private PdfPCell buildNestedTable_Adress()
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

            PdfPCell cell = new PdfPCell();
            try
            {





                string Tel = string.Empty; string FAXNo = string.Empty;

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



                cell = new PdfPCell(new Phrase(phrase_Adresss));
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.SetLeading(13f, 0f);

                cell.VerticalAlignment = Rectangle.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;



            }
            catch (Exception ex)
            {


            }
            return cell;

        }
        string FontName = "HELVETICA";
        private Font WriteFont(string FontStyle, float Size, int fontstyle)
        {
            if (FontStyle == "")
                FontStyle = FontName;
            Font fntNormalText = FontFactory.GetFont(FontStyle, Size, fontstyle);

            return fntNormalText;
        }
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);


        }


        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            CreateHeader(writer, document);


            CreateFooter(writer, document);



            tblHeader.WriteSelectedRows(0, -1, marginLeft, document.PageSize.GetTop(marginTop), writer.DirectContent);

            tblFooter.WriteSelectedRows(0, -1, marginLeft, document.PageSize.GetBottom(marginBottom), writer.DirectContent);



        }


        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);
            string logoURL = "~/CompanyLogo/IprsLogo.png";
            iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(logoURL));
            JPG.SetAbsolutePosition(125, 300);
            PdfContentByte waterMark;

        }

        protected Font SetFont(string FontName, int FontSize, int Style)
        {
            if (FontName == "")
                FontName = "Arial";
            return FontFactory.GetFont(FontName, FontSize, Style, BaseColor.BLACK);
        }
    }
}