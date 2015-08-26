// ResizableImage.h: interface for the CResizableImage class.
//
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// Name	  :	CResizableImage
// Version: 1.0 [01/11/2002]
// Author :	Zhuk Oleg, Russian Federation, Kaliningrad city
// Email  :	zhuk@mediachase.com
//////////////////////////////////////////////////////////////////////
// Info:  	This class draw a resizable image.
//////////////////////////////////////////////////////////////////////
//  Requirements:
//			Windows NT/2000: Requires Windows NT.
//			Windows 95/98: Requires Windows 95.
//////////////////////////////////////////////////////////////////////

#include <list>

#if !defined(AFX_RESIZABLEIMAGE_H__2B6A09D1_5B6B_423A_93A8_686321C0511E__INCLUDED_)
#define AFX_RESIZABLEIMAGE_H__2B6A09D1_5B6B_423A_93A8_686321C0511E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CResizableImage  
{
public:
	typedef enum _E_ZoomType
	{
		STRETCH		=	1,
		DUPLICATE	=	2,
	} ZoomType;
	
	struct Layout
	{
		Layout()
		{
			ZeroMemory(&HiSurface,sizeof(RECT));
			ZeroMemory(&PixelSurface,sizeof(RECT));
			ZeroMemory(&LTType,sizeof(SIZE));
			ZeroMemory(&RBType,sizeof(SIZE));
			ZoomVal	=	DUPLICATE;
		}
		Layout(RECT HiSurface, RECT PixelSurface, SIZE LTType, SIZE RBType, ZoomType ZoomVal)
		{
			this->HiSurface		=	HiSurface;
			this->PixelSurface	=	PixelSurface;
			this->LTType		=	LTType;
			this->RBType		=	RBType;
			this->ZoomVal		=	ZoomVal;
		}
		RECT		HiSurface;
		RECT		PixelSurface;
		SIZE		LTType;
		SIZE		RBType;
		ZoomType	ZoomVal;
	};
public:
	void AddWholeImage(ZoomType ZoomVal);
	void Render(HDC hDC, const RECT &Rect);
	void Render(HDC hDC, const SIZE &RectSize);
	static inline void PixelToHiMetric(const SIZEL * lpSizeInPix, LPSIZEL lpSizeInHiMetric);
	static inline void HiMetricToPixel(const SIZEL * lpSizeInHiMetric, LPSIZEL lpSizeInPix);
	void AddAnchor(RECT PixelSurface, SIZE LTType, SIZE RBType, ZoomType ZoomVal = CResizableImage::STRETCH);
	void Destroy();
	HRESULT Create(LPPICTURE& pImage, const LPSIZE pRealSize = NULL);
	CResizableImage();
	virtual ~CResizableImage();

protected:
	LPPICTURE			m_pImage;
	SIZE				m_RealSize;
	SIZE				m_ImageSize;
	std::list<Layout>	m_SurfacesList;
};

/************************************************************************/
/* LPPICTUREDISP CPictureHolder::GetPictureDispatch()
{
LPPICTUREDISP pPictDisp = NULL;

  if ((m_pPict != NULL) &&
		SUCCEEDED(m_pPict->QueryInterface(IID_IPictureDisp, (LPVOID*)&pPictDisp)))
		{
		ASSERT(pPictDisp != NULL);
		}
		
		  return pPictDisp;
		  }
		  
			void CPictureHolder::SetPictureDispatch(LPPICTUREDISP pDisp)
			{
			LPPICTURE pPict = NULL;
			
			  if (m_pPict != NULL)
			  m_pPict->Release();
			  
				if ((pDisp != NULL) &&
				SUCCEEDED(pDisp->QueryInterface(IID_IPicture, (LPVOID*)&pPict)))
				{
				ASSERT(pPict != NULL);
				
				  m_pPict = pPict;
				  }
				  else
				  {
				  m_pPict = NULL;
				  }
				  }
				  
					void CPictureHolder::Render(CDC* pDC, const CRect& rcRender,
					const CRect& rcWBounds)
					{
					if (m_pPict != NULL)
					{
					long hmWidth;
					long hmHeight;
					
					  m_pPict->get_Width(&hmWidth);
					  m_pPict->get_Height(&hmHeight);
					  
						m_pPict->Render(pDC->m_hDC, rcRender.left, rcRender.top,
						rcRender.Width(), rcRender.Height(), 0, hmHeight-1,
						hmWidth, -hmHeight, (LPCRECT)rcWBounds);
						}
						}
*/
/************************************************************************/

#endif // !defined(AFX_RESIZABLEIMAGE_H__2B6A09D1_5B6B_423A_93A8_686321C0511E__INCLUDED_)
