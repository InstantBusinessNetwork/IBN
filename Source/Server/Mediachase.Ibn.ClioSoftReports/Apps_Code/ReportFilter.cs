using System;
using System.Collections;
using System.Data;

using Security = Mediachase.IBN.Business.Security;
using System.Collections.Generic;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public enum SelectProjectType
	{
		All = 0,
		FinishedInCurrentPeriod = 1,
		ActiveOnly = 2
	}

	/// <summary>
	/// Represents .
	/// </summary>
	public class ReportFilter
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportFilter"/> class.
		/// </summary>
		public ReportFilter()
		{
		}
		#endregion

		#region Properties
		private DateTime _fromDate = DateTime.MinValue;

		/// <summary>
		/// Gets or sets from date.
		/// </summary>
		/// <value>From date.</value>
		public DateTime FromDate
		{
			get { return _fromDate; }
			set { _fromDate = value; }
		}

		private DateTime _toDate = DateTime.MaxValue;

		/// <summary>
		/// Gets or sets to date.
		/// </summary>
		/// <value>To date.</value>
		public DateTime ToDate
		{
			get { return _toDate; }
			set { _toDate = value; }
		}

		private List<int> _managers = new List<int>();

		/// <summary>
		/// Gets the selected managers.
		/// </summary>
		/// <value>The selected managers.</value>
		public List<int> Managers
		{
			get { return _managers; }
		}

		private List<int> _clients = new List<int>();

		/// <summary>
		/// Gets the selected clients ( &gt;0 - organization, &lt;0 - Contact ).
		/// </summary>
		/// <value>The selected clients.</value>
		public List<int> Clients
		{
			get { return _clients; }
		}

		private List<int> _projectGroups = new List<int>();

		/// <summary>
		/// Gets the project groups.
		/// </summary>
		/// <value>The project groups.</value>
		public List<int> ProjectGroups
		{
			get { return _projectGroups; }
		}

		private List<int> _projects = new List<int>();

		/// <summary>
		/// Gets the projects.
		/// </summary>
		/// <value>The projects.</value>
		public List<int> Projects
		{
			get { return _projects; }
		}

		private SelectProjectType _projectType;

		public SelectProjectType SelectProjectType
		{
			get { return _projectType; }
			set { _projectType = value; }
		}
	
		#endregion

		#region Methods
		#endregion

		
	}
}
