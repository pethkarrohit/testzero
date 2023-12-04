using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace IPRS_Member
{
    public partial class MemberLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            throw new System.ArgumentException("Parameter cannot be null", "original");

        }
    }
}