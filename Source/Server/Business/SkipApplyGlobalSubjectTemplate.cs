using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Represents SkipApplyGlobalAlertSubjectFormat.
	/// </summary>
	public sealed class SkipApplyGlobalSubjectTemplate: IDisposable
	{
		#region Const
		#endregion

		#region Fields
		[ThreadStatic]
		static SkipApplyGlobalSubjectTemplate currentItem;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SkipApplyGlobalAlertSubjectFormat"/> class.
		/// </summary>
		public SkipApplyGlobalSubjectTemplate()
		{
			if (SkipApplyGlobalSubjectTemplate.IsActive)
			{
				this.IsTop = false;
			}
			else
			{
				this.IsTop = true;
				currentItem = this;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether this instance is top.
		/// </summary>
		/// <value><c>true</c> if this instance is top; otherwise, <c>false</c>.</value>
		private bool IsTop { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Gets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		internal static bool IsActive
		{
			get 
			{
				return currentItem != null;
			}
		}
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			if (this.IsTop)
			{
				currentItem = null;
			}
		}

		#endregion
	}
}
