// IbnCommandWnd.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "IbnCommandWnd.h"
#include "MainDlg.h"
#include "GlobalMessengerDef.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CIbnCommandWnd

CIbnCommandWnd::CIbnCommandWnd(CMainDlg *pMessenger)
{
	this->pMessenger = pMessenger;
}

CIbnCommandWnd::~CIbnCommandWnd()
{
}


BEGIN_MESSAGE_MAP(CIbnCommandWnd, CWnd)
//{{AFX_MSG_MAP(CIbnCommandWnd)
ON_WM_COPYDATA()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CIbnCommandWnd message handlers
BOOL CIbnCommandWnd::OnCopyData(CWnd *pWnd, COPYDATASTRUCT *pCopyDataStruct)
{
	CComBSTR bstrCommandXML = (LPCWSTR)pCopyDataStruct->lpData;
	
	CComBSTR bstrCommandResponse;
	
	if(bstrCommandXML==L"clientInfo")
	{
		bstrCommandResponse = BuildClientInfoXml();
	}
	else if(bstrCommandXML==L"contactList")
	{
		bstrCommandResponse = BuildContactListXml();
	}
	else
	{
		CComPtr<IXMLDOMDocument>	xmlCommandDoc	=	NULL;
		xmlCommandDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
		
		VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
		xmlCommandDoc->loadXML(bstrCommandXML,&varSuc);
		
		if(varSuc==VARIANT_TRUE)
		{
			CComPtr<IXMLDOMNode>	pRootNode;
			xmlCommandDoc->selectSingleNode(CComBSTR(L"clientInfo"),&pRootNode);
			
			if(pRootNode!=NULL)
			{
				bstrCommandResponse = BuildClientInfoXml();
			}
			else
			{
				xmlCommandDoc->selectSingleNode(CComBSTR(L"contactList"),&pRootNode);
				if(pRootNode!=NULL)
				{
					bstrCommandResponse = BuildContactListXml();
				}
				else
				{
					xmlCommandDoc->selectSingleNode(CComBSTR(L"sendFile"),&pRootNode);
					if(pRootNode!=NULL)
					{
						SendFile(pRootNode);
					}
					
					// TODO: Not Supported Command
				}
			}
			
		}
	}
	
	if(bstrCommandResponse.Length()>0)
	{
		// Send WM_COPYDATA
		COPYDATASTRUCT copyDs = {0};
		
		copyDs.cbData = (bstrCommandResponse.Length()+1)*2; // Specifies the size, in bytes, of the data pointed to by the lpData member. 
		copyDs.lpData = (PVOID)(BSTR)bstrCommandResponse; // Pointer to data to be passed to the receiving application. This member can be NULL. 
		
		LRESULT bResult = pWnd->SendMessage(WM_COPYDATA, 
			(WPARAM)GetSafeHwnd(), 
			(LPARAM)&copyDs);
	}
	
	return TRUE;
}

CComBSTR CIbnCommandWnd::BuildClientInfoXml()
{
	CComBSTR bstrCommandXML;
	
	bstrCommandXML += L"<clientInfo><response><login></login><firstName></firstName><lastName></lastName><email></email><status></status></response></clientInfo>";
	
	CComPtr<IXMLDOMDocument>	xmlResponseDoc	=	NULL;
	xmlResponseDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
	xmlResponseDoc->loadXML(bstrCommandXML,&varSuc);
	
	CComPtr<IXMLDOMNode>	xmlLoginNode;
	CComPtr<IXMLDOMNode>	xmlFirstNameNode;
	CComPtr<IXMLDOMNode>	xmlLastNameNode;
	CComPtr<IXMLDOMNode>	xmlEmailNode;
	CComPtr<IXMLDOMNode>	xmlStatusNode;
	
	xmlResponseDoc->selectSingleNode(CComBSTR(L"clientInfo/response/login"),&xmlLoginNode);
	xmlResponseDoc->selectSingleNode(CComBSTR(L"clientInfo/response/firstName"),&xmlFirstNameNode);
	xmlResponseDoc->selectSingleNode(CComBSTR(L"clientInfo/response/lastName"),&xmlLastNameNode);
	xmlResponseDoc->selectSingleNode(CComBSTR(L"clientInfo/response/email"),&xmlEmailNode);
	xmlResponseDoc->selectSingleNode(CComBSTR(L"clientInfo/response/status"),&xmlStatusNode);
	
	xmlLoginNode->put_text(CComBSTR((LPCTSTR)pMessenger->GetCurrentUser().m_strLogin));
	xmlFirstNameNode->put_text(CComBSTR((LPCTSTR)pMessenger->GetCurrentUser().m_strFirstName));
	xmlLastNameNode->put_text(CComBSTR((LPCTSTR)pMessenger->GetCurrentUser().m_strLastName));
	xmlEmailNode->put_text(CComBSTR((LPCTSTR)pMessenger->GetCurrentUser().m_strEMail));
	
	WCHAR Buff[20]	=	L"";
	_ltow(pMessenger->GetCurrentUser().m_iStatus,Buff,10);
	xmlStatusNode->put_text(CComBSTR(Buff));
	
	
	bstrCommandXML.Empty();
	xmlResponseDoc->get_xml(&bstrCommandXML);
	
	return bstrCommandXML;
}

CComBSTR CIbnCommandWnd::BuildContactListXml()
{
	USES_CONVERSION;
	
	CUserCollection	UsersContactList;
	pMessenger->GetCopyContactList(UsersContactList);
	
	CComPtr<IXMLDOMDocument>	pTreeItemDoc	=	NULL;
	
	pTreeItemDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
	CComBSTR	bsDefaultXML	= L"<contactList><response></response></contactList>";	
	
	pTreeItemDoc->loadXML(bsDefaultXML,&varSuc);
	
	CComPtr<IXMLDOMNode>	pRootNode;
	pTreeItemDoc->selectSingleNode(CComBSTR(L"contactList/response"),&pRootNode);
	
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	CUser* pUser=NULL;
	
	int GroupGlobalId = -1;
	if(POSITION pos = UsersContactList.InitIteration())
	{
		for(int i=0; UsersContactList.GetNext(pos, pUser); i++)
		{
			// Step 1. Проверить создавали ли мы группу???
			CComBSTR	GroupName =	pUser->m_strType;
			
			if(CLMode==2)
			{
				if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
				{
					GroupName	=  GetString(IDS_OFFLINE);
				}
			}
			
			BOOL isCheck = FALSE;
			
			CComBSTR bsGroupPath = L"contactList/response/group[@name='";
			bsGroupPath += GroupName;
			bsGroupPath += L"']";
			
			CComPtr<IXMLDOMNode>	pGroupNode;
			
			pTreeItemDoc->selectSingleNode(bsGroupPath,&pGroupNode);
			
			if(pGroupNode==NULL)
			{
				CComPtr<IXMLDOMNode>	pTmpGroupNode;
				
				pTreeItemDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"group"),NULL,&pTmpGroupNode);
				
				SetAttribute(pTmpGroupNode,CComBSTR(L"name"),CComVariant(GroupName));
				
				pGroupNode = AppendWithSort(pRootNode, pTmpGroupNode, CComBSTR(L"text"));
			}
			
			// Step 3. добавить пользователя [1/28/2002]
			CComPtr<IXMLDOMNode> pUserNode;
			
			insertSingleNode(pGroupNode, CComBSTR(L"user"), NULL, NULL, &pUserNode);
			
			WCHAR buffUserId[20]	=	L"";
			_ltow(pUser->GetGlobalID(),buffUserId,10);
			
			WCHAR buffUserStatus[20]	=	L"";
			_ltow(pUser-> GetStatus(),buffUserStatus,10);
			
			insertSingleNode(pUserNode, CComBSTR(L"id"), NULL, CComBSTR(buffUserId));
			insertSingleNode(pUserNode, CComBSTR(L"login"), NULL, CComBSTR((LPCTSTR)pUser->m_strLogin));
			insertSingleNode(pUserNode, CComBSTR(L"firstName"), NULL, CComBSTR((LPCTSTR)pUser->m_strFirstName));
			insertSingleNode(pUserNode, CComBSTR(L"lastName"), NULL, CComBSTR((LPCTSTR)pUser->m_strLastName));
			insertSingleNode(pUserNode, CComBSTR(L"email"), NULL, CComBSTR((LPCTSTR)pUser->m_strEMail));
			insertSingleNode(pUserNode, CComBSTR(L"status"), NULL, CComBSTR(buffUserStatus));
		}
	}
	
	CComBSTR bsRetVal;
	pTreeItemDoc->get_xml(&bsRetVal);
	
	return bsRetVal;
}

void CIbnCommandWnd::SendFile(CComPtr<IXMLDOMNode>	&pRootNode)
{
	USES_CONVERSION;

	CComBSTR bsTmpFileName;


	CComPtr<IXMLDOMNode>	pFileNode;
	pRootNode->selectSingleNode(CComBSTR(L"request/file"),&pFileNode);
	if(pFileNode==NULL)
		return;
	pFileNode->get_text(&bsTmpFileName);

	CComPtr<IXMLDOMNodeList>	pRecipientIdNodeList	=	NULL;
	pRootNode->selectNodes(CComBSTR(L"request/recipients/id"),&pRecipientIdNodeList);
	if(pRecipientIdNodeList==NULL)
		return;

	SendFileInfo *pSendFileInfo = new SendFileInfo();

	long listLength	=	0;
	pRecipientIdNodeList->get_length(&listLength);

	CString strRecepientId = _T("");

	for(int index=0;index<listLength;index++)
	{
		CComBSTR bsUserId;

		CComPtr<IXMLDOMNode>	pIdNode	=	NULL;
		pRecipientIdNodeList->get_item(index,&pIdNode);

		pIdNode->get_text(&bsUserId);

		strRecepientId = W2CT(bsUserId);

		pSendFileInfo->RecepientID += strRecepientId;
		pSendFileInfo->RecepientID += _T(",");
	}

	

	if(listLength==1)
	{
		int globalId = _ttol(strRecepientId);
		CUser user;
		if(pMessenger->GetUserByGlobalId(globalId, user))
		{
			pSendFileInfo->RecepientName = 	user.GetShowName();
		}
		else
			pSendFileInfo->RecepientName = _T("Screen Capture User Group");
	}
	else
		pSendFileInfo->RecepientName = _T("Screen Capture User Group");
	pSendFileInfo->Description = GetString(IDS_SCREEN_CAPTURE_NAME);
	pSendFileInfo->File = W2CT(bsTmpFileName);

	pMessenger->PostMessage(WM_SEND_FILE2, (WPARAM)pSendFileInfo);
}


BOOL CIbnCommandWnd::SendScreenCaptureCommand(CComBSTR Command, CComBSTR Mode, CComBSTR RecipientsXml)
{
	// Create Hwnd String
	WCHAR wsTmpBuf[20];
	_ltow((long)pMessenger->GetSafeHwnd(),wsTmpBuf,10);
	
	CComBSTR bstrClientHwnd = wsTmpBuf;
	
	// Create Xml Command
	CComBSTR sendXML;
	
	sendXML += CComBSTR(L"<");
	sendXML += Command;
	sendXML += CComBSTR(L">");
	
	sendXML += CComBSTR(L"<request>");
	sendXML += CComBSTR(L"<ibnClientHandle>");
	sendXML += bstrClientHwnd;
	sendXML += CComBSTR(L"</ibnClientHandle>");
	
	sendXML += CComBSTR(L"<mode>");
	sendXML += Mode;
	sendXML += L"</mode>";
	
	if(RecipientsXml.Length()>0)
	{
		sendXML += CComBSTR(L"<recipients>");
		sendXML += RecipientsXml; 
		sendXML += L"</recipients>";
	}
	
	sendXML += CComBSTR(L"</request>");
	
	sendXML += CComBSTR(L"</");
	sendXML += Command;
	sendXML += CComBSTR(L">");

	// Test Only
	//sendXML = L"<clientInfo></clientInfo>";

	// TODO: Find Screen Capture Window
	LPCTSTR szScreenCaptureClassName = _T("#32770");
	LPCTSTR szScreenCaptureWindowName = _T("{19791104-A59E-42e5-BB49-200706080000}");
	
	CWnd *pScreenCaptureCommandWnd = FindWindow(szScreenCaptureClassName,szScreenCaptureWindowName);
	
	if(pScreenCaptureCommandWnd==NULL)
	{
		// TODO:
		CString screenCapurePath = pMessenger->ScreenCapturePath();

		// Run Screen Capture
		if(screenCapurePath.GetLength()>0)
		{
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,screenCapurePath,_T(""),NULL,SW_SHOWNORMAL);
			
			if((int)retVal>32)
			{
				// Find Screen Capture Window Again
				for(int index=0;index<20;index++)
				{
					Sleep(500);

					pScreenCaptureCommandWnd = FindWindow(szScreenCaptureClassName,szScreenCaptureWindowName);
					if(pScreenCaptureCommandWnd!=NULL)
						break;
				}
			}
		}

	}

	if(pScreenCaptureCommandWnd!=NULL)
	{
		// Send WM_COPYDATA
		COPYDATASTRUCT copyDs = {0};
		
		copyDs.cbData = (sendXML.Length()+1)*2; // Specifies the size, in bytes, of the data pointed to by the lpData member. 
		copyDs.lpData = (PVOID)(BSTR)sendXML; // Pointer to data to be passed to the receiving application. This member can be NULL. 
		
		LRESULT bResult = pScreenCaptureCommandWnd->SendMessage(WM_COPYDATA, 
			(WPARAM)GetSafeHwnd(), 
			(LPARAM)&copyDs);
		
		return TRUE;
	}
	
	return FALSE;
}

BOOL CIbnCommandWnd::Create()
{
	BOOL bResult = FALSE;
	
	bResult = CWnd::CreateEx(0,
		MC_IBNCOMMAND_CLASS_NAME, 
		MC_IBNCOMMAND_WINDOW_NAME, WS_OVERLAPPED, 
		CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT,CW_USEDEFAULT, ::GetDesktopWindow(), 0);
	
	return bResult;
}
