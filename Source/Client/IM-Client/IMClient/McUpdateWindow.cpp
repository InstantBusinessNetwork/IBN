// McUpdateWindow.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "McUpdateWindow.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMcUpdateWindow

CMcUpdateWindow::CMcUpdateWindow()
{
}

CMcUpdateWindow::~CMcUpdateWindow()
{
}


BEGIN_MESSAGE_MAP(CMcUpdateWindow, CWnd)
//{{AFX_MSG_MAP(CMcUpdateWindow)
	ON_MESSAGE(WM_UPDATE_EXIT,OnUpdateExit)
//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CMcUpdateWindow message handlers

void CMcUpdateWindow::SetMainHWND(HWND hWnd)
{
	m_hMainWnd = hWnd;
}

BOOL CMcUpdateWindow::Create()
{
	BOOL bResult = FALSE;
	
	if(RegisterWindowClass())
		bResult = CWnd::CreateEx(0,MC_AUTOUPDATE_CLASS_NAME, MC_AUTOUPDATE_WINDOW_NAME, WS_OVERLAPPED, 
		CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT, ::GetDesktopWindow(), 0);
	
	return bResult;
}

BOOL CMcUpdateWindow::RegisterWindowClass()
{
	return TRUE;
}

LRESULT CMcUpdateWindow::OnUpdateExit(WPARAM w, LPARAM l)
{
	::PostMessage(m_hMainWnd, WM_UPDATE_EXIT, 0, 0);
	return 0;
}
