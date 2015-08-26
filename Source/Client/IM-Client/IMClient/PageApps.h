#if !defined(AFX_PAGEAPPS_H__C361A73B_DDD4_46F1_B656_45B683457FD4__INCLUDED_)
#define AFX_PAGEAPPS_H__C361A73B_DDD4_46F1_B656_45B683457FD4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PageApps.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CPageApps dialog
#include "McSettingsDlg.h"
//#include "GlobalMessengerDef.h"

class CPageApps : public CMcSettingsPage
{
// Construction
public:
	CPageApps(CArray<McAppItem,McAppItem> &AppArray,	long UserId,	LPCTSTR UserRole,	LPCTSTR szTitle);   // standard constructor

	void OnDropped();

public:
	class CAppCDragListBox:public CDragListBox
	{
	public:
		CAppCDragListBox(CPageApps *pPageApps	=	NULL);
		virtual void Dropped(int nSrcIndex, CPoint pt);
		virtual void MeasureItem( LPMEASUREITEMSTRUCT lpMeasureItemStruct );
		virtual void DrawItem( LPDRAWITEMSTRUCT lpDrawItemStruct );
	private:
		CPageApps *m_pPageApps;
	};
	friend CAppCDragListBox;
// Dialog Data
	//{{AFX_DATA(CPageApps)
	enum { IDD = IDD_PAGE_STUBS };
	CAppCDragListBox	m_AppList;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPageApps)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	BOOL SaveSettings();
	// Generated message map functions
	//{{AFX_MSG(CPageApps)
	virtual BOOL OnInitDialog();
	afx_msg void OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	CString m_strUserId;
	CString m_strUserRole;
	CArray<McAppItem,McAppItem> m_AppArray;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PAGEAPPS_H__C361A73B_DDD4_46F1_B656_45B683457FD4__INCLUDED_)
