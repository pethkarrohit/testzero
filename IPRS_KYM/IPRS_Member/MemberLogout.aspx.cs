using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace IPRS_Member
{
    public partial class MemberLogout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                if (File.Exists("~/DSIT/" + Session["UserId"].ToString() + ".xml") == false)
                {
                    try
                    {
                        File.Delete(Server.MapPath("~/DSIT/" + Session["UserId"].ToString() + ".xml"));
                    }
                    catch { }
                }
                Session.Abandon();
            }
            Session["AccountId"] = null;
            Response.Redirect("~/Default.aspx", false);

        }
    }
}