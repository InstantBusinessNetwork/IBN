// PageMesTemplate.h: interface for the CPageMesTemplate class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_PAGEMESTEMPLATE_H__2003DB9A_227A_40D3_A156_5099B5B99131__INCLUDED_)
#define AFX_PAGEMESTEMPLATE_H__2003DB9A_227A_40D3_A156_5099B5B99131__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageAlerts.h : header file
//
#include "McSettingsDlg.h"
/////////////////////////////////////////////////////////////////////////////
// CPageMesTemplates dialog

class CPageMesTemplates : public CMcSettingsPage
{
// Construction
public:
	CPageMesTemplates(long UserId, LPCTSTR UserRole,  LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageMesTemplates)
	enum { IDD = IDD_PAGE_MES_TEMPLATE };
	CListCtrl	m_TemplateList;
	BOOL		m_ShowAuto;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageMesTemplates)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();
// Implementation
protected:
	CString		m_UserRole;
	long			m_UserId;


	// Generated message map functions
	//{{AFX_MSG(CPageMesTemplates)
	virtual BOOL OnInitDialog();
	afx_msg void OnClickMesTemplateList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnItemchangedMesTemplateList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnAddTemplateButton();
	afx_msg void OnEditTemplateButton();
	afx_msg void OnDeleteTemplateButton();
	afx_msg void OnShowAuto();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

#endif // !defined(AFX_PAGEMESTEMPLATE_H__2003DB9A_227A_40D3_A156_5099B5B99131__INCLUDED_)
