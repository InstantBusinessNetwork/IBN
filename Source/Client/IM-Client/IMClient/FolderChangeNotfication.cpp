// FolderChangeNotfication.cpp: implementation of the CFolderChangeNotification   class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "FolderChangeNotfication.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFolderChangeNotification::CFolderChangeNotification()
{
	m_hArray.reserve(32);
	m_PathList.reserve(32);

	m_hWorkThread	=	NULL;
	m_hEventWnd		=	NULL;
	m_dwCallbackMsg	=	NULL;
}

CFolderChangeNotification::~CFolderChangeNotification()
{
	Stop();
	DeleteAllPath();
}

HRESULT CFolderChangeNotification::Start(HWND hEventWindow, DWORD dwCallbackMsg)
{
	if(m_hWorkThread||!m_PathList.size())
		return E_FAIL;
	
	if(!IsWindow(hEventWindow)||!dwCallbackMsg)
		return E_INVALIDARG;
	
	m_hEventWnd		=	hEventWindow;
	m_dwCallbackMsg	=	dwCallbackMsg;
	
	// Step1 [3/25/2002]
	
	DWORD	dwTreadId	=	0;
	if(!(m_hWorkThread = CreateThread(NULL,30000,thWorkThread,this,CREATE_SUSPENDED,&dwTreadId)))
		return HRESULT_FROM_WIN32(GetLastError());
	
	//////////////////////////////////////////////////////////////////////////
	// Step 1. Create Nofify Object  [3/23/2002]
	m_hArray.push_back(CreateEvent(NULL,FALSE,FALSE,NULL));
		
	// Step 2. Add End Work Notify Object [3/23/2002]
	std::vector<_FCItem>::iterator iItem = m_PathList.begin();
	while(iItem!=m_PathList.end())
	{
		(*iItem).hEvent  = FindFirstChangeNotification((*iItem).lpPathName,(*iItem).bWatchSubtree,(*iItem).dwNotifyFilter);
		if((*iItem).hEvent!=INVALID_HANDLE_VALUE)
		{
			m_hArray.push_back((*iItem).hEvent);
		}
		iItem++;
	}
	
	ResumeThread(m_hWorkThread);
	
	return S_OK;
}

HRESULT CFolderChangeNotification::Stop(DWORD dwTimeOut)
{
	if(!m_hWorkThread)
		return E_FAIL;
	
	SetEvent(m_hArray[0]);

	if(WaitForSingleObject(m_hWorkThread,dwTimeOut)==WAIT_TIMEOUT)
	{
		TerminateThread(m_hWorkThread,2);
	}

	CloseHandle(m_hWorkThread);
	m_hWorkThread = NULL;
	
	std::vector<HANDLE>::iterator iItem = m_hArray.begin();
	while(iItem!=m_hArray.end())
	{
		if((*iItem))
		{
			FindCloseChangeNotification((*iItem));
		}
		iItem++;
	}
	m_hArray.clear();
	
	return S_OK;
}

BOOL CFolderChangeNotification::IsStarted()
{
	return (m_hWorkThread!=NULL);
}

DWORD WINAPI CFolderChangeNotification::thWorkThread(LPVOID pParam)
{
	CFolderChangeNotification *pThis = (CFolderChangeNotification *)pParam;
	pThis->thWorkTreadBody();
	return 0;
}

void CFolderChangeNotification::thWorkTreadBody()
{
	//TRACE(_T("\r\n--- FindNextChangeNotification Start ---"));
	//////////////////////////////////////////////////////////////////////////
	// Step 3. Wait  [3/23/2002]
	const HANDLE *pHandeleArray = &(*m_hArray.begin());

	while (TRUE) 
	{
		DWORD dwRetValue = WaitForMultipleObjects(m_hArray.size(),pHandeleArray,FALSE,INFINITE);
		
		DWORD dwSignalEventIndex = dwRetValue-WAIT_OBJECT_0;
		if(dwSignalEventIndex)
		{
			if(dwSignalEventIndex>=0&&dwSignalEventIndex<m_hArray.size())
			{
				// Step 4. Update Notification Event  [3/23/2002]
				FindNextChangeNotification( m_hArray[dwSignalEventIndex]);
				// Step 5. Update End Work Event  [3/23/2002]
				HANDLE hEventSet =  m_hArray[dwSignalEventIndex];

				long iRealVectorIndex	=	0;
				for(std::vector<_FCItem>::iterator iItem = m_PathList.begin();iItem!=m_PathList.end();iItem++)
				{
					if((*iItem).hEvent==hEventSet)
					{
						PostMessage(m_hEventWnd,m_dwCallbackMsg,iRealVectorIndex,0);
						break;
					}
					iRealVectorIndex++;
				}
			}
			else
			{
				// break [3/25/2002]
				ASSERT(FALSE);
				break;
			}
		}
		else
		{
			// Exit [3/25/2002]
			break;
		}
	}
	//TRACE(_T("\r\n--- FindNextChangeNotification End ---"));
	// End [3/23/2002]
	//////////////////////////////////////////////////////////////////////////
}

HRESULT CFolderChangeNotification::AddPath(LPCTSTR strPath, BOOL bWatchSubtree, DWORD dwNotifyFilter)
{
	if(m_hWorkThread!=NULL)
		return E_FAIL;
	
	if(strPath==NULL)
		return E_INVALIDARG;

	_FCItem Item	=	{0};

	Item.lpPathName		=	new TCHAR[_tcslen(strPath)+1];
	_tcscpy(Item.lpPathName,strPath);
	Item.bWatchSubtree	= bWatchSubtree;
	Item.dwNotifyFilter = dwNotifyFilter;
	Item.hEvent			= NULL;	

	m_PathList.push_back(Item);
	
	return S_OK;
}


HRESULT CFolderChangeNotification::DeleteAllPath()
{
	if(m_hWorkThread!=NULL)
		return E_FAIL;
	
	std::vector<_FCItem>::iterator iItem = m_PathList.begin();
	while(iItem!=m_PathList.end())
	{
		if((*iItem).lpPathName)
		{
			delete [](*iItem).lpPathName;
			(*iItem).lpPathName = NULL;
		}
		iItem++;
	}
	m_PathList.clear();
	return S_OK;
}


long CFolderChangeNotification::GetPathCount()
{
	return m_PathList.size();
}

LPCTSTR CFolderChangeNotification::GetPathItem(long Index,BOOL *pbWatchSubtree, DWORD *pdwNotifyFilter)
{
	if(Index<0||Index>=m_PathList.size())
		return NULL;
	
	long TmpIndex	=	0;
	
	const _FCItem iItem = m_PathList[Index];

	if (pbWatchSubtree) 
		*pbWatchSubtree = iItem.bWatchSubtree;
	if (pdwNotifyFilter) 
		*pdwNotifyFilter = iItem.dwNotifyFilter;

	return iItem.lpPathName;
}

