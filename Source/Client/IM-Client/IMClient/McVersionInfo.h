// McVersionInfo.h: interface for the CMcVersionInfo class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MCVERSIONINFO_H__53FD5E84_3A7D_4B07_A073_FBAF33B79A55__INCLUDED_)
#define AFX_MCVERSIONINFO_H__53FD5E84_3A7D_4B07_A073_FBAF33B79A55__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CMcVersionInfo  
{
public:
	DWORD GetFileDateLS();
	DWORD GetFileDateMS();
	DWORD GetFileSubType();
	DWORD GetFileType();
	DWORD GetFileOS();
	DWORD GetFileFlag();
	DWORD GetFIXEDFILEINFOSignature();
	DWORD GetProductVersionMS();
	DWORD GetProductVersionLS();
	CString GetProductVersionString();
	ULONGLONG GetProductVersionInfo();
	DWORD GetFileVersionMS();
	DWORD GetFileVersionLS();
	CString GetFileVersionString();
	ULONGLONG GetFileVersionInfo();
	
	CMcVersionInfo();
	CMcVersionInfo(LPCTSTR ModuleName);
	virtual ~CMcVersionInfo();

private:
	BYTE				*bufVersionInfo;
	VS_FIXEDFILEINFO	*pInfo;
	CString				m_strModuleName;
};

#endif // !defined(AFX_MCVERSIONINFO_H__53FD5E84_3A7D_4B07_A073_FBAF33B79A55__INCLUDED_)
