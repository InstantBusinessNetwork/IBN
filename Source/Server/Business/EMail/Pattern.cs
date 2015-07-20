using System;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for Pattern.
	/// </summary>
	public class Pattern
	{
		private Pattern()
		{
		}

		public static bool Match(string Source, string Mask)
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
