#if !defined(AFX_PAGEGENERAL_H__25723B57_6FF6_48D6_B8E0_83AD7766C54B__INCLUDED_)
#define AFX_PAGEGENERAL_H__25723B57_6FF6_48D6_B8E0_83AD7766C54B__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageGeneral.h : header file
//

#include "McSettingsDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CPageGeneral dialog

class CPageGeneral : public CMcSettingsPage
{
// Construction
public:
	CPageGeneral(LPCTSTR szTitle);

// Dialog Data
	//{{AFX_DATA(CPageGeneral)
	enum { IDD = IDD_PAGE_MES_GENERAL };
	BOOL	m_ShowNew;
	BOOL	m_AnimationClose;
	BOOL	m_HideMpa;
	BOOL	m_bKeepTop;
	BOOL	m_bShowOffline;
	BOOL	m_bRemoveTaskBar;
	BOOL	m_bShowSendTo;
	int		m_lDefaultView;
	int		m_lIBNActionBrowser;
	BOOL	m_bOpenInMaximaze;
	BOOL	m_bCreateOfflineFolder;
	BOOL	m_bMinimizeOnClose;
	BOOL	m_bGetDefBrowserFromRegistry;
	BOOL	m_bContactListSortByFirstName;
	BOOL	m_bUpdateAutoCheck;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageGeneral)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	int MessageMode;
	// Generated message map functions
	//{{AFX_MSG(CPageGeneral)
	virtual BOOL OnInitDialog();
	afx_msg void OnSingleRadio();
	afx_msg void OnSplitRadio();
	afx_msg void OnShownewmessageCheck();
	afx_msg void OnAnimationCheck();
	afx_msg void OnHidempaintrayCheck();
	afx_msg void OnKeeptopCheck();
	afx_msg void OnShowofflinecheck();
	afx_msg void OnCreateOfflineFolderCheck();
	afx_msg void OnRemoveIbnFromTaskBarCheck();
	afx_msg void OnShowSendTo();
	afx_msg void OnMinimizeOnCloseCheck();
	//  [12/15/2003]
	afx_msg void OnIBNActionsView();
	afx_msg void OnContactListView();
	//  [12/16/2003]
	afx_msg void OnIBNActionsViewInIBNMiniBrowser();
	afx_msg void OnIBNActionsViewInExternalBrowser();
	//  [12/28/2006]
	void OnGetDefBrowserFromRegistry();
	void OnContactlistSortByFirstName();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEGENERAL_H__25723B57_6FF6_48D6_B8E0_83AD7766C54B__INCLUDED_)
