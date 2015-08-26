// SelectColorListBox.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "SelectColorListBox.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define SCLB_ITEM_WIDTH										(18)

#define SCLB_LEFT_COLOR_SPACE								(3)
#define SCLB_RIGHT_COLOR_SPACE								(5)
#define SCLB_TOP_COLOR_SPACE								(1)
#define SCLB_WIDTH_COLOR									(30)
#define SCLB_HEIGHT_COLOR									(14)
#define SCLB_RECT_CIRCLE_SIZE_COLOR							(3)

#define SCLB_TOP_TEXT_SPACE									(2)
#define SCLB_HEIGHT_TEXT									(30)

/////////////////////////////////////////////////////////////////////////////
// CSelectColorListBox

CSelectColorListBox::CSelectColorListBox()
{
}

CSelectColorListBox::~CSelectColorListBox()
{
}


BEGIN_MESSAGE_MAP(CSelectColorListBox, CListBox)
	//{{AFX_MSG_MAP(CSelectColorListBox)
	ON_WM_MEASUREITEM_REFLECT()
	ON_WM_DRAWITEM_REFLECT()
	ON_WM_LBUTTONDBLCLK()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSelectColorListBox message handlers

void CSelectColorListBox::MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct) 
{
	// TODO: Add your message handler code here
	lpMeasureItemStruct->itemHeight = SCLB_ITEM_WIDTH;
}

void CSelectColorListBox::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	DWORD ItemId = lpDrawItemStruct->itemData;

	CColorListItem	ItemInfo = m_ItemMap[ItemId];

	if(ItemInfo.Id!=0)
	{
		CDC dc;
		dc.Attach(lpDrawItemStruct->hDC);
		dc.SetBkMode(TRANSPARENT);
		
		COLORREF crOldTextColor		=	dc.GetTextColor();
		COLORREF crOldBkColor		=	dc.GetBkColor();

		CBrush	*pOldBrush	=	NULL, newBrush;
		
		if((lpDrawItemStruct->itemAction | ODA_SELECT) && (lpDrawItemStruct->itemState & ODS_SELECTED))
		{
			// Selected True [3/12/2002]
			dc.SetTextColor(::GetSysColor(COLOR_HIGHLIGHTTEXT));
			dc.SetBkColor(::GetSysColor(COLOR_HIGHLIGHT));
			newBrush.CreateSolidBrush(::GetSysColor(COLOR_HIGHLIGHT));
			dc.FillSolidRect(&lpDrawItemStruct->rcItem, ::GetSysColor(COLOR_HIGHLIGHT));
			pOldBrush = dc.SelectObject(&newBrush);

			if(pOldBrush)
			{
				dc.SelectObject(pOldBrush);
				pOldBrush = NULL;
			}
				
		}
		else
		{
			// Selected False [3/12/2002]
			dc.FillSolidRect(&lpDrawItemStruct->rcItem, crOldBkColor);
		}
		
		// Draw Item for Color [3/12/2002]

		CRect	ColorFrame(lpDrawItemStruct->rcItem.left + SCLB_LEFT_COLOR_SPACE,
						   lpDrawItemStruct->rcItem.top	 + SCLB_TOP_COLOR_SPACE,
						   lpDrawItemStruct->rcItem.left + SCLB_LEFT_COLOR_SPACE + SCLB_WIDTH_COLOR,
						   lpDrawItemStruct->rcItem.top  + SCLB_TOP_COLOR_SPACE + SCLB_HEIGHT_COLOR);

		CBrush brColor;
		brColor.CreateSolidBrush(ItemInfo.Color);

		pOldBrush = dc.SelectObject(&brColor);

		dc.RoundRect(ColorFrame,CPoint(SCLB_RECT_CIRCLE_SIZE_COLOR,SCLB_RECT_CIRCLE_SIZE_COLOR));

		if(pOldBrush)
		{
			dc.SelectObject(pOldBrush);
			pOldBrush  = NULL;
		}
			
		
		// Wtite Message ... [8/24/2002]
		LOGFONT lgFont	=	{0};
		
		lgFont.lfHeight				= -10;
		lgFont.lfWeight				= FW_BOLD;
		lgFont.lfCharSet			=	DEFAULT_CHARSET;

		_tcscpy(lgFont.lfFaceName, _T("Arial"));
		
		CFont	LabelFond, *pOldFont;
		LabelFond.CreateFontIndirect(&lgFont);
		pOldFont = dc.SelectObject(&LabelFond);

		CRect TextFarme	=	lpDrawItemStruct->rcItem;
		TextFarme.left = ColorFrame.right + SCLB_RIGHT_COLOR_SPACE;
		TextFarme.top +=  SCLB_TOP_TEXT_SPACE;
		TextFarme.bottom = TextFarme.top + SCLB_HEIGHT_TEXT;

		dc.DrawText(ItemInfo.Text,TextFarme,DT_LEFT|DT_VCENTER|DT_END_ELLIPSIS|DT_TABSTOP);

		dc.SelectObject(pOldFont);
	
		// Reset the background color and the text color back to their
		// original values.
		dc.SetTextColor(crOldTextColor);
		dc.SetBkColor(crOldBkColor);
		
		dc.Detach();
	}
}

int CSelectColorListBox::CompareItem(LPCOMPAREITEMSTRUCT lpCompreItem)
{
	CString str1 = m_ItemMap[lpCompreItem->itemData1].Text;
	CString str2 = m_ItemMap[lpCompreItem->itemData2].Text;

	return 	str1.Compare(str2);
}

int CSelectColorListBox::AddItem(DWORD dwId, LPCTSTR strText, DWORD dwColor)
{
	m_ItemMap[dwId] = CColorListItem(dwId, strText, dwColor);

	//int iPos = AddString(strText);
	//SetItemData(iPos,dwId);
	return (int)::SendMessage(GetSafeHwnd(), LB_ADDSTRING, 0, (LPARAM)dwId);;
}

DWORD CSelectColorListBox::GetColorById(DWORD dwId)
{
	return m_ItemMap[dwId].Color;
}

BOOL CSelectColorListBox::SelectItemById(DWORD dwId)
{
	for(int iListIndex = 0;iListIndex<GetCount();iListIndex++)
	{
		DWORD dwCurId = GetItemData(iListIndex);
		if(dwId==dwCurId)
		{
			SetCurSel(iListIndex);
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CSelectColorListBox::SelectColorDialogById(DWORD dwId)
{
	CColorListItem	ItemInfo = m_ItemMap[dwId];

	if(ItemInfo.Id)
	{
		CColorDialog	colorDlg(ItemInfo.Color,CC_ANYCOLOR,this);
		if(colorDlg.DoModal()==IDOK)
		{
			m_ItemMap[dwId].Color = colorDlg.GetColor();
			Invalidate();
		}
	}
	
	return TRUE;
}

void CSelectColorListBox::OnLButtonDblClk(UINT nFlags, CPoint point) 
{
	int iCurSel = GetCurSel();
	
	if(iCurSel!=LB_ERR&&GetCount()>0)
	{
		DWORD dwId = GetItemData(iCurSel);

		CColorListItem	ItemInfo = m_ItemMap[dwId];
		
		CColorDialog	colorDlg(ItemInfo.Color,CC_ANYCOLOR|CC_FULLOPEN,this);
		if(colorDlg.DoModal()==IDOK)
		{
			m_ItemMap[dwId].Color = colorDlg.GetColor();
			GetParent()->SendMessage(WM_COLOR_CHANGED,dwId);
			Invalidate();
		}
	}
	
	CListBox::OnLButtonDblClk(nFlags, point);
}

DWORD CSelectColorListBox::GetColorByIndex(int Index)
{
	DWORD dwId = GetItemData(Index);
	return GetColorById(dwId);
}
