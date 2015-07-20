using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Sync.Core.TransferDataType
{
	[Serializable]
	public class RecipientTransferData : SyncTransferData
	{
		public const string DataName = "Recipient";

		public const string FieldContact = "Contact";
		public const string FieldContactId = "ContactId";
		public const string FieldCreated = "Created";
		public const string FieldCreatorId = "CreatorId";
		public const string FieldEmail = "Email";
		public const string FieldEvent = "Event";
		public const string FieldEventId = "EventId";
		public const string FieldModified = "Modified";
		public const string FieldModifierId = "ModifierId";
		public const string FieldName = "Name";
		public const string FieldOrganization = "Organization";
		public const string FieldOrganizationId = "OrganizationId";
		public const string FieldPrincipal = "Principal";
		public const string FieldPrincipalId = "PrincipalId";
		public const string FieldResourceEventOrganizator = "ResourceEventOrganizator";
		public const string FieldRsvp = "Rsvp";
		public const string FieldStatus = "Status";

		public RecipientTransferData()
			:base(DataName)
		{
			InitializeProperties();
		}

		protected void InitializeProperties()
		{
			base.Properties.Add("Contact", null);
			base.Properties.Add("ContactId", null);
			base.Properties.Add("Created", null);
			base.Properties.Add("CreatorId", null);
			base.Properties.Add("Email", null);
			base.Properties.Add("Event", null);
			base.Properties.Add("EventId", null);
			base.Properties.Add("Modified", null);
			base.Properties.Add("ModifierId", null);
			base.Properties.Add("Name", null);
			base.Properties.Add("Organization", null);
			base.Properties.Add("OrganizationId", null);
			base.Properties.Add("Principal", null);
			base.Properties.Add("PrincipalId", null);
			base.Properties.Add("ResourceEventOrganizator", null);
			base.Properties.Add("Rsvp", null);
			base.Properties.Add("Status", null);

		}

		#region Named Properties

		public System.String Contact
		{
			get
			{
				return (System.String)base.Properties["Contact"];
			}

		}

		public Nullable<Guid> ContactId
		{
			get
			{
				return (Nullable<Guid>)base.Properties["ContactId"];
			}

			set
			{
				base.Properties["ContactId"] = value;
			}

		}

		public System.DateTime Created
		{
			get
			{
				return (System.DateTime)base.Properties["Created"];
			}

			set
			{
				base.Properties["Created"] = value;
			}

		}

		public System.Int32 CreatorId
		{
			get
			{
				return (System.Int32)base.Properties["CreatorId"];
			}

			set
			{
				base.Properties["CreatorId"] = value;
			}

		}

		public System.String Email
		{
			get
			{
				return (System.String)base.Properties["Email"];
			}

			set
			{
				base.Properties["Email"] = value;
			}

		}

		public System.String Event
		{
			get
			{
				return (System.String)base.Properties["Event"];
			}

		}

		public Guid EventId
		{
			get
			{
				return (Guid)base.Properties["EventId"];
			}

			set
			{
				base.Properties["EventId"] = value;
			}

		}

		public System.DateTime Modified
		{
			get
			{
				return (System.DateTime)base.Properties["Modified"];
			}

			set
			{
				base.Properties["Modified"] = value;
			}

		}

		public System.Int32 ModifierId
		{
			get
			{
				return (System.Int32)base.Properties["ModifierId"];
			}

			set
			{
				base.Properties["ModifierId"] = value;
			}

		}

		public System.String Name
		{
			get
			{
				return (System.String)base.Properties["Name"];
			}

			set
			{
				base.Properties["Name"] = value;
			}

		}

		public System.String Organization
		{
			get
			{
				return (System.String)base.Properties["Organization"];
			}

		}

		public Nullable<Guid> OrganizationId
		{
			get
			{
				return (Nullable<Guid>)base.Properties["OrganizationId"];
			}

			set
			{
				base.Properties["OrganizationId"] = value;
			}

		}

		public System.String Principal
		{
			get
			{
				return (System.String)base.Properties["Principal"];
			}

		}

		public Nullable<Guid> PrincipalId
		{
			get
			{
				return (Nullable<Guid>)base.Properties["PrincipalId"];
			}

			set
			{
				base.Properties["PrincipalId"] = value;
			}

		}

		public System.Boolean ResourceEventOrganizator
		{
			get
			{
				return (System.Boolean)base.Properties["ResourceEventOrganizator"];
			}

			set
			{
				base.Properties["ResourceEventOrganizator"] = value;
			}

		}

		public System.Boolean Rsvp
		{
			get
			{
				return (System.Boolean)base.Properties["Rsvp"];
			}

			set
			{
				base.Properties["Rsvp"] = value;
			}

		}

		public Nullable<System.Int32> Status
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["Status"];
			}

			set
			{
				base.Properties["Status"] = value;
			}

		}

		#endregion
        
	}
}
