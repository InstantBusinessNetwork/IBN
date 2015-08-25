#if !defined(_OFSNCDLG_H)
#define _OFSNCDLG_H

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// OFSNcDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
/// Name   : Custom Dialog Caption. 
/// Author : Zhuk Oleg (OlegO)  
/// Mail   : zhukoo@mail.ru
/// Country: Russian Federation. Kaliningrad region. Kalinigrad city.
/// Version: 23.01.2001
/////////////////////////////////////////////////////////////////////////////
/// Description: Use COFSNcDlg to write you Dialog with Custom Draw Caption
///              and Custom Draw Boundary.
/// Best Regards OlegO.
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg dialog
#include "ResizableDialog.h"

class COFSNcDlg : public CResizableDialog
{
// Construction
public:
	COFSNcDlg();   // standard constructor
	COFSNcDlg(UINT nIDTemplate, CWnd* pParentWnd = NULL);
	COFSNcDlg(LPCTSTR lpszTemplateName, CWnd* pParentWnd = NULL);
	~COFSNcDlg();   // standard destructor

public:
	void SetButtonMaxiRestore(HBITMAP m_MaxiBMP, HBITMAP m_RestoryBMP);
	void SetButtonMini(HBITMAP m_MiniBMP);
	void SetButtonClose(HBITMAP m_CloseBMP);
	void SetBoundaryColor(COLORREF BoundaryColor);
	void SetBoundaryBMP(HBITMAP hBondary);
	void SetCaption(HBITMAP m_ActiveBMP, HBITMAP m_InactiveBMP, BOOL bZoom);
	void SetCaption(HBITMAP m_ActiveBMP, BOOL bZoom = FALSE);
	void SetCaption(COLORREF ActiveColor, COLORREF UnActiveColor , int CaptionHeight = 20);
	void GetBoundary(int &cx,int &cy);
	void SetBoundary(int cx = 3, int cy = 3);

// Implementation
protected:
	enum CAPTION_MODE
	{
		CAP_NONE = 0,
		CAP_COLOR,
		CAP_BITMAP_1,
		CAP_BITMAP_2,
		CAP_BITMAP_1_ZOOM,
		CAP_BITMAP_2_ZOOM,
	};
protected:	
	CBitmap *pCloseBMP, *pMiniBMP, *pRestoryBMP, *pMaxiBMP;
	BOOL bButtonDown;
	BOOL bSizeNow;
	UINT SizeMode;
	CPoint m_DownPoint;
	BOOL bMoveNow;
	CBitmap *pBoundaryBMP;
	COLORREF m_BoundaryColor;
	CSize   m_ActiveBMPSize, m_InactiveBMPSize;
	CBitmap *pActiveBMP, *pInactiveBMP;
	void DrawCaption(CDC &dc,const CRect &m_Rect);
	COLORREF m_ActiveColor, m_InactiveColor;
	CAPTION_MODE m_CaptionMode;
	int CaptionH;
	int BoundaryX, BoundaryY;
	BOOL m_bActive;
	void Construct();
	// Generated message map functions
	//{{AFX_MSG(COFSNcDlg)
	afx_msg void OnNcPaint( );
	afx_msg BOOL OnNcActivate(BOOL bActive);
	afx_msg void OnNcCalcSize( BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp );
	afx_msg void OnNcLButtonDown( UINT nHitTest, CPoint point);
	afx_msg void OnNcMouseMove( UINT nHitTest, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnCaptureChanged( CWnd* pWnd );
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(_OFSNCDLG_H)
