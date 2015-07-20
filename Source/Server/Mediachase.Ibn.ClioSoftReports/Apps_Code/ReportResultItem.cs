using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Collections.Generic;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	/// <summary>
	/// Represents report result item.
	/// </summary>
	public class ReportResultItem
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportResultItem"/> class.
		/// </summary>
		public ReportResultItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportResultItem"/> class.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="objectId">The object id.</param>
		/// <param name="name">The name.</param>
		public ReportResultItem(ObjectTypes objectType, int objectId, string name)
		{
			this.ObjectType = objectType;
			this.ObjectId = objectId;
			this.Name = name;
		}
		#endregion

		#region Properties
		private string _name;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private ObjectTypes _objectType;

		/// <summary>
		/// Gets or sets the type of the object.
		/// </summary>
		/// <value>The type of the object.</value>
		public ObjectTypes ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}

		private int _objectId;

		/// <summary>
		/// Gets or sets the object id.
		/// </summary>
		/// <value>The object id.</value>
		public int ObjectId
		{
			get { return _objectId; }
			set { _objectId = value; }
		}

		private double _total;

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		/// <value>The total.</value>
		public double Total
		{
			get { return _total; }
			set { _total = value; }
		}

		private List<ReportResultItem> _childsItems = new List<ReportResultItem>();


		/// <summary>
		/// Gets the child items.
		/// </summary>
		/// <value>The child items.</value>
		public List<ReportResultItem> ChildItems
		{
			get { return _childsItems; }
		}
	
		#endregion

		#region Methods
		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
				return base.ToString();

			return this.ObjectType.ToString() + ":" + this.ObjectId.ToString() + " [" + this.Name + "]";
		}
		#endregion

		
	}
}
