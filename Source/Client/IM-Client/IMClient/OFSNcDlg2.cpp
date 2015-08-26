// OFSNcDlg2.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "OFSNcDlg2.h"
#include "LoadSkins.h"
#include "cdib.h"
#include "GlobalFunction.h"
#include "PictureObj.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


extern CString GetCurrentSkin();

// XML Functions

HRESULT COFSNcDlg2::GetNodeAttribute(IXMLDOMNode *pNode, BSTR bsAttrName, CComBSTR &strAttrValue)
{
	HRESULT hr = S_OK;
	IXMLDOMElement *pEle = NULL;
	
	if(pNode == NULL)
		return E_INVALIDARG;
	
	hr = pNode->QueryInterface(IID_IXMLDOMElement, (void**)&pEle);
	if(pEle)
	{
		CComVariant var;
		hr = pEle->getAttribute(bsAttrName, &var);
		strAttrValue = var.bstrVal;
		pEle->Release();
	}
	return hr;
}

HRESULT COFSNcDlg2::GetNodeAttributeAsLong(IXMLDOMNode *pNode, BSTR bsAttrName, long *pAttrValue, int nBase)
{
	HRESULT hr;
	IXMLDOMElement *pEle = NULL;
	WCHAR *szNULL = L"\0x00";
	CComBSTR bs;
	
	ASSERT(pAttrValue != NULL);
	
	if(pNode == NULL)
		return E_INVALIDARG;

	hr = GetNodeAttribute(pNode, bsAttrName, bs);
	if(bs.m_str != NULL)
		*pAttrValue = wcstol(bs.m_str, &szNULL, nBase);
	
	return hr;
}

HRESULT COFSNcDlg2::SelectChildNode(IXMLDOMNode *pNodeParent, BSTR bsSelect, IXMLDOMNode **ppNodeChild, BSTR *pbsNodeText)
{
	HRESULT hr;
	IXMLDOMNodeList *pNodeList = NULL;
	IXMLDOMNode *pNodeChild = NULL;
	
	if(pNodeParent == NULL || (ppNodeChild == NULL && pbsNodeText == NULL))
		return E_INVALIDARG;
	
	if(ppNodeChild != NULL)
		*ppNodeChild = NULL;
	
	DOMNodeType nt;
	hr = pNodeParent->get_nodeType(&nt);
	hr = pNodeParent->selectNodes(bsSelect, &pNodeList);
	if(pNodeList)
	{
		hr = pNodeList->get_item(0, &pNodeChild);
		if(pNodeChild != NULL)
		{
			if(pbsNodeText != NULL)
			{
				BSTR bs;
				hr = pNodeChild->get_text(&bs);
				*pbsNodeText = bs;
			}
			
			if(ppNodeChild == NULL)
			{
				pNodeChild->Release();
				pNodeChild = NULL;
			}
			else
				*ppNodeChild = pNodeChild;
		}
		pNodeList->Release();
		pNodeList = NULL;
	}
	return hr;
}

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg2 dialog


COFSNcDlg2::COFSNcDlg2(UINT nIDTemplate, CWnd* pParentWnd)
	: OFS_NCDLG2_PARENT(nIDTemplate, pParentWnd)
{
	//{{AFX_DATA_INIT(COFSNcDlg2)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_bResizable = TRUE;
	m_bRound = TRUE;
	m_pBtnMax = NULL;
	m_pBtnRestore = NULL;
	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
	m_bBackgroundPicture = FALSE;
	m_bLoadSkin = TRUE;
	m_rgnTL = m_rgnTR = m_rgnBL = m_rgnBR = NULL;
	m_bIgnoreActivate = FALSE;
}


void COFSNcDlg2::DoDataExchange(CDataExchange* pDX)
{
	OFS_NCDLG2_PARENT::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COFSNcDlg2)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COFSNcDlg2, OFS_NCDLG2_PARENT)
	//{{AFX_MSG_MAP(COFSNcDlg2)
	ON_WM_CREATE()
	ON_WM_ERASEBKGND()
	ON_WM_DESTROY()
	ON_WM_SETCURSOR()
	ON_WM_SIZE()
	ON_WM_LBUTTONDOWN()
	ON_WM_PAINT()
	ON_WM_CTLCOLOR()
	ON_WM_ACTIVATE()
	ON_WM_WINDOWPOSCHANGING()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COFSNcDlg2 message handlers

int COFSNcDlg2::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (OFS_NCDLG2_PARENT::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	
	return 0;
}

BOOL COFSNcDlg2::OnInitDialog() 
{
	OFS_NCDLG2_PARENT::OnInitDialog();
	
	SetBoundary(0, 0);
	if(m_bLoadSkin)
	{
		LoadSkin();
		Invalidate();
	}

	m_ToolTip.Create(this);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void COFSNcDlg2::LoadSkin()
{
	CComPtr<IXMLDOMDocument> pDoc = NULL;
	CComPtr<IXMLDOMNode> pRoot = NULL;
	
	LoadSkinXML(&pDoc, &pRoot);
	if(pRoot)
	{
		CRect r, rMin, rMax;
		r.SetRectEmpty();
		rMin.SetRectEmpty();
		rMax.SetRectEmpty();
		LoadWindow(pRoot, r, rMin, rMax);
//		SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER);
		LoadColors(pRoot);
		LoadPictures(pRoot);
		LoadSkin(pRoot);
		
		SetMinTrackSize(rMin.Size());
		if(!rMax.IsRectEmpty())
			SetMaxTrackSize(rMax.Size());
	}
}

void COFSNcDlg2::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	// Override this function for loading custom skin elements
}

void COFSNcDlg2::LoadSkinXML(IXMLDOMDocument **ppDoc, IXMLDOMNode **ppRoot)
{
	ASSERT(ppDoc != NULL);
	ASSERT(ppRoot != NULL);
	
	HRESULT hr;
	CString strErrorMessage;
	VARIANT_BOOL b;
	LoadSkins skin;
	long nErrorCode = 0;
	IStreamPtr pStream = NULL;
	
	CString str = IBN_SCHEMA;
	str += (LPCTSTR)GetCurrentSkin();
	str += (LPCTSTR)m_strSkinSettings;
	
	hr = CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER, IID_IXMLDOMDocument, (void**)ppDoc);
	
	if(SUCCEEDED(hr) && (*ppDoc) != NULL)
	{
		hr = (*ppDoc)->put_async(VARIANT_FALSE);

		CComVariant	varLoad = (LPCTSTR)str;
		hr = (*ppDoc)->load(varLoad, &b);
		
#ifdef _DEBUG
		//(*ppDoc)->save(_variant_t(L"c:\\tmp.xml"));
		if(b == VARIANT_FALSE) 
		{
			strErrorMessage = "Skin Error: Can't Load XML " + str;
			AfxMessageBox(strErrorMessage);
		}
#endif
		if(b != VARIANT_FALSE)
		{
			hr = (*ppDoc)->selectSingleNode(_bstr_t("WindowSettings"), ppRoot);
			
#ifdef _DEVELOVER_VERSION_L1
			if((*ppRoot) == NULL) 
			{
				strErrorMessage = "Skin XML Error: Can't Find WindowSettings ( " + str + " )";
				AfxMessageBox(strErrorMessage);
			}
#endif
		}
	}
}

BOOL COFSNcDlg2::LoadWindow(IXMLDOMNode *pXmlRoot, CRect &r, CRect &rMin, CRect &rMax)
{
	ASSERT(pXmlRoot != NULL);
	
	CComPtr<IXMLDOMNode> pWindow = NULL;
	CComBSTR bs, bsBgPath;
	LoadSkins skin;
	long nErrorCode = 0;
	IStreamPtr pStream = NULL;
	CPaintDC dc(this);
	WCHAR *szNULL = L"\0x00";
	
	pXmlRoot->selectSingleNode(CComBSTR(L"Window"), &pWindow);
	if(pWindow)
	{
		// Load window size
		bs.Empty();
		SelectChildNode(pWindow, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			r.right = r.left + wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pWindow, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			r.bottom = r.top + wcstol(bs.m_str, &szNULL, 10);

		// Load min size
		LoadRect(pWindow, _T("MinSize"), rMin);
		// Load max size
		LoadRect(pWindow, _T("MaxSize"), rMax);
		
		// Load regions for corners
		DeleteObject(m_rgnTL);
		DeleteObject(m_rgnTR);
		DeleteObject(m_rgnBL);
		DeleteObject(m_rgnBR);
		m_rgnTL = m_rgnTR = m_rgnBL = m_rgnBR = NULL;
		LoadRegion(pWindow, _T("TL"), &m_rgnTL);
		LoadRegion(pWindow, _T("TR"), &m_rgnTR);
		LoadRegion(pWindow, _T("BL"), &m_rgnBL);
		LoadRegion(pWindow, _T("BR"), &m_rgnBR);
		
		CenterRect(r);
		AdjustRect(r);
		// Try Fix Focus behavior [7/23/2002]
		SetWindowPos(NULL, r.left, r.top, r.Width(), r.Height(), SWP_NOZORDER|SWP_NOACTIVATE);

		// Load background image
		m_bBackgroundPicture = (LoadPictures(pWindow) > 0);

		//pWindow->Release();
	}
	
	return TRUE;
}

void COFSNcDlg2::LoadRect(IXMLDOMNode *pParent, LPCTSTR szNodename, CRect &r)
{
	CComPtr<IXMLDOMNode> pNode = NULL;
	_bstr_t bs;
	long val;
	
	r.SetRectEmpty();
	
	pParent->selectSingleNode(_bstr_t(szNodename), &pNode);
	if(pNode)
	{
		if(S_OK == GetNodeAttributeAsLong(pNode, _bstr_t("x"), &val, 10))
			r.left = val;
		if(S_OK == GetNodeAttributeAsLong(pNode, _bstr_t("y"), &val, 10))
			r.top = val;
		if(S_OK == GetNodeAttributeAsLong(pNode, _bstr_t("cx"), &val, 10))
			r.right = r.left + val;
		if(S_OK == GetNodeAttributeAsLong(pNode, _bstr_t("cy"), &val, 10))
			r.bottom = r.top + val;
	}
}

void COFSNcDlg2::AdjustRect(CRect &r)
{
    // Solve multi monitor problem
	return;

	RECT rd;
	SystemParametersInfo(SPI_GETWORKAREA, 0, &rd, 0);
	
	// first check right and bottom edges
	if(r.right > rd.right)
		r.OffsetRect(rd.right - r.right, 0);
	if(r.bottom > rd.bottom)
		r.OffsetRect(0, rd.bottom - r.bottom);
	
	// then check left and top edges
	if(r.left < rd.left)
		r.OffsetRect(rd.left - r.left, 0);
	if(r.top < rd.top)
		r.OffsetRect(0, rd.top - r.top);

/*
	long ScreenCx = GetSystemMetrics(SM_CXFULLSCREEN);
	long ScreenCy = GetSystemMetrics(SM_CYFULLSCREEN);
	
	// first check right and bottom edges
	if(r.right > ScreenCx)
		r.OffsetRect(ScreenCx - r.right, 0);
	if(r.bottom > ScreenCy)
		r.OffsetRect(0, ScreenCy - r.bottom);
	
	// then check left and top edges
	if(r.left < 0)
		r.OffsetRect(-r.left, 0);
	if(r.top < 0)
		r.OffsetRect(0, -r.top);
*/
}

BOOL COFSNcDlg2::LoadButton(IXMLDOMNode *pXmlRoot, LPCTSTR szButtonName, CMcButton *pBtn, BOOL bAutoPressed, BOOL bCanStayPressed, int Type)
{
	//  [7/23/2002]
	BOOL bIsBtnEnable = pBtn->GetEnabled();

	if(bIsBtnEnable)
		pBtn->EnableWindow(FALSE);
	//  [7/23/2002]
	BOOL bResult = FALSE;
	LoadSkins skin;
	long nErrorCode = 0;
	HRESULT hr = S_OK;
	CString strErrorMessage;
	CComPtr<IStream> pStream = NULL;
	CComPtr<IXMLDOMNode> pButton = NULL;
	CComBSTR bs;
	_bstr_t bsImagePath;
	long x=0, y=0;
	WCHAR *szNULL = L"\0x00";
	
	bs.Empty();
	bs = L"Button[@Name='";
	bs += szButtonName;
	bs += L"']";
	pXmlRoot->selectSingleNode(bs, &pButton);
	if(pButton)
	{
		bResult = TRUE;
		// Get button coordinates
		bs.Empty();
		SelectChildNode(pButton, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pButton, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		
		// Load button picture
		bs.Empty();
		SelectChildNode(pButton, CComBSTR(L"Image"), NULL, &bs);
		if(bs.m_str != NULL)
		{
			bsImagePath = bstr_t(IBN_SCHEMA);
			bsImagePath += (LPCTSTR)GetCurrentSkin();
			bsImagePath += bs.m_str;
			skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
			if(pStream == NULL || nErrorCode) 
			{
				strErrorMessage = "Skin Error: Can't Load Image ( ";
				strErrorMessage += (char*)bsImagePath;
				strErrorMessage +=" )";
				AfxMessageBox(strErrorMessage);
			}
#endif
			if(pStream)
			{
				pBtn->LoadBitmapFromStream(pStream);

				UINT nFlags = SWP_NOZORDER|SWP_NOSIZE;

				// 1=Maximize, 2=Restore
				if(Type == 0)
				{
					nFlags |= SWP_SHOWWINDOW;
				}
				if(Type == 1)
				{
					m_pBtnMax = pBtn;
					nFlags |= SWP_SHOWWINDOW;
				}
				if(Type == 2)
				{
					m_pBtnRestore = pBtn;
				}
				pBtn->SetWindowPos(NULL, x, y, 0, 0, nFlags);
				pBtn->SetAutoPressed(bAutoPressed);
				pBtn->SetCanStayPressed(bCanStayPressed);
			}
		}
		
		// Load settings for resize
		//LoadResizeSettings(pButton, pBtn->m_hWnd);
		CComPtr<IXMLDOMNode> pResize = NULL;
		
		pButton->selectSingleNode(CComBSTR(L"Resize"), &pResize);
		if(pResize)
		{
			long tlcx, tlcy, brcx, brcy;
			
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCX"), &tlcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCY"), &tlcy, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCX"), &brcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCY"), &brcy, 10);
			
			if(tlcx || tlcy || brcx || brcy)
				AddAnchor((CWnd*)pBtn, CSize(tlcx, tlcy), CSize(brcx, brcy));
			
			//pResize->Release();
		}
		
		
		//pButton->Release();
	}

	if(bIsBtnEnable)
		pBtn->EnableWindow();
	
	return bResult;
}

BOOL COFSNcDlg2::LoadResizeSettings(IXMLDOMNode *pObject, CSize &szTL, CSize &szBR)
{
	ASSERT(pObject != NULL);
	
	CComPtr<IXMLDOMNode> pResize = NULL;
	long tlcx=0, tlcy=0, brcx=0, brcy=0;
	
	pObject->selectSingleNode(CComBSTR(L"Resize"), &pResize);
	if(pResize)
	{
		
		GetNodeAttributeAsLong(pResize, _bstr_t("TLCX"), &tlcx, 10);
		GetNodeAttributeAsLong(pResize, _bstr_t("TLCY"), &tlcy, 10);
		GetNodeAttributeAsLong(pResize, _bstr_t("BRCX"), &brcx, 10);
		GetNodeAttributeAsLong(pResize, _bstr_t("BRCY"), &brcy, 10);
		
		szTL.cx = tlcx;
		szTL.cy = tlcy;
		szBR.cx = brcx;
		szBR.cy = brcy;
		
		//pResize->Release();
	}
	return (tlcx || tlcy || brcx || brcy);
}

void COFSNcDlg2::LoadRectangle(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, CWnd *pWnd, BOOL bVisible, BOOL bDontHide)
{
	ASSERT(pXmlRoot != NULL);
	ASSERT(pWnd != NULL);

	CComPtr<IXMLDOMNode> pRectangle = NULL;
	CRect r;
	CComBSTR bs;
	WCHAR *szNULL = L"\0x00";
	long x=0, y=0, cx=0, cy=0;
	
	if(!bDontHide)
		pWnd->ShowWindow(SW_HIDE);
	r.SetRectEmpty();

	bs.Empty();
	bs = L"Rectangle[@Name=\"";
	bs += szRectangleName;
	bs += L"\"]";
	
	pXmlRoot->selectSingleNode(bs, &pRectangle);
	if(pRectangle)
	{
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		UINT nFlags = SWP_NOZORDER;
		if(bVisible)
			nFlags |= SWP_SHOWWINDOW;
		pWnd->SetWindowPos(NULL, x, y, cx, cy, nFlags);
		CSize szTL, szBR;
		LoadResizeSettings(pRectangle, szTL, szBR);
		AddAnchor(pWnd, szTL, szBR);
		
		//pRectangle->Release();
	}
}

BOOL COFSNcDlg2::OnEraseBkgnd(CDC* pDC)
{
	if(m_bBackgroundPicture)
	{
		return TRUE;
	}
	else
		return OFS_NCDLG2_PARENT::OnEraseBkgnd(pDC);
}

void COFSNcDlg2::DrawBackground(CDC *pDC)
{
	HDC hDC = pDC->GetSafeHdc();

	RECT r;
	CPictureObj *pic = NULL;
	int n = m_aPictures.GetSize();
	while(n > 0)
	{
		n--;
		pic = m_aPictures.GetAt(n);
		if(pic)
		{
			pic->Render(hDC);
			pic->GetRect(&r);
			ExcludeClipRect(hDC, r.left, r.top, r.right, r.bottom);
		}
	}
}

void COFSNcDlg2::OnDestroy() 
{
	OFS_NCDLG2_PARENT::OnDestroy();

	CPictureObj *pic = NULL;
	while(m_aPictures.GetSize())
	{
		pic = m_aPictures[0];
		m_aPictures.RemoveAt(0);
		delete pic;
	}
	DeleteObject(m_rgnTL);
	DeleteObject(m_rgnTR);
	DeleteObject(m_rgnBL);
	DeleteObject(m_rgnBR);
	m_rgnTL = m_rgnTR = m_rgnBL = m_rgnBR = NULL;
}

BOOL COFSNcDlg2::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CRect StatusRect, miniRect;
	
	GetClientRect(&StatusRect);
				
	CPoint point, inPoint;
	
	::GetCursorPos(&point);
	inPoint = point;
	ScreenToClient(&inPoint);
	
	miniRect = StatusRect;miniRect.DeflateRect(11,11,11,11);
	
	if(m_bResizable && !miniRect.PtInRect(inPoint)&&StatusRect.PtInRect(inPoint))
	{
		if(inPoint.x<miniRect.left)
			if(inPoint.y<miniRect.top)
				nHitTest = HTTOPLEFT;
			else if(inPoint.y>miniRect.bottom)
				nHitTest = HTBOTTOMLEFT;
			else
				nHitTest = HTLEFT;
			else if(inPoint.x>miniRect.right)
				if(inPoint.y<miniRect.top)
					nHitTest = HTTOPRIGHT;
				else if(inPoint.y>miniRect.bottom)
					nHitTest = HTBOTTOMRIGHT;
				else
					nHitTest = HTRIGHT;
				else if(inPoint.y<miniRect.top)
					nHitTest = HTTOP;
				else
					nHitTest = HTBOTTOM;
	}
	
	return OFS_NCDLG2_PARENT::OnSetCursor(pWnd, nHitTest, message);
}

void COFSNcDlg2::OnSize(UINT nType, int cx, int cy) 
{
	OFS_NCDLG2_PARENT::OnSize(nType, cx, cy);
	
//	if(m_bRound)
	{
		RECT rw, r;
		GetWindowRect(&rw);
		OffsetRect(&rw, -rw.left, -rw.top);
		
		HRGN WinRgn;
		WinRgn = CreateRectRgn(0, 0, rw.right, rw.bottom);

		if(m_rgnTL != NULL)
			CombineRgn(WinRgn, WinRgn, m_rgnTL, RGN_DIFF);
		if(m_rgnTR != NULL)
		{
			GetRgnBox(m_rgnTR, &r);
			OffsetRgn(m_rgnTR, rw.right - r.right, 0);
			CombineRgn(WinRgn, WinRgn, m_rgnTR, RGN_DIFF);
			OffsetRgn(m_rgnTR, -(rw.right - r.right), 0);
		}
		if(m_rgnBL != NULL)
		{
			GetRgnBox(m_rgnBL, &r);
			OffsetRgn(m_rgnBL, 0, rw.bottom - r.bottom);
			CombineRgn(WinRgn, WinRgn, m_rgnBL, RGN_DIFF);
			OffsetRgn(m_rgnBL, 0, -(rw.bottom - r.bottom));
		}
		if(m_rgnBR != NULL)
		{
			GetRgnBox(m_rgnBR, &r);
			OffsetRgn(m_rgnBR, rw.right - r.right, rw.bottom - r.bottom);
			CombineRgn(WinRgn, WinRgn, m_rgnBR, RGN_DIFF);
			OffsetRgn(m_rgnBR, -(rw.right - r.right), -(rw.bottom - r.bottom));
		}
		SetWindowRgn(WinRgn, TRUE);
		
		DeleteObject(WinRgn);
	}

	// Switch maximize and restore buttons
	if(nType == SIZE_MAXIMIZED)
	{
		if(m_pBtnMax)
			m_pBtnMax->ShowWindow(FALSE);
		if(m_pBtnRestore)
			m_pBtnRestore->ShowWindow(TRUE);
	}
	if(nType == SIZE_RESTORED)
	{
		if(m_pBtnMax)
			m_pBtnMax->ShowWindow(TRUE);
		if(m_pBtnRestore)
			m_pBtnRestore->ShowWindow(FALSE);
	}
	
	Invalidate(FALSE);
	UpdateWindow();
}

void COFSNcDlg2::OnLButtonDown(UINT nFlags, CPoint point) 
{
	CPoint inPoint	=	point;
	ClientToScreen(&point);
	
	CRect StatusRect, miniRect;
	GetClientRect(&StatusRect);
	
	miniRect = StatusRect;miniRect.DeflateRect(11,11,11,11);
	
	if(m_bResizable && !miniRect.PtInRect(inPoint))
	{
		if(inPoint.x<miniRect.left)
			if(inPoint.y<miniRect.top)
				OFS_NCDLG2_PARENT::OnNcLButtonDown(HTTOPLEFT,point);
			else if(inPoint.y>miniRect.bottom)
				OFS_NCDLG2_PARENT::OnNcLButtonDown(HTBOTTOMLEFT,point);
			else
				OFS_NCDLG2_PARENT::OnNcLButtonDown(HTLEFT,point);
			else if(inPoint.x>miniRect.right)
				if(inPoint.y<miniRect.top)
					OFS_NCDLG2_PARENT::OnNcLButtonDown(HTTOPRIGHT,point);
				else if(inPoint.y>miniRect.bottom)
					OFS_NCDLG2_PARENT::OnNcLButtonDown(HTBOTTOMRIGHT,point);
				else
					OFS_NCDLG2_PARENT::OnNcLButtonDown(HTRIGHT,point);
				else if(inPoint.y<miniRect.top)
					OFS_NCDLG2_PARENT::OnNcLButtonDown(HTTOP,point);
				else
					OFS_NCDLG2_PARENT::OnNcLButtonDown(HTBOTTOM,point);
	}
	else
		OFS_NCDLG2_PARENT::OnNcLButtonDown(HTCAPTION,point);
}

void COFSNcDlg2::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	
	DrawBackground(&dc);
	
	// Do not call OFS_NCDLG2_PARENT::OnPaint() for painting messages
}

BOOL COFSNcDlg2::GetCtlColor(CDC *pDC, CWnd *pWnd, UINT nCtlColor, HBRUSH *pBrush)
{
	BOOL bResult = FALSE;
	CTL_COLOR crNone, *pColor = &crNone;

#ifdef _DEBUG
	CString str;
	if(pWnd)
		pWnd->GetWindowText(str);
#endif

	UINT nControl = nCtlColor;

	TCHAR szBuf[100];
	szBuf[0] = _T('\0');
//	UINT n;
	if(pWnd)
	{
//		n = RealGetWindowClass(pWnd->GetSafeHwnd(), szBuf, 100);
	}
	
	if(0 == _tcsicmp(szBuf, _T("#32770")))
		nControl = CTLCOLOR_DLG;
	if(0 == _tcsicmp(szBuf, _T("Button")))
		nControl = CTLCOLOR_BTN;
	if(0 == _tcsicmp(szBuf, _T("Edit")))
		nControl = CTLCOLOR_EDIT;
	if(0 == _tcsicmp(szBuf, _T("Static")))
		nControl = CTLCOLOR_STATIC;

	switch(nControl)
	{
	case CTLCOLOR_BTN:
		pColor = &m_crButton;
		break;
	case CTLCOLOR_DLG:
		pColor = &m_crDialog;
		break;
	case CTLCOLOR_EDIT:
		pColor = &m_crEdit;
		break;
	case CTLCOLOR_LISTBOX:
		pColor = &m_crList;
		break;
	case CTLCOLOR_SCROLLBAR:
		pColor = &m_crScroll;
		break;
	case CTLCOLOR_STATIC:
		pColor = &m_crStatic;
		break;
	}
	
	if(pColor->IsValidColor())
	{
		BOOL bDisabled = FALSE;
		if(pWnd)
		{
			if(WS_DISABLED & pWnd->GetStyle())
				bDisabled = TRUE;
		}
		
		m_brush.DeleteObject();
		if(bDisabled)
		{
			pDC->SetBkColor(pColor->crBGD);
			pDC->SetTextColor(pColor->crTextD);
			m_brush.CreateSolidBrush(pColor->crBGD);
		}
		else
		{
			pDC->SetBkColor(pColor->crBG);
			pDC->SetTextColor(pColor->crText);
			m_brush.CreateSolidBrush(pColor->crBG);
		}
		*pBrush = m_brush;
		
		bResult = TRUE;
	}
	
	return bResult;
}

void COFSNcDlg2::LoadColors(IXMLDOMNode *pXmlRoot)
{
	CComPtr<IXMLDOMNode> pColors = NULL;
	
	m_crButton.Clear();
	m_crDialog.Clear();
	m_crEdit.Clear();
	m_crList.Clear();
	m_crScroll.Clear();
	m_crStatic.Clear();
//	if(m_pBrush)
//	{
//		m_pBrush->DeleteObject();
//		delete m_pBrush;
//		m_pBrush = NULL;
//	}
	
	if(pXmlRoot)
	{
		pXmlRoot->selectSingleNode(CComBSTR(L"Colors"), &pColors);
		if(pColors)
		{
			LoadColor(pColors, _T("Button"), m_crButton);
			LoadColor(pColors, _T("Dialog"), m_crDialog);
			LoadColor(pColors, _T("Edit"), m_crEdit);
			LoadColor(pColors, _T("List"), m_crList);
			LoadColor(pColors, _T("Scroll"), m_crScroll);
			LoadColor(pColors, _T("Static"), m_crStatic);
			//pColors->Release();
		}
	}
}

void COFSNcDlg2::LoadColor(IXMLDOMNode *pColors, LPCTSTR szNodeName, CTL_COLOR &cr)
{
	CComBSTR bs;
	CComPtr<IXMLDOMNode> pColor = NULL;
	
	if(pColors)
	{
		pColors->selectSingleNode(CComBSTR(szNodeName), &pColor);
		if(pColor)
		{
			bs.Empty();
			GetNodeAttribute(pColor, CComBSTR(L"bg"), bs);
			if(bs.m_str != NULL)
				swscanf(bs.m_str, L"0x%06x", &cr.crBG);
			bs.Empty();
			GetNodeAttribute(pColor, CComBSTR(L"fg"), bs);
			if(bs.m_str != NULL)
				swscanf(bs.m_str, L"0x%06x", &cr.crText);
			bs.Empty();
			GetNodeAttribute(pColor, CComBSTR(L"bgd"), bs);
			if(bs.m_str != NULL)
				swscanf(bs.m_str, L"0x%06x", &cr.crBGD);
			bs.Empty();
			GetNodeAttribute(pColor, CComBSTR(L"fgd"), bs);
			if(bs.m_str != NULL)
				swscanf(bs.m_str, L"0x%06x", &cr.crTextD);
		}
	}
}

HBRUSH COFSNcDlg2::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	
	GetCtlColor(pDC, pWnd, nCtlColor, &hbr);
	
	return hbr;
}

void COFSNcDlg2::OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized) 
{
	OFS_NCDLG2_PARENT::OnActivate(nState, pWndOther, bMinimized);

/*
	if(nState != WA_INACTIVE)
	{
		CRect r;
		GetWindowRect(&r);
		AdjustRect(r);
		SetWindowPos(NULL, r.left, r.top, 0, 0, SWP_NOZORDER|SWP_NOSIZE);
	}
*/
	
//	if(m_bIgnoreActivate)
		return;

	if(nState != WA_INACTIVE)
	{
		if(GetOptionInt(IDS_OFSMESSENGER, IDS_KEEPTOP, FALSE))
			SetWindowPos(&wndTopMost, 0, 0, 0, 0, SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	}
	else
	{
		SetWindowPos(&wndNoTopMost, 0, 0, 0, 0, SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	}
}

void COFSNcDlg2::CenterRect(CRect &r)
{
	HWND hParent = ::GetWindow(m_hWnd, GW_OWNER);

	//HWND hParent = ::GetParent(m_hWnd);
	if(hParent == NULL || hParent == ::GetDesktopWindow() || !(GetStyle()&WS_VISIBLE))
	{
		long ScreenCx = GetSystemMetrics(SM_CXFULLSCREEN);
		long ScreenCy = GetSystemMetrics(SM_CYFULLSCREEN);
		r.OffsetRect((ScreenCx - r.Width())/2, (ScreenCy - r.Height())/2);
	}
	else
	{
		CWnd *pParent = CWnd::FromHandle(hParent);
		if(pParent)
		{
			CRect rp;
			pParent->GetWindowRect(&rp);
			//r.OffsetRect(rp.left - r.left, rp.top - r.top);
			r.OffsetRect(rp.left+ (rp.Width() - r.Width())/2, rp.top+(rp.Height() - r.Height())/2);
		}
	}
}

void COFSNcDlg2::LoadPicture(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CPictureObj **ppPictureObj)
{
	CComPtr<IXMLDOMNode> pPicture = NULL;
	CComBSTR bs;
	
	bs = L"Picture[@Name=\"";
	bs += szName;
	bs += L"\"]";
	pXmlRoot->selectSingleNode(bs, &pPicture);
	if(pPicture)
	{
		LoadPicture(pPicture, ppPictureObj);
		//pPicture->Release();
	}
}

long COFSNcDlg2::LoadPictures(IXMLDOMNode *pXmlRoot)
{
	long nPictures = 0;
	CComPtr<IXMLDOMNodeList> pList = NULL;
	CComPtr<IXMLDOMNode> pPicture = NULL;

	pXmlRoot->selectNodes(CComBSTR(L"Picture"), &pList);
	if(pList)
	{
		pList->get_length(&nPictures);
		pList->nextNode(&pPicture);
		while(pPicture)
		{
			LoadPicture(pPicture);
			pPicture = NULL;
			pList->nextNode(&pPicture);
		}
		//pList->Release();
	}
	return nPictures;
}

void COFSNcDlg2::LoadPicture(IXMLDOMNode *pPicture, CPictureObj **ppPictureObj)
{
	ASSERT(pPicture != NULL);

	LoadSkins skin;
	long nErrorCode = 0;
	IStreamPtr pStream = NULL;
	CComBSTR bs, bsPath;
	WCHAR *szNULL = L"\0x00";
	long x=0, y=0, cx=0, cy=0;
	CComPtr<IXMLDOMNodeList> pList = NULL;
	CComPtr<IXMLDOMNode> pFragment = NULL;
	
	// Load image
	bs.Empty();
	SelectChildNode(pPicture, CComBSTR(L"Image"), NULL, &bs);
	bsPath = (BSTR)bstr_t(IBN_SCHEMA);
	bsPath += (LPCTSTR)GetCurrentSkin();
	bsPath += bs;
	skin.Load(bsPath, &pStream, &nErrorCode);
	if(pStream)
	{
		// Load coords
		bs.Empty();
		SelectChildNode(pPicture, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pPicture, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pPicture, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pPicture, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		CSize szTL, szBR;
		LoadResizeSettings(pPicture, szTL, szBR);
		
		CPictureObj* pic = new CPictureObj;
		if(pic)
		{
			if(pic->Create(pStream, x, y, cx, cy))
			{
				// Load picture fragments
				long nFragments = 0;
				pPicture->selectNodes(CComBSTR(L"Fragment"), &pList);
				if(pList)
				{
					pList->get_length(&nFragments);
					pList->nextNode(&pFragment);
					while(pFragment)
					{
						LoadPictureFragment(pFragment, pic);
						pFragment = NULL;
						pList->nextNode(&pFragment);
					}
					//pList->Release();
				}
				if(nFragments <= 0)
				{
					pic->AddWholeImage(CResizableImage::STRETCH);
				}

				if(ppPictureObj)
				{
					*ppPictureObj = pic;
				}
				else
				{
					m_aPictures.Add(pic);
					AddAnchor(pic, szTL, szBR);
				}
			}
			else
				delete pic;
		}
	}
}

BOOL COFSNcDlg2::LoadRectangle(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, CRect &r)
{
	ASSERT(pXmlRoot != NULL);
	ASSERT(szRectangleName != NULL);
	
	BOOL bResult = FALSE;
	CComPtr<IXMLDOMNode> pRectangle = NULL;
	CComBSTR bs;
	WCHAR *szNULL = L"\0x00";
	long x=0, y=0, cx=0, cy=0;
	
	bs.Empty();
	bs = L"Rectangle[@Name=\"";
	bs += szRectangleName;
	bs += L"\"]";
	
	pXmlRoot->selectSingleNode(bs, &pRectangle);
	if(pRectangle)
	{
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		//pRectangle->Release();
		r = CRect(CPoint(x, y), CSize(cx, cy));
		bResult = TRUE;
	}
	return bResult;
}

void COFSNcDlg2::LoadPictureFragment(IXMLDOMNode *pFragment, CPictureObj *pPictureObj)
{
	ASSERT(pFragment != NULL);
	ASSERT(pPictureObj != NULL);
	
	long x=0, y=0, cx=0, cy=0, tlcx=0, tlcy=0, brcx=0, brcy=0, type=0;
	long val;
	
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("x"), &val, 10))
		x = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("y"), &val, 10))
		y = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("cx"), &val, 10))
		cx = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("cy"), &val, 10))
		cy = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("tlcx"), &val, 10))
		tlcx = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("tlcy"), &val, 10))
		tlcy = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("brcx"), &val, 10))
		brcx = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("brcy"), &val, 10))
		brcy = val;
	if(S_OK == GetNodeAttributeAsLong(pFragment, _bstr_t("type"), &val, 10))
		type = val;
	
	pPictureObj->AddFragment(CRect(CPoint(x, y), CSize(cx, cy)), CSize(tlcx, tlcy), CSize(brcx, brcy), type);
}

void COFSNcDlg2::LoadRectangle2(IXMLDOMNode *pXmlRoot, LPCTSTR szRectangleName, HWND hWnd, BOOL bVisible, BOOL bDontHide)
{
	ASSERT(pXmlRoot != NULL);
	ASSERT(::IsWindow(hWnd));
	
	CComPtr<IXMLDOMNode> pRectangle = NULL;
	CRect r;
	CComBSTR bs;
	WCHAR *szNULL = L"\0x00";
	long x=0, y=0, cx=0, cy=0;
	
	if(!bDontHide)
		::ShowWindow(hWnd, SW_HIDE);
	r.SetRectEmpty();
	
	bs.Empty();
	bs = L"Rectangle[@Name=\"";
	bs += szRectangleName;
	bs += L"\"]";
	
	pXmlRoot->selectSingleNode(bs, &pRectangle);
	if(pRectangle)
	{
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pRectangle, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		UINT nFlags = SWP_NOZORDER;
		if(bVisible)
			nFlags |= SWP_SHOWWINDOW;
		::SetWindowPos(hWnd, HWND_TOP, x, y, cx, cy, nFlags);
		CSize szTL, szBR;
		LoadResizeSettings(pRectangle, szTL, szBR);
		AddAnchor(hWnd, szTL, szBR);
		
		//pRectangle->Release();
	}
}

void COFSNcDlg2::LoadRegion(IXMLDOMNode *pXmlRoot, LPCTSTR szName, HRGN *phrgn)
{
	ASSERT(pXmlRoot != NULL);
	ASSERT(szName != NULL);
	
	CComBSTR bs, bsRegion;
	WCHAR *szNULL = L"\0x00";
	long x=0, y=0, cx=0, cy=0;
	
	bs.Empty();
	bs = L"Region[@Name=\"";
	bs += szName;
	bs += L"\"]";
	
	SelectChildNode(pXmlRoot, bs, NULL, &bsRegion);
	if(bsRegion.Length() <= 0)
		return;
	
	WCHAR *pCur = bsRegion.m_str, *pEnd = pCur + wcslen(pCur);
	int x1, x2, y1, y2;
	
	// For better performances, we will use the ExtCreateRegion() function to create the
	// region. This function take a RGNDATA structure on entry. We will add rectangles by
	// amount of ALLOC_UNIT number in this structure.
#define ALLOC_UNIT	100
	DWORD maxRects = ALLOC_UNIT;
	HANDLE hData = GlobalAlloc(GMEM_MOVEABLE, sizeof(RGNDATAHEADER) + (sizeof(RECT) * maxRects));
	RGNDATA *pData = (RGNDATA *)GlobalLock(hData);
	pData->rdh.dwSize = sizeof(RGNDATAHEADER);
	pData->rdh.iType = RDH_RECTANGLES;
	pData->rdh.nCount = pData->rdh.nRgnSize = 0;
	SetRect(&pData->rdh.rcBound, MAXLONG, MAXLONG, 0, 0);
	
	int nScannedElements;
	while(pCur < pEnd)
	{
		nScannedElements = swscanf(pCur, L"%d,%d,%d,%d", &x1, &y1, &x2, &y2);
		if(nScannedElements == 4)
		{
			// Add the rectangle (x1, y1) - (x2, y2) in the region
			if(pData->rdh.nCount >= maxRects)
			{
				GlobalUnlock(hData);
				maxRects += ALLOC_UNIT;
				hData = GlobalReAlloc(hData, sizeof(RGNDATAHEADER) + (sizeof(RECT) * maxRects), GMEM_MOVEABLE);
				pData = (RGNDATA *)GlobalLock(hData);
			}
			
			RECT *pr = (RECT *)&pData->Buffer;
			SetRect(&pr[pData->rdh.nCount], x1, y1, x2, y2);
			if(x1 < pData->rdh.rcBound.left)
				pData->rdh.rcBound.left = x1;
			if(y1 < pData->rdh.rcBound.top)
				pData->rdh.rcBound.top = y1;
			if(x2 > pData->rdh.rcBound.right)
				pData->rdh.rcBound.right = x2;
			if(y2 > pData->rdh.rcBound.bottom)
				pData->rdh.rcBound.bottom = y2;
			pData->rdh.nCount++;
		}
		pCur = wcschr(pCur, L' ');
		if(pCur == NULL)
			break;
		pCur++;
	}

	// Create the region
	HRGN h = ExtCreateRegion(NULL, sizeof(RGNDATAHEADER) + (sizeof(RECT) * maxRects), pData);
	*phrgn = h;
	
	GlobalUnlock(hData);
	// Clean up
	GlobalFree(hData);
}

void COFSNcDlg2::LoadColor(IXMLDOMNode *pRoot, LPCTSTR szName, COLORREF &cr)
{
	CComBSTR bs, bsSelect;

	cr = CLR_NONE;

	bs.Empty();
	bsSelect = L"Color[@Name='";
	bsSelect += szName;
	bsSelect += L"']";
	SelectChildNode(pRoot, bsSelect, NULL, &bs);
	if(bs.m_str != NULL)
	{
		long ncr;
		int n = swscanf(bs.m_str, L"0x%06x", &ncr);
		if(n == 1)
			cr = ncr;
	}
}

BOOL COFSNcDlg2::LoadProgress(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CMcProgress *pMcProgress, BOOL bVisible)
{
	BOOL bResult = FALSE;
	LoadSkins skin;
	long nErrorCode = 0;
	HRESULT hr = S_OK;
	CString strErrorMessage;
	IStreamPtr pStream1 = NULL, pStream2 = NULL;
	CComPtr<IXMLDOMNode> pProgress = NULL;
	CComBSTR bs;
	_bstr_t bsImagePath;
	long x=0, y=0, cx=0, cy=0;
	WCHAR *szNULL = L"\0x00";
	
	pMcProgress->ShowWindow(SW_HIDE);

	bs.Empty();
	bs = L"Progress[@Name='";
	bs += szName;
	bs += L"']";
	pXmlRoot->selectSingleNode(bs, &pProgress);
	if(pProgress)
	{
		bResult = TRUE;
		// Get button coordinates
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		// Load button picture
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"Image[@Name='Empty']"), NULL, &bs);
		if(bs.m_str != NULL)
		{
			bsImagePath = bstr_t(IBN_SCHEMA);
			bsImagePath += (LPCTSTR)GetCurrentSkin();
			bsImagePath += bs.m_str;
			skin.Load(bsImagePath, &pStream1, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
			if(pStream1 == NULL || nErrorCode) 
			{
				strErrorMessage = "Skin Error: Can't Load Image ( ";
				strErrorMessage += (char*)bsImagePath;
				strErrorMessage +=" )";
				AfxMessageBox(strErrorMessage);
			}
#endif
		}
		bs.Empty();
		SelectChildNode(pProgress, CComBSTR(L"Image[@Name='Full']"), NULL, &bs);
		if(bs.m_str != NULL)
		{
			bsImagePath = bstr_t(IBN_SCHEMA);
			bsImagePath += (LPCTSTR)GetCurrentSkin();
			bsImagePath += bs.m_str;
			skin.Load(bsImagePath, &pStream2, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
			if(pStream2 == NULL || nErrorCode) 
			{
				strErrorMessage = "Skin Error: Can't Load Image ( ";
				strErrorMessage += (char*)bsImagePath;
				strErrorMessage +=" )";
				AfxMessageBox(strErrorMessage);
			}
#endif
		}
		
		CRect rFull;
		rFull.SetRectEmpty();
		LoadRect(pProgress, _T("RectFull"), rFull);

		if(pStream1 != NULL && pStream2 != NULL && !rFull.IsRectEmpty())
			pMcProgress->LoadBitmapsFromStream((LPUNKNOWN)pStream1, (LPUNKNOWN)pStream2, rFull.left, rFull.top, rFull.Width(), rFull.Height());
			
		UINT nFlags = SWP_NOZORDER;
			
		if(bVisible)
			nFlags |= SWP_SHOWWINDOW;

		pMcProgress->SetWindowPos(NULL, x, y, cx, cy, nFlags);
		
		// Load settings for resize
		//LoadResizeSettings(pButton, pBtn->m_hWnd);
		CComPtr<IXMLDOMNode> pResize = NULL;
		
		pProgress->selectSingleNode(CComBSTR(L"Resize"), &pResize);
		if(pResize)
		{
			long tlcx, tlcy, brcx, brcy;
			
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCX"), &tlcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCY"), &tlcy, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCX"), &brcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCY"), &brcy, 10);
			
			if(tlcx || tlcy || brcx || brcy)
				AddAnchor((CWnd*)pMcProgress, CSize(tlcx, tlcy), CSize(brcx, brcy));
			
			//pResize->Release();
		}
		
		
		//pProgress->Release();
	}
	return bResult;
}

BOOL COFSNcDlg2::LoadLabel(IXMLDOMNode *pXmlRoot, LPCTSTR szName, CLabel *pLbl, BOOL bVisible)
{
	BOOL bResult = FALSE;
	LoadSkins skin;
	long nErrorCode = 0;
	HRESULT hr = S_OK;
	CString strErrorMessage;
	IStreamPtr pStream = NULL;
	CComPtr<IXMLDOMNode> pLabel = NULL, pFont = NULL;
	CComBSTR bs;
	_bstr_t bsImagePath;
	long x=0, y=0, cx=0, cy=0;
	WCHAR *szNULL = L"\0x00";
	
	pLbl->ShowWindow(SW_HIDE);

	bs.Empty();
	bs = L"Label[@Name='";
	bs += szName;
	bs += L"']";
	pXmlRoot->selectSingleNode(bs, &pLabel);
	if(pLabel)
	{
		bResult = TRUE;
		// Get button coordinates
		bs.Empty();
		SelectChildNode(pLabel, CComBSTR(L"XPos"), NULL, &bs);
		if(bs.m_str != NULL)
			x = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pLabel, CComBSTR(L"YPos"), NULL, &bs);
		if(bs.m_str != NULL)
			y = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pLabel, CComBSTR(L"XLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cx = wcstol(bs.m_str, &szNULL, 10);
		bs.Empty();
		SelectChildNode(pLabel, CComBSTR(L"YLen"), NULL, &bs);
		if(bs.m_str != NULL)
			cy = wcstol(bs.m_str, &szNULL, 10);
		
		COLORREF crText, crBG;
		LoadColor(pLabel, _T("Text"), crText);
		if(crText != CLR_NONE)
			pLbl->SetTextColor(crText);
		LoadColor(pLabel, _T("BG"), crBG);
		if(crBG != CLR_NONE)
		{
			pLbl->SetTransparent(FALSE);
			pLbl->SetBkColor(crBG);
		}
		else
			pLbl->SetTransparent(TRUE);

		// Load font
		pLabel->selectSingleNode(CComBSTR(L"Font"), &pFont);
		if(pFont)
		{
			bs.Empty();
			GetNodeAttribute(pFont, CComBSTR(L"Face"), bs);
			if(bs.m_str != NULL && bs.Length())
				pLbl->SetFontName(CString(bs));

			long n = 0;
			GetNodeAttributeAsLong(pFont, CComBSTR(L"Size"), &n, 10);
			if(n != 0)
				pLbl->SetFontSize(n);

			n = 0;
			GetNodeAttributeAsLong(pFont, CComBSTR(L"Bold"), &n, 10);
				pLbl->SetFontBold(n != 0);
				
			n = 0;
			GetNodeAttributeAsLong(pFont, CComBSTR(L"Italic"), &n, 10);
			pLbl->SetFontItalic(n != 0);
				
			n = 0;
			GetNodeAttributeAsLong(pFont, CComBSTR(L"Underline"), &n, 10);
			pLbl->SetFontUnderline(n != 0);
				
			//pFont->Release();
		}

		
		UINT nFlags = SWP_NOZORDER;
		
		if(bVisible)
			nFlags |= SWP_SHOWWINDOW;
		
		pLbl->SetWindowPos(NULL, x, y, cx, cy, nFlags);
		
		// Load settings for resize
		IXMLDOMNode *pResize = NULL;
		
		pLabel->selectSingleNode(CComBSTR(L"Resize"), &pResize);
		if(pResize)
		{
			long tlcx, tlcy, brcx, brcy;
			
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCX"), &tlcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("TLCY"), &tlcy, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCX"), &brcx, 10);
			GetNodeAttributeAsLong(pResize, _bstr_t("BRCY"), &brcy, 10);
			
			if(tlcx || tlcy || brcx || brcy)
				AddAnchor(pLbl->GetSafeHwnd(), CSize(tlcx, tlcy), CSize(brcx, brcy));
			
			//pResize->Release();
		}
		
		
		//pLabel->Release();
	}
	return bResult;
}

void COFSNcDlg2::OnWindowPosChanging(WINDOWPOS FAR* lpwndpos) 
{
	OFS_NCDLG2_PARENT::OnWindowPosChanging(lpwndpos);
	// TODO: Add your message handler code here

/*
	if(!(lpwndpos->flags & SWP_HIDEWINDOW))
	{
		TRACE("begin %d ", m_hWnd);
		
		CRect r, rw(CPoint(lpwndpos->x, lpwndpos->y), CSize(lpwndpos->cx, lpwndpos->cy));
		GetWindowRect(&r);
		TRACE("%d,%d,%d,%d ", r.left, r.top, r.right, r.bottom);
		
		if(lpwndpos->flags & SWP_NOMOVE)
		{
			TRACE("nomove ");
			rw.OffsetRect(r.left - rw.left, r.top - rw.top);
		}
		if(lpwndpos->flags & SWP_NOSIZE)
		{
			TRACE("nosize ");
			rw.right = rw.left + r.Width();
			rw.bottom = rw.top + r.Height();
		}
		TRACE("other ");
		//	if(r.IsRectEmpty())
		//		return;
		
		//	rw.CopyRect(&r);
		AdjustRect(rw);
		//	if(rw.EqualRect(&r))
		//		return;
		lpwndpos->x = rw.left;
		lpwndpos->y = rw.top;
		TRACE("%d,%d,%d,%d ", rw.left-r.left, rw.top-r.top, rw.right-r.right, rw.bottom-r.bottom);
		TRACE("end\r\n");
	}
*/

}

BOOL COFSNcDlg2::PreTranslateMessage(LPMSG pMsg)
{
	if(IsWindow(m_ToolTip.GetSafeHwnd()))
		m_ToolTip.RelayEvent(pMsg);
	return OFS_NCDLG2_PARENT::PreTranslateMessage(pMsg);
}