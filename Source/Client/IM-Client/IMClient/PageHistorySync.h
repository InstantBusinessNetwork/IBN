#if !defined(AFX_PAGEHISTORYSYNC_H__395B583C_EE5E_469A_9979_3CD62FDA6F63__INCLUDED_)
#define AFX_PAGEHISTORYSYNC_H__395B583C_EE5E_469A_9979_3CD62FDA6F63__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageHistorySync.h : header file
//

#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageHistorySync dialog

class CPageHistorySync : public CMcSettingsPage
{
// Construction
public:
	CPageHistorySync(LPCTSTR szTitle);

// Dialog Data
	//{{AFX_DATA(CPageHistorySync)
	enum { IDD = IDD_PAGE_MES_SYNCHRONIZE };
	CTime	m_FromTime;
	CTime	m_ToTime;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageHistorySync)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	int Mode;
	// Generated message map functions
	//{{AFX_MSG(CPageHistorySync)
	virtual BOOL OnInitDialog();
	afx_msg void OnMonthRadio();
	afx_msg void OnWeekRadio();
	afx_msg void OnYearRadio();
	afx_msg void OnDayRadio();
	afx_msg void OnCustomRadio();
	afx_msg void OnDateTimeChangeFrom(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDateTimeChangeTo(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEHISTORYSYNC_H__395B583C_EE5E_469A_9979_3CD62FDA6F63__INCLUDED_)
