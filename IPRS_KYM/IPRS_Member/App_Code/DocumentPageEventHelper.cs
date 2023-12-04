using System;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace IPRS_Member.App_Code
{
    class DocumentPageEventHelper : PdfPageEventHelper
    {
        PdfContentByte headerPdfCB, footerPdfcb;
        PdfTemplate headerTemplate, footerTemplate;

        public Int32 intFontSize = 10;
        public string companyName = string.Empty;
        public string logoURL = string.Empty;
        public string reportType = string.Empty;
        public string filterFrom = string.Empty, filterTo = string.Empty;
        public string recordsFrom = string.Empty, recordsTo = string.Empty;
        public string printedBy = string.Empty, softwareVersion = string.Empty, headerNote = string.Empty, footerNote = string.Empty;
        public float pdfTableFont;
        //Processing text or the text that will be going to added to the template.
        string text;

        public BaseFont bfFonts;
        iTextSharp.text.Image imghead;

        //Variable is declared to utilize for current report line filled indexes (In terms of float) towards X-Axis.
        float currentLine_X_Size, currentLine_Y_Size, pageNo_X_Size;


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            #region "Initiating PdfContentByte and PdfTemplate variables."
            //Below objects will be used for Header contents.
            headerPdfCB = writer.DirectContent;
            headerTemplate = headerPdfCB.CreateTemplate(50, 50);

            //Below objects will be used for Footer contents.
            footerPdfcb = writer.DirectContent;
            footerTemplate = footerPdfcb.CreateTemplate(50, 50);
            #endregion
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            #region "Drawing the Header section of the Report."
            #region "Adding the Company Name and image."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetTop(40f);

            text = companyName;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize + 2);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);

            #region "Adding the image to the header."
            imghead = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath(logoURL));
            imghead.SetAbsolutePosition((document.PageSize.Width - imghead.Width) - document.RightMargin, document.PageSize.GetTop(46f));
            //imghead.ScaleAbsolute(42f, 32f);
            #endregion

            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize + 2);
            #endregion

            #region "Adding the Header Note."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetTop(55f);

            text = headerNote;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Drawing the header line."
            headerPdfCB.SetColorStroke(BaseColor.BLACK);
            headerPdfCB.SetLineWidth(0.5f);
            headerPdfCB.MoveTo(document.LeftMargin, document.PageSize.GetTop(70f));
            headerPdfCB.LineTo((document.PageSize.Width - document.RightMargin) + 4f, document.PageSize.GetTop(70f));
            headerPdfCB.Stroke();
            #endregion

            #region "Adding the Report Type name."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetTop(85f);

            text = reportType;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding the Filter From and Filter To fields."  
            currentLine_X_Size = 0f;
            currentLine_Y_Size = document.PageSize.GetTop(105f);

            #region "Adding Filter From Label."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;

            text = "Filter From";      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Filter From value."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 25f;

            headerPdfCB.BeginText();

            text = ": " + filterFrom;      //Setting the template text.

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Filter To Label."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin + 240f;

            headerPdfCB.BeginText();

            text = "Filter To";      //Setting the template text.

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Filter To value."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 38f;

            headerPdfCB.BeginText();

            text = ": " + filterTo;      //Setting the template text.

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion
            #endregion

            #region "Adding the Records From and Records To fields."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = 0f;
            currentLine_Y_Size = document.PageSize.GetTop(125f);

            #region "Adding Records From Label."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;

            text = "Records From";      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(document.LeftMargin, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Records From value."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 10f;

            text = ": " + recordsFrom;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Records To Label."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin + 240f;

            text = "Records To";      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Records To value."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 25f;

            text = ": " + recordsTo;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.AddImage(imghead);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion
            #endregion

            #region "Adding the Report Type name."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetTop(85f);

            text = reportType;      //Setting the template text.

            headerPdfCB.BeginText();
            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding Printed By and software version to the Footer."
            #region "Adding the Printed By Label"
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin + 200f;
            currentLine_Y_Size = document.PageSize.GetBottom(document.BottomMargin) - 35f;

            text = "Printed By - ";      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding the Printed By Value"
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 0f;
            currentLine_Y_Size = document.PageSize.GetBottom(document.BottomMargin) - 35f;

            text = printedBy;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Adding the Software Version."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + 45f;
            currentLine_Y_Size = document.PageSize.GetBottom(document.BottomMargin) - 35f;

            text = softwareVersion;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion
            #endregion

            #region "Adding the footer name."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetBottom(document.BottomMargin) - 23f;

            text = footerNote;      //Setting the template text.

            headerPdfCB.BeginText();

            headerPdfCB.SetFontAndSize(bfFonts, intFontSize);
            headerPdfCB.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);
            headerPdfCB.ShowText(text);
            headerPdfCB.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion

            #region "Drawing the Footer line."
            headerPdfCB.SetColorStroke(BaseColor.BLACK);
            headerPdfCB.SetLineWidth(0.5f);
            headerPdfCB.MoveTo(document.LeftMargin, document.PageSize.GetBottom(document.BottomMargin - 12f));
            headerPdfCB.LineTo((document.PageSize.Width - document.RightMargin), document.PageSize.GetBottom(document.BottomMargin - 12f));
            headerPdfCB.Stroke();
            #endregion
            #endregion

            //Adding Header content template field to the document.
            headerPdfCB.AddTemplate(headerTemplate, document.LeftMargin, document.PageSize.GetTop(document.TopMargin));
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            #region "Adding page number to the page footer."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = document.LeftMargin;
            currentLine_Y_Size = document.PageSize.GetBottom(document.BottomMargin) - 35f;

            text = "Page " + writer.PageNumber.ToString() + " of ";      //Setting the template text.

            footerPdfcb.BeginText();

            footerPdfcb.SetFontAndSize(bfFonts, intFontSize);
            footerPdfcb.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);

            footerPdfcb.ShowText(text);
            footerPdfcb.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            pageNo_X_Size = currentLine_X_Size;
            #endregion

            //Adding footer content template field to the document.
            footerPdfcb.AddTemplate(footerTemplate, document.LeftMargin + bfFonts.GetWidthPoint(text, intFontSize), document.PageSize.GetBottom(document.BottomMargin) - 35f);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            #region "Drawing the Footer section of the Report."
            #region "Adding the Total number of pages to the Report."
            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = 0;
            currentLine_Y_Size = 0;

            footerTemplate.BeginText();

            text = (writer.PageNumber - 1).ToString();      //Setting the template text.

            footerTemplate.SetFontAndSize(bfFonts, intFontSize);
            footerTemplate.SetTextMatrix(currentLine_X_Size, currentLine_Y_Size);

            footerTemplate.ShowText(text);
            footerTemplate.EndText();

            //Setting up the current line occupied indexes (In terms of float) till this position.
            currentLine_X_Size = currentLine_X_Size + bfFonts.GetWidthPoint(text, intFontSize);
            #endregion
            #endregion
        }

    }
}
