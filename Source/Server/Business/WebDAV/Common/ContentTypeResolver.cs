using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Mediachase.IBN.Business.WebDAV.Common
{
    internal class ContentTypeResolver
    {

        private static char[] WHITESPACE_CHARS = new char[] {'.', 
                '\t', '\n', '\v', '\f', '\r', ' ', 
                '\x0085', '\x00a0', '\u1680', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', 
                '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u200b', '\u2028', '\u2029', '\u3000', 
                '\ufeff'};


        /// <summary>
        /// Resolves the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string Resolve(string extension)
        {
            String retVal = String.Empty;
            extension = CleanupExtension(extension);

            /// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
            /// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
            using(IDataReader reader = ContentType.GetContentTypeByExtension(extension))
            {
                if(reader.Read())
                {
                    if (reader["ContentTypeString"] != DBNull.Value)
                    {
                        retVal = Convert.ToString(reader["ContentTypeString"]);
                    }
                }
            }

            return retVal;
        }

		/// <summary>
		/// Gets the content type id.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <returns></returns>
		public static int GetContentTypeId(string contentType)
		{
			int retVal = -1;

			using(IDataReader reader = ContentType.GetContentTypeByString(contentType))
            {
                if(reader.Read())
                {
					if (reader["ContentTypeId"] != DBNull.Value)
                    {
						retVal = Convert.ToInt32(reader["ContentTypeId"]);
                    }
                }
            }

			return retVal;
		}

        /// <summary>
        /// Determines whether [is web DAV supported extension] [the specified extension].
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// 	<c>true</c> if [is web DAV supported extension] [the specified extension]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWebDAVSupportedExtension(string extension)
        {
            bool retVal = false;

            extension = CleanupExtension(extension);

            /// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
            /// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
            using (IDataReader reader = ContentType.GetContentTypeByExtension(extension))
            {

                if (reader.Read())
                {
                    retVal = (reader["AllowWebDav"] != DBNull.Value && (bool)reader["AllowWebDav"]);
                }
            }

            return retVal;
        }

		/// <summary>
		/// Determines whether [is allow force download] [the specified extension].
		/// </summary>
		/// <param name="extension">The extension.</param>
		/// <returns>
		/// 	<c>true</c> if [is allow force download] [the specified extension]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsAllowForceDownload(string extension)
		{
			bool retVal = false;

			extension = CleanupExtension(extension);

			/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
			/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow, AllowForceDownload
			using (IDataReader reader = ContentType.GetContentTypeByExtension(extension))
			{

				if (reader.Read())
				{
					retVal = (reader["AllowForceDownload"] != DBNull.Value && (bool)reader["AllowForceDownload"]);
				}
			}

			return retVal;
		}

        /// <summary>
        /// Cleanups the Extension.
        /// </summary>
        /// <param name="Extension">The extension.</param>
        /// <returns></returns>
        private static string CleanupExtension(string extension)
        {
            string retVal = string.Empty;

            if(!String.IsNullOrEmpty(extension))
            {
                retVal =  extension.ToLower().Trim(WHITESPACE_CHARS);
            }

            return retVal;
        }

    }
}
