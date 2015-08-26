// IBNToHandlerWindow.cpp: implementation of the CIBNToHandlerWindow class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "IBNToHandlerWindow.h"

extern UINT g_IbnToMessage;

BOOL CALLBACK EnumWindowsProc(HWND hWnd, LPARAM l)
{
	BOOL bResult = TRUE;
	TCHAR szBuf[100];
	EnumWindowsParams *p = reinterpret_cast<EnumWindowsParams*>(l);
	
	if(p == NULL)
		return FALSE;
	
	if(::GetClassName(hWnd, szBuf, 100))
	{
		if(!_tcscmp(szBuf, p->szWindowClass))
		{
			if(::GetWindowText(hWnd, szBuf, 100))
			{
				if(!_tcscmp(szBuf, p->szWindowName))
				{
					// Send message
					COPYDATASTRUCT cds;
					cds.dwData = p->dwMessage;
					cds.lpData = (LPVOID)p->szCmdLine;
					cds.cbData = (_tcslen(p->szCmdLine)+1)*sizeof(TCHAR);
					if(1 == SendMessage(hWnd, WM_COPYDATA, 0, (LPARAM)&cds))
					{
						bResult = FALSE;
						p->bResult = TRUE;
					}
				}
			}
		}
	}
	return bResult;
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIBNToHandlerWindow::CIBNToHandlerWindow()
{

}

CIBNToHandlerWindow::~CIBNToHandlerWindow()
{

}

BEGIN_MESSAGE_MAP(CIBNToHandlerWindow, CWnd)
//{{AFX_MSG_MAP(CIBNToHandlerWindow)
	ON_MESSAGE(WM_COPYDATA, OnCopyData)
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void CIBNToHandlerWindow::SetMainHWND(HWND hWnd)
{
	m_hMainWnd = hWnd;
}

BOOL CIBNToHandlerWindow::Create()
{
	BOOL bResult = FALSE;
	
//	if(RegisterWindowClass())
		bResult = CWnd::CreateEx(0,MC_IBNTO_CLASS_NAME, MC_IBNTO_WINDOW_NAME, WS_OVERLAPPED, 
			CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT, ::GetDesktopWindow(), 0);
	
	return bResult;
}

LRESULT CIBNToHandlerWindow::OnCopyData(WPARAM w, LPARAM l)
{
	LRESULT ret = 0;
	COPYDATASTRUCT * pData = (COPYDATASTRUCT*)l;
	if(pData != NULL)
	{
		if(pData->dwData == IBN_MSG_IBNTO)
			ret = ::SendMessage(m_hMainWnd, g_IbnToMessage, (WPARAM)pData->lpData, (LPARAM)pData->dwData);
		else if(pData->dwData == IBN_MSG_FILES)
			ret = ::SendMessage(m_hMainWnd, g_IbnToMessage, (WPARAM)pData->lpData, (LPARAM)pData->dwData);
	}
	return ret;
}

IBNTO_MESSAGE* CIBNToHandlerWindow::GetIbnToCommand()
{
	CString cmd = GetCommandLine();

	IBNTO_MESSAGE *pmsg = new IBNTO_MESSAGE;
	if(pmsg->Parse(cmd))
		return pmsg;
	delete pmsg;
	return NULL;
}

BOOL CIBNToHandlerWindow::SendIbnToCommand()
{
	CString cmd = GetCommandLine();
	EnumWindowsParams p;
	p.dwMessage = IBN_MSG_IBNTO;
	p.szWindowClass = MC_IBNTO_CLASS_NAME;
	p.szWindowName = MC_IBNTO_WINDOW_NAME;
	p.szCmdLine = cmd;
	p.bResult = FALSE;
	::EnumWindows(EnumWindowsProc, (LPARAM)&p);
	return p.bResult;
}

SENDTO_MESSAGE* CIBNToHandlerWindow::GetFilesCommand()
{
	CString cmd = GetCommandLine();
	SENDTO_MESSAGE *pmsg = new SENDTO_MESSAGE;
	if(pmsg->Parse(cmd))
		return pmsg;
	delete pmsg;
	return NULL;
}

BOOL CIBNToHandlerWindow::SendFilesCommand()
{
	CString cmd = GetCommandLine();
	EnumWindowsParams p;
	p.dwMessage = IBN_MSG_FILES;
	p.szWindowClass = MC_IBNTO_CLASS_NAME;
	p.szWindowName = MC_IBNTO_WINDOW_NAME;
	p.szCmdLine = cmd;
	p.bResult = FALSE;
	::EnumWindows(EnumWindowsProc, (LPARAM)&p);
	return p.bResult;
}