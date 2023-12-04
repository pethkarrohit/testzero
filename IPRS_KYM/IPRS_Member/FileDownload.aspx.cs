using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;


namespace IPRS_Member
{
    public partial class FileDownload : System.Web.UI.Page
    {
        GeneralFunction objGeneralFunction = new GeneralFunction();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string path = Request.Url.AbsolutePath;
                string[] para = path.Split('?');
                if (para.Length == 1 || para.Length == 0)
                    return;
                string Parameter = clsCryptography.Decrypt(para[1]);
                hdnquery.Value = Parameter;
                Timer1.Enabled = true;
            }
        }


        public void DownloadFiles(string MemberId)
        {
            Timer1.Enabled = false;
            byte[] fileContent = null;
            DSIT_DataLayer objDAL = new DSIT_DataLayer();
            Hashtable HSTFile = new Hashtable();
            var parameters = new List<SqlParameter>();
            parameters.Add(objGeneralFunction.GetSqlParameter("@TableName", "App_Accounts", SqlDbType.VarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@ColumnName", "AccountName", SqlDbType.VarChar, 100, ParameterDirection.Input));
            parameters.Add(objGeneralFunction.GetSqlParameter("@WhereClause", "AccountId=" + MemberId + "", SqlDbType.VarChar, 100, ParameterDirection.Input));
            string MemberName = objDAL.ExecuteScalar("App_ExecuteScalar", parameters.ToArray());

            if (MemberName == "")
                return;

            MemberName = MemberName.Replace(" ", "_").Replace(".", "_");
            if (MemberId != "")
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        try
                        {

                            var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/MemberPhoto"), "*.*")
                                        let x = new FileInfo(o)
                                        where x.Name.ToUpper().Contains("MPU_" + MemberId.ToString() + "_".ToUpper())
                                        select o;

                            foreach (var Queryitem in query)
                            {

                                var zipArchiveEntry = archive.CreateEntry(Path.GetFileName(Queryitem), CompressionLevel.Fastest);
                                using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(File.ReadAllBytes(Queryitem), 0, File.ReadAllBytes(Queryitem).Length);
                                //zipArchiveEntry = archive.CreateEntry("file2.txt", CompressionLevel.Fastest);
                                //using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(file2, 0, file2.Length);

                            }
                        }
                        catch (Exception ex) { }

                        try
                        {

                            var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/MemberRegDocs"), "*.*")
                                        let x = new FileInfo(o)
                                        where x.Name.ToUpper().Contains("MRU_" + MemberId.ToString() + "_".ToUpper())
                                        select o;

                            foreach (var Queryitem in query)
                            {
                                using (FileStream fs = File.OpenRead(Queryitem))
                                {
                                    var binaryReader = new BinaryReader(fs);
                                    fileContent = binaryReader.ReadBytes((int)fs.Length);
                                    var zipArchiveEntry = archive.CreateEntry(Path.GetFileName(Queryitem), CompressionLevel.Fastest);
                                    using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(fileContent, 0, fileContent.Length);
                                }
                            }
                        }
                        catch (Exception ex) { }

                        try
                        {

                            var query = from o in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/MemberRegWorkDocs"), "*.*")
                                        let x = new FileInfo(o)
                                        where x.Name.ToUpper().Contains("MWN_" + MemberId.ToString() + "_".ToUpper())
                                        select o;

                            foreach (var Queryitem in query)
                            {
                                using (FileStream fs = File.OpenRead(Queryitem))
                                {
                                    var binaryReader = new BinaryReader(fs);
                                    fileContent = binaryReader.ReadBytes((int)fs.Length);
                                    var zipArchiveEntry = archive.CreateEntry(Path.GetFileName(Queryitem), CompressionLevel.Fastest);
                                    using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(fileContent, 0, fileContent.Length);
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }
                    Response.Clear();
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "filename=" + MemberName + ".zip");
                    Response.BinaryWrite(ms.ToArray());
                }
            }

            //if (fileContent != null)
            //{
            //    //HttpContext.Current.Response.ContentType = "image/jpeg";
            //    HttpContext.Current.Response.BinaryWrite(fileContent);
            //}
            //else
            //{ HttpContext.Current.Response.Write("No Image To Display"); }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            DownloadFiles(hdnquery.Value.Split('~')[0]);
        }
    }
}