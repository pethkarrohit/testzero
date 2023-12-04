using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    public partial class Information : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                divUnderProcess_Reg.Visible = false;
                divUpdate_Details.Visible = false;
                divUpdate_Profile_Revert.Visible = false;
                divUpdate_Details_Revert.Visible = false;
                DivRejection.Visible = false;
                //if (Request.QueryString["Type"] != null)
                //{
                //    if (Convert.ToString(Request.QueryString["Type"]) == "UPR")

                //        if (Convert.ToString(Request.QueryString["Type"]) == "UPD")
                //            divUpdate_Details.Visible = true;
                //}
                if (Convert.ToString(Session["ApplicationStatus"]) == "")
                {
                    Page.Response.Redirect("UpdateProfile", false);
                }
                else if ((Convert.ToString(Session["ApplicationStatus"]) == "1" || Convert.ToString(Session["ApplicationStatus"]) == "2") && Convert.ToString(Session["AccountStatus"]) == "1")
                {
                    divUnderProcess_Reg.Visible = true; // Member On Submission first cycle of Authorization
                }
                else if ((Convert.ToString(Session["ApplicationStatus"]) == "1" || Convert.ToString(Session["ApplicationStatus"]) == "2") && Convert.ToString(Session["AccountStatus"]) == "0")
                {
                    divUpdate_Details.Visible = true; // Member On Submission After first cycle of Authorization
                }
                else if (Convert.ToString(Session["ApplicationStatus"]) == "3" && Convert.ToString(Session["AccountStatus"]) == "0")
                {
                    Page.Response.Redirect("Home", false);
                }
                else
                {
                    GetApprovalData();
                }

            }

        }
        protected void GetApprovalData()
        {

            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));

            DSIT_DataLayer DAL = new DSIT_DataLayer();
            DataTable DT = DAL.GetDataTable("App_Accounts_Authorization_CurrentStatus", parameters.ToArray());
            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {
                    Session["AuthorizationLevel"] = DT.Rows[0]["AuthorizationLevel"].ToString();
                    if (DT.Rows[0]["App_Status"].ToString() == "R" && DT.Rows[0]["AuthorizationType"].ToString().ToUpper() == "A")
                    {
                        spRevertMsg.InnerText = DT.Rows[0]["Comment"].ToString();
                        divUpdate_Profile_Revert.Visible = true;

                    }
                    else if (DT.Rows[0]["App_Status"].ToString() == "R" && DT.Rows[0]["AuthorizationType"].ToString().ToUpper() == "R")
                    {
                        spRevertMsg_RE.InnerText = DT.Rows[0]["Comment"].ToString();
                        divUpdate_Details_Revert.Visible = true;
                    }

                    else if (DT.Rows[0]["App_Status"].ToString() == "RE")
                    {
                        DivRejection.Visible = true;
                    }
                    else
                    {
                        Page.Response.Redirect("UpdateProfile", false);

                    }
                }



            }

        }

        protected void btnReviewApplication_Click(object sender, ImageClickEventArgs e)
        {
            try
            {


                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "windowKey", "window.open('App_Reports/ApplicationForm_Rpt.aspx?RID=" + clsCryptography.Encrypt(Convert.ToString(Session["AccountId"])) + "');", true);
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "windowKey", "window.open('../Reports/PdfCall.aspx?RCID=" + APIId + "&HDR=Appheader&RPT=StudentApplication&FTR=Appfooter" + "');", true);

            }
            catch (Exception ex)
            {
                objGeneralFunction.BootBoxAlert(objGeneralFunction.ReplaceASC(ex.Message), this.Page);
            }
        }

        protected void lnkSubmit_Click(object sender, EventArgs e)
        {
            //Int64 RecordId = 0;
            //string ReturnMessage = string.Empty;
            //var parameters = new List<SqlParameter>();

            //parameters.Add(objGeneralFunction.GetSqlParameter("@AccountId", Convert.ToString(Session["AccountId"]), SqlDbType.BigInt, 0, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@ApplicationStatus", 5, SqlDbType.TinyInt, 0, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@UserName", Convert.ToString(Session["AccountName"]), SqlDbType.NVarChar, 100, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            //DSIT_DataLayer objDAL = new DSIT_DataLayer();

            //objDAL.ExecuteSP("App_Accounts_AppStatus_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
            //if (RecordId > 0)
            //{
            Response.Redirect("Home");
            //}
        }
    }
}