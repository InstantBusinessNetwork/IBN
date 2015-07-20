using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.Wdom;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.IBN.Business.EMail;
using System.IO;
using Mediachase.IBN.Business.WebDAV.Common;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    public class EmailStorageProvider : MetaDataPlusProvider
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
            ResourceInfo retVal = new ResourceInfo();
            EmailStorageAbsolutePath absPath = ticket.AbsolutePath as EmailStorageAbsolutePath;
            if(absPath == null)
                throw new ArgumentException("absPath");

            EMailMessageRow row = new EMailMessageRow(absPath.EmailMsgId);
            MemoryStream memStream = new MemoryStream(row.EmlMessage.Length);
            memStream.Write(row.EmlMessage, 0, row.EmlMessage.Length);
            memStream.Position = 0;

            Pop3Message message = new Pop3Message(memStream);
            int attachmentIndex = absPath.EmailAttachmentIndex;
            Mediachase.IBN.Business.EMail.EMailMessageInfo.AttachmentData entry = 
                            EMailMessageInfo.GetAttachment(message.MimeEntries, ref attachmentIndex);

            if (entry != null)
            {

                retVal.AbsolutePath = ticket.ToString();
                retVal.Tag = entry;
                retVal.Name = entry.FileName;
				//Fix ET:26-11-2008 Solve trouble inconsistency Content-Type email attachment and file extension
				//try first get Content-Type by file extension 
				retVal.ContentType = ContentTypeResolver.Resolve(Path.GetExtension(entry.FileName));
				if (String.IsNullOrEmpty(retVal.ContentType))
				{
					//otherwise set ContentType as ContentType email attachment
					retVal.ContentType = entry.ContentType;
				}
                retVal.ContentLength = entry.Data.Length;
                retVal.ParentName = "root";
                DateTime created = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, row.Created);
                retVal.Created = created;
                retVal.Modified = created;
            }

            return retVal;
        
        }

        public override System.IO.Stream OpenRead(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            if (element == null || element is CollectionInfo)
                return null;

            Mediachase.IBN.Business.EMail.EMailMessageInfo.AttachmentData entry = 
                                    element.Tag as Mediachase.IBN.Business.EMail.EMailMessageInfo.AttachmentData;

            return new MemoryStream(entry.Data);
        }

        #region Not implement

        public override System.IO.Stream OpenWrite(Mediachase.Net.Wdom.WebDavElementInfo element, long contentLength)
        {
            throw new NotImplementedException();
        } 
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
