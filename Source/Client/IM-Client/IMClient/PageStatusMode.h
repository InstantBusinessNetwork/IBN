#if !defined(AFX_PAGESTATUSMODE_H__8ABCC854_CCB9_45DC_83A6_65AA03EDDDE1__INCLUDED_)
#define AFX_PAGESTATUSMODE_H__8ABCC854_CCB9_45DC_83A6_65AA03EDDDE1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageStatusMode.h : header file
//
#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageStatusMode dialog

class CPageStatusMode : public CMcSettingsPage
{
// Construction
public:
	CPageStatusMode(LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageStatusMode)
	enum { IDD = IDD_PAGE_STATUS_MODE };
	CSpinButtonCtrl	m_SetNASpin;
	CSpinButtonCtrl	m_SetAwaySpin;
	BOOL	m_SetAway;
	BOOL	m_SetNA;
	long	m_SetNAMinute;
	long	m_SetAwayMinute;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageStatusMode)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	void Refresh();

	// Generated message map functions
	//{{AFX_MSG(CPageStatusMode)
	virtual BOOL OnInitDialog();
	afx_msg void OnSetawayCheck();
	afx_msg void OnSetnaCheck();
	afx_msg void OnChangeSetawayminutesEdit();
	afx_msg void OnChangeSetnaminutesEdit();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGESTATUSMODE_H__8ABCC854_CCB9_45DC_83A6_65AA03EDDDE1__INCLUDED_)
