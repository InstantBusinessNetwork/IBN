// SENDTO_MESSAGE.cpp: implementation of the SENDTO_MESSAGE class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SENDTO_MESSAGE.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

SENDTO_MESSAGE::SENDTO_MESSAGE()
{

}

SENDTO_MESSAGE::~SENDTO_MESSAGE()
{
	m_files.RemoveAll();
}

BOOL SENDTO_MESSAGE::Parse(LPCTSTR szCmd)
{
	BOOL ret = FALSE;
	LPCTSTR szCmdName = _T("files: ");
//	MessageBox(GetDesktopWindow(), szCmd, "", MB_OK);
	CString str = szCmd;
	str.MakeLower();
	int n = str.Find(szCmdName);
	if(n >= 0)
	{
		LPCTSTR szBegin = szCmd + n + _tcslen(szCmdName);
		LPCTSTR szEnd = szCmd + _tcslen(szCmd);
		LPCTSTR sz = szBegin;

		// Get file names
		while(sz < szEnd)
		{
			// Skip spaces
			while(*sz == _T(' '))
			{
				sz++;
				if(sz >= szEnd)
					break;
			}

			if(*sz == _T('\"'))
			{
				szBegin = ++sz;
				sz = _tcschr(szBegin, _T('\"'));
				if(NULL == sz || sz >= szEnd)
					sz = szEnd;
				m_files.Add(CString(szBegin, sz-szBegin));
				sz++;
			}
			else
			{
				szBegin = sz;
				sz = _tcschr(szBegin, _T(' '));
				if(NULL == sz || sz >= szEnd)
					sz = szEnd;
				m_files.Add(CString(szBegin, sz-szBegin));
			}
		}
		ret = TRUE;
/*
		CString str;
		for(int i=0; i<m_files.GetSize(); i++)
		{
			str += m_files.GetAt(i);
			str += _T("\r\n");
		}
		MessageBox(GetDesktopWindow(), str, "", MB_OK);
*/
	}
	return ret;
}
