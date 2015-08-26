#if !defined(AFX_USERDETAILSDLG_H__6840FC9E_395D_4507_A4DE_A063FFEE1B5B__INCLUDED_)
#define AFX_USERDETAILSDLG_H__6840FC9E_395D_4507_A4DE_A063FFEE1B5B__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// UserDetailsDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CUserDetailsDlg dialog
#include "ResizableDialog.h"
#include ".\\OFSDhtmlEditCtrl\\OFSDhtmlCtrl.h"
#include "user.h"	// Added by ClassView

class CUserDetailsDlg : public CDialog
{
// Construction
public:
	void AddInfo(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
	CUserDetailsDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CUserDetailsDlg)
	enum { IDD = IDD_DIALOG_USER_DETAILS };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CUserDetailsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
public:
	void Refresh();
	void ShowDialog();
	CUser m_User;

	CString	m_Email;
	CString	m_LastName;
	CString	m_NickName;
	CString	m_Role;
	CString	m_FirstName;

// Implementation
protected:
	long Handle;
	BOOL bAutoRefresh;
    ISessionPtr pSession;
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
    LRESULT OnRefresh(WPARAM w,LPARAM l);
	// Generated message map functions
	//{{AFX_MSG(CUserDetailsDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnClose();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_USERDETAILSDLG_H__6840FC9E_395D_4507_A4DE_A063FFEE1B5B__INCLUDED_)
