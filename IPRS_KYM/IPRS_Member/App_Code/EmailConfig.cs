using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

//namespace IPRS_Member.App_Code
//{

public class EmailConfig
{
    GeneralFunction objGeneralFunction = new GeneralFunction();
    private string _EmailContent;

    private string _EmailType;

    private string _BookType;


    private string _DocumentAttach;

    //private string _EmailToAddress;



    private DataTable _DTTransaction;


    //private List<Dictionary<string, object>> _DataList;

    private List<KeyValuePair<string, object>> _ParaList;

    public List<KeyValuePair<string, object>> ParaList
    {
        get { return _ParaList; }
        set { _ParaList = value; }
    }

    private string _Name;

    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    private string _EmailTo;

    public string EmailTo
    {
        get { return _EmailTo; }
        set { _EmailTo = value; }
    }

    private string _EmailCC;

    public string EmailCC
    {
        get { return _EmailCC; }
        set { _EmailCC = value; }
    }

    private string _Link;

    public string Link
    {
        get { return _Link; }
        set { _Link = value; }
    }


    //private IDictionary<string, object> _DataList = new Dictionary<string, object>()
    //                                        {
    //                                            {"{{NAME}}",""},
    //                                             {"{ACCOUNTCODE}",""},
    //                                            {"{LEVEL}", ""},
    //                                            {"{COMMENTS}",""},
    //                                             {"EMAILTO",""},
    //                                             {"EMAILCC",""},
    //                                              {"{LINK}",""},
    //                                            //{"3",""}
    //                                        };




    public DataTable DTTransaction
    {
        get { return _DTTransaction; }
        set { _DTTransaction = value; }
    }
    //public string EmailToAddress
    //{
    //    get { return _EmailToAddress == null ? string.Empty : _EmailToAddress; }
    //    set { _EmailToAddress = value; }
    //}

    public string DocumentAttach
    {
        get { return _DocumentAttach == null ? string.Empty : _DocumentAttach; }
        set { _DocumentAttach = value; }
    }



    public string BookType
    {
        get { return _BookType == null ? string.Empty : _BookType; }
        set { _BookType = value; }
    }

    private string _BookName;

    public string BookName
    {
        get { return _BookName; }
        set { _BookName = value; }
    }

    public string EmailType
    {
        get { return _EmailType == null ? string.Empty : _EmailType; }
        set { _EmailType = value; }
    }

    private string _RollType;
    public string RollType
    {
        get { return _RollType; }
        set { _RollType = value; }
    }

    public string EmailContent
    {
        get { return _EmailContent == null ? string.Empty : _EmailContent; }
        set { _EmailContent = value; }
    }

    public long CreateEmailLog()
    {
        Int64 RecordId = 0;
        string ReportParameterName = string.Empty;
        string ReportParameterValue = string.Empty;
        string ReturnMessage = string.Empty;
        string EmailStartLine = string.Empty;
        string EmailSignature = string.Empty;
        string EmailSubject = string.Empty;
        try
        {

            DSIT_DataLayer DAL = new DSIT_DataLayer();
            string Content = string.Empty;

            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@BookType", BookType, SqlDbType.NVarChar, 10, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailType", EmailType, SqlDbType.NVarChar, 10, ParameterDirection.Input));

            DataTable DT = DAL.GetDataTable("App_EmailSMSConfig_Display", parameters.ToArray());

            if (DT.Rows.Count > 0)
            {

                EmailSubject = DT.Rows[0]["EmailSubject"].ToString();
                EmailStartLine = DT.Rows[0]["EmailStartLine"].ToString();
                Content = DT.Rows[0]["EmailContent"].ToString();
                EmailSignature = DT.Rows[0]["EmailSignature"].ToString();

                if (DTTransaction != null)
                {
                    if (DTTransaction.Rows.Count > 0)
                    {
                        EmailSubject = getTableDataValue(DTTransaction, 0, EmailSubject);
                        EmailStartLine = getTableDataValue(DTTransaction, 0, EmailStartLine);
                        Content = getTableDataValue(DTTransaction, 0, Content);
                        EmailSignature = getTableDataValue(DTTransaction, 0, EmailSignature);
                    }
                }
                else
                {
                    EmailStartLine = EmailStartLine.ToUpper().Replace("{NAME}", Name);
                    Content = Content.Replace("{LINK}", Link);
                }


            }
            if (ParaList != null)
            {
                foreach (KeyValuePair<string, object> kvp in ParaList)
                {
                    ReportParameterName += kvp.Key.ToString() + "~";
                    ReportParameterValue += kvp.Value.ToString() + "~";
                    //Console.WriteLine(string.Format("Key: {0} Value: {1}", kvp.Key, kvp.Value));
                }
            }

            if (EmailContent == string.Empty)
                EmailContent = Content;

           
            if (EmailCC == null)
                EmailCC = string.Empty;

            if (EmailContent.ToUpper().Contains("{BOOKNAME}"))
            {
                EmailContent = EmailContent.Replace("{BOOKNAME}", BookName);
            }

            if (EmailContent.ToUpper().Contains("{ROLLTYPE}"))
            {
                EmailContent = EmailContent.Replace("{ROLLTYPE}", RollType);
            }
            parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailType", EmailType, SqlDbType.NVarChar, 10, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@BookType", BookType, SqlDbType.NVarChar, 50, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailToAddress", EmailTo.Trim(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailCCAddress", EmailCC.Trim(), SqlDbType.NVarChar, 200, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailSubject", EmailSubject, SqlDbType.NVarChar, 200, ParameterDirection.Input));

            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailStartLine", EmailStartLine.Replace("\n", "<br>").ToUpper(), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailContent", EmailContent.Replace("\n", "<br>"), SqlDbType.VarChar, -1, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailSignature", EmailSignature.Replace("\n", "<br>"), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@DocumentAttach", Convert.ToString(DocumentAttach), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@EmailReportValue", "", SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReportParameterName", ReportParameterName.TrimEnd('~'), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReportParameterValue", ReportParameterValue.TrimEnd('~'), SqlDbType.NVarChar, 1000, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnMessage", string.Empty, SqlDbType.NVarChar, 255, ParameterDirection.Output));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ReturnId", 0, SqlDbType.BigInt, 0, ParameterDirection.Output));

            DAL.ExecuteSP("App_EmailSMSSchedule_Manage", parameters.ToArray(), out ReturnMessage, out RecordId);
        }
        catch (Exception ex)
        {


        }
        return RecordId;
    }
    private string getTableDataValue(DataTable dsTable, int Row, string strValue)
    {
        string sendValue = strValue;
        try
        {

            if (dsTable.Columns.Contains("AccountName") == true)
            {
                sendValue = sendValue.Replace("{NAME}", dsTable.Rows[Row]["AccountName"].ToString());
            }
            if (dsTable.Columns.Contains("AccountCode") == true)
            {
                sendValue = sendValue.Replace("{CODE}", dsTable.Rows[Row]["AccountCode"].ToString());
            }
            if (sendValue.ToUpper().Contains("{LINK}"))
            {
                sendValue = sendValue.Replace("{LINK}", Link);
            }
            if (sendValue.ToUpper().Contains("{BOOKNAME}"))
            {
                sendValue = sendValue.Replace("{BOOKNAME}", BookName);
            }
            if (sendValue.ToUpper().Contains("{ROLLTYPE}"))
            {
                sendValue = sendValue.Replace("{ROLLTYPE}", RollType);
            }
            if (sendValue.ToUpper().Contains("{PASSWORD}"))
            {
                sendValue = sendValue.Replace("{PASSWORD}", clsCryptography.Decrypt(dsTable.Rows[Row]["AccountPassword"].ToString()));
            }
            if (sendValue.ToUpper().Contains("{EMAIL}"))
            {
                sendValue = sendValue.Replace("{EMAIL}", dsTable.Rows[Row]["AccountEmail"].ToString());
            }
            //if (dsTable.Tables[0].Columns.Contains("DocumentDate") == true)
            //{
            //    sendValue = sendValue.Replace("{DATE}", Convert.ToDateTime(dsTable.Tables[0].Rows[Row]["DocumentDate"].ToString()).ToString("dd/MM/yyyy"));
            //    sendValue = sendValue.Replace("{Date}", Convert.ToDateTime(dsTable.Tables[0].Rows[Row]["DocumentDate"].ToString()).ToString("dd/MM/yyyy"));
            //    sendValue = sendValue.Replace("{date}", Convert.ToDateTime(dsTable.Tables[0].Rows[Row]["DocumentDate"].ToString()).ToString("dd/MM/yyyy"));
            //}
            //if (dsTable.Tables[0].Columns.Contains("AccountName") == true)
            //{
            //    sendValue = sendValue.Replace("{NAME}", dsTable.Tables[0].Rows[Row]["AccountName"].ToString());
            //    sendValue = sendValue.Replace("{Name}", dsTable.Tables[0].Rows[Row]["AccountName"].ToString());
            //    sendValue = sendValue.Replace("{name}", dsTable.Tables[0].Rows[Row]["AccountName"].ToString());
            //}
            //if (dsTable.Tables[0].Columns.Contains("ItemQty") == true)
            //{
            //    sendValue = sendValue.Replace("{QTY}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["ItemQty"].ToString()).ToString("0.####"));
            //    sendValue = sendValue.Replace("{Qty}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["ItemQty"].ToString()).ToString("0.####"));
            //    sendValue = sendValue.Replace("{qty}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["ItemQty"].ToString()).ToString("0.####"));
            //}
            //if (dsTable.Tables[0].Columns.Contains("FinalAmount") == true)
            //{
            //    sendValue = sendValue.Replace("{AMOUNT}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["FinalAmount"].ToString()).ToString("0.00##"));
            //    sendValue = sendValue.Replace("{Amount}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["FinalAmount"].ToString()).ToString("0.00##"));
            //    sendValue = sendValue.Replace("{amount}", Convert.ToDouble(dsTable.Tables[0].Rows[Row]["FinalAmount"].ToString()).ToString("0.00##"));
            //}

            //if (dsTable.Tables[0].Columns.Contains("BusinessUnitName") == true)
            //{
            //    sendValue = sendValue.Replace("{BUSINESSUNIT}", dsTable.Tables[0].Rows[Row]["BusinessUnitName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //    sendValue = sendValue.Replace("{BusinessUnit}", dsTable.Tables[0].Rows[Row]["BusinessUnitName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //    sendValue = sendValue.Replace("{Businessunit}", dsTable.Tables[0].Rows[Row]["BusinessUnitName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //    sendValue = sendValue.Replace("{businessunit}", dsTable.Tables[0].Rows[Row]["BusinessUnitName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //}
            //if (dsTable.Tables[0].Columns.Contains("BranchName") == true)
            //{
            //    sendValue = sendValue.Replace("{BRANCH}", dsTable.Tables[0].Rows[Row]["BranchName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //    sendValue = sendValue.Replace("{Branch}", dsTable.Tables[0].Rows[Row]["BranchName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //    sendValue = sendValue.Replace("{branch}", dsTable.Tables[0].Rows[Row]["BranchName"].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
            //}


        }
        catch { }
        finally { }



        return sendValue;
    }

}
//}