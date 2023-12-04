using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{

    public partial class Home : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //App_Reports/ApplicationForm_Rpt.aspx?RID=<%# clsCryptography.Encrypt( Session["AccountId"].ToString()) %>
                anurl.HRef = "App_Reports/ApplicationForm_Rpt.aspx?RID=" + clsCryptography.Encrypt(Session["AccountId"].ToString());
                if (Session["AccountStatus"] != null)
                {
                    if (Convert.ToString(Session["AccountStatus"]) == "1")
                    {
                        divUpdateData.Visible = false;
                        divUpdateProfile.Visible = true;
                        // divUpdateNominee.Visible = true;
                        string test = "ASCV";
                    }
                    else
                    {
                        divUpdateProfile.Visible = true; //change by rohit for member can update profile after final approval 11/07/2023
                        divUpdateData.Visible = false;
                        if (Convert.ToString(Session["AccountRegType"]) == "C")
                            divUpdateBasicInfo.Visible = true;
                        else
                            divUpdateBasicInfo.Visible = false;
                    }
                }
              

            }
        }
    }
}