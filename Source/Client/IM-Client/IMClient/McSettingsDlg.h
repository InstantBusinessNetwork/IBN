#ifndef _MC_SETTINGS_DLG_H
#define _MC_SETTINGS_DLG_H

#pragma once

#include "Resource.h"
#include "OFSNcDlg2.h"

class CMcSettingsDlg;

//***********************************************************************************************
// CMcSettingsPage dialog

class CMcSettingsPage : public CDialog
{
	DECLARE_DYNAMIC(CMcSettingsPage)

	friend class CMcSettingsDlg;

public:
	CMcSettingsPage(UINT nIDTemplate, LPCTSTR szTitle);
	virtual ~CMcSettingsPage();

// Dialog Data
	enum { IDD = 0 };

private:
	void OnOK(void);
	void OnCancel(void);
	BOOL Create(CMcSettingsDlg* pParent);
	LPCTSTR GetTitle(void);

	CString m_strTitle;
	CMcSettingsDlg *m_pParent;
	UINT m_nTemplateID;

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	void SetModified(void);

	// Overridable
	virtual void Activate();
	virtual BOOL IsValidSettings(void);
	virtual BOOL SaveSettings(void);

public:
	DECLARE_MESSAGE_MAP()
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
};

//***********************************************************************************************
// CMcSettingsDlg dialog
#define MC_SETTINGS_DLG_PARENT CDialog

class CMcSettingsDlg : public MC_SETTINGS_DLG_PARENT
{
	DECLARE_DYNAMIC(CMcSettingsDlg)

public:
	CMcSettingsDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CMcSettingsDlg();

// Dialog Data
	//{{AFX_DATA(CMcSettingsDlg)
	enum { IDD = IDD_SETTINGS };
	CStatic	m_placeholder;
	CListCtrl	m_list;
	CButton m_btn2OK;
	CButton m_btn2Cancel;
	CButton m_btn2Apply;
	//}}AFX_DATA

	BOOL AddPage(CMcSettingsPage* pPage);
	void Clear(void);
	void RemovePage(CMcSettingsPage* pPage);
	void SetModified(BOOL bModified);
	afx_msg void OnLvnItemChanging_List(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnDestroy();
	virtual BOOL OnInitDialog();

	
protected:
		
	void AdjustWindowForPages();
	CRect m_rWindowInit;
	CRect m_rPlaceHolderInit;
	CRect m_rPlaceHolder;
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	CArray<CMcSettingsPage*, CMcSettingsPage*> m_aPages;
	int m_nCurrentPage;

	void ApplyChanges(bool bCloseDialog);
	void ClearList(void);
	void SelectPage(int nIndex);

	//{{AFX_MSG(CPageSound)
	afx_msg void OnButtonApply();
	virtual void OnOK();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

#endif