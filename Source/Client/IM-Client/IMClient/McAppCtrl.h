// McAppCtrl.h: interface for the CMcAppCtrl class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MCAPPCTRL_H__AE32A960_D4BC_435E_932F_62FEC4E9057A__INCLUDED_)
#define AFX_MCAPPCTRL_H__AE32A960_D4BC_435E_932F_62FEC4E9057A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CMcAppCtrl
{
public:
	friend class CPopupItem;

	void ShowAppMenu(CPoint &point);
	void SetSpacing(long cy);
	int GetMaxShowItemFromCY(int CY);
	void SetPos(int x, int y);
	BOOL SetMoreBtnImage(LPPICTUREDISP pImage);
	void DrawSingleItem(CDC *pDC,int ImageIndex, CPoint point);
	CSize GetImageSize();
	int GetButtonCount();
	int GetMaxShowItem();
	int SetMaxShowItem(int Value=4);
	int OnLButtonUp(UINT nFlags, CPoint point);
	int OnLButtonDown(UINT nFlags, CPoint point);
	void SetFont(const LOGFONT* lpLogFont );
	int  HitTest(CPoint point);
	BOOL TranslateMessage(MSG* pMsg);
	void Draw(CDC *pDc);
	void DeleteAllButton();
	BOOL IsCreated();
	void Create(int x,int y,CWnd* pParent);
	int SetCheckButton(int CheckItem);
	int GetCheckButton();
	LPPICTUREDISP GetImage();
	BOOL SetImage(LPPICTUREDISP pImage);
	int AddButton(LPCTSTR Name, LPCTSTR Tooltip, LPPICTURE Icon);
	CMcAppCtrl();
	virtual ~CMcAppCtrl();
	
private:
	struct McAppCtrl
	{	
		CString strName;
		CString strToolTip;
		CRect	rtBody;
		CPictureHolder	picIcon;
		CRect	rtIcon;
	};

		class CPopupItem : public CMenu
		{
			// Construction
		public:
			CPopupItem(DWORD MessageId = 20000);
			
			// Attributes
		public:
			DWORD m_MessageId;
			
		public:
			BOOL Create(CMcAppCtrl *pParentCtrl, int ColumnItem);
			virtual ~CPopupItem();
			virtual void DrawItem( LPDRAWITEMSTRUCT lpDIS);
			virtual void MeasureItem( LPMEASUREITEMSTRUCT lpMIS);
		private:
		CMcAppCtrl *m_pParentCtrl;
		int			m_Cx;
		int			m_Cy;
		int			m_ColumnItem;
	};
	

	CArray<McAppCtrl*,McAppCtrl*>	m_ButtonArray;

protected:
	long m_nSpacing;
	void RefreshButtons();

	int					m_CheckButton;
	CRect				m_WindowRect;	
	CWnd*				m_pParentWnd;
	CToolTipCtrl*		m_pToolTip;
	CPictureHolder		m_Pict, m_MoreBtnPict;
	CSize				m_ImageSizePxl, m_ImageSizeHi;
	CSize				m_MoreBtnImageSizePxl, m_MoreBtnImageSizeHi;
	CFont				m_TextFont;
	int					m_ShowMaxItem;
	CPopupItem			m_MoreItemPopup;
	COLORREF			m_dwBkColor;
};

#endif // !defined(AFX_MCAPPCTRL_H__AE32A960_D4BC_435E_932F_62FEC4E9057A__INCLUDED_)
