using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Web;


namespace IPRS_Member.App_Reports
{
    public partial class App_CheckList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
        }


        protected void GeneratePDF()
        {

            #region "REGISTER PAGE"
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=test.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            #endregion

            Document pdfDoc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);



        }

    }
}