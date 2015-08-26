#pragma once

#include "..\CxImage\ximage.h"

class CTextComment
{
public:
	CTextComment(CPoint point);
	virtual ~CTextComment(void);


	CRect GetRectangle();
	CFont& GetFont();
	void SetFont(CFont& newFont, CDC* dc);
	void SetPosition(CPoint point, CDC* dc);
	COLORREF GetColor();
	void SetColor(COLORREF newColor);
	CString& GetText();
	void SetText(CString& newText);
	void DrawText(CxImage& image, HDC dc);

	CPoint GetPosition() { return m_ptUL; }

private:
	CPoint m_ptUL;
	CFont m_font;
	COLORREF m_color;
	CString m_text;
	CRect	m_rect;
};
