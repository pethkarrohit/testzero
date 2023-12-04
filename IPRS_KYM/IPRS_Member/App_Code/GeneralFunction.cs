using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Data.SqlClient;
using System.Web.Mail;
using System.Net.Mail;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Net;
/// <summary>
/// Summary description for GeneralFunction
/// </summary>
public class GeneralFunction
{
    //struct FormulaSyntaxArg
    //{
    //    public bool Valid = true;
    //    public double Value = 0;
    //}

    public GeneralFunction()
    {
    }
    public void SetFocus(Page ActivePage, string Target)
    {
        ScriptManager manager = ScriptManager.GetCurrent(ActivePage);
        manager.SetFocus(Target);
    }

    public string CheckScripValidation(Control root, Type type)
    {
        Regex rgx = new Regex("&lt;[^<]*>/");

        var c = GetAll(root, type);
        foreach (var item in c)
        {
            if (type == typeof(TextBox))
            {
                TextBox txtbox = (TextBox)item;

                if (rgx.IsMatch(txtbox.Text))
                {
                    return "Invalid Input";
                }
            }
            else if (type == typeof(HtmlTextArea))
            {
                HtmlTextArea txtbox = (HtmlTextArea)item;
                if (rgx.IsMatch(txtbox.InnerText))
                {
                    return "Invalid Input";
                }
            }

        }
        return "";
    }

    public IEnumerable<Control> GetAll(Control control, Type type)
    {
        var controls = control.Controls.Cast<Control>();

        return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                  .Concat(controls)
                                  .Where(c => c.GetType() == type);
    }


    public string GetFormTitle()
    {
        return HttpContext.Current.Request.Url.AbsolutePath.Split('/')[HttpContext.Current.Request.Url.AbsolutePath.Split('/').Length - 1].Replace("_", " ");
    }
    public void GenerateError(Page ActivePage, string strSection, string strError)
    {
        string ErrorMessage = "<b>" + "Section Name:</b>" + strSection;
        ErrorMessage = ErrorMessage + "\n\n";
        ErrorMessage = ErrorMessage + "<b>" + "Error:</b> " + strError;
        throw new System.ArgumentException(ErrorMessage, "original");
    }

    public string ReplaceASC(string MsgValue)
    {
        //if (MsgValue.Length >= 25)
        //return MsgValue.Substring(0, MsgValue.Length - 25).Replace("'", "").Replace(":", "").Replace("/", "").Replace(";", "").Replace(",", "").Replace(".", "").Replace("\r", "").Replace("\n", "").Replace("\\", "");
        //else
        //return MsgValue.Replace("'", "").Replace(":", "").Replace("/", "").Replace(";", "").Replace(",", "").Replace(".", "").Replace("\r", "").Replace("\n", "").Replace("\\", ""); 

        return MsgValue.Replace("'", "").Replace(":", "").Replace(";", "").Replace("\r", "").Replace("\n", "");
    }
    public void AlertUser(string strMessage, System.Web.UI.Page page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "jsCall", "alert('" + (strMessage) + "');", true);
    }

    public void BootBoxAlert(string strMessage, System.Web.UI.Page page)
    {
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Pop", "bootbox.alert('" + (ReplaceASC(strMessage)) + "');", true);
    }
    public int GetColumnIndexByName(GridViewRow row, string columnName)
    {
        int columnIndex = 0;
        foreach (DataControlFieldCell cell in row.Cells)
        {
            if (cell.ContainingField is BoundField)
                if (((BoundField)cell.ContainingField).DataField.Equals(columnName))
                    break;
            columnIndex++; // keep adding 1 while we don't have the correct name
        }
        return columnIndex;
    }

    public enum WizardNavigationTempContainer
    {
        StartNavigationTemplateContainerID = 1,
        StepNavigationTemplateContainerID = 2,
        FinishNavigationTemplateContainerID = 3
    }
    public Control GetControlFromWizard(Wizard wizard, WizardNavigationTempContainer wzdTemplate, string controlName)
    {
        System.Text.StringBuilder strCtrl = new System.Text.StringBuilder();
        strCtrl.Append(wzdTemplate);
        strCtrl.Append("$");
        strCtrl.Append(controlName);

        return wizard.FindControl(strCtrl.ToString());
    }
    public void NoRecordFound(System.Web.UI.Page page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "jsCall", "alert('No records found...');", true);
    }

    public void AlertUserClient(string strMessage, System.Web.UI.Page page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "jsCall", "alert('" + (strMessage) + "');", true);
    }

    public void ConfirmDelete(string strMessage, System.Web.UI.Page page)
    {
        string strScript = string.Empty;
        strScript = "javascript:window.confirm(Do you want to delete this record);";
        ScriptManager.RegisterClientScriptBlock(page, this.GetType(), "lnkReturn", strScript, true);
    }
    public void NoRecordFoundClient(System.Web.UI.Page page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "jsCall", "alert('No records found...');", true);
    }

    public string NoRecordMsg()
    {
        return "No records found...";
    }
    public string FormatDate(DateTime strdate)
    {
        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public string EncryptRecordId(Int64 RecordId)
    {
        string key = clsCryptography.Encrypt(Convert.ToString(RecordId));

        StringWriter writer = new StringWriter();

        HttpContext.Current.Server.UrlEncode(key, writer);

        return writer.ToString();
    }
    public string EncryptString(string strQty)
    {
        string key = clsCryptography.Encrypt(strQty);

        StringWriter writer = new StringWriter();

        HttpContext.Current.Server.UrlEncode(key, writer);

        return writer.ToString();
    }
    public string ValidateQueryStringValue(string str)
    {
        if (str == "" || str == null)
            return null;
        else
        {
            str = str.Replace("UPDATE", "");
            str = str.Replace("SELECT", "");
            str = str.Replace("DELETE", "");
            str = str.Replace("INSERT", "");
            str = str.Replace("DROP", "");
            str = str.Replace("ALTER", "");
            str = str.Replace("EXECUTE", "");
            str = str.Replace(";", "");
            str = str.Replace("'", "''");
            return str;
        }
    }
    public void UpdateUserLogMaster(int UserId, string IpAddress, string Mode)
    {
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[3];

        SQLP[0] = new SqlParameter("@UserId", SqlDbType.SmallInt);
        SQLP[1] = new SqlParameter("@IpAddress", SqlDbType.VarChar, 50);
        SQLP[2] = new SqlParameter("@Mode", SqlDbType.VarChar, 1);

        SQLP[0].Value = UserId;
        SQLP[1].Value = IpAddress;
        SQLP[2].Value = Mode;

        try
        {
            DAL.ExecuteNonQuery("UserLogMaster_Manage", SQLP);

        }
        catch
        {
            throw;
        }
        finally
        {
            DAL = null;
        }
    }
    public string TranslateNumber(string strValue)
    {
        string functionReturnValue = null;
        if ((strValue != null))
        {
            if (strValue.Length > 0)
            {
                functionReturnValue = Convert.ToString(String.Format("{0:0.00}", strValue));
            }
            else
            {
                functionReturnValue = "0";
            }
        }
        else
        {
            functionReturnValue = "0";
        }
        return functionReturnValue;
    }
    public string TranslateGridCountNumber(string strValue)
    {
        string functionReturnValue = null;
        if ((strValue != null))
        {
            if (strValue.Length > 0)
            {
                functionReturnValue = Convert.ToString(String.Format("{0:0.00}", strValue));
            }
            else
            {
                functionReturnValue = "25";
            }
        }
        else
        {
            functionReturnValue = "25";
        }
        return functionReturnValue;
    }
    public string Mid(string param, int startIndex)
    {
        //start at the specified index and return all characters after it
        //and assign it to a variable
        string result = param.Substring(startIndex);
        //return the result of the operation
        return result;
    }
    public string Mid(string param, int startIndex, int length)
    {
        //start at the specified index in the string ang get N number of
        //characters depending on the lenght and assign it to a variable
        string result = param.Substring(startIndex, length);
        //return the result of the operation
        return result;
    }
    public string Left(string param, int length)
    {
        //we start at 0 since we want to get the characters starting from the
        //left and with the specified lenght and assign it to a variable
        string result = param.Substring(0, length);
        //return the result of the operation
        return result;
    }
    public string Right(string param, int length)
    {
        //start at the index based on the lenght of the sting minus
        //the specified lenght and assign it a variable
        string result = param.Substring(param.Length - length, length);
        //return the result of the operation
        return result;
    }

    public String RecordExecute(string SQLQuery)
    {
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        String cReturnValue = "false";
        Int64 lFlag = 0;
        try
        {
            lFlag = DAL.ExecuteSqlNonQuery(SQLQuery);
            if (lFlag == 1)
            {
                cReturnValue = "true";
            }
            else
            {
                cReturnValue = "false";
            }
        }
        catch (Exception ee)
        {
            Console.Write(ee);
            cReturnValue = "false";
        }
        finally
        {
            DAL = null;
        }
        return cReturnValue;
    }

    //public void SendMail(string MailFrom, string MailTo, string HTMLContent, string MemberDetails, string Type)
    //{
    //    string MailCC = null;
    //    string[] strMailCC = null;
    //    DSIT_DataLayer DAL = new DSIT_DataLayer();
    //    DataSet DS = new DataSet();
    //    SqlParameter[] SQLP = new SqlParameter[1];
    //    SQLP[0] = new SqlParameter("@SMTPTypeCode", SqlDbType.VarChar, 3);
    //    SQLP[0].Value = Type;
    //    try
    //    {
    //        DS = DAL.GetDataSet("SMTPMasterView", SQLP);
    //        if (DS.Tables[0].Rows.Count > 0)
    //        {
    //            if (MailTo != "")
    //            {
    //                HTMLContent = HTMLContent + "<BR>" + DS.Tables[0].Rows[0]["EmailContent"].ToString();
    //                if (Convert.ToInt16(DS.Tables[0].Rows[0]["AttachMember"]) == 0)
    //                {
    //                    HTMLContent = HTMLContent + "<BR><BR>" + MemberDetails;
    //                }
    //                HTMLContent = HTMLContent + "<BR><BR>" + DS.Tables[0].Rows[0]["EmailSignature"].ToString();
    //            }

    //            if (MailFrom == "")
    //                MailFrom = DS.Tables[0].Rows[0]["SMTPEmailAddress"].ToString();
    //            if (MailTo == "")
    //            {
    //                MailTo = DS.Tables[0].Rows[0]["SMTPEmailAddress"].ToString();
    //                MailCC = DS.Tables[0].Rows[0]["SMTPCCEmailAddress"].ToString();
    //            }

    //            string[] strMailTo = MailTo.Split(',');

    //            if (MailCC != null)
    //                strMailCC = MailCC.Split(',');

    //            System.Net.Mail.MailMessage CustomerMessage = new System.Net.Mail.MailMessage();
    //            CustomerMessage.From = new MailAddress(MailFrom);

    //            for (int i = 0; i < strMailTo.Length; i++)
    //            {
    //                CustomerMessage.To.Add(new MailAddress(strMailTo[i]));
    //            }
    //            if (strMailCC != null)
    //            {
    //                for (int i = 0; i < strMailCC.Length; i++)
    //                {
    //                    CustomerMessage.CC.Add(new MailAddress(strMailCC[i]));
    //                }
    //            }

    //            CustomerMessage.Subject = DS.Tables[0].Rows[0]["EmailSubject"].ToString();
    //            CustomerMessage.IsBodyHtml = true;
    //            CustomerMessage.Body = HTMLContent;
    //            SmtpClient Customer = new SmtpClient();
    //            Customer.Host = DS.Tables[0].Rows[0]["SMTPAddress"].ToString();
    //            Customer.Credentials = new System.Net.NetworkCredential(DS.Tables[0].Rows[0]["SMTPUserName"].ToString(), clsCryptography.Decrypt(DS.Tables[0].Rows[0]["SMTPPassword"].ToString()));
    //            Customer.Send(CustomerMessage);
    //        }
    //    }
    //    catch (Exception ee)
    //    {
    //        Console.Write(ee);
    //        throw;
    //    }
    //}

    //Modified by Renu
    public void SendMail(string MailFrom, string MailTo, string HTML, Hashtable hashtable, string Type, string bookType)
    {
        string MailCC = null; string HTMLContent = string.Empty; string ReplyTo = string.Empty;
        string[] strMailCC = null;
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        DataSet DS = new DataSet();
        SqlParameter[] SQLP = new SqlParameter[2];
        SQLP[0] = new SqlParameter("@BookType", SqlDbType.NVarChar, 10);
        SQLP[0].Value = bookType;
        SQLP[1] = new SqlParameter("@EmailType", SqlDbType.NVarChar, 10);
        SQLP[1].Value = Type;

        try
        {
            DS = DAL.GetDataSet("App_EmailSMSConfig_Display_IPM", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
            {
                string StartLine = string.Empty; string Name = string.Empty;
                string MemberDetails = string.Empty;
                if (hashtable != null)
                {
                    if (hashtable["name"] != null)
                        Name = (string)hashtable["name"];
                    if (hashtable["ReplyTo"] != null)
                        ReplyTo = (string)hashtable["ReplyTo"];

                }
                if (StartLine == string.Empty)
                {
                    StartLine = DS.Tables[0].Rows[0]["EmailStartLine"].ToString().Replace("{NAME}", Name);
                }

                if (MailTo != "")
                {
                    HTMLContent = HTMLContent + "<BR>" + StartLine;

                    HTMLContent = HTMLContent + "<BR>" + DS.Tables[0].Rows[0]["EmailContent"].ToString().Replace("{DETAILS}", HTML); ;

                    HTMLContent = HTMLContent + "<BR><BR>" + DS.Tables[0].Rows[0]["EmailSignature"].ToString();
                }
                else
                {
                    HTMLContent = HTML;
                }

                if (MailFrom == "")
                    MailFrom = DS.Tables[0].Rows[0]["EmailFromAddress"].ToString();
                if (MailTo == "")
                {
                    if (DS.Tables[0].Rows[0]["EmailToAddress"].ToString() == string.Empty)
                        MailTo = MailFrom;
                    else
                        MailTo = DS.Tables[0].Rows[0]["EmailToAddress"].ToString();
                    MailCC = DS.Tables[0].Rows[0]["EmailCCAddress"].ToString();
                }

                string[] strMailTo = MailTo.Split(',');

                if (MailCC != null)
                    strMailCC = MailCC.Split(',');

                System.Net.Mail.MailMessage CustomerMessage = new System.Net.Mail.MailMessage();
                CustomerMessage.From = new MailAddress(MailFrom);

                for (int i = 0; i < strMailTo.Length; i++)
                {
                    CustomerMessage.To.Add(new MailAddress(strMailTo[i]));
                }
                if (strMailCC != null)
                {
                    for (int i = 0; i < strMailCC.Length; i++)
                    {
                        if (strMailCC[i] != string.Empty)
                            CustomerMessage.CC.Add(new MailAddress(strMailCC[i]));
                    }
                }
                if (ReplyTo == "")
                {
                    ReplyTo = MailFrom;
                }
                //CustomerMessage.ReplyToList.Add(new MailAddress(ReplyTo));
                CustomerMessage.Subject = DS.Tables[0].Rows[0]["EmailSubject"].ToString();
                CustomerMessage.IsBodyHtml = true;
                CustomerMessage.Body = HTMLContent;

                SmtpClient Customer = new SmtpClient();


                Customer.Port = Convert.ToInt32(DS.Tables[0].Rows[0]["EmailSMTPPort"].ToString());
                Customer.Host = DS.Tables[0].Rows[0]["EmailSMTPAddress"].ToString();
                Customer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                string strEnableSSL = DS.Tables[0].Rows[0]["EmailSMTPSSL"].ToString();
                string strAuthentication = DS.Tables[0].Rows[0]["EmailSMTPAuthentication"].ToString();
                System.Net.ServicePointManager.SecurityProtocol =  SecurityProtocolType.Tls12;

                Customer.UseDefaultCredentials = false;
                if (strEnableSSL == "0")
                {
                    Customer.EnableSsl = true;
                    
                }
                else
                    Customer.EnableSsl = false;

                if (strAuthentication == "0")
                    Customer.Credentials = new System.Net.NetworkCredential(DS.Tables[0].Rows[0]["EmailSMTPUserName"].ToString(), clsCryptography.Decrypt(System.Web.HttpUtility.UrlDecode(DS.Tables[0].Rows[0]["EmailSMTPPassword"].ToString())));

                Customer.Timeout = 10000;

                //Customer.Timeout = 2147483647;
                //Customer.Credentials = new System.Net.NetworkCredential(DS.Tables[0].Rows[0]["EmailSMTPUserName"].ToString(), clsCryptography.Decrypt(DS.Tables[0].Rows[0]["EmailSMTPPassword"].ToString()));
                //Customer.UseDefaultCredentials = true;
                Customer.Send(CustomerMessage);
            }
        }
        catch (Exception ee)
        {
            Console.Write(ee);
            throw;
        }
    }
    public string FormatNullableDate(string strdate)
    {
        if (strdate == "" || strdate == null)
            return null;
        else
            return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public string FormatNullableDateTime(string strdate)
    {
        if (strdate == "" || strdate == null)
            return null;
        else
            return Convert.ToDateTime(strdate).ToString("dd-MM-yyyy HH:mm:ss");
    }
    public string FormatNullableDateTimeWithOutSec(string strdate)
    {
        if (strdate == "" || strdate == null)
            return null;
        else
            return Convert.ToDateTime(strdate).ToString("dd-MM-yyyy HH:mm");
    }

    public double TranslateDateTimeToDouble(string strdate)
    {
        if (strdate == "" || strdate == null)
            return 0;
        else
            return Convert.ToDateTime(strdate).ToOADate();
    }

    public void SubmitButtonScript(Button btn, System.Web.UI.Page page)
    {
        System.Text.StringBuilder sbValid = new System.Text.StringBuilder();
        sbValid.Append("if (typeof(validate) == 'function') { ");
        sbValid.Append("if (validate() == false) { return false; }} ");
        sbValid.Append("this.value = '...Wait...';");
        sbValid.Append("this.disabled = true;");
        sbValid.Append("document.all." + btn + ".disabled = true;");
        sbValid.Append("document.body.style.cursor = 'wait';");

        sbValid.Append(page.GetPostBackEventReference(btn));
        sbValid.Append(";");
        //return sbValid.ToString();
        btn.Attributes.Add("onclick", sbValid.ToString());
    }

    //=================
    public string FormatGridDate(DateTime strdate)
    {
        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public decimal ConvertToNumber(string strValue)
    {
        decimal functionReturnValue = 0;
        if ((strValue != null))
        {
            if (strValue.Length > 0)
            {
                functionReturnValue = Convert.ToDecimal(String.Format("{0:0.00}", strValue));
            }
            else
            {
                functionReturnValue = 0;
            }
        }
        else
        {
            functionReturnValue = 0;
        }
        return functionReturnValue;
    }
    public string FormatNumber(decimal strValue)
    {
        return String.Format("{0:0.00}", strValue);
    }
    public string FormatNumber1(decimal strValue)
    {
        return String.Format("{0:0.000}", strValue);
    }
    public bool IsAlphaNumeric(string strToCheck)
    {
        Regex pattern = new Regex("[^a-zA-Z0-9]");

        return !pattern.IsMatch(strToCheck);
    }

    public bool IsValidAlphaNumeric(string inputStr)
    {
        //make sure the user provided us with data to check
        //if not then return false
        bool blnAlpha = false;
        bool blnNumeric = false;
        if (string.IsNullOrEmpty(inputStr))
            return false;

        //now we need to loop through the string, examining each character
        for (int i = 0; i < inputStr.Length; i++)
        {
            //if this character isn't a letter and it isn't a number then return false
            //because it means this isn't a valid alpha numeric string

            if (char.IsLetter(inputStr[i]) == true)
            {
                blnAlpha = true;
            }
            if (char.IsNumber(inputStr[i]) == true)
            {
                blnNumeric = true;
            }
            //if (!(char.IsLetter(inputStr[i])) && (!(char.IsNumber(inputStr[i]))))
            //    return false;
        }
        if (blnNumeric == true && blnAlpha == true)
        {
            return true;
        }
        else
        {
            return false;
        }
        //we made it this far so return true

    }

    public bool IsNumeric(string strToCheck)
    {
        return Regex.IsMatch(strToCheck, "^\\d+(\\.\\d+)?$");
    }
    public decimal Date_Diff(string startDateString, string endDateString, string DiffType)
    {
        DateTime startDate = Convert.ToDateTime(startDateString);
        DateTime endDate = Convert.ToDateTime(endDateString);
        TimeSpan dateDifference = endDate.Subtract(startDate);

        if (DiffType == "h")
            return (decimal)dateDifference.Hours;
        else if (DiffType == "m")
            return (decimal)dateDifference.Minutes;
        else
            return (int)dateDifference.Days;
    }

    public string Rupee(string FinalAmountAfterTax)
    {
        string functionReturnValue = null;
        functionReturnValue = Rupee(FinalAmountAfterTax, "");
        return functionReturnValue;
    }
    public string Rupee(string FinalAmountAfterTax, string CurrencyName)
    {
        if (CurrencyName == "")
            CurrencyName = "Rupees";

        string functionReturnValue = null;
        string ss = null;
        string temp = null;
        string[] num = new string[21];
        string[] units = new string[11];
        long Amount = 0;
        double cr = 0;
        double lakhs = 0;
        double thou = 0;
        double hun = 0;
        double unit = 0;
        ss = " ";
        num[1] = " One ";
        num[2] = " Two ";
        num[3] = " Three ";
        num[4] = " Four ";
        num[5] = " Five ";
        num[6] = " Six ";
        num[7] = " Seven ";
        num[8] = " Eight ";
        num[9] = " Nine ";
        num[10] = " Ten ";
        num[11] = " Eleven ";
        num[12] = " Twelve ";
        num[13] = " Thirteen ";
        num[14] = " Fourteen ";
        num[15] = " Fifteen ";
        num[16] = " Sixteen ";
        num[17] = " Seventeen ";
        num[18] = " Eighteen ";
        num[19] = " Nineteen ";
        num[20] = " Twenty ";
        units[1] = " Ten ";
        units[2] = " Twenty ";
        units[3] = " Thirty ";
        units[4] = " Forty ";
        units[5] = " Fifty ";
        units[6] = " Sixty ";
        units[7] = " Seventy ";
        units[8] = " Eighty ";
        units[9] = " Ninety ";
        units[10] = " Hundred ";



        Amount = Convert.ToInt32(ConvertToNumber(FinalAmountAfterTax));

        cr = Convert.ToDouble(Amount / 10000000);

        lakhs = (int)((Amount - (cr * 10000000)) / 100000);

        thou = (int)((Amount - (cr * 10000000 + lakhs * 100000)) / 1000);

        hun = (int)((Amount - (cr * 10000000 + lakhs * 100000 + thou * 1000)) / 100);

        unit = (int)((Amount - (cr * 10000000 + lakhs * 100000 + thou * 1000 + hun * 100)));


        temp = Convert.ToString(cr);
        if ((Convert.ToInt32(temp) <= 20) && (Convert.ToInt32(temp) > 0))
        {
            ss = ss + num[Convert.ToInt32(temp)] + " Crores ";
        }
        if (Convert.ToInt32(temp) > 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + units[Convert.ToInt32(Mid(temp, 0, 1))] + num[Convert.ToInt32(Mid(temp, 1, 1))] + " Crores ";
        }

        temp = Convert.ToString(lakhs);
        if (Convert.ToInt32(temp) <= 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + num[Convert.ToInt32(temp)] + " Lacs ";
        }
        if (Convert.ToInt32(temp) > 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + units[Convert.ToInt32(Mid(temp, 0, 1))] + num[Convert.ToInt32(Mid(temp, 1, 1))] + " Lacs ";
        }

        temp = Convert.ToString(thou);
        if (Convert.ToInt32(temp) <= 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + num[Convert.ToInt32(temp)] + " Thousand ";
        }
        if (Convert.ToInt32(temp) > 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + units[Convert.ToInt32(Mid(temp, 0, 1))] + num[Convert.ToInt32(Mid(temp, 1, 1))] + " Thousand ";
        }

        temp = Convert.ToString(hun);
        if (Convert.ToInt32(temp) <= 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + num[Convert.ToInt32(temp)] + " Hundred ";
        }
        if (Convert.ToInt32(temp) > 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + units[Convert.ToInt32(Mid(temp, 0, 1))] + num[Convert.ToInt32(Mid(temp, 1, 1))] + " Hundred ";
        }

        temp = Convert.ToString(unit);
        if (Convert.ToInt32(temp) <= 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + num[Convert.ToInt32(temp)];
        }
        if (Convert.ToInt32(temp) > 20 && Convert.ToInt32(temp) > 0)
        {
            ss = ss + units[Convert.ToInt32(Mid(temp, 0, 1))] + num[Convert.ToInt32(Mid(temp, 1, 1))];
        }
        functionReturnValue = CurrencyName + " " + ss.Trim() + " only";

        return functionReturnValue;
    }

    public string GetBookTable(string cBookTypeCode)
    {
        string cBookTable = "";
        switch (cBookTypeCode.ToUpper().Trim())
        {
            case "IS":
                cBookTable = "ServiceMaster@Install/AMC Service";
                break;
            case "OS":
                cBookTable = "StockLedger@Opening Stock";
                break;
            case "II":
                cBookTable = "StockTransfer@Goods Issued";
                break;
            case "IR":
                cBookTable = "StockTransfer@Goods Received";
                break;
            case "IM":
                cBookTable = "StockTransfer@Goods Inward (Party)";
                break;
            case "BB":
                cBookTable = "Ledger";
                break;
            case "CC":
                cBookTable = "Ledger";
                break;
            case "CP":
                cBookTable = "Ledger";
                break;
            case "DD":
                cBookTable = "Ledger";
                break;
            case "DR":
                cBookTable = "Ledger";
                break;
            case "EE":
                cBookTable = "Ledger";
                break;
            case "JJ":
                cBookTable = "Ledger";
                break;
            case "QQ":
                cBookTable = "Ledger";
                break;
            case "NN":
                cBookTable = "Ledger";
                break;
            case "NR":
                cBookTable = "Ledger";
                break;
            case "PE":
                cBookTable = "PurchaseEnquiry@Purchase Enquiry";
                break;
            case "PI":
                cBookTable = "Indent@Purchase Indent";
                break;
            case "PQ":
                cBookTable = "PurchaseQuotation@Purchase Quotation";
                break;
            case "PO":
                cBookTable = "PurchaseOrder@Purchase Order";
                break;
            case "PG":
                cBookTable = "PurchaseGRN@Goods Received Note";
                break;
            case "PP":
                cBookTable = "PurchaseBill@Purchase";
                break;
            case "PR":
                cBookTable = "PurchaseGRNReturn@GRN Return";
                break;

            case "SE":
                cBookTable = "SalesEnquiry@Sales Enquiry";
                break;
            case "SQ":
                cBookTable = "SalesQuotation@Sales Quotation";
                break;
            case "SO":
                cBookTable = "SalesOrder@Sales Order";
                break;
            case "SG":
                cBookTable = "SalesChallan@Sales Challan";
                break;
            case "IG":
                cBookTable = "SalesChallan@Sales Challan";
                break;
            case "SS":
                cBookTable = "SalesBill@Sales";
                break;
            case "SR":
                cBookTable = "SalesGINReturn@GIN Return";
                break;

            case "TA":
                cBookTable = "TransferOrder@Inter-Branch Order";
                break;
            case "TR":
                cBookTable = "TransferGoods@Inter-Branch (GRN)";
                break;
            case "TI":
                cBookTable = "TransferGoods@Inter-Branch (GIN)";
                break;
        }
        return cBookTable;
    }

    public string GetBookDetails(string cBookTypeCode, string cBookCode, int nBookId)
    {
        string cBookTable = "";
        string SQLQuery = "Execute BookMainBrow  '" + GetFABook(cBookTypeCode) + "','" + cBookCode + "'," + nBookId;

        DSIT_DataLayer BAL = new DSIT_DataLayer();
        DataSet DS = new DataSet();
        try
        {
            DS = BAL.GetDataSet(SQLQuery);
            if (DS.Tables[0].Rows.Count >= 1)
            {
                cBookTable = DS.Tables[0].Rows[0]["BookTable"].ToString() + "@";
                cBookTable += DS.Tables[0].Rows[0]["BookName"].ToString();
            }
        }
        catch (Exception ee)
        {
            Console.Write(ee);
        }
        finally
        {
            BAL = null;
        }

        return cBookTable;
    }

    public String RecordFind(string SQLQuery, string ReturnField)
    {
        String cReturnValue = "";
        DSIT_DataLayer BAL = new DSIT_DataLayer();
        DataSet DS = new DataSet();
        try
        {
            DS = BAL.GetDataSetSql(SQLQuery);
            if (DS.Tables[0].Rows.Count >= 1)
            {
                if (ReturnField.ToString().Trim().Length >= 1)
                {
                    if (DS.Tables[0].Rows[0][ReturnField] != null)
                    {
                        cReturnValue = DS.Tables[0].Rows[0][ReturnField].ToString();
                    }
                    else
                    {
                        cReturnValue = "";
                    }
                }
                else
                {
                    cReturnValue = "true";
                }
            }
            else
            {
                if (ReturnField.ToString().Trim().Length == 0)
                {
                    cReturnValue = "false";
                }
                else
                {
                    cReturnValue = "";
                }
            }
        }
        catch (Exception ee)
        {
            Console.Write(ee);
            //lblmsg.Text = ee.Message.ToString();
            //lblmsg.Visible = true;
        }
        finally
        {
            BAL = null;
        }
        return cReturnValue;
    }

    public string FormatDate(string strdate, string cFormat, System.Web.UI.Page page)
    {
        if (strdate == "")
            return "";
        try
        {
            DateTime parsedDate = DateTime.Parse(strdate);
            return parsedDate.ToString(cFormat);
        }
        catch (Exception ex)
        {
            if (page != null)
                AlertUser(ex.Message, page);
            return "";
        }
    }
    public string GetFABook(string cBookType)
    {
        string cBkTyp = cBookType;
        switch (cBookType.ToString().ToUpper().Trim())
        {
            case "BR":
                cBkTyp = "BB";
                break;
            case "BP":
                cBkTyp = "BB";
                break;
            case "CR":
                cBkTyp = "CC";
                break;
            case "CP":
                cBkTyp = "CC";
                break;
        }
        return cBkTyp;
    }

    public string GetProductCircularValue()
    {
        string nRate = "";
        //nRate = row["Rate"].ToString();
        //if ((nRate == "") || (nRate == "0") || (nRate == "0.00"))
        //{
        //  nRate = GenFunction.RecordFind("Select Rate From PartyProductRate Where BranchId = " + Session["BranchId"].ToString() + " And LedgerId = " + HdnClientId.Value + " And ProductClassificationId = " + row["ProductClassificationId"].ToString(), "Rate");
        //  if ((nRate == "") || (nRate == "0") || (nRate == "0.00"))
        //      nRate = GenFunction.RecordFind("Select Rate From ProductBranchRate Where BranchId = " + Session["BranchId"].ToString() + " And ProductClassificationId = " + row["ProductClassificationId"].ToString(), "Rate");
        //  if ((nRate == "") || (nRate == "0") || (nRate == "0.00"))
        //      nRate = GenFunction.RecordFind("Select SalesRate From ProductMaster Where ProductId = " + row["ProductId"].ToString(), "SalesRate");
        //  if ((nRate == "") || (nRate == "0") || (nRate == "0.00"))
        //     nRate = "0";
        //}
        return nRate;
    }

    public string GetBranchName(int BranchId)
    {
        string cBranchTable = "";
        string SQLQuery = "SELECT BranchName From BranchMaster WHERE BranchId =  " + BranchId;

        DSIT_DataLayer BAL = new DSIT_DataLayer();
        DataSet DS = new DataSet();
        try
        {
            DS = BAL.GetDataSet(SQLQuery);
            if (DS.Tables[0].Rows.Count >= 1)
            {
                cBranchTable = DS.Tables[0].Rows[0]["BranchName"].ToString();
            }
        }
        catch (Exception ee)
        {
            Console.Write(ee);
        }
        finally
        {
            BAL = null;
        }
        return cBranchTable;
    }



    public string[] GetDistinctArray(string[] myStrArray)
    {

        for (int i = 0; i < myStrArray.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (myStrArray[i] == myStrArray[j])
                {
                    myStrArray[i] = null;
                }
            }
        }
        string[] strArray = new string[1]; ;
        strArray[0] = "";
        int s = 0;
        foreach (string val in myStrArray)
        {
            if (val != null)
            {
                if (s > 0)
                    Array.Resize(ref (strArray), strArray.Length + 1);

                strArray[strArray.Length - 1] = val;
                s = s + 1;
                //strArray
            }

        }
        return strArray;

        //ArrayList list = new ArrayList();
        //for (int i = 0; i < array.Length; i++)
        //{
        //    if (list.Contains(array[i])) continue;
        //    list.Add(array[i]);
        //}
        //return (int[])list.ToArray(typeof(int));
    }

    public Int32 GetCheckboxValue(CheckBox CB)
    {
        int i = 0;
        if (CB.Checked == true)
            i = 1;
        else
            i = 0;
        return i;
    }



    public string GetListBoxCheckValue(ListBox lst, string strMode)
    {

        if (strMode == string.Empty)
            strMode = "V";

        string strTemp = string.Empty;

        foreach (ListItem li in lst.Items)
        {
            if (li.Selected == true)
            {
                if (strTemp != string.Empty)
                    strTemp = strTemp + ",";

                if (strMode == "V")
                    strTemp += li.Value;
                else
                    strTemp += li.Text;
            }
        }
        return strTemp;
    }

    public Boolean GetCheckboxChecked(int i)
    {
        if (i == 1)
            return true;
        else
            return false;
    }

    public Control FindAllControl(Control root, string id)
    {
        if (root.ID == id)
        {
            return root;
        }

        foreach (Control c in root.Controls)
        {
            Control t = FindAllControl(c, id);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }

    public void ClearControl(Control control)
    {
        var textbox = control as TextBox;
        if (textbox != null)
            textbox.Text = string.Empty;

        var comboBox = control as DropDownList;
        if (comboBox != null)
            comboBox.SelectedIndex = -1;

        var listbox = control as ListBox;
        if (listbox != null)
            listbox.SelectedIndex = -1;


        //var dropDownList = control as DropDownList;
        //if (dropDownList != null)
        //    dropDownList.SelectedIndex = 0;

        var HiddenField = control as HiddenField;
        if (HiddenField != null)
        {
            if (HiddenField.ID != "hdnSearchDelete" && HiddenField.ID != "hdnSearchQuery"
                && HiddenField.ID != "hdnSearchColumns" && HiddenField.ID != "hdnRecordIds"
                && HiddenField.ID != "hdnDeleteRecordCol" && HiddenField.ID != "hdnTotalRecords"
                && HiddenField.ID != "hdnPageIndex")
                HiddenField.Value = string.Empty;
        }

        foreach (Control childControl in control.Controls)
        {
            ClearControl(childControl);
        }
    }
    public void ClearAllControl(Control cRoot, Control ActivePage)
    {

        int count = cRoot.Controls.Count;
        for (int i = 0; i < count; i++)
        {
            if ((cRoot.Controls[i].ID != null) || (cRoot.Controls[i].ClientID != null))
            //if (cRoot.Controls[i].ID != null)
            {
                object ctrl = FindAllControl(cRoot, cRoot.Controls[i].ID);
                if (ctrl == null)
                    ctrl = FindAllControl(cRoot, cRoot.Controls[i].ClientID);
                if (ctrl != null)
                {
                    if (ctrl.GetType() == typeof(HiddenField))
                        ((HiddenField)ctrl).Value = "0";
                    else if (ctrl.GetType() == typeof(TextBox))
                    {
                        ((TextBox)ctrl).Text = "";
                        if (((TextBox)ctrl).CssClass == "numInput")
                            ((TextBox)ctrl).Text = "0.00";
                        if (((TextBox)ctrl).CssClass == "intInput")
                            ((TextBox)ctrl).Text = "0";
                        if (((TextBox)ctrl).CssClass == "dateInput")
                            ((TextBox)ctrl).Text = FormatDate(DateTime.Now);
                    }
                    else if (ctrl.GetType() == typeof(DropDownList))
                        ((DropDownList)ctrl).SelectedIndex = -1;
                    else if (ctrl.GetType() == typeof(ListBox))
                        for (int j = 0; j < ((ListBox)ctrl).Items.Count; j++)
                        {
                            ((ListBox)ctrl).Items[j].Selected = false;
                        }
                    //((ListBox)ctrl).SelectedIndex = -1;
                    else if (ctrl.GetType() == typeof(CheckBoxList))
                        for (int j = 0; j < ((CheckBoxList)ctrl).Items.Count; j++)
                        {
                            ((CheckBoxList)ctrl).Items[j].Selected = false;
                        }
                    else if (ctrl.GetType() == typeof(CheckBox))
                        ((CheckBox)ctrl).Checked = false;
                    else if (ctrl.GetType() == typeof(RadioButton))
                        ((RadioButton)ctrl).Checked = false;
                    else if (ctrl.GetType() == typeof(Panel))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlGenericControl))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlContainerControl))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlControl))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlTable))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlTableRow))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlTableCell))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(HtmlForm))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(TemplateControl))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(TemplateColumn))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(LiteralControl))
                        ClearAllControl((Control)ctrl, cRoot);
                    else if (ctrl.GetType() == typeof(Control))
                        ClearAllControl((Control)ctrl, cRoot);
                    //else
                    //    ClearAllControl((Control)ctrl, cRoot);
                }
            }
        }
    }

    public object GetObjectGridControls(ControlCollection ControlObj, string ControlName)
    {
        object cObject = null;
        for (int i = 0; i < ControlObj.Count; i++)
        {
            if (ControlObj[i].ID != null)
            {
                if (ControlObj[i].ID.ToString().ToUpper() == ControlName.ToString().ToUpper())
                {
                    cObject = (object)ControlObj[i];
                    break;
                }
            }
        }
        return cObject;
    }


    public int GetIndexOfGridControls(ControlCollection ControlObj, string ControlName)
    {
        int nIndex = -1;
        for (int i = 0; i < ControlObj.Count; i++)
        {
            if (ControlObj[i].ID != null)
            {
                if (ControlObj[i].ID.ToString().ToUpper() == ControlName.ToString().ToUpper())
                {
                    nIndex = i;
                    break;
                }
            }
        }
        return nIndex;
    }

    public string FormatDate(DateTime strdate, string cFormatString)
    {
        return Convert.ToDateTime(strdate).ToString(cFormatString);
    }
    public string getFirstDate()
    {
        string strdate = "01-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
        //if (DateTime.Now.Month < 4)
        //    strdate = "01" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.AddYears(-1).ToString();

        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public string getLastDate()
    {
        string strdate = "01-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();

        return Convert.ToDateTime(strdate).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy");
    }

    public string getFAFirstDate(int nMth)
    {

        string strdate = "01-" + GetMonthName(nMth, true) + "-" + DateTime.Now.Year.ToString();
        if (DateTime.Now.Month < 4)
            strdate = "01-" + GetMonthName(nMth, true) + "-" + DateTime.Now.AddYears(-1).Year.ToString();

        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public string getFALastDate(int nMth)
    {
        string strdate = "31-" + GetMonthName(nMth, true) + "-" + DateTime.Now.Year.ToString();
        if (DateTime.Now.Month > 3)
            strdate = "31-" + GetMonthName(nMth, true) + "-" + DateTime.Now.AddYears(1).Year.ToString();

        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }

    public string getFAFirstDate(int nMth, string cDate)
    {
        string strdate = "01-" + GetMonthName(nMth, true) + "-" + Convert.ToDateTime(cDate).Year.ToString();
        if (Convert.ToDateTime(cDate).Month < 4)
            strdate = "01-" + GetMonthName(nMth, true) + "-" + Convert.ToDateTime(cDate).AddYears(-1).Year.ToString();

        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }
    public string getFAYear()
    {
        string strdate = DateTime.Now.Year.ToString();
        if (DateTime.Now.Month < 4)
            strdate = DateTime.Now.AddYears(-1).Year.ToString();

        return strdate;
    }
    public string getArchieveYear()
    {
        string strdate = DateTime.Now.AddYears(-2).Year.ToString();
        if (DateTime.Now.Month < 4)
            strdate = DateTime.Now.AddYears(-3).Year.ToString();

        return strdate;
    }
    public string getFALastDate(int nMth, string cDate)
    {
        string strdate = "31-" + GetMonthName(nMth, true) + "-" + Convert.ToDateTime(cDate).Year.ToString();
        if (Convert.ToDateTime(cDate).Month > 3)
            strdate = "31-" + GetMonthName(nMth, true) + "-" + Convert.ToDateTime(cDate).AddYears(1).Year.ToString();

        return Convert.ToDateTime(strdate).ToString("dd-MMM-yyyy");
    }



    public string getDocumentPrefix(string BookId, string FAMONTH, string DocumentDate)
    {

        string FAyearfrom = getFAFirstDate(Convert.ToInt32(FAMONTH), DocumentDate);
        string FAyearto = Convert.ToDateTime(FAyearfrom.ToString()).AddYears(1).ToString("dd-MMM-yyyy");
        FAyearto = Convert.ToDateTime(FAyearto.ToString()).AddDays(-1).ToString("dd-MMM-yyyy");

        string DocumentPrefix = "";
        DataSet DS = new DataSet();
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[1];
        SQLP[0] = new SqlParameter("@BookId", SqlDbType.BigInt);
        SQLP[0].Value = BookId.ToString();
        try
        {
            DS = DAL.GetDataSet("BookMaster_InfoData", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
            {
                //C-Company, U-BusinessUnit, B-Branch
                if (DS.Tables[0].Rows[0]["PrefixMain"].ToString().ToUpper() == "C")
                    DocumentPrefix = DS.Tables[0].Rows[0]["CompanyCode"].ToString() + "/";
                else if (DS.Tables[0].Rows[0]["PrefixMain"].ToString().ToUpper() == "U")
                    DocumentPrefix = DS.Tables[0].Rows[0]["BusinessUnitCode"].ToString() + "/";
                else
                    DocumentPrefix = DS.Tables[0].Rows[0]["BranchCode"].ToString() + "/";

                string PrefixSub = "";
                //[YYYYMMDD] D-Daily, [YYYYMM00] M-Monthly, [YYYYYYYY] Y-FinancialYear else [PrefixFixed] F-Fixed . e.g Daily - [ 2011 01 15]
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "D")
                {
                    PrefixSub = Convert.ToDateTime(DocumentDate).Year.ToString("0000") + Convert.ToDateTime(DocumentDate).Month.ToString("00") + Convert.ToDateTime(DocumentDate).Day.ToString("00");
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "M")
                {
                    PrefixSub = Convert.ToDateTime(DocumentDate).Year.ToString("0000") + Convert.ToDateTime(DocumentDate).Month.ToString("00") + "00";
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "Y")
                {
                    PrefixSub = Convert.ToDateTime(FAyearfrom).Year.ToString("0000") + Convert.ToDateTime(FAyearto).Year.ToString("0000");
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "F")
                {
                    PrefixSub = DS.Tables[0].Rows[0]["PrefixFixed"].ToString().ToUpper();
                }
                if (PrefixSub.Trim() == string.Empty)
                    PrefixSub = "00000000";
                DocumentPrefix += PrefixSub + "/";

                //BookPrefix
                if (DS.Tables[0].Rows[0]["BookPrefix"].ToString().Trim() != string.Empty)
                    DocumentPrefix += DS.Tables[0].Rows[0]["BookPrefix"].ToString().Trim() + "/";
                else
                    DocumentPrefix += DS.Tables[0].Rows[0]["BookCode"].ToString().Trim() + "/";

                //string PrefixSerial = "A";
                ////PrefixSerial
                //if (DS.Tables[0].Rows[0]["PrefixSerial"].ToString().Trim() != string.Empty)
                //    PrefixSerial = DS.Tables[0].Rows[0]["PrefixSerial"].ToString().Trim();
                //if ((PrefixSerial != "A") || (PrefixSerial != "M"))
                //    PrefixSerial = "A";
                //DocumentPrefix += PrefixSerial;
            }
        }
        catch
        {
        }
        finally
        {
            DS = null;
            DAL = null;
            SQLP = null;
        }
        return DocumentPrefix;
    }

    public string getDocumentPrefix(string BookId, string FAyearfrom, string FAyearto, string DocumentDate)
    {
        string DocumentPrefix = "";
        DataSet DS = new DataSet();
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[1];
        SQLP[0] = new SqlParameter("@BookId", SqlDbType.BigInt);
        SQLP[0].Value = BookId.ToString();
        try
        {
            DS = DAL.GetDataSet("BookMaster_InfoData", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
            {
                //C-Company, U-BusinessUnit, B-Branch
                if (DS.Tables[0].Rows[0]["PrefixMain"].ToString().ToUpper() == "C")
                    DocumentPrefix = DS.Tables[0].Rows[0]["CompanyCode"].ToString() + "/";
                else if (DS.Tables[0].Rows[0]["PrefixMain"].ToString().ToUpper() == "U")
                    DocumentPrefix = DS.Tables[0].Rows[0]["BusinessUnitCode"].ToString() + "/";
                else
                    DocumentPrefix = DS.Tables[0].Rows[0]["BranchCode"].ToString() + "/";

                string PrefixSub = "";
                //[YYYYMMDD] D-Daily, [YYYYMM00] M-Monthly, [YYYYYYYY] Y-FinancialYear else [PrefixFixed] F-Fixed . e.g Daily - [ 2011 01 15]
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "D")
                {
                    PrefixSub = Convert.ToDateTime(DocumentDate).Year.ToString("0000") + Convert.ToDateTime(DocumentDate).Month.ToString("00") + Convert.ToDateTime(DocumentDate).Day.ToString("00");
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "M")
                {
                    PrefixSub = Convert.ToDateTime(DocumentDate).Year.ToString("0000") + Convert.ToDateTime(DocumentDate).Month.ToString("00") + "00";
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "Y")
                {
                    PrefixSub = Convert.ToDateTime(FAyearfrom).Year.ToString("0000") + Convert.ToDateTime(FAyearto).Year.ToString("0000");
                }
                if (DS.Tables[0].Rows[0]["PrefixSub"].ToString().ToUpper() == "F")
                {
                    PrefixSub = DS.Tables[0].Rows[0]["PrefixFixed"].ToString().ToUpper();
                }
                if (PrefixSub.Trim() == string.Empty)
                    PrefixSub = "00000000";
                DocumentPrefix += PrefixSub + "/";

                //BookPrefix
                if (DS.Tables[0].Rows[0]["BookPrefix"].ToString().Trim() != string.Empty)
                    DocumentPrefix += DS.Tables[0].Rows[0]["BookPrefix"].ToString().Trim() + "/";
                else
                    DocumentPrefix += DS.Tables[0].Rows[0]["BookCode"].ToString().Trim() + "/";

                //string PrefixSerial = "A";
                ////PrefixSerial
                //if (DS.Tables[0].Rows[0]["PrefixSerial"].ToString().Trim() != string.Empty)
                //    PrefixSerial = DS.Tables[0].Rows[0]["PrefixSerial"].ToString().Trim();
                //if ((PrefixSerial != "A") || (PrefixSerial != "M"))
                //    PrefixSerial = "A";
                //DocumentPrefix += PrefixSerial;
            }
        }
        catch
        {
        }
        finally
        {
            DS = null;
            DAL = null;
            SQLP = null;
        }
        return DocumentPrefix;
    }


    public double getProductRate(string ProductId, string BranchId, string AccountId, string RateType, string RatePlan, double DefaultRate)
    {
        double ProductRate = 0;
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[4];
        SQLP[0] = new SqlParameter("@ProductId", SqlDbType.BigInt);
        SQLP[0].Value = ProductId.ToString();
        SQLP[1] = new SqlParameter("@BranchId", SqlDbType.BigInt);
        SQLP[1].Value = BranchId.ToString();
        SQLP[2] = new SqlParameter("@AccountId", SqlDbType.BigInt);
        SQLP[2].Value = AccountId.ToString();
        SQLP[3] = new SqlParameter("@RatePlan", SqlDbType.BigInt);
        SQLP[3].Value = RatePlan.ToString();

        DataSet DS = new DataSet();
        try
        {
            DS = DAL.GetDataSet("ProductMaster_RateList", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
                ProductRate = Convert.ToDouble(DS.Tables[0].Rows[0][RateType + "Rate"].ToString());

            if (ProductRate == 0)
                ProductRate = DefaultRate;
        }
        catch
        {
        }
        finally
        {
        }
        return ProductRate;
    }

    public string[] getBookInfo(string BookId)
    {
        string[] BookInfo = new string[45];
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[1];
        SQLP[0] = new SqlParameter("@BookId", SqlDbType.BigInt);
        SQLP[0].Value = BookId.ToString();
        DataSet DS = new DataSet();
        try
        {
            DS = DAL.GetDataSet("BookMaster_InfoData", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
            {
                BookInfo[0] = DS.Tables[0].Rows[0]["BookType"].ToString();
                BookInfo[1] = DS.Tables[0].Rows[0]["BookTable"].ToString();
                BookInfo[2] = DS.Tables[0].Rows[0]["BookKey"].ToString();
                BookInfo[3] = DS.Tables[0].Rows[0]["BookCode"].ToString();
                BookInfo[4] = DS.Tables[0].Rows[0]["BookPrefix"].ToString();
                BookInfo[5] = DS.Tables[0].Rows[0]["BookSrNo"].ToString();
                BookInfo[6] = DS.Tables[0].Rows[0]["BookName"].ToString();
                BookInfo[7] = DS.Tables[0].Rows[0]["StockEffect"].ToString();
                BookInfo[8] = DS.Tables[0].Rows[0]["AccountEffect"].ToString();
                BookInfo[9] = DS.Tables[0].Rows[0]["AccountId"].ToString();
                BookInfo[10] = DS.Tables[0].Rows[0]["RoundUpId"].ToString();
                BookInfo[11] = DS.Tables[0].Rows[0]["PostingId"].ToString();
                BookInfo[12] = DS.Tables[0].Rows[0]["BookDescription"].ToString();
                BookInfo[12] = DS.Tables[0].Rows[0]["BookStatus"].ToString();
                BookInfo[14] = DS.Tables[0].Rows[0]["FixedBook"].ToString();
                BookInfo[15] = DS.Tables[0].Rows[0]["QCRequired"].ToString();
                BookInfo[16] = DS.Tables[0].Rows[0]["NoBilling"].ToString();
                BookInfo[17] = DS.Tables[0].Rows[0]["NoInventory"].ToString();
                BookInfo[18] = DS.Tables[0].Rows[0]["BookCondition"].ToString();
                BookInfo[19] = DS.Tables[0].Rows[0]["PrefixMain"].ToString();
                BookInfo[20] = DS.Tables[0].Rows[0]["PrefixSub"].ToString();
                BookInfo[21] = DS.Tables[0].Rows[0]["PrefixFixed"].ToString();
                BookInfo[22] = DS.Tables[0].Rows[0]["PrefixSerial"].ToString();
                BookInfo[23] = DS.Tables[0].Rows[0]["PrefixPrint"].ToString();
                BookInfo[24] = DS.Tables[0].Rows[0]["IsExcisable"].ToString();
                BookInfo[25] = DS.Tables[0].Rows[0]["IsTaxable"].ToString();
                BookInfo[26] = DS.Tables[0].Rows[0]["ProductGroups"].ToString();
                BookInfo[27] = DS.Tables[0].Rows[0]["AccountCategoryIds"].ToString();
                BookInfo[28] = DS.Tables[0].Rows[0]["ProductCategoryIds"].ToString();
                BookInfo[29] = DS.Tables[0].Rows[0]["StoreLocationIds"].ToString();
                BookInfo[30] = DS.Tables[0].Rows[0]["DepartmentIds"].ToString();
                BookInfo[31] = DS.Tables[0].Rows[0]["TaxMasterIds"].ToString();
                BookInfo[32] = DS.Tables[0].Rows[0]["ExciseMasterIds"].ToString();
                BookInfo[33] = DS.Tables[0].Rows[0]["TermsConditionIds"].ToString();
                BookInfo[34] = DS.Tables[0].Rows[0]["ParentBookIds"].ToString();
                BookInfo[35] = DS.Tables[0].Rows[0]["ParentBookType"].ToString();
                BookInfo[36] = DS.Tables[0].Rows[0]["ApprovalLevel"].ToString();
                BookInfo[37] = DS.Tables[0].Rows[0]["Approval1UserId"].ToString();
                BookInfo[38] = DS.Tables[0].Rows[0]["Approval2UserId"].ToString();
                BookInfo[39] = DS.Tables[0].Rows[0]["DisplayRate"].ToString();
                BookInfo[40] = DS.Tables[0].Rows[0]["MessageType"].ToString();
                BookInfo[41] = DS.Tables[0].Rows[0]["MessagePrompt"].ToString();
                BookInfo[42] = "";
                BookInfo[43] = "";
                BookInfo[44] = "";
            }
        }
        catch
        {
        }
        finally
        {
        }
        return BookInfo;
    }

    public string[] getBookMainInfo(string BookType)
    {
        string[] BookInfo = new string[5];
        DSIT_DataLayer DAL = new DSIT_DataLayer();
        SqlParameter[] SQLP = new SqlParameter[1];
        SQLP[0] = new SqlParameter("@BookType", SqlDbType.NVarChar, 50);
        SQLP[0].Value = BookType.ToString();
        DataSet DS = new DataSet();
        try
        {
            DS = DAL.GetDataSet("BookMain_ListData", SQLP);
            if (DS.Tables[0].Rows.Count > 0)
            {
                BookInfo[0] = DS.Tables[0].Rows[0]["BookType"].ToString();
                BookInfo[1] = DS.Tables[0].Rows[0]["BookTable"].ToString();
                BookInfo[2] = DS.Tables[0].Rows[0]["BookKey"].ToString();
                BookInfo[3] = DS.Tables[0].Rows[0]["BookName"].ToString();
                BookInfo[4] = "";
            }
        }
        catch
        {
        }
        finally
        {
        }
        return BookInfo;
    }

    //public bool PrintReport(Page ActivePage, CrystalDecisions.CrystalReports.Engine.ReportDocument CRSource, CrystalDecisions.Web.CrystalReportViewer CRViewer, string PrintOutput)
    //{
    //    bool lPrint = true;
    //    try
    //    {
    //        //CRSource.PrintToPrinter(0, false, 0, 0); // Direct Print

    //        //switch (System.Web.HttpContext.Current.Session["REPORTOPTION"].ToString().Trim().ToUpper())
    //        switch (PrintOutput.Trim().ToUpper())
    //        {
    //            case "PDF":         ////"Adobe (PDF)"
    //                CRSource.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, ActivePage.Response, false, "");
    //                break;
    //            case "DOC":         ////"Word (DOC)"
    //                CRSource.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.WordForWindows, ActivePage.Response, false, "");
    //                break;
    //            case "XLS":         ////"Excel (XLS)"
    //                CRSource.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, ActivePage.Response, false, "");
    //                break;
    //            default:            ////"Crystal (RPT)"
    //                CRViewer.ReportSource = CRSource;
    //                CRViewer.PrintMode = CrystalDecisions.Web.PrintMode.ActiveX;
    //                CRViewer.EnableDrillDown = false;
    //                CRViewer.DisplayGroupTree = false;
    //                CRViewer.HasCrystalLogo = false;
    //                CRViewer.HasDrillUpButton = false;
    //                CRViewer.HasToggleGroupTreeButton = false;
    //                CRViewer.HasViewList = false;
    //                break;
    //        }
    //    }
    //    catch (Exception ee)
    //    {

    //        Console.Write(ee);
    //        //if (System.Web.HttpContext.Current.Session["REPORTOUTPUT"].ToString().Trim().ToUpper() != "RPT")
    //        //{
    //        //    CRViewer.ReportSource = CRSource;
    //        //    CRViewer.PrintMode = CrystalDecisions.Web.PrintMode.ActiveX;
    //        //    CRViewer.EnableDrillDown = false;
    //        //    CRViewer.DisplayGroupTree = false;
    //        //    CRViewer.HasCrystalLogo = false;
    //        //    CRViewer.HasDrillUpButton = false;
    //        //    CRViewer.HasToggleGroupTreeButton = false;
    //        //    CRViewer.HasViewList = false;
    //        //}
    //    }
    //    finally
    //    {
    //    }
    //    return lPrint;
    //}


    public void SetProjectSetUp(string cUserId)
    {

        DSIT_DataLayer BAL = new DSIT_DataLayer();
        DataSet ds = new DataSet();
        string Query = "";
        try
        {
            Query = "Execute ProjectSetup_ListData 'WEBSERVICE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["WEBSERVICE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'GRNQCDIRECT'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["GRNQCDIRECT"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'CLASS2NEW'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["CLASS2NEW"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'WODESIGNER'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["WODESIGNER"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();


            Query = "Execute ProjectSetup_ListData 'SALESCONSIGNEE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["SALESCONSIGNEE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'SALESPAYSCHEDULE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["SALESPAYSCHEDULE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'PURCHASECONSIGNEE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["PURCHASECONSIGNEE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'PURCHASEPAYSCHEDULE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["PURCHASEPAYSCHEDULE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'AGENT'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["AGENT"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'SALESMAN'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["SALESMAN"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'TRANSPORTER'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["TRANSPORTER"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'EXCISE'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["EXCISE"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'NEGATIVESTOCK'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["NEGATIVESTOCK"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'NEGATIVEPROMPT'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["NEGATIVEPROMPT"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute ProjectSetup_ListData 'REPORTOUTPUT'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                System.Web.HttpContext.Current.Session["REPORTOUTPUT"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            string nMonthFr = "4";
            string nMonthTo = "3";
            Query = "Execute ProjectSetup_ListData 'FINANCIALMONTH'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                nMonthFr = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();
                nMonthTo = (Convert.ToInt16(nMonthFr) - 1).ToString();
                if (nMonthTo == "0")
                    nMonthTo = "12";
            }
            System.Web.HttpContext.Current.Session["MONTHFROM"] = nMonthFr;
            System.Web.HttpContext.Current.Session["MONTHTO"] = nMonthTo;
            //string.Format("MMMM",1)    

            //System.Web.HttpContext.Current.Session["REPORTOUTPUT"] = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

            Query = "Execute App_DefaultData " + cUserId;
            BAL.ExcuteNonSQL(Query);

            //''

        }
        catch (Exception ee)
        {
            Console.Write(ee);
            return;
        }
        finally
        {
            BAL = null;
            ds = null;
        }
    }

    public string GetMonthName(int monthNum, bool abbreviate)
    {
        if (monthNum < 1 || monthNum > 12)
            throw new ArgumentOutOfRangeException("monthNum");
        DateTime date = new DateTime(1, monthNum, 1);
        if (abbreviate)
            return date.ToString("MMM");
        else
            return date.ToString("MMMM");
    }

    public string GetAccountGroupKey(string BookType)
    {
        String AccountGroupKey = "";
        switch (BookType)
        {
            case "BB": //"Bank Book"
                AccountGroupKey = "B";
                break;
            case "CC": //"Cash Book"
                AccountGroupKey = "C";
                break;
            case "JJ": //"Journal"
                AccountGroupKey = "";
                break;
            case "QQ": //"Contra"
                AccountGroupKey = "";
                break;
            case "EE": //"Petty Cash"
                AccountGroupKey = "C";
                break;
            case "DD": //"Debit Note"
                AccountGroupKey = "";
                break;
            case "NN": //"Credit Note"
                AccountGroupKey = "";
                break;
            case "SS": //"Sales"
                AccountGroupKey = "I";
                break;
            case "PP": //"Purchase"
                AccountGroupKey = "X";
                break;
            case "D": //"Debtors"
                AccountGroupKey = "D";
                break;
            case "S": //"Creditors"
                AccountGroupKey = "S";
                break;
            case "R": //"Reserve & Surplus"
                AccountGroupKey = "R";
                break;
            case "X": //"Expense"
                AccountGroupKey = "X";
                break;
            case "I": //"Income"
                AccountGroupKey = "I";
                break;
        }
        return AccountGroupKey;
    }

    public string DefaultPrinterName()
    {
        string PrinterName = "";
        System.Drawing.Printing.PrinterSettings oPS = new System.Drawing.Printing.PrinterSettings();

        try
        {
            PrinterName = oPS.PrinterName;
        }
        catch (System.Exception ex)
        {
            Console.Write(ex);
            PrinterName = "";
        }
        finally
        {
            oPS = null;
        }
        return PrinterName;
    }

    public void CheckProjectYear(string ddtFAFrom, string ddtFATo)
    {
        string SQLQuery = "Execute ProjectYear_Manage  '" + ddtFAFrom + "','" + ddtFATo + "'";
        DSIT_DataLayer BAL = new DSIT_DataLayer();
        try
        {
            BAL.ExcuteNonSQL(SQLQuery);
        }
        catch (Exception ee)
        {
            Console.Write(ee);
        }
        finally
        {
            BAL = null;
        }
    }

    public bool FormulaSyntaxValid(string formulaField, Int16 columnRange, string[] pattenField, out double formulaValue)
    {


        bool lflg = true;
        formulaValue = 0;
        if (formulaField.Trim() != string.Empty)
        {
            string[] cFields = formulaField.Trim().Split('{');
            Array.Sort(pattenField);
            for (int i = 0; i < cFields.Length; i++)
            {
                if (cFields[i].ToString().IndexOf("}") >= 0)
                {
                    cFields[i] = cFields[i].Substring(0, cFields[i].ToString().IndexOf("}"));
                    if (cFields[i].Trim() != string.Empty)
                    {
                        if (Array.BinarySearch(pattenField, cFields[i].ToString().ToUpper()) >= 0)
                        {
                        }
                        else
                        {
                            if (IsNumeric(cFields[i]) == true)
                            {
                                if (cFields[i].ToString().IndexOf(".") >= 0)
                                {
                                    lflg = false;
                                    break;
                                }
                                else if (cFields[i].ToString().IndexOf(",") >= 0)
                                {
                                    lflg = false;
                                    break;
                                }
                                else if (Convert.ToInt16(cFields[i]) > columnRange)
                                {
                                    lflg = false;
                                    break;
                                }
                                else if (Convert.ToInt32(cFields[i]) <= 0)
                                {
                                    lflg = false;
                                    break;
                                }
                            }
                            else
                            {
                                lflg = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cFields[i] = "";
                }
            }
            if (lflg == true)
            {
                string cQuery = formulaField.Trim();
                cQuery = cQuery.Replace("{QTY}", "1").Replace("{RATE}", "1").Replace("{ITEMQTY}", "1").Replace("{ITEMAMT}", "1").Replace("{TAXABLE}", "1").Replace("{TAXAMT}", "1").Replace("{ASSESSABLE}", "1").Replace("{EXCISEAMT}", "1").Replace("{RUNNINGAMT}", "1").Replace("{", "").Replace("}", "");

                DSIT_DataLayer BAL = new DSIT_DataLayer();
                SqlDataReader dsReader = null;
                string Query = "EXECUTE APP_ExecuteFormula '" + cQuery + "'";
                dsReader = BAL.GetDataReader(Query);
                while (dsReader.Read())
                {
                    bool nn = double.TryParse(dsReader["FormulaValue"].ToString(), out formulaValue);
                    if (nn == false)
                        lflg = false;
                }
                dsReader.Dispose();
                BAL = null;
            }
        }
        return lflg;

    }

    public double FormulaSyntaxValue(string formulaField, Control cRoot, Control ActivePage)
    {
        double formulaValue = 0;
        if (formulaField.Trim() != string.Empty)
        {
            try
            {
                string[] pattenField = { "QTY", "RATE", "ITEMQTY", "ITEMAMT", "TAXABLE", "ASSESSABLE", "EXCISEAMT", "TAXAMT", "RUNNINGAMT" };
                formulaField = formulaField.Trim().ToUpper();

                string[] cFields = formulaField.Trim().Split('{');
                Array.Sort(pattenField);
                for (int i = 0; i < cFields.Length; i++)
                {
                    if (cFields[i].ToString().IndexOf("}") >= 0)
                    {
                        cFields[i] = cFields[i].Substring(0, cFields[i].ToString().IndexOf("}"));
                        if (cFields[i].Trim() != string.Empty)
                        {
                            if (Array.BinarySearch(pattenField, cFields[i].ToString().ToUpper()) >= 0)
                            {
                            }
                            else
                            {
                                if (IsNumeric(cFields[i]) == true)
                                {
                                    string cValue = "";
                                    Control ctrlValue = ActivePage.FindControl("txtvalue" + cFields[i].ToString());
                                    Control ctrlCombo = ActivePage.FindControl("cmbvalue" + cFields[i].ToString());
                                    if ((((TextBox)ctrlValue).Visible == true) && (((TextBox)ctrlValue).Text != string.Empty))
                                        cValue = ((TextBox)ctrlValue).Text;
                                    if ((((DropDownList)ctrlCombo).Visible == true) && (((DropDownList)ctrlCombo).SelectedItem.Text != string.Empty))
                                        cValue = ((DropDownList)ctrlCombo).SelectedItem.Text;

                                    formulaField = formulaField.Replace("{" + cFields[i] + "}", "{" + cValue + "}");
                                }
                            }
                        }
                    }
                }

                object ctrl = FindAllControl(cRoot, "txtQty");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{QTY}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtRate");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{RATE}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtItemQty");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{ITEMQTY}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtItemAmount");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{ITEMAMT}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtTaxableAmount");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{TAXABLE}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtTaxAmount");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{TAXAMT}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtAssessebleAmount");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{ASSESSABLE}", ((TextBox)ctrl).Text);

                ctrl = FindAllControl(cRoot, "txtExciseAmount");
                if (ctrl != null)
                    if (ctrl.GetType() == typeof(TextBox))
                        formulaField = formulaField.Replace("{EXCISEAMT}", ((TextBox)ctrl).Text);

                formulaField = formulaField.Replace("{RUNNINGAMT}", "1").Replace("{", "").Replace("}", "");

                DSIT_DataLayer BAL = new DSIT_DataLayer();
                SqlDataReader dsReader = null;
                string Query = "EXECUTE APP_ExecuteFormula '" + formulaField + "'";
                dsReader = BAL.GetDataReader(Query);
                while (dsReader.Read())
                {
                    bool nn = double.TryParse(dsReader["FormulaValue"].ToString(), out formulaValue);
                    break;
                }
                dsReader.Dispose();
                BAL = null;
            }
            catch
            {
            }
            finally
            {
            }
        }
        return formulaValue;
    }

    public double ConvertUnitQty(string txtPackSizeQty, string cmbPackSizeUnitId, string cmbUnitId)
    {
        double ConvertValue = Convert.ToDouble(txtPackSizeQty);
        if (cmbPackSizeUnitId != cmbUnitId)
        {
            DSIT_DataLayer BAL = new DSIT_DataLayer();
            DataSet DS = new DataSet();
            string UnitFactor = "";
            try
            {
                string StrQuery = "Execute UnitConvertion_Populate " + cmbPackSizeUnitId + "," + cmbUnitId;
                DS = BAL.GetDataSetSql(StrQuery);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() != "")
                        if (DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() != "0")
                            UnitFactor = "(" + txtPackSizeQty + " " + DS.Tables[0].Rows[0]["UnitConvertionFactor"].ToString() + " " + DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() + ")";
                }
                if (UnitFactor.Trim() == string.Empty)
                {
                    StrQuery = "Execute UnitConvertion_Populate " + cmbUnitId + "," + cmbPackSizeUnitId;
                    DS = BAL.GetDataSetSql(StrQuery);
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        if (DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() != "")
                            if (DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() != "0")
                                UnitFactor = "(" + txtPackSizeQty + " " + DS.Tables[0].Rows[0]["UnitConvertionFactor"].ToString() + " " + DS.Tables[0].Rows[0]["UnitConvertionValue"].ToString() + ")";
                    }
                }
                if (UnitFactor.Trim() != string.Empty)
                {
                    StrQuery = "Select " + UnitFactor + " As ConvertValue ";
                    DS = BAL.GetDataSetSql(StrQuery);
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        if (DS.Tables[0].Rows.Count > 0)
                        {
                            ConvertValue = Convert.ToDouble(DS.Tables[0].Rows[0]["ConvertValue"].ToString());
                        }
                    }

                }
            }
            catch
            {
            }
            finally
            {
                DS = null;
                BAL = null;
            }
        }
        return ConvertValue;
    }

    public string[] CalculateItemTaxExcise(string txtQty, string txtAmount, string txtTaxableAmt, string txtAssessebleAmt, string txtTaxAmt, string txtExciseAmt, string cmbTaxExciseId)
    {
        string[] CalculateValue = new string[3];
        string[] pattenField = { "QTY", "RATE", "ITEMQTY", "ITEMAMT", "TAXABLE", "ASSESSABLE", "EXCISEAMT", "TAXAMT", "RUNNINGAMT" };
        CalculateValue[0] = "0.00";
        CalculateValue[1] = "";
        CalculateValue[2] = "0.00";
        //CalculateValue[3] = "0.00";
        string formulaField = "";
        double formulaValue = 0;
        if (Convert.ToInt32(cmbTaxExciseId) > 0)
        {
            DSIT_DataLayer BAL = new DSIT_DataLayer();
            DataSet DS = new DataSet();
            try
            {
                string StrQuery = "Execute TaxMaster_ListData " + cmbTaxExciseId;
                DS = BAL.GetDataSetSql(StrQuery);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    double[] TaxExciseValue = new double[DS.Tables[0].Rows.Count];
                    for (int r = 0; r < DS.Tables[0].Rows.Count; r++)
                    {
                        formulaField = DS.Tables[0].Rows[r]["ApplicableOn"].ToString().Trim().ToUpper();
                        string[] cFields = formulaField.Trim().Split('{');
                        Array.Sort(pattenField);
                        for (int i = 0; i < cFields.Length; i++)
                        {
                            if (cFields[i].ToString().IndexOf("}") >= 0)
                            {
                                cFields[i] = cFields[i].Substring(0, cFields[i].ToString().IndexOf("}"));
                                if (cFields[i].Trim() != string.Empty)
                                {
                                    if (Array.BinarySearch(pattenField, cFields[i].ToString().ToUpper()) >= 0)
                                    {
                                    }
                                    else
                                    {
                                        if (IsNumeric(cFields[i]) == true)
                                        {
                                            string cValue = "";
                                            if (TaxExciseValue.Length >= Convert.ToInt16(cFields[i]))
                                                cValue = TaxExciseValue[Convert.ToInt16(cFields[i]) - 1].ToString();

                                            formulaField = formulaField.Replace("{" + cFields[i] + "}", "{" + cValue + "}");
                                        }
                                    }
                                }
                            }
                        }
                        formulaField = formulaField.Replace("{QTY}", txtQty);
                        //formulaField = formulaField.Replace("{RATE}", ((TextBox)ctrl).Text);
                        //formulaField = formulaField.Replace("{ITEMQTY}", ((TextBox)ctrl).Text);
                        formulaField = formulaField.Replace("{ITEMAMT}", txtAmount);
                        formulaField = formulaField.Replace("{TAXABLE}", txtTaxableAmt);
                        formulaField = formulaField.Replace("{TAXAMT}", txtTaxAmt);
                        formulaField = formulaField.Replace("{ASSESSABLE}", txtAssessebleAmt);
                        formulaField = formulaField.Replace("{EXCISEAMT}", txtExciseAmt);
                        if (r > 0)
                            formulaField = formulaField.Replace("{RUNNINGAMT}", CalculateValue[0].ToString());
                        formulaField = formulaField.Replace("{", "").Replace("}", "");

                        SqlDataReader dsReader = null;
                        string Query = "EXECUTE APP_ExecuteFormula '" + formulaField + "'";
                        dsReader = BAL.GetDataReader(Query);
                        while (dsReader.Read())
                        {
                            bool nn = double.TryParse(dsReader["FormulaValue"].ToString(), out formulaValue);
                            double nCalPer = Convert.ToDouble(DS.Tables[0].Rows[r]["ApplicableValue"].ToString());
                            double nCalVal = 0;
                            double nCalAmt = 0;
                            if (DS.Tables[0].Rows[r]["ApplicableType"].ToString() == "0")
                            {
                                if (nCalPer != 0)
                                {
                                    nCalVal = ((formulaValue * nCalPer) / 100);
                                    nCalAmt = formulaValue + nCalVal;
                                }
                            }
                            else
                            {
                                if (nCalPer != 0)
                                {
                                    nCalVal = nCalPer;
                                    nCalAmt = formulaValue + nCalPer;
                                }
                            }
                            TaxExciseValue[r] = nCalAmt;
                            CalculateValue[0] = (Convert.ToDouble(CalculateValue[0]) + nCalVal).ToString("0.00");
                            CalculateValue[1] += DS.Tables[0].Rows[r]["AccountId"].ToString().Trim() + "~" + DS.Tables[0].Rows[r]["AccountName"].ToString().Trim() + "~" + formulaValue.ToString("0.00").Trim() + "~" + nCalPer.ToString("0.00").Trim() + "~" + DS.Tables[0].Rows[r]["ApplicableType"].ToString().Trim() + "~" + nCalVal.ToString("0.00").Trim() + "~" + nCalAmt.ToString("0.00").Trim() + "|";
                            CalculateValue[2] = Convert.ToDouble(formulaValue).ToString("0.00");
                            //CalculateValue[3] = Convert.ToDouble(nCalVal).ToString("0.00");
                            // AccountId + AccountName + Applied Value + Applicable Value + Applicable Type + Calculated Value
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                DS = null;
                BAL = null;
            }
        }
        return CalculateValue;
    }


    public string[] CalculateTaxCharges(string txtItemQty, string txtItemAmount, string txtTaxableAmount, string txtAssessebleAmount, string txtTaxAmount, string txtExciseAmount, string txtRunningAmount, DataSet DS, int r)
    {
        string[] CalculateValue = new string[3];
        string[] pattenField = { "QTY", "RATE", "ITEMQTY", "ITEMAMT", "TAXABLE", "ASSESSABLE", "EXCISEAMT", "TAXAMT", "RUNNINGAMT" };
        CalculateValue[0] = "0.00";
        CalculateValue[1] = "";
        CalculateValue[2] = "0.00";
        //CalculateValue[3] = "0.00";
        string formulaField = "";
        double formulaValue = 0;
        DSIT_DataLayer BAL = new DSIT_DataLayer();
        //double[] TaxExciseValue = new double[DS.Tables[0].Rows.Count];
        try
        {
            formulaField = DS.Tables[0].Rows[r]["TaxChargeApplicableOn"].ToString().Trim().ToUpper();
            string[] cFields = formulaField.Trim().Split('{');
            Array.Sort(pattenField);
            for (int i = 0; i < cFields.Length; i++)
            {
                if (cFields[i].ToString().IndexOf("}") >= 0)
                {
                    cFields[i] = cFields[i].Substring(0, cFields[i].ToString().IndexOf("}"));
                    if (cFields[i].Trim() != string.Empty)
                    {
                        if (Array.BinarySearch(pattenField, cFields[i].ToString().ToUpper()) >= 0)
                        {
                            if (cFields[i].ToUpper() == "RUNNINGAMT")
                            {
                                string cValue = "";
                                if (r >= 0)
                                    cValue = DS.Tables[0].Rows[(r - 1)]["TaxChargeTotal"].ToString();

                                formulaField = formulaField.Replace("{" + cFields[i] + "}", "{" + cValue + "}");
                            }
                        }
                        else
                        {
                            if (IsNumeric(cFields[i]) == true)
                            {
                                string cValue = "";
                                if (DS.Tables[0].Rows.Count >= Convert.ToInt16(cFields[i]))
                                    cValue = DS.Tables[0].Rows[Convert.ToInt16(cFields[i]) - 1]["TaxChargeAmount"].ToString();

                                formulaField = formulaField.Replace("{" + cFields[i] + "}", "{" + cValue + "}");
                            }
                            else if (cFields[i].ToUpper() == "RUNNINGAMT")
                            {
                                string cValue = "";
                                if (r >= 0)
                                    cValue = DS.Tables[0].Rows[(r - 1)]["TaxChargeTotal"].ToString();

                                formulaField = formulaField.Replace("{" + cFields[i] + "}", "{" + cValue + "}");
                            }

                        }
                    }
                }
            }
            //formulaField = formulaField.Replace("{QTY}", txtQty);
            //formulaField = formulaField.Replace("{RATE}", ((TextBox)ctrl).Text);
            formulaField = formulaField.Replace("{ITEMQTY}", txtItemQty);
            formulaField = formulaField.Replace("{ITEMAMT}", txtItemAmount);
            formulaField = formulaField.Replace("{TAXABLE}", txtTaxableAmount);
            formulaField = formulaField.Replace("{TAXAMT}", txtTaxAmount);
            formulaField = formulaField.Replace("{ASSESSABLE}", txtAssessebleAmount);
            formulaField = formulaField.Replace("{EXCISEAMT}", txtExciseAmount);
            formulaField = formulaField.Replace("{RUNNINGAMT}", txtRunningAmount);
            formulaField = formulaField.Replace("{", "").Replace("}", "");

            SqlDataReader dsReader = null;
            string Query = "EXECUTE APP_ExecuteFormula '" + formulaField + "'";
            dsReader = BAL.GetDataReader(Query);
            while (dsReader.Read())
            {
                bool nn = double.TryParse(dsReader["FormulaValue"].ToString(), out formulaValue);
                double nCalPer = Convert.ToDouble(DS.Tables[0].Rows[r]["TaxChargeValue"].ToString());
                double nCalVal = 0;
                double nCalAmt = 0;
                if (DS.Tables[0].Rows[r]["TaxChargeValueType"].ToString() == "0")
                {
                    if (nCalPer != 0)
                    {
                        nCalVal = ((formulaValue * nCalPer) / 100);
                        nCalAmt = formulaValue + nCalVal;
                    }
                }
                else
                {
                    if (nCalPer != 0)
                    {
                        nCalVal = nCalPer;
                        nCalAmt = formulaValue + nCalPer;
                    }
                }
                CalculateValue[0] = Convert.ToDouble(nCalVal).ToString("0.00");
                CalculateValue[1] = "";
                CalculateValue[2] = Convert.ToDouble(formulaValue).ToString("0.00");
                //CalculateValue[3] = Convert.ToDouble(nCalVal).ToString("0.00");
                //DS.Tables[0].Rows[r]["AccountId"].ToString().Trim() + "~" + DS.Tables[0].Rows[r]["AccountName"].ToString().Trim() + "~" + formulaValue.ToString("0.00").Trim() + "~" + nCalPer.ToString("0.00").Trim() + "~" + DS.Tables[0].Rows[r]["ApplicableType"].ToString().Trim() + "~" + nCalAmt.ToString("0.00").Trim() + "|";
                break;
            }
        }
        catch
        {
        }
        finally
        {
            DS = null;
            BAL = null;
        }
        return CalculateValue;
    }
    public string RemoveNull(string strValue)
    {
        string strReturnvalue = "";
        if (strValue != null)
            strReturnvalue = strValue;
        return strReturnvalue;
    }


    public string RemoveCommas(string strValue)
    {
        string strReturnvalue = "";

        if (strValue.Trim() != string.Empty)
        {
            if (strValue.Trim().Substring(0, 1) == ",")
                strValue = strValue.Substring(1);
            if (strValue.Trim().Substring(strValue.Length - 1, 1) == ",")
                strValue = strValue.Substring(0, strValue.Length - 1);

            strValue = strValue.Replace(",,", "");
            strReturnvalue = strValue;
        }
        return strReturnvalue;
    }

    public string ConvertIntIfempty(string strValue)
    {
        string nVal = strValue.Replace("&nbsp;", "0");
        if (strValue.Trim() == string.Empty)
            nVal = "0";
        return nVal;
    }

    public int ConvertToInt(string strValue)
    {
        int nVal = 0;
        if (strValue != string.Empty)
            nVal = Convert.ToInt16(strValue);
        return nVal;
    }
    public string setCurrencyBase()
    {
        string BaseCurrency = "0";
        DataSet DS = new DataSet();
        DSIT_DataLayer EDAL = new DSIT_DataLayer();
        SqlParameter[] SQLPM = new SqlParameter[1];
        SQLPM[0] = new SqlParameter("@BaseCurrency", SqlDbType.TinyInt, 0);
        SQLPM[0].Value = 1;
        try
        {
            DS = EDAL.GetDataSet("CurrencyMaster_Base", SQLPM);
            if (DS.Tables[0].Rows.Count > 0)
            {
                BaseCurrency = DS.Tables[0].Rows[0]["CurrencyId"].ToString();
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            DS = null;
            EDAL = null;
        }
        return BaseCurrency;
    }

    public void setCurrencyRate(string cmbCurrency, TextBox txtCurrencyValue, DropDownList cmbConversionFactor)
    {
        DataSet DS = new DataSet();
        DSIT_DataLayer EDAL = new DSIT_DataLayer();
        SqlParameter[] SQLPM = new SqlParameter[1];
        SQLPM[0] = new SqlParameter("@CurrencyId", SqlDbType.BigInt, 0);
        SQLPM[0].Value = cmbCurrency;
        try
        {
            DS = EDAL.GetDataSet("CurrencyMaster_ListData", SQLPM);
            if (DS.Tables[0].Rows.Count > 0)
            {
                txtCurrencyValue.Text = DS.Tables[0].Rows[0]["CurrencyValue"].ToString();
                cmbConversionFactor.SelectedValue = DS.Tables[0].Rows[0]["ConversionFactor"].ToString();
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            DS = null;
            EDAL = null;
        }
    }

    public string setCurrencyValue(string ConvertionValue, string CurrencyValue, string ConversionFactor)
    {
        if (ConvertionValue.Trim() == string.Empty)
            ConvertionValue = "0.00";
        if (CurrencyValue.Trim() == string.Empty)
            CurrencyValue = "1";
        if (Convert.ToDouble(CurrencyValue) == 0)
            CurrencyValue = "1";
        if (ConversionFactor.Trim() == string.Empty)
            ConversionFactor = "*";
        string convertValue = ConvertionValue;
        if (ConversionFactor == "*")
            convertValue = (Convert.ToDouble(ConvertionValue) * Convert.ToDouble(CurrencyValue)).ToString("0.00");
        else
            convertValue = (Convert.ToDouble(ConvertionValue) / Convert.ToDouble(CurrencyValue)).ToString("0.00");

        return convertValue;
    }
    public bool checkProductReferenceExist(DataRow DR1, DataRow DR2, string BookKey)
    {
        string ProductClass1 = "", ProductClass2 = "";
        for (int c = 1; c <= 20; c++)
        {
            ProductClass1 += DR1["ProductClass" + c.ToString().Trim()].ToString() + ",";
            ProductClass2 += DR2["ProductClass" + c.ToString().Trim()].ToString() + ",";
        }

        if (DR1["RefBookId"].ToString() == DR2["BookId"].ToString() &&
            DR1["RefRecordId"].ToString() == DR2[BookKey + "Id"].ToString() &&
            DR1["RefRecordDetailId"].ToString() == DR2[BookKey + "DetailId"].ToString() &&
            DR1["ProductId"].ToString() == DR2["ProductId"].ToString() &&
            DR1["PackSizeUnitId"].ToString() == DR2["PackSizeUnitId"].ToString() &&
            DR1["UnitId"].ToString() == DR2["UnitId"].ToString() &&
            ProductClass1 == ProductClass2)
        {
            return true;
        }
        return false;
    }

    public bool checkProductReferenceExist(DataSet DR1, DataSet DR2, string BookKey, Int32 r)
    {
        string ProductClass1 = "", ProductClass2 = "";
        for (int c = 1; c <= 20; c++)
        {
            ProductClass1 += DR1.Tables[0].Rows[r]["ProductClass" + c.ToString().Trim()].ToString() + ",";
            ProductClass2 += DR2.Tables[0].Rows[r]["ProductClass" + c.ToString().Trim()].ToString() + ",";
        }

        if (DR1.Tables[0].Rows[r]["RefBookId"].ToString() == DR2.Tables[0].Rows[r]["BookId"].ToString() &&
            DR1.Tables[0].Rows[r]["RefRecordId"].ToString() == DR2.Tables[0].Rows[r][BookKey + "Id"].ToString() &&
            DR1.Tables[0].Rows[r]["RefRecordDetailId"].ToString() == DR2.Tables[0].Rows[r][BookKey + "DetailId"].ToString() &&
            DR1.Tables[0].Rows[r]["ProductId"].ToString() == DR2.Tables[0].Rows[r]["ProductId"].ToString() &&
            ProductClass1 == ProductClass2)
        {
            return true;
        }
        return false;
    }

    public bool checkProductExist(DataRow DR1, DataRow DR2)
    {
        string ProductClass1 = "", ProductClass2 = "";
        for (int c = 1; c <= 20; c++)
        {
            ProductClass1 += DR1["ProductClass" + c.ToString().Trim()].ToString() + ",";
            ProductClass2 += DR2["ProductClass" + c.ToString().Trim()].ToString() + ",";
        }

        if (DR1["RefBookId"].ToString() == DR2["RefBookId"].ToString() &&
            DR1["RefRecordId"].ToString() == DR2["RefRecordId"].ToString() &&
            DR1["RefRecordDetailId"].ToString() == DR2["RefRecordDetailId"].ToString() &&
            DR1["ProductId"].ToString() == DR2["ProductId"].ToString() &&
            ProductClass1 == ProductClass2)
        {
            return true;
        }
        return false;
    }

    public bool checkProductExist(DataSet DR1, DataSet DR2, Int32 r)
    {
        string ProductClass1 = "", ProductClass2 = "";
        for (int c = 1; c <= 20; c++)
        {
            ProductClass1 += DR1.Tables[0].Rows[r]["ProductClass" + c.ToString().Trim()].ToString() + ",";
            ProductClass2 += DR2.Tables[0].Rows[r]["ProductClass" + c.ToString().Trim()].ToString() + ",";
        }

        if (DR1.Tables[0].Rows[r]["RefBookId"].ToString() == DR2.Tables[0].Rows[r]["RefBookId"].ToString() &&
            DR1.Tables[0].Rows[r]["RefRecordId"].ToString() == DR2.Tables[0].Rows[r]["RefRecordId"].ToString() &&
            DR1.Tables[0].Rows[r]["RefRecordDetailId"].ToString() == DR2.Tables[0].Rows[r]["RefRecordDetailId"].ToString() &&
            DR1.Tables[0].Rows[r]["ProductId"].ToString() == DR2.Tables[0].Rows[r]["ProductId"].ToString() &&
            ProductClass1 == ProductClass2)
        {
            return true;
        }
        return false;
    }

    //public FormulaSyntaxArg FormulaSyntaxValue(string formulaField, Int16 columnRange, string[] pattenField)
    //{
    //    FormulaSyntaxArg FormulaSyntax = new FormulaSyntaxArg();
    //    bool lflg = true;
    //    double formulaValue = 0;
    //    if (formulaField.Trim() != string.Empty)
    //    {
    //        string[] cFields = formulaField.Trim().Split('{');
    //        Array.Sort(pattenField);
    //        for (int i = 0; i < cFields.Length; i++)
    //        {
    //            if (cFields[i].ToString().IndexOf("}") >= 0)
    //            {
    //                cFields[i] = cFields[i].Substring(0, cFields[i].ToString().IndexOf("}"));
    //                if (cFields[i].Trim() != string.Empty)
    //                {
    //                    if (Array.BinarySearch(pattenField, cFields[i].ToString().ToUpper()) >= 0)
    //                    {
    //                    }
    //                    else
    //                    {
    //                        if (IsNumeric(cFields[i]) == true)
    //                        {
    //                            if (cFields[i].ToString().IndexOf(".") >= 0)
    //                            {
    //                                lflg = false;
    //                                break;
    //                            }
    //                            else if (cFields[i].ToString().IndexOf(",") >= 0)
    //                            {
    //                                lflg = false;
    //                                break;
    //                            }
    //                            else if (Convert.ToInt16(cFields[i]) > columnRange)
    //                            {
    //                                lflg = false;
    //                                break;
    //                            }
    //                            else if (Convert.ToInt32(cFields[i]) <= 0)
    //                            {
    //                                lflg = false;
    //                                break;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            lflg = false;
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                cFields[i] = "";
    //            }
    //        }
    //        if (lflg == true)
    //        {
    //            string cQuery = formulaField.Trim();
    //            cQuery = cQuery.Replace("{QTY}", "1").Replace("{RATE}", "1").Replace("{", "").Replace("}", "");

    //            DSIT_DataLayer BAL = new DSIT_DataLayer();
    //            SqlDataReader dsReader = null;
    //            string Query = "EXECUTE APP_ExecuteFormula '" + cQuery + "'";
    //            dsReader = BAL.GetDataReader(Query);
    //            while (dsReader.Read())
    //            {
    //                bool nn = double.TryParse(dsReader["FormulaValue"].ToString(), out formulaValue);
    //                if (nn == false)
    //                    lflg = false;
    //            }
    //            dsReader.Dispose();
    //            BAL = null;
    //        }
    //    }
    //    FormulaSyntax.Valid = lflg;
    //    FormulaSyntax.Value = formulaValue;
    //    return  FormulaSyntax;  //(new {IsValid = lflg, Value = formulaValue}); 
    //}



    public void UpdateUserAuditTrial(DSIT_DataLayer BAL, string intUserId, string LogEvent)
    {
        string Query = "Execute UserAuditTrail_Manage '" + intUserId + "','" + LogEvent + "'";

        try
        {
            BAL.ExecuteSqlNonQuery(Query);
        }
        catch
        {
        }
        finally
        {

        }
    }
    public void UpdateUserAuditTrial(string intUserId, string LogEvent)
    {

        DSIT_DataLayer BAL = new DSIT_DataLayer();


        string Query = "Execute UserAuditTrail_Manage '" + intUserId + "','" + LogEvent + "'";

        try
        {
            BAL.ExecuteSqlNonQuery(Query);
        }
        catch
        {
        }
        finally
        {

            BAL = null;
        }
    }


    public void UserLogoutAuditEntry(ref DSIT_DataLayer BAL, string oUserId, string logOutMsg)
    {
        #region "Variable declaration section."
        SqlParameter[] SQLP = null;
        #endregion

        try
        {
            #region "Loading the sql parameters."
            SQLP = new SqlParameter[4];

            SQLP[0] = new SqlParameter("@UserId", SqlDbType.BigInt);
            SQLP[0].Value = oUserId;

            SQLP[1] = new SqlParameter("@LogEvent", SqlDbType.NVarChar, 250);
            SQLP[1].Value = logOutMsg;

            SQLP[2] = new SqlParameter("@LogDateTime", SqlDbType.DateTime);
            SQLP[2].Value = DateTime.Now;

            SQLP[3] = new SqlParameter("@hdnRecordKeyId", SqlDbType.BigInt);
            SQLP[3].Value = "0";

            BAL = new DSIT_DataLayer();
            BAL.ExecuteSP("UserAuditTrail_Manage", SQLP);
            #endregion


        }
        catch { }
    }

    public SqlParameter GetSqlParameter(string strParameterName, object strValue, SqlDbType dbType, int size, ParameterDirection ParaDirection)
    {
        SqlParameter SQLPM = new SqlParameter();
        SQLPM.ParameterName = strParameterName;
        SQLPM.Size = size;
        SQLPM.Value = strValue;
        SQLPM.SqlDbType = dbType;
        SQLPM.Direction = ParaDirection;
        return SQLPM;
    }
    public bool GeneratingPDFWithoutRendering(string strArguments, string EXE_Path, string OutputPDFFilePath)
    {
        bool blFileStatus = false;
        try
        {
            blFileStatus = true;
            //Execute command on wkhtmltopdf
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + EXE_Path + "\\" + strArguments);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;


            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();



            FileStream fs = new FileStream(OutputPDFFilePath, FileMode.Open, FileAccess.Read);
            byte[] fileContent = null;
            fileContent = new byte[(int)fs.Length];

            //read the content
            fs.Read(fileContent, 0, (int)fs.Length);

            //close the stream
            fs.Close();



            if (fileContent != null)
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("content-length", fileContent.Length.ToString());
                System.Web.HttpContext.Current.Response.BinaryWrite(fileContent);

            }

            System.Threading.Thread.Sleep(1000);

            if (File.Exists(OutputPDFFilePath))
            {
                try { File.Delete(OutputPDFFilePath); }
                catch (Exception ex) { }
            }
            blFileStatus = true;
            System.Web.HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            blFileStatus = false;
        }
        return blFileStatus;
    }
    public bool GeneratingPDF(string strArguments, string EXE_Path, string OutputPDFFilePath)
    {
        bool blFileStatus = false;
        try
        {
            blFileStatus = true;
            //Execute command on wkhtmltopdf
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + EXE_Path + "\\" + strArguments);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;


            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();




        }
        catch (Exception ex)
        {
            blFileStatus = false;
        }
        return blFileStatus;
    }

    public bool RenderingPDF(string OutputPDFFilePath)
    {
        bool blFileStatus = false;
        try
        {
            blFileStatus = true;

            FileStream fs = new FileStream(OutputPDFFilePath, FileMode.Open, FileAccess.Read);
            byte[] fileContent = null;
            fileContent = new byte[(int)fs.Length];

            //read the content
            fs.Read(fileContent, 0, (int)fs.Length);

            //close the stream
            fs.Close();



            if (fileContent != null)
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("content-length", fileContent.Length.ToString());
                System.Web.HttpContext.Current.Response.BinaryWrite(fileContent);

            }

            System.Threading.Thread.Sleep(1000);

            if (File.Exists(OutputPDFFilePath))
            {
                try { File.Delete(OutputPDFFilePath); }
                catch (Exception ex) { }
            }
            blFileStatus = true;
            System.Web.HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            blFileStatus = false;
        }
        return blFileStatus;
    }
    public Int64 UpdatePaymentSubmission_CreateEmailLog(Int64 AccountId, string Fname)
    {

        DSIT_DataLayer objDAL = new DSIT_DataLayer();
        var parameters = new List<SqlParameter>();
        string ReturnMessage = string.Empty;
        Int64 RecordId = 0;

        parameters.Add(GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ApplicationStatus", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@UserName", Fname, SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
        parameters.Add(GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));


        objDAL.ExecuteSP("App_Accounts_AppStatus_Update_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);


        EmailConfig EmailConfig = new EmailConfig();
        Int64 ReturnId = 0;
        try
        {


            parameters = new List<SqlParameter>();
            parameters.Add(GetSqlParameter("@AccountId", AccountId, SqlDbType.BigInt, 0, ParameterDirection.Input));
            parameters.Add(GetSqlParameter("@AuthorizationLevel", 1, SqlDbType.TinyInt, 0, ParameterDirection.Input));

            DataTable DT = objDAL.GetDataTable("App_Accounts_List_Email", parameters.ToArray());

            List<KeyValuePair<string, object>> Paralist = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("RecordKeyId", AccountId),

                };
            ////-------------------------- GET BOOK NAME------------------

            //parameters = new List<SqlParameter>();
            //parameters.Add(objGeneralFunction.GetSqlParameter("@BookId", DT.Rows[0]["BookId"].ToString(), SqlDbType.BigInt, 0, ParameterDirection.Input));
            //parameters.Add(objGeneralFunction.GetSqlParameter("@BookType", "AA", SqlDbType.NVarChar, 4, ParameterDirection.Input));

            //DataTable DTBookName = DAL.GetDataTable("App_Get_BookName", parameters.ToArray());

            #region "Email Config Member"
            EmailConfig = new EmailConfig();
            EmailConfig.BookType = "AA";
            EmailConfig.EmailType = "MPS";
            EmailConfig.DTTransaction = DT;
            EmailConfig.ParaList = Paralist;
            EmailConfig.EmailTo = DT.Rows[0]["AccountEmail"].ToString();
            ReturnId = EmailConfig.CreateEmailLog();

            #endregion

            if (DT.Rows.Count > 0)
            {
                #region "Email Config Member"

                EmailConfig = new EmailConfig();
                EmailConfig.BookType = "AA";
                EmailConfig.EmailType = "MA1";
                EmailConfig.DTTransaction = DT;
                EmailConfig.EmailTo = DT.Rows[0]["Auth_EmailAddress"].ToString();
                EmailConfig.RollType = DT.Rows[0]["MemberRoleType"].ToString();
                EmailConfig.BookName = DT.Rows[0]["BookName"].ToString();
                ReturnId = EmailConfig.CreateEmailLog();
            }
            #endregion
        }
        catch (Exception ex)
        {

            throw;
        }
        return RecordId;
    }

    //public System.Drawing.Bitmap ProportionallyResizeBitmap(System.Drawing.Bitmap src, int maxHeight, int maxWidth)
    //{
    //    // original dimensions
    //    int w = src.Width;
    //    int h = src.Height;


    //    int longestDimension = (w > h) ? w : h;
    //    int shortestDimension = (w < h) ? w : h;

    //    // propotionality
    //    float factor = ((float)longestDimension) / shortestDimension;



    //    double newWidth = 0;
    //    double newHeight = 0;

    //    //newWidth = maxWidth;
    //    //newHeight = maxWidth / factor;

    //    newWidth = maxHeight * factor;
    //    newHeight = maxHeight;


    //    System.Drawing.Bitmap result = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);

    //    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)result))
    //    {
    //        g.CompositingQuality = CompositingQuality.HighQuality;
    //        g.SmoothingMode = SmoothingMode.HighQuality;
    //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //        // g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    //        g.DrawImage(src, 0, 0, (int)newWidth, (int)newHeight);
    //    }


    //    return result;
    //}

    public Bitmap ProportionallyResizeBitmap(Bitmap src, int maxHeight, int maxWidth)
    {
        // original dimensions
        int w = src.Width;
        int h = src.Height;



        // Longest and shortest dimension
        int longestDimension = (w > h) ? w : h;
        int shortestDimension = (w < h) ? w : h;

        // propotionality
        float factor = ((float)longestDimension) / shortestDimension;

        double newWidth = 0;
        double newHeight = 0;

        if (w > maxWidth)
            newWidth = maxWidth / factor;
        else
            newWidth = maxWidth;
        // newHeight = maxWidth / factor;
        if (h > maxHeight)
            newHeight = maxHeight / factor;
        //newWidth = maxHeight * factor;
        else
            newHeight = maxHeight;

        Bitmap result = new Bitmap((int)newWidth, (int)newHeight);

        using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(src, 0, 0, (int)newWidth, (int)newHeight);
        }


        return result;
    }

    public string GetProjectSetUp(string ProjectValue)
    {
        string ProjectReturn = "";
        DSIT_DataLayer BAL = new DSIT_DataLayer();
        DataSet ds = new DataSet();
        string Query = "";
        try
        {
            Query = "Execute ProjectSetup_ListData '" + ProjectValue + "'";
            ds = BAL.GetDataSetSql(Query);
            if (ds.Tables[0].Rows.Count > 0)
                ProjectReturn = ds.Tables[0].Rows[0]["ProjectSetUpValue"].ToString();

        }
        catch (Exception ee)
        {
            Console.Write(ee);
        }
        finally
        {
            BAL = null;
            ds = null;
        }

        return ProjectReturn;
    }

    public string SplitCheckListBoxID(CheckBoxList objLst)
    {
        string CounId = "";
        int i = 0;
        Int32 K = 1;
        for (i = 0; i <= objLst.Items.Count - 1; i++)
        {
            if (objLst.Items[i].Selected)
            {
                CounId += "," + objLst.Items[i].Value.Trim();
                K = K + 1;
            }
        }
        if ((CounId.Length) > 0)
        {
            return Right(CounId, (CounId.Length) - 1);
        }
        else
        {
            return CounId;
        }
    }
    public void SetGridWidth(GridView gridName, string ScreenWidth, HtmlGenericControl dvScroll)
    {
        try
        {
            if (ScreenWidth == null)
                return;
            if (ScreenWidth == string.Empty)
                return;

            if (Convert.ToInt32(ScreenWidth) <= 1800)
            {
                gridName.Width = Unit.Pixel(1800);
                dvScroll.Style.Add("width", (Convert.ToInt32(ScreenWidth) - 100).ToString() + "px");
            }
            else
            {
                gridName.Width = Unit.Pixel((Convert.ToInt32(ScreenWidth) - 300));
                dvScroll.Style.Add("width", (Convert.ToInt32(ScreenWidth) - 300).ToString() + "px");
            }

            for (int i = 0; i < gridName.Columns.Count; i++)
            {
                try
                {
                    if (gridName.Columns[i].ItemStyle.CssClass != "hidden")
                    {
                        if (gridName.Columns[i].ItemStyle.Width.Value.ToString().Contains("%") == true)
                        {
                            Int32 intGridWidth = Convert.ToInt32(gridName.Columns[i].ItemStyle.Width.Value.ToString().Replace("%", string.Empty));
                            intGridWidth = Convert.ToInt32(Convert.ToDouble(ScreenWidth) * (Convert.ToDouble(intGridWidth) / 100));
                            gridName.Columns[i].ItemStyle.Width = Unit.Pixel(intGridWidth);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        catch (Exception ex)
        {
            throw new Exception("SetGridWidth " + "\n" + ex.Message);
        }
    }
    public Int64 ResizeImage(string originalPath, string originalFileName, string newPath, string newFileName, int maximumWidth, int maximumHeight, bool enforceRatio, bool addPadding)
    {
        var image = System.Drawing.Image.FromFile(originalPath + "\\" + originalFileName);
        var imageEncoders = ImageCodecInfo.GetImageEncoders();
        EncoderParameters encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        var canvasWidth = maximumWidth;
        var canvasHeight = maximumHeight;
        var newImageWidth = maximumWidth;
        var newImageHeight = maximumHeight;
        var xPosition = 0;
        var yPosition = 0;
        Int64 ReturnId = 0;
        try
        {
            if (enforceRatio)
            {
                var ratioX = maximumWidth / (double)image.Width;
                var ratioY = maximumHeight / (double)image.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                newImageHeight = (int)(image.Height * ratio);
                newImageWidth = (int)(image.Width * ratio);

                if (addPadding)
                {
                    xPosition = (int)((maximumWidth - (image.Width * ratio)) / 2);
                    yPosition = (int)((maximumHeight - (image.Height * ratio)) / 2);
                }
                else
                {
                    canvasWidth = newImageWidth;
                    canvasHeight = newImageHeight;
                }
            }

            var thumbnail = new System.Drawing.Bitmap(canvasWidth, canvasHeight);
            var graphic = Graphics.FromImage(thumbnail);

            if (enforceRatio && addPadding)
            {
                graphic.Clear(Color.White);
            }

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(image, xPosition, yPosition, newImageWidth, newImageHeight);

            thumbnail.Save(newPath + "\\" + newFileName, imageEncoders[1], encoderParameters);
            ReturnId = 1;
            graphic.Dispose();
            image.Dispose();
        }
        catch (Exception Ex)
        {
            ReturnId = 0;
            File.Delete(originalPath + "\\" + originalFileName);
            return ReturnId;
        }
        finally
        {

            try
            {
                File.Delete(originalPath + "\\" + originalFileName);
            }
            catch (Exception ex) { }
        }
        return ReturnId;
    }

    /// <summary>
    /// For SavePaymentLog
    /// In this Function  insert/Update payment detail into Table(App_Accounts_RegPayment) and return record id.
    /// here we create transaction no like as (K+RecordID+_+Date(K15800_231117094129)). if PaymentRecieptId =0 then insert record 
    /// otherwise update record by using Store Procedure(App_Accounts_RegPayment_Manage_IPM) and return datarow of TransactionNo and PaymentRecieptId
    /// Commented By Rohit
    /// </summary>
    /// <param name="DR"></param>
    /// <param name="Fname"></param>
    /// <param name="ReturnMessage"></param>
    /// <param name="RecordId"></param>
    public void SavePaymentLog(DataRow DR,string Fname, out string ReturnMessage, out Int64 RecordId)
    {
        ReturnMessage = "";
        RecordId = 0;
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter("@PaymentRecieptId", DR["PaymentRecieptId"], SqlDbType.BigInt, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@AccountId", DR["AccountId"], SqlDbType.BigInt, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@TransactionNo", DR["TransactionNo"], SqlDbType.NVarChar, 100, ParameterDirection.InputOutput));
        parameters.Add(GetSqlParameter("@PaymentAmount", DR["PaymentAmount"], SqlDbType.Money, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@PaidAmount", DR["PaidAmount"], SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@PaymentStatus", DR["PaymentStatus"], SqlDbType.TinyInt, 0, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@PaymentGatewayResponse", DR["PaymentGatewayResponse"], SqlDbType.NVarChar, 255, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ResponseNo", DR["ResponseNo"], SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ResponseString", DR["ResponseString"], SqlDbType.NVarChar, 2000, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@LastTransactionNo", DR["LastTransactionNo"], SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@LastResponseNo", DR["LastResponseNo"], SqlDbType.NVarChar, 2000, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@LastResponseString", DR["LastResponseString"], SqlDbType.NVarChar, 2000, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@PaymentBankName", DR["PaymentBankName"], SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@UserName", Fname, SqlDbType.NVarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
        parameters.Add(GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

        DSIT_DataLayer objDAL = new DSIT_DataLayer();

        objDAL.ExecuteSP("App_Accounts_RegPayment_Manage_IPM", parameters.ToArray(), out ReturnMessage, out RecordId);
        if (RecordId > 0)
        { 
            DR["TransactionNo"] = parameters[2].Value;
            DR["PaymentRecieptId"] = RecordId;
        }
    }

    public string TranslateDateTime(string strdate)
    {
        DateTime dt;
        try
        {
            dt = DateTime.ParseExact(strdate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        try
        {
            dt = DateTime.ParseExact(strdate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        try
        {
            dt = DateTime.ParseExact(strdate, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        try
        {
            dt = DateTime.ParseExact(strdate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        try
        {
            dt = DateTime.ParseExact(strdate, "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        try
        {
            dt = DateTime.ParseExact(strdate, "dd/MMM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        catch { }

        return strdate;


    }

    public void ValidatePincode(string PincodeIdval, out string Pincode, out string AreaName)
    {
        Pincode = "";
        AreaName = "";
        DSIT_DataLayer objDAL = new DSIT_DataLayer();
        DataTable DT = new DataTable();
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter("@Type", "AM", SqlDbType.VarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@ColumnName", "AM.GeographicalCode,AM.GeographicalName", SqlDbType.VarChar, 100, ParameterDirection.Input));
        parameters.Add(GetSqlParameter("@WhereClause", "AM.GeographicalId=" + PincodeIdval + "", SqlDbType.VarChar, 100, ParameterDirection.Input));

        DT = objDAL.GetDataTable("App_GetGeographicalData", parameters.ToArray());
        if (DT.Rows.Count > 0)
            if (DT.Rows.Count > 0)
            {
                Pincode = DT.Rows[0]["GeographicalCode"].ToString();
                AreaName = DT.Rows[0]["GeographicalName"].ToString();
            }

    }
}
