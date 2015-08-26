// IBNTO_MESSAGE.cpp: implementation of the IBNTO_MESSAGE class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "IBNTO_MESSAGE.h"
#include <Shlwapi.h>

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

IBNTO_MESSAGE::IBNTO_MESSAGE()
{
}

IBNTO_MESSAGE::~IBNTO_MESSAGE()
{
}

BOOL IBNTO_MESSAGE::Parse(LPCTSTR szURL)
{
	BOOL ret = FALSE;
	LPCTSTR szIbnTo = _T("ibnto:");
	CString str = szURL;
	str.MakeLower();
	int n = str.Find(szIbnTo);
	if(n >= 0)
	{
		LPCTSTR szBegin = szURL + n + _tcslen(szIbnTo);
		LPCTSTR szEnd = szURL + _tcslen(szURL);
		LPCTSTR sz = szBegin;
		// Get login and domain
		while(sz < szEnd && *sz != _T('?'))
			sz++;
		str = CString(szBegin, sz-szBegin);

		CStringArray	MailboxArray;

		int CommaIndex	=	-1;
		while((CommaIndex = str.Find(_T(",")))>0)
		{
			MailboxArray.Add(str.Mid(0,CommaIndex));
			str = str.Mid(CommaIndex+1);
		}
		if(str.GetLength()>0)
			MailboxArray.Add(str);

		for(int MailboxArrayIndex = 0 ; MailboxArrayIndex<MailboxArray.GetSize();MailboxArrayIndex++)
		{
			str = MailboxArray[MailboxArrayIndex];
			
			n = str.ReverseFind(_T('@'));
			if(n > 0)
			{
				CString strLogin = str.Left(n);
				Unescape(&strLogin);
				CString strDomain = str.Mid(n+1);
				Unescape(&strDomain);
				
				Logins.Add(LoginItem(strLogin,strDomain));
			}
		}

		sz++;
		szBegin = sz;
		// Get message body
		while(sz < szEnd)
			sz++;
		Message = CString(szBegin, sz-szBegin);
		Unescape(&Message);
		ret = TRUE;
	}
	return ret;
}

void IBNTO_MESSAGE::Unescape(CString* pstr)
{
	DWORD dwSize = pstr->GetLength();
	LPTSTR sz = pstr->GetBuffer(dwSize);
	HRESULT hr = UrlUnescape(sz, NULL, &dwSize, URL_DONT_UNESCAPE_EXTRA_INFO|URL_UNESCAPE_INPLACE);
	pstr->ReleaseBuffer();
}