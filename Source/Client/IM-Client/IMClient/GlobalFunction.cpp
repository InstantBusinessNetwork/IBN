#include "StdAfx.h"
#include "OfsTv.h"
#include "GlobalFunction.h"
#include <Mmsystem.h>
#include <Mshtml.h>
#include <shlobj.h>
#include "McSettings.h"
#include <atlutil.h>

extern COfsTvApp theApp;
extern CMcSettings g_settings;

#define HIMETRIC_PER_INCH   2540
#define MAP_PIX_TO_LOGHIM(x,ppli)   ( (HIMETRIC_PER_INCH*(x) + ((ppli)>>1)) / (ppli) )
#define MAP_LOGHIM_TO_PIX(x,ppli)   ( ((ppli)*(x) + HIMETRIC_PER_INCH/2) / HIMETRIC_PER_INCH )

void HiMetricToPixel(const SIZEL *lpSizeInHiMetric, LPSIZEL lpSizeInPix)
{
	int nPixelsPerInchX;    // Pixels per logical inch along width
	int nPixelsPerInchY;    // Pixels per logical inch along height
	
	HDC hDCScreen = GetDC(NULL);
	_ASSERTE(hDCScreen != NULL);
	nPixelsPerInchX = GetDeviceCaps(hDCScreen, LOGPIXELSX);
	nPixelsPerInchY = GetDeviceCaps(hDCScreen, LOGPIXELSY);
	ReleaseDC(NULL, hDCScreen);
	
	lpSizeInPix->cx = MAP_LOGHIM_TO_PIX(lpSizeInHiMetric->cx, nPixelsPerInchX);
	lpSizeInPix->cy = MAP_LOGHIM_TO_PIX(lpSizeInHiMetric->cy, nPixelsPerInchY);
}


void UpdateMenu(CWnd *pWnd, CMenu *pMenu)
{
	CCmdUI cmdUI;
	cmdUI.m_pMenu = pMenu;
	cmdUI.m_nIndexMax = pMenu->GetMenuItemCount();
	for (cmdUI.m_nIndex = 0; cmdUI.m_nIndex < cmdUI.m_nIndexMax; ++cmdUI.m_nIndex)
	{
		CMenu* pSubMenu = pMenu->GetSubMenu(cmdUI.m_nIndex);
		if(pSubMenu==NULL)
		{
			cmdUI.m_nID = pMenu->GetMenuItemID(cmdUI.m_nIndex);
			cmdUI.DoUpdate(pWnd, FALSE);
		}
		else
		{
			UpdateMenu(pWnd, pSubMenu);
		}
		
	}
}


CString GetString(UINT nID)
{
	CString str;
	str.LoadString(nID);
	return str;
}

CString GetOptionString(UINT nSectionID, UINT nEntryID, LPCTSTR lpszDefault)
{
	return theApp.GetProfileString(GetString(nSectionID), GetString(nEntryID), lpszDefault);
}

UINT GetOptionInt(UINT nSectionID, UINT nEntryID, int nDefault)
{
	return theApp.GetProfileInt(GetString(nSectionID), GetString(nEntryID), nDefault);
}

BOOL WriteOptionString(UINT nSectionID, UINT nEntryID, LPCTSTR lpszValue)
{
	return theApp.WriteProfileString(GetString(nSectionID), GetString(nEntryID), lpszValue);
}

BOOL WriteOptionInt(UINT nSectionID, UINT nEntryID, int nValue)
{
	return theApp.WriteProfileInt(GetString(nSectionID), GetString(nEntryID), nValue);
}

_bstr_t DelSpace(_bstr_t &wstr)
{
	CString str = (char*)wstr;
	int findPos ;
	while((findPos = str.Find(_T("-"), 0)) != -1)
	{
		str=str.Mid(0,findPos)+str.Mid(findPos+1);
	}
	
	wstr = str;
	
	return wstr;
}

INT CALLBACK NEnumFontNameProc(LOGFONT *plf, TEXTMETRIC *ptm, INT nFontType, LPARAM lParam)
{
	CComboBox* comboBox = (CComboBox*) lParam;
	
	comboBox->AddString(plf->lfFaceName);
	
	return TRUE;
}

CString RectToString(const CRect &m_Rect)
{
	CString m_TmpString;
	m_TmpString.Format(_T("%ld;%ld;%ld;%ld"), m_Rect.left ,m_Rect.top ,m_Rect.right, m_Rect.bottom);
	return m_TmpString;
}

CRect   StringToRect(const CString &m_StrRect)
{
	CRect mTmpRect = CRect(0,0,0,0);
	
	char *pEnd = (char*)(LPCTSTR)m_StrRect;
	mTmpRect.left   = strtol(pEnd,&pEnd,10);
	ASSERT(pEnd!=NULL);
	if(!pEnd) return mTmpRect;
	pEnd++;
	mTmpRect.top    = strtol(pEnd,&pEnd,10);
	ASSERT(pEnd!=NULL);
	if(!pEnd) return mTmpRect;
	pEnd++;
	mTmpRect.right  = strtol(pEnd,&pEnd,10);
	ASSERT(pEnd!=NULL);
	if(!pEnd) return mTmpRect;
	pEnd++;
	mTmpRect.bottom = strtol(pEnd,&pEnd,10);
	
	return mTmpRect;
}

void Pack(CString &strPack,const CString &strPath)
{
	int Length = strPath.GetLength();
	int j = 0;
	for(int i=0;i<strPack.GetLength();i++)
	{
		strPack.SetAt(i, strPack.GetAt(i) + strPath.GetAt(j%Length));
		j++;
	}
}

void    UnPack(CString &strPack,const CString &strPath)
{
	int Length = strPath.GetLength();
	int j = 0;
	for(int i=0;i<strPack.GetLength();i++)
	{
		strPack.SetAt(i, strPack.GetAt(i) - strPath.GetAt(j%Length));
		j++;
	}	
}

int Replace(BSTR &lpszData, LPCWSTR lpszOld, LPCWSTR lpszNew)
{
	if(SysStringLen(lpszData)==0)
		return 0;
	
	// can't have empty or NULL lpszOld
	int nSourceLen = wcslen(lpszOld);
	if (nSourceLen == 0)
		return 0;
	int nReplacementLen = wcslen(lpszNew);
	
	// loop once to figure out the size of the result string
	int nCount = 0;
	LPWSTR lpszStart = lpszData;
	LPWSTR lpszEnd = lpszData + nSourceLen;
	LPWSTR lpszTarget;
	while (lpszStart < lpszEnd)
	{
		while ((lpszTarget = wcsstr(lpszStart, lpszOld)) != NULL)
		{
			nCount++;
			lpszStart = lpszTarget + nSourceLen;
		}
		lpszStart += wcslen(lpszStart) + 1;
	}
	
	// if any changes were made, make them
	if (nCount > 0)
	{
		// if the buffer is too small, just
		//   allocate a new buffer (slow but sure)
		int nOldLength = SysStringLen(lpszData);
		int nNewLength =  nOldLength + (nReplacementLen-nSourceLen)*nCount;
		if (nOldLength < nNewLength)
		{
			LPWSTR pstr = lpszData;
			lpszData = SysAllocStringLen(pstr,nNewLength);
			SysFreeString(BSTR(pstr));
		}

		// else, we just do it in-place
		lpszStart = lpszData;
		lpszEnd = lpszData + nNewLength;
		
		// loop again to actually do the work
		if(lpszStart < lpszEnd)
		{
			while (lpszStart < lpszEnd)
			{
				while ( (lpszTarget = wcsstr(lpszStart, lpszOld)) != NULL)
				{
					int nBalance = nOldLength - (lpszTarget - lpszData + nSourceLen);
					memmove(lpszTarget + nReplacementLen, lpszTarget + nSourceLen,
						nBalance * sizeof(WCHAR));
					memcpy(lpszTarget, lpszNew, nReplacementLen*sizeof(WCHAR));
					lpszStart = lpszTarget + nReplacementLen;
					lpszStart[nBalance] = '\0';
					nOldLength += (nReplacementLen - nSourceLen);
				}
				lpszStart += wcslen(lpszStart) + 1;
			}
			ASSERT(lpszData[nNewLength] == '\0');
		}
		else
		{
			SysFreeString(lpszData);
			lpszData = SysAllocString(L"");
		}
		
	}
	
	return nCount;
}

class ParserState
{
public:
	BOOL		IsTag;
	BOOL		IsSingleTag;
	BOOL		IsClosedTag;

	CComBSTR	TagName;
	CComBSTR	Text;

	ParserState()
	{
		ParserState(CComBSTR(L""));
	}

	ParserState(const ParserState&  src)
	{
		*this = src;
	}
	
	const ParserState& operator=(const ParserState&  src)
	{
		IsTag			=	src.IsTag;
		IsSingleTag		=	src.IsSingleTag;
		IsClosedTag		=	src.IsClosedTag;
		TagName			=	src.TagName;
		Text			=	src.Text;
		return *this;
	}

	ParserState(CComBSTR strText)
	{
		Text = strText;

		IsTag = FALSE;
		IsClosedTag = FALSE;
		IsSingleTag = FALSE;

		if(Text.Length()>0)
		{
			BSTR	tmpVariable	=	(BSTR)Text;

			IsTag = tmpVariable[0]==L'<';
			IsClosedTag = tmpVariable[1]==L'/';

			if(!IsClosedTag)
				IsSingleTag = tmpVariable[Text.Length()-2]==L'/';

			if(IsTag)
			{
				for(unsigned int Index = (IsClosedTag?2:1);Index<Text.Length();Index++)
				{
					if(tmpVariable[Index]==' '||
						tmpVariable[Index]=='/'||
						tmpVariable[Index]=='>')
						break;
					
					TagName	+= CComBSTR(1,tmpVariable+Index);
				}

				if(TagName==L"BR"||TagName==L"IMG")
					IsClosedTag	=	TRUE;
			}
		}
	}
};

BOOL IsClosedTag(CArray<ParserState,ParserState>	&ParserStack, int &StopPos, int CheckTagIndex)
{
	ParserState	stateToCheck = ParserStack[CheckTagIndex];

	for(int Index = CheckTagIndex+1; Index<=StopPos;Index++)
	{
		ParserState	state = ParserStack[Index];

		if(state.IsTag&&state.TagName==stateToCheck.TagName)
		{
			if(!state.IsClosedTag)
			{
				int TmpStopPos	=	StopPos	;
				BOOL bTmpVal = IsClosedTag(ParserStack,TmpStopPos,Index);

				if(!bTmpVal)
					return FALSE;

				Index	=	TmpStopPos;
			}
			else
				return TRUE;
		}
	}

	return FALSE;
}

CComBSTR SplitHTML(unsigned int MaxLen, CComBSTR &bsText)
{
	USES_CONVERSION;

	CComBSTR bsRetVal = L"";

	/*if(bsText.Length()<MaxLen)
	{
		bsRetVal = bsText;
		bsText.Empty();
		return bsRetVal;
	}*/

	CArray<ParserState,ParserState>	ParserStack;
	int							State			=	0;
	CComBSTR					currentValue;
	WCHAR						Sim[3]			=	L"";

	for(unsigned int Index = 0; Index<bsText.Length();Index++)
	{
		Sim[0]	=	((BSTR)bsText)[Index];

		switch(State)
		{
			case 0: // Text
				if(Sim[0]==L'<')
				{
					State = 1;
					if(currentValue.Length()>0)
					{
						if(currentValue.Length()>MaxLen)
						{
							int Div = MaxLen/2;

							unsigned int StringLen = currentValue.Length();
							WCHAR* BeginPos = (BSTR)currentValue;

							int AddOnSpace = 0;
							for(unsigned int Pos=0;Pos<currentValue.Length();Pos+=(Div+AddOnSpace))
							{
								AddOnSpace = 0;
								for(int SpaceIndex = 0; SpaceIndex<(Div/4); SpaceIndex++ )
								{
									if(*(BeginPos+Pos+Div-SpaceIndex)==L' '&&Pos!=(StringLen-1))
									{
										AddOnSpace = -(SpaceIndex-1);
										break;
									}
								}

								CComBSTR tmpCurrValue = CComBSTR(min(Div+AddOnSpace,StringLen-Pos),BeginPos+Pos);

								ParserStack.Add(ParserState(tmpCurrValue));
							}
						}
						else
							ParserStack.Add(ParserState(currentValue));
						currentValue.Empty();
					}
				}
				currentValue	+=	Sim;
				break;
			case 1: // Node
				if(Sim[0]==L'>')
				{
					State = 0;
					currentValue	+=	Sim;
					ParserStack.Add(ParserState(currentValue));
					currentValue.Empty();
				}
				else
					currentValue	+=	Sim;
				break;
		}
	}

	// Add Final Item [9/24/2004]
	if(currentValue.Length()>0)
	{
		ParserStack.Add(ParserState(currentValue));
		currentValue.Empty();
	}
	
	// Remove Empty tags [9/24/2004]
	BOOL bFoundReplace = FALSE;

	do 
	{
		bFoundReplace = FALSE;
		
		for(int ParsedItemIndex = ParserStack.GetSize()-1; ParsedItemIndex>=0; ParsedItemIndex--)
		{
			ParserState	current =  ParserStack.GetAt(ParsedItemIndex);

			if(!current.IsTag)
			{
				CComBSTR bsSpaceTest = current.Text;

				Replace(bsSpaceTest.m_str,L"\r\n",L"");
				Replace(bsSpaceTest.m_str,L" ",L"");
				Replace(bsSpaceTest.m_str,L"&nbsp;",L"");
				
				if(wcslen(bsSpaceTest)==0)
				{
					ParserStack.RemoveAt(ParsedItemIndex);
					continue;
				}
			}

			if(current.IsTag&&!current.IsClosedTag&&!current.IsSingleTag)
			{
				if((ParsedItemIndex+1)<ParserStack.GetSize())
				{
					ParserState	next =  ParserStack.GetAt(ParsedItemIndex+1);	
					if(next.IsTag&&next.IsClosedTag&&!next.IsSingleTag&&
						current.TagName==next.TagName&&
						!(current.TagName==CComBSTR(L"P")))
					{
						// Remove Tags [9/24/2004]
						ParserStack.RemoveAt(ParsedItemIndex);
						ParserStack.RemoveAt(ParsedItemIndex);

						bFoundReplace = TRUE;

						continue;
					}
				}
				
				if((ParsedItemIndex+2)<ParserStack.GetSize())
				{
					ParserState	next1 =  ParserStack.GetAt(ParsedItemIndex+1);	
					ParserState	next2 =  ParserStack.GetAt(ParsedItemIndex+2);	
					
					if(next2.IsTag&&next2.IsClosedTag&&!next2.IsSingleTag&&
						current.TagName==next2.TagName&&!next1.IsTag&&next1.Text==L"&nbsp;")
					{
						// Remove Tags [9/24/2004]
						ParserStack.RemoveAt(ParsedItemIndex);
						ParserStack.RemoveAt(ParsedItemIndex);
						ParserStack.RemoveAt(ParsedItemIndex);

						bFoundReplace = TRUE;
						
						continue;
					}
				}
			}
		}
	} 
	while(bFoundReplace);

	bsText.Empty();

	BOOL bTextFound = FALSE;

	for(int ParsedItemIndex = 0; ParsedItemIndex<ParserStack.GetSize(); ParsedItemIndex++)
	{
		if(!ParserStack.GetAt(ParsedItemIndex).IsTag)
			bTextFound = TRUE;

		bsRetVal	+=	ParserStack.GetAt(ParsedItemIndex).Text;

		if(bsRetVal.Length()>(MaxLen))
		{
			// Close Open Tags
			for(int BackIndex = ParsedItemIndex; BackIndex>=0;BackIndex--)
			{
				ParserState	stackItem = ParserStack.GetAt(BackIndex);

				if(stackItem.IsTag && !stackItem.IsSingleTag && !stackItem.IsClosedTag)
				{
					if(!IsClosedTag(ParserStack,ParsedItemIndex,BackIndex))
					{
						bsRetVal	+=	L"</";
						bsRetVal	+=	stackItem.TagName;
						bsRetVal	+=	">";

						CComBSTR	tmpVal = stackItem.Text;
						tmpVal += bsText;
						bsText		=	tmpVal;
					}
				}
			}

			// Append Open Tags
			for(int NewIndex = ParsedItemIndex+1; NewIndex<ParserStack.GetSize();NewIndex++)
			{
				bsText	+=	ParserStack.GetAt(NewIndex).Text;
			}

			break;
		}
	}

	///TRACE("-------------------\r\n%s\r\n",W2CT(bsRetVal));

	return bTextFound?bsRetVal:CComBSTR(L"");
}

void RemoveParagraf(BSTR &string)
{
	// Old Code [10/7/2002]
	Replace(string,L"<P>",L"");
	Replace(string,L"</P>",L"<BR>");

	Replace(string,L"<STRONG>",L"<B>");
	Replace(string,L"</STRONG>",L"</B>");

	Replace(string,L"<EM>",L"<I>");
	Replace(string,L"</EM>",L"</I>");

	return;
	
	/*// Remove Empty Tag [10/7/2002]
	LPWSTR srcFreeTagStr = NULL;
	UINT stringLen		=	0;

	LPCWSTR strEmptyShablon[10] =	{L"></FONT>",L"<BR>",L"\r\n",L"&nbsp;",L">&nbsp;</FONT>"};
	//size_t	szEmptyShablon	=	wcslen(strEmptyShablon);

	if((stringLen = SysStringLen(string))>0)
	{
		srcFreeTagStr	=	new WCHAR[stringLen+1];
		wcscpy(srcFreeTagStr,string);

		stringLen	=	wcslen(string);

		while(true)
		{
			// Step 3. Check \r\n is end ???
			LPWSTR lpszStartDel	=	srcFreeTagStr+stringLen-1;
			
			if(lpszStartDel<srcFreeTagStr)
				break;
			
			if(*lpszStartDel<33)
			{
				while(*lpszStartDel<33)
				{
					*lpszStartDel = NULL;				
					lpszStartDel--;
				}
				
				stringLen = wcslen(srcFreeTagStr);
				
				continue;
			}
			
			// Step 1. Check ></FONT> is end ???
			lpszStartDel	=	srcFreeTagStr+stringLen-wcslen(strEmptyShablon[0]);

			if(lpszStartDel<srcFreeTagStr)
				break;

			if(wcscmp(lpszStartDel,strEmptyShablon[0])==0L)
			{
				while(lpszStartDel>=srcFreeTagStr&&*lpszStartDel!=L'<')
					lpszStartDel--;
				
				*lpszStartDel = NULL;
				
				stringLen = wcslen(srcFreeTagStr);

				continue;
			}
			// Step 2. Check &nbsp;<BR/> is end ???

			lpszStartDel	=	srcFreeTagStr+stringLen-wcslen(strEmptyShablon[1]);
			
			if(lpszStartDel<srcFreeTagStr)
				break;
			
			if(wcscmp(lpszStartDel,strEmptyShablon[1])==0L)
			{
				*lpszStartDel = NULL;

				stringLen = wcslen(srcFreeTagStr);
				
				continue;
			}
			

			// Step 4. Check &nbsp; is end ???
			lpszStartDel	=	srcFreeTagStr+stringLen-wcslen(strEmptyShablon[3]);

			if(lpszStartDel<srcFreeTagStr)
				break;
			
			if(wcscmp(lpszStartDel,strEmptyShablon[3])==0L)
			{
				*lpszStartDel = NULL;				
				
				stringLen = wcslen(srcFreeTagStr);
				
				continue;
			}

			// Step 1. Check >&nbsp</FONT> is end ???
			lpszStartDel	=	srcFreeTagStr+stringLen-wcslen(strEmptyShablon[4]);

			if(lpszStartDel<srcFreeTagStr)
				break;
			
			if(wcscmp(lpszStartDel,strEmptyShablon[4])==0L)
			{
				while(lpszStartDel>=srcFreeTagStr&&*lpszStartDel!=L'<')
					lpszStartDel--;
				
				*lpszStartDel = NULL;
				
				stringLen = wcslen(srcFreeTagStr);
				
				continue;
			}
			
			
			// Just Exit [10/7/2002]
			break;
		}

		SysFreeString(string);
		string = SysAllocString(srcFreeTagStr);

		delete [] srcFreeTagStr;
	}*/
}

void RoundExitAddon(CWnd *m_hWnd,int Step /*= 10*/,DWORD dwTime/* = 25*/)
{
	if(!IsWindow(m_hWnd->GetSafeHwnd())) return;
	
	CRect m_Ret;
	m_hWnd->GetWindowRect(&m_Ret);
	CRgn m_Rgn;	m_Rgn.CreateEllipticRgn(0,0,0,0);
	
	int imin = min(m_Ret.Width(),m_Ret.Height());
	int x0   = m_Ret.Width()/2;
	int y0   = m_Ret.Height()/2;
	int h    = imin/(2*Step);
	
	for(int i = Step;i>0;i--)
	{
		m_Rgn.DeleteObject();
		m_Rgn.CreateEllipticRgn(x0-i*h,y0-i*h,x0+i*h,y0+i*h);
		m_hWnd->SetWindowRgn(m_Rgn,TRUE);
		m_hWnd->UpdateWindow();
		Sleep(dwTime);
	}
}

void FitRectToWindow(CRect &r)
{
	// Solve multi monitor problem
	return;

	RECT rd;
	SystemParametersInfo(SPI_GETWORKAREA, 0, &rd, 0);
	
	// first check right and bottom edges
	if(r.right > rd.right)
		r.OffsetRect(rd.right - r.right, 0);
	if(r.bottom > rd.bottom)
		r.OffsetRect(0, rd.bottom - r.bottom);
	
	// then check left and top edges
	if(r.left < rd.left)
		r.OffsetRect(rd.left - r.left, 0);
	if(r.top < rd.top)
		r.OffsetRect(0, rd.top - r.top);
}
/*
void	FitRectToWindow(CRect &ModRect)
{
	RECT rd;
	SystemParametersInfo(SPI_GETWORKAREA, 0, &rd, 0);
	long WindowsCX = rd.right - rd.left;
	long WindowsCY = rd.bottom - rd.top;
//	long WindowsCX = GetSystemMetrics(SM_CXFULLSCREEN);
//	long WindowsCY = GetSystemMetrics(SM_CYFULLSCREEN);
	if(ModRect.left<0)
	{
		ModRect.right += -ModRect.left + 10;
		ModRect.left = 10;
	}
	if(ModRect.top<0)	
	{
		ModRect.bottom += -ModRect.top +10;
		ModRect.top = 10;
	}
	if(ModRect.right>WindowsCX)	
	{
		ModRect.left -= ModRect.right-WindowsCX + 10;
		ModRect.right	= WindowsCX-10;
	}
	if(ModRect.bottom>WindowsCY)	
	{
		ModRect.top	 -= ModRect.bottom-WindowsCY + 10;
		ModRect.bottom	= WindowsCY-10;
	}
	if(ModRect.left<0)	ModRect.left = 10;
	if(ModRect.top<0)	ModRect.top = 10;
	if(ModRect.right>WindowsCX)	ModRect.right = WindowsCX-10;
	if(ModRect.bottom>WindowsCY)	ModRect.bottom = WindowsCY-10;
}
*/

HRESULT LoadDATAFromStream(IN IStream* pDataStream,OUT BYTE **pBuffer, OUT DWORD *pSize)
{
	if(pDataStream==NULL||pBuffer==NULL||pSize==NULL)
		return E_INVALIDARG;
	
	HRESULT hr = S_OK;
	
	try
	{
		IStreamPtr pTMPStream = NULL;
		CreateStreamOnHGlobal(NULL,TRUE,&pTMPStream);
		
		const ULONG Read = 1000;
		ULONG pRealyRead = 0, pRealyWrite = 0;
		BYTE pRead[Read+1];
		
		do
		{
			pDataStream->Read((LPVOID)pRead,Read,&pRealyRead);
			pTMPStream->Write((LPVOID)pRead,pRealyRead,&pRealyWrite);
			*pSize += pRealyWrite;
		}
		while(Read==pRealyRead);
		
		LARGE_INTEGER lI = {0,0};
		ULARGE_INTEGER	uI;
		pTMPStream->Seek(lI,STREAM_SEEK_SET,&uI);
		
		*pBuffer = (BYTE*)GlobalAlloc(GPTR,*pSize);
		
		pTMPStream->Read((void*)*pBuffer,*pSize,&pRealyRead);
		
	}
	catch(_com_error &e)
	{	
		hr = e.Error();
	}
	
	return hr;
}

void TestDirAndCreateDir(LPCTSTR Path)
{
	CString strPath = Path, strTmp;
	while(TRUE)
	{
		int Index = strPath.Find('\\');
		if(Index==-1) 
			break;
		strTmp += strPath.Left(Index+1);
		CreateDirectory(strTmp,NULL);
		strPath = strPath.Mid(Index+1);
	}
}

HIMAGELIST GetSystemImageList(BOOL fSmall)
{
	USES_CONVERSION;

	HIMAGELIST  himl;
	SHFILEINFO  sfi	=	{0};
	
	himl = (HIMAGELIST)SHGetFileInfo(TEXT(".txt"), FILE_ATTRIBUTE_NORMAL, &sfi,	sizeof(SHFILEINFO), SHGFI_USEFILEATTRIBUTES|SHGFI_SYSICONINDEX |(fSmall ? SHGFI_SMALLICON : SHGFI_LARGEICON));	
	
	return himl;
} 

int GetIconIndexInSystemImageList(BOOL fSmall, LPCTSTR Path)
{
	SHFILEINFO shfi;
	memset(&shfi,0,sizeof(shfi));
	SHGetFileInfo(Path, FILE_ATTRIBUTE_NORMAL,&shfi, sizeof(shfi),SHGFI_SYSICONINDEX|SHGFI_USEFILEATTRIBUTES|(fSmall ? SHGFI_SMALLICON : SHGFI_LARGEICON));
	return 	shfi.iIcon;
}

BOOL IsTVFileType(BSTR URL)
{
	// Determine MIME type
	BOOL bResult = FALSE;
	HRESULT hr;
	LPWSTR pStrMimeType = NULL;
	
	hr = FindMimeFromData(NULL, URL, NULL, 0, NULL, 0, &pStrMimeType, 0);
	if(hr == NOERROR)
	{
		if(0 == wcsicmp(pStrMimeType, L"application/x-shockwave-flash"))
			return TRUE;
		
		if(0 == wcsicmp(pStrMimeType, L"audio/wav"))
			return TRUE;
		if(0 == wcsicmp(pStrMimeType, L"audio/mpeg"))
			return TRUE;
		
		if(0 == wcsicmp(pStrMimeType, L"video/avi"))
			return TRUE;
		if(0 == wcsicmp(pStrMimeType, L"video/mpeg"))
			return TRUE;
		if(0 == wcsicmp(pStrMimeType, L"video/x-ms-asf"))
			return TRUE;
	}
	
	return bResult;
}

HRESULT GetTargetRect(LPDISPATCH pDisp, long &cx, long &cy, long &tearoff, long &fullscreen)
{
	HRESULT hr = E_FAIL;
	
	CComPtr<IWebBrowser2> pBrowser = NULL;
	CComPtr<IHTMLDocument2> pDoc = NULL;
	CComPtr<IDispatch> pDispDoc = NULL;
	CComPtr<IHTMLElement> pEle = NULL;
	CComVariant value;
	WCHAR* szNULL = L"\0";
	
	hr = pDisp->QueryInterface(__uuidof(IWebBrowser2), (void**)&pBrowser);
	if(pBrowser == NULL) return hr;
	hr = pBrowser->get_Document(&pDispDoc);
	if(pDispDoc == NULL) return hr;
	hr = pDispDoc->QueryInterface(__uuidof(IHTMLDocument2), (void**)&pDoc);
	if(pDoc == NULL) return hr;
	
	// when document is loaded
	hr = pDoc->get_activeElement(&pEle);
	if(pEle == NULL) return hr;
	
	pEle->getAttribute(CComBSTR(L"tearoff"), 0, &value);
	if(value.vt == VT_BSTR && value.bstrVal != NULL)
		tearoff = wcstol(value.bstrVal, &szNULL, 10);
	pEle->getAttribute(CComBSTR(L"height"), 0, &value);
	if(value.vt == VT_BSTR && value.bstrVal != NULL)
		cy = wcstol(value.bstrVal, &szNULL, 10);
	pEle->getAttribute(CComBSTR(L"width"), 0, &value);
	if(value.vt == VT_BSTR && value.bstrVal != NULL)
	{
		if(tearoff > 0 && 0 == wcscmp(value.bstrVal, L"100%"))
		{
			fullscreen = 1;
			cx = 400;
			cy = 300;
		}
		else
			cx = wcstol(value.bstrVal, &szNULL, 10);
	}
	
	return hr;
}

CString GetProductLanguage()
{
	CString strRetVal;


#ifndef RADIUS
	if(!McRegGetString(HKEY_CURRENT_USER,_T("Software\\Mediachase\\Instant Business Network\\4.5\\ClientTools"),_T("Language"), strRetVal,_T("1033")))
	{
		McRegGetString(HKEY_LOCAL_MACHINE,_T("Software\\Mediachase\\Instant Business Network\\4.5\\ClientTools"),_T("Language"), strRetVal,_T("1033"));
	}
#else
	if(!McRegGetString(HKEY_CURRENT_USER,_T("Software\\Radius-Soft\\MagRul\\4.5\\ClientTools"),_T("Language"), strRetVal,_T("1033")))
	{
		McRegGetString(HKEY_LOCAL_MACHINE,_T("Software\\Radius-Soft\\MagRul\\4.5\\ClientTools"),_T("Language"), strRetVal,_T("1033"));
	}
#endif

	return strRetVal;
}

CString GetProductPath()
{
	CString strRetVal;

#ifndef RADIUS
	if(!McRegGetString(HKEY_CURRENT_USER,_T("Software\\Mediachase\\Instant Business Network\\4.5\\ClientTools"),_T("Path"), strRetVal,_T("")))
	{
		McRegGetString(HKEY_LOCAL_MACHINE,_T("Software\\Mediachase\\Instant Business Network\\4.5\\ClientTools"),_T("Path"), strRetVal,_T(""));
	}
#else
	if(!McRegGetString(HKEY_CURRENT_USER,_T("Software\\Radius-Soft\\MagRul\\4.5\\ClientTools"),_T("Path"), strRetVal,_T("")))
	{
		McRegGetString(HKEY_LOCAL_MACHINE,_T("Software\\Radius-Soft\\MagRul\\4.5\\ClientTools"),_T("Path"), strRetVal,_T(""));
	}
#endif

	return strRetVal;
}

CComPtr<IXMLDOMNode> AppendWithSort(IXMLDOMNode* parentNode, IXMLDOMNode* newChildNode, BSTR bstCmpAttributeName)
{
	CComPtr<IXMLDOMNode> retValNode;

	VARIANT_BOOL hasChildNodes = VARIANT_FALSE;

	HRESULT hr = parentNode->hasChildNodes(&hasChildNodes);

	if(hasChildNodes==VARIANT_TRUE)
	{
		CComVariant	varNewItemText;
		GetAttribute(newChildNode,bstCmpAttributeName,&varNewItemText);				
		varNewItemText.ChangeType(VT_BSTR);
		CComBSTR bstNewItemTex = varNewItemText.bstrVal;

		CComPtr<IXMLDOMNodeList> childNodes;
		hr = parentNode->get_childNodes(&childNodes);

		long lCount;
		hr = childNodes->get_length(&lCount);

		for(int index=0;index<lCount;index++)
		{
			CComPtr<IXMLDOMNode> childNode;
			hr = childNodes->get_item(index, &childNode);

			CComVariant	varItemText;
			GetAttribute(childNode,bstCmpAttributeName,&varItemText);				
			varItemText.ChangeType(VT_BSTR);
			CComBSTR bstItemTex = varItemText.bstrVal;

			if(bstNewItemTex<bstItemTex)
			{
				CComVariant         varRef;
				varRef = childNode;
				hr = parentNode->insertBefore(newChildNode, varRef, &retValNode);
				break;
			}
		}

		if(retValNode==NULL)
			hr = parentNode->appendChild(newChildNode, &retValNode);

	}
	else
	{
		hr = parentNode->appendChild(newChildNode, &retValNode);
	}


	return retValNode;
}



BOOL McPlaySound(UINT SoundId)
{
//	return PlaySound(GetOptionString(IDS_SOUND,SoundId,_T("")),NULL,SND_FILENAME|SND_ASYNC|SND_NODEFAULT);
	CString str;
	if(g_settings.GetString(IDS_SOUND, SoundId, str, _T("")))
	{
		if(str.GetLength()!=0)
		{
			DWORD dwFileAtr = GetFileAttributes(str);
			
			if(dwFileAtr==INVALID_FILE_ATTRIBUTES)
			{
				g_settings.SetStringFromLocal2Current(IDS_SOUND, SoundId);
				g_settings.GetString(IDS_SOUND, SoundId, str, _T(""));
			}
			
			return PlaySound(str, NULL, SND_FILENAME|SND_ASYNC|SND_NODEFAULT);
		}
	}

	return FALSE;
}

CMap<CWnd*, CWnd*, BOOL, BOOL> g_mapOpenedWindows;
void AddWindowToClose(CWnd *pWnd)
{
	g_mapOpenedWindows.SetAt(pWnd, TRUE);
}

void RemoveWindowToClose(CWnd *pWnd)
{
	g_mapOpenedWindows.RemoveKey(pWnd);
}

void CloseAllWindows()
{
	CWnd *pWnd = NULL;
	BOOL val;
	POSITION pos = g_mapOpenedWindows.GetStartPosition();
	while(pos != NULL)
	{
		g_mapOpenedWindows.GetNextAssoc(pos, pWnd, val);
		if(!IsBadReadPtr(pWnd,sizeof(CWnd)))
		{
			if(IsWindow(pWnd->GetSafeHwnd()))
				pWnd->SendMessage(WM_CLOSE);
		}
	}
	g_mapOpenedWindows.RemoveAll();
}

HRESULT	Array2Stream(VARIANT varData, IStream** ppDataStream)
{
	if(ppDataStream==NULL)
		return E_INVALIDARG;
	
	HRESULT hr = E_FAIL;
	
	if(varData.vt&VT_ARRAY)
	{
		SAFEARRAY *pSafeArray = varData.parray;
		
		void HUGEP *pData	=	NULL;
		
		hr = SafeArrayAccessData(pSafeArray,&pData);
		
		if(SUCCEEDED(hr))
		{
			ULONG pDataRead	=	0;
			ULONG pDataPos	=	0;
			
			CreateStreamOnHGlobal(NULL,TRUE,ppDataStream);
			
			HRESULT hr = (*ppDataStream)->Write(pData,pSafeArray->rgsabound[0].cElements,&pDataRead);
			if(SUCCEEDED(hr))
			{
				LARGE_INTEGER lPos = {0};
				ULARGE_INTEGER lNowPos;
				
				hr = (*ppDataStream)->Seek(lPos,STREAM_SEEK_SET,&lNowPos);
			}
			
			SafeArrayUnaccessData(pSafeArray);
		}
	}
	
	return hr;
}

CString	GetAppDataDir()
{
	static	CString		strRetValue;
	
	if(!strRetValue.IsEmpty())
		return strRetValue;
	
	TCHAR		pBuff[MAX_PATH];	

	// Oleg Addon
	CRegKey	AppDataKey;
	if(AppDataKey.Open(HKEY_CURRENT_USER,_T("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"),KEY_READ)==ERROR_SUCCESS)
	{
		DWORD BuffSize	=	MAX_PATH;
		
		if(AppDataKey.QueryValue(pBuff,_T("AppData"),&BuffSize)==ERROR_SUCCESS)
		{
			for(int Step = 1; Step<4; Step++)
			{
				switch(Step)
				{
				case 1:
					strRetValue = pBuff;
					break;
				case 2:
					strRetValue += _T("\\");
					strRetValue += GetString(IDS_REGISTRY_KEY);
					break;
				case 3:
					strRetValue += _T("\\");
					strRetValue += GetString(IDR_MAINFRAME);
					break;
				}
				
				CreateDirectory(strRetValue,NULL);
			}
		}
	}
	return strRetValue;
}

CString	GetRegFileText(LPCTSTR lpszSection, LPCTSTR lpszEntry)
{
	CString strRetData;
	CString strFileName = theApp.GetProfileString(lpszSection, lpszEntry, NULL);
	if(!strFileName.IsEmpty())
	{
		HANDLE	hFile	=	CreateFile(strFileName,GENERIC_READ,FILE_SHARE_READ|FILE_SHARE_WRITE,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,NULL);
		if(hFile!= INVALID_HANDLE_VALUE)
		{
			DWORD dwFileSize	=	0;
			dwFileSize = GetFileSize(hFile, NULL);
			LPBYTE pData = new BYTE[dwFileSize+2];
			if(pData != NULL)
			{
				memset(pData, 0, dwFileSize+2);
				DWORD	dwRealRead	=	0;
				ReadFile(hFile, reinterpret_cast<LPVOID>(pData), dwFileSize, &dwRealRead, NULL);
				if(dwRealRead >= 2)
				{
					USES_CONVERSION;
					LPTSTR szData;
					WORD byteOrderIndicator = *reinterpret_cast<LPWORD>(pData);
					if(byteOrderIndicator == 0xFFFE)
						szData = W2T(reinterpret_cast<LPWSTR>(pData+2));
					else
						szData = A2T(reinterpret_cast<LPSTR>(pData));
					DWORD dwDataSize = _tcslen(szData);
					LPTSTR szBuff = strRetData.GetBuffer(dwDataSize+1);
					_tcscpy(szBuff, szData);
					strRetData.ReleaseBuffer();
				}
				delete[] pData;
			}
			CloseHandle(hFile);
		}
		else
		{
			theApp.WriteProfileString(lpszSection,lpszEntry,NULL);
		}
	}
	
	return strRetData;
}

BOOL	SetRegFileText(LPCTSTR lpszSection, LPCTSTR lpszEntry, LPCTSTR pData)
{
	BOOL bRetVal	=	FALSE;
	if(pData==NULL||IsBadStringPtr(pData,MAX_PATH))
		return bRetVal;
	
	CString strFileName = theApp.GetProfileString(lpszSection,lpszEntry,NULL);
	if(strFileName.IsEmpty())
	{
		CString	strDataDirPath	=	GetAppDataDir();
		LPTSTR	Buf = strFileName.GetBuffer(MAX_PATH);
		GetTempFileName(strDataDirPath,_T("McData"),0,Buf);
		strFileName.ReleaseBuffer();
		theApp.WriteProfileString(lpszSection,lpszEntry,strFileName);
	}
	
	if(!strFileName.IsEmpty())
	{
		HANDLE	hFile	=	CreateFile(strFileName,GENERIC_WRITE,FILE_SHARE_READ,NULL,CREATE_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);
		
		if(hFile!= INVALID_HANDLE_VALUE)
		{
			USES_CONVERSION;
			LPWSTR wszData = T2W(const_cast<LPTSTR>(pData));
			DWORD	dwBuffLen =	wcslen(wszData)*sizeof(WCHAR);
			DWORD	dwWritten	=	0;
			WORD byteOrderIndicator = 0xFFFE;
			WriteFile(hFile,(LPVOID)&byteOrderIndicator, 2, &dwWritten, NULL);
			WriteFile(hFile,(LPVOID)wszData, dwBuffLen, &dwWritten, NULL);
			SetEndOfFile(hFile);
			CloseHandle(hFile);
			bRetVal = TRUE;
		}
	}
	
	return bRetVal;
}


CString	ByteSizeToStr(DWORD dwSize)
{
	CString	strRet;
	
	if(dwSize<512)
		strRet.Format(GetString(IDS_DATA_BYTE_FORMAT),dwSize);
	else
		if(dwSize<1000*1024)
			strRet.Format(GetString(IDS_DATA_KBYTE_FORMAT),dwSize/1024.0);
		else
			strRet.Format(GetString(IDS_DATA_MBYTE_FORMAT),dwSize/(1024.0*1000));
		
		return strRet;
}

CString	GetMyDocumetPath(LPCTSTR UserRoleName, int UserId)
{
	CString	strUserId;
	strUserId.Format(_T("%d"), UserId);
				
	CString strMyDocumentPath	=	theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + UserRoleName+_T("\\")+strUserId,GetString(IDS_RECEIVED_FILES));
	if(strMyDocumentPath.IsEmpty())
	{
		CRegKey	MyDocReg;
		if(MyDocReg.Open(HKEY_CURRENT_USER,_T("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"),KEY_READ)==ERROR_SUCCESS)
		{
			TCHAR	strBuff[MAX_PATH];
			DWORD	dwBuffSize	=	MAX_PATH;
			int Val;
			if((Val=MyDocReg.QueryValue(strBuff,_T("Personal"),&dwBuffSize))==ERROR_SUCCESS)
			{
				CString StrFormat = _T("%s\\");
				StrFormat	+=	GetString(IDS_IBN_RECEIVED_FILE_NAME);
				strMyDocumentPath.Format(StrFormat,strBuff);
				CreateDirectory(strMyDocumentPath,NULL);
			}
		}
	}
	return strMyDocumentPath;
}

BOOL	SetMyDocumetPath(LPCTSTR UserRoleName, int UserId, LPCTSTR Path)
{
	CString	strUserId;
	strUserId.Format(_T("%d"), UserId);
				
	return	theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + UserRoleName+_T("\\")+strUserId,GetString(IDS_RECEIVED_FILES),Path);
}

int ShowContextMenu(long TreeAddon, HWND hWnd, LPCTSTR pszPath, int x, int y, LPCTSTR stVer, IContextMenu2 **pContextMenu2,IContextMenu3 **pContextMenu3)
{
	USES_CONVERSION;
	
	// Строим полное имя файла/каталога
	TCHAR tchFullPath[10000];
	GetFullPathName(pszPath, sizeof(tchFullPath)/sizeof(TCHAR), tchFullPath, NULL);
	
	// Получаем интерфейс IShellFolder рабочего стола
	IShellFolder *pDesktopFolder;
	SHGetDesktopFolder(&pDesktopFolder);
	
	// Преобразуем заданный путь в LPITEMIDLIST
	LPITEMIDLIST pidl;
	pDesktopFolder->ParseDisplayName(hWnd, NULL, T2OLE(tchFullPath), NULL, &pidl, NULL);
	
	// Ищем последний идентификатор в полученном списке pidl
	LPITEMIDLIST pLastId = pidl;
	
	if(pLastId==NULL)
		return 0;
	
	USHORT temp;
	while(1)
	{
		int offset = pLastId->mkid.cb;
		temp = *(USHORT*)((BYTE*)pLastId + offset);
		
		if(temp == 0)
			break;
		
		pLastId = (LPITEMIDLIST)((BYTE*)pLastId + offset);
	}
	
	// Получаем интерфейс IShellFolder родительского объекта для заданного файла/каталога
	// Примечание: родительский каталог идентифицируется списком pidl за вычетом последнего
	//             элемента, поэтому мы временно зануляем pLastId->mkid.cb, отрезая его от списка
	temp = pLastId->mkid.cb;
	pLastId->mkid.cb = 0;
	IShellFolder *pFolder	=	NULL;
	pDesktopFolder->BindToObject(pidl, NULL, IID_IShellFolder, (void**)&pFolder);
	
	// Получаем интерфейс IContextMenu для заданного файла/каталога
	// Примечание: относительно родительского объекта заданный файл/каталог идентифицируется
	//             единственным элементом pLastId
	pLastId->mkid.cb = temp;
	IContextMenu *pContextMenu;
	pFolder->GetUIObjectOf(hWnd, 1, (LPCITEMIDLIST *)&pLastId, IID_IContextMenu, NULL, (void**)&pContextMenu);

	if(pContextMenu2)
	{
		pContextMenu->QueryInterface(IID_IContextMenu2,(LPVOID*)pContextMenu2);
	}

	if(pContextMenu3)
	{
		pContextMenu->QueryInterface(IID_IContextMenu3,(LPVOID*)pContextMenu3);
	}
	
	UINT nCmd	=	0;
	
	HMENU hPopupMenu = CreatePopupMenu();

	// Заполняем меню

	if(TreeAddon)
	{
		InsertMenu(hPopupMenu,0,MF_BYPOSITION|MF_DEFAULT|MF_STRING|(TreeAddon&2?MF_ENABLED:MF_GRAYED),0x8001,TreeAddon&4?GetString(IDS_COLLAPSE_NAME):GetString(IDS_EXPAND_NAME));
		pContextMenu->QueryContextMenu(hPopupMenu, 1, 1, 0x7FFF, CMF_EXPLORE|CMF_CANRENAME);
	}		
	else
		pContextMenu->QueryContextMenu(hPopupMenu, 0, 1, 0x7FFF, CMF_EXPLORE|CMF_CANRENAME);

	if(TreeAddon)
	{
		//InsertMenu(hPopupMenu,0,MF_BYPOSITION|MF_DEFAULT|MF_STRING|(TreeAddon&2?MF_ENABLED:MF_GRAYED),0x8001,TreeAddon&4?_T("Collapse"):_T("Expand"));
		InsertMenu(hPopupMenu,1,MF_BYPOSITION|MF_SEPARATOR,0,0);
		InsertMenu(hPopupMenu,2,MF_BYPOSITION|MF_ENABLED|MF_STRING,0x8002,GetString(IDS_NEW_CHILD_FOLDER_NAME));
		InsertMenu(hPopupMenu,3,MF_BYPOSITION|MF_SEPARATOR,0,0);
	}
	
	if(stVer==NULL)
	{
		// Создаём меню
		// Отображаем меню
		nCmd = TrackPopupMenu(hPopupMenu,
			TPM_LEFTALIGN|TPM_LEFTBUTTON|TPM_RIGHTBUTTON|TPM_RETURNCMD, x, y, 0, hWnd, 0);
	}
	else
	{
		nCmd = (UINT)stVer;
	}

	DestroyMenu(hPopupMenu);
	
	// Выполняем команду (если она была выбрана)
	if(nCmd>=1&&nCmd<=0x7FFF)
	{
		CMINVOKECOMMANDINFO ici;
		ZeroMemory(&ici, sizeof(CMINVOKECOMMANDINFO));
		ici.cbSize = sizeof(CMINVOKECOMMANDINFO);
		
		ici.hwnd = hWnd;
		ici.lpVerb = reinterpret_cast<LPCSTR>(MAKEINTRESOURCE(nCmd-1));
		ici.nShow = SW_SHOWNORMAL;
		
		pContextMenu->InvokeCommand(&ici);
	}
	
	// Получаем интерфейс IMalloc
	IMalloc *pMalloc;
	SHGetMalloc(&pMalloc);
	
	// Освобождаем память, выделенную для pidl
	if(pMalloc)
		pMalloc->Free(pidl);
	
	// Освобождаем все полученные интерфейсы
	if(pDesktopFolder)
		pDesktopFolder->Release();
	if(pFolder)
		pFolder->Release();
	if(pContextMenu2&&(*pContextMenu2))
	{
		(*pContextMenu2)->Release();
		*pContextMenu2 = NULL;
	}
	if(pContextMenu3&&(*pContextMenu3))
	{
		(*pContextMenu3)->Release();
		*pContextMenu3 = NULL;
	}
	
	if(pContextMenu)
		pContextMenu->Release();
	if(pMalloc)
		pMalloc->Release();
	
	return nCmd;
}

HRESULT NavigateNewWindow(LPUNKNOWN pBrowserDispatch, LPCTSTR strUrl)
{
	if(GetOptionInt(IDS_OFSMESSENGER,IDS_GET_DEFAULT_BROWSER_FROM_REGISTRY,TRUE)==0)
		return E_FAIL;

	HRESULT hr = S_OK;

	ATL::CUrl		urlParser;
	
	if(!urlParser.CrackUrl(strUrl)||
		urlParser.GetScheme()>ATL_URL_SCHEME_HTTPS||
		urlParser.GetScheme()<0)
		return E_INVALIDARG;

	TCHAR	Browser[MAX_PATH]		= _T("");
	TCHAR	buf[MAX_PATH]			= _T("");

	HKEY	hKey;
	DWORD	Size = 256;
	
	if(RegOpenKeyEx (HKEY_CLASSES_ROOT, _T("http\\shell\\open\\command"),0, KEY_QUERY_VALUE, &hKey)==ERROR_SUCCESS)
	{
		if(RegQueryValueEx (hKey, NULL, NULL, NULL, (LPBYTE)buf, &Size)==ERROR_SUCCESS)
		{
			int		i;
			
			int		pos;
			int		len;
			TCHAR	*EXE = _T(".exe");
			
			// Отрежем то, что после пути с именем
			// Найдем, где кончается путь
			if (buf[0] == _TEXT('\"'))
			{
				lstrcpy (Browser, &buf[1]);
			}
			else
			{
				lstrcpy (Browser, buf);
			}
			
			len = lstrlen (Browser);
			pos = len - 4;
			for (i = 0; i < len - 3; i++)
			{
				if (!_tcsncicmp (&Browser[i], EXE, 4))
				{
					pos = i;
					break;
				}
			}
			Browser[pos + 4] = 0;

			if(!ShellExecute(::GetDesktopWindow(), _T("open"), Browser,strUrl, NULL, SW_SHOWDEFAULT))
				hr = HRESULT_FROM_WIN32(GetLastError());

		}
		else
			hr = HRESULT_FROM_WIN32(GetLastError());

		RegCloseKey (hKey);
	}
	else
		hr = HRESULT_FROM_WIN32(GetLastError());
	
	return hr;
	
/*	USES_CONVERSION;
	
	HRESULT	hr	=	E_FAIL;
	LPUNKNOWN pDispatch = 	pBrowserDispatch;
	if(pDispatch)
	{
		CComQIPtr<IWebBrowser2, &IID_IWebBrowser2> spWB2(pDispatch);
		if(spWB2)
		{
			CComPtr<IDispatch> spDispDoc = NULL;
			spWB2->get_Document(&spDispDoc);
			if(spDispDoc)
			{
				CComQIPtr<IHTMLDocument2, &IID_IHTMLDocument2> spDoc(spDispDoc);
				if(spDoc)
				{
					CComPtr<IHTMLWindow2>	spWindow	=	NULL;
					spDoc->get_parentWindow(&spWindow);

					if(spWindow)
					{
						CComPtr<IHTMLWindow2>	spTmpWindow	=	NULL;
						try
						{
							CComBSTR bstrUrl = T2OLE(strUrl);
								
							if(SUCCEEDED(spWindow->open(bstrUrl,NULL,NULL,VARIANT_FALSE,&spTmpWindow)))
								return S_OK;
						}
						catch (...) 
						{
						}
					}
				}
			}
		}
	}
	return hr;		*/
}

SYSTEMTIME TimeTToSystemTime(time_t &TimeT)
{
	SYSTEMTIME retValue = {0};
	
	struct tm *newtime;
	newtime = localtime( &TimeT); /* Convert to local time. */
	retValue.wYear      = newtime->tm_year+1900;
	retValue.wMonth     = newtime->tm_mon+1;
	retValue.wDay       = newtime->tm_mday;
    retValue.wHour      = newtime->tm_hour;
	retValue.wMinute    = newtime->tm_min;
	retValue.wDayOfWeek = newtime->tm_wday;
	retValue.wSecond    = newtime->tm_sec;
	
	return retValue;
}

BOOL SetItemToColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, LPCTSTR strText, DWORD dwColor)
{
	USES_CONVERSION;

	// Confert RGB Color to BGR [8/28/2002]
	//dwColor	=	RGB(GetBValue(dwColor),GetGValue(dwColor),GetRValue(dwColor));
	// Confert RGB Color to BGR [8/28/2002]

	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),UserRole,UserId);
	
	CString strColorXML = GetRegFileText(strSection,GetString(IDS_SELECT_COLOR));
	
	CComPtr<IXMLDOMDocument>	pColorDoc	=	NULL;
	pColorDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);

	VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
	
	if(!strColorXML.IsEmpty())
	{
		CComBSTR bsXML;
		bsXML.Attach(strColorXML.AllocSysString());
		
		pColorDoc->loadXML(bsXML,&varLoad);

		CString strQueryFormat;
		strQueryFormat.Format(_T("colors/color_item[id=\"%d\"]"),dwId);
		
		CComBSTR bsQuery;
		bsQuery.Attach(strQueryFormat.AllocSysString());
		
		CComPtr<IXMLDOMNode>	pColorItemNode = NULL;
		
		pColorDoc->selectSingleNode(bsQuery,&pColorItemNode);
		
		if(pColorItemNode)
		{
			CComPtr<IXMLDOMNode> pColorItemNodeParent	=	NULL;
			pColorItemNode->get_parentNode(&pColorItemNodeParent);

			if(pColorItemNodeParent)
			{
				pColorItemNodeParent->removeChild(pColorItemNode,NULL);
			}
		}	
									
	}
	else
	{
		pColorDoc->loadXML(CComBSTR(L"<colors/>"),&varLoad);
	}

	if(varLoad==VARIANT_FALSE)
		return FALSE;


	CComPtr<IXMLDOMNode>	pColorsNode = NULL;

	pColorDoc->selectSingleNode(CComBSTR(L"colors"),&pColorsNode);

	if(pColorsNode)
	{
		CComPtr<IXMLDOMNode>	pColorItemNode = NULL;

		insertSingleNode(pColorsNode,CComBSTR("color_item"),NULL,NULL,&pColorItemNode);

		if(pColorItemNode)
		{
			WCHAR wsBuffId[100],wsBuff[100];

			_ltow(dwId,wsBuffId,10);

			//_ltow(dwColor,wsBuffColor,16);
			swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));

			insertSingleNode(pColorItemNode,CComBSTR("id"),NULL,CComBSTR(wsBuffId));
			insertSingleNode(pColorItemNode,CComBSTR("text"),NULL,CComBSTR(T2CW(strText)));
			insertSingleNode(pColorItemNode,CComBSTR("color"),NULL,CComBSTR(wsBuff));

			CComBSTR bsXML;
			pColorDoc->get_xml(&bsXML);
			
			SetRegFileText(strSection,GetString(IDS_SELECT_COLOR),W2CT(bsXML));
	
			return TRUE;
		}
	}
	
	return FALSE;
}

BOOL GetItemFromColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, CString& strText, DWORD &dwColor)
{
	CString strSection;
	strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),UserRole,UserId);
	
	CString strColorXML = GetRegFileText(strSection,GetString(IDS_SELECT_COLOR));
	
	if(strColorXML.IsEmpty())
		return FALSE;
	
	CComPtr<IXMLDOMDocument>	pColorDoc	=	NULL;
	pColorDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
	
	CComBSTR bsXML;
	bsXML.Attach(strColorXML.AllocSysString());

	VARIANT_BOOL varLoad	=	VARIANT_FALSE;
	
	pColorDoc->loadXML(bsXML,&varLoad);
	
	if(varLoad==VARIANT_TRUE)
	{
		CString strQueryFormat;
		strQueryFormat.Format(_T("colors/color_item[id=\"%d\"]"),dwId);
		
		CComBSTR bsQuery;
		bsQuery.Attach(strQueryFormat.AllocSysString());
		
		CComPtr<IXMLDOMNode>	pColorItemNode = NULL;
		
		pColorDoc->selectSingleNode(bsQuery,&pColorItemNode);
		
		if(pColorItemNode)
		{
			CComBSTR bsUserId, bsText, bsColor;
			
			GetTextByPath(pColorItemNode,CComBSTR(L"text"),&bsText);
			GetTextByPath(pColorItemNode,CComBSTR(L"color"),&bsColor);
			
			strText = (BSTR)bsText;
			
			WCHAR *pEnd = NULL;

			dwColor = wcstol(bsColor,&pEnd,16);
			
			dwColor	=	RGB(GetBValue(dwColor),GetGValue(dwColor),GetRValue(dwColor));
			
			return TRUE;
		}		
	}
	
	return FALSE;
}

BOOL GetColorFromColorStorage(LPCTSTR UserRole, int UserId, DWORD dwId, DWORD &dwColor)
{
	CString strTmp;
	return GetItemFromColorStorage(UserRole,UserId,dwId,strTmp,dwColor);
}

/************************************************************************/
/* BNF Form from WWW.RSDN.RU 

	<Email>		::= <Name> @ <Domain>
	<Name>		::= <Words>
	<Domain>	::= <Words>
	<Words>		::= <Word> ['.' <Words>]
	<Word>		::= <letter> <letter>*

	<letter> - любые символы в диапазоне #33 (-) до #126 (~), кроме ()<>@,;:\/.[] (согласно RFC 822)
*/
/************************************************************************/
BOOL CheckEmailString(LPCTSTR strEmail)
{
	// Check strEmail Is Good String [9/2/2002]
	if(IsBadStringPtr(strEmail,256))
		return FALSE;

	//  [9/2/2002]
	const TCHAR char_min = _T('-');  // #33
    const TCHAR char_max = _T('~');  // #126

	// в этот диапазон входят:
    //  -
    //  .
    //  /
    //  012456789
    //  :;<=>?
    //  @
    //  ABCDEFGHIJKLMNOPQRSTUVWXYZ
    //  [\]^
    //  _
    //  `
    //  abcdefghijklmnopqrstuvwxyz
    //  { | }
    //  ~

    enum State { er = -2, ok = -1, n1, n2, d1, d2, states_count };

	// макросы для заполнения таблицы переходов (см.ниже)
	#define t_4(st)     st,st,st,st
	#define t_6(st)     st,st,st,st,st,st
	#define t_10(st)    t_6(st), t_4(st)
	#define t_26(st)    t_10(st), t_10(st), t_6(st)

	#define er_3        er,er,er
	#define er_4        t_4(er)
	#define er_6        t_6(er)

	#define t_dot(st)       st, er
	#define t_digits(st)    t_10(st), er_6
	#define t_at(st)        st
	#define t_alpha(st)     t_26(st), er_4, st, er, t_26(st), er_3, st

	#define t_all(end, dot, at, letter) \
		{ end, letter, t_dot(dot), t_digits(letter), t_at(at), t_alpha(letter) }

    // под индексом [0] идет реакция на конец строки
    // анализ символов начинается с [1]: index = c - char_min + 1

    static const State transfer[states_count][char_max-char_min+2] =
    {
        t_all(er,er,er,n2),
        t_all(er,n1,d1,n2),
        t_all(er,er,er,d2),
        t_all(ok,d1,er,d2),
    };
	
	#undef t_4
	#undef t_6
	#undef t_10
	#undef t_26
	#undef er_3
	#undef er_4
	#undef er_6
	#undef t_dot
	#undef t_digits
	#undef t_at
	#undef t_alpha
	#undef t_all
	
    State state = n1;

    while(true)
    {
        TCHAR c = *(strEmail++);
        if(c==_T('\0'))
            return transfer[state][0] == ok;
        if(c < char_min || c > char_max)
            return FALSE;
        state = transfer[state][c - char_min + 1];
        if(state == er)
            return FALSE;
    }
}

BOOL GetSendToLinkFileName(CString &Result)
{
	BOOL result = FALSE;
	CString strFolder;

	if(McRegGetString(HKEY_CURRENT_USER, _T("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"), _T("SendTo"), strFolder, NULL))
	{
		if( GetProductLanguage() == "1049")
		{
			#ifndef RADIUS
				Result.Format(_T("%s\\IBN Группа или пользователь.lnk"), strFolder);
			#else
				Result.Format(_T("%s\\MagRul Группа или пользователь.lnk"), strFolder);
			#endif
		}
		else
		{
			#ifndef RADIUS
				Result.Format(_T("%s\\IBN Group or User.lnk"), strFolder);
			#else
				Result.Format(_T("%s\\MagRul Group or User.lnk"), strFolder);
			#endif
		}

		result = TRUE;
	}
	return result;
}

HRESULT CreateShortcut(LPCTSTR szTargetFile, LPCTSTR szArgs, LPCTSTR szLinkFile, LPCTSTR szDesc)
{
	HRESULT hr;
	IShellLink* psl = NULL;

	// Get a pointer to the IShellLink interface.
	hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, reinterpret_cast<void**>(&psl));
	if(SUCCEEDED(hr))
	{
	   IPersistFile* ppf = NULL;

	   // Query IShellLink for the IPersistFile interface for 
	   // saving the shell link in persistent storage.
	   hr = psl->QueryInterface(IID_IPersistFile, reinterpret_cast<void**>(&ppf));
	   if(SUCCEEDED(hr))
	   {   
//		 WORD wsz[MAX_PATH];

		 // Set the path to the shell link target.
		 hr = psl->SetPath(szTargetFile);

		if(SUCCEEDED(hr))
		{
			// Set arguments
			hr = psl->SetArguments(szArgs);
			if(SUCCEEDED(hr))
			{
				// Set the description of the shell link.
				hr = psl->SetDescription(szDesc);
				
				USES_CONVERSION;
				// Save the link via the IPersistFile::Save method.
				hr = ppf->Save(T2COLE(szLinkFile), TRUE);
			}
		 }
	
		 // Release pointer to IPersistFile.
		 ppf->Release();
	   }

	   // Release pointer to IShellLink.
	   psl->Release();
	}
	return hr;
}


BOOL IsSendToLinkCreated()
{
	BOOL result = FALSE;
	CString strFile;

	if(GetSendToLinkFileName(strFile))
	{
		DWORD dw = GetFileAttributes(strFile);
		if(dw != INVALID_FILE_ATTRIBUTES && (dw & FILE_ATTRIBUTE_DIRECTORY) != FILE_ATTRIBUTE_DIRECTORY)
			result = TRUE;
	}

	return result;
}

void CreateSendToLink()
{
	CString strLinkFile;

	if(GetSendToLinkFileName(strLinkFile))
	{
		TCHAR szTarget[MAX_PATH];
		if(0 < GetModuleFileName(NULL, szTarget, MAX_PATH))
		{
			CreateShortcut(szTarget, _T("files:"), strLinkFile, _T("IBN Client"));
		}
	}
}

BOOL DeleteSendToLink()
{
	CString strFile;
	if(GetSendToLinkFileName(strFile))
		return ::DeleteFile(strFile);
	else
		return FALSE;
}

DWORD GetDllVersion(LPCTSTR lpszDllName)
{
    HINSTANCE hinstDll;
    DWORD dwVersion = 0;
	
    /* For security purposes, LoadLibrary should be provided with a 
	fully-qualified path to the DLL. The lpszDllName variable should be
	tested to ensure that it is a fully qualified path before it is used. */
    hinstDll = LoadLibrary(lpszDllName);
	
    if(hinstDll)
    {
        DLLGETVERSIONPROC pDllGetVersion;
        pDllGetVersion = (DLLGETVERSIONPROC)GetProcAddress(hinstDll, 
			"DllGetVersion");
		
			/* Because some DLLs might not implement this function, you
			must test for it explicitly. Depending on the particular 
			DLL, the lack of a DllGetVersion function can be a useful
        indicator of the version. */
		
        if(pDllGetVersion)
        {
            DLLVERSIONINFO dvi;
            HRESULT hr;
			
            ZeroMemory(&dvi, sizeof(dvi));
            dvi.cbSize = sizeof(dvi);
			
            hr = (*pDllGetVersion)(&dvi);
			
            if(SUCCEEDED(hr))
            {
				dwVersion = PACKVERSION(dvi.dwMajorVersion, dvi.dwMinorVersion);
            }
        }
		
        FreeLibrary(hinstDll);
    }
    return dwVersion;
}

