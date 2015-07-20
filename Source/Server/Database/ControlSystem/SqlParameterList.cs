using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Represents the SqlParameter collection.
	/// </summary>
	public class SqlParameterList: CollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the SqlParameterList class.
		/// </summary>
		public SqlParameterList()
		{
		}

		/// <summary>
		/// Adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="param">The SqlParameter to be added to the SqlParameterList</param>
		/// <returns>The index at which the value has been added.</returns>
		public virtual int Add(SqlParameter	param)
		{
			return base.InnerList.Add(param);
		}

		/// <summary>
		/// Creates a new SqlParameter by parameterName and value and adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="value">An Object that is the value of the SqlParameter.</param>
		/// <returns>A new SqlParameter object.</returns>
		public virtual SqlParameter Add(string parameterName, object value)
		{
			SqlParameter	retVal	=	new SqlParameter(parameterName,value);
			base.InnerList.Add(retVal);
			return retVal;
		}

		/// <summary>
		/// Creates a new SqlParameter by parameterName and SqlDbType and adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="dbType">One of the <see cref="SqlDbType"/> values.</param>
		/// <returns>A new SqlParameter object.</returns>
		public virtual SqlParameter Add(string parameterName, SqlDbType dbType)
		{
			SqlParameter	retVal	=	new SqlParameter(parameterName,dbType);
			base.InnerList.Add(retVal);
			return retVal;
		}

		/// <summary>
		/// Creates a new SqlParameter by parameterName, SqlDbType, size and adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="dbType">One of the <see cref="SqlDbType"/> values.</param>
		/// <param name="size">The length of the parameter.</param>
		/// <returns>A new SqlParameter object.</returns>
		public virtual SqlParameter Add(string parameterName, SqlDbType dbType, int size)
		{
			SqlParameter	retVal	=	new SqlParameter(parameterName,dbType,size);
			base.InnerList.Add(retVal);
			return retVal;
		}

		/// <summary>
		/// Creates a new SqlParameter by parameterName, SqlDbType, value and adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="dbType">One of the <see cref="SqlDbType"/> values.</param>
		/// <param name="value">An Object that is the value of the SqlParameter.</param>
		/// <returns>A new SqlParameter object.</returns>
		public virtual SqlParameter Add(string parameterName, SqlDbType dbType, object value)
		{
			SqlParameter	retVal	=	new SqlParameter(parameterName,dbType);
			retVal.Value	= value;
			base.InnerList.Add(retVal);
			return retVal;
		}

		/// <summary>
		/// Creates a new SqlParameter by parameterName, SqlDbType, value and adds an SqlParameter to the SqlParameterList.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="dbType">One of the <see cref="SqlDbType"/> values.</param>
		/// <param name="size">The length of the parameter.</param>
		/// <param name="value">An Object that is the value of the SqlParameter.</param>
		/// <returns>A new SqlParameter object.</returns>
		public virtual SqlParameter Add(string parameterName, SqlDbType dbType, int size, object value)
		{
			SqlParameter	retVal	=	new SqlParameter(parameterName,dbType,size);
			retVal.Value	= value;
			base.InnerList.Add(retVal);
			return retVal;
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the SqlParameterList.
		/// </summary>
		/// <param name="param">The SqlParameter to remove.</param>
		public virtual void Remove(SqlParameter	param)
		{
			base.InnerList.Remove(param);
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		public virtual SqlParameter this[int index]
		{
			get
			{
				return (SqlParameter)base.InnerList[index];
			}
			set
			{
				base.InnerList[index]	=	value;
			}
		}

		/// <summary>
		/// Copies the elements of the SqlParameterList to a new array.
		/// </summary>
		/// <returns>An SqlParameter array containing copies of the elements of the SqlParameterList.</returns>
		public SqlParameter[]	ToArray()
		{
			return ToArray(null);
		}

		/// <summary>
		/// Copies the elements of the SqlParameterList to a new array and adds an additional params.
		/// </summary>
		/// <param name="addParams">An additioanl SqlParameter items or null (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>An SqlParameter array containing copies of the elements of the SqlParameterList.</returns>
		public virtual SqlParameter[]	ToArray(params SqlParameter[] addParams)
		{
			ArrayList	retVal	=	new ArrayList((addParams==null?0:addParams.Length) + this.Count);

			retVal.AddRange(base.InnerList);

			if(addParams!=null)
			{
				retVal.AddRange(addParams);
			}

			return (SqlParameter[])retVal.ToArray(typeof(SqlParameter));
		}
	}
}
