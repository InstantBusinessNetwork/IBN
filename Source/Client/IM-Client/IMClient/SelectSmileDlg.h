#if !defined(AFX_SELECTSMILEDLG_H__D0C7EC6F_6092_4454_A9AA_6110281C6817__INCLUDED_)
#define AFX_SELECTSMILEDLG_H__D0C7EC6F_6092_4454_A9AA_6110281C6817__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SelectSmileDlg.h : header file
//

#include "OfsNCDlg2.h"
#include "Label.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "SortHeaderCtrl.h"
#include "PictureEx.h"

class CMainDlg;

/////////////////////////////////////////////////////////////////////////////
// CSelectSmileDlg dialog

class CSelectSmileDlg : public COFSNcDlg2
{
// Construction
public:
	CSelectSmileDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CSelectSmileDlg)
	enum { IDD = IDD_SELECTSMILE_DIALOG };
	CMcButton	m_btnX;
	CMcButton	m_btnOK;
	CListCtrl   m_SmileList;
	CPictureEx  m_SmilePreview;
	//}}AFX_DATA

	int GetSelectedSmileIndex()
	{
		return m_SelectedSmileIndex;
	}


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSelectSmileDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

private:
	//CSortHeaderCtrl	m_SortHeader;
	CImageList m_SmileImageList;
	int m_SelectedSmileIndex;


// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	// Generated message map functions
	//{{AFX_MSG(CSelectSmileDlg)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnClose();
	afx_msg void OnItemchangedSmileList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkSmileList(NMHDR* pNMHDR, LRESULT* pResult);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG

	void OnClickMcclose();
	void OnClickMcOK() ;
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SELECTSMILEDLG_H__D0C7EC6F_6092_4454_A9AA_6110281C6817__INCLUDED_)
