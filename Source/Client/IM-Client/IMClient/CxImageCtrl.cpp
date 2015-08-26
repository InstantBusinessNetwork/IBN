// CxImageCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "CxImageCtrl.h"
#include "memdc.h"
#include "TextCommentSettings.h"
#include ".\cximagectrl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCxImageCtrl

CCxImageCtrl::CCxImageCtrl()
{
	m_dwWidth = 0;
	m_dwHeight = 0;
	m_nVScrollPos = 0;
	m_nHScrollPos = 0;

	m_bStretchMode	=	FALSE;
	m_iZoomValue = 1.0;
	m_btQuality = 100;

	m_brHatch.CreateHatchBrush(HS_DIAGCROSS, RGB(191, 191, 191));

	m_State	=	CCxImageCtrl::IS_Move;

	m_bCaptureWasSet = FALSE;

	m_iUndoListCurrPosition	=	0;

	m_bCurrentImageWasSave	=	false;

	m_bIsSaved = false;

	m_PenColor	=	RGB(0xFF,0x0,0x0);
	m_TextColor = RGB(0x0,0x0,0x0);
	m_TextSize = 12;
	m_FontName = _T("Arial");

	m_pTextComment = NULL;

}

CCxImageCtrl::~CCxImageCtrl()
{
	for(CImageUndoEnum	Index = m_UndoList.begin();Index!=m_UndoList.end();Index++)
	{
		delete *Index;
	}
	m_UndoList.clear();

	if (m_pTextComment != NULL)
	{
		delete m_pTextComment;
	}
}


BEGIN_MESSAGE_MAP(CCxImageCtrl, CWnd)
	//{{AFX_MSG_MAP(CCxImageCtrl)
	ON_WM_PAINT()
	ON_WM_ERASEBKGND()
	ON_WM_SIZE()
	ON_WM_VSCROLL()
	ON_WM_HSCROLL()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_SETCURSOR()
	ON_WM_MOUSEMOVE()
	ON_WM_CAPTURECHANGED()
	
	//}}AFX_MSG_MAP
	ON_WM_LBUTTONDBLCLK()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCxImageCtrl message handlers
void	CCxImageCtrl::Refresh()
{
	if(m_hImage.IsValid()&&IsWindow(GetSafeHwnd()))
	{
		SetZoomValue(GetZoomValue());
		
		CRect ClipRect;
		GetClientRect(&ClipRect);
		OnSize(SIZE_RESTORED,ClipRect.Width(),ClipRect.Height());
		
		Invalidate(FALSE);
	}
}

bool CCxImageCtrl::CreateFromHBITMAP (HBITMAP hbmp, HPALETTE hpal)
{
	bool bRetVal = m_hImage.CreateFromHBITMAP(hbmp,hpal);

	if(m_hImage.IsIndexed())
		m_hImage.IncreaseBpp(24);

	Refresh();

	return bRetVal;
}

void CCxImageCtrl::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	CRect ClipRect;
	GetClientRect(&ClipRect);

	if(m_hImage.IsValid())
	{
		if(GetStretchMode())
		{
			m_hImage.Stretch(dc,ClipRect);

			if(m_bCaptureWasSet&&GetState()==CCxImageCtrl::IS_Crop)
			{
				dc.Draw3dRect(CRect(m_RefPoint,m_CropActivePoint),RGB(255, 0, 0), RGB(0, 255, 0));
			}
		}
		else
		{
			CRect renderRect(-m_nHScrollPos,-m_nVScrollPos,m_dwWidth-m_nHScrollPos,m_dwHeight-m_nVScrollPos);
			m_hImage.Draw(dc,renderRect);

			if(m_bCaptureWasSet&&GetState()==CCxImageCtrl::IS_Crop)
			{
				dc.Draw3dRect(CRect(m_RefPoint,m_CropActivePoint),RGB(255, 0, 0), RGB(0, 255, 0));
			}
			
			dc.SetBkColor(0xEEEEEE);
			dc.ExcludeClipRect(renderRect);
			dc.FillRect(ClipRect,&m_brHatch);
		}

	}

	
	// Do not call CWnd::OnPaint() for painting messages
}

BOOL CCxImageCtrl::OnEraseBkgnd(CDC* pDC) 
{
	return CWnd::OnEraseBkgnd(pDC);
}

void CCxImageCtrl::SetScrollSize(int cx, int cy)
{
	//////////////////////////////////////////////////////////////////////////
	
	m_nCurHeight = cy;
	int nScrollMax;
	if (cy < m_dwHeight&&!GetStretchMode())
	{
		nScrollMax = m_dwHeight - cy;
	}
	else
		nScrollMax = 0;

	if(m_nVScrollPos<0)
		m_nVScrollPos = 0;
	if(m_nVScrollPos>nScrollMax)
		m_nVScrollPos = nScrollMax;
	
	SCROLLINFO si;
	si.cbSize = sizeof(SCROLLINFO);
	si.fMask = SIF_ALL; // SIF_ALL = SIF_PAGE | SIF_RANGE | SIF_POS;
	si.nMin = 0;
	si.nMax = nScrollMax;
	si.nPage = si.nMax/5;
	si.nPos = m_nVScrollPos;
	SetScrollInfo(SB_VERT, &si, TRUE); 

	
	//////////////////////////////////////////////////////////////////////////
	
	m_nCurWidth = cx;
	if (cx < m_dwWidth&&!GetStretchMode())
	{
		nScrollMax = m_dwWidth - cx;
	}
	else
		nScrollMax = 0;


	if(m_nHScrollPos<0)
		m_nHScrollPos = 0;
	if(m_nHScrollPos>nScrollMax)
		m_nHScrollPos = nScrollMax;
	
	
	si.cbSize = sizeof(SCROLLINFO);
	si.fMask = SIF_ALL; // SIF_ALL = SIF_PAGE | SIF_RANGE | SIF_POS;
	si.nMin = 0;
	si.nMax = nScrollMax;
	si.nPage = si.nMax/5;
	si.nPos = m_nHScrollPos;
	SetScrollInfo(SB_HORZ, &si, TRUE); 


	//////////////////////////////////////////////////////////////////////////
	
	
	Invalidate(FALSE);
}

void CCxImageCtrl::OnSize(UINT nType, int cx, int cy) 
{
	CWnd::OnSize(nType, cx, cy);
	
	SetScrollSize(cx, cy);
}

void CCxImageCtrl::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	int nDelta;
	int nMaxPos = m_dwHeight - m_nCurHeight;
	
	switch (nSBCode)
	{
	case SB_LINEDOWN:
		if (m_nVScrollPos >= nMaxPos)
			return;
		nDelta = min(nMaxPos/20,nMaxPos-m_nVScrollPos);
		break;
		
	case SB_LINEUP:
		if (m_nVScrollPos <= 0)
			return;
		nDelta = -min(nMaxPos/20,m_nVScrollPos);
		break;
		
	case SB_PAGEDOWN:
		if (m_nVScrollPos >= nMaxPos)
			return;
		nDelta = min(nMaxPos/2,nMaxPos-m_nVScrollPos);
		break;
		
	case SB_THUMBPOSITION:
		nDelta = (int)nPos - m_nVScrollPos;
		break;
		
	case SB_PAGEUP:
		if (m_nVScrollPos <= 0)
			return;
		nDelta = -min(nMaxPos/2,m_nVScrollPos);
		break;
		
	default:
		return;
	}
	m_nVScrollPos += nDelta;
	SetScrollPos(SB_VERT,m_nVScrollPos,TRUE);
	//ScrollWindow(0,-nDelta);

	if(IsWindow(GetSafeHwnd()))
		Invalidate(FALSE);
	
	CWnd::OnVScroll(nSBCode, nPos, pScrollBar);
}

void CCxImageCtrl::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	int nDelta;
	int nMaxPos = m_dwWidth - m_nCurWidth;

	switch (nSBCode)
	{
	case SB_LINEDOWN:
		if (m_nHScrollPos >= nMaxPos)
			return;
		nDelta = min(nMaxPos/20,nMaxPos-m_nHScrollPos);
		break;
		
	case SB_LINEUP:
		if (m_nHScrollPos <= 0)
			return;
		nDelta = -min(nMaxPos/20,m_nHScrollPos);
		break;
		
	case SB_PAGEDOWN:
		if (m_nHScrollPos >= nMaxPos)
			return;
		nDelta = min(nMaxPos/2,nMaxPos-m_nHScrollPos);
		break;
		
	case SB_THUMBPOSITION:
		nDelta = (int)nPos - m_nHScrollPos;
		break;
		
	case SB_PAGEUP:
		if (m_nHScrollPos <= 0)
			return;
		nDelta = -min(nMaxPos/20,m_nHScrollPos);
		break;
		
	default:
		return;
	}
	m_nHScrollPos += nDelta;
	SetScrollPos(SB_HORZ,m_nHScrollPos,TRUE);
	//ScrollWindow(-nDelta,0);
	
	Invalidate(FALSE);
	
	CWnd::OnHScroll(nSBCode, nPos, pScrollBar);
}

BOOL CCxImageCtrl::PreCreateWindow(CREATESTRUCT& cs) 
{
	// TODO: Add your specialized code here and/or call the base class
	//cs.style	|=	WS_VSCROLL|WS_HSCROLL;
	
	return CWnd::PreCreateWindow(cs);
}

void	CCxImageCtrl::SetStretchMode(BOOL bIsStretch)
{
	m_bStretchMode	=	bIsStretch;

	if(IsWindow(GetSafeHwnd()))
		Invalidate(FALSE);
}

BOOL	CCxImageCtrl::GetStretchMode()
{
	return	m_bStretchMode;
}

void	CCxImageCtrl::SetZoomValue(double ZoomValue)
{
	m_iZoomValue	=	ZoomValue;

	if(m_hImage.IsValid())
	{
		m_dwWidth = (DWORD)(m_hImage.GetWidth()*GetZoomValue());
		m_dwHeight = (DWORD)(m_hImage.GetHeight()*GetZoomValue());
	}

	if(IsWindow(GetSafeHwnd()))
	{
		CRect ClipRect;
		GetClientRect(&ClipRect);
		OnSize(SIZE_RESTORED,ClipRect.Width(),ClipRect.Height());
		
		Invalidate(FALSE);
	}
}

double	CCxImageCtrl::GetZoomValue()
{
	return m_iZoomValue;
}


void CCxImageCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	CPoint	imagePoint(point);
	ClientToImage(imagePoint);

	CRect clientRect;
	GetClientRect(&clientRect);
	

	if(m_hImage.IsInside(imagePoint.x,imagePoint.y))
	{
		switch(GetState()) 
		{
		case CCxImageCtrl::IS_Move:
			if (m_pTextComment)
			{
				delete m_pTextComment;
				m_pTextComment = NULL;
			}
			SetCapture();
			m_bCaptureWasSet = TRUE;
			SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR2));

			m_RefScroll = CSize(m_nHScrollPos,m_nVScrollPos);
			m_RefPoint = point;
			break;
		case CCxImageCtrl::IS_Crop:
			if (m_pTextComment)
			{
				delete m_pTextComment;
				m_pTextComment = NULL;
			}
			SetCapture();
			m_bCaptureWasSet = TRUE;
			SetCursor(AfxGetApp()->LoadCursor(IDC_SELECTCURSOR));

			m_RefScroll = CSize(m_nHScrollPos,m_nVScrollPos);
			m_RefPoint = point;

			ClientToImage(m_RefPoint);
			ImageToClient(m_RefPoint);
			
			break;
		case CCxImageCtrl::IS_Pen:
			if (m_pTextComment)
			{
				delete m_pTextComment;
				m_pTextComment = NULL;
			}
			SaveImageToUndo();

			SetCapture();
			m_bCaptureWasSet = TRUE;
			SetCursor(AfxGetApp()->LoadCursor(IDC_PENCURSOR));

			m_RefScroll = CSize(m_nHScrollPos,m_nVScrollPos);
			m_RefPoint = point;
			break;
		case IS_TextMove:
			SetCapture();
			m_RefOffset= m_pTextComment->GetPosition() - imagePoint;
			m_bCaptureWasSet = TRUE;
			break;
		case CCxImageCtrl::IS_Text:
			SaveImageToUndo();
			SetCapture();
			m_bCaptureWasSet = TRUE;
			SetCursor(AfxGetApp()->LoadCursor(IDC_TEXTCURSOR));

			m_RefScroll = CSize(m_nHScrollPos, m_nVScrollPos);
			m_RefPoint = imagePoint;

			CTextCommentSettings settingsDlg(this);

			settingsDlg.m_fontColor = m_TextColor;
			settingsDlg.m_fontSize = m_TextSize;
			settingsDlg.m_fontName = m_FontName;

			if(settingsDlg.DoModal()==IDOK)
			{
				m_TextColor = settingsDlg.m_fontColor;
				m_TextSize = settingsDlg.m_fontSize;
				m_FontName = settingsDlg.m_fontName;

				if (m_pTextComment) 
				{
					delete m_pTextComment;
					m_pTextComment = NULL;
				}

				m_pTextComment = new CTextComment(m_RefPoint);
				m_pTextComment->SetColor(settingsDlg.m_fontColor);
				m_pTextComment->SetText(settingsDlg.GetCommentText());
				m_pTextComment->SetFont(settingsDlg.m_Font, GetDC());

				m_hImageBuffer.Copy(m_hImage);

				m_pTextComment->DrawText(m_hImage, CPaintDC(GetDesktopWindow()));	

				Invalidate(FALSE);
			} 

			break;
		}
	}
	
	CWnd::OnLButtonDown(nFlags, point);
}

void CCxImageCtrl::OnMouseMove(UINT nFlags, CPoint point) 
{
	CPoint	imagePoint(point);
	
	ClientToImage(imagePoint);

	CRect clientRect;
	GetClientRect(&clientRect);
	
	if(m_hImage.IsInside(imagePoint.x,imagePoint.y) || (GetState()==CCxImageCtrl::IS_TextMove && m_bCaptureWasSet))
	{
		switch(GetState()) 
		{
		case CCxImageCtrl::IS_Move:
			if(m_bCaptureWasSet)
			{
				SCROLLINFO si	=	{0};
				
				if(m_dwWidth>clientRect.Width())
				{
					GetScrollInfo(SB_HORZ,&si);
					m_nHScrollPos = m_RefScroll.cx - point.x + m_RefPoint.x;

					if(m_nHScrollPos<si.nMin)
						m_nHScrollPos = si.nMin;
					if(m_nHScrollPos>si.nMax)
						m_nHScrollPos = si.nMax;
					
					SetScrollPos(SB_HORZ,m_RefScroll.cx - point.x + m_RefPoint.x); 
					Invalidate(FALSE);
				}
				
				if(m_dwHeight>clientRect.Height())
				{
					GetScrollInfo(SB_VERT,&si);
					m_nVScrollPos = m_RefScroll.cy - point.y + m_RefPoint.y;
					
					if(m_nVScrollPos<si.nMin)
						m_nVScrollPos = si.nMin;
					if(m_nVScrollPos>si.nMax)
						m_nVScrollPos = si.nMax;
					
					SetScrollPos(SB_VERT,m_RefScroll.cy - point.y + m_RefPoint.y); 
					Invalidate(FALSE);
				}
			}
			break;
		case CCxImageCtrl::IS_Crop:
			if(m_bCaptureWasSet)
			{
				m_CropActivePoint = point;

				ClientToImage(m_CropActivePoint);
				ImageToClient(m_CropActivePoint);
				
				Invalidate(FALSE);
			}
			break;
		case CCxImageCtrl::IS_TextMove:
			if (m_pTextComment != NULL)
			{
				if (!m_bCaptureWasSet)
				{
					if (!m_pTextComment->GetRectangle().PtInRect(imagePoint))
					{
						SetState(IS_Text);
						SetCursor(AfxGetApp()->LoadCursor(IDC_TEXTCURSOR));
					}
				}
				else
				{
					m_RefPoint = imagePoint;
					m_RefPoint.Offset(m_RefOffset);

					m_hImage.Copy(m_hImageBuffer);
					m_pTextComment->SetPosition(m_RefPoint, GetDC());

					m_pTextComment->DrawText(m_hImage, CPaintDC(GetDesktopWindow()));
					Invalidate(false);
				}
			}
			break;
		case CCxImageCtrl::IS_Text:
			if (m_pTextComment != NULL)
			{
				if (m_pTextComment->GetRectangle().PtInRect(imagePoint))
				{
					SetState(IS_TextMove);
					SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR2));
					
					Invalidate(false);
				}
			}
			break;
		case CCxImageCtrl::IS_Pen:
			{
				// Draw Line 
				if(m_bCaptureWasSet)
				{
					CPoint	fromPoint(m_RefPoint), toPoint(point);
					
					ClientToImage(fromPoint);
					ClientToImage(toPoint);
					
					m_hImage.DrawLine(fromPoint.x, toPoint.x, m_hImage.GetHeight()-fromPoint.y,m_hImage.GetHeight()-toPoint.y,GetPenColor());
					
					//and remember new position [4/7/2004]
					m_RefScroll = CSize(m_nHScrollPos,m_nVScrollPos);
					m_RefPoint = point;

					Invalidate(FALSE);
				}
			}
			break;
		}
	}
	
	
	CWnd::OnMouseMove(nFlags, point);
}

void CCxImageCtrl::OnLButtonUp(UINT nFlags, CPoint point) 
{
	CPoint	imagePoint(point);
	
	ClientToImage(imagePoint);
	
	if(m_bCaptureWasSet&&m_hImage.IsInside(imagePoint.x,imagePoint.y))
	{
		CPoint lefttop(m_RefPoint), rightbottom(point);
		ClientToImage(lefttop);
		ClientToImage(rightbottom);

		switch(GetState()) 
		{
		case CCxImageCtrl::IS_Move:
			SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR1));
			break;
		case CCxImageCtrl::IS_Crop:

			SaveImageToUndo();

			m_hImage.Crop(CRect(lefttop,rightbottom));

			//  [Tue 7/6/2004]
			//m_bCurrentImageWasSave = false;
			m_bIsSaved = false;

			Refresh();

			break;
		case CCxImageCtrl::IS_TextMove:
		case CCxImageCtrl::IS_Text:
			
			m_bIsSaved = false;

			break;
		case CCxImageCtrl::IS_Pen:
			CPoint	fromPoint(m_RefPoint), toPoint(point);
			
			ClientToImage(fromPoint);
			ClientToImage(toPoint);
			
			m_hImage.DrawLine(fromPoint.x, toPoint.x, m_hImage.GetHeight()-fromPoint.y,m_hImage.GetHeight()-toPoint.y,GetPenColor());

			//  [Tue 7/6/2004]
			//m_bCurrentImageWasSave = false;
			m_bIsSaved = false;

			break;		
		}
	}

	if(m_bCaptureWasSet)
	{
		ReleaseCapture();
		Invalidate(FALSE);
		m_bCaptureWasSet = FALSE;
	}
	
	
	CWnd::OnLButtonUp(nFlags, point);
}

void CCxImageCtrl::OnLButtonDblClk(UINT nFlags, CPoint point)
{
	switch (GetState())
	{
		case CCxImageCtrl::IS_TextMove:
			if (m_pTextComment != NULL)
			{
				CTextCommentSettings settingsDlg(this);

				settingsDlg.m_fontColor = m_TextColor;
				settingsDlg.m_fontSize = m_TextSize;
				settingsDlg.m_fontName = m_FontName;

				settingsDlg.SetCommentText(m_pTextComment->GetText());
				settingsDlg.SetFont(m_pTextComment->GetFont());
				settingsDlg.SetColor(m_pTextComment->GetColor());

				if(settingsDlg.DoModal()==IDOK)
				{
					m_TextColor = settingsDlg.m_fontColor;
					m_TextSize = settingsDlg.m_fontSize;
					m_FontName = settingsDlg.m_fontName;

					m_pTextComment->SetColor(m_TextColor);
					m_pTextComment->SetText(settingsDlg.GetCommentText());
					m_pTextComment->SetFont(settingsDlg.m_Font, GetDC());

					m_hImage.Copy(m_hImageBuffer);

					m_pTextComment->DrawText(m_hImage, CPaintDC(GetDesktopWindow()));	

					Invalidate(FALSE);
				} 
			}
			break;
	}

	CWnd::OnLButtonDblClk(nFlags, point);
}

void	CCxImageCtrl::SetState(CCxImageCtrl::CCxImageCtrlState State)
{
	m_State = State;
}

CCxImageCtrl::CCxImageCtrlState	CCxImageCtrl::GetState()
{
	return m_State;
}

BOOL CCxImageCtrl::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CRect clientRect;
	GetClientRect(&clientRect);

	//TRACE("CCxImageCtrl::OnSetCursor nHitTest = %d\r\n",nHitTest);
	
	switch(GetState())
	{
		case CCxImageCtrl::IS_Move:
			if(m_dwWidth>clientRect.Width()||m_dwHeight>clientRect.Height())
			{
				if(m_bCaptureWasSet)
				{
					SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR2));
					return FALSE;
				}
				else
				{
					if(nHitTest == HTCLIENT)
					{
						SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR1));
						return FALSE;
					}
				}
			}
			break;
		case CCxImageCtrl::IS_Crop:
			if(nHitTest == HTCLIENT||m_bCaptureWasSet)
			{
				SetCursor(AfxGetApp()->LoadCursor(IDC_SELECTCURSOR));
				return FALSE;
			}
			break;
		case CCxImageCtrl::IS_Pen:
			if(nHitTest == HTCLIENT||m_bCaptureWasSet)
			{
				SetCursor(AfxGetApp()->LoadCursor(IDC_PENCURSOR));
				return FALSE;
			}
			break;
		case CCxImageCtrl::IS_TextMove:
			if (nHitTest == HTCLIENT)
			{
				SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCATCHCURSOR2));
				return FALSE;
			}
			break;
		case CCxImageCtrl::IS_Text:
			if(nHitTest == HTCLIENT||m_bCaptureWasSet)
			{
				SetCursor(AfxGetApp()->LoadCursor(IDC_TEXTCURSOR));
				return FALSE;
			}
			break;
	}
	
	return CWnd::OnSetCursor(pWnd,nHitTest,message);
}

void CCxImageCtrl::ClientToImage(CPoint &point)
{
	if (!m_hImage.IsValid()) 
		return;

	if (GetStretchMode())	
	{
		CRect rect;
		GetClientRect(&rect);
		
		int width = rect.right - rect.left;
		int height = rect.bottom - rect.top;
		
		double fx = point.x*m_hImage.GetWidth()/(double)width;
		double fy = point.y*m_hImage.GetHeight()/(double)height;

		point.x = (long)fx;
		point.y = (long)fy;
	}
	else
	{
		double fx =(double)(m_nHScrollPos + point.x);
		double fy =(double)(m_nVScrollPos + point.y);
		
		fx	/=	GetZoomValue();
		fy	/=	GetZoomValue();

		point.x = (long)fx;
		point.y = (long)fy;
	}
}

void CCxImageCtrl::ImageToClient(CPoint &point)
{
	if (!m_hImage.IsValid()) 
		return;

	if (GetStretchMode())	
	{
		CRect rect;
		GetClientRect(&rect);
		
		int width = rect.right - rect.left;
		int height = rect.bottom - rect.top;
		
		double fx = point.x*width/(double)m_hImage.GetWidth();
		double fy = point.y*height/(double)m_hImage.GetHeight();

		point.x = (long)fx;
		point.y = (long)fy;
	}
	else
	{
		double fx	=	point.x*GetZoomValue();
		double fy	=	point.y*GetZoomValue();

		fx	-=	m_nHScrollPos;
		fy	-=	m_nVScrollPos;
		
		point.x = (long)fx;
		point.y = (long)fy;
	}
}


void CCxImageCtrl::OnCaptureChanged(CWnd *pWnd) 
{
	if(m_bCaptureWasSet)
	{
		ReleaseCapture();
		m_bCaptureWasSet = FALSE;
		Invalidate(FALSE);
	}
	
	CWnd::OnCaptureChanged(pWnd);
}

BOOL	CCxImageCtrl::Cancel()
{
	if(m_bCaptureWasSet)
	{
		ReleaseCapture();
		m_bCaptureWasSet = FALSE;
		Invalidate(FALSE);
		return TRUE;
	}
	return FALSE;
}

bool CCxImageCtrl::Save (LPCTSTR filename, DWORD imagetype)
{
	return m_hImage.Save(filename, imagetype);
}

bool	CCxImageCtrl::ShowSaveFileDialog()
{
	CFileDialog		saveImageDialog(FALSE,NULL,_T("ScreenCapture"),OFN_EXPLORER|OFN_OVERWRITEPROMPT,_T("Jpg Files (*.jpg)|*.jpg|Bmp Files(*.bmp)|*.bmp|"),this);

	int iRetVal = saveImageDialog.DoModal();
	if(iRetVal==IDOK)

	{
		CString strFileExt = saveImageDialog.GetFileExt();
		CString strFilePath = saveImageDialog.GetPathName();

		if(saveImageDialog.m_ofn.nFilterIndex==1)

		{
			if(strFileExt.GetLength()==0)
				strFilePath	+=	_T(".jpg");
			
			return Save(strFilePath, CXIMAGE_FORMAT_JPG);

		}
		else if(saveImageDialog.m_ofn.nFilterIndex==2)
		{
			if(strFileExt.GetLength()==0)
				strFilePath	+=	_T(".bmp");
			
			return Save(strFilePath, CXIMAGE_FORMAT_BMP);
		}
	}

	return false;
}

void CCxImageCtrl::SaveImageToUndo()
{
	if(m_iUndoListCurrPosition<m_UndoList.size())
	{
		// The Complex Mode. Clear all items Just Add item to end [4/8/2004]
		while(m_iUndoListCurrPosition<m_UndoList.size())
		{
			CxImage *pImage =  m_UndoList.back();
			delete pImage;
			m_UndoList.pop_back();
		}
	}

	if(m_bCurrentImageWasSave)
	{
		m_bCurrentImageWasSave = false;
	}
	else
	{
		// The Simple Mode. Just Add item to end [4/8/2004]
		CxImage	*pNewImage	=	new CxImage(m_hImage);
		m_UndoList.push_back(pNewImage);
		m_iUndoListCurrPosition =	m_UndoList.size();
	}

	GetParent()->SendMessage(WM_UNDO_UPDATED,0,0);
	
}

bool CCxImageCtrl::IsUndoEnable()
{
	return (m_iUndoListCurrPosition>0&&m_iUndoListCurrPosition==m_UndoList.size())||
		(m_iUndoListCurrPosition>1&&m_iUndoListCurrPosition<m_UndoList.size());
}

void CCxImageCtrl::Undo()
{
	// Step 1. Save Current Image [4/8/2004]
	if(m_iUndoListCurrPosition==m_UndoList.size()&&!m_bCurrentImageWasSave)
	{
		m_bCurrentImageWasSave = true;

		CxImage	*pNewImage	=	new CxImage(m_hImage);
		m_UndoList.push_back(pNewImage);

		// Step 2. Restore old value [4/8/2004]
		long iTemValue	=	m_iUndoListCurrPosition;
		for(CImageUndoEnum	Index = m_UndoList.begin();Index!=m_UndoList.end();Index++)
		{
			iTemValue--;
			if(iTemValue==0)
			{
				m_hImage	=	*(*Index);
				break;
			}
		}
	}
	else
	{
		// Step 2. Restore old value [4/8/2004]
		long iTemValue	=	--m_iUndoListCurrPosition;
		for(CImageUndoEnum	Index = m_UndoList.begin();Index!=m_UndoList.end();Index++)
		{
			iTemValue--;
			if(iTemValue==0)
			{
				m_hImage	=	*(*Index);
				break;
			}
		}
		//  [Tue 7/13/2004]
		m_bIsSaved = false;
	}

	GetParent()->SendMessage(WM_UNDO_UPDATED,0,0);

	if (m_pTextComment)
	{
		delete m_pTextComment;
		m_pTextComment = NULL;
	}

	Refresh();
}

bool CCxImageCtrl::IsRedoEnable()
{
	return m_iUndoListCurrPosition<m_UndoList.size();
}

void CCxImageCtrl::Redo()
{
	// Step 2. Restore old value [4/8/2004]
	long iTemValue	=	++m_iUndoListCurrPosition ;
	for(CImageUndoEnum	Index = m_UndoList.begin();Index!=m_UndoList.end();Index++)
	{
		iTemValue--;
		if(iTemValue==0)
		{
			m_hImage	=	*(*Index);
			break;
		}
	}

	//  [Tue 7/13/2004]
	m_bIsSaved = false;

	GetParent()->SendMessage(WM_UNDO_UPDATED,0,0);
	
	Refresh();
}

void	CCxImageCtrl::SetTextColor(COLORREF Color)
{
	m_TextColor	=	Color;
}

COLORREF	CCxImageCtrl::GetTextColor()
{
	return m_TextColor;
}

void	CCxImageCtrl::SetTextSize(int Size)
{
	m_TextSize	=	Size;
}

int		CCxImageCtrl::GetTextSize()
{
	return m_TextSize;
}

void	CCxImageCtrl::SetFontName(LPCTSTR Name)
{
	m_FontName	=	Name;
}

CString		CCxImageCtrl::GetFontName()
{
	return m_FontName;
}

void	CCxImageCtrl::SetPenColor(COLORREF Color)
{
	m_PenColor	=	Color;
}

COLORREF	CCxImageCtrl::GetPenColor()
{
	return m_PenColor;
}

BOOL CCxImageCtrl::IsSaved()
{
	return m_bIsSaved;
}

void CCxImageCtrl::SetQuality(BYTE q)
{
	m_btQuality = q>100 || q<0 ? m_btQuality : q;
	m_hImage.SetJpegQuality(m_btQuality);
}

BYTE CCxImageCtrl::GetQuality()
{
	return m_btQuality;
}