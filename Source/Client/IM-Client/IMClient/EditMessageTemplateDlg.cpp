// EditMessageTemplateDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "EditMessageTemplateDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CEditMessageTemplateDlg dialog


CEditMessageTemplateDlg::CEditMessageTemplateDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CEditMessageTemplateDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CEditMessageTemplateDlg)
	m_strName	=	_T("");
	m_strText	=	_T("");
	//}}AFX_DATA_INIT
}

CEditMessageTemplateDlg::CEditMessageTemplateDlg(CString strName, CString strText, CWnd* pParent /*=NULL*/)
	: CDialog(CEditMessageTemplateDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CEditMessageTemplateDlg)
	m_strName	=	strName;
	m_strText	=	strText;
	//}}AFX_DATA_INIT
}


void CEditMessageTemplateDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CEditMessageTemplateDlg)
	DDX_Text(pDX, IDC_NAME_EDIT, m_strName);
	DDX_Text(pDX, IDC_TEXT_EDIT, m_strText);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CEditMessageTemplateDlg, CDialog)
	//{{AFX_MSG_MAP(CEditMessageTemplateDlg)
	ON_EN_CHANGE(IDC_NAME_EDIT, OnChangeName)
	ON_EN_CHANGE(IDC_TEXT_EDIT, OnChangeText)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CEditMessageTemplateDlg message handlers

void CEditMessageTemplateDlg::OnOK()
{
	CDialog::OnOK();
}

void CEditMessageTemplateDlg::OnCancel()
{
	CDialog::OnCancel();
}


BOOL CEditMessageTemplateDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	GetDlgItem(IDOK)->EnableWindow(m_strName.GetLength()!=0);

	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}


void CEditMessageTemplateDlg::OnChangeName() 
{
	UpdateData();

	GetDlgItem(IDOK)->EnableWindow(m_strName.GetLength()!=0);
}

void CEditMessageTemplateDlg::OnChangeText() 
{
}
