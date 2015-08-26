// PictureObj.cpp: implementation of the CPictureObj class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "PictureObj.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define HIMETRIC_PER_INCH   2540
#define MAP_PIX_TO_LOGHIM(x,ppli)   ( (HIMETRIC_PER_INCH*(x) + ((ppli)>>1)) / (ppli) )
#define MAP_LOGHIM_TO_PIX(x,ppli)   ( ((ppli)*(x) + HIMETRIC_PER_INCH/2) / HIMETRIC_PER_INCH )
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CPictureObj::CPictureObj()
{
	m_pPicture = NULL;
	x = 0;
	y = 0;
	cx = 0;
	cy = 0;
}

CPictureObj::~CPictureObj()
{
	if(m_pPicture)
	{
		delete m_pPicture;
		m_pPicture = NULL;
	}
}

BOOL CPictureObj::Create(IStream *pStream, long x, long y, long cx, long cy)
{
	ASSERT(pStream != NULL);
	ASSERT(m_pPicture == NULL);
	
	BOOL bResult = FALSE;
	LPPICTURE pic = NULL;
	HRESULT hr = ::OleLoadPicture(pStream, 0, TRUE, IID_IPicture, (void**)&pic);
	if(SUCCEEDED(hr) && pic != NULL)
	{
		m_pPicture = new CResizableImage;
		m_pPicture->Create(pic);
		
		this->x = x;
		this->y = y;
		if(cx > 0 && cy > 0)
		{
			this->cx = cx;
			this->cy = cy;
		}
		else
		{
			pic->get_Width(&m_RealSize.cx);
			pic->get_Height(&m_RealSize.cy);
			HiMetricToPixel(&m_RealSize, &m_RealSize);
			this->cx = m_RealSize.cx;
			this->cy = m_RealSize.cy;
		}

		pic->Release();

		bResult = TRUE;
	}
	return bResult;
}

void CPictureObj::GetRect(LPRECT pRect)
{
	pRect->left = x;
	pRect->top = y;
	pRect->right = x + cx;
	pRect->bottom = y + cy;
}

void CPictureObj::SetRect(long x, long y, long cx, long cy)
{
	this->x = x;
	this->y = y;
	this->cx = cx;
	this->cy = cy;
}

void CPictureObj::Render(HDC hDC)
{
	if(m_pPicture)
	{
		RECT r;
		r.left = x;
		r.right = x + cx;
		r.top = y;
		r.bottom = y + cy;
		m_pPicture->Render(hDC, r);
	}
}

inline void CPictureObj::HiMetricToPixel(const SIZEL *lpSizeInHiMetric, LPSIZEL lpSizeInPix)
{
	int nPixelsPerInchX;    // Pixels per logical inch along width
	int nPixelsPerInchY;    // Pixels per logical inch along height
	
	HDC hDCScreen = GetDC(NULL);
	_ASSERTE(hDCScreen != NULL);
	nPixelsPerInchX = GetDeviceCaps(hDCScreen, LOGPIXELSX);
	nPixelsPerInchY = GetDeviceCaps(hDCScreen, LOGPIXELSY);
	ReleaseDC(NULL, hDCScreen);
	
	lpSizeInPix->cx = MAP_LOGHIM_TO_PIX(lpSizeInHiMetric->cx, nPixelsPerInchX);
	lpSizeInPix->cy = MAP_LOGHIM_TO_PIX(lpSizeInHiMetric->cy, nPixelsPerInchY);
}

inline void CPictureObj::PixelToHiMetric(const SIZEL *lpSizeInPix, LPSIZEL lpSizeInHiMetric)
{
	int nPixelsPerInchX;    // Pixels per logical inch along width
	int nPixelsPerInchY;    // Pixels per logical inch along height
	
	HDC hDCScreen = GetDC(NULL);
	_ASSERTE(hDCScreen != NULL);
	nPixelsPerInchX = GetDeviceCaps(hDCScreen, LOGPIXELSX);
	nPixelsPerInchY = GetDeviceCaps(hDCScreen, LOGPIXELSY);
	ReleaseDC(NULL, hDCScreen);
	
	lpSizeInHiMetric->cx = MAP_PIX_TO_LOGHIM(lpSizeInPix->cx, nPixelsPerInchX);
	lpSizeInHiMetric->cy = MAP_PIX_TO_LOGHIM(lpSizeInPix->cy, nPixelsPerInchY);
}

void CPictureObj::AddWholeImage(CResizableImage::ZoomType ZoomVal)
{
	ASSERT(m_pPicture != NULL);
	if(m_pPicture)
		m_pPicture->AddWholeImage(ZoomVal);
}

void CPictureObj::Render(HDC hDC, RECT r)
{
	ASSERT(m_pPicture != NULL);
	if(m_pPicture)
		m_pPicture->Render(hDC, r);
}

void CPictureObj::AddFragment(RECT PixelSurface, SIZE LTType, SIZE RBType, long type)
{
	ASSERT(m_pPicture != NULL);
	if(m_pPicture == NULL)
		return;

	CResizableImage::ZoomType ztype = CResizableImage::STRETCH;
	if(type == 2)
		ztype = CResizableImage::DUPLICATE;
	m_pPicture->AddAnchor(PixelSurface, LTType, RBType, ztype);
}

