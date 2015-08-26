// MonitorDialog.cpp: implementation of the CMonitorDialog class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "MonitorDialog.h"
#include "MainDlg.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMonitorDialog::CMonitorDialog(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: COFSNcDlg2(CMonitorDialog::IDD, pParent)
{
	this->pMessenger = pMessenger;
	//{{AFX_DATA_INIT(CDlgAbout)
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/NetMonitor/skin.xml");
	m_bResizable = FALSE;

	m_lTotalSent			= -1;
	m_lTotalReceived		= -1;
	m_lMessageSent			= -1;
	m_lMessageReceived		= -1;
	m_lFileSent				= -1;
	m_lFileReceived			= -1;

	bIsKillWinodow			= FALSE;

	CString strCost = GetOptionString(IDS_NETOPTIONS, IDS_COST, _T("0"));
	if(_stscanf(strCost, _T("%f"), &m_Cost)==0)
		m_Cost = 0;
}

CMonitorDialog::~CMonitorDialog()
{

}

void CMonitorDialog::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgAbout)
	DDX_Control(pDX, IDC_BTN_X, m_btnX);
	DDX_Control(pDX, IDC_BTN_MINI, m_btnMini);
	DDX_Control(pDX, IDC_BTN_MENU, m_btnMenu);
	DDX_Control(pDX, IDC_BTN_UPDATE,m_btnUpdate);
	DDX_Control(pDX, IDC_COUNTTYPE_COMBO, m_comboCountType);
	DDX_Control(pDX, IDC_COST_EDIT, m_editCost);
	DDX_Text(pDX,IDC_COST_EDIT,m_Cost);
	DDX_Control(pDX,IDC_NETGRAPH_STATIC,m_graph);
	DDX_Control(pDX,IDC_TOTAL_SENT_STATIC, m_lbTotalSent);
	DDX_Control(pDX,IDC_TOTAL_RECEIVED_STATIC,m_lbTotalReceived);
	DDX_Control(pDX,IDC_MESSAGE_SENT_STATIC, m_lbMessageSent);
	DDX_Control(pDX,IDC_MESSAGE_RECEIVED_STATIC,m_lbMessageReceived);
	DDX_Control(pDX,IDC_FILE_SENT_STATIC,m_lbFileSent);
	DDX_Control(pDX,IDC_FILE_RECEIVED_STATIC,m_lbFileReceived);
	DDX_Control(pDX,IDC_TOTAL_COST_STATIC,m_lbTotalCost);

	DDX_Control(pDX,IDC_BTN_LINE_TOTAL,m_btnLineTotal);
	DDX_Control(pDX,IDC_BTN_LINE_SENT,m_btnLineSent);
	DDX_Control(pDX,IDC_BTN_LINE_RECEIVED,m_btnLineReceived);
	
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CMonitorDialog, COFSNcDlg2)
//{{AFX_MSG_MAP(CDlgAbout)
ON_WM_PAINT()
ON_WM_TIMER()
ON_CBN_SELCHANGE(IDC_COUNTTYPE_COMBO,OnCountTypeComboSelChange)
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CMonitorDialog, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CDlgAbout)
ON_EVENT(CMonitorDialog, IDC_BTN_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_MINI, -600 /* Click */, OnClickBtnMini, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_UPDATE, -600 /* Click */, OnClickBtnUpdate, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_MENU, -600 /* Click */, OnClickBtnMenu, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_LINE_TOTAL, -600 /* Click */, OnClickBtnLineTotal, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_LINE_SENT, -600 /* Click */, OnClickBtnLineSent, VTS_NONE)
ON_EVENT(CMonitorDialog, IDC_BTN_LINE_RECEIVED, -600 /* Click */, OnClickBtnLineReceived, VTS_NONE)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CMonitorDialog::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMini, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Update"), &m_btnUpdate, TRUE, FALSE);
	LoadRectangle(pXmlRoot, _T("ComboCountType") ,&m_comboCountType, TRUE, FALSE);
	LoadRectangle(pXmlRoot, _T("EditCost"), &m_editCost, TRUE, FALSE);
	LoadRectangle(pXmlRoot, _T("NetGraph"), &m_graph, TRUE, FALSE);

	LoadLabel(pXmlRoot, _T("TotalSent"), &m_lbTotalSent, TRUE);
	LoadLabel(pXmlRoot, _T("TotalReceived"), &m_lbTotalReceived, TRUE);
	LoadLabel(pXmlRoot, _T("FileSent"), &m_lbFileSent, TRUE);
	LoadLabel(pXmlRoot, _T("FileReceived"), &m_lbFileReceived, TRUE);
	LoadLabel(pXmlRoot, _T("MessageSent"), &m_lbMessageSent, TRUE);
	LoadLabel(pXmlRoot, _T("MessageReceived"), &m_lbMessageReceived, TRUE);
	LoadLabel(pXmlRoot, _T("TotalCost"), &m_lbTotalCost, TRUE);

	LoadButton(pXmlRoot, _T("LineTotal"), &m_btnLineTotal, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("LineSent"), &m_btnLineSent, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("LineReceived"), &m_btnLineReceived, TRUE, TRUE);
}	

BOOL CMonitorDialog::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	//////////////////////////////////////////////////////////////////////////

	m_comboCountType.SetCurSel(GetOptionInt(IDS_NETOPTIONS,IDS_COSTTYPE,0));
	
	CComPtr<ISession> pSession = theNet2.GetSession();
	if(pSession)
		HRESULT hr = pSession->QueryInterface(IID_IMonitor,(void**)&m_pMonitor);

	m_btnLineTotal.SetPressed(GetOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_TOTAL,1));
	m_btnLineSent.SetPressed(GetOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_SENT,1));
	m_btnLineReceived.SetPressed(GetOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_RECEIVED,1));
	
	m_graph.SetDrawLine(m_btnLineTotal.GetPressed(),m_btnLineSent.GetPressed(),m_btnLineReceived.GetPressed());

	OnTimer(101);
	SetTimer(101,1000,NULL);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CMonitorDialog::OnPaint() 
{
	COFSNcDlg2::OnPaint();
	
	m_lbTotalSent.Invalidate();
	m_lbTotalReceived.Invalidate();
	m_lbMessageSent.Invalidate();
	m_lbMessageReceived.Invalidate();
	m_lbFileSent.Invalidate();
	m_lbFileReceived.Invalidate();
	m_lbTotalCost.Invalidate();
}

void CMonitorDialog::OnClickBtnX() 
{
	OnCancel();
}

void CMonitorDialog::OnClickBtnUpdate()
{
	UpdateData();

	WriteOptionInt(IDS_NETOPTIONS,IDS_COSTTYPE,m_comboCountType.GetCurSel());
	
	CString strCost;
	strCost.Format(_T("%f"), m_Cost);

	WriteOptionString(IDS_NETOPTIONS,IDS_COST,strCost);

	CString winText;
	winText.Format(IDS_TOTAL_COST,RecaluclateTotalCost());
	m_lbTotalCost.SetText(winText);
}

float CMonitorDialog::RecaluclateTotalCost()
{
	int curSelItem = m_comboCountType.GetCurSel();

	float totalCost = 0;

	switch(curSelItem) 
	{
	case 0:
		totalCost = ((m_lTotalReceived + m_lTotalSent)/(1024.0*1024.0))*m_Cost;
		break;
	case 1:
		totalCost = ((m_lTotalReceived + m_lTotalSent)/(1024.0*128.0))*m_Cost;
		break;
	case 2:
		totalCost = ((m_lTotalReceived + m_lTotalSent)/(1024.0))*m_Cost;
		break;
	case 3:
		totalCost = ((m_lTotalReceived + m_lTotalSent)/(128.0))*m_Cost;
		break;
	}

	return totalCost;
}

void CMonitorDialog::OnOK() 
{
}

void CMonitorDialog::OnClickBtnMini()
{
	ShowWindow(SW_MINIMIZE);
}

void CMonitorDialog::OnClickBtnMenu()
{
	pMessenger->ShowGeneralMenu();
}

void CMonitorDialog::OnTimer(UINT nIDEvent)
{
	if(nIDEvent==101)
	{
		CString winText;

		long lValue	=	0;

		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_BytesSent(&lValue);

		BOOL bCostWasModifyed = FALSE;

		if(lValue!=m_lTotalSent)
		{
			m_lTotalSent = lValue;
			winText.Format(IDS_TOTAL_SENT,lValue);
			m_lbTotalSent.SetText(winText);
			bCostWasModifyed = TRUE;
		}

		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_BytesReceived(&lValue);

		if(lValue!=m_lTotalReceived)
		{
			m_lTotalReceived = lValue;
			winText.Format(IDS_TOTAL_RECEIVED,lValue);
			m_lbTotalReceived.SetText(winText);
			bCostWasModifyed = TRUE;
		}

		if(bCostWasModifyed)
		{
			winText.Format(IDS_TOTAL_COST,RecaluclateTotalCost());
			m_lbTotalCost.SetText(winText);
		}
		
		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_MessageSent(&lValue);

		if(lValue!=m_lMessageSent)
		{
			m_lMessageSent = lValue;
			winText.Format(IDS_MESSAGES_SENT,lValue);
			m_lbMessageSent.SetText(winText);
		}

		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_MessageReceived(&lValue);

		if(lValue!=m_lMessageReceived)
		{
			m_lMessageReceived = lValue;
			winText.Format(IDS_MESSAGES_RECEIVED,lValue);
			m_lbMessageReceived.SetText(winText);
		}

		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_FileSend(&lValue);

		if(lValue!=m_lFileSent)
		{
			m_lFileSent = lValue;
			winText.Format(IDS_FILE_SENT,lValue);
			m_lbFileSent.SetText(winText);
		}

		lValue = 0;
		if(m_pMonitor)
			m_pMonitor->get_FileReceived(&lValue);

		if(lValue!=m_lFileReceived)
		{
			m_lFileReceived = lValue;
			winText.Format(IDS_FILE_RECEIVED,lValue);
			m_lbFileReceived.SetText(winText);
		}

		// Update Graph [9/22/2003]
		if(m_pMonitor)
		{
			m_pMonitor->put_BytesSentPerSecondInterval((LONG)m_graph.m_glSent.GetDataBuffer());
			m_pMonitor->put_BytesReceivedPerSecondInterval((LONG)m_graph.m_glReceived.GetDataBuffer());

			m_graph.UpdateTotalLine();
			m_graph.Invalidate(FALSE);
		}

	}
}

void CMonitorDialog::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	COFSNcDlg2::OnClose();
	
	if(!bIsKillWinodow)
	{
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}

void CMonitorDialog::OnCancel() 
{
	KillWindow();
}

void CMonitorDialog::OnCountTypeComboSelChange()
{
}

void CMonitorDialog::OnClickBtnLineTotal()
{
	WriteOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_TOTAL,m_btnLineTotal.GetPressed());
	m_graph.SetDrawLine(m_btnLineTotal.GetPressed(),m_btnLineSent.GetPressed(),m_btnLineReceived.GetPressed());
}

void CMonitorDialog::OnClickBtnLineSent()
{
	WriteOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_SENT,m_btnLineSent.GetPressed());
	m_graph.SetDrawLine(m_btnLineTotal.GetPressed(),m_btnLineSent.GetPressed(),m_btnLineReceived.GetPressed());
}

void CMonitorDialog::OnClickBtnLineReceived()
{
	WriteOptionInt(IDS_NETOPTIONS,IDS_SHOW_LINE_RECEIVED,m_btnLineReceived.GetPressed());
	m_graph.SetDrawLine(m_btnLineTotal.GetPressed(),m_btnLineSent.GetPressed(),m_btnLineReceived.GetPressed());
}
