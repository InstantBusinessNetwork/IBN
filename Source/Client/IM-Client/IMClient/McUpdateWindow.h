#if !defined(AFX_MCUPDATEWINDOW_H__3400D60D_8784_41F7_A657_5E6E1A3FF230__INCLUDED_)
#define AFX_MCUPDATEWINDOW_H__3400D60D_8784_41F7_A657_5E6E1A3FF230__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// McUpdateWindow.h : header file
//

//===================================================================================
// Definitions for automatic update
//===================================================================================
#define MC_PRODUCT_CODE					 _T("{F99B1678-ED11-40CC-9C73-EC18089F7464}")
#define MC_AUTOUPDATE_CLASS_NAME         _T("#32770")//_T("{FAABF076-218C-4328-BFC8-B2F68A8F3F1A}")
#define MC_AUTOUPDATE_WINDOW_NAME        _T("{D979BF88-6837-4e4a-B10F-0CC79789F712}")
#define MC_AUTOUPDATE_SYNC_OBJECT_NAME   _T("{CDACF5C0-1920-4a70-8044-775CAE4C50F3}")
#define WM_UPDATE_EXIT WM_USER + 0x44
//===================================================================================

/////////////////////////////////////////////////////////////////////////////
// CMcUpdateWindow window

class CMcUpdateWindow : public CWnd
{
// Construction
public:
	CMcUpdateWindow();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMcUpdateWindow)
	//}}AFX_VIRTUAL

// Implementation
public:
	void SetMainHWND(HWND hWnd);
	BOOL Create();
	virtual ~CMcUpdateWindow();

	// Generated message map functions
protected:
	BOOL RegisterWindowClass();
	HWND m_hMainWnd;
	//{{AFX_MSG(CMcUpdateWindow)
	afx_msg LRESULT OnUpdateExit(WPARAM w, LPARAM l);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MCUPDATEWINDOW_H__3400D60D_8784_41F7_A657_5E6E1A3FF230__INCLUDED_)
