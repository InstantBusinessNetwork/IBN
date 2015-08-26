#if !defined(AFX_PAGELAUNCHMODE_H__9D6D3161_82E0_488F_B6D3_9AA28CE011D8__INCLUDED_)
#define AFX_PAGELAUNCHMODE_H__9D6D3161_82E0_488F_B6D3_9AA28CE011D8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageLaunchMode.h : header file
//

#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageLaunchMode dialog

class CPageLaunchMode : public CMcSettingsPage
{
// Construction
public:
	CPageLaunchMode(LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageLaunchMode)
	enum { IDD = IDD_PAGE_CONECTIONS };
	BOOL	m_bLaunchOnStartup;
	BOOL	m_bAutoLogon;
	CEdit	m_ServerPortEdit;
	CEdit	m_ServerNameEdit;
	CString	m_ServerName;
	CString	m_ServerPort;
	BOOL	m_BypassCheck;
	CString	m_FireWallPass;
	CString	m_FireWallUser;
	BOOL	m_UserFireWall;
	BOOL	m_UseSSL;
	//}}AFX_DATA

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageLaunchMode)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	DWORD dwAccessType;
	
	// Generated message map functions
	//{{AFX_MSG(CPageLaunchMode)
	virtual BOOL OnInitDialog();
	afx_msg void OnAutoconnectCheck();
	afx_msg void OnLaunchonstartCheck();
	afx_msg void OnDefaultRadio();
	afx_msg void OnDirectRadio();
	afx_msg void OnProxyRadio();
	afx_msg void OnUsefirewallCheck();
	afx_msg void OnChangeFirewalluser();
	afx_msg void OnErrspaceFirewallpass();
	afx_msg void OnUseSSLCheck();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGELAUNCHMODE_H__9D6D3161_82E0_488F_B6D3_9AA28CE011D8__INCLUDED_)
