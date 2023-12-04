using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member.User_Controls
{

    public partial class ucTooltip : System.Web.UI.UserControl
    {
        
        public string tooltiptext = "";
        public string tooltipanchor = "";
        public string imgsrc = "";
        #region "Class level variable declaration section."
        public string strTitle = string.Empty;
        GeneralFunction genFunc = new GeneralFunction();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

    }
}