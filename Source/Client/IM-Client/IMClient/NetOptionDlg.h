#if !defined(AFX_NETOPTIONDLG_H__369A32B4_EA0A_4B63_B6EC_8A6689D5111C__INCLUDED_)
#define AFX_NETOPTIONDLG_H__369A32B4_EA0A_4B63_B6EC_8A6689D5111C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// NetOptionDlg.h : header file
//
#include "OfsNcDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CNetOptionDlg dialog

class CNetOptionDlg : public COFSNcDlg
{
// Construction
public:
	CNetOptionDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CNetOptionDlg)
	enum { IDD = IDD_NETOPTION_DIALOG };
	CEdit	m_ServerPortEdit;
	CEdit	m_ServerNameEdit;
	CString	m_ServerName;
	CString	m_ServerPort;
	BOOL	m_BypassCheck;
	CString	m_FireWallPass;
	CString	m_FireWallUser;
	BOOL	m_UserFireWall;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CNetOptionDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
    DWORD dwAccessType;
	// Generated message map functions
	//{{AFX_MSG(CNetOptionDlg)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnDefaultRadio();
	afx_msg void OnDirectRadio();
	afx_msg void OnProxyRadio();
	afx_msg void OnUsefirewallCheck();
	afx_msg void OnChangeFirewalluser();
	afx_msg void OnErrspaceFirewallpass();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_NETOPTIONDLG_H__369A32B4_EA0A_4B63_B6EC_8A6689D5111C__INCLUDED_)
