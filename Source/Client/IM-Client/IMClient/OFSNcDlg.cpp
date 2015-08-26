// OFSNcDlg.cpp : implementation file
//

/////////////////////////////////////////////////////////////////////////////
/// Name   : Custom Dialog Caption. 
/// Author : Zhuk Oleg (OlegO).
/// Mail   : zhukoo@mail.ru
/// Country: Russian Federation. Kaliningrad region. Kalinigrad city.
/// Version: 23.01.2001
/////////////////////////////////////////////////////////////////////////////
/// Description: see OFSNcDlg.h file Header :0)
/////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "OFSNcDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg dialog
void COFSNcDlg::Construct()
{
	m_bActive       =  FALSE;
	BoundaryX       = BoundaryY = 3;
	CaptionH        = 20;
	m_CaptionMode   =  CAP_NONE;
	m_ActiveColor   =  ::GetSysColor(COLOR_ACTIVECAPTION);
	m_InactiveColor =  ::GetSysColor(COLOR_INACTIVECAPTION);
	pActiveBMP      = NULL;
	pInactiveBMP    = NULL;
	pBoundaryBMP    = NULL;
	m_BoundaryColor = GetSysColor(COLOR_3DSHADOW);
	bMoveNow        = FALSE;
	bSizeNow        = FALSE;
	pCloseBMP       = NULL;
	pMiniBMP        = NULL;
	pRestoryBMP     = NULL;
	pMaxiBMP        = NULL;    
}

COFSNcDlg::COFSNcDlg()
{
	Construct();
}

COFSNcDlg::COFSNcDlg(UINT nIDTemplate, CWnd* pParentWnd)
     : CResizableDialog(nIDTemplate, pParentWnd)
{
	Construct();
}

COFSNcDlg::COFSNcDlg(LPCTSTR lpszTemplateName, CWnd* pParentWnd )
      : CResizableDialog(lpszTemplateName, pParentWnd)
{
	Construct();
}

COFSNcDlg::~COFSNcDlg()
{
	if(pActiveBMP)
	{
		pActiveBMP->DeleteObject();
		delete pActiveBMP;
		pActiveBMP = NULL;
	}
	if(pInactiveBMP)
	{
		pInactiveBMP->DeleteObject();
		delete pInactiveBMP;
		pInactiveBMP = NULL;
	}
	if(pBoundaryBMP)
	{
		pBoundaryBMP->DeleteObject();
		delete pBoundaryBMP;
		pBoundaryBMP = NULL;
	}
	if(pCloseBMP)
	{
		pCloseBMP->DeleteObject();
		delete pCloseBMP;
		pCloseBMP = NULL;
	}
	if(pMiniBMP)
	{
		pMiniBMP->DeleteObject();
		delete pMiniBMP;
		pMiniBMP = NULL;
	}
	if(pRestoryBMP)
	{
		pRestoryBMP->DeleteObject();
		delete pRestoryBMP;
		pRestoryBMP = NULL;
	}
	if(pMaxiBMP)
	{
		pMaxiBMP->DeleteObject();
		delete pMaxiBMP;
		pMaxiBMP = NULL;
	}

}


BEGIN_MESSAGE_MAP(COFSNcDlg, CResizableDialog)
	//{{AFX_MSG_MAP(COFSNcDlg)
	ON_WM_NCPAINT()
	ON_WM_NCACTIVATE()
	ON_WM_NCCALCSIZE()
	ON_WM_NCLBUTTONDOWN()
	ON_WM_NCMOUSEMOVE()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONUP()
	ON_WM_SETCURSOR()
	ON_WM_CAPTURECHANGED()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg message handlers
void COFSNcDlg::OnNcPaint( )
{
	//// Творим Что хотим
	CRect rWin,rClient;
	
	GetWindowRect(&rWin);
	GetClientRect(&rClient);
	
	CRect m_CaptionRect;
	m_CaptionRect.left   = BoundaryX;
	m_CaptionRect.top    = BoundaryY;
	m_CaptionRect.right  = rWin.Width()- BoundaryX;
	m_CaptionRect.bottom = CaptionH + BoundaryY;

	ScreenToClient(&rWin);

	CRgn rgnWin,rgnClient,rgnUp,rgnCap;

	rgnUp.CreateRectRgn(0,0,0,0);
	rgnWin.CreateRectRgnIndirect(&rWin);
	rgnClient.CreateRectRgnIndirect(&rClient);
	rgnUp.CombineRgn(&rgnWin,&rgnClient,RGN_DIFF);
	rgnUp.OffsetRgn(CPoint(-rWin.left,-rWin.top));

	rgnCap.CreateRectRgnIndirect(m_CaptionRect);
	
    CWindowDC dc(this);
	dc.SelectClipRgn(&rgnUp,RGN_COPY);
	
	CBrush br;
	if(pBoundaryBMP)
		br.CreatePatternBrush(pBoundaryBMP);
	else
		br.CreateSolidBrush(m_BoundaryColor);

	rgnUp.CombineRgn(&rgnUp,&rgnCap,RGN_DIFF);
	dc.FillRgn(&rgnUp,&br);
	
	DrawCaption(dc,m_CaptionRect);
}

BOOL COFSNcDlg::OnNcActivate(BOOL bActive)
{
   /// Обработка OnNcActivate и Запоминания состояния для перерисовки
   if (m_nFlags & WF_STAYACTIVE)     bActive = TRUE;
   if (!IsWindowEnabled())  bActive = FALSE;
   if (bActive==m_bActive)
       return TRUE;

   CWnd* pActiveWnd = GetActiveWindow();
   if (pActiveWnd&&pActiveWnd!=this) {
       pActiveWnd->SendMessage(WM_NCACTIVATE,bActive);
       pActiveWnd->SendMessage(WM_NCPAINT);
   }

    // Turn WS_VISIBLE off before calling DefWindowProc,
    // so DefWindowProc won't paint and thereby cause flicker.
    // 
    DWORD dwStyle = GetStyle();
    if (dwStyle & WS_VISIBLE)
        ::SetWindowLong(*this, GWL_STYLE, (dwStyle & ~ WS_VISIBLE));

    MSG& msg = AfxGetThreadState()->m_lastSentMsg;
    msg.wParam = bActive;
    Default();
    if (dwStyle & WS_VISIBLE) ::SetWindowLong(*this, GWL_STYLE, dwStyle);
	
    // At this point, nothing has happened (since WS_VISIBLE was off).
    // Now it's time to paint.
    //
    m_bActive = bActive;                  // update state
    SendMessage(WM_NCPAINT);    // paint non-client area (frame too)
	
    return TRUE;                          // done OK
}

void COFSNcDlg::OnNcCalcSize( BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp )
{
	if(!(GetStyle()&WS_CAPTION)) CaptionH = 0;
	////Запросить для диалога Стандартные Параматры
	if(lpncsp->rgrc[0].top!=-32000)
		lpncsp->rgrc[0].top += CaptionH+BoundaryY;
	if(lpncsp->rgrc[0].left!=-32000)
		lpncsp->rgrc[0].left += BoundaryX;
	if(lpncsp->rgrc[0].bottom!=-32000)
		lpncsp->rgrc[0].bottom -= BoundaryY;
	if(lpncsp->rgrc[0].right!=-32000)
		lpncsp->rgrc[0].right -= BoundaryX;
	
	SendMessage(WM_NCPAINT);
}

/*UINT COFSNcDlg::OnNcHitTest( CPoint point)
{
	CPoint pClient = point;
	CRect m_CaptionRect;
	GetWindowRect(&m_CaptionRect);
	m_CaptionRect.left   += BoundaryX;
	m_CaptionRect.top    += BoundaryY;
	m_CaptionRect.right  -= BoundaryX;
	m_CaptionRect.bottom = m_CaptionRect.top + CaptionH;

	UINT rUint = HTTOP;
	//if(m_CaptionRect.PtInRect(point))
	//	return HTCLIENT ;

	//HTCAPTION;
	rUint = CResizableDialog::OnNcHitTest(point);
	//// Перехватить стандартные кнопки и Вставит обработку своих 
	if(rUint==HTCLOSE||rUint==HTMAXBUTTON||rUint==HTHELP||rUint==HTMINBUTTON
		||rUint==HTMINBUTTON||rUint==HTTRANSPARENT) 
		rUint = HTCLIENT;
	return rUint;
}*/

void COFSNcDlg::OnNcLButtonDown( UINT nHitTest, CPoint point )
{
	switch(nHitTest)
	{
	case HTTOP:
	case HTBOTTOM:		
	case HTLEFT:
    case HTRIGHT:		
	case HTTOPLEFT:
	case HTBOTTOMRIGHT:
	case HTTOPRIGHT:
	case HTBOTTOMLEFT:	
		if(!bSizeNow&&!IsZoomed())
		{
			SizeMode = nHitTest;
			bSizeNow = TRUE;
			m_DownPoint = point;
			ScreenToClient(&m_DownPoint);
			SetCapture();
		}
		break;
	case HTCAPTION:
		if(!bMoveNow&&!IsZoomed())
		{
			SetCursor(LoadCursor(NULL,IDC_SIZEALL));
			bMoveNow = TRUE;
			m_DownPoint = point;
			ScreenToClient(&m_DownPoint);
			SetCapture();
		}
		break;
	case HTCLOSE:
		CResizableDialog::OnCancel();
		//PostMessage(WM_CLOSE);
		break;
	case HTMINBUTTON:
		ShowWindow(SW_MINIMIZE);
		break;
	case HTMAXBUTTON:
		if(IsZoomed())
			ShowWindow(SW_RESTORE);
		else
			ShowWindow(SW_MAXIMIZE);
		break;

	}
	SetFocus();
}

/*void COFSNcDlg::OnNcLButtonUp( UINT nHitTest, CPoint point)
{
	if(bMoveNow)
	{
		bMoveNow = FALSE;
		ReleaseCapture();
	}
}*/

void COFSNcDlg::OnNcMouseMove( UINT nHitTest, CPoint point)
{
}

void COFSNcDlg::SetBoundary(int cx, int cy)
{
	BoundaryX = cx;
	BoundaryY = cy;
	if(::IsWindow(GetSafeHwnd()))
		SendMessage(WM_NCPAINT);
}

void COFSNcDlg::GetBoundary(int &cx, int &cy)
{
	cx = BoundaryX;
	cy = BoundaryY;
}

void COFSNcDlg::SetCaption(COLORREF ActiveColor, COLORREF UnActiveColor, int CaptionHeight)
{
	m_CaptionMode   = CAP_COLOR;
	
	CaptionH        = CaptionHeight;
	m_ActiveColor   = ActiveColor;
	m_InactiveColor = UnActiveColor;

	if(::IsWindow(GetSafeHwnd()))
		SendMessage(WM_NCPAINT);
}

void COFSNcDlg::DrawCaption(CDC &dc,const CRect &m_Rect)
{
	if(m_Rect.Width()==0||m_Rect.Height()==0) return;
	switch(m_CaptionMode)
	{
     case CAP_NONE:
	 case CAP_COLOR:	 
		 {
			 CFont m_font;
			 CBrush m_br;
			 CRect m_TextRect = m_Rect;
			 
			 if(m_bActive)
				 m_br.CreateSolidBrush(m_ActiveColor);
		     else
			     m_br.CreateSolidBrush(m_InactiveColor);

			 dc.FillRect(m_Rect,&m_br);
			 CString str;
			 GetWindowText(str);
			 m_font.Attach(GetStockObject(DEFAULT_GUI_FONT));
			 dc.SelectObject(&m_font);
			 dc.SetTextColor(RGB(255,255,255));
			 dc.SetBkMode(TRANSPARENT);
			 m_TextRect.right -= 80;
			 dc.DrawText(str,(LPRECT)&m_TextRect ,DT_LEFT|DT_VCENTER|DT_END_ELLIPSIS|DT_SINGLELINE);
		 }
		 break;
     case CAP_BITMAP_1:	 		
		 {
			 CDC dcT;
			 dcT.CreateCompatibleDC(&dc);
			 dcT.SelectObject(pActiveBMP);
			 dc.BitBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,SRCCOPY);
		 }
		 break;
     case CAP_BITMAP_1_ZOOM:	 		 
		 {
			 CDC dcT;
			 dcT.CreateCompatibleDC(&dc);
			 dcT.SelectObject(pActiveBMP);
			 dc.StretchBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,m_ActiveBMPSize.cx,m_ActiveBMPSize.cy,SRCCOPY);
//			 dc.BitBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,SRCCOPY);
		 }
		 break;
     case CAP_BITMAP_2:	 		 
		 {
			 CDC dcT;
			 dcT.CreateCompatibleDC(&dc);
			 if(m_bActive)
				 dcT.SelectObject(pActiveBMP);
			 else
                 dcT.SelectObject(pInactiveBMP);    
			 dc.BitBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,SRCCOPY);
		 }
		 break;
     case CAP_BITMAP_2_ZOOM:	 		 
		 {
			 CDC dcT;
			 dcT.CreateCompatibleDC(&dc);
			 if(m_bActive)
			 {
				 dcT.SelectObject(pActiveBMP);
				 dc.StretchBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,m_ActiveBMPSize.cx,m_ActiveBMPSize.cy,SRCCOPY);
			 }
			 else
			 {
                 dcT.SelectObject(pInactiveBMP);    
				 dc.StretchBlt(m_Rect.left,m_Rect.top,m_Rect.Width(),m_Rect.Height(),&dcT,0,0,m_InactiveBMPSize.cx,m_InactiveBMPSize.cy,SRCCOPY);
			 }
		 }
		 break;
	}

	DWORD dwStyle = GetStyle();
	CRect m_ButtonRect;
	m_ButtonRect.SetRect(m_Rect.right - CaptionH +2 ,m_Rect.top + 4,
		m_Rect.right - 2,m_Rect.bottom - 2);
	
	if(dwStyle&WS_SYSMENU)
		dc.DrawFrameControl(&m_ButtonRect,DFC_CAPTION,DFCS_CAPTIONCLOSE);
	
	if (dwStyle & WS_MAXIMIZEBOX) 
	{
	   m_ButtonRect-=CPoint(CaptionH-2,0);
       dc.DrawFrameControl(&m_ButtonRect, DFC_CAPTION, IsZoomed()?DFCS_CAPTIONRESTORE:DFCS_CAPTIONMAX);
	}
	
	if (dwStyle & WS_MINIMIZEBOX) 
	{
		m_ButtonRect-=CPoint(CaptionH-2,0);
        dc.DrawFrameControl(&m_ButtonRect, DFC_CAPTION ,DFCS_CAPTIONMIN);
	}

}


void COFSNcDlg::SetCaption(HBITMAP m_ActiveBMP, BOOL bZoom)
{
	m_CaptionMode = (!bZoom)?CAP_BITMAP_1:CAP_BITMAP_1_ZOOM;
	
	if(!pActiveBMP) 
		pActiveBMP = new CBitmap;
	else
		pActiveBMP->DeleteObject();

	pActiveBMP->Attach(m_ActiveBMP);

	BITMAP m_BmpInfo;
	pActiveBMP->GetBitmap(&m_BmpInfo);
	m_ActiveBMPSize.cx = m_BmpInfo.bmWidth;
	m_ActiveBMPSize.cy = m_BmpInfo.bmHeight;
	CaptionH           = m_ActiveBMPSize.cy;
}

void COFSNcDlg::SetCaption(HBITMAP m_ActiveBMP, HBITMAP m_InactiveBMP, BOOL bZoom = FALSE)
{
	m_CaptionMode = (!bZoom)?CAP_BITMAP_2:CAP_BITMAP_2_ZOOM;
	
	if(!pActiveBMP) 
		pActiveBMP = new CBitmap;
	else
		pActiveBMP->DeleteObject();

	pActiveBMP->Attach(m_ActiveBMP);

	BITMAP m_BmpInfo;
	pActiveBMP->GetBitmap(&m_BmpInfo);
	m_ActiveBMPSize.cx = m_BmpInfo.bmWidth;
	m_ActiveBMPSize.cy = m_BmpInfo.bmHeight;

	if(!pInactiveBMP) 
		pInactiveBMP = new CBitmap;
	else
		pInactiveBMP->DeleteObject();

	pInactiveBMP->Attach(m_InactiveBMP);

	pInactiveBMP->GetBitmap(&m_BmpInfo);
	m_InactiveBMPSize.cx = m_BmpInfo.bmWidth;
	m_InactiveBMPSize.cy = m_BmpInfo.bmHeight;

	CaptionH           = (m_ActiveBMPSize.cy>m_InactiveBMPSize.cy)?m_InactiveBMPSize.cy:m_ActiveBMPSize.cy;
}

void COFSNcDlg::SetBoundaryBMP(HBITMAP hBondary)
{
	if(!pBoundaryBMP)
		pBoundaryBMP = new CBitmap;
	else
		pBoundaryBMP->DeleteObject();

	pBoundaryBMP->Attach(hBondary);

	if(IsWindow(GetSafeHwnd()))
		SendMessage(WM_NCPAINT);
}

void COFSNcDlg::SetBoundaryColor(COLORREF BoundaryColor)
{
	m_BoundaryColor = BoundaryColor;
	if(pBoundaryBMP)
	{
		pBoundaryBMP->DeleteObject();
		delete pBoundaryBMP;
		pBoundaryBMP = NULL;
	}

	if(IsWindow(GetSafeHwnd()))
		SendMessage(WM_NCPAINT);
}

void COFSNcDlg::OnMouseMove(UINT nFlags, CPoint point) 
{
	if(bMoveNow)
	{
        CRect rPos;
		GetWindowRect(&rPos);
		
		int NewX = rPos.left + point.x - m_DownPoint.x;
		int NewY = rPos.top + point.y - m_DownPoint.y;
		
		SetWindowPos(NULL,NewX,NewY,-1,-1,SWP_NOZORDER|SWP_NOSIZE);
		UpdateWindow();
		Sleep(10);
	}
	
	if(bSizeNow)
	{
        CRect rPos;
		GetWindowRect(&rPos);
		
		CPoint	winPoint  = point;
		ClientToScreen(&winPoint);
		
		int NewX = rPos.left;// + point.x - m_DownPoint.x;
		int NewY = rPos.top;// + point.y - m_DownPoint.y;
		int NewCX = rPos.Width();
		int NewCY = rPos.Height();

		switch(SizeMode)
		{
		case HTTOP:
			NewY += (point.y - m_DownPoint.y);
			NewCY -= (point.y - m_DownPoint.y);
			break;
		case HTBOTTOM:
			//???//
			if(point.y <= m_DownPoint.y||winPoint.y>=(NewY+NewCY))
				NewCY += (point.y - m_DownPoint.y);
			m_DownPoint.y = point.y;
            break;			
		case HTLEFT:
			NewX += (point.x - m_DownPoint.x);
			NewCX -= (point.x - m_DownPoint.x);
			break;
		case HTRIGHT:		
			//???//
			if(point.x <= m_DownPoint.x||winPoint.x>=(NewX+NewCX))
				NewCX += (point.x - m_DownPoint.x);
			m_DownPoint.x = point.x;
			break;
		case HTTOPLEFT:
			NewY += (point.y - m_DownPoint.y);
			NewCY -= (point.y - m_DownPoint.y);
			NewX += (point.x - m_DownPoint.x);
			NewCX -= (point.x - m_DownPoint.x);
			
			break;
		case HTBOTTOMRIGHT:
			if(point.y <= m_DownPoint.y||winPoint.y>=(NewY+NewCY))
				NewCY += point.y - m_DownPoint.y;
			if(point.x <= m_DownPoint.x||winPoint.x>=(NewX+NewCX))
				NewCX += (point.x - m_DownPoint.x);
			m_DownPoint = point;
			break;
		case HTTOPRIGHT:
			NewY += (point.y - m_DownPoint.y);
			NewCY -= (point.y - m_DownPoint.y);
			NewCX += (point.x - m_DownPoint.x);
			m_DownPoint.x = point.x;
			break;
		case HTBOTTOMLEFT:	
			NewCY += point.y - m_DownPoint.y;
			NewX += (point.x - m_DownPoint.x);
			NewCX -= (point.x - m_DownPoint.x);
			m_DownPoint.y = point.y;
			break;
		}

		SetWindowPos(NULL,NewX,NewY,NewCX ,NewCY,SWP_NOZORDER);
		UpdateWindow();
		Sleep(10);
	}
	
}



void COFSNcDlg::OnLButtonUp(UINT nFlags, CPoint point) 
{
	if(bMoveNow)
	{
		bMoveNow = FALSE;
		ReleaseCapture();
	}
	if(bSizeNow)
	{
		bSizeNow = FALSE;
        ReleaseCapture();
	}
}

BOOL COFSNcDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	switch(nHitTest)
	{
	case HTTOP:
	case HTBOTTOM:		
        SetCursor(LoadCursor(NULL,IDC_SIZENS));
		break;
	case HTLEFT:
    case HTRIGHT:		
		SetCursor(LoadCursor(NULL,IDC_SIZEWE));
		break;
	case HTTOPLEFT:
	case HTBOTTOMRIGHT:
		SetCursor(LoadCursor(NULL,IDC_SIZENWSE));
		break;
	case HTTOPRIGHT:
	case HTBOTTOMLEFT:		
		SetCursor(LoadCursor(NULL,IDC_SIZENESW));
		break;
	default:
		SetCursor(LoadCursor(NULL,IDC_ARROW));
	}

	
	return FALSE;//CResizableDialog::OnSetCursor(pWnd, nHitTest, message);
}

void COFSNcDlg::OnCaptureChanged( CWnd* pWnd )
{
	if(bMoveNow)
	{
		bMoveNow = FALSE;
		ReleaseCapture();
	}
	if(bSizeNow)
	{
		bSizeNow = FALSE;
        ReleaseCapture();
	}	
}

void COFSNcDlg::SetButtonClose(HBITMAP m_CloseBMP)
{
	if(!pCloseBMP)
		pCloseBMP = new CBitmap;
	else
		pCloseBMP->DeleteObject();

	pCloseBMP->Attach(m_CloseBMP);
}

void COFSNcDlg::SetButtonMini(HBITMAP m_MiniBMP)
{
	if(!pMiniBMP)
		pMiniBMP = new CBitmap;
	else
		pMiniBMP->DeleteObject();

	pMiniBMP->Attach(m_MiniBMP);
}

void COFSNcDlg::SetButtonMaxiRestore(HBITMAP m_MaxiBMP, HBITMAP m_RestoryBMP)
{
	if(!pMaxiBMP)
		pMaxiBMP = new CBitmap;
	else
		pMaxiBMP->DeleteObject();

	pMaxiBMP->Attach(m_MaxiBMP);

	if(!pRestoryBMP)
		pRestoryBMP = new CBitmap;
	else
		pRestoryBMP->DeleteObject();

	pRestoryBMP->Attach(m_RestoryBMP);

}



BOOL COFSNcDlg::OnInitDialog() 
{
	CResizableDialog::OnInitDialog();
	
	ShowSizeGrip(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
