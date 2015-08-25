#if !defined(AFX_PAGEALERTS_H__0CE9B6A3_1571_4557_AC3A_51D4FBE04662__INCLUDED_)
#define AFX_PAGEALERTS_H__0CE9B6A3_1571_4557_AC3A_51D4FBE04662__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageAlerts.h : header file
//
#include "McSettingsDlg.h"
/////////////////////////////////////////////////////////////////////////////
// CPageAlerts dialog

class CPageAlerts : public CMcSettingsPage
{
// Construction
public:
	CPageAlerts(LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageAlerts)
	enum { IDD = IDD_PAGE_MES_ALERT };
	CListCtrl	m_AlertList;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageAlerts)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();
// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CPageAlerts)
	virtual BOOL OnInitDialog();
	afx_msg void OnClickAlertList(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEALERTS_H__0CE9B6A3_1571_4557_AC3A_51D4FBE04662__INCLUDED_)
