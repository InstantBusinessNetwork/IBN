// PageApps.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageApps.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern COfsTvApp theApp;
/////////////////////////////////////////////////////////////////////////////
// CPageApps dialog


CPageApps::CPageApps(CArray<McAppItem,McAppItem> &AppArray,long UserId,	LPCTSTR UserRole,	LPCTSTR szTitle)
	: CMcSettingsPage(CPageApps::IDD, szTitle),m_AppList(this)
{
	//{{AFX_DATA_INIT(CPageApps)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_strUserId.Format(_T("%d"),UserId);
	m_strUserRole	=	UserRole;
	m_AppArray.Copy(AppArray);
}


void CPageApps::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageApps)
	DDX_Control(pDX, IDC_APP_LIST, m_AppList);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageApps, CMcSettingsPage)
//{{AFX_MSG_MAP(CPageApps)
	ON_WM_MEASUREITEM()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageApps message handlers

BOOL CPageApps::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();
	
	// Load App priority [3/11/2002]
	for(int i=0;i<m_AppArray.GetSize();i++)
	{
		McAppItem	Item	=	m_AppArray[i];
		int iData = m_AppList.InsertString(i,Item.GetVisualName());
		m_AppList.SetItemData(iData,i);
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageApps::SaveSettings()
{
	CString strOutPriority;
	for(int i=0;i<m_AppArray.GetSize();i++)
	{
		long ItemId	= m_AppList.GetItemData(i);
		McAppItem	Item	=	m_AppArray[ItemId];
		CString	strTmpId;
		strTmpId.Format(_T("%d"),Item.Id);
		strOutPriority += (strTmpId + _T(';'));
	}

	theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + m_strUserRole+_T("\\")+m_strUserId,GetString(IDS_STUBS),strOutPriority);

	return CMcSettingsPage::SaveSettings();
}

void CPageApps::OnDropped()
{
	SetModified();
}

//////////////////////////////////////////////////////////////////////////
//  [3/11/2002]
//////////////////////////////////////////////////////////////////////////

CPageApps::CAppCDragListBox::CAppCDragListBox(CPageApps *pPageApps)
{
	m_pPageApps	=  pPageApps;
}

void CPageApps::CAppCDragListBox::Dropped(int nSrcIndex, CPoint pt)
{
	if(m_pPageApps)
	{
		int nDestIndex = ItemFromPt(pt);
		
		if (!(nSrcIndex == -1 || nDestIndex == -1 ||
			nDestIndex == nSrcIndex || nDestIndex == nSrcIndex+1))
			m_pPageApps->OnDropped();
	}
	
	CDragListBox::Dropped(nSrcIndex, pt);
}

void CPageApps::CAppCDragListBox::MeasureItem( LPMEASUREITEMSTRUCT lpMeasureItemStruct )
{
	lpMeasureItemStruct->itemHeight = 45;
}

void CPageApps::CAppCDragListBox::DrawItem( LPDRAWITEMSTRUCT lpDrawItemStruct )
{
	if(lpDrawItemStruct->itemID == (UINT)-1)
		return;
	
	int ItemId = lpDrawItemStruct->itemData;
	
	if(ItemId>=0&&ItemId<m_pPageApps->m_AppArray.GetSize())
	{
		CDC dc;
		dc.Attach(lpDrawItemStruct->hDC);
		dc.SetBkMode(TRANSPARENT);
		
		COLORREF crOldTextColor = dc.GetTextColor();
		COLORREF crOldBkColor = dc.GetBkColor();

		CBrush	*pOldBrush	=	NULL, newBrush;
		
		McAppItem	Item	=	m_pPageApps->m_AppArray[ItemId];
		if((lpDrawItemStruct->itemAction | ODA_SELECT) && (lpDrawItemStruct->itemState & ODS_SELECTED))
		{
			// Selected True [3/12/2002]
			dc.SetTextColor(::GetSysColor(COLOR_HIGHLIGHTTEXT));
			dc.SetBkColor(::GetSysColor(COLOR_HIGHLIGHT));
			newBrush.CreateSolidBrush(::GetSysColor(COLOR_HIGHLIGHT));
			dc.FillSolidRect(&lpDrawItemStruct->rcItem, ::GetSysColor(COLOR_HIGHLIGHT));
			pOldBrush = dc.SelectObject(&newBrush);
		}
		else
		{
			// Selected False [3/12/2002]
			dc.FillSolidRect(&lpDrawItemStruct->rcItem, crOldBkColor);
		}
		
		// Draw Item Icon [3/12/2002]
		CRect	ImageFrame(lpDrawItemStruct->rcItem.left+2,lpDrawItemStruct->rcItem.top+6,lpDrawItemStruct->rcItem.left+34,lpDrawItemStruct->rcItem.top+38);
		dc.RoundRect(ImageFrame.left-1,ImageFrame.top-1,ImageFrame.right+1,ImageFrame.bottom+1,2,2);

		if(pOldBrush)
			dc.SelectObject(pOldBrush);
		

		if(Item.Icon)
		{
			CSize hmSize, pxSize;
			
			Item.Icon->get_Width(&hmSize.cx);
			Item.Icon->get_Height(&hmSize.cy);
			
			HiMetricToPixel(&hmSize,&pxSize);
			
			CRect	rcRender(CPoint(ImageFrame.left+(32-pxSize.cx)/2,ImageFrame.top+(32-pxSize.cy)/2),pxSize), rcWBounds(0,0,0,0);
			
			Item.Icon->Render(dc.m_hDC, rcRender.left, rcRender.top,
				rcRender.Width(), rcRender.Height(), 0, hmSize.cy-1,
				hmSize.cx, -hmSize.cy, (LPCRECT)rcWBounds);
		}
		else
		{
			LOGFONT lgFont	=	{0};
			
			lgFont.lfHeight = -14;
			lgFont.lfWeight = FW_BOLD;
			lgFont.lfCharSet	=	DEFAULT_CHARSET;
			_tcscpy(lgFont.lfFaceName, _T("Arial"));
			
			CFont	LabelFond, *pOldFont;
			LabelFond.CreateFontIndirect(&lgFont);
			pOldFont = dc.SelectObject(&LabelFond);
			dc.DrawText(Item.Name,ImageFrame,DT_CENTER|DT_VCENTER|DT_SINGLELINE);
			dc.SelectObject(pOldFont);
		}
		
		// Draw Text 
		CRect	TextFrame(lpDrawItemStruct->rcItem.left+40,lpDrawItemStruct->rcItem.top+2,lpDrawItemStruct->rcItem.right,lpDrawItemStruct->rcItem.top+41);
		LOGFONT lgFont	=	{0};
		
		lgFont.lfCharSet	=	DEFAULT_CHARSET;
		
		lgFont.lfHeight = -11;
		lgFont.lfWeight = FW_MEDIUM;
		_tcscpy(lgFont.lfFaceName, _T("Arial"));
		
		CFont	LabelFont, TextFont, *pOldFont;
		LabelFont.CreateFontIndirect(&lgFont);

		lgFont.lfHeight = -11;
		lgFont.lfWeight = FW_SEMIBOLD;
		_tcscpy(lgFont.lfFaceName, _T("Arial"));

		TextFont.CreateFontIndirect(&lgFont);

		pOldFont = dc.SelectObject(&LabelFont);

		CString strFormat = GetString(IDS_APP_NAME_INFO_NAME);
		if(!Item.Version.IsEmpty())
			strFormat += GetString(IDS_APP_VER_NAME);

		dc.DrawText(strFormat,TextFrame,DT_LEFT|DT_VCENTER|DT_END_ELLIPSIS|DT_TABSTOP);
		strFormat.Format(_T("%s\r\n%s"),Item.Name,Item.ToolTip);

		if(!Item.Version.IsEmpty())
			strFormat.Format(_T("%s\r\n%s\r\n%s"),Item.Name,Item.ToolTip, Item.Version);
		else
			strFormat.Format(_T("%s\r\n%s"),Item.Name,Item.ToolTip);
		
		dc.SelectObject(&TextFont);
		TextFrame.left = 80;
		dc.DrawText(strFormat,TextFrame,DT_LEFT|DT_VCENTER|DT_END_ELLIPSIS|DT_TABSTOP);

		dc.SelectObject(pOldFont);
		
		// Reset the background color and the text color back to their
		// original values.
		dc.SetTextColor(crOldTextColor);
		dc.SetBkColor(crOldBkColor);
		
		dc.Detach();
	}
}

void CPageApps::OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpMeasureItemStruct) 
{
	// TODO: Add your message handler code here and/or call default
	if(nIDCtl==IDC_APP_LIST)
	{
		m_AppList.MeasureItem(lpMeasureItemStruct);
	}
	
	CMcSettingsPage::OnMeasureItem(nIDCtl, lpMeasureItemStruct);
}
