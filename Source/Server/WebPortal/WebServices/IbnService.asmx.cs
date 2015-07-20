using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using System.Xml.Serialization;

namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for IbnService
	/// </summary>
	[WebService(Namespace = "http://mediachase.net/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class IbnService : System.Web.Services.WebService
	{
		#region Execute
		/// <summary>
		/// Executes the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		public Response Execute(Request request)
		{
			throw new NotImplementedException();
		} 
		#endregion

		#region Create
		[WebMethod]
		public PrimaryKeyId Create(EntityObject target)
		{
			throw new NotImplementedException();
		} 
		#endregion

		#region Update
		[WebMethod]
		public void Update(EntityObject target)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Delete
		[WebMethod]
		public void Delete(string metaClassName, PrimaryKeyId primaryKeyId)
		{
			throw new NotImplementedException();
		}
		#endregion


		#region Load
		[WebMethod]
		public EntityObject Load(string metaClassName, PrimaryKeyId primaryKeyId)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region List
		[WebMethod]
		public EntityObject[] List(string metaClassName, FilterElement[] filters)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
