// FileDownloader.h: interface for the CFileDownloader class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FILEDOWNLOADER_H__DFC272F7_58F8_4631_AA12_B44536D6A479__INCLUDED_)
#define AFX_FILEDOWNLOADER_H__DFC272F7_58F8_4631_AA12_B44536D6A479__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "HttpDownloader.h"

class CFileDownloader  
{
public:
	CFileDownloader();
	virtual ~CFileDownloader();
public:
	BOOL Clear();
	BOOL StopDownload(DWORD WaitTime=1000);
	BOOL IsDownload();
	BOOL Init(HWND hWnd, UINT msg);
	BOOL Load(LPCTSTR szUrl);
	BOOL Save(LPCTSTR szPath);
private:
	void DownloadBody();
	CHttpDownloader	m_HTTPDownloader;
	CString			m_strUrl;
	HWND			m_MessageWnd;
	UINT			m_RetMessage;
	HANDLE			m_hWorkTread;
	DWORD			m_dwWorkTreadId;
	LPSTREAM		m_Data;
protected:
	static DWORD WINAPI trWorkThread(LPVOID lParam);
};

#endif // !defined(AFX_FILEDOWNLOADER_H__DFC272F7_58F8_4631_AA12_B44536D6A479__INCLUDED_)
