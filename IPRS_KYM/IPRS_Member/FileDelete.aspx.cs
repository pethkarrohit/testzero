using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IPRS_Member
{
    public partial class FileDelete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod()]
        public static bool DeleteFile_Member(string MemberIds)
        {
            string[] ParaMemberId = MemberIds.Split(',');
            bool Flag = true;
            for (int i = 0; i < ParaMemberId.Length; i++)
            {

                Hashtable HSTFile = new Hashtable();

                string MemberId = ParaMemberId[i];
                if (MemberId != "")
                {
                    HSTFile.Add("MemberPhoto", "MPU_" + MemberId.ToString() + "_");
                    HSTFile.Add("MemberRegDocs", "MRU_" + MemberId.ToString() + "_");
                    HSTFile.Add("MemberRegWorkDocs", "MWN_" + MemberId.ToString() + "_");


                    int FileExistStatus = 0;
                    try
                    {

                        foreach (DictionaryEntry item in HSTFile)
                        {


                            var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath(item.Key.ToString()), "*.*")
                                        let x = new FileInfo(o)
                                        where x.FullName.ToUpper().Contains(item.Value.ToString().ToUpper())
                                        select o;

                            foreach (var Queryitem in query)
                            {
                                FileExistStatus = 1;
                                File.Delete(Queryitem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Flag = false;

                    }
                }
            }
            return Flag;
        }
    }



}