using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for ExpressionInfo.
	/// </summary>
	public class ExpressionInfo
	{
		public static readonly ExpressionInfo Empty = new ExpressionInfo();

		//private string _functionName = string.Empty;
		private ArrayList _params = new ArrayList();

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpressionInfo"/> class.
		/// </summary>
		/// <param name="FunctionName">Name of the function.</param>
		/// <param name="Params">The params.</param>
		protected ExpressionInfo(params string[] Params)
		{
			//_functionName = FunctionName;

			if(Params!=null)
			{
				_params.AddRange(Params);
			}
		}

		/// <summary>
		/// Parses the specified expression.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		/// <returns></returns>
		public static ExpressionInfo Parse(string Expression)
		{
			if(Expression==string.Empty)
				return ExpressionInfo.Empty;

			ExpressionInfo retVal = new ExpressionInfo();

			//Regex regex = new Regex(@"\[(?<CellUid>[^\]]+)]", RegexOptions.Compiled);

			ArrayList arrParams = new ArrayList();

            foreach (Match match in Regex.Matches(Expression, @"\[(?<CellUid>[^\]]+)]", RegexOptions.Compiled))
			{
				string strCellUid = match.Groups["CellUid"].Value;
				retVal._params.Add(strCellUid);
			}

			return retVal;
		}

		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		/// <value>The name of the function.</value>
//		public string FunctionName
//		{
//			get 
//			{
//				return _functionName;
//			}
//		}

		/// <summary>
		/// Gets the params.
		/// </summary>
		/// <value>The params.</value>
		public string[] Params
		{
			get 
			{
				return (string[])_params.ToArray(typeof(string));
			}
		}

		/// <summary>
		/// Determines whether this instance has params.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance has params; otherwise, <c>false</c>.
		/// </returns>
		public bool HasParams()
		{
			return _params.Count>0;
		}

		/// <summary>
		/// Determines whether the specified param contains param.
		/// </summary>
		/// <param name="Param">The param.</param>
		/// <returns>
		/// 	<c>true</c> if the specified param contains param; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsParam(string Param)
		{
			return _params.Contains(Param);
		}
	}
}
