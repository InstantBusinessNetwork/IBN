// FolderChangeNotfication.h: interface for the CFolderChangeNotification   class.
//
//////////////////////////////////////////////////////////////////////
#include <list>
#include <vector>

#if !defined(AFX_FOLDERCHANGENOTFICATION_H__C4FFEBE1_155F_4575_98B2_F43BC68A79A8__INCLUDED_)
#define AFX_FOLDERCHANGENOTFICATION_H__C4FFEBE1_155F_4575_98B2_F43BC68A79A8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CFolderChangeNotification  
{
public:
	struct _FCItem
	{
		LPTSTR	lpPathName;
		BOOL	bWatchSubtree;
		DWORD	dwNotifyFilter;
		HANDLE	hEvent;
	};

public:
	LPCTSTR GetPathItem(long Index,BOOL *pbWatchSubtree = NULL, DWORD *spdwNotifyFilter = NULL);
	long GetPathCount();
	BOOL	IsStarted();
	HRESULT DeleteAllPath();
	HRESULT AddPath(LPCTSTR strPath, BOOL bWatchSubtree, DWORD dwNotifyFilter =	FILE_NOTIFY_CHANGE_FILE_NAME|
																FILE_NOTIFY_CHANGE_DIR_NAME|
																FILE_NOTIFY_CHANGE_ATTRIBUTES|
																FILE_NOTIFY_CHANGE_SIZE|
																FILE_NOTIFY_CHANGE_LAST_WRITE|
																FILE_NOTIFY_CHANGE_LAST_ACCESS|
																FILE_NOTIFY_CHANGE_CREATION|
																FILE_NOTIFY_CHANGE_SECURITY);
	CFolderChangeNotification();
	virtual ~CFolderChangeNotification();

	HRESULT Start(HWND hEventWindow, DWORD dwCallbackMsg);
	HRESULT Stop(DWORD dwTimeOut	=	INFINITE);

protected:
	static DWORD WINAPI thWorkThread(LPVOID pParam);

private:
	void thWorkTreadBody();

private:
	HANDLE					m_hWorkThread;
	std::vector<_FCItem>	m_PathList;

	HWND					m_hEventWnd;
	DWORD					m_dwCallbackMsg;

	std::vector<HANDLE>		m_hArray;
};

#endif // !defined(AFX_FOLDERCHANGENOTFICATION_H__C4FFEBE1_155F_4575_98B2_F43BC68A79A8__INCLUDED_)
