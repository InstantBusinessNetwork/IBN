using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for DefaultAccessControlList.
	/// </summary>
	public class DefaultAccessControlList
	{
		protected class DACLItem
		{
			public string Filter = string.Empty;
			public AccessControlEntry Ace = null;

			public DACLItem(string filter, AccessControlEntry ace)
			{
				this.Filter = filter;
				this.Ace = ace;
			}
		}

		private ArrayList _items = new ArrayList();

		public DefaultAccessControlList()
		{
		}

		internal void Add(string filter, AccessControlEntry ace)
		{
			_items.Add(new DACLItem(filter, ace));
		}

		public AccessControlList GetACL(string ContainerKey)
		{
			AccessControlList retVal = AccessControlList.CreateDettachedACL();

			foreach(DACLItem item in _items)
			{
				if(PatternMatch(ContainerKey, item.Filter))
					retVal.Add(item.Ace);
			}

			return retVal;
		}

		/// <summary>
		/// Validate the source string with the specific mask.
		/// </summary>
		/// <param name="Source">The string to validate.</param>
		/// <param name="Mask">The mask expression.</param>
		/// <returns>
		/// <b>true</b> if the Source string are valid; otherwise, <b>false</b>.
		/// </returns>
		/// <remarks>
		/// The static <b>PatternMatch</b> method allow you to validate your string with the specific mask.
		/// <br/><br/>
		/// The PatternMatch mask expression allows you to use two basic character types: literal (normal) text characters and metacharacters. 
		/// Metacharacters used to ? and * characters.<br/><br/>     
		/// <list type="table">
		///		<item>
		///			<term><B>?</B></term>
		///			<description>Match any simbol</description>
		///		</item>
		///		<item>
		///			<term><B>*</B></term>
		///			<description>Zero or more matches</description>
		///		</item>
		/// </list>
		/// </remarks>
		/// <example>
		/// The following code illustrates the use of the PatternMatch method. 
		/// <code>
		/// bool bRet1 = McHttpModule.PatternMatch("text","*");		// return true
		/// bool bRet1 = McHttpModule.PatternMatch("text","*e?t");	// return true
		/// bool bRet1 = McHttpModule.PatternMatch("text","*exx");	// return false
		/// bool bRet1 = McHttpModule.PatternMatch("text","t?et");	// return true
		/// bool bRet1 = McHttpModule.PatternMatch("text","??e?");	// return false
		/// bool bRet1 = McHttpModule.PatternMatch("text","t*t*");	// return true
		/// </code>
		/// </example>
		public static bool PatternMatch(string Source, string Mask)
		{
			if(Source==null)
				throw new ArgumentNullException("Source");
			if(Mask==null)
				throw new ArgumentNullException("Source");

			if(Source==Mask)
				return true;

			char[] strSource	= new char[Source.Length + 1];
			char[] strMask		= new char[Mask.Length + 1];

			Source.CopyTo(0,strSource,0,Source.Length);
			Mask.CopyTo(0,strMask,0,Mask.Length);

			int SourceIndex = 0;
			int MaskIndex = 0;

			for(;SourceIndex<strSource.Length&&strMask[MaskIndex]!='*';MaskIndex++,SourceIndex++)
				if(strMask[MaskIndex]!=strSource[SourceIndex]&&strMask[MaskIndex]!='?')
					return false;

			int pSourceIndex = 0;
			int pMaskIndex = 0;

			for(;;)
			{
				if(strSource[SourceIndex]==0)
				{
					while(strMask[MaskIndex]=='*')
						MaskIndex++;
					return strMask[MaskIndex]==0?true:false;
				}
				if(strMask[MaskIndex]=='*')
				{
					if (strMask[++MaskIndex]==0) 
						return true;
					pMaskIndex = MaskIndex;
					pSourceIndex = SourceIndex + 1;
					continue;
				}
				if(strMask[MaskIndex]==strSource[SourceIndex]||strMask[MaskIndex]=='?')
				{
					MaskIndex++;
					SourceIndex++;
					continue;
				}
				MaskIndex=pMaskIndex; SourceIndex=pSourceIndex++;
			}
		}


	}
}
