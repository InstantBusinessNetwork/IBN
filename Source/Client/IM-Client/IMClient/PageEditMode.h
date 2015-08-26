#if !defined(AFX_PAGEEDITMODE_H__1E015776_9E57_4114_9DA8_C42AC22FAC58__INCLUDED_)
#define AFX_PAGEEDITMODE_H__1E015776_9E57_4114_9DA8_C42AC22FAC58__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageEditMode.h : header file
//
#include "McSettingsDlg.h"
/////////////////////////////////////////////////////////////////////////////
// CPageEditMode dialog

class CPageEditMode : public CMcSettingsPage
{
// Construction
public:
	CPageEditMode(LPCTSTR UserRole, int UserId, LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageEditMode)
	enum { IDD = IDD_PAGE_EDIT };
	BOOL	m_bClearChatWindow;
	BOOL	m_bShowFriendStatus;
	CString	m_strReceivedFilePath;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageEditMode)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();
// Implementation
protected:
	CString m_UserRole;
	int m_UserId;
	// Generated message map functions
	//{{AFX_MSG(CPageEditMode)
	virtual BOOL OnInitDialog();
	afx_msg void OnSelectButton();
	afx_msg void OnClearChatwindowCheck();
	afx_msg void OnShowfriendstatusCheck();
	afx_msg void OnEnterSendRadio();
	afx_msg void OnEnterNewlineRadio();
	afx_msg void OnCtrlenterNewlineRadio();
	afx_msg void OnCtrlenterSendRadio();
	afx_msg void OnShiftenterSendRadio();
	afx_msg void OnShiftenterNewlineRadio();
	afx_msg void OnAltsSendRadio();
	afx_msg void OnAltsNewlineRadio();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEEDITMODE_H__1E015776_9E57_4114_9DA8_C42AC22FAC58__INCLUDED_)
