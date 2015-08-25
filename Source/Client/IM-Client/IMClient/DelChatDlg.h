//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_CDelChatDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_)
#define AFX_CDelChatDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CDelChatDlg.h : header file
//

#include "ResizableDialog.h"
#include "chat.h"

/////////////////////////////////////////////////////////////////////////////
// CDelChatDlg dialog
class CMainDlg;
class CDelChatDlg : public CDialog
{
// Construction
public:
	CDelChatDlg(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDelChatDlg)
	enum { IDD = IDD_DELCHAT_DIALOG };
	CString	m_Description;
	//}}AFX_DATA
protected:
	ISessionPtr		pSession;
	long			Handle;
	CMainDlg	*pMessenger;
	BOOL			bIsKillWinodow;
	CComBSTR		m_bsChatId;
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDelChatDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
public:
	void	SetKillChat(const CChat &ChatName);
protected:
	void KillWindow();
	void Block();
	void UnBlock();
	// Generated message map functions
	//{{AFX_MSG(CDelChatDlg)
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

#endif // !defined(AFX_CDelChatDlg_H__DD63E5E4_EC98_4B9D_A1E3_580EEA65B45D__INCLUDED_)
