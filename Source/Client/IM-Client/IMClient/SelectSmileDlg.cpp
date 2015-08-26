// SelectSmileDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "SelectSmileDlg.h"
#include "SmileManager.h"
#include "McSettings.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


extern  CSmileManager CurrentSmileManager;
/////////////////////////////////////////////////////////////////////////////
// CSelectSmileDlg dialog


CSelectSmileDlg::CSelectSmileDlg(CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CSelectSmileDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSelectSmileDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

	SetBoundary(0,0);
	SetCaption(RGB(0,0,0),RGB(0,0,0),0);
	m_strSkinSettings = _T("/Shell/Smiles/skin.xml");

	m_SelectedSmileIndex = -1;
}


void CSelectSmileDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSelectSmileDlg)
	DDX_Control(pDX, IDC_MCCLOSE, m_btnX);
	DDX_Control(pDX, IDC_MCOK, m_btnOK);
	DDX_Control(pDX, IDC_SMILE_LIST, m_SmileList);
	DDX_Control(pDX, IDC_SMILEPREVIEW, m_SmilePreview);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CSelectSmileDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CSelectSmileDlg)
	ON_WM_CLOSE()
	ON_WM_CAPTURECHANGED()
	ON_WM_DESTROY()
	ON_WM_CREATE()
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_SMILE_LIST, OnItemchangedSmileList)
	ON_NOTIFY(NM_DBLCLK, IDC_SMILE_LIST, OnDblclkSmileList)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CSelectSmileDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CSelectSmileDlg)
	ON_EVENT(CSelectSmileDlg, IDC_MCCLOSE, -600 /* Click */, OnClickMcclose, VTS_NONE)
	ON_EVENT(CSelectSmileDlg, IDC_MCOK, -600 /* Click */, OnClickMcOK, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()


/////////////////////////////////////////////////////////////////////////////
// CSelectSmileDlg message handlers

void CSelectSmileDlg::OnOK() 
{
}

void CSelectSmileDlg::OnCancel() 
{
	COFSNcDlg2::OnCancel();
}

BOOL CSelectSmileDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	//m_SortHeader.SubclassWindow(m_SmileList.GetHeaderCtrl()->GetSafeHwnd());

	m_SmileImageList.Create(16, 16, ILC_MASK|ILC_COLOR24, 0, 10);
	m_SmileList.SetImageList(&m_SmileImageList,LVSIL_SMALL);

	m_SmileList.InsertColumn(0,_T("Text"),LVCFMT_LEFT,140);
	m_SmileList.InsertColumn(1,_T("Smile"),LVCFMT_RIGHT,50,1);
	
	AddAnchor(&m_SmileList,CSize(0,0),CSize(100,100));

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	//

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	// Load Smiles
	int index = 0;
	for(CSmileInfoListEnum item = CurrentSmileManager.GetSmiles().begin(); item!=CurrentSmileManager.GetSmiles().end(); item++, index++)
	{
		CBitmap *pBitmap = CurrentSmileManager.GetSmilePreview((*item).GetId());

		int ImageIndex = -1;

		if(pBitmap!=NULL)
		{
			ImageIndex = m_SmileImageList.Add(pBitmap, 0x808080);

			delete pBitmap;
		}

		int iSubIndex = m_SmileList.InsertItem(index,(*item).GetText(),ImageIndex);
		m_SmileList.SetItemText(iSubIndex ,1,(*item).GetSmile());
		m_SmileList.SetItemData(iSubIndex,(*item).GetIndex());
	}



	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CSelectSmileDlg::OnClickMcclose() 
{
	COFSNcDlg2::OnCancel();
}

void CSelectSmileDlg::OnClickMcOK() 
{
	COFSNcDlg2::OnOK();
}


void CSelectSmileDlg::OnClose() 
{
	COFSNcDlg2::OnClose();
}

void CSelectSmileDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("OK"), &m_btnOK, TRUE, FALSE);

	LoadRectangle2(pXmlRoot, _T("SmilePreview"), m_SmilePreview.GetSafeHwnd(), TRUE, FALSE);

	LoadRectangle2(pXmlRoot, _T("Smiles"), m_SmileList.GetSafeHwnd(), TRUE);

	// Temp Value

	AddAnchor(m_SmilePreview, CSize(0,0),CSize(0,0));

}

void CSelectSmileDlg::OnDblclkSmileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	POSITION pos = m_SmileList.GetFirstSelectedItemPosition();

	if(pos!=NULL)
	{
		int iSelectedItemIndex = m_SmileList.GetNextSelectedItem(pos);
		m_SelectedSmileIndex = m_SmileList.GetItemData(iSelectedItemIndex);

		COFSNcDlg2::OnOK();
	}

	*pResult = 0;
}

void CSelectSmileDlg::OnItemchangedSmileList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	POSITION pos = m_SmileList.GetFirstSelectedItemPosition();

	if(pos!=NULL)
	{
		int iSelectedItemIndex = m_SmileList.GetNextSelectedItem(pos);

		m_SelectedSmileIndex = m_SmileList.GetItemData(iSelectedItemIndex);

		CSmileInfo smile = CurrentSmileManager.GetSmile(m_SelectedSmileIndex);

		CString strPath;

		LPCTSTR szKey1 = _T("Software\\Mediachase\\Instant Business Network\\4.5\\Client\\Skins");
		LPCTSTR szEntry = _T("Skins Folder");
		McRegGetString(HKEY_LOCAL_MACHINE, szKey1, szEntry, strPath);


		strPath += GetProductLanguage();
		strPath += _T("\\Shell\\Smiles\\");
		strPath += smile.GetId();
		strPath += _T(".gif");

		m_SmilePreview.ShowWindow(SW_SHOW);

		m_SmilePreview.Load(strPath);

		m_SmilePreview.SetWindowPos(NULL,-1,-1, m_SmilePreview.GetSize().cx,m_SmilePreview.GetSize().cy, SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE);
		m_SmilePreview.Draw();

		m_SmilePreview.SetBkColor(0xd7d7d7);

	}
	else
	{
		m_SmilePreview.ShowWindow(SW_HIDE);
		m_SelectedSmileIndex = -1;
	}
	
	*pResult = 0;
}
