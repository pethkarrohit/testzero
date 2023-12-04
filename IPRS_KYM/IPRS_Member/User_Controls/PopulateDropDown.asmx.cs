using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace IPRS_Member.User_Controls
{
    /// <summary>
    /// Summary description for PopulateDropDown
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class PopulateDropDown : System.Web.Services.WebService
    {

        [System.Web.Services.WebMethod]
        ////[System.Web.Script.Services.ScriptMethod()]
        /// <param name="contextKey"></param>
        ///In this function we get  geographical area value from its area pincode like (ANDIPALAYAM - 642120) and return in ouput 
        /// we pass dropdown value to prefixText (642120) and Store procedure and its prameter ID value in contextKey (Area_Populate^@GroupId|0|BigInt|0|Input) 
        /// and in output ("{""First"":""AGRAHARA KANNADIPUTHUR - 642111"",""Second"":""1953""}")
        ///Commented By Rohit
        public List<string> Populateddl(string prefixText, string contextKey)
        {
            DataTable Result = new DataTable();
            DataSet myDataSet = new DataSet();
            string[] strMaiString = contextKey.Split('^');
            GeneralFunction objGeneralFunction = new GeneralFunction();

            string[] strParameters = strMaiString[1].Split('#');
            string[] strParametersValue;
            var parameters = new List<SqlParameter>();
            for (int i = 0; i < strParameters.Length; i++)
            {
                strParametersValue = strParameters[i].Split('|');
                parameters.Add(objGeneralFunction.GetSqlParameter(strParametersValue[0], strParametersValue[1], (SqlDbType)Enum.Parse(typeof(SqlDbType), strParametersValue[2], true), Convert.ToInt32(strParametersValue[3]), (ParameterDirection)Enum.Parse(typeof(ParameterDirection), strParametersValue[4], true)));

            }
            parameters.Add(objGeneralFunction.GetSqlParameter("@prefixText", prefixText, SqlDbType.NVarChar, 100, ParameterDirection.Input));
            //SqlDbType type = (SqlDbType)Enum.Parse(typeof(SqlDbType), "bit", true);

            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            myDataSet = objDAL.GetDataSet(strMaiString[0], parameters.ToArray());


            List<string> Output = new List<string>();

            if (myDataSet == null)
            {
                Output.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem("No Records Found", "0"));

            }


            if (myDataSet.Tables[0].Rows.Count == 0)
            {
                Output.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem("No Records Found", "0"));
                return Output;
            }

            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
            {
                Output.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(myDataSet.Tables[0].Rows[i][1].ToString().ToString(), myDataSet.Tables[0].Rows[i][0].ToString()));

            }
            return Output;
        }
    }
}
