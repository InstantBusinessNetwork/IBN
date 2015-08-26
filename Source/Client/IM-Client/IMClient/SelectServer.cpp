// SelectServer.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "SelectServer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSelectServer dialog


CSelectServer::CSelectServer(CWnd* pParent /*=NULL*/)
	: CDialog(CSelectServer::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSelectServer)
	m_strServerName = _T("");
	//}}AFX_DATA_INIT
}


void CSelectServer::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSelectServer)
	DDX_Control(pDX, IDC_SERVER_LIST, m_ServerList);
	DDX_LBString(pDX, IDC_SERVER_LIST, m_strServerName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CSelectServer, CDialog)
	//{{AFX_MSG_MAP(CSelectServer)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSelectServer message handlers

void CSelectServer::OnOK() 
{
	// TODO: Add extra validation here
	
	CDialog::OnOK();
}

BOOL CSelectServer::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	for(int i=0;i<m_ServerNameArr.GetSize();i++)
	{
		m_ServerList.AddString(m_ServerNameArr[i]);
	}

	m_ServerList.SetCurSel(0);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
