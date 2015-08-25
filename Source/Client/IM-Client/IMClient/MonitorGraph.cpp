// MonitorGraph.cpp: implementation of the CMonitorGraph class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MonitorGraph.h"
#include "resource.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMonitorGraph::CMonitorGraph()
{
	m_lPointCount = 200L;
	m_lVerticalRulerCount = 10L;
	m_lHorizontalRulerCount = 10L;

	m_lVerticalRulerSize = 40;
	m_lHorizontalRulerSize = 40;

	m_glSent.CreateLine(_T("Sent"), RGB(0xFF,0x0,0x0), 200);
	m_glReceived.CreateLine(_T("Received"), RGB(0x0,0x0,0xFF), 200);
	m_glTotal.CreateLine(_T("Total"), RGB(0x0,0xFF,0x0), 200);

	m_glTotal.SetLineWidth(2);

	m_MaxTotalValue = 0;

	m_bShowTotalLine		=	TRUE;
	m_bShowSentLine		=	TRUE; 
	m_bShowReceivedLine	=	TRUE;
}

CMonitorGraph::~CMonitorGraph()
{

}

BEGIN_MESSAGE_MAP(CMonitorGraph, CStatic)
//{{AFX_MSG_MAP(CLabel)
ON_WM_PAINT()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void CMonitorGraph::SetDrawLine(BOOL bShowTotalLine, BOOL bShowSentLine, BOOL bShowReceivedLine)
{
	m_bShowTotalLine	=	bShowTotalLine;
	m_bShowSentLine		=	bShowSentLine; 
	m_bShowReceivedLine	=	bShowReceivedLine;
}

void CMonitorGraph::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	CRect cleintRect;
	GetClientRect(&cleintRect);

	dc.FillSolidRect(cleintRect,RGB(0xFF,0xFF,0xFF));
	//dc.DrawFocusRect(cleintRect);

	DrawFrame(dc);

	if(m_bShowTotalLine)
		DrawLine(dc,m_glTotal);
	if(m_bShowSentLine)
		DrawLine(dc,m_glSent);
	if(m_bShowReceivedLine)
		DrawLine(dc,m_glReceived);
}

void CMonitorGraph::DrawFrame(CPaintDC &dc)
{
	CPen	pen3, pen1  , *pOldPen;

	pen1.CreatePen(PS_SOLID,1,RGB(102,102,102));
	pen3.CreatePen(PS_SOLID,3,RGB(102,102,102));

	pOldPen = dc.SelectObject(&pen3);
		
	CRect cleintRect;
	GetClientRect(&cleintRect);

	dc.MoveTo(m_lVerticalRulerSize,m_lHorizontalRulerSize);
	dc.LineTo(m_lVerticalRulerSize, cleintRect.Height() - m_lHorizontalRulerSize);
	dc.LineTo(cleintRect.Width() - m_lVerticalRulerSize,cleintRect.Height() - m_lHorizontalRulerSize);

	dc.SelectObject(&pen1);

	CFont	font, *pOldFont;

	CFont *pStaticFont = this->GetFont();

	LOGFONT lf	=	{0};
	pStaticFont->GetLogFont(&lf);
	
	font.CreateFont(-10,0,0,0,
		FW_NORMAL,FALSE,FALSE,0,lf.lfCharSet,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,DEFAULT_QUALITY,DEFAULT_PITCH | FF_SWISS, _T("Arial"));

	pOldFont = dc.SelectObject(&font);

	LONG lMaxKilobytes = (m_MaxTotalValue/1024)+1;
	
	for(int i=0;i<=m_lVerticalRulerCount;i++)
	{
		dc.MoveTo(m_lVerticalRulerSize - 5, cleintRect.Height() - m_lHorizontalRulerSize - i*(cleintRect.Height() - 2*m_lHorizontalRulerSize)/m_lVerticalRulerCount);
		dc.LineTo(m_lVerticalRulerSize, cleintRect.Height() - m_lHorizontalRulerSize - i*(cleintRect.Height() - 2*m_lHorizontalRulerSize)/m_lVerticalRulerCount);

		CRect vLabelRect(0,
						cleintRect.Height() - m_lHorizontalRulerSize - i*(cleintRect.Height() - 2*m_lHorizontalRulerSize)/m_lVerticalRulerCount - 5,
						m_lVerticalRulerSize - 5,
						cleintRect.Height() - m_lHorizontalRulerSize - i*(cleintRect.Height() - 2*m_lHorizontalRulerSize)/m_lVerticalRulerCount + 5);
				
		CString strLabelTmp;
		strLabelTmp.Format(_T("%.1f"), (lMaxKilobytes *1.0 * i)/m_lHorizontalRulerCount);
		dc.DrawText(strLabelTmp,&vLabelRect,DT_RIGHT|DT_VCENTER);
	}

	for(int i=0;i<=m_lHorizontalRulerCount;i++)
	{
		dc.MoveTo(m_lVerticalRulerSize + i*(cleintRect.Width() - 2*m_lVerticalRulerSize)/m_lHorizontalRulerCount, cleintRect.Height() - m_lHorizontalRulerSize);
		dc.LineTo(m_lVerticalRulerSize + i*(cleintRect.Width() - 2*m_lVerticalRulerSize)/m_lHorizontalRulerCount, cleintRect.Height() - m_lHorizontalRulerSize + 5);

		CRect vLabelRect(m_lVerticalRulerSize + i*(cleintRect.Width() - 2*m_lVerticalRulerSize)/m_lHorizontalRulerCount -10, 
			cleintRect.Height() - m_lHorizontalRulerSize +5,
			m_lVerticalRulerSize + i*(cleintRect.Width() - 2*m_lVerticalRulerSize)/m_lHorizontalRulerCount + 10, 
			cleintRect.Height());

		CString strLabelTmp;
		strLabelTmp.Format(_T("%d"), 200 - (200 * i)/m_lHorizontalRulerCount);
		dc.DrawText(strLabelTmp,&vLabelRect,DT_CENTER|DT_VCENTER);
	}

	dc.SetBkMode(TRANSPARENT);

	CRect	rectKilobytes(0,10,2*m_lVerticalRulerSize,m_lHorizontalRulerSize);
	CRect	rectSeconds((cleintRect.Width() - m_lVerticalRulerSize)/2 - m_lVerticalRulerSize,cleintRect.Height() - 20 ,
							(cleintRect.Width() - m_lVerticalRulerSize)/2 + m_lVerticalRulerSize,cleintRect.Height());
	

	dc.DrawText(GetString(IDS_KILOBYTES),&rectKilobytes,DT_CENTER|DT_VCENTER);
	dc.DrawText(GetString(IDS_SECONDS),&rectSeconds,DT_CENTER|DT_BOTTOM);
	
	dc.SelectObject(pOldFont);
	dc.SelectObject(pOldPen);
}

void CMonitorGraph::DrawLine(CPaintDC &dc, CMonitorGraphLine &line)
{
	LONG lMaxKilobytes = (m_MaxTotalValue/1024)+1;

	CPen	pen3, pen1  , *pOldPen;
	
	
	pen1.CreatePen(PS_SOLID,line.GetLineWidth(),line.GetColor());
	//pen3.CreatePen(PS_SOLID,3,line.GetColor());

	pOldPen = dc.SelectObject(&pen1);

	CRect cleintRect;
	GetClientRect(&cleintRect);
	

	LONG GraphHeight = cleintRect.Height() - m_lHorizontalRulerSize;
	LONG GraphWidth = cleintRect.Width() - m_lVerticalRulerSize;
	LONG LineSize  = line.GetDataSize();

	for(int Index = 0; Index<LineSize ;Index++)
	{
		//line.GetDataBuffer()[Index] = rand()%1000;

		long xPoint = (long)(GraphWidth - (Index * (cleintRect.Width() - 2.0*m_lVerticalRulerSize))/LineSize );
		long yPoint = (long)(
								GraphHeight - 
								(
									(cleintRect.Height() - 2*m_lHorizontalRulerSize)*
									(line.GetDataBuffer()[199-Index])
									/1024.0
								)
								/lMaxKilobytes
							);

		if(Index == 0)
			dc.MoveTo(xPoint,yPoint);
		else
			dc.LineTo(xPoint,yPoint);
	}


	dc.SelectObject(pOldPen);
}

LONG CMonitorGraph::GetPointCount()
{
	return m_lPointCount;
}

void CMonitorGraph::SetPointCount(LONG lNewValue)
{
	m_lPointCount = lNewValue;
}

LONG CMonitorGraph::GetVerticalRulerCount()
{
	return m_lVerticalRulerCount;
}

void CMonitorGraph::SetVerticalRulerCount(LONG lNewValue)
{
	m_lVerticalRulerCount = lNewValue;
}

LONG CMonitorGraph::GetHorizontalRulerCount()
{
	return m_lHorizontalRulerCount;
}

void CMonitorGraph::SetHorizontalRulerCount(LONG lNewValue)
{
	m_lHorizontalRulerCount = lNewValue;
}

void CMonitorGraph::UpdateTotalLine()
{
	m_MaxTotalValue = 0;

	memcpy(m_glTotal.GetDataBuffer(),m_glSent.GetDataBuffer(),m_glSent.GetDataSize()*sizeof(LONG));

	for(int Index = 0; Index<200;Index++)
	{
		m_glTotal.GetDataBuffer()[Index] += m_glReceived.GetDataBuffer()[Index];
		if(m_glTotal.GetDataBuffer()[Index]>m_MaxTotalValue)
			m_MaxTotalValue = m_glTotal.GetDataBuffer()[Index];
	}
}
