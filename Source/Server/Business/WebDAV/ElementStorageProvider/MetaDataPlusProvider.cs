using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.WebDavServer;
using Mediachase.Net.Wdom;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.MetaDataPlus;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Mediachase.IBN.Business.WebDAV.Common;
using System.Web;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    /// <summary>
    /// Обеспечивает доступ к файлам в компоненте MetaDataPlus
    /// </summary>
    public class MetaDataPlusProvider : WebDavPropertyStorage
    {
        /// <summary>
        /// Initializes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="config">The config.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override Mediachase.Net.Wdom.WebDavElementInfo GetElementInfo(string strTicket)
        {
            WebDavElementInfo retVal = null;
            try
            {
                WebDavTicket ticket = WebDavTicket.Parse(strTicket);
                if (ticket.IsCollection)
                {
                    retVal = GetCollectionInfo(ticket);
                }
                else
                {

                    retVal = GetResourceInfo(ticket);
                }

            }
            catch (FormatException)
            {
            }

            return retVal;
        }

        protected virtual CollectionInfo GetCollectionInfo(WebDavTicket ticket)
        {

            CollectionInfo retVal = new CollectionInfo();

            retVal.Name = "root";
            retVal.Created = DateTime.MinValue;
            retVal.Modified = DateTime.MinValue;
            retVal.AbsolutePath = ticket.ToString();

            return retVal;
        }

        protected virtual ResourceInfo GetResourceInfo(WebDavTicket ticket)
        {
			if (ticket == null)
			{
				throw new ArgumentNullException("ticket");
			}
            MetaDataPlusAbsolutePath metaAbsPath = ticket.AbsolutePath as MetaDataPlusAbsolutePath;
			if (metaAbsPath == null)
			{
				throw new NullReferenceException("metaAbsPath");
			}

            ResourceInfo retVal = new ResourceInfo();
            MetaObject obj = MetaDataWrapper.LoadMetaObject(metaAbsPath.MetaObjectId, metaAbsPath.MetaObjectType);
			if (obj == null)
			{
				throw new NullReferenceException("obj");
			}

            MetaFile metaFile = (MetaFile)obj[metaAbsPath.MetaFieldName];
           
            if (metaFile != null)
            {
                retVal.AbsolutePath = ticket.ToString();
                retVal.Tag = new object[] { obj, metaFile, metaAbsPath};
                retVal.Name = metaFile.Name;
                retVal.ContentType = ContentTypeResolver.Resolve(Path.GetExtension(metaFile.Name));
                retVal.ContentLength = metaFile.Size;
                retVal.ParentName = "root";
                retVal.Modified = metaFile.LastWriteTime;
                retVal.Created = metaFile.CreationTime;
            }

            return retVal;
        }

        public override System.IO.Stream OpenRead(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            if (element == null || element is CollectionInfo)
                return null;

            object[] param = (object[])element.Tag;
            MetaFile metaFile = (MetaFile)param[1];

            return new MemoryStream(metaFile.Buffer);
        }

        public override System.IO.Stream OpenWrite(Mediachase.Net.Wdom.WebDavElementInfo element, long contentLength)
        {
            if (element == null || element is CollectionInfo)
                return null;

            object[] param = (object[])element.Tag;
            MetaFile metaFile = (MetaFile)param[1];
            MetaObject metaObj = (MetaObject)param[0];
            MetaDataPlusAbsolutePath absPath = (MetaDataPlusAbsolutePath)param[2];
            

            return new MetaFileStream(metaObj, metaFile, absPath.MetaFieldName, contentLength);
        }

        #region NotImplemented
        public override void CopyTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override Mediachase.Net.Wdom.CollectionInfo CreateCollection(string path)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        public override void Delete(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override Mediachase.Net.Wdom.WebDavElementInfo[] GetChildElements(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        public override void MoveTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        public override Mediachase.Net.Wdom.ResourceInfo CreateResource(string path)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion
    }
}
