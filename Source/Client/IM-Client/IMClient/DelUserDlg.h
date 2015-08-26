//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_CDelUserDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_)
#define AFX_CDelUserDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CDelUserDlg.h : header file
//

#include "ResizableDialog.h"
#include "User.h"

/////////////////////////////////////////////////////////////////////////////
// CDelUserDlg dialog
class CMainDlg;
class CDelUserDlg : public CDialog
{
// Construction
public:
	CDelUserDlg(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDelUserDlg)
	enum { IDD = IDD_DELUSER_DIALOG };
	CString	m_Description;
	//}}AFX_DATA
protected:
	ISessionPtr		pSession;
	long			Handle;
	CMainDlg	*pMessenger;
	CUser			m_KillUser;
	long			m_ListType;
	BOOL			bIsKillWinodow;
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDelUserDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
public:
	void	SetKillUser(CUser& user, long ListType = 1 /*Contact List*/);
protected:
	void KillWindow();
	void Block();
	void UnBlock();
	// Generated message map functions
	//{{AFX_MSG(CDelUserDlg)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnYes();
	afx_msg void OnNo();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CDelUserDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_)
