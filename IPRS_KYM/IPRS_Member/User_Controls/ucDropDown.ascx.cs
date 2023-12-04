using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member.User_Controls
{
    public partial class ucDropDown : System.Web.UI.UserControl
    {
        /// <summary>
        /// Here we declare necessary variable and also crate eventhandler for drop down 
        /// Commented By Rohit
        /// </summary>
        public string ServiceMethod = "Populateddl"; // IN CASE ANY SPECIAL METHOD
        public bool blnChangeEvent; // IF REQUIRED PASS TURE FROM USERCONTROL
        public bool blnRequired = true; // IF REQUIRED PASS FALSE FROM USERCONTROL
        public string strMessage; //DISPLAY PLACE HOLDER VALUE
        public int MinimumPrefixLength = 1;
        public bool blEnabled = true; // IF REQUIRED PASS FALSE FROM USERCONTROL

        public SqlParameter[] sqlParams;
        public event EventHandler<PostButtonClickArg> PostButtonClick;


        /// <summary> For PostButtonClickArg
        /// here we define Evnethandler (PostButtonClickArg) for dropdown selected value and selected text
        /// here we get value from btnPostBack_Click and asgin to SelectedValue and SelectedText
        /// Commented By Rohit
        /// </summary>

        #region "ButtonClick"

        public class PostButtonClickArg : EventArgs
        {
            //public enum ButtonClick { First };

            public string SelectedValue { get; private set; }
            public string SelectedText { get; private set; }

            public Boolean handled = true;
            public PostButtonClickArg(string strSelectedText, string strSelectedValue) : base()
            {
                this.SelectedValue = strSelectedValue;
                this.SelectedText = strSelectedText;
            }
        }

        /// <summary>
        /// here we get values from btnPostBack_Click and assign to PostButtonClickArg 
        /// Commented By Rohit
        /// </summary>
        /// <param name="SelectedText"></param>
        /// <param name="SelectedValue"></param>

        
        protected void ButtonClick(string SelectedText, string SelectedValue)
        {
            PostButtonClickArg e = new PostButtonClickArg(SelectedText, SelectedValue);
            PostButtonClick(this, e);
        }
        #endregion "ButtonClick"

        /// <summary>
        /// In Page Load event here we check requird field are ture or false
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region PostButtonClick
        protected void Page_Load(object sender, EventArgs e)
        {
            AutoCompleteExtender1.ServiceMethod = ServiceMethod;
            if (blnRequired == true)
                txtDropDown.Attributes.Add("required", "required");
            if (strMessage != string.Empty)
                txtDropDown.Attributes.Add("placeholder", "Select " + strMessage);
            if (blEnabled == false)
                txtDropDown.Enabled = blEnabled;
            AutoCompleteExtender1.MinimumPrefixLength = MinimumPrefixLength;
        }
        #endregion


        public string GetSelectedValue()
        {

            return hdnSelectedValue.Value;
        }

        public string GetSelectedText()
        {

            return txtDropDown.Text;
        }

        /// <summary>For PopulateDropDown
        /// here we get data from specific store procedure with using parameter ID and return value in strContextKey(Area_Populate^@GroupId|0|BigInt|0|Input)
        /// Commented By Rohit
        /// </summary>
        /// <param name="strProcedureName"like (Area_Populate)></param>
        /// <param name="parameters" like (@GroupId)></param>
        /// <param name="strMessage"like (Area)></param>
        public void PopulateDropDown(string strProcedureName, List<SqlParameter> parameters, string strMessage)
        {

            string strContextKey = string.Empty;

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (strContextKey != string.Empty)
                        strContextKey = strContextKey + "#";
                    strContextKey = strContextKey + parameters[i].ParameterName.ToString() + "|" + parameters[i].SqlValue.ToString() + "|" + parameters[i].SqlDbType.ToString() + "|" + parameters[i].Size.ToString() + "|" + parameters[i].Direction.ToString();
                }
            }

            strContextKey = strProcedureName + "^" + strContextKey;

            AutoCompleteExtender1.ContextKey = strContextKey;

        }

        /// <summary> For SelectDropDown
        /// Here we assign the value to hdnSelectedValue and txtDropDown
        /// Commented By Rohit
        /// </summary>
        /// <param name="strRecordId"></param>
        /// <param name="strRecordText"></param>
        public void SelectDropDown(string strRecordId, string strRecordText)
        {
            hdnSelectedValue.Value = strRecordId;
            txtDropDown.Text = strRecordText;
        }

        public void Lock(bool blnStatus)
        {
            txtDropDown.ReadOnly = blnStatus;
        }

        /// <summary>
        /// Here we chaeck blnChangeEvent is true or false . if its true means requird pass true from usercontrol
        /// hdnSelectedValue is primary Id (41067) of data and in txtDropDown (DOMBIVALI - 421201)is related data assign from primary Id of data
        /// Commented By Rohit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            if (blnChangeEvent == false)
                return;
            ButtonClick(txtDropDown.Text, hdnSelectedValue.Value);
        }

    }
}
