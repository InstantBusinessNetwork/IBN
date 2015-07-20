using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.Wdom;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.IBN.Business.WebDAV.Common;
using System.IO;
using System.Web;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    /// <summary>
    /// Обеспечивает доступ к файлам в компоненте MetaData
    /// </summary>
    public class MetaDataProvider : MetaDataPlusProvider
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

        protected override ResourceInfo GetResourceInfo(WebDavTicket ticket)
        {
            MetaDataAbsolutePath metaAbsPath = ticket.AbsolutePath as MetaDataAbsolutePath;
            if (metaAbsPath == null)
                throw new ArgumentException("absPath");

            ResourceInfo retVal = new ResourceInfo();

            Mediachase.Ibn.Data.Meta.FileInfo fileInfo = new Mediachase.Ibn.Data.Meta.FileInfo(metaAbsPath.FileUID);

            retVal.AbsolutePath = ticket.ToString();
            retVal.Tag = fileInfo;
            retVal.Name = fileInfo.Name;
            retVal.ContentType = ContentTypeResolver.Resolve(Path.GetExtension(fileInfo.Name));
            retVal.ContentLength = fileInfo.Length;
            retVal.ParentName = "root";
            retVal.Modified = DateTime.Now;
            retVal.Created = DateTime.Now;

            return retVal;
        }

        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override System.IO.Stream OpenRead(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            if (element == null || element is CollectionInfo)
                return null;

            Mediachase.Ibn.Data.Meta.FileInfo fileInfo = (Mediachase.Ibn.Data.Meta.FileInfo)element.Tag;
            return fileInfo.OpenRead();
        }

        /// <summary>
        /// Opens the write.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override System.IO.Stream OpenWrite(Mediachase.Net.Wdom.WebDavElementInfo element, long contentLength)
        {
            if (element == null || element is CollectionInfo)
                return null;

            Mediachase.Ibn.Data.Meta.FileInfo fileInfo = (Mediachase.Ibn.Data.Meta.FileInfo)element.Tag;
            try
            {
				AutoCommitedTransactedStream tranStream = new AutoCommitedTransactedStream(DataContext.Current.BeginTransaction(), contentLength);
                tranStream.InnerStream = fileInfo.OpenWrite();
				tranStream.SetLength(0);

                return tranStream;
            }
            catch (Exception)
            {
                throw new HttpException(404, "Not Found");
            }

          
        } 
    
        #region Not implement
        public override void CopyTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Mediachase.Net.Wdom.CollectionInfo CreateCollection(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Mediachase.Net.Wdom.ResourceInfo CreateResource(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Delete(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Mediachase.Net.Wdom.WebDavElementInfo[] GetChildElements(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            throw new Exception("The method or operation is not implemented.");
        }


        public override void MoveTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
