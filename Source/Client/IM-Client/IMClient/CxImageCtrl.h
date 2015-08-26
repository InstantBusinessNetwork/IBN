#if !defined(AFX_CXIMAGECTRL_H__1E8FCD38_E55A_4EEB_AE07_806974FB2B8F__INCLUDED_)
#define AFX_CXIMAGECTRL_H__1E8FCD38_E55A_4EEB_AE07_806974FB2B8F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CxImageCtrl.h : header file
//
#include "..\CxImage\ximage.h"
#include "TextComment.h"
/////////////////////////////////////////////////////////////////////////////
// CCxImageCtrl window

#include <list>

#define WM_UNDO_UPDATED			(WM_USER+1)

class CCxImageCtrl : public CWnd
{
// Construction
public:
	CCxImageCtrl();

	enum CCxImageCtrlState
	{
		IS_Move	=	0,	//Default
		IS_Crop,
		IS_Pen,
		IS_Text,
		IS_TextMove
	};

	typedef std::list<CxImage*> CImageUndoList, *LPCImageUndoList;
	typedef CImageUndoList::iterator	CImageUndoEnum, *LPCImageUndoEnum;
/*
	typedef std::list<CTextComment*> CTextCommentList, *LPCTextCommentList;
	typedef CTextCommentList::iterator	CTextCommentEnum, *LPCTextCommentEnum;
*/

// Attributes

// Operations
public:
	bool CreateFromHBITMAP (HBITMAP hbmp, HPALETTE hpal=0);
	void	SetStretchMode(BOOL bIsStretch	=	TRUE);
	BOOL	GetStretchMode();
	void	SetZoomValue(double ZoomValue = 1.0);
	double	GetZoomValue();
	void	SetQuality(BYTE q);
	BYTE	GetQuality();
	void	SetState(CCxImageCtrl::CCxImageCtrlState State = CCxImageCtrl::IS_Move);
	CCxImageCtrl::CCxImageCtrlState	GetState();

	BOOL	Cancel();
	void	Refresh();
	bool	ShowSaveFileDialog();
	bool	Save (const TCHAR* filename, DWORD imagetype);

	bool	IsUndoEnable();
	void	Undo();
	bool	IsRedoEnable();
	void	Redo();

	void		SetPenColor(COLORREF Color);
	COLORREF	GetPenColor();

	void		SetTextColor(COLORREF Color);
	COLORREF	GetTextColor();

	void		SetFontName(LPCTSTR Name);
	CString		GetFontName();

	void	SetTextSize(int Size);
	int		GetTextSize();

	BOOL	IsSaved();
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCxImageCtrl)
	protected:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CCxImageCtrl();

	// Generated message map functions
protected:
	void SetScrollSize(int cx, int cy);
	void ClientToImage(CPoint &point);
	void ImageToClient(CPoint &point);
	void SaveImageToUndo();

	CxImage		m_hImage;
	CxImage		m_hImageBuffer;
	int			m_nCurHeight;
	int			m_nCurWidth;

	int			m_nHScrollPos;
	int			m_nVScrollPos;
	

	DWORD		m_dwWidth;
	DWORD		m_dwHeight;

	BOOL		m_bStretchMode;
	double		m_iZoomValue;
	BYTE		m_btQuality;

	CBrush		m_brHatch;

	CCxImageCtrl::CCxImageCtrlState	m_State;

	CSize		m_RefScroll;
	CSize		m_RefOffset;
	CPoint		m_RefPoint;
	BOOL		m_bCaptureWasSet;

	CPoint		m_CropActivePoint;

	CImageUndoList	m_UndoList;
	long			m_iUndoListCurrPosition;
	bool			m_bCurrentImageWasSave;
	bool			m_bIsSaved;

	COLORREF		m_PenColor;
	COLORREF		m_TextColor;
	int				m_TextSize;
	CString			m_FontName;

	//CTextCommentList m_TextCommentList;
	CTextComment*	m_pTextComment;
	
	//{{AFX_MSG(CCxImageCtrl)
	afx_msg void OnPaint();
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CXIMAGECTRL_H__1E8FCD38_E55A_4EEB_AE07_806974FB2B8F__INCLUDED_)
