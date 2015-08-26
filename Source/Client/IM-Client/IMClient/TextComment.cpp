#include "StdAfx.h"
#include ".\textcomment.h"

CTextComment::CTextComment(CPoint point)
{
	m_ptUL = point;
	VERIFY(m_font.CreateFont(
		12,                        // nHeight
		0,                         // nWidth
		0,                         // nEscapement
		0,                         // nOrientation
		FW_NORMAL,                 // nWeight
		FALSE,                     // bItalic
		FALSE,                     // bUnderline
		0,                         // cStrikeOut
		ANSI_CHARSET,              // nCharSet
		OUT_DEFAULT_PRECIS,        // nOutPrecision
		CLIP_DEFAULT_PRECIS,       // nClipPrecision
		DEFAULT_QUALITY,           // nQuality
		DEFAULT_PITCH | FF_SWISS,  // nPitchAndFamily
		"Arial"));                 // lpszFacename

	m_color = RGB(0, 0, 0);
	m_text = _T("");
}

CTextComment::~CTextComment(void)
{
}

void CTextComment::SetPosition(CPoint point, CDC* dc)
{
	CFont* pFont = dc->SelectObject(&m_font);
	CSize size = dc->GetTextExtent(m_text);
	dc->SelectObject(pFont);

	m_rect = CRect(m_ptUL.x, m_ptUL.y, m_ptUL.x+size.cx, m_ptUL.y+size.cy);

	m_ptUL = point;
}

CRect CTextComment::GetRectangle()
{
	return m_rect;
}

CFont& CTextComment::GetFont()
{
	return m_font;
}

void CTextComment::SetFont(CFont& newFont, CDC* dc)
{
	LOGFONT logfont;
	newFont.GetLogFont(&logfont);

	m_font.DeleteObject();
	m_font.CreateFontIndirect(&logfont);

	CFont* pFont = dc->SelectObject(&m_font);
	CSize size = dc->GetTextExtent(m_text);
	dc->SelectObject(pFont);

	m_rect = CRect(m_ptUL.x, m_ptUL.y, m_ptUL.x+size.cx, m_ptUL.y+size.cy);
}

COLORREF CTextComment::GetColor()
{
	return m_color;
}

void CTextComment::SetColor(COLORREF newColor)
{
	m_color = newColor;
}

CString& CTextComment::GetText()
{
	return m_text;
}

void CTextComment::SetText(CString& newText)
{
	m_text = newText;
}

void CTextComment::DrawText(CxImage& image, HDC dc)
{
	if(m_text.IsEmpty())
		return;

	RGBQUAD col;
	col.rgbBlue = GetBValue(m_color);
	col.rgbRed = GetRValue(m_color);
	col.rgbGreen = GetGValue(m_color);

	LOGFONT	lf;
	m_font.GetLogFont(&lf);

	image.DrawString(dc, m_ptUL.x, m_rect.bottom, m_text, col, lf.lfFaceName, lf.lfHeight, lf.lfWeight, lf.lfItalic,lf.lfUnderline);
}