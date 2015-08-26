// PageChat.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "PageChat.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageChat dialog


CPageChat::CPageChat(LPCTSTR UserRole, int UserId, LPCTSTR szTitle)
	: CMcSettingsPage(CPageChat::IDD, szTitle)
{
	//{{AFX_DATA_INIT(CPageChat)
	m_lShowMessages = 0;
	//}}AFX_DATA_INIT
	m_UserRole	=	UserRole;
	m_UserId	=	UserId;

	m_bWasChanged	=	FALSE;
	m_bWasSaved		=	FALSE;
}


void CPageChat::DoDataExchange(CDataExchange* pDX)
{
	CMcSettingsPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageChat)
	DDX_Control(pDX, IDC_INFO_COLOR_LIST, m_InformationColorListBox);
	DDX_Control(pDX, IDC_USER_COLOR_LIST, m_UserColorListBox);
	DDX_Control(pDX, IDC_LOAD_LASTMES_SPIN, m_LoadLastMesSpin);
	DDX_Text(pDX, IDC_LOAD_LASTMES_EDIT, m_lShowMessages);
	DDV_MinMaxLong(pDX, m_lShowMessages, 0, 500);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPageChat, CMcSettingsPage)
//{{AFX_MSG_MAP(CPageChat)
ON_EN_CHANGE(IDC_LOAD_LASTMES_EDIT, OnChangeLoadLastmesEdit)
ON_MESSAGE(WM_COLOR_CHANGED,OnColorChanged)
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageChat message handlers

BOOL CPageChat::SaveSettings()
{
	USES_CONVERSION;
	
	UpdateData();
	
	WriteOptionInt(IDS_OFSMESSENGER,IDS_CHATLOADMESS,m_lShowMessages);

	// Save Settings [8/28/2002]
	if(m_UserId)
	{
		m_bWasSaved	=	TRUE;

		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),m_UserRole,m_UserId);
		
		CString strColorXML = GetRegFileText(strSection,GetString(IDS_SELECT_COLOR));

		if(strColorXML.IsEmpty())
			strColorXML = _T("<colors/>");
		
		CComPtr<IXMLDOMDocument>	pColorDoc	=	NULL;
		pColorDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
		if(pColorDoc)
		{
			CComBSTR bsXML;
			bsXML.Attach(strColorXML.AllocSysString());
			
			VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
			
			pColorDoc->loadXML(bsXML,&varLoad);

			if(varLoad==VARIANT_FALSE)
			{
				bsXML.Empty();
				strColorXML = _T("<colors/>");
				bsXML.Attach(strColorXML.AllocSysString());

				pColorDoc->loadXML(bsXML,&varLoad);
			}

			if(varLoad==VARIANT_TRUE)
			{
				for(int iItemIndex = 0; iItemIndex< (m_UserColorListBox.GetCount() + m_InformationColorListBox.GetCount());iItemIndex++)
				{
					DWORD	dwId		= 0;
					DWORD	dwColor	= 0;
					CString	strText;

					if(iItemIndex<m_UserColorListBox.GetCount())
					{
						dwId		= m_UserColorListBox.GetItemData(iItemIndex);
						dwColor		= m_UserColorListBox.GetColorById(dwId);
						strText		= m_UserColorListBox.m_ItemMap[dwId].Text;
					}
					else
					{
						dwId		= m_InformationColorListBox.GetItemData(iItemIndex-m_UserColorListBox.GetCount());
						dwColor		= m_InformationColorListBox.GetColorById(dwId);
						strText		= m_InformationColorListBox.m_ItemMap[dwId].Text;
					}
					
					CString strQueryFormat;
					strQueryFormat.Format(_T("colors/color_item[id=\"%d\"]"),dwId);
					
					CComBSTR bsQuery;
					bsQuery.Attach(strQueryFormat.AllocSysString());
					
					CComPtr<IXMLDOMNode>	pColorItemNode = NULL;
					
					pColorDoc->selectSingleNode(bsQuery,&pColorItemNode);
					
					if(pColorItemNode)
					{
						CComPtr<IXMLDOMNode>	pColorItemColorNode = NULL;
						CheckNodeByPath(pColorItemNode,CComBSTR(L"color"),&pColorItemColorNode);
						
						if(pColorItemColorNode)
						{
							CString strColorFormat;
							
							// Confert RGB Color to BGR [8/28/2002]
							WCHAR wsBuff[100];
							swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
							
							CComBSTR	bsColor = wsBuff;
							
							pColorItemColorNode->put_text(bsColor);
						}
					}
					else
					{
						CComPtr<IXMLDOMNode>	pColorsNode = NULL;
						
						pColorDoc->selectSingleNode(CComBSTR(L"colors"),&pColorsNode);

						if(pColorsNode)
						{
							CComPtr<IXMLDOMNode>	pColorItemNode = NULL;
							
							insertSingleNode(pColorsNode,CComBSTR("color_item"),NULL,NULL,&pColorItemNode);
							
							if(pColorItemNode)
							{
								WCHAR wsBuffId[100],wsBuff[100];
								
								_ltow(dwId,wsBuffId,10);
								
								//_ultow(dwColor,wsBuffColor,16);
								swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
								
								insertSingleNode(pColorItemNode,CComBSTR("id"),NULL,CComBSTR(wsBuffId));
								insertSingleNode(pColorItemNode,CComBSTR("text"),NULL,CComBSTR(T2CW(strText)));
								insertSingleNode(pColorItemNode,CComBSTR("color"),NULL,CComBSTR(wsBuff));
							}
						}
					}
				}
				
				CComBSTR bsXML;
				pColorDoc->get_xml(&bsXML);

				SetRegFileText(strSection,GetString(IDS_SELECT_COLOR),W2CT(bsXML));
			}		
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	//  [8/28/2002]
	
	return CMcSettingsPage::SaveSettings();
}

BOOL CPageChat::OnInitDialog() 
{
	USES_CONVERSION;
	
	CMcSettingsPage::OnInitDialog();
	
	m_lShowMessages = GetOptionInt(IDS_OFSMESSENGER,IDS_CHATLOADMESS,20);
	
	m_LoadLastMesSpin.SetRange(0,500);
	m_LoadLastMesSpin.SetBuddy(GetDlgItem(IDC_LOAD_LASTMES_EDIT));
	
	UpdateData(FALSE);

	if(m_UserId)
	{
		// Step 1. Load Color Settings [8/24/2002]
		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),m_UserRole,m_UserId);
		
		CString strColorXML = GetRegFileText(strSection,GetString(IDS_SELECT_COLOR));

		if(!strColorXML.IsEmpty())
		{
			CComPtr<IXMLDOMDocument>	pColorDoc	=	NULL;
			pColorDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
			if(pColorDoc)
			{
				CComBSTR bsXML;
				bsXML.Attach(strColorXML.AllocSysString());

				VARIANT_BOOL	varLoad	=	VARIANT_FALSE;

				pColorDoc->loadXML(bsXML,&varLoad);

				if(varLoad==VARIANT_TRUE)
				{
					/************************************************************************/
					/*	<colors>                                                                     
							<color_item>
								<id></id>
								<text></text>
								<color></color>
							</item>
						</colors>
					*/
					/************************************************************************/
					// Step 2. Unpack XML and Load Settings [8/24/2002]
					CComPtr<IXMLDOMNodeList>	pUserList	=	NULL;
					pColorDoc->selectNodes(CComBSTR(L"colors/color_item"),&pUserList);
					
					if(pUserList)
					{
						long lUserLength  = 0;
						pUserList->get_length(&lUserLength);

						for(long lUserIndex = 0;lUserIndex<lUserLength;lUserIndex++)
						{
							CComPtr<IXMLDOMNode>	pUserNode	=	NULL;
							pUserList->get_item(lUserIndex,&pUserNode);
							if(pUserNode)
							{
								CComBSTR bsUserId, bsText, bsColor;

								GetTextByPath(pUserNode,CComBSTR(L"id"),&bsUserId);
								GetTextByPath(pUserNode,CComBSTR(L"text"),&bsText);
								GetTextByPath(pUserNode,CComBSTR(L"color"),&bsColor);

								WCHAR *pStr;

								DWORD dwId		= wcstol(bsUserId,&pStr,10);

								DWORD dwColor	=	wcstoul(bsColor,&pStr,16);

								// Confert HTML Color (BGR) to RGB [8/28/2002]
								dwColor	=	RGB(GetBValue(dwColor),GetGValue(dwColor),GetRValue(dwColor));
								// Confert HTML Color (BGR) to RGB [8/28/2002]

								if(LONG(dwId)>0)
									m_UserColorListBox.AddItem(dwId,W2CT(bsText),dwColor);
								else
									m_InformationColorListBox.AddItem(dwId,W2CT(bsText),dwColor);
							}
						}
					}
				}
			}
		}

		if(m_InformationColorListBox.GetCount()==0&&m_UserId)
		{
			// IDS_INVITED_MESSAGE [8/29/2002]
			// IDS_LEAVED_MESSAGE  [8/29/2002]
			// IDS_ACCEPT_MESSAGE  [8/29/2002]
			// IDS_DENY_MESSAGE    [8/29/2002]

			m_InformationColorListBox.AddItem(-IDS_INVITED_MESSAGE,GetString(IDS_INVITED_MESSAGE),RGB(0,0,0xFF));
			m_InformationColorListBox.AddItem(-IDS_LEAVED_MESSAGE,GetString(IDS_LEAVED_MESSAGE),RGB(0,0,0xFF));
			m_InformationColorListBox.AddItem(-IDS_ACCEPT_MESSAGE,GetString(IDS_ACCEPT_MESSAGE),RGB(0,0,0xFF));
			m_InformationColorListBox.AddItem(-IDS_DENY_MESSAGE,GetString(IDS_DENY_MESSAGE),RGB(0,0,0xFF));

			// Load Default Messages and Colors [8/28/2002]
		}
	}
	
	return TRUE;
}

void CPageChat::OnChangeLoadLastmesEdit() 
{
	SetModified();
}	

LPARAM CPageChat::OnColorChanged(WPARAM w, LPARAM l)
{
	m_bWasChanged	=	TRUE;
	SetModified();
	return 1;
}