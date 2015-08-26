#if !defined(AFX_OFSNCDLG2_H__44448F94_96DC_4479_9808_00937EBA5366__INCLUDED_)
#define AFX_OFSNCDLG2_H__44448F94_96DC_4479_9808_00937EBA5366__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// OFSNcDlg2.h : header file
//

#include "OFSNcDlg.h"
#include "ResizableImage.h"	// Added by ClassView
#include "MCButton.h"
#include "MCProgress.h"
#include "Label.h"

struct CTL_COLOR
{
	COLORREF crBG;
	COLORREF crText;
	COLORREF crBGD;		// disabled
	COLORREF crTextD;	// disabled
	
	CTL_COLOR()
	{
		Clear();
	}
	
	void Clear()
	{
		crBG = CLR_NONE;
		crText = CLR_NONE;
		crBGD = CLR_NONE;
		crTextD = CLR_NONE;
	}
	
	inline BOOL IsValidColor()
	{
		return crBG != CLR_NONE && crText != CLR_NONE && crBGD != CLR_NONE && crTextD != CLR_NONE;
	}
};

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg2 dialog
#define OFS_NCDLG2_PARENT COFSNcDlg

class COFSNcDlg2 : public OFS_NCDLG2_PARENT
{
// Construction
public:
	BOOL GetCtlColor(CDC *pDC, CWnd *pWnd, UINT nCtlColor, HBRUSH *pBrush);
	COFSNcDlg2(UINT nIDTemplate, CWnd* pParentWnd);   // standard constructor

// Dialog Data
	//{{AFX_DATA(COFSNcDlg2)
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COFSNcDlg2)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	BOOL m_bIgnoreActivate;
	BOOL LoadLabel(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CLabel *pLbl, BOOL bVisible);
	BOOL LoadProgress(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CMcProgress *pMcProgress, BOOL bVisible);
	void LoadColor(IXMLDOMNode *pRoot, LPCTSTR szName, COLORREF &cr);
	HRGN m_rgnTL;
	HRGN m_rgnTR;
	HRGN m_rgnBL;
	HRGN m_rgnBR;
	void LoadRegion(IXMLDOMNode *pXmlRoot, LPCTSTR szName, HRGN *phrgn);
	void LoadRectangle2(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, HWND hWnd, BOOL bVisible, BOOL bDontHide = FALSE);
	void LoadPictureFragment(IXMLDOMNode *pFragment, CPictureObj *pPictureObj);
	BOOL LoadRectangle(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, CRect& r);
	void LoadPicture(IXMLDOMNode *pPicture, CPictureObj **ppPictureObj = NULL);
	long LoadPictures(IXMLDOMNode *pXmlRoot);
	void LoadPicture(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CPictureObj **ppPictureObj);
	void CenterRect(CRect &r);
	void LoadColor(IXMLDOMNode *pColors, LPCTSTR szNodeName, CTL_COLOR &cr);
	void LoadColors(IXMLDOMNode *pXmlRoot);
	void LoadRectangle(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, CWnd *pWnd, BOOL bVisible, BOOL bDontHide = FALSE);
	HRESULT GetNodeAttribute(IXMLDOMNode *pNode, BSTR bsAttrName, CComBSTR &strAttrValue);
	HRESULT GetNodeAttributeAsLong(IXMLDOMNode *pNode, BSTR bsAttrName, long *pAttrValue, int nBase);
	HRESULT SelectChildNode(IXMLDOMNode *pNodeParent, BSTR bsSelect, IXMLDOMNode **ppNodeChild, BSTR *pbsNodeText);

	virtual void LoadSkin(IXMLDOMNode *pXmlRoot);

	void DrawBackground(CDC *pDC);
	BOOL LoadResizeSettings(IXMLDOMNode *pObject, CSize& szTL, CSize& szBR);
	BOOL LoadButton(IXMLDOMNode *pXmlRoot, LPCTSTR szButtonName, CMcButton *pBtn, BOOL bAutoPressed, BOOL bCanStayPressed, int Type = 0);	// 1=Maximize, 2=Restore
	void AdjustRect(CRect &r);
	void LoadRect(IXMLDOMNode *pParent, LPCTSTR szNodename, CRect& r);
	BOOL LoadWindow(IXMLDOMNode *pXmlRoot, CRect &r, CRect &rMin, CRect &rMax);
	void LoadSkinXML(IXMLDOMDocument **ppDoc, IXMLDOMNode **ppRoot);
	void LoadSkin();

	BOOL m_bLoadSkin;
	BOOL m_bResizable;
	BOOL m_bRound;
	CString m_strSkinSettings;
	BOOL m_bBackgroundPicture;
	CMcButton* m_pBtnMax;
	CMcButton* m_pBtnRestore;
	CArray<CPictureObj*, CPictureObj*> m_aPictures;

	CBrush m_brush;
	CTL_COLOR m_crButton;
	CTL_COLOR m_crDialog;
	CTL_COLOR m_crEdit;
	CTL_COLOR m_crList;
	CTL_COLOR m_crStatic;
	CTL_COLOR m_crScroll;

	CToolTipCtrl	m_ToolTip;
	
	// Generated message map functions
	//{{AFX_MSG(COFSNcDlg2)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	virtual BOOL OnInitDialog();
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnDestroy();
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnPaint();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized);
	afx_msg void OnWindowPosChanging(WINDOWPOS FAR* lpwndpos);
	virtual	BOOL PreTranslateMessage(LPMSG pMsg);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_OFSNCDLG2_H__44448F94_96DC_4479_9808_00937EBA5366__INCLUDED_)
