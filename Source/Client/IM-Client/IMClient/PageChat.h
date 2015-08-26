#if !defined(AFX_PAGECHAT_H__8C64B8C7_4B38_4D49_AC4F_63FBF1AFCBFE__INCLUDED_)
#define AFX_PAGECHAT_H__8C64B8C7_4B38_4D49_AC4F_63FBF1AFCBFE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageChat.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CPageChat dialog
#include "McSettingsDlg.h"
#include "SelectColorListBox.h"

class CPageChat : public CMcSettingsPage
{
// Construction
public:
	CPageChat(LPCTSTR UserRole, int UserId, LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageChat)
	enum { IDD = IDD_PAGE_CHAT };
	CSelectColorListBox	m_InformationColorListBox;
	CSelectColorListBox	m_UserColorListBox;
	CSpinButtonCtrl		m_LoadLastMesSpin;
	long	m_lShowMessages;
	//}}AFX_DATA

	CString		strRegUserSection;
	BOOL		m_bWasChanged;
	BOOL		m_bWasSaved;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageChat)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	CString m_UserRole;
	int		m_UserId;
	
	// Generated message map functions
	//{{AFX_MSG(CPageChat)
	afx_msg LPARAM OnColorChanged(WPARAM w, LPARAM l);
	virtual BOOL OnInitDialog();
	afx_msg void OnChangeLoadLastmesEdit();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGECHAT_H__8C64B8C7_4B38_4D49_AC4F_63FBF1AFCBFE__INCLUDED_)
