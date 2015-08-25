#if !defined(AFX_PAGECONTACTLISTMODE_H__8F2A2948_3E6C_4F28_8671_412974EDF99C__INCLUDED_)
#define AFX_PAGECONTACTLISTMODE_H__8F2A2948_3E6C_4F28_8671_412974EDF99C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageContactListMode.h : header file
//
#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageContactListMode dialog

class CPageContactListMode : public CMcSettingsPage
{
// Construction
public:
	CPageContactListMode(LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageContactListMode)
	enum { IDD = IDD_PAGE_CONTACTLIST };
	BOOL	m_bShowOffline;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageContactListMode)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	int CLMode;		
	BOOL SaveSettings();
	// Generated message map functions
	//{{AFX_MSG(CPageContactListMode)
	virtual BOOL OnInitDialog();
	afx_msg void OnClmode1Radio();
	afx_msg void OnClmode2Radio();
	afx_msg void OnShowofflinecheck();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGECONTACTLISTMODE_H__8F2A2948_3E6C_4F28_8671_412974EDF99C__INCLUDED_)
