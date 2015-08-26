// PageMesTemplate.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageMesTemplate.h"
#include "GlobalFunction.h"

#include "EditMessageTemplateDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageMesTemplates dialog


CPageMesTemplates::CPageMesTemplates(long UserId, LPCTSTR UserRole, LPCTSTR szTitle)
	: CMcSettingsPage(CPageMesTemplates::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageMesTemplates)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

	m_UserRole	=	UserRole;
	m_UserId	=	UserId;
}


void CPageMesTemplates::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageMesTemplates)
	DDX_Control(pDX, IDC_MESTEMPLATE_LIST, m_TemplateList);
	DDX_Check(pDX,IDC_SEND_AUTOMATICALLY,m_ShowAuto);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageMesTemplates, CMcSettingsPage)
	//{{AFX_MSG_MAP(CPageMesTemplates)
	ON_NOTIFY(NM_DBLCLK, IDC_MESTEMPLATE_LIST, OnClickMesTemplateList)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_MESTEMPLATE_LIST, OnItemchangedMesTemplateList)
	ON_BN_CLICKED(IDC_BUTTON_ADD, OnAddTemplateButton)
	ON_BN_CLICKED(IDC_BUTTON_DEL, OnDeleteTemplateButton)
	ON_BN_CLICKED(IDC_BUTTON_EDIT, OnEditTemplateButton)
	ON_BN_CLICKED(IDC_SEND_AUTOMATICALLY, OnShowAuto)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageMesTemplates message handlers

BOOL CPageMesTemplates::OnInitDialog() 
{
	CMcSettingsPage::OnInitDialog();

	m_ShowAuto =  GetOptionInt(IDS_OFSMESSENGER,IDS_SEND_MESSAGE_TEMPLATE_AUTO,1);

	DWORD dwStyle = m_TemplateList.GetExtendedStyle();
	dwStyle |= LVS_EX_FULLROWSELECT;
	m_TemplateList.SetExtendedStyle(dwStyle);

	m_TemplateList.InsertColumn(0,GetString(IDS_TEMPLATE_NAME),LVCFMT_LEFT,150);
	m_TemplateList.InsertColumn(1,GetString(IDS_TEMPLATE_TEXT),LVCFMT_LEFT,300);

	if(m_UserId)
	{
		// Step 1. Load Color Settings [8/24/2002]
		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),m_UserRole,m_UserId);

		CString strMessageTemplateXML = GetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG));

		if(strMessageTemplateXML.IsEmpty())
			strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);


		CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
		pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);

		if(pMTDoc)
		{
			CComBSTR bsXML;
			bsXML.Attach(strMessageTemplateXML.AllocSysString());
			
			VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
			
			pMTDoc->loadXML(bsXML,&varLoad);

			if(varLoad==VARIANT_FALSE)
			{
				bsXML.Empty();

				strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);
				bsXML.Attach(strMessageTemplateXML.AllocSysString());

				pMTDoc->loadXML(bsXML,&varLoad);
			}

			if(varLoad==VARIANT_TRUE)
			{
				CComPtr<IXMLDOMNodeList>	pTemplatesList	=	NULL;
					
				pMTDoc->selectNodes(CComBSTR(L"message_templates/mt"),&pTemplatesList);

				if(pTemplatesList!=NULL)
				{
					USES_CONVERSION;

					long ListLength	=	0;
					pTemplatesList->get_length(&ListLength);

					for(int Index=0;Index<ListLength;Index++)
					{
						CComBSTR	bsName, bsText;

						CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
						pTemplatesList->get_item(Index,&pStubNode);

						GetTextByPath(pStubNode, CComBSTR(L"name"),&bsName);
						GetTextByPath(pStubNode, CComBSTR(L"text"),&bsText);

						m_TemplateList.InsertItem(Index,W2CT(bsName));
						m_TemplateList.SetItemText(Index,1,W2CT(bsText));

					}
				}
			}
		}

	}

	UpdateData(FALSE);

	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPageMesTemplates::SaveSettings()
{
	UpdateData();

	WriteOptionInt(IDS_OFSMESSENGER,IDS_SEND_MESSAGE_TEMPLATE_AUTO,m_ShowAuto);

	CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
	pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);

	if(pMTDoc)
	{
		USES_CONVERSION;

		CComBSTR	bsRootXML	=	L"<message_templates/>";

		VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
		pMTDoc->loadXML(bsRootXML,&varLoad);

		//-------------------------------------------------------------------------------------
		CComPtr<IXMLDOMNode>	pRoot;

		pMTDoc->selectSingleNode(L"message_templates",&pRoot);

		for(int Index = 0; Index<m_TemplateList.GetItemCount();Index++)
		{
			CComPtr<IXMLDOMNode>	pItem;
			insertSingleNode(pRoot,CComBSTR(L"mt"),NULL,NULL,&pItem);

			
			insertSingleNode(pItem,CComBSTR(L"name"),NULL,T2BSTR(m_TemplateList.GetItemText(Index,0)));
			insertSingleNode(pItem,CComBSTR(L"text"),NULL,T2BSTR(m_TemplateList.GetItemText(Index,1)));
		}

		//-------------------------------------------------------------------------------------

		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),m_UserRole,m_UserId);

		CComBSTR bsOutXML;
		pMTDoc->get_xml(&bsOutXML);

		//-------------------------------------------------------------------------------------

		CString strMessageTemplateXML = W2CT(bsOutXML);
			
		SetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG),strMessageTemplateXML);
	}

	return CMcSettingsPage::SaveSettings();
}

void CPageMesTemplates::OnClickMesTemplateList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	*pResult = 0;

	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
	if(pNMListView->iItem!=-1)
	{
		CEditMessageTemplateDlg	addDlg(m_TemplateList.GetItemText(pNMListView->iItem,0),m_TemplateList.GetItemText(pNMListView->iItem,1));

		if(addDlg.DoModal()==IDOK)
		{
			m_TemplateList.SetItemText(pNMListView->iItem,0,addDlg.m_strName);
			m_TemplateList.SetItemText(pNMListView->iItem,1,addDlg.m_strText);

			SetModified();
		}
	}
}

void CPageMesTemplates::OnItemchangedMesTemplateList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	GetDlgItem(IDC_BUTTON_EDIT)->EnableWindow(m_TemplateList.GetSelectedCount()>0);
	GetDlgItem(IDC_BUTTON_DEL)->EnableWindow(m_TemplateList.GetSelectedCount()>0);

	*pResult = 0;
}


void CPageMesTemplates::OnAddTemplateButton()
{
	CEditMessageTemplateDlg	addDlg;

	if(addDlg.DoModal()==IDOK)
	{
		int ItemsCount = m_TemplateList.GetItemCount();
		m_TemplateList.InsertItem(ItemsCount,addDlg.m_strName);
		m_TemplateList.SetItemText(ItemsCount,1,addDlg.m_strText);

		SetModified();
	}
}

void CPageMesTemplates::OnEditTemplateButton()
{
	if(m_TemplateList.GetSelectedCount()>0)
	{
		POSITION	pos = m_TemplateList.GetFirstSelectedItemPosition();

		int iSel = m_TemplateList.GetNextSelectedItem(pos);

		CEditMessageTemplateDlg	addDlg(m_TemplateList.GetItemText(iSel,0),m_TemplateList.GetItemText(iSel,1));

		if(addDlg.DoModal()==IDOK)
		{
			m_TemplateList.SetItemText(iSel,0,addDlg.m_strName);
			m_TemplateList.SetItemText(iSel,1,addDlg.m_strText);

			SetModified();
		}
	}
}

void CPageMesTemplates::OnDeleteTemplateButton()
{
	if(m_TemplateList.GetSelectedCount()>0)
	{
		POSITION	pos = m_TemplateList.GetFirstSelectedItemPosition();

		while(pos!=NULL)
		{
			int Index = m_TemplateList.GetNextSelectedItem(pos);
			m_TemplateList.DeleteItem(Index);

			pos	=	m_TemplateList.GetFirstSelectedItemPosition();

			SetModified();
		}
	}
}

void CPageMesTemplates::OnShowAuto()
{
		SetModified();
}

