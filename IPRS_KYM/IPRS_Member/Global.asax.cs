using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;

namespace IPRS_Member
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                RegisterRoutes(RouteTable.Routes);
                VIEWSTATEDeleteCache();
                PARAMETERDeleteCache();
            }
            catch (Exception ex) { }
        }
        static void RegisterRoutes(RouteCollection routes)
        {
            //Route Name   : Index 
            //Route URL    : Home
            //Physical File: Index.aspx


            //routes.MapPageRoute("Default", "MemberLogin", "~/MemberLogin.aspx");
            //routes.MapPageRoute("Home", "Home", "~/Home.aspx");
            ////routes.MapPageRoute("UpdateProfile", "~/UpdateProfile", "~/UpdateProfile.aspx");
            //routes.MapPageRoute("MemberRegistration", "~/MemberRegistration", "~/MemberRegistration.aspx");
            //routes.MapPageRoute("Login", "Login", "~/MemberLogin.aspx");


            //routes.MapPageRoute("MemberLogout", "MemberLogout", "~/MemberLogout.aspx");
            //routes.MapPageRoute("ChangePassword", "ChangePassword", "~/ChangePassword.aspx");
            //routes.MapPageRoute("MemberChangePassword", "MemberChangePassword", "~/MemberChangePassword.aspx");
            //routes.MapPageRoute("ForgotPassword", "ForgotPassword", "~/ForgotPassword.aspx");

            routes.Clear();
            routes.EnableFriendlyUrls();
            routes.MapPageRoute("MemberLogin", "MemberLogin", "~/MemberLogin.aspx");
            routes.MapPageRoute("UpdateProfile", "UpdateProfile", "~/UpdateProfile.aspx");
            routes.MapPageRoute("Main", "Main", "~/Main.aspx");
            routes.MapPageRoute("Home", "Home", "~/Home.aspx");
            routes.MapPageRoute("LogOut", "LogOut", "~/MemberLogOut.aspx");
            routes.MapPageRoute("MemberChangePassword", "MemberChangePassword", "~/MemberChangePassword.aspx");
            routes.MapPageRoute("MemberWelcome", "Welcome", "~/MemberWelcome.aspx");
            routes.MapPageRoute("ForgotPassword", "ForgotPassword", "~/ForgotPassword.aspx");
            routes.MapPageRoute("MemberRegistration", "MemberRegistration", "~/MemberRegistration.aspx");
            routes.MapPageRoute("UpdateAddressDetails", "UpdateAddressDetails", "~/UpdateAddressDetails.aspx");
            routes.MapPageRoute("UpdateBankInfo", "UpdateBankInfo", "~/UpdateBankInfo.aspx");
            routes.MapPageRoute("UpdateGSTDetails", "UpdateGSTDetails", "~/UpdateGSTDetails.aspx");
            routes.MapPageRoute("UpdateBasicInfo", "UpdateBasicInfo", "~/UpdateBasicInfo.aspx");
            routes.MapPageRoute("Information", "Information", "~/Information.aspx");
            routes.MapPageRoute("UpdateNominee", "UpdateNominee", "~/UpdateNomineeDetails.aspx");


        }
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Preflight request comes with HttpMethod OPTIONS
            // The following line solves the error message
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            //{
            //    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            //    // If any http headers are shown in preflight error in browser console add them below
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Pragma, Cache-Control, Authorization ");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
            //    HttpContext.Current.Response.End();
            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            string strError = string.Empty;

            // Get the exception object.
            Exception exc = Server.GetLastError();
            if (exc == null)
                return;

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                strError = exc.Message.ToString();

                //Redirect HTTP errors to HttpError page
                Server.Transfer("ApplicationError.aspx");
            }
            else if (exc.InnerException is ViewStateException)
            {
                strError = exc.Message.ToString();
            }
            else
            {
                strError = exc.InnerException.ToString();
            }

            strError = Request.Url.ToString() + Environment.NewLine + "Error Message: -" + strError;

            insertintoWriteXML(strError);

            Server.ClearError();
            HttpContext.Current.Application.Add("test", strError);
            Response.Redirect("~/ApplicationError.aspx", false);


        }
        public void insertintoWriteXML(string strErrorDesc)
        {
            try
            {
                if (File.Exists(Server.MapPath("~/DSIT/Application_Error.xml")) == false)
                {
                    XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/DSIT/Application_Error.xml"), System.Text.Encoding.UTF8);
                    //Start XM DOcument
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    //ROOT Element
                    writer.WriteStartElement("ApplicationError");

                    //ApplicationError
                    writer.WriteEndElement();
                    //End XML Document
                    writer.WriteEndDocument();
                    //Close writer
                    writer.Close();
                }
                string session = string.Empty;
                try { session = Session.SessionID; } catch { };
                XDocument doc = XDocument.Load(Server.MapPath("~/DSIT/Application_Error.xml"));
                XElement ApplicationError = doc.Element("ApplicationError");

                ApplicationError.Add(new XElement("ApplicationLog",
                    new XElement("LogDateTime", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")),
                    new XElement("SessionId", session),
                    new XElement("ErrorLog", strErrorDesc)));
                doc.Save(Server.MapPath("~/DSIT/Application_Error.xml"));
            }
            catch (Exception ex) { }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //VIEWSTATEDeleteCache();
            //PARAMETERDeleteCache();
        }

        protected void VIEWSTATEDeleteCache()
        {
            IFormatProvider dateFormatProvider = new System.Globalization.CultureInfo("en-GB", true);
            try
            {
                string cachePath = "~/App_Data/Cache";
                System.IO.DirectoryInfo Dir = new System.IO.DirectoryInfo(Server.MapPath(cachePath));
                if (System.IO.Directory.Exists(Server.MapPath(cachePath)))
                {
                    string[] files = System.IO.Directory.GetFiles(Server.MapPath(cachePath), "VIEWSTATE_*.cache");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = file;
                            string[] fileinf = file.Split('\\');
                            if (fileinf.Length > 0)
                                filename = fileinf[fileinf.Length - 1].ToString();
                            System.IO.FileInfo[] FileList = Dir.GetFiles(filename, System.IO.SearchOption.AllDirectories);

                            TimeSpan TimeDifference = (Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss", dateFormatProvider), dateFormatProvider) - Convert.ToDateTime(FileList[0].LastWriteTimeUtc.ToString("dd-MMM-yyyy HH:mm:ss"), dateFormatProvider));

                            //if (Convert.ToDateTime(FileList[0].LastWriteTimeUtc.ToString("dd-MMM-yyyy HH:mm:ss"), dateFormatProvider) < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss", dateFormatProvider), dateFormatProvider))
                            if (TimeDifference.TotalHours >= 12)    //(TimeDifference.TotalDays >= 1)
                                System.IO.File.Delete(file);
                        }
                        catch { }
                        finally { }
                    }
                }
            }
            catch { }
            finally { }
        }

        protected void PARAMETERDeleteCache()
        {
            IFormatProvider dateFormatProvider = new System.Globalization.CultureInfo("en-GB", true);
            try
            {
                string cachePath = "~/App_Data/Cache";
                System.IO.DirectoryInfo Dir = new System.IO.DirectoryInfo(Server.MapPath(cachePath));
                if (System.IO.Directory.Exists(Server.MapPath(cachePath)))
                {
                    string[] files = System.IO.Directory.GetFiles(Server.MapPath(cachePath), "PARAMETER_*.cache");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = file;
                            string[] fileinf = file.Split('\\');
                            if (fileinf.Length > 0)
                                filename = fileinf[fileinf.Length - 1].ToString();
                            System.IO.FileInfo[] FileList = Dir.GetFiles(filename, System.IO.SearchOption.AllDirectories);

                            TimeSpan TimeDifference = (Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss", dateFormatProvider), dateFormatProvider) - Convert.ToDateTime(FileList[0].LastWriteTimeUtc.ToString("dd-MMM-yyyy HH:mm:ss"), dateFormatProvider));

                            //if (Convert.ToDateTime(FileList[0].LastWriteTimeUtc.ToString("dd-MMM-yyyy HH:mm:ss"), dateFormatProvider) < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss", dateFormatProvider), dateFormatProvider))
                            if (TimeDifference.TotalHours >= 8)    //(TimeDifference.TotalDays >= 1)
                                System.IO.File.Delete(file);
                        }
                        catch { }
                        finally { }
                    }
                }
            }
            catch { }
            finally { }
        }
    }
}