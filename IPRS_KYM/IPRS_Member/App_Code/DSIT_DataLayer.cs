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
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
/// Summary description for DSIT_DataLayer
/// </summary>

public class DSIT_DataLayer
{
    SqlDataAdapter SQLD;
    SqlCommand SqlComm;
    SqlConnection myGlobalConn = new SqlConnection();
    SqlTransaction myGlobalTrans = null;

    SqlConnection myLocalConn = new SqlConnection();

    public GeneralFunction objGeneralFunction = new GeneralFunction();

    private string strDBDatabaseName = System.Configuration.ConfigurationManager.AppSettings["DBDatabaseName"];
    private string strDBServerName = System.Configuration.ConfigurationManager.AppSettings["DBServerName"];
    public string strDBUserName = System.Configuration.ConfigurationManager.AppSettings["DBUserName"];
    public string strDBPassword = System.Configuration.ConfigurationManager.AppSettings["DBPassword"];

    //public string strDBUserName = "sa";
    //public string strDBPassword = "LicRV1318";
    private string strConnectionString = string.Empty;


    public DSIT_DataLayer()
    {

        //myGlobalConn = new SqlConnection(connetionString);

    }

    public DataSet GetProjectSearchDataSet(string strProjectSearchCode)
    {
        DataSet ds = new DataSet();
        SqlConnection sqlConnection = null;
        SQLD = null;
        try
        {
            sqlConnection = CreateSQLConnection();
            sqlConnection.Open();
            SQLD = new SqlDataAdapter("App_ProjectSearch_List", sqlConnection);

            SqlParameter SQLPM = new SqlParameter();
            SQLPM.ParameterName = "@SearchCode";
            SQLPM.Size = 50;
            SQLPM.Value = strProjectSearchCode;
            SQLPM.SqlDbType = SqlDbType.NVarChar;
            SQLPM.Direction = ParameterDirection.Input;
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            SQLD.SelectCommand.Parameters.Add(SQLPM);
            ds = new DataSet();
            SQLD.Fill(ds);

            SQLD.SelectCommand.Parameters.Clear();
            return ds;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("GetProjectSearch" + "\n" + ex.Message);
        }
        finally
        {
            if (ds != null)
                ds.Dispose();
            if (SQLD != null)
                SQLD.Dispose();
            if (sqlConnection != null)
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            sqlConnection.Dispose();
        }
    }


    public DataSet Get_UserData(int UserId)
    {
        DataSet ds = new DataSet();
        ds = getUserData(UserId);
        return ds;
    }

    /// ////////////////////////////////////////////////////////////////

    public SqlDataReader GetDataReader(string strStoredProc, SqlParameter[] sqlParams)
    {
        SqlCommand myCmd = new SqlCommand();
        SqlDataReader SD;
        SqlConnection oConnection = CreateSQLConnection();
        oConnection.Open();
        try
        {
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }
            SD = SQLD.SelectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return SD;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("GetDataReader " + "\n" + strStoredProc + "\n" + ex.Message);
        }
        finally
        {
            SQLD.Dispose();
        }
    }
    public SqlConnection CreateSQLConnection()
    {
        try
        {
            if (ConfigurationManager.AppSettings["DBServerName"].ToString() != null)
                strDBServerName = ConfigurationManager.AppSettings["DBServerName"].ToString();

            if (strDBServerName == string.Empty)
            {
                return null;
            }

            strConnectionString = "Data Source=" + strDBServerName + ";Initial Catalog = " + strDBDatabaseName + ";User ID = " + strDBUserName +
            "; Password = " + strDBPassword + ";";

            SqlConnection Conn;
            Conn = new SqlConnection(strConnectionString);

            if (Conn.State == ConnectionState.Open)
                Conn.Close();
            return Conn;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("CreateSQLConnection" + "\n" + ex.Message);
        }

    }
    public Boolean Check_SQLConnection([Optional] string DBType)
    {
        Boolean ConFlag = false;
        DSIT_DataLayer EDAL = new DSIT_DataLayer();
        SqlConnection SqlConn = EDAL.CreateSQLConnection();
        try
        {
            SqlConn.Open();
            ConFlag = true;
            SqlConn.Close();
        }
        catch
        {
            ConFlag = false;
        }
        finally
        {
            SqlConn.Close();
            SqlConn.Dispose();
        }
        return ConFlag;

    }

    public void UpdateData(string Sql)
    {
        DSIT_DataLayer EDAL = new DSIT_DataLayer();
        try
        {
            EDAL.ExcuteNonSQL(Sql);
        }
        catch
        {
            throw;
        }
        finally
        {
            EDAL = null;
        }
    }

    public Int64 ExecuteSqlNonQuery(string sqlText)
    {
        Int64 Flag = 0;
        SqlConnection SqlConn = CreateSQLConnection();
        SqlComm = new SqlCommand(sqlText, SqlConn);
        SqlComm.CommandType = CommandType.Text;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            SqlConn.Open();
            Flag = Convert.ToInt64(SqlComm.ExecuteNonQuery());
        }
        catch
        {
            throw;
        }
        finally
        {
            SqlComm.Dispose();
            SqlConn.Close();
            SqlConn.Dispose();
        }
        return Flag;
    }

    public void ExecuteNonQuery(string strStoredProc, SqlParameter[] sqlParams, out int Flag)
    {
        SqlConnection SqlConn = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, SqlConn);
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            SqlConn.Open();
            SqlComm.ExecuteNonQuery();
            Flag = Convert.ToInt16(SqlComm.Parameters[sqlParams.Length - 1].Value);
        }
        catch
        {
            throw;
        }
        finally
        {

            SqlComm.Dispose();
            SqlConn.Close();
            SqlConn.Dispose();
        }
    }
    public void ExecuteNonQuery(string strStoredProc, SqlParameter[] sqlParams, [Optional] string DBType)
    {
        SqlConnection SqlConn = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, SqlConn);
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            SqlConn.Open();
            SqlComm.ExecuteNonQuery();


        }
        catch
        {
            throw;
        }
        finally
        {

            SqlComm.Dispose();
            SqlConn.Close();
            SqlConn.Dispose();
        }
    }

    public void ExecuteNonQuery(string strStoredProc, SqlParameter[] sqlParams, DSIT_DataLayer EDAL)
    {
        SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            string str = "";
            foreach (SqlParameter sqlParam in sqlParams)
            {
                SqlComm.Parameters.Add(sqlParam);
                str += "'" + (sqlParam.Value.ToString() + "',");
            }
            str += "";

            SqlComm.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {

            SqlComm.Dispose();
        }
    }

    public DataSet GetDataSetSql(string sqlText)
    {


        SqlDataAdapter SqlAdapter = new SqlDataAdapter();
        DataSet DS = new DataSet();
        SqlConnection SqlConn = CreateSQLConnection();
        SqlConn.Open();
        try
        {
            SqlAdapter = new SqlDataAdapter(sqlText, SqlConn);
            SqlAdapter.SelectCommand.CommandTimeout = 0;
            DS = new DataSet();
            SqlAdapter.Fill(DS);
            return DS;
        }
        catch
        {
            throw;
        }
        finally
        {
            DS.Dispose();
            SqlAdapter.Dispose();
            SqlConn.Dispose();
            SqlConn.Close();
        }
    }
    public DataSet GetDataSet(string strStoredProc)
    {


        DataSet ds = new DataSet();
        SqlConnection oConnection = CreateSQLConnection();
        oConnection.Open();
        try
        {
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;
            ds = new DataSet();
            SQLD.Fill(ds);
            return ds;
        }
        catch
        {
            throw;
        }
        finally
        {
            ds.Dispose();
            SQLD.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }
    public DataSet GetDataSet(string strStoredProc, SqlParameter[] sqlParams)
    {
        DataSet ds = new DataSet();
        SqlConnection sqlConnection = null;
        SQLD = null;
        try
        {
            sqlConnection = CreateSQLConnection();
            sqlConnection.Open();
            SQLD = new SqlDataAdapter(strStoredProc, sqlConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }

            ds = new DataSet();
            SQLD.Fill(ds);

            SQLD.SelectCommand.Parameters.Clear();
            return ds;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("GetDataSet " + "\n" + strStoredProc + "\n" + ex.Message);
        }
        finally
        {
            if (ds != null)
                ds.Dispose();
            if (SQLD != null)
                SQLD.Dispose();
            if (sqlConnection != null)
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            sqlConnection.Dispose();
        }
    }

    public DataTable GetDataTable(string strStoredProc, SqlParameter[] sqlParams, [Optional] string DBType)
    {
        SqlConnection oConnection = CreateSQLConnection();
        oConnection.Open();
        DataTable DT = new DataTable();
        try
        {
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }


            SQLD.Fill(DT);

            SQLD.SelectCommand.Parameters.Clear();
            return DT;
        }
        catch(Exception ex)
        {
            throw;
        }
        finally
        {
            DT.Dispose();
            SQLD.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }
    public DataSet GetDataSet(string strStoredProc, SqlParameter[] sqlParams, SqlConnection SQLConnection)
    {
        SqlConnection oConnection = SQLConnection;
        oConnection.Open();
        DataSet ds = new DataSet();
        try
        {
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }

            ds = new DataSet();
            SQLD.Fill(ds);
            return ds;
        }
        catch
        {
            throw;
        }
        finally
        {
            ds.Dispose();
            SQLD.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }

    public SqlDataReader GetDataReader(string sql)
    {
        SqlCommand myCmd = new SqlCommand();
        SqlDataReader SD;
        SqlConnection conn = CreateSQLConnection();
        conn.Open();
        try
        {
            myCmd = new SqlCommand(sql, conn);
            myCmd.CommandTimeout = 0;
            SD = myCmd.ExecuteReader(CommandBehavior.CloseConnection);
            return SD;
        }
        catch
        {
            throw;
        }
        finally
        {
            myCmd.Dispose();
        }
    }



    public SqlDataReader GetDataReader(string sql, DSIT_DataLayer EDEL)
    {
        SqlCommand myCmd = new SqlCommand();
        SqlDataReader SD;
        try
        {
            myCmd = new SqlCommand(sql, EDEL.myGlobalConn, EDEL.myGlobalTrans);
            myCmd.CommandTimeout = 0;
            SD = myCmd.ExecuteReader();
            return SD;
        }
        catch
        {
            throw;
        }
        finally
        {
            myCmd.Dispose();
        }
    }




    public SqlDbType GetSqlDbType(string DataType)
    {
        SqlDbType DtType = new SqlDbType();
        switch (DataType.Trim().ToUpper())
        {
            case "BIGINT":
                DtType = SqlDbType.BigInt;
                break;
            case "TINYINT":
                DtType = SqlDbType.TinyInt;
                break;
            case "SMALLINT":
                DtType = SqlDbType.SmallInt;
                break;
            case "NVARCHAR":
                DtType = SqlDbType.NVarChar;
                break;
            case "VARCHAR":
                DtType = SqlDbType.VarChar;
                break;
            case "CHAR":
                DtType = SqlDbType.Char;
                break;
            case "DATETIME":
                DtType = SqlDbType.DateTime;
                break;
            case "DATE":
                DtType = SqlDbType.Date;
                break;
            case "FLOAT":
                DtType = SqlDbType.Float;
                break;
            case "MONEY":
                DtType = SqlDbType.Money;
                break;
            case "DECIMAL":
                DtType = SqlDbType.Decimal;
                break;
            case "REAL":
                DtType = SqlDbType.Real;
                break;
            case "INT":
                DtType = SqlDbType.Int;
                break;
            case "VARIANT":
                DtType = SqlDbType.Variant;
                break;
            case "IMAGE":
                DtType = SqlDbType.Image;
                break;
            case "NTEXT":
                DtType = SqlDbType.NText;
                break;
            case "TEXT":
                DtType = SqlDbType.Text;
                break;
            default:
                DtType = SqlDbType.VarChar;
                break;
        }
        return DtType;
    }

    public SqlParameter[] GetSqlParameter(string Parameters)
    {
        string[] strPara = Parameters.Split(',');
        SqlParameter[] SQLP = new SqlParameter[strPara.Length];
        for (int i = 0; i < strPara.Length; i++)
        {
            string[] strParameters = strPara[i].ToString().Split('=');
            string[] strParaName = strParameters[0].ToString().Split('@');
            string strParaValue = strParameters[1].ToString();
            if (strParaName[2].ToString() == "0")
            {
                SQLP[i] = new SqlParameter("@" + strParaName[0].ToString().Trim(), GetSqlDbType(strParaName[1].ToString()));
            }
            else
            {
                SQLP[i] = new SqlParameter("@" + strParaName[0].ToString().Trim(), GetSqlDbType(strParaName[1].ToString()), Convert.ToInt32(strParaName[2].ToString()));
            }
            SQLP[i].Value = strParaValue;
        }
        return SQLP;
    }
    public void ExecuteSP(string strStoredProc, SqlParameter[] sqlParams)
    {
        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, oConnection);
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            if (myGlobalConn.State == ConnectionState.Closed)
                oConnection.Open();
            SqlComm.ExecuteNonQuery();
            //ReturnMessage = (string)SqlComm.Parameters[sqlParams.Length - 2].Value;
            ////ReturnCode = Convert.ToInt64(SqlComm.Parameters[ParaCount - 2].Value);
            //ReturnId = Convert.ToInt64(SqlComm.Parameters[sqlParams.Length - 1].Value);
        }
        catch
        {
            throw;
        }
        finally
        {
            if (myGlobalConn.State == ConnectionState.Closed)
            {
                SqlComm.Dispose();
                oConnection.Close();
                oConnection.Dispose();
            }
        }
    }
    public void ExecuteSP(string strStoredProc, SqlParameter[] sqlParams, out string ReturnMessage, out Int64 ReturnId)
    {
        ReturnMessage = string.Empty;
        ReturnId = 0;
        string STR = string.Empty;
        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, oConnection);
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams)
            {
                SqlComm.Parameters.Add(sqlParam);
                STR += "'" + sqlParam.Value + "',";
            }
            if (myGlobalConn.State == ConnectionState.Closed)
                oConnection.Open();
            SqlComm.ExecuteNonQuery();

            if (((string)SqlComm.Parameters["@ReturnMessage"].Value) == null)
            {
                ReturnMessage = "ERROR PRCESSING RECORD";
            }
            else
            {
                ReturnMessage = (string)SqlComm.Parameters["@ReturnMessage"].Value;
            }

            if (SqlComm.Parameters.Contains("@ReturnId") == true)
            {
                if ((SqlComm.Parameters["@ReturnId"].Value) == null)
                {
                    ReturnId = 0;
                }
                else
                {
                    ReturnId = Convert.ToInt64(SqlComm.Parameters["@ReturnId"].Value);
                }
            }
            else
                ReturnId = 0;

        }

        catch (Exception ex)
        {
            ReturnMessage = ex.Message.Replace("'", "\"");
        }
        finally
        {
            if (myGlobalConn.State == ConnectionState.Closed)
            {
                SqlComm.Dispose();
                oConnection.Close();
                oConnection.Dispose();
            }
        }
    }
    public void ExecuteSP(string strStoredProc, SqlParameter[] sqlParams, out string ReturnMessage, out int ReturnCode)
    {
        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, oConnection);
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        SqlComm.CommandType = CommandType.StoredProcedure;
        //*//SqlComm.CommandTimeout = 0;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            if (myGlobalConn.State == ConnectionState.Closed)
                oConnection.Open();
            SqlComm.ExecuteNonQuery();
            ReturnMessage = (string)SqlComm.Parameters[sqlParams.Length - 2].Value;
            ReturnCode = Convert.ToInt16(SqlComm.Parameters[sqlParams.Length - 1].Value);
        }
        catch
        {
            throw;
        }
        finally
        {
            if (myGlobalConn.State == ConnectionState.Closed)
            {
                SqlComm.Dispose();
                oConnection.Close();
                oConnection.Dispose();
            }
        }
    }



    public enum DataProvider
    {
        Oracle, SqlServer, OleDb, Odbc
    }

    public void DataOpenTran()
    {
        if (myGlobalConn.State == ConnectionState.Open)
            myGlobalConn.Close();

        myGlobalConn.Open();
    }

    public void DataBeginTran()
    {

        if (myGlobalConn.State == ConnectionState.Closed)
            DataOpenTran();

        if (myGlobalConn.State == ConnectionState.Open)
            myGlobalTrans = myGlobalConn.BeginTransaction();
    }

    public void DataCommitTran()
    {
        if (myGlobalConn.State == ConnectionState.Open)
        {
            myGlobalTrans.Commit();
            DataCloseTran();
        }
    }

    public void DataRollback()
    {
        if (myGlobalConn.State == ConnectionState.Open)
        {
            myGlobalTrans.Rollback();
            DataCloseTran();
        }
    }

    public void DataCloseTran()
    {
        if (myGlobalConn.State == ConnectionState.Open)
            myGlobalConn.Close();
    }

    public void DeleteRecord(string sql)
    {

        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(sql, oConnection);
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(sql, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        if (myGlobalConn.State == ConnectionState.Closed)
            oConnection.Open();

        //SqlConnection oConnection = CreateSQLConnection();
        //SqlComm = new SqlCommand(sql, oConnection);
        //oConnection.Open();
        try
        {
            SqlComm.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            SqlComm.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }
    public void ExcuteNonSQL(string sql, [Optional] string DBType)
    {
        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(sql, oConnection);
        //*//SqlComm.CommandTimeout = 0;
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(sql, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        if (myGlobalConn.State == ConnectionState.Closed)
            oConnection.Open();

        //SqlConnection oConnection = CreateSQLConnection();
        //SqlComm = new SqlCommand(sql, oConnection);
        //oConnection.Open();
        try
        {
            SqlComm.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            SqlComm.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }

    public void DatabaseBackup(string Path, out string FilePath)
    {
        FilePath = Path + @"\" + System.Configuration.ConfigurationManager.AppSettings["DatabaseName"].ToString() + ".bak";
        string FilePath_Archive = Path + @"\" + System.Configuration.ConfigurationManager.AppSettings["DatabaseName_Archive"].ToString() + ".bak";

        if (File.Exists(FilePath) == true)
        {
            File.Delete(FilePath);
        }

        if (File.Exists(FilePath_Archive) == true)
        {
            File.Delete(FilePath_Archive);
        }

        string Query = "BACKUP DATABASE [" + System.Configuration.ConfigurationManager.AppSettings["DatabaseName"].ToString() + "] TO DISK='" + FilePath + "'";
        string Query_Archive = "BACKUP DATABASE [" + System.Configuration.ConfigurationManager.AppSettings["DatabaseName_Archive"].ToString() + "] TO DISK='" + FilePath_Archive + "'";
        try
        {
            SqlConnection con;
            if (System.Configuration.ConfigurationManager.AppSettings["DBType"] == "0")
            {
                string decryptConnString = string.Empty;
                decryptConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                decryptConnString = clsCryptography.Decrypt(decryptConnString);
                decryptConnString = decryptConnString.Replace("\\", @"\");
                con = new SqlConnection(decryptConnString);
            }
            else
            {
                string decryptConnString = string.Empty;
                decryptConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString;
                decryptConnString = clsCryptography.Decrypt(decryptConnString);
                decryptConnString = decryptConnString.Replace("\\", @"\");
                con = new SqlConnection(decryptConnString);
            }
            SqlCommand cmdSQL = new SqlCommand(Query, con);
            SqlCommand cmdSQL1 = new SqlCommand(Query_Archive, con);
            cmdSQL.CommandTimeout = 1500;
            cmdSQL1.CommandTimeout = 1500;
            con.Open();
            try
            {
                cmdSQL1.ExecuteNonQuery();
                cmdSQL.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                cmdSQL.Dispose();
                con = null;
            }
        }
        catch
        {
            throw;
        }
    }
    public DataSet ExecuteSql(string sql)
    {
        DataSet ds = new DataSet();
        SqlConnection oConnection = CreateSQLConnection();
        oConnection.Open();
        try
        {
            SQLD = new SqlDataAdapter(sql, oConnection);
            //*//SQLD.SelectCommand.CommandTimeout = 0;
            ds = new DataSet();
            SQLD.Fill(ds);
            return ds;
        }
        catch
        {
            throw;
        }
        finally
        {
            ds.Dispose();
            SQLD.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }
    public string ExecuteScalar(string strProcedure, SqlParameter[] sqlParams)
    {
        string stringValue = string.Empty;
        SqlConnection oConnection = CreateSQLConnection();

        using (SqlCommand cmd = new SqlCommand(strProcedure, oConnection))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter sqlParam in sqlParams) { cmd.Parameters.Add(sqlParam); }



            oConnection.Open();
            object o = cmd.ExecuteScalar();
            if (o != null)
            {
                stringValue = o.ToString();
            }
            oConnection.Dispose();
            oConnection.Close();
            return stringValue;
        }

    }
    public int GetMax()
    {
        string sql;
        DataSet ds;
        int id;
        ds = new DataSet();
        sql = "SELECT Max(UserId) FROM UserMaster";
        ds = ExecuteSql(sql);
        id = int.Parse(ds.Tables[0].Rows[0][0].ToString());
        return id;
    }
    public DataSet getUserData(int UserId)
    {
        DataSet ds = new DataSet();
        string sql;
        sql = "EXECUTE GetUserData " + UserId;
        ds = ExecuteSql(sql);
        return ds;
    }


    public DataTable GetDataTableSql(string sql, [Optional] string DBType)
    {
        SqlDataAdapter SqlAdapter = new SqlDataAdapter();

        DataTable DT = new DataTable();
        SqlConnection SqlConn = CreateSQLConnection();
        SqlConn.Open();
        try
        {
            SqlAdapter = new SqlDataAdapter(sql, SqlConn);
            SqlAdapter.SelectCommand.CommandTimeout = 0;
            SqlAdapter.Fill(DT);
            return DT;
        }
        catch
        {
            throw;
        }
        finally
        {
            DT.Dispose();

            SqlAdapter.Dispose();
            SqlConn.Dispose();
            SqlConn.Close();
        }
    }

    public DataTable GetDataTable(string strStoredProc)
    {
        DataSet ds = new DataSet();
        SqlConnection oConnection = CreateSQLConnection();
        oConnection.Open();
        try
        {
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;
            ds = new DataSet();
            SQLD.Fill(ds);
            return ds.Tables[0];
        }
        catch
        {
            throw;
        }
        finally
        {
            ds.Dispose();
            SQLD.Dispose();
            oConnection.Dispose();
            oConnection.Close();
        }
    }


    public string[] ExecuteSPWithOutPut(string strStoredProc, int intOutPutStart, int intOutPutLenght, SqlParameter[] sqlParams)
    {
        int j = 0;
        string[] ReturnValues = new string[intOutPutLenght - intOutPutStart + 1];
        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, oConnection);
        //*//SqlComm.CommandTimeout = 0;
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
        }
        SqlComm.CommandType = CommandType.StoredProcedure;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            if (myGlobalConn.State == ConnectionState.Closed)
                oConnection.Open();
            SqlComm.ExecuteNonQuery();


            for (int i = intOutPutStart; i <= intOutPutLenght; i++)
            {
                if ((string)SqlComm.Parameters[i].Value.ToString() == "")
                {
                    ReturnValues[j] = "";
                }
                else
                {
                    ReturnValues[j] = (string)SqlComm.Parameters[i].Value;
                }
                j = j + 1;
            }
            return ReturnValues;

        }
        catch
        {
            throw;
        }
        finally
        {
            if (myGlobalConn.State == ConnectionState.Closed)
            {
                SqlComm.Dispose();
                oConnection.Close();
                oConnection.Dispose();
            }
        }
    }

    public void ExecuteReturnValue(string strStoredProc, SqlParameter[] sqlParams, int ParaCount, out decimal ReturnValue)
    {

        SqlConnection oConnection = CreateSQLConnection();
        SqlComm = new SqlCommand(strStoredProc, oConnection);
        //*//SqlComm.CommandTimeout = 0;
        if (myGlobalConn.State == ConnectionState.Open)
        {
            SqlComm = new SqlCommand(strStoredProc, myGlobalConn, myGlobalTrans);
            //SqlComm.Transaction = myGlobalTrans;
        }
        SqlComm.CommandType = CommandType.StoredProcedure;

        //SqlConnection oConnection = CreateSQLConnection();
        //SqlComm = new SqlCommand(strStoredProc, oConnection);
        //SqlComm.CommandType = CommandType.StoredProcedure;
        try
        {
            foreach (SqlParameter sqlParam in sqlParams) { SqlComm.Parameters.Add(sqlParam); }
            if (myGlobalConn.State == ConnectionState.Closed)
                oConnection.Open();
            SqlComm.ExecuteNonQuery();
            ReturnValue = Convert.ToDecimal(SqlComm.Parameters[ParaCount - 1].Value);
        }
        catch
        {
            throw;
        }
        finally
        {
            if (myGlobalConn.State == ConnectionState.Closed)
            {
                SqlComm.Dispose();
                oConnection.Close();
                oConnection.Dispose();
            }
        }
    }

    public SqlParameter[] GetSQLParameterFromProcedure(string StoreProcedureName, string[] strParaValue)
    {

        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        dSet = GetDataSetSql(Query);
        ParaCount = (int)dSet.Tables[0].Rows.Count;
        SqlParameter[] SQLPM = new SqlParameter[ParaCount];
        if ((int)dSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ParaCount; i++)
            {
                ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                if ((ParaSize > 0) && (ParaPrecision == 0))
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType), ParaSize);
                }
                else
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType));
                }
                if (ParaOut == true)
                {
                    SQLPM[i].Direction = ParameterDirection.Output;
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
                else
                {

                    SQLPM[i].Value = strParaValue[i];
                }
            }
        }

        return SQLPM;
    }

    public SqlParameter[] GetSQLParameterFromProcedure(string StoreProcedureName)
    {


        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        dSet = GetDataSetSql(Query);
        ParaCount = (int)dSet.Tables[0].Rows.Count;
        SqlParameter[] SQLPM = new SqlParameter[ParaCount];
        if ((int)dSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ParaCount; i++)
            {
                ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                if ((ParaSize > 0) && (ParaPrecision == 0))
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType), ParaSize);
                }
                else
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType));
                }
                if (ParaOut == true)
                {
                    SQLPM[i].Direction = ParameterDirection.Output;
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
                else
                {
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
            }
        }

        return SQLPM;
    }

    public SqlParameter[] GetSQLParameterFromProcedure(string StoreProcedureName, Control ActivePage)
    {


        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        dSet = GetDataSetSql(Query);
        ParaCount = (int)dSet.Tables[0].Rows.Count;
        SqlParameter[] SQLPM = new SqlParameter[ParaCount];
        if ((int)dSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ParaCount; i++)
            {
                ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                if ((ParaSize > 0) && (ParaPrecision == 0))
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType), ParaSize);
                }
                else
                {
                    SQLPM[i] = new SqlParameter(ParaName, GetSqlDbType(ParaType));
                }
                if (ParaOut == true)
                {
                    SQLPM[i].Direction = ParameterDirection.Output;
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
                else
                {
                    if (ParaName.Substring(1, 3).ToLower() == "sss")
                    {
                        SQLPM[i].Value = System.Web.HttpContext.Current.Session[ParaName.Substring(4)];
                    }
                    else if (ParaName.Substring(1, 3).ToLower() == "qsr")
                    {
                        SQLPM[i].Value = ((Page)ActivePage).Request.QueryString[ParaName.Substring(4)];
                    }
                    else
                    {
                        object ctrl = objGeneralFunction.FindAllControl(ActivePage, ParaName.Replace("@", ""));
                        if (ctrl != null)
                        {
                            if (ctrl.GetType() == typeof(HiddenField))
                                SQLPM[i].Value = GetObjectData(ParaType, ((HiddenField)ctrl).Value.Trim());
                            if (ctrl.GetType() == typeof(Label))
                                SQLPM[i].Value = GetObjectData(ParaType, ((Label)ctrl).Text.Trim());
                            if (ctrl.GetType() == typeof(TextBox))
                                SQLPM[i].Value = GetObjectData(ParaType, ((TextBox)ctrl).Text.Trim());
                            if (ctrl.GetType() == typeof(DropDownList))
                                SQLPM[i].Value = GetObjectData(ParaType, ((DropDownList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(CheckBox))
                                SQLPM[i].Value = GetObjectData(ParaType, ((CheckBox)ctrl).Checked);
                            if (ctrl.GetType() == typeof(CheckBoxList))
                                SQLPM[i].Value = GetObjectData(ParaType, ((CheckBoxList)ctrl).SelectedIndex);
                            if (ctrl.GetType() == typeof(RadioButton))
                                SQLPM[i].Value = GetObjectData(ParaType, ((RadioButton)ctrl).Checked);
                            if (ctrl.GetType() == typeof(RadioButtonList))
                                SQLPM[i].Value = GetObjectData(ParaType, ((RadioButtonList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(ListBox))
                                SQLPM[i].Value = GetObjectData(ParaType, ((ListBox)ctrl).SelectedValue);
                        }
                    }
                }
            }
        }

        return SQLPM;
    }

    public object GetObjectData(string DataType, object DataValue)
    {
        object PassedDataValue = DataValue;
        object ConvertDataValue = "";
        switch (DataType.Trim().ToUpper())
        {
            case "BIGINT":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if ((PassedDataValue.ToString().Trim() == string.Empty) || (PassedDataValue.ToString().Trim() == "&nbsp;"))
                    PassedDataValue = 0;

                if ((PassedDataValue.ToString().ToLower() == "false") || (PassedDataValue.ToString().ToLower() == "true"))
                    ConvertDataValue = Convert.ToInt64(PassedDataValue);
                else
                {
                    PassedDataValue = Convert.ToInt64(PassedDataValue.ToString().Replace(".000", "").ToString().Replace(".00", "").ToString().Replace(".0", ""));
                    if (PassedDataValue.ToString() == "")
                        PassedDataValue = 0;

                    ConvertDataValue = Convert.ToInt64(PassedDataValue);
                }
                break;
            case "TINYINT":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if ((PassedDataValue.ToString().Trim() == string.Empty) || (PassedDataValue.ToString().Trim() == "&nbsp;"))
                    PassedDataValue = 0;

                if ((PassedDataValue.ToString().ToLower() == "false") || (PassedDataValue.ToString().ToLower() == "true"))
                    ConvertDataValue = Convert.ToInt16(PassedDataValue);
                else
                {
                    PassedDataValue = Convert.ToInt16(PassedDataValue.ToString().Replace(".000", "").ToString().Replace(".00", "").ToString().Replace(".0", ""));
                    if (PassedDataValue.ToString() == "")
                        PassedDataValue = 0;

                    ConvertDataValue = Convert.ToInt16(PassedDataValue);
                }
                break;
            case "SMALLINT":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if ((PassedDataValue.ToString().Trim() == string.Empty) || (PassedDataValue.ToString().Trim() == "&nbsp;"))
                    PassedDataValue = 0;

                if ((PassedDataValue.ToString().ToLower() == "false") || (PassedDataValue.ToString().ToLower() == "true"))
                    ConvertDataValue = Convert.ToInt32(PassedDataValue);
                else
                {
                    PassedDataValue = Convert.ToInt16(PassedDataValue.ToString().Replace(".000", "").ToString().Replace(".00", "").ToString().Replace(".0", ""));
                    if (PassedDataValue.ToString() == "")
                        PassedDataValue = 0;

                    ConvertDataValue = Convert.ToInt32(PassedDataValue);
                }
                break;
            case "FLOAT":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = 0;

                ConvertDataValue = Convert.ToDecimal(PassedDataValue);
                break;
            case "MONEY":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = 0;

                ConvertDataValue = Convert.ToDecimal(PassedDataValue);
                break;
            case "DECIMAL":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = 0;

                ConvertDataValue = Convert.ToDecimal(PassedDataValue);
                break;
            case "DOUBLE":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = 0;

                ConvertDataValue = Convert.ToDouble(PassedDataValue);
                break;
            case "REAL":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = 0;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = 0;

                ConvertDataValue = Convert.ToDouble(PassedDataValue);
                break;
            case "INT":
                if (PassedDataValue == null)
                    PassedDataValue = 0;
                if ((PassedDataValue.ToString().Trim() == string.Empty) || (PassedDataValue.ToString().Trim() == "&nbsp;"))
                    PassedDataValue = 0;

                if ((PassedDataValue.ToString().ToLower() == "false") || (PassedDataValue.ToString().ToLower() == "true"))
                    ConvertDataValue = Convert.ToInt32(PassedDataValue);
                else
                {
                    PassedDataValue = Convert.ToInt32(PassedDataValue.ToString().Replace(".000", "").ToString().Replace(".00", "").ToString().Replace(".0", ""));
                    if (PassedDataValue.ToString() == "")
                        PassedDataValue = 0;

                    ConvertDataValue = Convert.ToInt32(PassedDataValue);
                }
                break;
            case "NVARCHAR":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            case "VARCHAR":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            case "CHAR":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            case "DATETIME":
                if (PassedDataValue == null)
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy h:mm:ss tt");
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy h:mm:ss tt"); ;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy h:mm:ss tt"); ;

                ConvertDataValue = PassedDataValue;
                break;
            case "DATE":
                if (PassedDataValue == null)
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy");
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy"); ;
                if (PassedDataValue.ToString() == "")
                    PassedDataValue = DateTime.Now.ToString("dd-MMM-yyyy"); ;

                ConvertDataValue = PassedDataValue;
                break;
            case "VARIANT":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            case "IMAGE":
                ConvertDataValue = PassedDataValue;
                break;
            case "TEXT":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            case "NTEXT":
                if (PassedDataValue == null)
                    PassedDataValue = "";
                if (PassedDataValue.ToString() == "&nbsp;")
                    PassedDataValue = "";

                ConvertDataValue = PassedDataValue;
                break;
            default:
                ConvertDataValue = PassedDataValue;
                break;
        }
        return ConvertDataValue;
    }

    public Int64 ExecuteUpdateData(DSIT_DataLayer EDEL, Control ActivePage, bool promptMessage, string StoreProcedureName, string PageMode, ref string ReturnMessage)
    {
        Int64 returnid = 0;

        string returnmess = "";


        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        string strTemp = string.Empty;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        dSet = EDEL.GetDataSetSql(Query);
        ParaCount = (int)dSet.Tables[0].Rows.Count;
        SqlParameter[] SQLPM = new SqlParameter[ParaCount];
        if ((int)dSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ParaCount; i++)
            {
                ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                if ((ParaSize > 0) && (ParaPrecision == 0))
                {
                    SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType), ParaSize);
                }
                else
                {
                    SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType));
                }
                if (ParaOut == true)
                {
                    SQLPM[i].Direction = ParameterDirection.Output;
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
                else
                {
                    if (ParaName.Substring(1, 3).ToLower() == "sss")
                    {
                        //SQLPM[i].Value = System.Web.HttpContext.Current.Session[ParaName.Substring(4)];
                        SQLPM[i].Value = System.Web.HttpContext.Current.Request.Cookies["Userinfo"][ParaName.Substring(4)];
                    }
                    else if (ParaName.Substring(1, 3).ToLower() == "qsr")
                    {
                        SQLPM[i].Value = ((Page)ActivePage).Request.QueryString[ParaName.Substring(4)];
                    }
                    else
                    {
                        object ctrl = objGeneralFunction.FindAllControl(ActivePage, ParaName.Replace("@", ""));
                        if (ctrl != null)
                        {
                            if (ctrl.GetType() == typeof(HiddenField))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((HiddenField)ctrl).Value.Trim());
                            if (ctrl.GetType() == typeof(Label))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((Label)ctrl).Text.Trim());
                            if (ctrl.GetType() == typeof(TextBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((TextBox)ctrl).Text.Trim());
                            if (ctrl.GetType() == typeof(DropDownList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((DropDownList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(CheckBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((CheckBox)ctrl).Checked);
                            if (ctrl.GetType() == typeof(CheckBoxList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((CheckBoxList)ctrl).SelectedIndex);
                            if (ctrl.GetType() == typeof(RadioButton))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((RadioButton)ctrl).Checked);
                            if (ctrl.GetType() == typeof(RadioButtonList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((RadioButtonList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(ListBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((ListBox)ctrl).SelectedValue);
                        }
                    }
                }

                if (strTemp != string.Empty)
                {
                    strTemp = strTemp + ",";

                }
                strTemp = strTemp + "'" + SQLPM[i].Value.ToString() + "'";


            }
        }
        try
        {
            EDEL.ExecuteSP(StoreProcedureName, SQLPM, out returnmess, out returnid);
            ReturnMessage = returnmess;
            if (returnid == 0)
            {
                if (promptMessage == true)
                    objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
                return returnid;
            }
            else
            {
                if (promptMessage == true)
                    if (returnmess.Trim() != "")
                        objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
            }
        }
        catch (Exception ee)
        {
            if (promptMessage == true)
                objGeneralFunction.AlertUser(objGeneralFunction.ReplaceASC(ee.Message.ToString()), (Page)ActivePage);

            ReturnMessage = objGeneralFunction.ReplaceASC(ee.Message.ToString());
            returnid = 0;
            return returnid;
        }
        finally
        {
        }

        return returnid;
    }

    public Int64 ExecuteUpdateDataSet(DSIT_DataLayer EDEL, Control ActivePage, bool promptMessage, string StoreProcedureName, string PageMode, Int64 ReturnKeyId, DataSet DSset)
    {
        Int64 returnid = 0;
        string returnmess = "";


        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        try
        {

            dSet = EDEL.GetDataSetSql(Query);
            ParaCount = (int)dSet.Tables[0].Rows.Count;
            SqlParameter[] SQLPM = new SqlParameter[ParaCount];
            if ((int)dSet.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < DSset.Tables[0].Rows.Count; j++)
                {
                    for (int i = 0; i < ParaCount; i++)
                    {
                        ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                        ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                        ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                        ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                        ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                        if ((ParaSize > 0) && (ParaPrecision == 0))
                        {
                            SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType), ParaSize);
                        }
                        else
                        {
                            SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType));
                        }
                        if (ParaOut == true)
                        {
                            SQLPM[i].Direction = ParameterDirection.Output;
                            if (ParaPrecision == 0)
                                SQLPM[i].Value = "";
                            else
                                SQLPM[i].Value = 0;
                        }
                        else
                        {
                            ParaName = SQLPM[i].ParameterName.ToString();

                            if (ParaName.Substring(1, 3).ToLower() == "sss")
                            {
                                //SQLPM[i].Value = System.Web.HttpContext.Current.Session[ParaName.Substring(4)];
                                SQLPM[i].Value = System.Web.HttpContext.Current.Request.Cookies["Userinfo"][ParaName.Substring(4)];
                            }
                            else if (ParaName.Substring(1, 3).ToLower() == "qsr")
                            {
                                SQLPM[i].Value = ((Page)ActivePage).Request.QueryString[ParaName.Substring(4)];
                            }
                            else if (ParaName.Substring(1, 3).ToLower() == "hdn")
                            {
                                object ctrl = objGeneralFunction.FindAllControl(ActivePage, ParaName.Replace("@", ""));
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((HiddenField)ctrl).Value);
                            }
                            else
                            {
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, DSset.Tables[0].Rows[j][ParaName.Substring(4)]);
                            }
                        }
                    }
                    EDEL.ExecuteSP(StoreProcedureName, SQLPM, out returnmess, out returnid);
                    if (returnid == 0)
                    {
                        if (promptMessage == true)
                            objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
                        return returnid;
                    }
                    else
                    {
                        if (promptMessage == true)
                            if (returnmess.Trim() != "")
                                objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
                    }
                }
            }
        }
        catch (Exception ee)
        {
            if (promptMessage == true)
                objGeneralFunction.AlertUser(objGeneralFunction.ReplaceASC(ee.Message.ToString()), (Page)ActivePage);
            returnid = 0;
            return returnid;
        }
        finally
        {
        }
        return returnid;
    }

    public Int64 ExecuteUpdateDataRow(DSIT_DataLayer EDEL, Control ActivePage, bool promptMessage, string StoreProcedureName, string PageMode, Int64 ReturnKeyId, DataRow DSrow)
    {
        Int64 returnid = 0;
        string returnmess = "";


        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        try
        {
            dSet = EDEL.GetDataSetSql(Query);
            ParaCount = (int)dSet.Tables[0].Rows.Count;
            SqlParameter[] SQLPM = new SqlParameter[ParaCount];
            if ((int)dSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ParaCount; i++)
                {
                    ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                    ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                    ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                    ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                    ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                    if ((ParaSize > 0) && (ParaPrecision == 0))
                    {
                        SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType), ParaSize);
                    }
                    else
                    {
                        SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType));
                    }
                    if (ParaOut == true)
                    {
                        SQLPM[i].Direction = ParameterDirection.Output;
                        if (ParaPrecision == 0)
                            SQLPM[i].Value = "";
                        else
                            SQLPM[i].Value = 0;
                    }
                    else
                    {
                        ParaName = SQLPM[i].ParameterName.ToString();

                        if (ParaName.Substring(1, 3).ToLower() == "sss")
                        {
                            SQLPM[i].Value = System.Web.HttpContext.Current.Session[ParaName.Substring(4)];
                        }
                        else if (ParaName.Substring(1, 3).ToLower() == "qsr")
                        {
                            SQLPM[i].Value = ((Page)ActivePage).Request.QueryString[ParaName.Substring(4)];
                        }
                        else if (ParaName.Substring(1, 3).ToLower() == "hdn")
                        {
                            object ctrl = objGeneralFunction.FindAllControl(ActivePage, ParaName.Replace("@", ""));
                            SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((HiddenField)ctrl).Value);
                        }
                        else
                        {
                            SQLPM[i].Value = EDEL.GetObjectData(ParaType, DSrow[ParaName.Substring(4)]);
                        }
                    }
                }
                EDEL.ExecuteSP(StoreProcedureName, SQLPM, out returnmess, out returnid);
                if (returnid == 0)
                {
                    if (promptMessage == true)
                        objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
                    return returnid;
                }
                else
                {
                    if (promptMessage == true)
                        if (returnmess.Trim() != "")
                            objGeneralFunction.AlertUser(returnmess, (Page)ActivePage);
                }
            }
        }
        catch (Exception ee)
        {
            if (promptMessage == true)
                objGeneralFunction.AlertUser(objGeneralFunction.ReplaceASC(ee.Message.ToString()), (Page)ActivePage);
            returnid = 0;
            return returnid;
        }
        finally
        {
        }
        return returnid;
    }

    public string ExecuteDeleteData(string strDeleteString)
    {

        //App_General_Delete#RecordKeyId|@Col1|T|15|I,TableName|UserGroup|T|100|I,ReturnId|0|BI|0|O,,ReturnMessage|0|T|100|O

        if (strDeleteString.Contains("#") == false)
            return "DELETE PROCEDURE NOT CONFIGURED";


        string StoreProcedureName = strDeleteString.Split('#')[0];

        var parameters = new List<SqlParameter>();

        string[] strParameters = strDeleteString.Split('#')[1].Split(',');
        string[] strParameter;

        for (int i = 0; i < strParameters.Length; i++)
        {
            strParameter = strParameters[i].Split('|');
            parameters.Add(objGeneralFunction.GetSqlParameter("@" + strParameter[0], strParameter[1].Replace("@", ","), GetSQLDataType(strParameter[2]), Convert.ToInt32(strParameter[3]), GetSQLParameterDirection(strParameter[4])));
        }

        string ReturnMessage = string.Empty;
        Int64 RecordId = 0;
        ExecuteSP(StoreProcedureName, parameters.ToArray(), out ReturnMessage, out RecordId);

        return ReturnMessage;

    }

    public SqlDbType GetSQLDataType(string strSqlDbType)
    {
        if (strSqlDbType == "T")
            return SqlDbType.NVarChar;
        else if (strSqlDbType == "BI")
            return SqlDbType.BigInt;
        else if (strSqlDbType == "D")
            return SqlDbType.Decimal;
        else
            return SqlDbType.NVarChar;
    }

    public ParameterDirection GetSQLParameterDirection(string strSqlParameterDirection)
    {
        if (strSqlParameterDirection == "I")
            return ParameterDirection.Input;
        else if (strSqlParameterDirection == "O")
            return ParameterDirection.Output;
        else
            return ParameterDirection.ReturnValue;
    }

    public string GetSQLParameterValue(string Parameter, Control ctrl)
    {
        if (Parameter.Contains("qsr"))
        {
            if (((Page)ctrl).RouteData.Values[Parameter.Replace("qsr", string.Empty)] != null)
                return ((Page)ctrl).RouteData.Values[Parameter.Replace("qsr", string.Empty)].ToString();
        }
        else if (Parameter.Contains("txt"))
        {
            if (((TextBox)ctrl).FindControl(Parameter) != null)
                return ((TextBox)(ctrl.FindControl(Parameter))).Text;
        }
        else if (Parameter.Contains("hdn"))
        {
            if (((HiddenField)ctrl).FindControl(Parameter) != null)
                return ((HiddenField)(ctrl.FindControl(Parameter))).Value;
        }
        else if (Parameter.Contains("sss"))
        {
            return HttpContext.Current.Session[Parameter.Replace("sss", string.Empty)].ToString();
        }

        return string.Empty;
    }

    public DataSet ExecuteReportData(Control ActivePage, string StoreProcedureName)
    {
        int ParaCount = 0;
        string ParaName = "";
        string ParaType = "";
        int ParaSize = 0;
        int ParaPrecision = 0;
        bool ParaOut = false;

        DSIT_DataLayer EDEL = new DSIT_DataLayer();

        DataSet dSet = new DataSet();
        string Query = "Execute SysObjects_ListData '" + StoreProcedureName + "'";
        dSet = EDEL.GetDataSetSql(Query);
        ParaCount = (int)dSet.Tables[0].Rows.Count;
        SqlParameter[] SQLPM = new SqlParameter[ParaCount];
        if ((int)dSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ParaCount; i++)
            {
                ParaName = dSet.Tables[0].Rows[i]["Parameter_Name"].ToString();
                ParaType = dSet.Tables[0].Rows[i]["Parameter_Type"].ToString();
                ParaSize = Convert.ToInt16(dSet.Tables[0].Rows[i]["max_length"].ToString());
                ParaPrecision = Convert.ToInt16(dSet.Tables[0].Rows[i]["precision"].ToString());
                ParaOut = Convert.ToBoolean(dSet.Tables[0].Rows[i]["is_output"].ToString());

                if ((ParaSize > 0) && (ParaPrecision == 0))
                {
                    SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType), ParaSize);
                }
                else
                {
                    SQLPM[i] = new SqlParameter(ParaName, EDEL.GetSqlDbType(ParaType));
                }
                if (ParaOut == true)
                {
                    SQLPM[i].Direction = ParameterDirection.Output;
                    if (ParaPrecision == 0)
                        SQLPM[i].Value = "";
                    else
                        SQLPM[i].Value = 0;
                }
                else
                {
                    if (ParaName.Substring(1, 3).ToLower() == "sss")
                    {
                        SQLPM[i].Value = System.Web.HttpContext.Current.Session[ParaName.Substring(4)];
                    }
                    else if (ParaName.Substring(1, 3).ToLower() == "qsr")
                    {
                        if (((Page)ActivePage).Request.QueryString[ParaName.Substring(4)] != null)
                            SQLPM[i].Value = ((Page)ActivePage).Request.QueryString[ParaName.Substring(4)].ToString().Replace("|", "'").Replace("~", ",").Replace("^", "''");
                        else
                            SQLPM[i].Value = "";
                    }
                    else
                    {
                        object ctrl = objGeneralFunction.FindAllControl(ActivePage, ParaName.Replace("@", ""));
                        if (ctrl != null)
                        {
                            if (ctrl.GetType() == typeof(HiddenField))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((HiddenField)ctrl).Value.Trim().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
                            if (ctrl.GetType() == typeof(Label))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((Label)ctrl).Text.Trim().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
                            if (ctrl.GetType() == typeof(TextBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((TextBox)ctrl).Text.Trim().Replace("|", "'").Replace("~", ",").Replace("^", "''"));
                            if (ctrl.GetType() == typeof(DropDownList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((DropDownList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(CheckBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((CheckBox)ctrl).Checked);
                            if (ctrl.GetType() == typeof(CheckBoxList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((CheckBoxList)ctrl).SelectedIndex);
                            if (ctrl.GetType() == typeof(RadioButton))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((RadioButton)ctrl).Checked);
                            if (ctrl.GetType() == typeof(RadioButtonList))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((RadioButtonList)ctrl).SelectedValue);
                            if (ctrl.GetType() == typeof(ListBox))
                                SQLPM[i].Value = EDEL.GetObjectData(ParaType, ((ListBox)ctrl).SelectedValue);
                        }
                    }
                }
            }
        }
        try
        {
            dSet = GetDataSet(StoreProcedureName, SQLPM);
        }
        catch (Exception ee)
        {
            Console.Write(ee);
            throw;
        }
        finally
        {
        }

        return dSet;
    }

    public void AddNewRow(DataSet DS)
    {
        DataRow drwRow = default(DataRow);
        drwRow = DS.Tables[0].NewRow();
        DS.Tables[0].Rows.Add(drwRow);
    }

    public void FillListControl(string strStoredProc, SqlParameter[] sqlParams, Control ctrl, out DataSet ddDataSet, bool isAttribute = false)
    {


        SqlConnection oConnection = CreateSQLConnection();
        DataSet DS = new DataSet();
        try
        {
            oConnection.Open();
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }

            DS = new DataSet();
            SQLD.Fill(DS);

            if (ctrl.GetType() == typeof(ListBox))
            {
                ((ListBox)ctrl).Items.Clear();

                if (isAttribute == false)
                {
                    ((ListBox)ctrl).DataSource = DS;
                    ((ListBox)ctrl).DataTextField = DS.Tables[0].Columns[1].ColumnName;
                    ((ListBox)ctrl).DataValueField = DS.Tables[0].Columns[0].ColumnName;
                    ((ListBox)ctrl).DataBind();
                }
                else
                {
                    ListItem Item = new ListItem();
                    for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                    {
                        Item = new ListItem();
                        Item.Value = DS.Tables[0].Rows[i][0].ToString();
                        Item.Text = DS.Tables[0].Rows[i][1].ToString();
                        if (DS.Tables[0].Columns.Count > 2)
                            Item.Attributes.Add("dval", DS.Tables[0].Rows[i][2].ToString());
                        ((ListBox)ctrl).Items.Add(Item);
                    }
                }

            }
            if (ctrl.GetType() == typeof(CheckBoxList))
            {
                if (isAttribute == false)
                {
                    ((CheckBoxList)ctrl).Items.Clear();
                    ((CheckBoxList)ctrl).DataSource = DS;
                    ((CheckBoxList)ctrl).DataTextField = DS.Tables[0].Columns[1].ColumnName;
                    ((CheckBoxList)ctrl).DataValueField = DS.Tables[0].Columns[0].ColumnName;
                    ((CheckBoxList)ctrl).DataBind();
                }
                else
                {
                    ListItem Item = new ListItem();
                    for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                    {
                        Item = new ListItem();
                        Item.Value = DS.Tables[0].Rows[i][0].ToString();
                        Item.Text = DS.Tables[0].Rows[i][1].ToString();
                        if (DS.Tables[0].Columns.Count > 2)
                            Item.Attributes.Add("dval", DS.Tables[0].Rows[i][2].ToString());
                        ((CheckBoxList)ctrl).Items.Add(Item);
                    }
                }
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            ddDataSet = DS;
            DS = null;
            oConnection.Close();
            oConnection.Dispose();

        }

    }
    public void FillDropDown(string strStoredProc, DropDownList Ddl, string strMessage)

    {



        Ddl.Items.Clear();



        DataSet DS = new DataSet();
        DSIT_DataLayer EDAL = new DSIT_DataLayer();


        try

        {

            DS = EDAL.GetDataSet(strStoredProc);

            if (strMessage != "")

            {

                DataRow drwRow = DS.Tables[0].NewRow();

                drwRow[DS.Tables[0].Columns[0].ColumnName] = 0;

                drwRow[DS.Tables[0].Columns[1].ColumnName] = "Select " + strMessage;

                DS.Tables[0].Rows.InsertAt(drwRow, 0);

            }

            Ddl.DataSource = DS;

            Ddl.DataTextField = DS.Tables[0].Columns[1].ColumnName;

            Ddl.DataValueField = DS.Tables[0].Columns[0].ColumnName;

            Ddl.DataBind();


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

    public void FillDropDown(string strStoredProc, SqlParameter[] sqlParams, DropDownList Ddl, string strMessage)
    {



        Ddl.Items.Clear();



        DataSet DS = new DataSet();
        DSIT_DataLayer EDAL = new DSIT_DataLayer();


        try

        {

            DS = EDAL.GetDataSet(strStoredProc, sqlParams);

            if (strMessage != "")

            {

                DataRow drwRow = DS.Tables[0].NewRow();

                drwRow[DS.Tables[0].Columns[0].ColumnName] = 0;

                drwRow[DS.Tables[0].Columns[1].ColumnName] = "Select " + strMessage;

                DS.Tables[0].Rows.InsertAt(drwRow, 0);

            }

            Ddl.DataSource = DS;

            Ddl.DataTextField = DS.Tables[0].Columns[1].ColumnName;

            Ddl.DataValueField = DS.Tables[0].Columns[0].ColumnName;

            Ddl.DataBind();


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



    public void FillDropDown(string strStoredProc, SqlParameter[] sqlParams, DropDownList Ddl, string strMessage, out DataSet ddDataSet)
    {
        SqlConnection oConnection = CreateSQLConnection();
        DataSet DS = new DataSet();
        try
        {
            oConnection.Open();
            SQLD = new SqlDataAdapter(strStoredProc, oConnection);
            SQLD.SelectCommand.CommandType = CommandType.StoredProcedure;
            SQLD.SelectCommand.CommandTimeout = 0;

            foreach (SqlParameter sqlParam in sqlParams)
            {
                SQLD.SelectCommand.Parameters.Add(sqlParam);
            }

            DS = new DataSet();
            SQLD.Fill(DS);
            Ddl.Items.Clear();

            if (strMessage != "")
            {
                DataRow drwRow = DS.Tables[0].NewRow();
                drwRow[DS.Tables[0].Columns[0].ColumnName] = 0;
                drwRow[DS.Tables[0].Columns[1].ColumnName] = "Select " + strMessage;
                DS.Tables[0].Rows.InsertAt(drwRow, 0);
            }

            Ddl.DataSource = DS;
            Ddl.DataTextField = DS.Tables[0].Columns[1].ColumnName;
            Ddl.DataValueField = DS.Tables[0].Columns[0].ColumnName;
            Ddl.DataBind();

        }
        catch
        {
            throw;
        }
        finally
        {
            ddDataSet = DS;
            DS = null;
            oConnection.Close();
            oConnection.Dispose();

        }

    }



}



