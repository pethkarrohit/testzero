using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace IPRS_Member
{
    public partial class ApplicationError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindGridUsingXML();
        }

        public void ApplicationErrorDisplay(string strPagename, string strSection, string strError)
        {
        }

        protected void BindGridUsingXML()
        {
            try
            {
                DataSet myDataSet = new DataSet();
                DataTable myDatatable = new DataTable();
                DataTable myTable = new DataTable();

                if (File.Exists(Server.MapPath("~/DSIT/Application_Error.xml")))
                {
                    myDataSet.ReadXml(Server.MapPath("~/DSIT/Application_Error.xml"));

                    if (myDataSet.Tables.Count > 0)
                    {
                        DataRow[] rows = myDataSet.Tables[0].Select("SessionId = '" + Session.SessionID + "'");
                        myDatatable = myDataSet.Tables[0].Clone();
                        myTable = myDataSet.Tables[0].Clone();

                        if (rows.ToList().Count > 0)
                        {

                            for (int i = 0; i < rows.ToList().Count; i++)
                            {
                                myDatatable.Rows.Add(rows[i].ItemArray);
                            }

                            DataView view = myDatatable.DefaultView;
                            view.Sort = "LogDatetime DESC";
                            DataTable sortedDate = view.ToTable();

                            grdError.DataSource = sortedDate;
                            grdError.DataBind();

                            myDataSet.Dispose();

                        }
                        else
                        {
                            grdError.DataSource = null;
                            grdError.DataBind();
                        }
                    }
                    else
                    {
                        grdError.DataSource = null;
                        grdError.DataBind();
                    }
                }
                else
                {
                    grdError.DataSource = null;
                    grdError.DataBind();
                }
            }
            catch (Exception ex)
            {
                grdError.DataSource = null;
                grdError.DataBind();
            }
            finally
            {
                RemoveFromXML();
            }
        }

        protected void RemoveFromXML()
        {
            if (File.Exists(Server.MapPath("~/DSIT/Application_Error.xml")))
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(Server.MapPath("~/DSIT/Application_Error.xml"));

                XmlNode rootNode = xmlDoc.SelectSingleNode("//ApplicationError");

                XmlNodeList ErrorLog = rootNode.SelectNodes("ApplicationLog");

                for (int i = 0; i < ErrorLog.Count; i++)
                {
                    if (ErrorLog[i].SelectSingleNode("SessionId").InnerText.Equals(Session.SessionID))
                    {
                        rootNode.RemoveChild(ErrorLog[i]);
                        xmlDoc.Save(Server.MapPath("~/DSIT/Application_Error.xml"));
                    }
                }

            }
        }

    }


}