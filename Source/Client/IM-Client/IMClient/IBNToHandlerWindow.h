// IBNToHandlerWindow.h: interface for the CIBNToHandlerWindow class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_IBNTOHANDLERWINDOW_H__58D4DC4A_2A4A_4E65_B1E6_1CA16DC92DEF__INCLUDED_)
#define AFX_IBNTOHANDLERWINDOW_H__58D4DC4A_2A4A_4E65_B1E6_1CA16DC92DEF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "IBNTO_MESSAGE.h"
#include "SENDTO_MESSAGE.h"

#define MC_IBNTO_CLASS_NAME         _T("#32770")//_T("{64A03E25-020D-4925-977B-730A686643D4}")
#define MC_IBNTO_WINDOW_NAME        _T("{D6DB3F1D-A59E-42e5-BB49-19086A3E7300}")

enum IBN_MESSAGE
{
	  IBN_MSG_IBNTO
	, IBN_MSG_FILES
};

struct EnumWindowsParams
{
	LPCTSTR szWindowClass;
	LPCTSTR szWindowName;
	BOOL bResult;
	LPCTSTR szCmdLine;
	DWORD dwMessage;
};

class CIBNToHandlerWindow : public CWnd  
{
public:
	CIBNToHandlerWindow();
	virtual ~CIBNToHandlerWindow();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMcUpdateWindow)
	//}}AFX_VIRTUAL

// Implementation
public:
	void SetMainHWND(HWND hWnd);
	BOOL Create();
	IBNTO_MESSAGE* GetIbnToCommand();
	SENDTO_MESSAGE* GetFilesCommand();
	BOOL SendIbnToCommand();
	BOOL SendFilesCommand();
	
	// Generated message map functions
protected:
	HWND m_hMainWnd;
	//{{AFX_MSG(CMcUpdateWindow)
	afx_msg LRESULT OnCopyData(WPARAM w, LPARAM l);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

#endif // !defined(AFX_IBNTOHANDLERWINDOW_H__58D4DC4A_2A4A_4E65_B1E6_1CA16DC92DEF__INCLUDED_)
