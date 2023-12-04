
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    
    public partial class ApplicationMember : System.Web.UI.MasterPage
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_PreInit(object sender, EventArgs e)
        { }
        protected void Page_Init(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetNoStore();
            if (Session["AccountId"] == null)
                Response.Redirect("MemberLogin", true);//Temproary Basis
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();
            try
            {
                if (!IsPostBack)
                {
                    lblUserName.Text = Session["AccountName"].ToString();
                    //lblCompanyName.Text = Session["CompanyName"].ToString();
                    //lblBranchName.Text = Session["BranchName"].ToString();
                    #region "When ApprovalStatus is No then allow to remove style and left menu just show the Update Profile form"

                    //if (Session["ApprovalStatus"].ToString() == "Yes")
                    //{
                    //    divLeftMenu.Style.Add("display", "none");
                    //    myDiv.Style.Remove("Class");
                    //    myDiv.Attributes.Add("Class", "container");
                    //    divtopnav.Style.Remove("Class");
                    //    divtopnav.Style.Add("display", "block");
                    //    divtopnav.Style.Add("margin-left", "0px");
                    //    divtopnav.Style.Add("z-index", "2");
                    //    menu_toggle.Style.Add("display", "none");
                    //}
                    #endregion
                    #region "CREATING LITERAL FOR CHANGE OF BRANCH"
                    //if (Session["AccessibleCompanyId"].ToString() != string.Empty)
                    //{

                    //    lblBranchDetails.Text = "<li><a href='ChangeBranch.aspx?BId=" + Session["DefaultBranchId"].ToString() + "'><i class='fa fa-exchange pull-right'></i>" + Session["DefaultBranchName"].ToString() + "</a></li>";
                    //    var parameters = new List<SqlParameter>();
                    //    parameters.Add(objGeneralFunction.GetSqlParameter("@RecordKeyIds", Session["AccessibleCompanyId"].ToString(), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
                    //    DataSet myDataSet = new DataSet();
                    //    DSIT_DataLayer objDAL = new DSIT_DataLayer();
                    //    myDataSet = objDAL.GetDataSet("App_Company_Accesible_List", parameters.ToArray());

                    //    for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
                    //    {
                    //        lblBranchDetails.Text = lblBranchDetails.Text + "<li><a href='ChangeBranch.aspx?BId=" + myDataSet.Tables[0].Rows[i]["BranchId"].ToString() + "'><i class='fa fa-exchange pull-right'></i>" + myDataSet.Tables[0].Rows[i]["BranchName"].ToString() + "</a></li>";
                    //    }
                    //    myDataSet.Dispose();
                    //    parameters.Clear();
                    //    objDAL = null;
                    //    lblBranchDropDown.Visible = true;
                    //}
                    //else
                    //{
                    //    lblBranchDropDown.Visible = false;
                    //}

                    #endregion


                }
            }
            catch (Exception ex)
            {

            }


        }
    }
}