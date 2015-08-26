// McAppCtrl.cpp: implementation of the CMcAppCtrl class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "McAppCtrl.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMcAppCtrl::CMcAppCtrl()
{
	m_pToolTip		=	NULL;
	m_pParentWnd	=	NULL;
	m_CheckButton	=	-1;
	m_ShowMaxItem	=	4;
	m_ImageSizePxl = m_ImageSizeHi = m_MoreBtnImageSizePxl = m_MoreBtnImageSizeHi = CSize(0,0);
	m_nSpacing = 0;
}

CMcAppCtrl::~CMcAppCtrl()
{
	DeleteAllButton();

	if(m_pToolTip)
	{
		delete m_pToolTip;
		m_pToolTip = NULL;
	}
}

int CMcAppCtrl::AddButton(LPCTSTR Name, LPCTSTR Tooltip, LPPICTURE Icon)
{
	ASSERT(m_pParentWnd!=NULL);
	McAppCtrl *newItem	=	new	McAppCtrl;
	newItem->strName		=	Name;
	newItem->strToolTip		=	Tooltip;
	newItem->picIcon.SetPictureDispatch((LPPICTUREDISP)Icon);
	int RetVal =  m_ButtonArray.Add(newItem);
	RefreshButtons();
	return RetVal;
}

BOOL CMcAppCtrl::SetImage(LPPICTUREDISP pImage)
{
	m_Pict.SetPictureDispatch(pImage);
	if(m_Pict.m_pPict==NULL)
		return FALSE;
	
	m_Pict.m_pPict->get_Width(&m_ImageSizeHi.cx);
	m_Pict.m_pPict->get_Height(&m_ImageSizeHi.cy);
	
	HiMetricToPixel(&m_ImageSizeHi,&m_ImageSizePxl);
	
	return TRUE;
}

BOOL CMcAppCtrl::SetMoreBtnImage(LPPICTUREDISP pImage)
{
	m_MoreBtnPict.SetPictureDispatch(pImage);
	if(m_MoreBtnPict.m_pPict==NULL)
		return FALSE;
	
	m_MoreBtnPict.m_pPict->get_Width(&m_MoreBtnImageSizeHi.cx);
	m_MoreBtnPict.m_pPict->get_Height(&m_MoreBtnImageSizeHi.cy);
	
	HiMetricToPixel(&m_MoreBtnImageSizeHi,&m_MoreBtnImageSizePxl);
	
	return TRUE;
}


LPPICTUREDISP CMcAppCtrl::GetImage()
{
	return m_Pict.GetPictureDispatch();
}


int CMcAppCtrl::GetCheckButton()
{
	return m_CheckButton;
}

int CMcAppCtrl::SetCheckButton(int CheckItem)
{
	int OldValue = m_CheckButton;
	m_CheckButton = CheckItem;
	return OldValue;
}

void CMcAppCtrl::Create(int x,int y, CWnd *pParent)
{
	ASSERT(pParent!=NULL);
	if(!pParent)
		return;
	m_pParentWnd		=	pParent;
	m_pToolTip			=	new CToolTipCtrl;
	m_pToolTip->Create(pParent);
	m_pToolTip->Activate(TRUE);
	m_WindowRect.left	=	x;
	m_WindowRect.top	=	y;
}

BOOL CMcAppCtrl::IsCreated()
{
	return (m_pParentWnd!=NULL)&&IsWindow(m_pParentWnd->GetSafeHwnd()); 
}

void CMcAppCtrl::DeleteAllButton()
{
	int iSize = m_ButtonArray.GetSize();
	
	for(int i=0;i<iSize;i++)
	{
		delete m_ButtonArray[i];
	}
	m_ButtonArray.RemoveAll();
	
	if(m_pToolTip)
	{
		delete m_pToolTip;
		m_pToolTip = NULL;
	}
	m_pToolTip			=	new CToolTipCtrl;
	m_pToolTip->Create(m_pParentWnd);
	m_pToolTip->Activate(TRUE);
}

void CMcAppCtrl::Draw(CDC *pDc)
{
	RECT rbBoundRect = {0,0,0,0};
	
	int iSize = m_ButtonArray.GetSize();
	
	for(int i=0;i<GetMaxShowItem();i++)
	{
		McAppCtrl	*pItem = m_ButtonArray[i];
		
		if(m_Pict.m_pPict!=NULL)
			m_Pict.m_pPict->Render(pDc->GetSafeHdc(), m_WindowRect.left, m_WindowRect.top+i*m_ImageSizePxl.cy+m_nSpacing*i,
			m_ImageSizePxl.cx/2, m_ImageSizePxl.cy, (m_CheckButton==i)?(m_ImageSizeHi.cx/2):0, m_ImageSizeHi.cy-1,
			m_ImageSizeHi.cx/2, -m_ImageSizeHi.cy, &rbBoundRect);
		
		if(pItem->picIcon.m_pPict)
		{
			pItem->picIcon.Render(pDc,pItem->rtIcon,rbBoundRect);
		}
		else
		{
			pDc->SetBkMode(TRANSPARENT);
			pDc->SetTextColor(0xffffff);

			if(m_TextFont.m_hObject)
			{
				CFont *oldFont = pDc->SelectObject(&m_TextFont);
				pDc->DrawText(pItem->strName,pItem->rtBody,DT_CENTER|DT_VCENTER|DT_SINGLELINE);
				pDc->SelectObject(oldFont);
			}
			else
				pDc->DrawText(pItem->strName,pItem->rtBody,DT_CENTER|DT_VCENTER|DT_SINGLELINE);
		}
	}
	if(GetMaxShowItem()!=GetButtonCount())
	{
		// Eto mozet bit More button ? [2/12/2002]
		if(m_MoreBtnPict.m_pPict!=NULL)
			m_MoreBtnPict.m_pPict->Render(pDc->GetSafeHdc(), m_WindowRect.left, m_WindowRect.top+GetMaxShowItem()*(m_ImageSizePxl.cy+m_nSpacing),
			m_MoreBtnImageSizePxl.cx/2,m_MoreBtnImageSizePxl.cy,0,m_MoreBtnImageSizeHi.cy-1,
			m_MoreBtnImageSizeHi.cx/2,-m_MoreBtnImageSizeHi.cy,&rbBoundRect);
	}
	
}

BOOL CMcAppCtrl::TranslateMessage(MSG *pMsg)
{
	if(m_pToolTip)
		m_pToolTip->RelayEvent(pMsg);
	return TRUE;
}

void CMcAppCtrl::RefreshButtons()
{
	int iSize = m_ButtonArray.GetSize();

	if(m_pToolTip)
	{
		delete m_pToolTip;
		m_pToolTip = NULL;
	}
	m_pToolTip			=	new CToolTipCtrl;
	m_pToolTip->Create(m_pParentWnd);
	m_pToolTip->Activate(TRUE);
	
	for(int i=0;i<iSize;i++)
	{
		McAppCtrl	*pItem = m_ButtonArray[i];
		pItem->rtBody	=	CRect(m_WindowRect.left, m_WindowRect.top+m_ImageSizePxl.cy*i+m_nSpacing*i, m_WindowRect.left+m_ImageSizePxl.cx/2, m_WindowRect.top+m_ImageSizePxl.cy*(i+1)+m_nSpacing*i);
		if(pItem->picIcon.m_pPict)
		{
			CSize IconSizeHi, IconSizePixel;
			pItem->picIcon.m_pPict->get_Width(&IconSizeHi.cx);
			pItem->picIcon.m_pPict->get_Height(&IconSizeHi.cy);
			HiMetricToPixel(&IconSizeHi,&IconSizePixel);
			
			pItem->rtIcon = CRect(CPoint(pItem->rtBody.left+(pItem->rtBody.Width()-IconSizePixel.cx)/2,pItem->rtBody.top+(pItem->rtBody.Height()-IconSizePixel.cy)/2),IconSizePixel);
		}
		if(i<GetMaxShowItem())
			m_pToolTip->AddTool(m_pParentWnd,pItem->strToolTip,pItem->rtBody,i+1);
	}
}

int CMcAppCtrl::HitTest(CPoint point)
{
	int iSize = m_ButtonArray.GetSize();
	
	for(int i=0;i<GetMaxShowItem();i++)
	{
		McAppCtrl	*pItem = m_ButtonArray[i];
		if(pItem->rtBody.PtInRect(point))
			return i;
	}
	if(GetMaxShowItem()!=GetButtonCount())
	{
		// Eto mozet bit More button ? [2/12/2002]
		McAppCtrl	*pItem = m_ButtonArray[GetMaxShowItem()-1];
		CRect		tmpRect = pItem->rtBody;
		tmpRect.top = tmpRect.bottom+m_nSpacing;
		tmpRect.bottom = tmpRect.top+m_MoreBtnImageSizePxl.cy;
		if(tmpRect.PtInRect(point))
		{
			return 0x8FFF;
		}
	}
	
	return -1;
}

void CMcAppCtrl::SetFont(const LOGFONT* lpLogFont)
{
	if(m_TextFont.m_hObject)
		m_TextFont.DeleteObject();
	m_TextFont.CreateFontIndirect(lpLogFont);
}

int CMcAppCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
	for(int i=0;i<GetMaxShowItem();i++)
	{
		McAppCtrl	*pItem = m_ButtonArray[i];
		if(pItem->rtBody.PtInRect(point))
		{
			if(m_pParentWnd&&m_CheckButton!=-1)
				m_pParentWnd->InvalidateRect(&m_ButtonArray[m_CheckButton]->rtBody,TRUE);
			m_CheckButton = i;
			if(m_pParentWnd)
				m_pParentWnd->InvalidateRect(&pItem->rtBody,TRUE);
			return m_CheckButton;
		}
	}
	if(GetMaxShowItem()!=GetButtonCount())
	{
		// Eto mozet bit More button ? [2/12/2002]
		McAppCtrl	*pItem = m_ButtonArray[GetMaxShowItem()-1];
		CRect		tmpRect = pItem->rtBody;
		tmpRect.top = tmpRect.bottom+m_nSpacing;
		tmpRect.bottom = tmpRect.top+m_MoreBtnImageSizePxl.cy;
		if(tmpRect.PtInRect(point))
		{
			CMenu	PopupMenu;
			PopupMenu.CreatePopupMenu();
			for(int i=GetMaxShowItem();i<GetButtonCount();i++)
			{
				CString strFormat; 
				strFormat.Format(_T("%s"),m_ButtonArray[i]->strToolTip);
				PopupMenu.AppendMenu(MF_STRING|MF_ENABLED|(GetCheckButton()==i?MF_CHECKED:0),20000+i,strFormat);
			}
			CPoint	Point = tmpRect.TopLeft();
			m_pParentWnd->ClientToScreen(&Point);
			PopupMenu.TrackPopupMenu(TPM_LEFTBUTTON,Point.x,Point.y+m_MoreBtnImageSizePxl.cy,m_pParentWnd->GetParent());
				
			/*CPopupItem	PopupItems;
			PopupItems.Create(this,4);
			CPoint	Point = tmpRect.TopLeft();
			m_pParentWnd->ClientToScreen(&Point);
			PopupItems.TrackPopupMenu(TPM_LEFTBUTTON,Point.x,Point.y,m_pParentWnd);*/
			return GetButtonCount();
		}
	}
	return -1;
}

int CMcAppCtrl::OnLButtonUp(UINT nFlags, CPoint point)
{
	for(int i=0;i<GetMaxShowItem();i++)
	{
		McAppCtrl	*pItem = m_ButtonArray[i];
		if(pItem->rtBody.PtInRect(point))
		{
			if(m_pParentWnd)
				m_pParentWnd->InvalidateRect(&pItem->rtBody,TRUE);
			return i;
		}
	}
	return -1;
}

int CMcAppCtrl::SetMaxShowItem(int Value)
{
	if(Value<1)
		return m_ShowMaxItem;
	int OldVal = m_ShowMaxItem;
	m_ShowMaxItem = Value;
	if(OldVal!=Value)
		RefreshButtons();
	return OldVal;
}

int CMcAppCtrl::GetMaxShowItem()
{
	return min(m_ShowMaxItem,m_ButtonArray.GetSize());
}

int CMcAppCtrl::GetButtonCount()
{
	return m_ButtonArray.GetSize();
}

CSize CMcAppCtrl::GetImageSize()
{
	return m_ImageSizePxl;
}

void CMcAppCtrl::DrawSingleItem(CDC *pDc, int ImageIndex, CPoint point)
{
	RECT rbBoundRect = {0,0,0,0};
	
	//int iSize = m_ButtonArray.GetSize();
	
	McAppCtrl	*pItem = m_ButtonArray[ImageIndex];

	if(m_Pict.m_pPict!=NULL)
		m_Pict.m_pPict->Render(pDc->GetSafeHdc(),point.x,point.y,
		m_ImageSizePxl.cx/2,m_ImageSizePxl.cy,(m_CheckButton==ImageIndex)?(m_ImageSizeHi.cx/2):0,m_ImageSizeHi.cy-1,
		m_ImageSizeHi.cx/2,-m_ImageSizeHi.cy,&rbBoundRect);

	CRect rtFrame (point,pItem->rtBody.Size());

	if(pItem->picIcon.m_pPict)
	{
		CRect IconFrame (CPoint(point.x+(pItem->rtBody.Width()-pItem->rtIcon.Width())/2,point.y+(pItem->rtBody.Height()-pItem->rtIcon.Height())/2),pItem->rtIcon.Size());
		pItem->picIcon.Render(pDc,IconFrame ,rbBoundRect);
	}
	else
	{
		pDc->SetBkMode(TRANSPARENT);
		pDc->SetTextColor(0xffffff);
		
		if(m_TextFont.m_hObject)
		{
			CFont *oldFont = pDc->SelectObject(&m_TextFont);
			pDc->DrawText(pItem->strName,rtFrame,DT_CENTER|DT_VCENTER|DT_SINGLELINE);
			pDc->SelectObject(oldFont);
		}
		else
			pDc->DrawText(pItem->strName,rtFrame,DT_CENTER|DT_VCENTER|DT_SINGLELINE);
	}
}



//////////////////////////////////////////////////////////////////////////
#define ITEM_BOUNDARY		(1)

CMcAppCtrl::CPopupItem::CPopupItem(DWORD MessageId)
{
	m_MessageId = MessageId;
	
}

CMcAppCtrl::CPopupItem::~CPopupItem()
{
	DestroyMenu();
}


/////////////////////////////////////////////////////////////////////////////
// CPopupItem message handlers

void CMcAppCtrl::CPopupItem::DrawItem( LPDRAWITEMSTRUCT lpDIS )
{
	CDC* pDC = CDC::FromHandle(lpDIS->hDC);
	
	int ImageIndex = lpDIS->itemID - m_MessageId;

	TRACE(_T("Image Index = %d\r\n"), ImageIndex);
	
	if ((lpDIS->itemState & ODS_SELECTED) &&
		(lpDIS->itemAction & (ODA_SELECT | ODA_DRAWENTIRE)))
	{
		CBrush brFon;
		brFon.Attach(GetSysColorBrush(COLOR_ACTIVECAPTION));
		pDC->FillRect(&(lpDIS->rcItem),&brFon);
	}
	else
	{
		CBrush brFon;
		brFon.Attach(GetSysColorBrush(COLOR_MENU));
		pDC->FillRect(&(lpDIS->rcItem),&brFon);
	}
	
	CPoint point(lpDIS->rcItem.left + (lpDIS->rcItem.right - lpDIS->rcItem.left - m_Cx)/2,lpDIS->rcItem.top+ (lpDIS->rcItem.bottom - lpDIS->rcItem.top- m_Cy)/2);
	m_pParentCtrl->DrawSingleItem(pDC,ImageIndex,point);
}

void CMcAppCtrl::CPopupItem::MeasureItem( LPMEASUREITEMSTRUCT lpMIS )
{
	lpMIS->itemWidth  = m_Cx-10;
	lpMIS->itemHeight = m_Cy+2;
}


BOOL CMcAppCtrl::CPopupItem::Create(CMcAppCtrl *pParentCtrl, int ColumnItem)
{
	ASSERT(pParentCtrl!=NULL);
	m_pParentCtrl = pParentCtrl;

	CSize size = m_pParentCtrl->GetImageSize();
	m_Cx = size.cx/2;
	m_Cy = size.cy;
	m_ColumnItem = ColumnItem;
	
	CreatePopupMenu();

	for(int i=m_pParentCtrl->GetMaxShowItem();i<m_pParentCtrl->GetButtonCount();i++)
	{
		if((i-m_pParentCtrl->GetMaxShowItem())%ColumnItem||(i==m_pParentCtrl->GetMaxShowItem()))
			AppendMenu(MF_OWNERDRAW|MF_ENABLED,m_MessageId+i);
		else
			AppendMenu(MF_OWNERDRAW|MF_ENABLED|MF_MENUBREAK,m_MessageId+i);
	}
	
	
	return TRUE;
}

void CMcAppCtrl::SetPos(int x, int y)
{
	m_WindowRect.left	=	x;
	m_WindowRect.top	=	y;
}

int CMcAppCtrl::GetMaxShowItemFromCY(int CY)
{
	if(m_ImageSizePxl.cy == 0)
		return 1;
	
	int nTotal = m_ButtonArray.GetSize();
	int n = (CY + m_nSpacing)/(m_ImageSizePxl.cy+m_nSpacing);

	if(n < nTotal)
		n = (CY - m_MoreBtnImageSizePxl.cy)/(m_ImageSizePxl.cy+m_nSpacing);

	//TRACE("\r\n CMcAppCtrl::GetMaxShowItemFromCY (CY = %d) return %d",CY,n);
	
	return n;
}

void CMcAppCtrl::SetSpacing(long cy)
{
	m_nSpacing = cy;
	RefreshButtons();
}

void CMcAppCtrl::ShowAppMenu(CPoint &point)
{
	CMenu	PopupMenu;
	PopupMenu.CreatePopupMenu();
	for(int i=0;i<GetButtonCount();i++)
	{
		CString strFormat; 
		strFormat.Format(_T("%s"),m_ButtonArray[i]->strToolTip);
		PopupMenu.AppendMenu(MF_STRING|MF_ENABLED|(GetCheckButton()==i?MF_CHECKED:0),20000+i,strFormat);
	}
	PopupMenu.TrackPopupMenu(TPM_LEFTBUTTON,point.x,point.y,m_pParentWnd->GetParent());
}
