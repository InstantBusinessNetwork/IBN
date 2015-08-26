// ResizableImage.cpp: implementation of the CResizableImage class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ResizableImage.h"

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

CResizableImage::CResizableImage()
{
	m_pImage	=	NULL;
}

CResizableImage::~CResizableImage()
{
	Destroy();
}

HRESULT CResizableImage::Create(LPPICTURE &pImage, const LPSIZE pRealSize)
{
	_ASSERTE(m_pImage==NULL);

	HRESULT hr	=	S_OK;
	
	if(pImage!=NULL)
		hr = pImage->QueryInterface(IID_IPicture,(void**)&m_pImage);

	if(SUCCEEDED(hr))
	{
		m_pImage->get_Width(&m_ImageSize.cx);
		m_pImage->get_Height(&m_ImageSize.cy);
		if(pRealSize)
		{
			memcpy(&m_RealSize,pRealSize,sizeof(SIZE));
			//PixelToHiMetric(&m_RealSize,&m_RealSize);
		}
		else 
		{
			m_pImage->get_Width(&m_RealSize.cx);
			m_pImage->get_Height(&m_RealSize.cy);
			HiMetricToPixel(&m_RealSize,&m_RealSize);
		}
		
	}
	
	return hr;
}

void CResizableImage::Destroy()
{
	if(m_pImage)
	{
		m_pImage->Release();
		m_pImage	=	NULL;
	}
}

void CResizableImage::AddAnchor(RECT PixelSurface, SIZE LTType, SIZE RBType, ZoomType ZoomVal)
{
	SIZE tmpSizeLT = {PixelSurface.left,PixelSurface.top}, tmpSizeRB = {PixelSurface.right,PixelSurface.bottom};
	PixelToHiMetric(&tmpSizeLT,&tmpSizeLT);
	PixelToHiMetric(&tmpSizeRB,&tmpSizeRB);
	
	RECT HiSurface;
	HiSurface.left = tmpSizeLT.cx;
	HiSurface.top = tmpSizeLT.cy;
	HiSurface.right = tmpSizeRB.cx;
	HiSurface.bottom = tmpSizeRB.cy;

	Layout	newItem(HiSurface, PixelSurface, LTType, RBType, ZoomVal);
	m_SurfacesList.push_back(newItem);
}

inline void CResizableImage::HiMetricToPixel(const SIZEL *lpSizeInHiMetric, LPSIZEL lpSizeInPix)
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

inline void CResizableImage::PixelToHiMetric(const SIZEL *lpSizeInPix, LPSIZEL lpSizeInHiMetric)
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

void CResizableImage::Render(HDC hDC, const SIZE &RectSize)
{
	RECT rbBoundRect = {0,0,0,0};

	SIZE HiRectSize	=	{0};

	//PixelToHiMetric(&RectSize,&HiRectSize);
	CSize PixelZoomSize;
	PixelZoomSize.cx	= RectSize.cx	-	m_RealSize.cx;
	PixelZoomSize.cy	= RectSize.cy	-	m_RealSize.cy;

	for(std::list<Layout>::iterator i = m_SurfacesList.begin();i!=m_SurfacesList.end();i++)
	{
		RECT	newDestSurfaceRect;

		newDestSurfaceRect.left		=	(*i).PixelSurface.left	+ PixelZoomSize.cx*(*i).LTType.cx/100;
		newDestSurfaceRect.right	=	(*i).PixelSurface.right	+ PixelZoomSize.cx*(*i).RBType.cx/100;
		newDestSurfaceRect.top		=	(*i).PixelSurface.top	+ PixelZoomSize.cy*(*i).LTType.cy/100;
		newDestSurfaceRect.bottom	=	(*i).PixelSurface.bottom + PixelZoomSize.cy*(*i).RBType.cy/100;

		_ASSERTE(m_pImage!=NULL);

		switch((*i).ZoomVal)
		{
		case STRETCH:
			m_pImage->Render(hDC,
				newDestSurfaceRect.left, newDestSurfaceRect.top,
				newDestSurfaceRect.right-newDestSurfaceRect.left, newDestSurfaceRect.bottom-newDestSurfaceRect.top,
				(*i).HiSurface.left, m_ImageSize.cy-(*i).HiSurface.top,
				(*i).HiSurface.right-(*i).HiSurface.left, -(*i).HiSurface.bottom+(*i).HiSurface.top,
				&rbBoundRect);
			break;
		case DUPLICATE:
			{
				SIZE SrcPixel, DestPixel, AddonPixel, AddonHi, SrcHi;

				DestPixel.cx	=	newDestSurfaceRect.right - newDestSurfaceRect.left;
				DestPixel.cy	=	newDestSurfaceRect.bottom - newDestSurfaceRect.top;
				SrcPixel.cx		=	(*i).PixelSurface.right-(*i).PixelSurface.left;
				SrcPixel.cy		=	(*i).PixelSurface.bottom-(*i).PixelSurface.top;

				int WidthAddonCount		= DestPixel.cx/SrcPixel.cx;
				int HeightAddonCount	= DestPixel.cy/SrcPixel.cy;

				AddonPixel.cx	=	DestPixel.cx-WidthAddonCount*SrcPixel.cx;
				AddonPixel.cy	=	DestPixel.cy-HeightAddonCount*SrcPixel.cy;

				PixelToHiMetric(&AddonPixel,&AddonHi);
				PixelToHiMetric(&SrcPixel,&SrcHi);

				for(int xIter=0;xIter<=WidthAddonCount;xIter++)
					for(int yIter=0;yIter<=HeightAddonCount;yIter++)
					{
						m_pImage->Render(hDC,
							newDestSurfaceRect.left+SrcPixel.cx*xIter,
							newDestSurfaceRect.top+SrcPixel.cy*yIter,
							(xIter==WidthAddonCount)?(AddonPixel.cx):(SrcPixel.cx),
							(yIter==HeightAddonCount)?(AddonPixel.cy):(SrcPixel.cy),
							(*i).HiSurface.left,
							m_ImageSize.cy-(*i).HiSurface.top,
							(xIter==WidthAddonCount)?(AddonHi.cx):(SrcHi.cx),
							(yIter==HeightAddonCount)?(-AddonHi.cy):(-SrcHi.cy),
							&rbBoundRect);
					}
			}
			break;
		default:
			_ASSERTE(FALSE);
		}
	}
}

void CResizableImage::Render(HDC hDC, const RECT &Rect)
{
	RECT rbBoundRect = {0,0,0,0};
	
	SIZE HiRectSize	=	{0};
	
	//PixelToHiMetric(&RectSize,&HiRectSize);
	CSize PixelZoomSize;
	PixelZoomSize.cx = Rect.right - Rect.left - m_RealSize.cx;
	PixelZoomSize.cy = Rect.bottom - Rect.top - m_RealSize.cy;
	
	for(std::list<Layout>::iterator i = m_SurfacesList.begin();i!=m_SurfacesList.end();i++)
	{
		RECT	newDestSurfaceRect;
		
		newDestSurfaceRect.left		= Rect.left + (*i).PixelSurface.left + PixelZoomSize.cx*(*i).LTType.cx/100;
		newDestSurfaceRect.right	= Rect.left + (*i).PixelSurface.right + PixelZoomSize.cx*(*i).RBType.cx/100;
		newDestSurfaceRect.top		= Rect.top + (*i).PixelSurface.top + PixelZoomSize.cy*(*i).LTType.cy/100;
		newDestSurfaceRect.bottom	= Rect.top + (*i).PixelSurface.bottom + PixelZoomSize.cy*(*i).RBType.cy/100;
		
		_ASSERTE(m_pImage!=NULL);
		
		switch((*i).ZoomVal)
		{
		case STRETCH:
			m_pImage->Render(hDC,
				newDestSurfaceRect.left, newDestSurfaceRect.top,
				newDestSurfaceRect.right-newDestSurfaceRect.left, newDestSurfaceRect.bottom-newDestSurfaceRect.top,
				(*i).HiSurface.left,m_ImageSize.cy-(*i).HiSurface.top,
				(*i).HiSurface.right-(*i).HiSurface.left,-(*i).HiSurface.bottom+(*i).HiSurface.top,
				&rbBoundRect);
			break;
		case DUPLICATE:
			{
				SIZE SrcPixel, DestPixel, AddonPixel, AddonHi, SrcHi;
				
				DestPixel.cx	=	newDestSurfaceRect.right - newDestSurfaceRect.left;
				DestPixel.cy	=	newDestSurfaceRect.bottom - newDestSurfaceRect.top;
				SrcPixel.cx		=	(*i).PixelSurface.right-(*i).PixelSurface.left;
				SrcPixel.cy		=	(*i).PixelSurface.bottom-(*i).PixelSurface.top;
				
				int WidthAddonCount		= DestPixel.cx/SrcPixel.cx;
				int HeightAddonCount	= DestPixel.cy/SrcPixel.cy;
				
				AddonPixel.cx	=	DestPixel.cx-WidthAddonCount*SrcPixel.cx;
				AddonPixel.cy	=	DestPixel.cy-HeightAddonCount*SrcPixel.cy;
				
				PixelToHiMetric(&AddonPixel,&AddonHi);
				PixelToHiMetric(&SrcPixel,&SrcHi);
				
				for(int xIter=0;xIter<=WidthAddonCount;xIter++)
					for(int yIter=0;yIter<=HeightAddonCount;yIter++)
					{
						m_pImage->Render(hDC,
							newDestSurfaceRect.left+SrcPixel.cx*xIter,
							newDestSurfaceRect.top+SrcPixel.cy*yIter,
							(xIter==WidthAddonCount)?(AddonPixel.cx):(SrcPixel.cx),
							(yIter==HeightAddonCount)?(AddonPixel.cy):(SrcPixel.cy),
							(*i).HiSurface.left,
							m_ImageSize.cy-(*i).HiSurface.top,
							(xIter==WidthAddonCount)?(AddonHi.cx):(SrcHi.cx),
							(yIter==HeightAddonCount)?(-AddonHi.cy):(-SrcHi.cy),
							&rbBoundRect);
					}
			}
			break;
		default:
			_ASSERTE(FALSE);
		}
	}
}

void CResizableImage::AddWholeImage(ZoomType ZoomVal)
{
	AddAnchor(CRect(CPoint(0, 0), m_RealSize), CSize(0, 0), CSize(100, 100), ZoomVal);
}
