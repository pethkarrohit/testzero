using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            divMsg.InnerHtml += "HttpContext.Current.Request.Url.PathAndQuery  <br>" + strPathAndQuery;

            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery,  / )<br>" + strUrl;

            divMsg.InnerHtml += "<hr><br>";

            divMsg.InnerHtml += "HttpContext.Current.Request.Url<br>" + HttpContext.Current.Request.Url;
            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "HttpContext.Current.Request.Url.AbsoluteUri <br>" + HttpContext.Current.Request.Url.AbsoluteUri;
            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "HttpContext.Current.Request.Url.AbsolutePath <br>" + HttpContext.Current.Request.Url.AbsolutePath;
            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "HttpContext.Current.Request.Url.Authority <br>" + HttpContext.Current.Request.Url.Authority;

            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "HttpContext.Current.Request.Url.Host <br>" + HttpContext.Current.Request.Url.Host;

            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "Server.MapPath('.') <br>" + Server.MapPath(".");

            divMsg.InnerHtml += "<hr><br>";
            divMsg.InnerHtml += "Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath <br>" + Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;


        }

    
    }
}