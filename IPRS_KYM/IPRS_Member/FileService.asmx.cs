using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace IPRS_Member
{
    /// <summary>
    /// Summary description for FileService
    /// </summary>
    [WebService(Namespace = "http://membership.iprs.org/Fileservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FileService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public void GetMemberDocs(string MemberId)
        {
            byte[] fileContent = null;

            Hashtable HSTFile = new Hashtable();


            if (MemberId != "")
            {

                int FileExistStatus = 0;
                try
                {

                    var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/MemberRegDocs"), "*.*")
                                let x = new FileInfo(o)
                                where x.Name.Contains("MRU_"+ MemberId.ToString().ToUpper()+"_")
                                select o;

                    foreach (var Queryitem in query)
                    {
                        FileExistStatus = 1;
                        using (FileStream fs = File.OpenRead(Queryitem))
                        {
                            var binaryReader = new BinaryReader(fs);
                            fileContent = binaryReader.ReadBytes((int)fs.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileExistStatus = 0;

                }
            }
            if (fileContent != null)
            {
                //HttpContext.Current.Response.ContentType = "image/jpeg";
                HttpContext.Current.Response.BinaryWrite(fileContent);
            }
            else
            { HttpContext.Current.Response.Write("No Image To Display"); }
        }
    }
}
