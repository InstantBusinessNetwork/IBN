#if !defined(AFX_SELECTCOLORLISTBOX_H__17760CEE_1B12_4BEB_B536_84CFBDF94EA1__INCLUDED_)
#define AFX_SELECTCOLORLISTBOX_H__17760CEE_1B12_4BEB_B536_84CFBDF94EA1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SelectColorListBox.h : header file
//
#include <map>

using namespace std;

/////////////////////////////////////////////////////////////////////////////
// CSelectColorListBox window
#define WM_COLOR_CHANGED	(WM_USER+333)

class CSelectColorListBox : public CListBox
{
// Construction
public:
	CSelectColorListBox();

// Child struct
public:
	class CColorListItem
	{
	public:
		CColorListItem()
		{
			Id		= 0;
			Color	= 0;
		}
		CColorListItem(const CColorListItem& Item)
		{
			Id = Item.Id;
			Color = Item.Color;
			Text = Item.Text;
		}
		CColorListItem(DWORD dwId, LPCTSTR strText, DWORD dwColor)
		{
			Id = dwId;
			Color = dwColor;
			Text = strText;
		}
	public:
		// Params [8/24/2002]
		DWORD	Id;
		DWORD	Color;
		CString Text;
		// operators [8/24/2002]
	public:
		const CColorListItem& operator=(const CColorListItem& Item)
		{
			Id = Item.Id;
			Color = Item.Color;
			Text = Item.Text;

			return  *this;
		}
	};
	
// Attributes
public:
	map< DWORD ,CColorListItem >		m_ItemMap;
// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSelectColorListBox)
	//}}AFX_VIRTUAL

// Implementation
public:
	DWORD GetColorByIndex(int Index);
	BOOL SelectColorDialogById(DWORD dwId);
	BOOL SelectItemById(DWORD dwId);
	DWORD GetColorById(DWORD dwId);
	int AddItem(DWORD dwId, LPCTSTR strText, DWORD dwColor);
	virtual ~CSelectColorListBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CSelectColorListBox)
	afx_msg void MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	afx_msg void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	afx_msg virtual int CompareItem(LPCOMPAREITEMSTRUCT lpCompreItem);
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SELECTCOLORLISTBOX_H__17760CEE_1B12_4BEB_B536_84CFBDF94EA1__INCLUDED_)
