using System;
using System.Collections;
using System.Xml;

using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	#region * Enums *
	public enum VariableType
	{
		String,
		Date,
		DateTime
	}

	public enum Variable
	{
		CreatedBy,
		Description,
		DocumentStatus,
		Email,
		End,
		EventDate,
		EventName,
		EventType,
		FileSize,
		FirstName,
		InitiatedBy,
		IssueState,
		LastName,
		Link,
		ListStatus,
		ListType,
		Login,
		LogonLink,
		MailBoxLink,
		MailBoxTitle,
		Manager,
		Password,
		PercentCompleted,
		PortalLink,
		Priority,
		ProjectLink,
		ProjectStatus,
		ProjectTitle,
		ServerLink,
		Start,
		State,
		Text,
		Ticket,
		Title,
		UnsubscribeLink,
		UpdatedBy,
		Company,
		Department,
		Fax,
		Location,
		Mobile,
		Phone,
		Position,
		OwnerTitle,
		OwnerLink,
		Participant,
		/*
		FolderTitle,
		FolderLink,
		OwnerType,
		Name,
		UserLink,
		FileName,
		*/
	}
	#endregion

	#region class VariableInfo
	public class VariableInfo
	{
		public string name = string.Empty;
		public string value = string.Empty;
		public string languageId = string.Empty;
		public string type = VariableType.String.ToString();

		private string _isLink;
		private string _external;
		private string _disableNoMenu;
		private string _isHtml;

		public bool IsLink { get { return _isLink == "1"; } }
		public bool External { get { return _external == "1"; } }
		public bool DisableNoMenu { get { return _disableNoMenu == "1"; } }
		public bool IsHtml
		{
			get { return _isHtml == "1"; }
			private set { _isHtml = (value ? "1" : "0"); }
		}

		#region GetAttribute()
		private string GetAttribute(XmlNode var, string name)
		{
			string result = string.Empty;

			XmlAttribute a = var.Attributes[name];
			if (a != null)
				result = a.Value;

			return result;
		}
		#endregion

		#region WriteAttribute()
		private void WriteAttribute(XmlTextWriter w, string name, string value)
		{
			if (!string.IsNullOrEmpty(value))
				w.WriteAttributeString(name, value);
		}
		#endregion

		#region * Constructors *
		public VariableInfo(string Name, string Value, VariableType Type, bool IsLink, bool IsExternal)
		{
			name = Name;
			value = Value;
			type = Type.ToString();
			this._isLink = IsLink ? "1" : "0";
			this._external = IsExternal ? "1" : "0";
		}

		private VariableInfo(bool isRelObject, Variable name, string value)
		{
			this.name = string.Format("{0}{1}", isRelObject ? "Rel" : "", name.ToString());
			this.value = value;
		}

		public VariableInfo(AlertVariable var, string value)
			: this(var, value, string.Empty, VariableType.String)
		{
		}

		public VariableInfo(AlertVariable var, string value, bool isHtml)
			: this(var, value, string.Empty, VariableType.String)
		{
			this.IsHtml = isHtml;
		}

		public VariableInfo(AlertVariable var, string value, string languageId)
			: this(var, value, languageId, VariableType.String)
		{
		}

		public VariableInfo(AlertVariable var, string value, VariableType type)
			: this(var, value, string.Empty, type)
		{
		}

		public VariableInfo(AlertVariable var, string value, string languageId, VariableType type)
			: this(var.isRelObject, var.name, value)
		{
			this.languageId = languageId;
			this.type = type.ToString();
			if (var.isLink)
				this._isLink = "1";
			if (var.external)
				this._external = "1";
			this._disableNoMenu = var.disableNoMenu ? "1" : "0";
		}

		public VariableInfo(XmlNode var)
		{
			this.languageId = GetAttribute(var, "lang");
			this.name = GetAttribute(var, "name");
			this.value = GetAttribute(var, "value");
			this.type = GetAttribute(var, "type");
			this._isLink = GetAttribute(var, "isLink");
			this._external = GetAttribute(var, "external");
			this._disableNoMenu = GetAttribute(var, "disableNoMenu");
			this._isHtml = GetAttribute(var, "isHtml");
		}
		#endregion

		#region Save()
		public void Save(XmlTextWriter w)
		{
			w.WriteStartElement("var");

			WriteAttribute(w, "lang", languageId);
			WriteAttribute(w, "type", type);
			WriteAttribute(w, "isLink", _isLink);
			WriteAttribute(w, "external", _external);
			if (DisableNoMenu)
				WriteAttribute(w, "disableNoMenu", _disableNoMenu);
			WriteAttribute(w, "name", name);
			WriteAttribute(w, "value", value);
			WriteAttribute(w, "isHtml", _isHtml);

			w.WriteEndElement();
		}
		#endregion
	}
	#endregion

	#region class AlertVariable
	public class AlertVariable
	{
		#region * Fields *

		public Variable name;
		public bool isLink;
		public bool external;
		public bool isRelObject;
		public bool disableNoMenu;
		public bool isDataLink;
		public bool isResourceVariable;

		#endregion

		#region * Properties *
		public string Name
		{
			get
			{
				return string.Format("{0}{1}", isRelObject ? "Rel" : "", name);
			}
		}
		#endregion

		#region * Constructors *
		private AlertVariable()
		{
		}

		internal AlertVariable(Variable name)
			: this(name, false, true, false)
		{
		}

		internal AlertVariable(Variable name, bool isRelObject)
			: this(name, false, true, isRelObject)
		{
		}

		internal AlertVariable(Variable name, bool isLink, bool external)
			: this(name, isLink, external, false)
		{
		}

		internal AlertVariable(Variable name, bool isLink, bool external, bool isRelObject)
		{
			this.name = name;
			this.isLink = isLink;
			this.external = external;
			this.isRelObject = isRelObject;
			this.disableNoMenu = false;
		}
		#endregion

		#region GetVariables()
		internal static AlertVariable[] GetVariables(SystemEventTypes eventType)
		{
			ObjectTypes objectType = (ObjectTypes)DBSystemEvents.GetObjectType((int)eventType);
			ObjectTypes relObjectType = (ObjectTypes)DBSystemEvents.GetRelObjectType((int)eventType);

			ArrayList vars = new ArrayList();

			GetVariables(eventType, objectType, false, vars);
			GetVariables(eventType, relObjectType, true, vars);

			return vars.ToArray(typeof(AlertVariable)) as AlertVariable[];
		}

		private static void GetVariables(SystemEventTypes eventType, ObjectTypes objectType, bool isRelObject, ArrayList vars)
		{
			// Common variables
			if (!isRelObject)
			{
				vars.Add(new AlertVariable(Variable.ServerLink, isRelObject));
				vars.Add(new AlertVariable(Variable.PortalLink, isRelObject));
				vars.Add(new AlertVariable(Variable.InitiatedBy, isRelObject));
				vars.Add(new AlertVariable(Variable.UnsubscribeLink, true, true, isRelObject));
			}

			AlertVariable var;

			switch (objectType)
			{
				case ObjectTypes.CalendarEntry:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.End, isRelObject));
					vars.Add(new AlertVariable(Variable.EventType, isRelObject));
					if (eventType != SystemEventTypes.CalendarEntry_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, eventType != SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentDeleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Start, isRelObject));
					vars.Add(new AlertVariable(Variable.State, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.Comment:
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					break;
				case ObjectTypes.Document:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.DocumentStatus, isRelObject));
					if (eventType != SystemEventTypes.Document_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, eventType != SystemEventTypes.Document_Updated_ResourceList_AssignmentDeleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.File_FileStorage:
					vars.Add(new AlertVariable(Variable.FileSize, isRelObject));
					if (eventType != SystemEventTypes.Todo_Updated_FileList_FileDeleted
						&& eventType != SystemEventTypes.Task_Updated_FileList_FileDeleted
						&& eventType != SystemEventTypes.CalendarEntry_Updated_FileList_FileDeleted
						&& eventType != SystemEventTypes.Project_Updated_FileList_FileDeleted
						&& eventType != SystemEventTypes.Issue_Updated_FileList_FileDeleted
						&& eventType != SystemEventTypes.Document_Updated_FileList_FileDeleted
						)
					{
						var = new AlertVariable(Variable.Link, true, true, isRelObject);
						var.disableNoMenu = true;
						vars.Add(var);
					}
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					break;
				case ObjectTypes.Issue:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.IssueState, isRelObject));
					if (eventType != SystemEventTypes.Issue_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, eventType != SystemEventTypes.Issue_Updated_ResourceList_AssignmentDeleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.Ticket, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.IssueBox:
					vars.Add(new AlertVariable(Variable.Link, true, true, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					break;
				case ObjectTypes.IssueRequest:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Email, isRelObject));
					if (eventType != SystemEventTypes.IssueRequest_Approved && eventType != SystemEventTypes.IssueRequest_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, true, isRelObject));
					vars.Add(new AlertVariable(Variable.MailBoxLink, true, false));
					vars.Add(new AlertVariable(Variable.MailBoxTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					break;
				case ObjectTypes.List:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					if (eventType != SystemEventTypes.List_Deleted)
					{
						var = new AlertVariable(Variable.Link, true, true, isRelObject);
						if (eventType == SystemEventTypes.List_Updated_Data)
							var.isDataLink = true;
						vars.Add(var);
					}
					var = new AlertVariable(Variable.ListStatus, isRelObject);
					var.isResourceVariable = true;
					vars.Add(var);
					var = new AlertVariable(Variable.ListType, isRelObject);
					var.isResourceVariable = true;
					vars.Add(var);
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.Project:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.End, isRelObject));
					if (eventType != SystemEventTypes.Project_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.PercentCompleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectStatus, isRelObject));
					vars.Add(new AlertVariable(Variable.Start, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.Task:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.End, isRelObject));
					if (eventType != SystemEventTypes.Task_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, eventType != SystemEventTypes.Task_Updated_ResourceList_AssignmentDeleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.PercentCompleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Start, isRelObject));
					vars.Add(new AlertVariable(Variable.State, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.ToDo:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.Description, isRelObject));
					vars.Add(new AlertVariable(Variable.End, isRelObject));
					if (eventType != SystemEventTypes.Todo_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, eventType != SystemEventTypes.Todo_Updated_ResourceList_AssignmentDeleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Manager, isRelObject));
					vars.Add(new AlertVariable(Variable.PercentCompleted, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.ProjectTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Start, isRelObject));
					vars.Add(new AlertVariable(Variable.State, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.UpdatedBy, isRelObject));
					break;
				case ObjectTypes.User:
					vars.Add(new AlertVariable(Variable.Email, isRelObject));
					vars.Add(new AlertVariable(Variable.Login, isRelObject));
					vars.Add(new AlertVariable(Variable.Password, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					vars.Add(new AlertVariable(Variable.Link, true, true, isRelObject));
					break;
				case ObjectTypes.Assignment:
					vars.Add(new AlertVariable(Variable.CreatedBy, isRelObject));
					vars.Add(new AlertVariable(Variable.End, isRelObject));
					if (eventType != SystemEventTypes.Assignment_Deleted)
						vars.Add(new AlertVariable(Variable.Link, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.OwnerLink, true, false, isRelObject));
					vars.Add(new AlertVariable(Variable.OwnerTitle, isRelObject));
					vars.Add(new AlertVariable(Variable.Participant, isRelObject));
					vars.Add(new AlertVariable(Variable.Priority, isRelObject));
					vars.Add(new AlertVariable(Variable.State, isRelObject));
					vars.Add(new AlertVariable(Variable.Title, isRelObject));
					break;
			}
		}
		#endregion
	}
	#endregion
}
