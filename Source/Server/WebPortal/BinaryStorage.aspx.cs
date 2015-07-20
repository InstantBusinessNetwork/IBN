using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data.SqlClient;
using Mediachase.IBN.Database;
using System.Data;
using System.Configuration;
using System.IO;

namespace Mediachase.UI.Web
{
    public partial class BinaryStorage : System.Web.UI.Page
    {
        private static Hashtable m_Files = new Hashtable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string fid = Request["FID"];
                if (fid != null)
                    GetFile(fid);
                else
                    Response.Write("Error in request");
            }
        }

        #region GetFile
        private void GetFile(string fid)
        {
            using (SqlConnection cn = new SqlConnection(DbHelper2.ConnectionString))
            {
                cn.Open();
				SqlCommand cmd = new SqlCommand("SELECT id FROM IM_BINARY_DATA WHERE fid=@fid", cn);
                cmd.Parameters.Add("@fid", SqlDbType.Char, 36).Value = fid;
                cmd.CommandType = CommandType.Text;

                object obj = cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                {
                    int fileId = (int)obj;

                    // Delete old files
                    string folder = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["TempDownloadDir"]);
                    if (System.IO.Directory.Exists(folder))
                    {
                        foreach (string filePath in System.IO.Directory.GetFiles(folder, "*.bin"))
                        {
                            try
                            {
                                if (File.GetLastWriteTime(filePath) < DateTime.Now.AddHours(-1))
                                    File.Delete(filePath);
                            }
                            catch (UnauthorizedAccessException) { }
                        }
                    }

                    // Save file
                    string shortName = string.Format("{0}.bin", fid);
                    string fullName = string.Format("{0}\\{1}", folder, shortName);
                    try
                    {
                        lock (m_Files)
                        {
                            if (!File.Exists(fullName) || File.GetLastWriteTime(fullName) < DateTime.Now.AddHours(-1))
                            {
                                CreatePath(fullName);
                                using (FileStream file = File.Open(fullName, FileMode.Create))
                                {
                                    WriteToStream(fileId, file, cn);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Cannot write to file - most likely somebody is reading it.
                        // Send existing file.
                    }
                    //Response.ContentType = "application/octet-stream";
                    //Response.TransmitFile(fullName);
                    Response.Redirect(string.Format("~/imdownload/{0}", shortName));
                }
                else
                    throw new Exception(string.Format("No file with fid={0}", fid));
            }
            Response.End();
        } 
        #endregion

        #region WriteToStream
        static void WriteToStream(int fileId, Stream stream, SqlConnection cn)
        {
            int szReadSize = 65535;

            if (null == stream)
                throw new ArgumentNullException("Stream cannot be null.");

            SqlCommand cmd = new SqlCommand("OM_BINARYREAD", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@id", System.Data.SqlDbType.Int, 4));
            cmd.Parameters["@id"].Direction = ParameterDirection.Input;
            cmd.Parameters["@id"].Value = fileId;
            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@offset", System.Data.SqlDbType.Int, 4));
            cmd.Parameters["@offset"].Direction = ParameterDirection.Input;
            cmd.Parameters["@offset"].Value = 0;
            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@size", System.Data.SqlDbType.Int, 4));
            cmd.Parameters["@size"].Direction = ParameterDirection.Input;
            cmd.Parameters["@size"].Value = szReadSize;
            SqlDataReader dr = cmd.ExecuteReader();
            int totalread = 0;
            while (dr.Read())
            {
                if ((dr["FileData"] == null) || (dr["FileData"] == DBNull.Value))
                {
                    dr.Close();
                    break;
                }
                byte[] bt = (byte[])dr["FileData"];
                dr.Close();
                if ((bt == null) || (bt.Length == 0))
                    break;
                totalread += bt.Length;
                stream.Write(bt, 0, bt.Length);
                cmd.Parameters["@offset"].Value = totalread;
                cmd.Parameters["@size"].Value = szReadSize;
                dr = cmd.ExecuteReader();
                if (dr == null)
                    break;
            }
        }
        #endregion

        #region CreatePath
        static void CreatePath(string path)
        {
            string p = path, s = "";
            int i;
            while (true)
            {
                i = p.IndexOf('\\');
                if (i < 0)
                    break;
                s += p.Substring(0, i + 1);
                System.IO.Directory.CreateDirectory(s);
                p = p.Substring(i + 1);
            }
        }
        #endregion
    }
}
