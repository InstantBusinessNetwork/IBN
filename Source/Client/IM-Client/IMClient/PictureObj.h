// PictureObj.h: interface for the CPictureObj class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_PICTUREOBJ_H__FEE56678_FD6F_4E2C_8C84_FCCE577DC4B7__INCLUDED_)
#define AFX_PICTUREOBJ_H__FEE56678_FD6F_4E2C_8C84_FCCE577DC4B7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "ResizableImage.h"

class CPictureObj  
{
public:
	void AddFragment(RECT PixelSurface, SIZE LTType, SIZE RBType, long type);
	void Render(HDC hDC, RECT r);
	void AddWholeImage(CResizableImage::ZoomType ZoomVal);
	static inline void PixelToHiMetric(const SIZEL * lpSizeInPix, LPSIZEL lpSizeInHiMetric);
	static inline void HiMetricToPixel(const SIZEL * lpSizeInHiMetric, LPSIZEL lpSizeInPix);
	void Render(HDC hDC);
	void SetRect(long x, long y, long cx, long cy);
	void GetRect(LPRECT pRect);
	BOOL Create(IStream *pStream, long x, long y, long cx = 0, long cy = 0);
	CPictureObj();
	virtual ~CPictureObj();

protected:
//	CWnd *m_pParentWnd;
//	LPPICTURE m_pPicture;
	CResizableImage *m_pPicture;
	long x;
	long y;
	long cx;
	long cy;
	SIZE m_RealSize;
};

#endif // !defined(AFX_PICTUREOBJ_H__FEE56678_FD6F_4E2C_8C84_FCCE577DC4B7__INCLUDED_)
