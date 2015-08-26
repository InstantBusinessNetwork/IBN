#if !defined(AFX_PAGESOUND_H__92B80DE5_2AD4_4F2D_BD6B_58AA68712526__INCLUDED_)
#define AFX_PAGESOUND_H__92B80DE5_2AD4_4F2D_BD6B_58AA68712526__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageSound.h : header file
//
#include "McSettingsDlg.h"
/////////////////////////////////////////////////////////////////////////////
// CPageSound dialog

class CPageSound : public CMcSettingsPage
{
	// Construction
public:
	CPageSound(LPCTSTR szTitle);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPageSound)
	enum { IDD = IDD_PAGE_MES_SOUND };
	CListCtrl	m_SoundPathList;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageSound)
	public:
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	BOOL SaveSettings();

// Implementation
protected:
	int GetSelectedItem();
	void BlockOrUnBlock();
	// Generated message map functions
	//{{AFX_MSG(CPageSound)
	afx_msg void OnSelectButton();
	afx_msg void OnClearButton();
	afx_msg void OnTestButton();
	virtual BOOL OnInitDialog();
	afx_msg void OnItemchangedSoundpathList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkSoundpathList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnEndtrackSoundpathList(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGESOUND_H__92B80DE5_2AD4_4F2D_BD6B_58AA68712526__INCLUDED_)
