// Chat.cpp: implementation of the CChat class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "Chat.h"
#include "SupportXMLFunction.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
static CMapStringToPtr		globalWndMsgChatMap;

CChat::CChat()
{
	m_pChat			=	NULL;
	m_lChatStatus	=	SC_INACTIVE;
	m_lTID			=	0;
//	m_hWndMsgChat	=	NULL;
}

CChat::~CChat()
{
	
}

CChat::CChat(const CChat &srcChat)
{
	*this = srcChat;
}

CChat::CChat(IChat *pChat)
{
	m_pChat			=	pChat;
	m_lChatStatus	=	SC_INACTIVE;
	m_lTID			=	0;

	m_pHistoryXML	=	NULL;
}

const CChat& CChat::operator=(const CChat &pSrc)
{
	m_bsId.Empty();
	
	m_pChat			=	pSrc.m_pChat;
	m_lChatStatus	=	pSrc.m_lChatStatus;
	m_lTID			=	pSrc.m_lTID;

	m_pHistoryXML	=	pSrc.m_pHistoryXML;

//	m_hWndMsgChat	=	pSrc.m_hWndMsgChat;

	m_ContactList.Clear();
	
	POSITION pos = const_cast<CChat&>(pSrc).m_ContactList.InitIteration();
	
	CUser *pUser	=	NULL;

	while(const_cast<CChat&>(pSrc).m_ContactList.GetNext(pos,pUser))
	{
		m_ContactList.SetAt(*pUser);
	}
	
	return *this;
}


BOOL CChat::operator ==(const CChat &pSrc) const
{
	if(GetInterface()==NULL&&pSrc.GetInterface()==NULL)
		return TRUE;

	return GetId()==pSrc.GetId();
}

BOOL CChat::operator !=(const CChat &pSrc) const
{
	return !(operator==(pSrc));
}

void CChat::Attach(IChat* pChat)
{
	m_pChat			=	NULL;
	m_lChatStatus	=	SC_INACTIVE;
	m_lTID			=	0;
	m_pHistoryXML	=	NULL;
	//m_hWndMsgChat	=	NULL;

	m_ContactList.Clear();

	m_bsId.Empty();

	m_pChat.Attach(pChat);
}

IChat* CChat::Detach()
{
	m_bsId.Empty();
	m_lChatStatus	=	SC_INACTIVE;
	m_lTID			=	0;
	m_pHistoryXML	=	NULL;
	//m_hWndMsgChat	=	NULL;
	
	m_ContactList.Clear();

	return m_pChat.Detach();
}

IChat* CChat::operator->() const
{
	return m_pChat.operator->();
}

const CComPtr<IChat>& CChat::GetInterface() const
{
	return m_pChat;
}

HRESULT CChat::GetValueBSTR(BSTR ValName, BSTR* pOut) const
{
	HRESULT hr = S_OK;

	CComVariant	varData;
	hr = GetValue(ValName,&varData);
	if(hr==S_OK)
	{
		hr = varData.ChangeType(VT_BSTR);
		if(hr==S_OK)
			*pOut = SysAllocString(varData.bstrVal);
	}
	return hr;	
}

HRESULT CChat::GetValueLONG(BSTR ValName, LONG* pOut) const
{
	HRESULT hr = S_OK;

	CComVariant	varData;
	hr = GetValue(ValName,&varData);
	if(hr==S_OK)
	{
		hr = varData.ChangeType(VT_I4);
		if(hr==S_OK)
			*pOut = varData.lVal;
	}
	return hr;	
}

HRESULT CChat::GetValue(BSTR ValName, VARIANT* pOut) const
{
	HRESULT hr = S_OK;
	if(m_pChat)
	{
		hr = m_pChat->get_Value(ValName,pOut);
	}
	return hr;
}



CComBSTR CChat::GetId() const
{
	if(!m_bsId)
		GetValueBSTR(CComBSTR(L"@cid"),&(const_cast<CChat*>(this)->m_bsId));
	return		m_bsId;
}

CComBSTR CChat::GetName() const
{
	CComBSTR	bsName;
	GetValueBSTR(CComBSTR(L"name"),&bsName);
	return bsName;
}

CComBSTR CChat::GetDescription() const
{
	CComBSTR	bsDescription;
	GetValueBSTR(CComBSTR(L"descr"),&bsDescription);
	return bsDescription;
}

LONG CChat::GetOwnerId() const
{
	LONG	lOwnerId	=	0;
	GetValueLONG(CComBSTR(L"owner"),&lOwnerId);
	return lOwnerId;
}

LONG CChat::GetTime() const
{
	LONG	lTime	=	0;
	GetValueLONG(CComBSTR(L"time"),&lTime);
	return lTime;
}

LONG CChat::GetStatus()
{
	return m_lChatStatus;
}

LONG CChat::GetTID()
{
	return m_lTID;
}

LONG CChat::SetTID(LONG newVal)
{
	LONG oldVal = m_lTID;
	m_lTID	=	newVal;
	return oldVal;
}

CString CChat::GetShowName()  const
{
	USES_CONVERSION;
	CString strFormat = GetString(IDS_CHAT_SHOW_NAME);
	
	CString retVal;
	retVal.Format(strFormat,W2CT(GetName()));
	
	return retVal;
}

LONG CChat::SetStatus(LONG newVal)
{
	LONG oldVal = m_lChatStatus;
	m_lChatStatus	=	newVal;
	return oldVal;
	
}

void CChat::LoadUser(IUsers *pUsers)
{
	long iArraySize = 0L;
	
	HRESULT hr = pUsers->get_Count(&iArraySize);
	
	for(long i=1;i<=iArraySize;i++)
	{
		CUser user(pUsers->GetItem(i));
		m_ContactList.SetAt(user);
	}
}

CUserCollection& CChat::GetUsers()
{
	return m_ContactList;
}

BOOL CChat::LoadMessages(LPCTSTR UserRole, int UserId, BSTR bsChatMessages)
{
	if(m_pHistoryXML==NULL)
		m_pHistoryXML.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
		/* Messages XML Format ...
		<log>
			<mess>
				<user id="1">
					<first_name>Employee</first_name> 
					<last_name>UserTwo</last_name> 
				</user>
				<body>54353543</body> 
				<time>888</time> 
				<show_time>..</show_time> *
				<color></color>
		</mess>
		...
		</log>
	*/
	VARIANT_BOOL	vbLoad	=	VARIANT_FALSE;
	m_pHistoryXML->loadXML(bsChatMessages,&vbLoad);
	
	if(vbLoad==VARIANT_TRUE)
	{
		// Confert time to Str
		CComPtr<IXMLDOMNodeList>	pMessList	=	NULL;
		m_pHistoryXML->selectNodes(CComBSTR(L"//mess"),&pMessList);
		
		if(pMessList)
		{
			long listLength	=	0;
			pMessList->get_length(&listLength);
			for(long lHodeIndex = 0;lHodeIndex<listLength;lHodeIndex++)
			{
				CComPtr<IXMLDOMNode>	pMessNode=NULL;
				pMessList->get_item(lHodeIndex,&pMessNode);
				if(pMessNode)
				{
					CComPtr<IXMLDOMNode>	pTimeNode	=	NULL;
					pMessNode->selectSingleNode(CComBSTR(L"time"),&pTimeNode);

					if(pTimeNode)
					{
						CComBSTR	bsText;
						pTimeNode->get_text(&bsText);
						
						long lTime = _wtol(bsText);
						
						if(lTime)
						{
							CTime	Time(lTime);
							
							SYSTEMTIME		sysTime		=	{0};
							TCHAR			szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");
							
							Time.GetAsSystemTime(sysTime);
							
							GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysTime,NULL,szDate,MAX_PATH);
							GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysTime,NULL,szTime,MAX_PATH);
							
							CString	strDataFormat;
							strDataFormat.Format(_T("%s %s"),szDate,szTime);
							
							CComBSTR bsStrTime;
							bsStrTime.Attach(strDataFormat.AllocSysString());
							
							//pTimeNode->put_text(bsStrTime);
							
							//CComPtr<IXMLDOMNode> pTimeParantNode	=	NULL;
							
							//pTimeNode->get_parentNode(&pTimeParantNode);
							
							insertSingleNode(pMessNode,CComBSTR(L"show_time"),NULL,bsStrTime);
						}
					}
					
					if(UserId)
					{
						CComPtr<IXMLDOMNode>	pUserIdNode = NULL;
						
						pMessNode->selectSingleNode(CComBSTR(L"user"),&pUserIdNode);
						
						if(pUserIdNode)
						{
							CComVariant	varID;
							GetAttribute(pUserIdNode,CComBSTR(L"id"),&varID);

							varID.ChangeType(VT_I4);

							DWORD dwColor =	0;
							
							if(GetColorFromColorStorage(UserRole, UserId,varID.lVal,dwColor))
							{
								//Color	=	RGB(GetBValue(dwColor),GetGValue(dwColor),GetRValue(dwColor));
								
								WCHAR wsBuff[100];
								swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
								//_ultow(dwColor,wsBuff,16);
								
								insertSingleNode(pMessNode,CComBSTR(L"color"),NULL,CComBSTR(wsBuff));
							}
							else
							{
								ASSERT(FALSE);
							}
						}
					}
				}
			}
		}
	}
	else
	{
		m_pHistoryXML->loadXML(CComBSTR(L"<log/>"),&vbLoad);
	}

#ifdef _DEBUG
	m_pHistoryXML->save(CComVariant(L"c:\\log_chatmess.xml"));
#endif

	return (vbLoad==VARIANT_TRUE)?TRUE:FALSE;
}

BOOL CChat::GetMessages(BSTR *pbsMessages)
{
	if(m_pHistoryXML==NULL)
		return FALSE;

	return (m_pHistoryXML->get_xml(pbsMessages)==S_OK)?TRUE:FALSE;
}

BOOL CChat::AddNewMessages(BSTR bsMessages)
{
	if(m_pHistoryXML==NULL)
	{
		return LoadMessages(NULL,0,bsMessages);
	}
	
	CComPtr<IXMLDOMDocument> pNewMessages;
	pNewMessages.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);

	VARIANT_BOOL	vbLoad	=	VARIANT_FALSE;
	pNewMessages->loadXML(bsMessages,&vbLoad);
	if(vbLoad==VARIANT_FALSE)
		return FALSE;

	// Step 1. Check Message By Message Id [8/15/2002]
	CComPtr<IXMLDOMNodeList>	pNodes	=	NULL;
	pNewMessages->selectNodes(CComBSTR(L"log/mess"),&pNodes);
	if(pNodes)
	{
		long listLength	=	0;
		pNodes->get_length(&listLength);

		CComPtr<IXMLDOMNode>	pHistoryLogNode	=	NULL;
		m_pHistoryXML->selectSingleNode(CComBSTR(L"log"),&pHistoryLogNode);

		for(int iNodeInedex = 0;iNodeInedex<listLength;iNodeInedex++)
		{
			CComPtr<IXMLDOMNode>	pNode	=	NULL, pIdNode	=	NULL, pResultNode	=	NULL;
			pNodes->get_item(iNodeInedex,&pNode);
			if(pNode)
			{
				CComBSTR bsId;
				GetTextByPath(pNode,CComBSTR(L"id"),&bsId);

				CComBSTR	bsQueryString = L"mess[id=\"";
				bsQueryString += bsId;
				bsQueryString += L"\"]";

				pHistoryLogNode->selectSingleNode(bsQueryString,&pResultNode);

				if(!pResultNode)
				{
					// Step 2. If need add new message, add message and return TRUE;
					pHistoryLogNode->appendChild(pNode,NULL);
				}
			}
			
		}
	}

#ifdef _DEBUG
	m_pHistoryXML->save(CComVariant(L"c:\\log_chatmess.xml"));
#endif
	
	return TRUE;
}

HWND CChat::GetChatWindow()
{
	HWND result = NULL;

	USES_CONVERSION;
	LPCTSTR szId = W2CT(GetId());
	if(szId)
		result = (HWND)globalWndMsgChatMap[szId];

	return result;
}

HWND CChat::SetChatWindow(HWND hWnd)
{
	USES_CONVERSION;

	LPCTSTR strId = W2CT(GetId());

 	HWND OldhWnd = (HWND)globalWndMsgChatMap[strId];
 	globalWndMsgChatMap[strId] = hWnd;

 	return OldhWnd;
}

LPDISPATCH CChat::GetMessagesInterface()
{
	if(m_pHistoryXML==NULL)
	{
		LoadMessages(NULL,0,NULL);
	}
	
	return (LPDISPATCH)m_pHistoryXML;
}

void CChat::AddNewEvent(DWORD dwId, BSTR FirstName, BSTR LastName, BSTR bsMessage, DWORD dwTime, DWORD dwColor)
{
	if(m_pHistoryXML==NULL)
	{
		LoadMessages(NULL,0,NULL);
	}
	
/*	
<log>
<event>
<id>
<user>
<first_name>Employee</first_name> 
<last_name>UserTwo</last_name> 
</user>
<body>54353543</body> 
<time>888</time> 
<show_time>..</show_time> 
<color></color>
</event>
</log>
	*/		
	if(m_pHistoryXML)
	{
		CComPtr<IXMLDOMNode>	pHistoryLogNode	=	NULL;
		m_pHistoryXML->selectSingleNode(CComBSTR(L"log"),&pHistoryLogNode);
		if(pHistoryLogNode)
		{
			CComPtr<IXMLDOMNode> pNewHistoryEventNode	=	NULL;
			insertSingleNode(pHistoryLogNode,CComBSTR(L"event"),NULL,NULL,&pNewHistoryEventNode);
			if(pNewHistoryEventNode)
			{
				WCHAR wsBuf[100];
				_ltow(dwId,wsBuf,10);

				//CComVariant	varId =	(BSTR)wsBuf;

				//SetAttribute(pNewHistoryEventNode,CComBSTR(L"id"),varId);

				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"id"),NULL,CComBSTR(wsBuf));

				CComPtr<IXMLDOMNode> pHistoryEventUserNode	=	NULL;
				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"user"),NULL,NULL,&pHistoryEventUserNode);
				if(pHistoryEventUserNode)
				{
					insertSingleNode(pHistoryEventUserNode,CComBSTR(L"first_name"),NULL,FirstName);
					insertSingleNode(pHistoryEventUserNode,CComBSTR(L"last_name"),NULL,LastName);
				}
				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"body"),NULL,bsMessage);
				
				CTime	Time((time_t)dwTime);
				
				SYSTEMTIME		sysTime		=	{0};
				TCHAR			szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");
				
				Time.GetAsSystemTime(sysTime);
				
				GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysTime,NULL,szDate,MAX_PATH);
				GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysTime,NULL,szTime,MAX_PATH);
				
				CString	strDataFormat;
				strDataFormat.Format(_T("%s %s"),szDate,szTime);
				
				CComBSTR bsStrTime;
				bsStrTime.Attach(strDataFormat.AllocSysString());
				
				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"show_time"),NULL,bsStrTime);
				
				WCHAR wsBuff[100]	=	L"";
				
				_ltow((time_t)dwTime,wsBuff,10);
				
				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"time"),NULL,CComBSTR(wsBuff));

				//WCHAR wsBuff[100];
				swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
				
				insertSingleNode(pNewHistoryEventNode,CComBSTR(L"color"),NULL,CComBSTR(wsBuff));
			}
		}
	}
}

void CChat::RefreshColors(LPCTSTR UserRole, int UserId)
{
	if(m_pHistoryXML)
	{
		// Refresh Messages [8/29/2002]
		CComPtr<IXMLDOMNodeList>	pMessNodeList	=	NULL;
		
		m_pHistoryXML->selectNodes(CComBSTR(L"log/mess"),&pMessNodeList);
		if(pMessNodeList)
		{
			long lMessNodeListLength	=	0;
			pMessNodeList->get_length(&lMessNodeListLength);
			
			for(long lMessNodeIndex = 0 ; lMessNodeIndex <lMessNodeListLength;lMessNodeIndex++)
			{
				CComPtr<IXMLDOMNode>	pMessNode	=	NULL;
				pMessNodeList->get_item(lMessNodeIndex,&pMessNode);
				if(pMessNode)
				{
					CComPtr<IXMLDOMNode>	pMessUserNode	=	NULL;
					
					CheckNodeByPath(pMessNode,CComBSTR(L"user"),&pMessUserNode);
					
					if(pMessUserNode)
					{
						CComVariant	varId;
						GetAttribute(pMessUserNode,CComBSTR(L"id"),&varId);
						if(varId.ChangeType(VT_I4)==S_OK)
						{
							DWORD dwColor = 0;
							if(GetColorFromColorStorage(UserRole, UserId, varId.lVal,dwColor))
							{
								WCHAR wsBuff[100];
								swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
								
								CComPtr<IXMLDOMNode>	pMessColorNode	=	NULL;
								CheckNodeByPath(pMessNode,CComBSTR(L"color"),&pMessColorNode);
								
								if(pMessColorNode)
								{
									pMessColorNode->put_text(CComBSTR(wsBuff));
								}
							}
							else
							{
								ASSERT(FALSE);
							}
						}
					}
				}
			}
		}
		// Refresh Events [8/29/2002]
		CComPtr<IXMLDOMNodeList>	pEventNodeList	=	NULL;
		
		m_pHistoryXML->selectNodes(CComBSTR(L"log/event"),&pEventNodeList);
		if(pEventNodeList)
		{
			long lEventNodeListLength	=	0;
			pEventNodeList->get_length(&lEventNodeListLength);
			
			for(long lEventNodeIndex = 0 ; lEventNodeIndex <lEventNodeListLength;lEventNodeIndex++)
			{
				CComPtr<IXMLDOMNode>	pEventNode	=	NULL;
				pEventNodeList->get_item(lEventNodeIndex,&pEventNode);
				if(pEventNode)
				{
					CComBSTR	bsId;	
					GetTextByPath(pEventNode,CComBSTR(L"id"),&bsId);
					
					DWORD dwColor = 0;

					if(GetColorFromColorStorage(UserRole, UserId, _wtol(bsId),dwColor))
					{
						WCHAR wsBuff[100];
						swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
						
						CComPtr<IXMLDOMNode>	pEventColorNode	=	NULL;
						CheckNodeByPath(pEventNode,CComBSTR(L"color"),&pEventColorNode);
						
						if(pEventColorNode)
						{
							pEventColorNode->put_text(CComBSTR(wsBuff));
						}
					}
					else
					{
						ASSERT(FALSE);
					}
					
				}
			}
		}
	}
}


void CChat::SetInterface(IChat *pChat)
{
	m_pChat	=	pChat;
}
