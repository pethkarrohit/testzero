using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.Routing;
using System.Data.SqlClient;

namespace IPRS_Member
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            RegisterRoutes(RouteTable.Routes);
            Page.Response.Redirect("MemberLogin", false);
        }

        private void RegisterRoutes(RouteCollection routes)
        {

            //routes.Clear();

             
            //routes.MapPageRoute("MemberLogin", "MemberLogin", "~/MemberLogin.aspx");
            //routes.MapPageRoute("Profile", "UpdateProfile", "~/UpdateProfile.aspx");
            //routes.MapPageRoute("Main", "Main", "~/Main.aspx");
            //routes.MapPageRoute("Home", "Home", "~/Home.aspx");
            //routes.MapPageRoute("LogOut", "LogOut", "~/LogOut.aspx");
            //routes.MapPageRoute("MemberChangePassword", "MemberChangePassword", "~/MemberChangePassword.aspx");
            //routes.MapPageRoute("MemberWelcome", "Welcome", "~/MemberWelcome.aspx");
            //routes.MapPageRoute("ForgotPassword", "ForgotPassword", "~/ForgotPassword.aspx");


        }
    }
}