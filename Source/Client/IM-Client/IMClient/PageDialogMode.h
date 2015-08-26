#if !defined(AFX_PAGEDIALOGMODE_H__EA78EF8F_2D2E_4894_8E6D_B37E68EAEE34__INCLUDED_)
#define AFX_PAGEDIALOGMODE_H__EA78EF8F_2D2E_4894_8E6D_B37E68EAEE34__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageDialogMode.h : header file
//

#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageDialogMode dialog

class CPageDialogMode : public CMcSettingsPage
{
// Construction
public:
	CPageDialogMode(LPCTSTR szTitle);

// Dialog Data
	//{{AFX_DATA(CPageDialogMode)
	enum { IDD = IDD_PAGE_DIALOGMODE };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageDialogMode)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	int DialogMode;
	// Generated message map functions
	//{{AFX_MSG(CPageDialogMode)
	virtual BOOL OnInitDialog();
	afx_msg void OnIcqRadio();
	afx_msg void OnYahooRadio();
	afx_msg void OnMcmessengerRadio();
	afx_msg void OnMcmessengerSundanceeditionRadio();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEDIALOGMODE_H__EA78EF8F_2D2E_4894_8E6D_B37E68EAEE34__INCLUDED_)
