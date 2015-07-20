using System;
using System.Collections;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for DefaultEMailIncidentMappingBlock.
	/// </summary>
	public class DefaultEMailIncidentMappingBlock: BaseConfigBlock
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultEMailIncidentMappingBlock"/> class.
		/// </summary>
		public DefaultEMailIncidentMappingBlock()
		{
		}

		public int DefaultCreator
		{
			set 
			{
				base.Params["DefaultCreator"] = value;
			}
			get 
			{
				if(base.Params.Contains("DefaultCreator"))
					return (int)base.Params["DefaultCreator"];

				return -1;
			}
		}


		/// <summary>
		/// Gets or sets the severity id.
		/// </summary>
		/// <value>The severity id.</value>
		public int SeverityId
		{
			set 
			{
				base.Params["SeverityId"] = value;
			}
			get 
			{
				if(base.Params.Contains("SeverityId"))
					return (int)base.Params["SeverityId"];

				return 1;
			}
		}

		/// <summary>
		/// Gets or sets the type id.
		/// </summary>
		/// <value>The type id.</value>
		public int TypeId
		{
			set 
			{
				base.Params["TypeId"] = value;
			}
			get 
			{
				if(base.Params.Contains("TypeId"))
					return (int)base.Params["TypeId"];

				return 1;
			}
		}

		/// <summary>
		/// Gets or sets the priority id.
		/// </summary>
		/// <value>The priority id.</value>
		public int PriorityId
		{
			set
			{
				base.Params["PriorityId"] = value;
			}
			get
			{
				if (base.Params.Contains("PriorityId"))
					return (int)base.Params["PriorityId"];

				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the general categories.
		/// </summary>
		/// <value>The general categories.</value>
		public ArrayList GeneralCategories
		{
			set 
			{
				base.Params["GeneralCategories"] = value;
			}
			get 
			{
				if(!base.Params.Contains("GeneralCategories"))
					base.Params["GeneralCategories"] = new ArrayList();

				return (ArrayList)base.Params["GeneralCategories"];
			}
		}

		/// <summary>
		/// Gets or sets the incident categories.
		/// </summary>
		/// <value>The incident categories.</value>
		public ArrayList IncidentCategories
		{
			set 
			{
				base.Params["IncidentCategories"] = value;
			}
			get 
			{
				if(!base.Params.Contains("IncidentCategories"))
					base.Params["IncidentCategories"] = new ArrayList();

				return (ArrayList)base.Params["IncidentCategories"];
			}
		}

		/// <summary>
		/// Gets or sets the project id.
		/// </summary>
		/// <value>The project id.</value>
		public int ProjectId
		{
			set 
			{
				base.Params["ProjectId"] = value;
			}
			get 
			{
				if(base.Params.Contains("ProjectId"))
					return (int)base.Params["ProjectId"];

				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the incident box id.
		/// </summary>
		/// <value>The incident box id.</value>
		public int IncidentBoxId
		{
			set 
			{
				base.Params["IncidentBoxId"] = value;
			}
			get 
			{
				if(base.Params.Contains("IncidentBoxId"))
					return (int)base.Params["IncidentBoxId"];

				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the org uid.
		/// </summary>
		/// <value>The org uid.</value>
		public PrimaryKeyId OrgUid
		{
			set 
			{
				base.Params["OrgUid"] = value.ToString();
			}
			get 
			{
				if (base.Params.Contains("OrgUid"))
					return PrimaryKeyId.Parse(base.Params["OrgUid"].ToString());

				return PrimaryKeyId.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the contact uid.
		/// </summary>
		/// <value>The contact uid.</value>
		public PrimaryKeyId ContactUid
		{
			set 
			{
				base.Params["ContactUid"] = value.ToString();
			}
			get 
			{
				if (base.Params.Contains("ContactUid"))
					return PrimaryKeyId.Parse(base.Params["ContactUid"].ToString());

				return PrimaryKeyId.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the type of the description.
		/// </summary>
		/// <value>The type of the description.</value>
		public int DescriptionId
		{
			set
			{
				base.Params["DescriptionId"] = value;
			}
			get
			{
				if (base.Params.Contains("DescriptionId"))
					return (int)base.Params["DescriptionId"];

				return -1;
			}
		}
	}
}
