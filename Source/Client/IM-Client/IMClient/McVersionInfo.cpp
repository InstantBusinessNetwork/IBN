// McVersionInfo.cpp: implementation of the CMcVersionInfo class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "McVersionInfo.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMcVersionInfo::CMcVersionInfo()
{
	bufVersionInfo	= NULL;
	pInfo			= NULL;

	LPTSTR pstrFN = m_strModuleName.GetBuffer (255);
	GetModuleFileName(0,pstrFN,255);
    m_strModuleName.ReleaseBuffer();
	
	ULONGLONG	u64Version = 0L;
	DWORD		dwHandle;
	
	DWORD dwFileVersionSize = GetFileVersionInfoSize((LPTSTR)(LPCTSTR)m_strModuleName, &dwHandle);
	
	if(dwFileVersionSize)
	{
		bufVersionInfo = new BYTE[dwFileVersionSize];
		BOOL bResult = ::GetFileVersionInfo((LPTSTR)(LPCTSTR)m_strModuleName, dwHandle, dwFileVersionSize, bufVersionInfo);
		if(bResult)
		{
			UINT uLen;
			bResult = VerQueryValue(bufVersionInfo, _T("\\"), (LPVOID*)&pInfo, &uLen);
		}
	}
}

CMcVersionInfo::CMcVersionInfo(LPCTSTR ModuleName)
{
	bufVersionInfo	= NULL;
	pInfo			= NULL;
	
	m_strModuleName = ModuleName;
	
	ULONGLONG	u64Version = 0L;
	DWORD		dwHandle;
	
	DWORD dwFileVersionSize = GetFileVersionInfoSize((LPTSTR)(LPCTSTR)m_strModuleName, &dwHandle);
	
	if(dwFileVersionSize)
	{
		bufVersionInfo = new BYTE[dwFileVersionSize];
		BOOL bResult = ::GetFileVersionInfo((LPTSTR)(LPCTSTR)m_strModuleName, dwHandle, dwFileVersionSize, bufVersionInfo);
		if(bResult)
		{
			UINT uLen;
			bResult = VerQueryValue(bufVersionInfo, _T("\\"), (LPVOID*)&pInfo, &uLen);
		}
	}
}


CMcVersionInfo::~CMcVersionInfo()
{
	if(bufVersionInfo)
		delete bufVersionInfo;
}

ULONGLONG CMcVersionInfo::GetProductVersionInfo()
{
	ULONGLONG	u64Version	=	0;

	if(pInfo)
	{
		u64Version = ((ULONGLONG)pInfo->dwProductVersionLS) | (((ULONGLONG)pInfo->dwProductVersionMS) << 32);
	}

	return u64Version;
}

CString CMcVersionInfo::GetProductVersionString()
{
	CString strVersion;
	ULONGLONG	u64Version = GetProductVersionInfo();
	strVersion.Format(_T("%I64u.%I64u.%I64u.%I64u"), (u64Version&0xffff000000000000)>>48, (u64Version&0xffff00000000)>>32, (u64Version&0xffff0000)>>16, (u64Version&0xffff));
	return strVersion;
}

DWORD CMcVersionInfo::GetProductVersionLS()
{
	return (DWORD)((GetProductVersionInfo()&0x00000000ffffffff));
}

DWORD CMcVersionInfo::GetProductVersionMS()
{
	return (DWORD)((GetProductVersionInfo()&0xffffffff00000000)>>32);
}

ULONGLONG CMcVersionInfo::GetFileVersionInfo()
{
	ULONGLONG	u64Version	=	0;
	
	if(pInfo)
	{
		u64Version = ((ULONGLONG)pInfo->dwFileVersionLS) | (((ULONGLONG)pInfo->dwFileVersionMS) << 32);
	}
	
	return u64Version;
}

CString CMcVersionInfo::GetFileVersionString()
{
	CString strVersion;
	ULONGLONG	u64Version = GetFileVersionInfo();
	strVersion.Format(_T("%I64u.%I64u.%I64u.%I64u"), (u64Version&0xffff000000000000)>>48, (u64Version&0xffff00000000)>>32, (u64Version&0xffff0000)>>16, (u64Version&0xffff));
	return strVersion;
}

DWORD CMcVersionInfo::GetFileVersionLS()
{
	return (DWORD)((GetFileVersionInfo()&0x00000000ffffffff));
}

DWORD CMcVersionInfo::GetFileVersionMS()
{
	return (DWORD)((GetFileVersionInfo()&0xffffffff00000000)>>32);
}

DWORD CMcVersionInfo::GetFIXEDFILEINFOSignature()
{
	if(!pInfo)
		return 0;
	// pInfo->dwSignature == 0xFEEFO4BD
	return pInfo->dwSignature;
}

DWORD CMcVersionInfo::GetFileFlag()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileFlags;
}

DWORD CMcVersionInfo::GetFileOS()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileOS;
}

DWORD CMcVersionInfo::GetFileType()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileType;
}

DWORD CMcVersionInfo::GetFileSubType()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileSubtype;
}

DWORD CMcVersionInfo::GetFileDateMS()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileDateMS;
}

DWORD CMcVersionInfo::GetFileDateLS()
{
	if(!pInfo)
		return 0;
	return pInfo->dwFileDateLS;
}
