// MonitorGraph.h: interface for the CMonitorGraph class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MONITORGRAPH_H__E35431E4_D1BA_4E25_B91D_0D25F8D3A445__INCLUDED_)
#define AFX_MONITORGRAPH_H__E35431E4_D1BA_4E25_B91D_0D25F8D3A445__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CMonitorGraph : public CStatic  
{
public:
	class CMonitorGraphLine
	{
		public:
			CMonitorGraphLine()
			{
				m_pLineData = NULL;
				m_lDataCount = 0;
				m_LineColor = RGB(0,0,0);
				m_strTile = _T("");
				m_lLineWidth = 1;
			}

			CMonitorGraphLine(LPCTSTR LineTitle, COLORREF LineColor, LONG *pLineData, LONG lDataCount)
			{
				this->CreateLine(LineTitle, LineColor, pLineData, lDataCount);
			}
			
			CMonitorGraphLine(const CMonitorGraphLine& src)
			{
				*this = src;
			}

			virtual ~CMonitorGraphLine()
			{
				this->Delete();
			}

			const CMonitorGraphLine& operator=(const CMonitorGraphLine& src)
			{
				CreateLine(src.m_strTile, src.m_LineColor, src.m_pLineData ,src.m_lDataCount);
				return *this;
			}

			LPCTSTR	GetTitle() const
			{
				return m_strTile;
			}
			
			COLORREF GetColor() const
			{
				return m_LineColor;
			}
			

			LONG GetDataSize() const
			{
				return m_lDataCount;
			}

			LONG* GetDataBuffer() const
			{
				return m_pLineData;
			}

			/*LONG operator[](LONG Index) 
			{
				if(Index>=0 && Index<this->GetDataSize())
					return m_pLineData[Index];
				
				ASSERT(FALSE); // Index Out From Range
				return 0;
			}*/

			void Delete()
			{
				m_lDataCount = 0;
				if(m_pLineData!=NULL)
				{
					delete [] m_pLineData;
					m_pLineData = NULL;
				}
				m_LineColor = RGB(0,0,0);
				m_strTile = _T("");
			}

			void CreateLine(LPCTSTR LineTitle, COLORREF LineColor, LONG lDataCount)
			{
				this->Delete();
				m_lDataCount = lDataCount;
				m_strTile = LineTitle;
				m_LineColor = LineColor;
				m_pLineData = new LONG[lDataCount];
				ZeroMemory(this->m_pLineData,this->m_lDataCount*sizeof(LONG));
			}
			
			void CreateLine(LPCTSTR LineTitle, COLORREF LineColor, LONG *pLineData, LONG lDataCount)
			{
				this->Delete();
				m_lDataCount = lDataCount;
				m_strTile = LineTitle;
				m_LineColor = LineColor;
				m_pLineData = new LONG[lDataCount];
				memcpy(m_pLineData,pLineData,this->m_lDataCount);
			}

			void SetLineWidth(LONG LineWidth)
			{
				m_lLineWidth = LineWidth;
			}

			LONG GetLineWidth()
			{
				return m_lLineWidth;
			}

		private:
			LONG		m_lLineWidth;
			LONG		m_lDataCount;
			LONG		*m_pLineData;
			CString		m_strTile;
			COLORREF	m_LineColor;
	} ;
	
public:
	CMonitorGraph();
	virtual ~CMonitorGraph();

protected:
	void DrawLine(CPaintDC &dc,CMonitorGraphLine &line);
	void DrawFrame(CPaintDC &dc);
public:
	LONG GetPointCount();
	void SetPointCount(LONG lNewValue);

	LONG GetVerticalRulerCount();
	void SetVerticalRulerCount(LONG lNewValue);

	LONG GetHorizontalRulerCount();
	void SetHorizontalRulerCount(LONG lNewValue);

	void SetDrawLine(BOOL bShowTotalLine, BOOL bShowSentLine, BOOL bShowReceivedLine);

	void UpdateTotalLine();

	CMonitorGraphLine	m_glSent;
	CMonitorGraphLine	m_glReceived;

private:
	CMonitorGraphLine	m_glTotal;
	LONG				m_MaxTotalValue;
		
private:
	LONG m_lPointCount;
	LONG m_lVerticalRulerCount;
	LONG m_lHorizontalRulerCount;

	LONG m_lVerticalRulerSize;
	LONG m_lHorizontalRulerSize;

	BOOL m_bShowTotalLine;
	BOOL m_bShowSentLine; 
	BOOL m_bShowReceivedLine;
	

protected:
	//{{AFX_MSG(CLabel)
	afx_msg void OnPaint();
	//}}AFX_MSG
	
	DECLARE_MESSAGE_MAP()
};

#endif // !defined(AFX_MONITORGRAPH_H__E35431E4_D1BA_4E25_B91D_0D25F8D3A445__INCLUDED_)
