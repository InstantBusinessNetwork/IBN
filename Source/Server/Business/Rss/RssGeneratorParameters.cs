using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.IBN.Business.Rss
{
	/// <summary>
	/// Represents RSS generator parameters.
	/// </summary>
	public class RssGeneratorParameters
	{
		#region Const
		#endregion

		#region Properties
		public Guid UserId { get; set; }
		public string ClassName { get; set; }
		public int? ObjectId { get; set; }
		public string CurrentView { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RssGeneratorParameters"/> class.
		/// </summary>
		public RssGeneratorParameters()
		{
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
