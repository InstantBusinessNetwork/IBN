// TextCommentSettings.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "TextCommentSettings.h"
#include ".\textcommentsettings.h"

#define INITIAL_TEXT _T("Sample text 0123456789")
// CTextCommentSettings dialog

IMPLEMENT_DYNAMIC(CTextCommentSettings, CDialog)
CTextCommentSettings::CTextCommentSettings(CWnd* pParent /*=NULL*/)
	: CDialog(CTextCommentSettings::IDD, pParent)
{
	m_txtComment = _T("");
	m_fontColor = RGB(0, 0, 0);
	m_fontSize	= 12;
	m_fontName	=	_T("Arial");

}

CTextCommentSettings::~CTextCommentSettings()
{
	m_Font.DeleteObject();
}

void CTextCommentSettings::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT1, m_editComment);
}


BEGIN_MESSAGE_MAP(CTextCommentSettings, CDialog)
	ON_WM_DESTROY()
	ON_WM_CREATE()
	ON_BN_CLICKED(IDOK, OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, OnBnClickedCancel)	
	ON_BN_CLICKED(IDC_BTNFONT, OnBnClickedBtnfont)
	ON_EN_CHANGE(IDC_EDIT1, OnEnChangeEdit1)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// CTextCommentSettings message handlers

void CTextCommentSettings::OnDestroy()
{
	CDialog::OnDestroy();
}

BOOL CTextCommentSettings::OnInitDialog()
{
	CDialog::OnInitDialog();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);

	if(m_Font.m_hObject==NULL)
	{
		VERIFY(m_Font.CreateFont(
		m_fontSize,                        // nHeight
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
		m_fontName));                 // lpszFacename
	}

	m_editComment.SetFont(&m_Font, TRUE);
	m_editComment.SetWindowText(m_txtComment);
//	m_editComment.GetWindowText(m_txtComment);

	return TRUE;
}

int CTextCommentSettings::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDialog::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	return 0;
}

void CTextCommentSettings::OnBnClickedOk()
{
	// TODO: Add your control notification handler code here
	OnOK();
	//CloseWindow();
}

void CTextCommentSettings::OnBnClickedCancel()
{
	// TODO: Add your control notification handler code here
	OnCancel();
	//CloseWindow();
}

void CTextCommentSettings::OnBnClickedBtnfont()
{
	LOGFONT lf;
	m_Font.GetLogFont(&lf);

	CFontDialog	dlg(&lf,CF_EFFECTS | CF_SCREENFONTS,NULL,this);
	dlg.m_cf.rgbColors = m_fontColor;

	if (dlg.DoModal()==IDOK)
	{
		memcpy(&lf, dlg.m_cf.lpLogFont, sizeof(LOGFONT));

		m_Font.DeleteObject();
		m_Font.CreateFontIndirect(&lf);

		m_fontColor = dlg.GetColor();
		m_fontSize = lf.lfHeight;
		m_fontName = lf.lfFaceName;

		m_editComment.SetFont(&m_Font, TRUE);
	}
}

void CTextCommentSettings::OnEnChangeEdit1()
{
	m_editComment.GetWindowText(m_txtComment);
}

CString CTextCommentSettings::GetCommentText()
{
	return m_txtComment;
}

void CTextCommentSettings::SetCommentText(CString Text)
{
	m_txtComment = Text;
}

void CTextCommentSettings::SetFont(CFont& newFont)
{
	LOGFONT logfont;
	newFont.GetLogFont(&logfont);

	m_Font.DeleteObject();
	m_Font.CreateFontIndirect(&logfont);
}

void CTextCommentSettings::SetColor(COLORREF fontColor)
{
	m_fontColor = fontColor;
}

HBRUSH CTextCommentSettings::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	if (pWnd->GetDlgCtrlID()==IDC_EDIT1)	
	{
		pDC->SetTextColor(m_fontColor);
	}
	return hbr;
}
