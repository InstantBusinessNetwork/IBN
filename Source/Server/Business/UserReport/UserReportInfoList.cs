using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.IBN.Business.UserReport
{
	/// <summary>
	/// Summary description for UserReportInfoList.
	/// </summary>
	public class UserReportInfoList: CollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserReportInfoList"/> class.
		/// </summary>
		public UserReportInfoList()
		{
		}

		/// <summary>
		/// Adds the specified report.
		/// </summary>
		/// <param name="report">The report.</param>
		/// <returns></returns>
		public virtual int Add(UserReportInfo report)
		{
			return this.List.Add(report);
		}

		/// <summary>
		/// Adds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="url">The URL.</param>
		/// <param name="isPersonal">if set to <c>true</c> [is personal].</param>
		/// <param name="infoType">Type of the info.</param>
		/// <returns></returns>
		public int Add(string name, string url, UserReportType type, string infoType)
		{
			return this.Add(new UserReportInfo(name, url, type, infoType));
		}

		/// <summary>
		/// Removes the specified report.
		/// </summary>
		/// <param name="report">The report.</param>
		public virtual void Remove(UserReportInfo report)
		{
			this.List.Remove(report);
		}

		/// <summary>
		/// Removes the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Remove(string name)
		{
			foreach(UserReportInfo item in this)
			{
				if(item.Name==name)
				{
					this.Remove(item);
					break;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="UserReportInfo"/> at the specified index.
		/// </summary>
		/// <value></value>
		public virtual  UserReportInfo this[int index]
		{
			get
			{
				return (UserReportInfo)this[index];
			}
		}

		/// <summary>
		/// Gets the <see cref="UserReportInfo"/> with the specified name.
		/// </summary>
		/// <value></value>
		public virtual  UserReportInfo this[string name]
		{
			get
			{
				foreach(UserReportInfo item in this)
				{
					if(item.Name==name)
						return item;
				}
				return null;
			}
		}

		/// <summary>
		/// Determines whether [contains] [the specified report].
		/// </summary>
		/// <param name="report">The report.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified report]; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool Contains(UserReportInfo report)
		{
			return this.List.Contains(report);
		}

		/// <summary>
		/// Determines whether [contains] [the specified name].
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool Contains(string name)
		{
			return this[name]!=null;
		}
	}
}
