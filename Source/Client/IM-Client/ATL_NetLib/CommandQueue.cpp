// CommandQueue.cpp: implementation of the CCommandQueue class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "CommandQueue.h"
#include "Session.h"
#include "Message.h"
#include "Messages.h"
#include "Users.h"
#include "User.h"
#include "file.h"
#include "files.h"
#include "promo.h"
#include "promos.h"
#include "sid.h"
#include "sids.h"
#include "ichat.h"
#include "ichats.h"


#include <ATLCONV.H>
//#define AlREADY_IN_QUEUE	2
#define CHECKHR(r) {if(FAILED(r)){hr = r; goto CleanUp;}}

#define INIT_QUEUE_OUT_STR _T("<Queue><SID/>"\
									"<Commands>"\
												"<LogOff/>"\
												"<LogOn/>"\
												"<Messages/>"\
												"<ChangeStatus/>"\
												"<ChatCreate/>"\
												"<ChatStatus/>"\
												"<ChatEdit/>"\
												"<ChatInvite/>"\
												"<ChatAccept/>"\
												"<ChatLeave/>"\
												"<Promos/>"\
												"<AddUser/>"\
												"<LoadPromos/>"\
												"<DelUser/>"\
												"<ConfirmMessages/>"\
												"<ConfirmPromos/>"\
												"<ConfirmFiles/>"\
												"<DelUserR/>"\
												"<AddUserR/>"\
												"<ConfirmFiles/>"\
												"<LoadList/>"\
												"<Search/>"\
												"<Details/>"\
									"</Commands>"\
									"<History>"\
												"<LoadSIDs/>"\
												"<LoadMessages/>"\
									"</History>"\
									"<SendFiles/>"\
									"<ReceiveFiles/>"\
								"</Queue>")

#define INIT_QUEUE_IN_STR _T("<Queue><event/></Queue>")

	
void UnPackString(IXMLDOMNode* pNode, BSTR path,CComBSTR& Value)
{
	CComPtr<IXMLDOMNode> pTempNode;
try
{
	Value.Empty();
	pNode->selectSingleNode(path,&pTempNode);
	if(pTempNode)
	{
		pTempNode->get_text(&Value);

	}
}
catch(...){}
}

void UnPackLong(IXMLDOMNode* pNode, BSTR path,long& Value)
{
	CComPtr<IXMLDOMNode> pTempNode;
	CComBSTR			 text;
try
{
	Value = 0;
	pNode->selectSingleNode(path,&pTempNode);
	if(pTempNode)
	{
		USES_CONVERSION;
		pTempNode->get_text(&text);
		Value =atol(OLE2T(text));

	}
}
catch(...){}
}
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCommandQueue::CCommandQueue()
{
	MCTRACE(1,"CCommandQueue::CCommandQueue() BEGIN");
	::InitializeCriticalSection(&CS_Queue);
	m_Handle			 = 1;
	MCTRACE(1,"CCommandQueue::CCommandQueue() end");
}

CCommandQueue::~CCommandQueue()
{
	::DeleteCriticalSection(&CS_Queue);
}

HRESULT CCommandQueue::InPutQueueInit()
{
	return S_OK;
}
HRESULT CCommandQueue::OutPutQueueInit()
{
	VARIANT_BOOL res;
	HRESULT hr = S_OK;

	///////////////////////////
	// AnswerRoot

	
	if(pAnswerDom == NULL)
	{
		MCTRACE(1,"MSXML.FreeThreadedDOMDocument found hr =%d \r\n",hr);
		CHECKHR(pAnswerDom.CoCreateInstance(CLSID_FreeThreadedDOMDocument40))
			MCTRACE(1,"HRESULT CLSID_FreeThreadedDOMDocument%d \r\n",hr);


		CHECKHR(pAnswerDom->loadXML(CComBSTR(INIT_QUEUE_IN_STR),&res));
		CHECKHR(pAnswerDom->get_firstChild(&pAnswerRootNode));
	}

	///////////////////////////
	// Root
	if(pDom != NULL) {pDom.Release(); pRootNode.Release();};
	
	CHECKHR(pDom.CoCreateInstance(CLSID_FreeThreadedDOMDocument40))

	CHECKHR(pDom->loadXML(CComBSTR(INIT_QUEUE_OUT_STR),&res));
	CHECKHR(pDom->get_firstChild(&pRootNode));

		
#ifdef _DEBUG
		pDom->save(CComVariant("c:\\im\\init_queue_out.xml"));
		pAnswerDom->save(CComVariant("c:\\im\\init_queue_in.xml"));
#endif
CleanUp:
	return hr;
}



DWORD CCommandQueue::x_LogOn(CComBSTR SID, CComBSTR UserName, CComBSTR Password, long Status)
{
	MCTRACE(1,"x_logOn begin");
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pLogonNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	long				 ErrorCode;
	char				 Value[30];
	

	m_bstrLogin.Empty();
	m_bstrLogin.Attach(UserName.Copy());
	m_bstrPassword.Empty();
	m_bstrPassword.Attach(Password.Copy());
	m_Status = Status;

	pRootNode->selectSingleNode(CComBSTR("Commands/LogOn/*"),&pChildNode);
	if(pChildNode) throw(ErrorCode = AlREADY_IN_QUEUE);
	
	pRootNode->selectSingleNode(CComBSTR("SID"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	pChildNode->put_text(SID);
	pChildNode.Release();


	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLogOn));
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_logon"));
	pCommandNode->appendChild(pNode,&pLogonNode);
	pNode.Release();
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("login"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	pChildNode->put_text(UserName);
	pNode.Release();
	pChildNode.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("password"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	pChildNode->put_text(Password);
	pNode.Release();
	pChildNode.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("status"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	ltoa(Status,Value,10);
	pChildNode->put_text(CComBSTR(Value));
	pChildNode.Release();
	pLogonNode.Release();
	pNode.Release();

	pRootNode->selectSingleNode(CComBSTR("Commands/LogOn"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	pChildNode->appendChild(pCommandNode,&pNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	MCTRACE(1,"x_logOn end");
	return m_Handle;
}


bool CCommandQueue::CheckQueueItem(CComBSTR CommandType, IXMLDOMDocument** pComm, DWORD& dwHandle, DWORD& dwType, BSTR *FileName, long* hBackWind, long* size, BSTR *bstrSID)
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pCommandNodeTemp;
	CComPtr<IXMLDOMNode>	pSID;
	CComPtr<IXMLDOMDocument>pCommandDoc;
	CComPtr<IXMLDOMElement> pEle;
	CComVariant				pVar;
	CComBSTR				bstrSTR;
	
	if(pRootNode==NULL)
		return false;

	pRootNode->selectSingleNode(CommandType,&pNode); 
	pRootNode->selectSingleNode(CComBSTR("SID"),&pSID); 
	if(pNode)
	{
		pCommandDoc.CoCreateInstance(CLSID_FreeThreadedDOMDocument40);//?????????????????????????
		
		//Create <packet> && sid
		pCommandDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR("packet"),NULL,&pCommandNodeTemp); 
		pCommandNodeTemp->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		pSID->get_text(&bstrSTR);
		pEle->setAttribute(CComBSTR("sid"),CComVariant(bstrSTR));
		pCommandDoc->appendChild(pCommandNodeTemp,&pCommandNode);
		pCommandNodeTemp.Release();
		pEle.Release();
		pSID.Release();
		
		//Get Handle and Type
		pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		USES_CONVERSION;
   		pEle->getAttribute(CComBSTR("handle"),&pVar);
		dwHandle = atol(OLE2T(pVar.bstrVal));
		
		pEle->getAttribute(CComBSTR("type"),&pVar);
		dwType = atol(OLE2T(pVar.bstrVal));

		pEle->getAttribute(CComBSTR("FileSize"),&pVar);
		if(pVar.vt != VT_NULL)
		*size = atol(OLE2T(pVar.bstrVal));
		
		pEle->getAttribute(CComBSTR("hWnd"),&pVar);
		if(pVar.vt != VT_NULL)
		*hBackWind = atol(OLE2T(pVar.bstrVal));

		pEle->getAttribute(CComBSTR("FilePath"),&pVar);
		if(pVar.vt != VT_NULL)
		*FileName = CComBSTR(pVar.bstrVal).Detach();
		
		if(bstrSID != 0)
			bstrSTR.CopyTo(bstrSID);
		
		//Insert Command
		pNode->get_firstChild(&pCommandNodeTemp);
		pNode.Release();
		pCommandNodeTemp->cloneNode(VARIANT_TRUE,&pNode);
		pCommandNodeTemp.Release();
		pCommandNode->appendChild(pNode,&pCommandNodeTemp);
#ifdef _DEBUG		
		pCommandDoc->save(CComVariant("c:\\im\\command.xml"));
#endif
		*pComm = pCommandDoc.Detach();
			
		return true;
	}
	return false;
}

void CCommandQueue::LockQueue()
{
	::EnterCriticalSection(&CS_Queue);
}

void CCommandQueue::UnlockQueue()
{
	::LeaveCriticalSection(&CS_Queue);
}

DWORD CCommandQueue::x_LogOff()
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	long					ErrorCode;

	pRootNode->selectSingleNode(CComBSTR("Commands/LogOff/*"),&pChildNode);
	if(pChildNode) throw(ErrorCode = AlREADY_IN_QUEUE);
	
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLogOff));
	pEle.Release();
	
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_logoff"));
	pCommandNode->appendChild(pNode,&pChildNode);
	pChildNode.Release();
	pNode.Release();
	pEle.Release();

	pRootNode->selectSingleNode(CComBSTR("Commands/LogOff"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	pChildNode->appendChild(pCommandNode,&pNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

LONG CCommandQueue::DeleteCommand(DWORD handle)
{	
	TCHAR				 szFiltr[100];
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pParrentNode;
	CComBSTR			 bstrFiltr;
	LPCTSTR				 temp[4]= {_T("Commands/*/*[@handle = \"%d\"]"),
								   _T("History/*/*[@handle = \"%d\"]"),
								   _T("ReceiveFiles/*[@handle = \"%d\"]"),
								   _T("SendFiles/*[@handle = \"%d\"]")
								   };

	for(int k=0; k<4; k++)
	{
		bstrFiltr.Empty();
		sprintf(szFiltr,temp[k],handle);
		bstrFiltr = CComBSTR(szFiltr);	
	
		pRootNode->selectSingleNode(bstrFiltr,&pChildNode);
		if(pChildNode) 
		{
		pChildNode->get_parentNode(&pParrentNode);
		pParrentNode->removeChild(pChildNode,&pNode);

#ifdef _DEBUG
		pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
		return k;
		}
	}

	return -1;
}

DWORD CCommandQueue::x_Message(CMessage *pMessage,long& Handle)
{
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pMessageNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pRecipientsNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/Messages"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/message[@mid =\"");
	bstrFiltr += pMessage->m_sMessage.m_MID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pTempNode);
	if(pTempNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmMessage));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_message"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//message
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("message"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("mid"),CComVariant(pMessage->m_sMessage.m_MID));
	pRequestNode->appendChild(pNode,&pMessageNode);
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();

	//Create body Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("body"),NULL,&pNode); 
	pNode->put_text(pMessage->m_sMessage.m_Body);
	pMessageNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	if(pMessage->m_sMessage.m_bChat)
	{
		//CHAT
		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
		pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		pEle->setAttribute(CComBSTR("cid"),CComVariant(pMessage->m_sMessage.m_Chat.m_CID));
		pMessageNode->appendChild(pNode,NULL);
		pNode.Release();
		pEle.Release();
	}
	else
	{
		//recipients
		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("recipients"),NULL,&pNode); 
		pMessageNode->appendChild(pNode,&pRecipientsNode);
		pNode.Release();
		
		long count,lID;
		CComPtr<IUser>  pUser;
		pMessage->m_pRecipients->get_Count(&count);

		if(count == 0) throw(ErrorCode = WRONG_PARAM);
		for(long k=1; k<=count; ++k)
		{
			pMessage->m_pRecipients->get_Item(k,&pUser);
			CComVariant vID;
			pUser->get_Value(CComBSTR(_T("@id")),&vID);
			lID = vID.lVal;

			pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
			pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
			ltoa(lID,Value,10);
			pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
			pRecipientsNode->appendChild(pNode,&pTempNode);
		
			pTempNode.Release();
			pNode.Release();
			pEle.Release();
			pUser.Release();
		}
	}

	pChildNode->appendChild(pCommandNode,&pTempNode);
	if(pTempNode == NULL) throw(ErrorCode = WRONG_QUEUE);
	Handle = m_Handle;
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_SendFile(CFile *pFile, long& Handle)
{
	USES_CONVERSION;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pFileNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pRecipientsNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR;
	DWORD				 ErrorCode;
	char				 Value[30];
	LPTSTR				 FileName = NULL;
	LPTSTR				 FilePath;
	long				 FileSize,k;
	HANDLE 				 hFile;

	pRootNode->selectSingleNode(CComBSTR("SendFiles"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	//File check
	FilePath = OLE2T(pFile->m_sFile.m_RealName);
	hFile = CreateFile(FilePath,GENERIC_READ, FILE_SHARE_READ|FILE_SHARE_WRITE,NULL,OPEN_EXISTING,NULL,NULL);
	if(hFile != INVALID_HANDLE_VALUE)
	{
		FileSize = GetFileSize(hFile,NULL);
		CloseHandle(hFile);
	}
	else
	{
		throw(WRONG_PARAM);
	}
	
	for(k = strlen(FilePath)-2; k>= 0; --k)
	{
		if(FilePath[k] == '\\')
		{
			FileName = new TCHAR[strlen(FilePath)-k];
			strcpy(FileName,FilePath+k+1);
			break;
		}
		
	}
	
	if(FileName == NULL) throw(WRONG_PARAM);

	//command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	Handle = m_Handle;
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmSendFile));
	pEle->setAttribute(CComBSTR("FilePath"),CComVariant(pFile->m_sFile.m_RealName));
	pEle->setAttribute(CComBSTR("hWnd"),CComVariant(pFile->m_sFile.hBackWind));
	pEle->setAttribute(CComBSTR("FileSize"),CComVariant(FileSize));
	pEle.Release();
	
	//request Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_send_file"));
	pCommandNode->appendChild(pNode,&pRequestNode);
//	pCommandNode.Release();
	pNode.Release();
	pEle.Release();


	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("file"),NULL,&pNode); 
	pRequestNode->appendChild(pNode,&pFileNode);
	pRequestNode.Release();
	pNode.Release();

	//RealName
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("real_name"),NULL,&pNode); 
	pNode->put_text(CComBSTR(FileName));
	pFileNode->appendChild(pNode,&pTempNode);
	delete[] FileName;
	pNode.Release();
	pTempNode.Release();

	//Body
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("body"),NULL,&pNode); 
	pNode->put_text(pFile->m_sFile.m_Body);
	pFileNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();
	
	//Size
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("size"),NULL,&pNode); 
	ltoa(FileSize,Value,10);
	pNode->put_text(CComBSTR(Value));
	pFileNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Recipients
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("recipients"),NULL,&pNode); 
	pFileNode->appendChild(pNode,&pRecipientsNode);
	pNode.Release();

	long count,lID;
	CComPtr<IUser>  pUser;
	pFile->m_pRecipients->get_Count(&count);

	if(count == 0) throw(ErrorCode = WRONG_PARAM);
	for(k=1; k<=count; ++k)
	{
		pFile->m_pRecipients->get_Item(k,&pUser);
		CComVariant vID;
		pUser->get_Value(CComBSTR(_T("@id")),&vID);
		lID = vID.lVal;

		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
		pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		ltoa(lID,Value,10);
		pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
		pRecipientsNode->appendChild(pNode,&pTempNode);
	
		pTempNode.Release();
		pNode.Release();
		pEle.Release();
		pUser.Release();
	}

	pChildNode->appendChild(pCommandNode,&pTempNode);
	pTempNode.Release();
	pChildNode.Release();
	pCommandNode.Release();
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_Promo(CPromo *pPromo, long& Handle)
{
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pPromoNode;
	CComPtr<IXMLDOMNode> pProductNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pRecipientsNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/Promos"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/promo[@pid =\"");
	bstrFiltr += pPromo->m_sPromo.m_PID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pTempNode);
	if(pTempNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmPromo));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_promo"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//Promo
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("promo"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("pid"),CComVariant(pPromo->m_sPromo.m_PID));
	pRequestNode->appendChild(pNode,&pPromoNode);
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();

	//Create body Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("body"),NULL,&pNode); 
	pNode->put_text(pPromo->m_sPromo.m_Body);
	pPromoNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Create subject Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("subject"),NULL,&pNode); 
	pNode->put_text(pPromo->m_sPromo.m_Subject);
	pPromoNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Create Product Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("product"),NULL,&pProductNode); 
	pProductNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("id"),CComVariant(pPromo->m_sPromo.m_Product_ID));
	pPromoNode->appendChild(pProductNode,&pTempNode);
	pEle.Release();
	pProductNode.Release();
	pTempNode.Release();
	pNode.Release();

	
	//recipients
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("recipients"),NULL,&pNode); 
	pPromoNode->appendChild(pNode,&pRecipientsNode);
	pNode.Release();
	
	long count,lID;
	CComPtr<IUser>  pUser;
	pPromo->m_pRecipients->get_Count(&count);

	if(count == 0) throw(ErrorCode = WRONG_PARAM);
	for(long k=1; k<=count; ++k)
	{
		pPromo->m_pRecipients->get_Item(k,&pUser);
		CComVariant vID;
		pUser->get_Value(CComBSTR(_T("@id")),&vID);
		lID = vID.lVal;

		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
		pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		ltoa(lID,Value,10);
		pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
		pRecipientsNode->appendChild(pNode,&pTempNode);
	
		pTempNode.Release();
		pNode.Release();
		pEle.Release();
		pUser.Release();
	}

	pChildNode->appendChild(pCommandNode,&pTempNode);
	if(pTempNode == NULL) throw(ErrorCode = WRONG_QUEUE);
	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_ReceiveFile(CFile *pFile, long& Handle)
{
	USES_CONVERSION;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pFileNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pRecipientsNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR;
	DWORD				 ErrorCode;
	LPTSTR				 FileName = NULL;
//	HANDLE 				 hFile;

	pRootNode->selectSingleNode(CComBSTR("ReceiveFiles"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	//command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	Handle = m_Handle;
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmReceiveFile));
	pEle->setAttribute(CComBSTR("FilePath"),CComVariant(pFile->m_sFile.m_RealName));
//	pEle->setAttribute(CComBSTR("fid"),CComVariant(pFile->m_sFile.m_FID));
	pEle->setAttribute(CComBSTR("hWnd"),CComVariant(pFile->m_sFile.hBackWind));
	pEle.Release();
	
	//request Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_receive_file"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();


	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("file"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("fid"),CComVariant(pFile->m_sFile.m_FID));
	pRequestNode->appendChild(pNode,&pFileNode);
	pEle.Release();
	pRequestNode.Release();
	pNode.Release();


	pChildNode->appendChild(pCommandNode,&pTempNode);
	pTempNode.Release();
	pChildNode.Release();
	pCommandNode.Release();
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_ConfirmMessage(BSTR MID)
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pMessageNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR,filtr;
	DWORD				 ErrorCode;

	//command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmConfirmMessage));
	
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_confirm_message"));
	pCommandNode->appendChild(pNode,&pRequestNode);

//	pCommandNode.Release();
	pNode.Release();
	pEle.Release();

	//message
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("message"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("mid"),CComVariant(MID));
	pRequestNode->appendChild(pNode,&pMessageNode);
	
	bstrSTR.Empty();
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();
	
	filtr += CComBSTR("Commands/ConfirmMessages/command/request/message[@mid = \"");
	filtr += MID;
	filtr += CComBSTR("\"]");
	pRootNode->selectSingleNode(filtr,&pTempNode);
	if(pTempNode) throw(ErrorCode = WRONG_QUEUE);

	pRootNode->selectSingleNode(CComBSTR("Commands/ConfirmMessages"),&pNode);
	if(!pNode) throw(ErrorCode = WRONG_QUEUE);
	pNode->appendChild(pCommandNode,&pTempNode);

	return m_Handle;
}

DWORD CCommandQueue::x_ConfirmPromo(BSTR PID)
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pPromoNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR,filtr;
	DWORD				 ErrorCode;

	//command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmConfirmPromo));
	
	pTempNode.Release();
	pNode.Release();
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_confirm_promo"));
	pCommandNode->appendChild(pNode,&pRequestNode);

	pNode.Release();
	pEle.Release();

	//message
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("promo"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("pid"),CComVariant(PID));
	pRequestNode->appendChild(pNode,&pPromoNode);
	
	bstrSTR.Empty();
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();
	
	filtr += CComBSTR("Commands/ConfirmPromos/command/request/promo[@pid = \"");
	filtr += PID;
	filtr += CComBSTR("\"]");
	pRootNode->selectSingleNode(filtr,&pTempNode);
	if(pTempNode) throw(ErrorCode = WRONG_QUEUE);

	pRootNode->selectSingleNode(CComBSTR("Commands/ConfirmPromos"),&pNode);
	if(!pNode) throw(ErrorCode = WRONG_QUEUE);
	pNode->appendChild(pCommandNode,&pTempNode);

	return m_Handle;
}


DWORD CCommandQueue::x_LoadList(ltListType ListType)
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pTempNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR				bstrSTR,filtr;
	DWORD					ErrorCode;
try
{
	switch(ListType)
	{
	case ltContact:
		bstrSTR=CComBSTR(L"contact");
		break;
	case ltIgnore:
		bstrSTR=CComBSTR(L"ignore");
		break;
	case ltFiles:
		bstrSTR=CComBSTR(L"files");
		break;
	case ltChats:
		bstrSTR=CComBSTR(L"chats");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}
	
	//Command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLoadList));

	pEle.Release();

	//Request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_load_list"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	
	pNode.Release();
	pEle.Release();

	//List
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("type"),CComVariant(bstrSTR));
	pRequestNode->appendChild(pNode,&pTempNode);
	
	pNode.Release();
	pEle.Release();
	pTempNode.Release();

	//Check Queue
	filtr += CComBSTR("Commands/LoadList/command/request/list[@type = \"");
	filtr += bstrSTR;
	filtr += CComBSTR("\"]");

	pRootNode->selectSingleNode(filtr,&pChildNode);
	if(pChildNode) 
	{	
		CComBSTR pp;
		pChildNode->get_text(&pp);
		MCTRACE(1,pp);
		throw(ErrorCode = AlREADY_IN_QUEUE);
	}
	//Insert Command
	pRootNode->selectSingleNode(CComBSTR("Commands/LoadList"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	pChildNode->appendChild(pCommandNode,&pTempNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}
catch(DWORD ErrorCode)
{
	throw(ErrorCode);
}
catch(...)
{}
	throw(ErrorCode = UNKNOWN_PROBLEM);
}

DWORD CCommandQueue::x_DeleteUser(long ID, long listtype)
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	char				 Value[30];
	CComBSTR			 bstrSTR;
	long				 ErrorCode;

	pRootNode->selectSingleNode(CComBSTR("Commands/DelUser"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmDelUser));
	pEle.Release();

	//Create Request Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_delete_user"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();
	
	//Create list node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	switch(listtype)
	{
	case ltContact:
		bstrSTR=CComBSTR("contact");
		break;
	case ltIgnore:
		bstrSTR=CComBSTR("ignore");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}
	pEle->setAttribute(CComBSTR("type"),CComVariant(bstrSTR));
	pRequestNode->appendChild(pNode,&pTempNode);
	
	pTempNode.Release();
	bstrSTR.Empty();
	pNode.Release();
	pEle.Release();

	//Create user node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	ltoa(ID,Value,10);
	pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
	pRequestNode->appendChild(pNode,&pTempNode);
	
	pRequestNode.Release();
	pTempNode.Release();
	pNode.Release();
	pEle.Release();
	
	pChildNode->appendChild(pCommandNode,&pTempNode);
	pTempNode.Release();
	pCommandNode.Release();
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

// Add User
DWORD CCommandQueue::x_AddUser(long ID, long listtype, BSTR body)
{
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pWorkNode;
	CComPtr<IXMLDOMElement>	pEle;
	CComBSTR				bstrBody(body);
	char					Value[30];
	CComBSTR				bstrSTR;
	DWORD					ErrorCode;
	

	pRootNode->selectSingleNode(CComBSTR("Commands/AddUser"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmAddUser));
	pEle.Release();

	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_add_user"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();
	
	//Create List Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	switch(listtype)
	{
	case ltContact:
		bstrSTR=CComBSTR("contact");
		break;
	case ltIgnore:
		bstrSTR=CComBSTR("ignore");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}

	pEle->setAttribute(CComBSTR("type"),CComVariant(bstrSTR));
	pRequestNode->appendChild(pNode,&pWorkNode);
	pWorkNode.Release();
	pNode.Release();
	bstrSTR.Empty();
	pEle.Release();

	//Create User Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	ltoa(ID,Value,10);
	pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
	pRequestNode->appendChild(pNode,&pWorkNode);
	pWorkNode.Release();
	pNode.Release();
	pEle.Release();
	
	//Create body Node
	if(listtype == ltContact && body != NULL)
	{
		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("body"),NULL,&pNode); 
		pNode->put_text(body);
		pRequestNode->appendChild(pNode,&pWorkNode);
		pNode.Release();
		pWorkNode.Release();
	}
	
	pChildNode->appendChild(pCommandNode,&pWorkNode);
	pCommandNode.Release();
	pWorkNode.Release();
	pChildNode.Release();

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

//Add User Reply
DWORD CCommandQueue::x_AddUserR(long ID, long AnswerType)
{
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pWorkNode;
	CComPtr<IXMLDOMElement>	pEle;
	char					Value[30];
	CComBSTR				bstrSTR;
	DWORD					ErrorCode;

	pRootNode->selectSingleNode(CComBSTR("Commands/AddUserR"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmAddUserR));
	pEle.Release();

	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_add_user_r"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();
	
	//Create List Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("result"),NULL,&pNode); 
	switch(AnswerType)
	{
	case atAccept:
		bstrSTR=CComBSTR("accept");
		break;
	case atDeny:
		bstrSTR=CComBSTR("deny");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}
	pNode->put_text(bstrSTR);
	pRequestNode->appendChild(pNode,&pWorkNode);
	//pWorkNode.Release();
	pNode.Release();
	bstrSTR.Empty();
	pEle.Release();

	//Create User Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	ltoa(ID,Value,10);
	pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
	pRequestNode->appendChild(pNode,NULL);
	pNode.Release();
	pEle.Release();
	
	pChildNode->appendChild(pCommandNode,NULL);
	pCommandNode.Release();
	pChildNode.Release();
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

//Change Status     ????????????????????????????????????????????????????????
DWORD CCommandQueue::x_ChangeStatus(long Status)
{
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMElement> pEle;
	char				 Value[30];
	CComBSTR			 bstrSTR;
	DWORD				 ErrorCode;


	pRootNode->selectSingleNode(CComBSTR("Commands/ChangeStatus"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChangeStatus));
	pEle.Release();

	//Create request Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_change_status"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();
	
	//Create Status Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("status"),NULL,&pNode); 
	ltoa(Status,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,&pTempNode);
	pTempNode.Release();
	pNode.Release();
	
	pChildNode->get_firstChild(&pNode);
	pChildNode->insertBefore(pCommandNode,CComVariant(pNode),&pTempNode);
	LastStatus = Status;	
	pTempNode.Release();
	pChildNode.Release();
	pCommandNode.Release();

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}


//Confirm File
DWORD CCommandQueue::x_ConfirmFile(BSTR FID, long Flag)
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pFileNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR			 bstrSTR,filtr;
	char				 Value[30];
	long				 ErrorCode;

	//command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmConfirmFile));
	
	pTempNode.Release();
	pNode.Release();
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_confirm_file"));
	pCommandNode->appendChild(pNode,&pRequestNode);

	//pCommandNode.Release();
	pNode.Release();
	pEle.Release();

	//file
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("file"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("fid"),CComVariant(FID));
	pRequestNode->appendChild(pNode,&pFileNode);
	
	pFileNode.Release();
	//pRequestNode.Release();
	pNode.Release();
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("result"),NULL,&pNode); 
	switch(Flag)
	{
	case fcDelete:
		bstrSTR = CComBSTR(L"delete");
		break;
	case fcDoOffline:
		bstrSTR = CComBSTR(L"offline");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
		break;
	}
	ltoa(Flag,Value,10);
	pNode->put_text(bstrSTR);
	pRequestNode->appendChild(pNode,&pFileNode);
	
	pFileNode.Release();
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();
	
	filtr += CComBSTR("Commands/ConfirmFiles/command/request/file[@fid = \"");
	filtr += FID;
	filtr += CComBSTR("\"]");
	pRootNode->selectSingleNode(filtr,&pTempNode);
	if(pTempNode) throw(ErrorCode = WRONG_QUEUE);

	pRootNode->selectSingleNode(CComBSTR("Commands/ConfirmFiles"),&pNode);
	if(!pNode) throw(ErrorCode = WRONG_QUEUE);
	pNode->appendChild(pCommandNode,&pTempNode);
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}


DWORD CCommandQueue::x_DeleteUserR(long ID)
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pWorkNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	char				 Value[30];
	CComBSTR			 bstrSTR;
	DWORD				 ErrorCode;

	pRootNode->selectSingleNode(CComBSTR("Commands/AddUserR"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmDelUserR));
	pChildNode->appendChild(pNode,&pCommandNode);
	pChildNode.Release();
	pNode.Release();
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_del_user_r"));
	pCommandNode->appendChild(pNode,&pWorkNode);
	pCommandNode.Release();
	pNode.Release();
	pEle.Release();
	
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	ltoa(ID,Value,10);
	pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
	pWorkNode->appendChild(pNode,NULL);

	
	pNode.Release();
	pEle.Release();
#ifdef _DEBUG	
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}
DWORD CCommandQueue::x_LastPromos(long Count)
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pTempNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR				bstrSTR,filtr;
	DWORD					ErrorCode;
	char					Value[30];
try
{
	//Command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLoadList));
	pEle.Release();

	//Request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_load_list"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	
	pNode.Release();
	pEle.Release();

	//List
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("type"),CComVariant(CComBSTR("promos")));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pEle.Release();
	pTempNode.Release();

	//Count
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("count"),NULL,&pNode); 
	ltoa(Count,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Check Queue
	filtr = CComBSTR("Commands/LoadList/command/request/list[@type = \"promos\"]");

	pRootNode->selectSingleNode(filtr,&pChildNode);
	if(pChildNode) 
	{	
		CComBSTR pp;
		pChildNode->get_text(&pp);
//		MCTRACE((char*)pp);
		throw(ErrorCode = AlREADY_IN_QUEUE);
	}
	//Insert Command
	pRootNode->selectSingleNode(CComBSTR("Commands/LoadList"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	pChildNode->appendChild(pCommandNode,&pTempNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}
catch(DWORD ErrorCode)
{
	throw(ErrorCode);
}
catch(...)
{}
	throw(ErrorCode = UNKNOWN_PROBLEM);
}

DWORD CCommandQueue::x_UserDetails(long ID, long type)
{
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pWorkNode;
	CComPtr<IXMLDOMElement>	pEle;
	char					Value[30];
	CComBSTR				bstrSTR;
	DWORD					ErrorCode;

	pRootNode->selectSingleNode(CComBSTR("Commands/Details"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmDetails));
	pEle.Release();

	//Create Command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_details"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();
	
	//Create List Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("info_type"),NULL,&pNode); 
	switch(type)
	{
	case 1:
		bstrSTR=CComBSTR("short");
		break;
	case 2:
		bstrSTR=CComBSTR("full");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}
	pNode->put_text(bstrSTR);
	pRequestNode->appendChild(pNode,&pWorkNode);
	pWorkNode.Release();
	pNode.Release();
	bstrSTR.Empty();
	pEle.Release();

	//Create User Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	ltoa(ID,Value,10);
	pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
	pRequestNode->appendChild(pNode,&pWorkNode);
	pWorkNode.Release();
	pNode.Release();
	pEle.Release();
	
	pChildNode->appendChild(pCommandNode,&pWorkNode);
	pCommandNode.Release();
	pWorkNode.Release();
	pChildNode.Release();

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_SearchUser(IUser *pUser)
{
	return m_Handle;
}

DWORD CCommandQueue::x_LoadSIDs(long From, long To)
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pTempNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR				bstrSTR,filtr;
	DWORD					ErrorCode;
	char					Value[30];
try
{
	//Command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLoadList));
	pEle.Release();

	//Request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_load_list"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	
	pNode.Release();
	pEle.Release();

	//List
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("type"),CComVariant(CComBSTR("sessions")));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pEle.Release();
	pTempNode.Release();

	//from
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("from"),NULL,&pNode); 
	ltoa(From,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//to
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("to"),NULL,&pNode); 
	ltoa(To,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Check Queue
	filtr = CComBSTR("Commands/LoadList/command/request/list[@type = \"sessions\"]");

	pRootNode->selectSingleNode(filtr,&pChildNode);
	if(pChildNode) 
	{	
		CComBSTR pp;
		pChildNode->get_text(&pp);
//		MCTRACE(pp);
		throw(ErrorCode = AlREADY_IN_QUEUE);
	}
	//Insert Command
	pRootNode->selectSingleNode(CComBSTR("Commands/LoadList"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	pChildNode->appendChild(pCommandNode,&pTempNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}
catch(DWORD ErrorCode)
{
	throw(ErrorCode);
}
catch(...)
{}
	throw(ErrorCode = UNKNOWN_PROBLEM);
}


DWORD CCommandQueue::x_LoadMessages(BSTR SID)
{
	CComPtr<IXMLDOMNode>	pNode;
	CComPtr<IXMLDOMNode>	pRequestNode;
	CComPtr<IXMLDOMNode>	pTempNode;
	CComPtr<IXMLDOMNode>	pCommandNode;
	CComPtr<IXMLDOMNode>	pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	CComBSTR				bstrSTR,filtr;
	DWORD					ErrorCode;
	//Command
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLoadList));
	pEle.Release();

	//Request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_load_list"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	
	pNode.Release();
	pEle.Release();

	//List
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("list"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("type"),CComVariant(CComBSTR("messages")));
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pEle.Release();
	pTempNode.Release();

	//from
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("sid"),NULL,&pNode); 

	pNode->put_text(SID);
	pRequestNode->appendChild(pNode,&pTempNode);
	pNode.Release();
	pTempNode.Release();

	//Check Queue
	filtr = CComBSTR("Commands/LoadList/command/request/list[@type = \"messages\"]");

	pRootNode->selectSingleNode(filtr,&pChildNode);
	if(pChildNode) 
	{	
		CComBSTR pp;
		pChildNode->get_text(&pp);
//		MCTRACE(pp);
		throw(ErrorCode = AlREADY_IN_QUEUE);
	}
	//Insert Command
	pRootNode->selectSingleNode(CComBSTR("Commands/LoadList"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);
	
	pChildNode->appendChild(pCommandNode,&pTempNode);

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

DWORD CCommandQueue::x_LogOn_Inter()
{
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pLogonNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMElement> pEle;
	DWORD				 ErrorCode;
	char				 Value[30];
	
	pRootNode->selectSingleNode(CComBSTR("Commands/LogOn/*"),&pChildNode);
	if(pChildNode) throw(ErrorCode = AlREADY_IN_QUEUE);
	
	pRootNode->selectSingleNode(CComBSTR("Commands/LogOn"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmLogOn));
	pChildNode->appendChild(pNode,&pCommandNode);
	pChildNode.Release();
	pNode.Release();
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_logon"));
	pCommandNode->appendChild(pNode,&pLogonNode);
	pCommandNode.Release();
	pNode.Release();
	pEle.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("login"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	pChildNode->put_text(m_bstrLogin);
	pNode.Release();
	pChildNode.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("password"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	pChildNode->put_text(m_bstrPassword);
	pNode.Release();
	pChildNode.Release();

	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("status"),NULL,&pNode); 
	pLogonNode->appendChild(pNode,&pChildNode);
	ltoa(m_Status,Value,10);
	pChildNode->put_text(CComBSTR(Value));
	
#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

//Add new answer node to end of Answer Queue
void CCommandQueue::AddAnswer(long Handle, long Type, IXMLDOMNode *pNode)
{
	CComPtr<IXMLDOMNode> pInComingNode;
	CComPtr<IXMLDOMNode> pTempNode;
	CComPtr<IXMLDOMNode> pAnswerNode;
	CComPtr<IXMLDOMElement> pEle;
	
	pNode->cloneNode(VARIANT_TRUE,&pInComingNode);
	

	pAnswerDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("answer"),NULL,&pTempNode); 
	pTempNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(Type));
	pAnswerRootNode->appendChild(pTempNode,&pAnswerNode);
	pTempNode.Release();
	pEle.Release();

	//Append node to end of queue
	pAnswerNode->appendChild(pInComingNode,&pTempNode);
#ifdef _DEBUG	
	pAnswerDom->save(CComVariant("c:\\im\\answer.xml"));
#endif
}

//Add new event node to end of Event Queue
void CCommandQueue::AddEvent(IXMLDOMNode *pNode)
{
	CComPtr<IXMLDOMNode>	pInComingNode;
	CComPtr<IXMLDOMNode>	pTempNode;
	CComPtr<IXMLDOMNode>	pAnswerNode;
	
	pNode->cloneNode(VARIANT_TRUE,&pInComingNode);
	if(pInComingNode == NULL) return;

	pAnswerDom->selectSingleNode(CComBSTR(L"Queue/event"),&pTempNode);
	if(pTempNode == NULL) return;
	
	//Append Child to end of Queue
	pTempNode->appendChild(pInComingNode,&pAnswerNode);
	pTempNode.Release();
#ifdef _DEBUG	
	pAnswerDom->save(CComVariant("c:\\im\\event.xml"));
#endif
}

/////////////
void CCommandQueue::UnPackUser(sUser& user, CComPtr<IXMLDOMNode> pUserNode)
{
	UnPackString(pUserNode,CComBSTR(L"nick_name"),user.m_UserName);
	UnPackString(pUserNode,CComBSTR(L"first_name"),user.m_FirstName);
	UnPackString(pUserNode,CComBSTR(L"last_name"),user.m_LastName);
	UnPackString(pUserNode,CComBSTR(L"role/name"),user.m_Role);
	UnPackString(pUserNode,CComBSTR(L"email"),user.m_EMail);
	UnPackLong(pUserNode,CComBSTR(L"status"),user.m_Status);
	UnPackLong(pUserNode,CComBSTR(L"@id"),user.m_ID);
	UnPackLong(pUserNode,CComBSTR(L"role/@id"),user.m_Role_ID);
	UnPackLong(pUserNode,CComBSTR(L"time"),user.m_time);
//	UnPackString(pUserNode,CComBSTR(L"company/name"),user.m_Company);
//	UnPackLong(pUserNode,CComBSTR(L"company/@id"),user.m_Company_ID);
}

void CCommandQueue::UnPackPromo(sPromo& promo, CComPtr<IXMLDOMNode> pPNode)
{
	CComPtr<IXMLDOMNode>	pUserNode;

	UnPackString(pPNode,CComBSTR(L"body"),promo.m_Body);
	UnPackString(pPNode,CComBSTR(L"subject"),promo.m_Subject);
	UnPackString(pPNode,CComBSTR(L"@pid"),promo.m_PID);
	UnPackString(pPNode,CComBSTR(L"sid"),promo.m_SID);
	UnPackString(pPNode,CComBSTR(L"product/name"),promo.m_ProductName);
	UnPackLong(pPNode,CComBSTR(L"product/@id"),promo.m_Product_ID);
	UnPackLong(pPNode,CComBSTR(L"time"),promo.m_Time);
	
	pPNode->selectSingleNode(CComBSTR(L"user"),&pUserNode);
	if(pUserNode)
	UnPackUser(promo.m_sSender,pUserNode);
}

void CCommandQueue::UnPackMessage(sMessage& message, CComPtr<IXMLDOMNode> pMNode)
{
 	CComPtr<IXMLDOMNode>	pUserNode;

	UnPackString(pMNode,CComBSTR(L"body"),	message.m_Body);
	UnPackString(pMNode,CComBSTR(L"@mid"),	message.m_MID);
	UnPackLong(pMNode,CComBSTR(L"@id"),	message.m_nMID);
	UnPackString(pMNode,CComBSTR(L"sid"),	message.m_SID);
	UnPackLong	(pMNode,CComBSTR(L"time"),	message.m_Time);
	
	pMNode->selectSingleNode(CComBSTR(L"user"),&pUserNode);
	if(pUserNode != NULL)
	{
		UnPackUser(message.m_Sender,pUserNode);
		pUserNode.Release();
	}

	pMNode->selectSingleNode(CComBSTR(L"chat"),&pUserNode);
	if(pUserNode != NULL)
	{
		UnPackChat(message.m_Chat,pUserNode);
		pUserNode.Release();
	}
}

void CCommandQueue::UnPackChat(LONG Handle, IChat** pChat)
{

	LPTSTR				 szTemplate = _T("answer[@handle = \"%d\"]");
	TCHAR				 szFiltr[100];
	sprintf(szFiltr,szTemplate,Handle);
	CComBSTR			 filtr(szFiltr);

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pChatNode;

	HRESULT				 hr = 0;
	CChat*				 m_pChat;	
	
	//Find Handle
	pAnswerRootNode->selectSingleNode(filtr,&pNode);
	if(pNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	filtr.Empty();
	
	
	pNode->selectSingleNode(CComBSTR("packet/response/chat"), &pChatNode);
	if(pChatNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	hr = CChat::CreateInstance(pChat);
	m_pChat = static_cast<CChat*>(*pChat);
	m_pChat->m_pSession = (CSession*)lpSession;

	if(FAILED(hr)) {ATLASSERT(FALSE); return;}
	UnPackChat(m_pChat->m_sChat,pChatNode);
}

void CCommandQueue::UnPackChat(sChat& chat, CComPtr<IXMLDOMNode> pNode)
{
	UnPackString(pNode,CComBSTR(L"name"),	chat.m_Name);
	UnPackString(pNode,CComBSTR(L"descr"),	chat.m_Descr);
	UnPackString(pNode,CComBSTR(L"@cid"),	chat.m_CID);
	UnPackLong	(pNode,CComBSTR(L"time"),	chat.m_CreationTime);
	UnPackLong	(pNode,CComBSTR(L"owner"),	chat.m_Creator);
}

void CCommandQueue::UnPackFile(sFile& file, CComPtr<IXMLDOMNode> pNode)
{
	CComPtr<IXMLDOMNode>	pUserNode;

	UnPackString(pNode,CComBSTR(L"body"),	file.m_Body);
	UnPackString(pNode,CComBSTR(L"real_name"),file.m_RealName);
	UnPackString(pNode,CComBSTR(L"@fid"),	file.m_FID);
	UnPackString(pNode,CComBSTR(L"sid"),	file.m_SID);
	UnPackLong	(pNode,CComBSTR(L"time"),	file.m_Time);
	UnPackLong	(pNode,CComBSTR(L"size"),	file.m_size);

	pNode->selectSingleNode(CComBSTR(L"user"),&pUserNode);
	if(pUserNode)
	UnPackUser(file.m_sSender,pUserNode);
}

void CCommandQueue::UnPackSession(slocalSID& session, CComPtr<IXMLDOMNode> pNode)
{
	UnPackString(pNode,CComBSTR(L"sid"),	session.m_SID);
	UnPackLong(pNode,CComBSTR(L"count"),	session.m_Count);
}
void CCommandQueue::UnPackSelfInfo(sUser& pUser, DWORD handle)
{
	char				 Value[30];
	CComPtr<IXMLDOMNode> pPacketNode;
	CComPtr<IXMLDOMNode> pUserNode;
	CComBSTR			 filtr;

	ltoa(handle,Value,10);
	CComBSTR	bstrHandle(Value);

	filtr += CComBSTR("answer[@handle = \"");
	filtr += bstrHandle;
	filtr += CComBSTR("\"]");
	
	pAnswerRootNode->selectSingleNode(filtr,&pPacketNode);
	if(pPacketNode == NULL) 
	{	
		//ATLASSERT(FALSE);
		return;
	}

	pPacketNode->selectSingleNode(CComBSTR(L"packet/response/user"),&pUserNode);
	if(pUserNode == NULL) 
	{	
		//ATLASSERT(FALSE);
		return;
	}
	UnPackUser(pUser,pUserNode);
}

BOOL CCommandQueue::DeleteAnswer(DWORD handle)
{
	char				 Value[30];
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pParrentNode;
	
	ltoa(handle,Value,10);
	CComBSTR			bstrHandle(Value);
	CComBSTR			filtr;
	

	filtr += CComBSTR("answer[@handle = \"");
	filtr += bstrHandle;
	filtr += CComBSTR("\"]");
	pAnswerRootNode->selectSingleNode(filtr,&pChildNode);
	if(pChildNode) 
	{
		filtr.Empty();
		pChildNode->get_text(&filtr);
		MCTRACE(1,"remove %s \r\n",filtr);
		pChildNode->get_parentNode(&pParrentNode);
		pParrentNode->removeChild(pChildNode,&pNode);
#ifdef _DEBUG
		pAnswerDom->save(CComVariant("c:\\im\\answer.xml"));
#endif
		return true;
	}
	return false;
}

void CCommandQueue::UnPackList(IUnknown **pList, LPVOID pWorkClass, WPARAM handle, long& outListType, BSTR* pString)
{
	LPTSTR				 szTemplate = _T("answer[@handle = \"%d\"]");
	TCHAR				 szFiltr[100];
	sprintf(szFiltr,szTemplate,handle);
	CComBSTR			 filtr(szFiltr);

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pNodeUser;
	CComPtr<IXMLDOMNode> pListTypeNode;
	CComPtr<IXMLDOMNode> pListNode;
	CComPtr<IXMLDOMNode> pListItemNode;
	
	CComBSTR			 ListType;
	HRESULT				 hr = 0;

	
	//Find Handle
	pAnswerRootNode->selectSingleNode(filtr,&pNode);
	if(pNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	filtr.Empty();
	
	
	//Find response
	pNode->selectSingleNode(CComBSTR(L"packet/response/list"),&pListTypeNode);
	if(pListTypeNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	pNode.Release();
	pListTypeNode->get_text(&ListType);

	//Get ListType
 	pListTypeNode->get_previousSibling(&pListNode);
	if(pListNode == NULL)
	{
		pListTypeNode->get_nextSibling(&pListNode);
		if(pListNode == NULL)
		{
			ATLASSERT(FALSE);
			return;
		}
	}

	outListType = 0;
	if(ListType == CComBSTR(L"contact"))	outListType = ltContact;
	if(ListType == CComBSTR(L"ignore"))		outListType = ltIgnore;
	if(ListType == CComBSTR(L"files"))		outListType = ltFiles;
	if(ListType == CComBSTR(L"messages"))	outListType = ltMessages;
	if(ListType == CComBSTR(L"promos"))		outListType = ltPromos;
	if(ListType == CComBSTR(L"sessions"))	outListType = ltSIDs;
	if(ListType == CComBSTR(L"chats"))		outListType = ltChats;

	pListNode->get_xml(pString);

	pListNode->get_firstChild(&pListItemNode);
	if(pListItemNode == NULL)
	return;


	CComPtr<IUsers>		pUsers;
	CComPtr<IUser>		pUser;
	CComPtr<IUser>		pFromUser;
	CUser*				pcUser;
	CComPtr<IMessages>	pMessages;
	CComPtr<IMessage>	pMessage;
	CMessage*			pcMessage;
	CComPtr<IFiles>		pFiles;
	CComPtr<IFile>		pFile;
	CFile*				pcFile;
	CComPtr<IPromos>	pPromos;
	CComPtr<IPromo>		pPromo;
	CPromo*				pcPromo;
	CComPtr<IlocalSIDs> plocalSIDs;
	CComPtr<IlocalSID>  plocalSID;
	ClocalSID*			pclocalSID;
	CComPtr<IChats>		pChats;
	CComPtr<IChat>		pChat;
	CChat*				pcChat;

	switch(outListType)
	{
	case ltContact:
	case ltIgnore:
			hr = CUsers::CreateInstance(&pUsers);
			if(hr == 0)
			{
				do
				{
				pUsers->AddUser(&pUser);
				pcUser = static_cast<CUser*>((LPVOID)pUser);
				UnPackUser(pcUser->m_sUser,pListItemNode);
				pUser.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}
			(*pList) = pUsers.Detach();
		return;

	case ltFiles:
			hr = CFiles::CreateInstance(&pFiles);
			if(hr == 0)
			{
				do
				{
				pFiles->AddFile(&pFile);
				pcFile = static_cast<CFile*>((LPVOID)pFile);
				UnPackFile(pcFile->m_sFile,pListItemNode);
				pcFile->m_pSession = (CSession*)lpSession;
				pFile.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}
			(*pList) = pFiles.Detach();
		return;
	
	case ltMessages:
			hr = CMessages::CreateInstance(&pMessages);
			if(hr == 0)
			{
				do
				{
				pMessages->AddMessage(&pMessage);
				pcMessage = static_cast<CMessage*>((LPVOID)pMessage);
				UnPackMessage(pcMessage->m_sMessage,pListItemNode);
				
				pListItemNode->selectSingleNode(CComBSTR("recipients/user"),&pNodeUser);
				if(pNodeUser!=NULL)
				{
					pcMessage->m_pRecipients->AddUser(&pUser);
					pcUser = static_cast<CUser*>((LPVOID)pUser);
					UnPackUser(pcUser->m_sUser,pNodeUser);
					pNodeUser.Release();
					pUser.Release();
				}


				pMessage.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}

			(*pList) = pMessages.Detach();
		return;

	case ltSIDs:		
			hr = ClocalSIDs::CreateInstance(&plocalSIDs);
			if(hr == 0)
			{
				do
				{
				plocalSIDs->AddSID(&plocalSID);
				pclocalSID = static_cast<ClocalSID*>((LPVOID)plocalSID);
				UnPackSession(pclocalSID->m_slocalSID,pListItemNode);
				plocalSID.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}
			(*pList) = plocalSIDs.Detach();
		return;

	case ltPromos:
			hr = CPromos::CreateInstance(&pPromos);
			if(SUCCEEDED(hr))
			{
				do
				{
				pPromos->AddPromo(&pPromo);
				pcPromo = static_cast<CPromo*>((LPVOID)pPromo);
				UnPackPromo(pcPromo->m_sPromo,pListItemNode);
				pPromo.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}
			(*pList) = pPromos.Detach();
		return;

	case ltChats:
		hr = CChats::CreateInstance(&pChats);
			if(SUCCEEDED(hr))
			{
				do
				{
				pChats->AddChat(&pChat);
				pcChat = static_cast<CChat*>((LPVOID)pChat);
				pcChat->m_pSession = (CSession*)lpSession;
				UnPackChat(pcChat->m_sChat,pListItemNode);
				pChat.Release();

				pListItemNode->get_nextSibling(&pNode);
				pListItemNode.Release();
				pListItemNode = pNode;
				pNode.Release();
				}
				while(pListItemNode);
			}
			(*pList) = pChats.Detach();
		return;
	}

	ATLASSERT(FALSE);
}

void CCommandQueue::GetSID(TCHAR* szSID)
{
	CComBSTR				bstrSID;
	CComPtr<IXMLDOMNode>	pSID;
	pRootNode->selectSingleNode(CComBSTR("SID"),&pSID); 
try
{
	USES_CONVERSION;	
	pSID->get_text(&bstrSID);
	ATLASSERT(bstrSID.Length() == 36);
	strcpy(szSID,OLE2T(bstrSID));
}
catch(...)
{
	Beep(1000,10);
}
}

void CCommandQueue::UnPackEvents(void *pWorkClass)
{
	USES_CONVERSION;
	CSession*				pSession;
	pSession = (CSession*)	pWorkClass;

	CComPtr<IMessage>				pMessage = NULL;
	CComPtr<IFile>					pFile	= NULL;
	CComPtr<IPromo>					pPromo	= NULL;
	CComPtr<IUser>					pUser	= NULL;
	CComPtr<IUser>					pFromUser	= NULL;
	CComPtr<IChat>					pChat	= NULL;

	HRESULT					hr;
	CComBSTR				bstrSID,bstrEvent,PromoAsString;
	CComPtr<IXMLDOMNode>	pNode,pEventNode,pPacketNode,pTempNode,pEnd;
	CComPtr<IXMLDOMNode>	pEventRootNode;
	
	pAnswerDom->selectSingleNode(CComBSTR(L"Queue/event"),&pEventRootNode);
	
	if(pEventRootNode == NULL) {ATLASSERT(FALSE); return;}
	
	pEventRootNode->selectSingleNode(CComBSTR(L"root"),&pNode);
	while (pNode != NULL)
	{
		pNode->selectSingleNode(CComBSTR(L"packet"),&pPacketNode );
		while (pPacketNode != NULL)
		{
			pPacketNode->selectSingleNode(CComBSTR(L"event"),&pEventNode);
			while(pEventNode != NULL)
			{
				pEventNode->get_firstChild(&pEnd);
				if(pEnd)
				{
					pEnd->get_nodeName(&bstrEvent);
					
					//New Message
					if(bstrEvent == CComBSTR(L"message"))
					{
						CMessage* m_pMessage;
						CMessage::CreateInstance(&pMessage);
						m_pMessage = static_cast<CMessage*>(pMessage.p);
						UnPackMessage(m_pMessage->m_sMessage,pEnd);
						x_ConfirmMessage(m_pMessage->m_sMessage.m_MID);
						try
						{
						MCTRACE(0,"Fire_eMessage");
						hr = pSession->Fire_eMessage(pMessage);
						}catch(...){};
						pMessage.Release();
						pMessage = NULL;
						bstrEvent.Empty();
					}
					
					if(bstrEvent == CComBSTR(L"chat_message"))
					{
						CMessage* m_pMessage;
						CMessage::CreateInstance(&pMessage);
						m_pMessage = static_cast<CMessage*>(pMessage.p);
						UnPackMessage(m_pMessage->m_sMessage,pEnd);
						
						CChat* m_pChat;
						CChat::CreateInstance(&pChat);
						m_pChat = static_cast<CChat*>(pChat.p);
						m_pChat->m_sChat.m_CID = m_pMessage->m_sMessage.m_Chat.m_CID;

						try
						{
							MCTRACE(0,"Fire_eMessage");
							hr = pSession->Fire_eChatMessage(pChat,pMessage);
						}catch(...){};
						pMessage.Release();
						pChat.Release();
						pMessage = NULL;
						bstrEvent.Empty();
					}

					//New Promo
					if(bstrEvent == CComBSTR(L"promo"))
					{
						PromoAsString.Empty();
						pEnd->get_xml(&PromoAsString);
						CPromo* m_pPromo;
						CPromo::CreateInstance(&pPromo);
						m_pPromo = static_cast<CPromo*>(pPromo.p);
						UnPackPromo(m_pPromo->m_sPromo,pEnd);
						x_ConfirmPromo(m_pPromo->m_sPromo.m_PID);
						MCTRACE(0,"Fire_ePromo");
						pSession->Fire_ePromo(pPromo,PromoAsString);
						pPromo.Release();
						pPromo = NULL;
						bstrEvent.Empty();
					}
					//New File
					if(bstrEvent == CComBSTR(L"file"))
					{
						CFile* m_pFile;
						CFile::CreateInstance(&pFile);
						m_pFile = static_cast<CFile*>(pFile.p);
						m_pFile->m_pSession = (CSession*)pWorkClass;
						UnPackFile(m_pFile->m_sFile,pEnd);
						try
						{
						MCTRACE(0,"Fire_eFile");
						hr = pSession->Fire_eFile(pFile);
						}
						catch(...){};
						pFile.Release();
						pFile = NULL;
					}
					//User Status
					if(bstrEvent == CComBSTR(L"user"))
					{
						
						CUser*				 m_pUser   = NULL;
						CUser::CreateInstance(&pUser);
						m_pUser = static_cast<CUser*>(pUser.p);
						UnPackUser(m_pUser->m_sUser,pEnd);
						MCTRACE(0,"Fire_eChangedStatus");
						hr = pSession->Fire_eChangedStatus(pUser);
						pUser.Release();
						pUser = NULL;

					}
					//New AddUser
					if(bstrEvent == CComBSTR(L"adduser"))
					{
						CComPtr<IXMLDOMNode> pUserNode = NULL;
						CComBSTR			 bstrBody;
						CUser*				 m_pUser   = NULL;

						pEnd->selectSingleNode(CComBSTR(L"user"),&pUserNode);
						if(pUserNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pUserNode);
							pUserNode.Release();

							pEnd->selectSingleNode(CComBSTR(L"body"),&pUserNode);
							if(pUserNode)
							{
								pUserNode->get_text(&bstrBody);
								pUserNode.Release();
							}
							MCTRACE(0,"Fire_eAdd");
							hr = pSession->Fire_eAdd(pUser,bstrBody);
							pUser.Release();
							pUser = NULL;
							
						}
					}
					//New AddUserR
					if(bstrEvent == CComBSTR(L"adduserr"))
					{
						CComPtr<IXMLDOMNode> pUserNode = NULL;
						CComBSTR			 bstrResult;
						CUser*				 m_pUser   = NULL;

						pEnd->selectSingleNode(CComBSTR(L"user"),&pUserNode);
						if(pUserNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pUserNode);
							pUserNode.Release();

							x_DeleteUserR(m_pUser->m_sUser.m_ID);
							pEnd->selectSingleNode(CComBSTR(L"result"),&pUserNode);
							if(pUserNode)
							{
								pUserNode->get_text(&bstrResult);
								if(bstrResult == CComBSTR(L"1") ||
								   bstrResult == CComBSTR(L"3") )
								{	
									MCTRACE(0,"Fire_eAddR");
									hr = pSession->Fire_eAddR(pUser,atAccept);
								}
								else
								if(bstrResult == CComBSTR(L"2"))
								{
									MCTRACE(0,"Fire_eAddR");
									hr = pSession->Fire_eAddR(pUser,atDeny);
								}
								pUserNode.Release();
							}
							
							pUser.Release();
							pUser = NULL;
						}
						
					}
					//New reklama
					if(bstrEvent == CComBSTR(L"reklama"))
					{
						CComPtr<IXMLDOMNode> pURLNode;
						CComBSTR			 bstrURL;
						pEnd->selectSingleNode(CComBSTR(L"url"),&pURLNode);
						if(pURLNode)
						{
							pURLNode->get_text(&bstrURL);
							pURLNode.Release();
							MCTRACE(0,"bstrURL");
							hr = pSession->Fire_eReklama(bstrURL);
							bstrURL.Empty();
						}
						
					}

					//New SysMess
					if(bstrEvent == CComBSTR(L"system"))
					{
						CComPtr<IXMLDOMNode> pCodeNode;
						LONG				 lnCode;
						CComBSTR			 bstrCode;
						
						pEnd->selectSingleNode(CComBSTR(L"code"),&pCodeNode);
						if(pCodeNode)
						{
							pCodeNode->get_text(&bstrCode);
							lnCode = atol(OLE2T(bstrCode));
							pCodeNode.Release();
							bstrCode.Empty();

							pEnd->selectSingleNode(CComBSTR(L"descr"),&pCodeNode);
							if(pCodeNode)
							{
								pCodeNode->get_text(&bstrCode);
							}
							CSession* pSess;
							switch(lnCode)
							{
							case 1: //UpdateGroup
								try{x_LoadList(ltContact);}catch(...){}
								pSess = (CSession*)lpSession;
								pSess->m_IM_NET->SetNewCommand();
								break;
							case 2: //UpdateUser
								hr = pSession->Fire_eSysMess(lnCode,bstrCode);
								//pSess->m_IM_NET->m_WaitForReconnect = TRUE;
								//pSess->m_IM_NET->m_BaseNetManager.StopAllOperations();
								break;
							case 3://UpdateWebStub
								hr = pSession->Fire_eSysMess(lnCode,bstrCode);
								break;
							default:
								hr = pSession->Fire_eSysMess(lnCode,bstrCode);
								break;
							}
							MCTRACE(0,"System Message %d Description %s",lnCode,OLE2T(bstrCode));
							
							
							pCodeNode.Release();
							bstrCode.Empty();
						}
						
					}
					
					if(bstrEvent == CComBSTR(L"chat_status"))
					{
						CComPtr<IXMLDOMNode> pNode = NULL;
						CUser*				 m_pUser   = NULL;
						CChat*				 m_pChat   = NULL;

						pEnd->selectSingleNode(CComBSTR(L"user"),&pNode);
						if(pNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pNode);
							pNode.Release();
						}
						
						pEnd->selectSingleNode(CComBSTR(L"chat"),&pNode);
						if(pNode)
						{
							CChat::CreateInstance(&pChat);
							m_pChat = static_cast<CChat*>(pChat.p);
							UnPackChat(m_pChat->m_sChat,pNode);
							pNode.Release();
							m_pChat->m_pSession = pSession;
						}

						if(pChat && pUser)
						{
							MCTRACE(0,"Fire_eChatStatus");
							hr = pSession->Fire_eChatUserStatus(pUser,pChat);
						}
						else
							ATLASSERT(FALSE);

						pUser.Release();
						pChat.Release();
						
					}

					if(bstrEvent == CComBSTR(L"chat_leave"))
					{
						CComPtr<IXMLDOMNode> pNode = NULL;
						CUser*				 m_pUser   = NULL;
						CChat*				 m_pChat   = NULL;

						pEnd->selectSingleNode(CComBSTR(L"user"),&pNode);
						if(pNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pNode);
							pNode.Release();
						}
						
						pEnd->selectSingleNode(CComBSTR(L"chat"),&pNode);
						if(pNode)
						{
							CChat::CreateInstance(&pChat);
							m_pChat = static_cast<CChat*>(pChat.p);
							UnPackChat(m_pChat->m_sChat,pNode);
							pNode.Release();
							m_pChat->m_pSession = pSession;
						}

						if(pChat && pUser)
						{
							MCTRACE(0,"Fire_eChatLeave");
							hr = pSession->Fire_eChatLeave(pUser,pChat);
						}
						else
							ATLASSERT(FALSE);

						pUser.Release();
						pChat.Release();
					}

					if(bstrEvent == CComBSTR(L"chat_accept"))
					{
						CComPtr<IXMLDOMNode> pNode = NULL;
						CComBSTR			 bsBody;
						CUser*				 m_pUser   = NULL;
						CChat*				 m_pChat   = NULL;

						pEnd->selectSingleNode(CComBSTR(L"user"),&pNode);
						if(pNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pNode);
							pNode.Release();
						}

						
						pEnd->selectSingleNode(CComBSTR(L"chat"),&pNode);
						if(pNode)
						{
							CChat::CreateInstance(&pChat);
							m_pChat = static_cast<CChat*>(pChat.p);
							UnPackChat(m_pChat->m_sChat,pNode);
							pNode.Release();
							m_pChat->m_pSession = pSession;
						}  

						if(pChat && pUser)
						{
						
							pEnd->selectSingleNode(CComBSTR(L"result"),&pNode);
							if(pNode)
							{
								pNode->get_text(&bsBody);
								pNode.Release();

								if(bsBody == CComBSTR(L"0"))
								{
									MCTRACE(0,"Fire_ChatAccept 0");
										hr = pSession->Fire_eChatAccept(pChat,pUser,0);
								}
								if(bsBody == CComBSTR(L"1"))
								{
									MCTRACE(0,"Fire_ChatAccept 1");
										hr = pSession->Fire_eChatAccept(pChat,pUser,1);
								}
							}
							else
							{
								MCTRACE(0,"Fire_ChatAccept 0");
								hr = pSession->Fire_eChatAccept(pChat,pUser,0);
							}
									   								
							

						
						}
						else
							ATLASSERT(FALSE);

						pUser.Release();
						pChat.Release();
					}

					
					if(bstrEvent == CComBSTR(L"chat_invite"))
					{
						CComPtr<IXMLDOMNode> pNode = NULL;
						CUser*				 m_pUser   = NULL;
						CUser*				 m_pFromUser   = NULL;
						CChat*				 m_pChat   = NULL;
						CComBSTR			 m_bsBody;
						pEnd->selectSingleNode(CComBSTR(L"user"),&pNode);
						if(pNode)
						{
							CUser::CreateInstance(&pUser);
							m_pUser = static_cast<CUser*>(pUser.p);
							UnPackUser(m_pUser->m_sUser,pNode);
							pNode.Release();
						}
						
						pEnd->selectSingleNode(CComBSTR(L"from_user"),&pNode);
						if(pNode)
						{
							CUser::CreateInstance(&pFromUser);
							m_pFromUser = static_cast<CUser*>(pFromUser.p);
							UnPackUser(m_pFromUser->m_sUser,pNode);
							pNode.Release();
						}

						pEnd->selectSingleNode(CComBSTR(L"chat"),&pNode);
						if(pNode)
						{
							CChat::CreateInstance(&pChat);
							m_pChat = static_cast<CChat*>(pChat.p);
							UnPackChat(m_pChat->m_sChat,pNode);
							pNode.Release();
							m_pChat->m_pSession = pSession;
						}
					
						pEnd->selectSingleNode(CComBSTR(L"body"),&pNode);
						if(pNode)
						{
							pNode->get_text(&m_bsBody);
							pNode.Release();
						}
						
						if(pChat && pUser && pFromUser)
						{
							MCTRACE(0,"Fire_eChatLeave");
							hr = pSession->Fire_eChatInvite(pChat,pFromUser,pUser,m_bsBody);
						}
						else
							ATLASSERT(FALSE);

						pUser.Release();
						pFromUser.Release();
						pChat.Release();	
					}
					
					/*if(bstrEvent == CComBSTR(L"system"))
					{
					}
					if(bstrEvent == CComBSTR(L"system"))
					{
					}*/

					pEnd.Release();
				}
				bstrEvent.Empty();
				pEventNode->get_nextSibling(&pTempNode);
				pEventNode.Release();
				if(pTempNode) {pEventNode = pTempNode; pTempNode.Release();}
			}

			pPacketNode->get_nextSibling(&pTempNode);
			pPacketNode.Release();
			if(pTempNode) {pPacketNode = pTempNode; pTempNode.Release();}
		}
	pEventRootNode->removeChild(pNode,&pEnd);
	pEnd.Release();
	pNode.Release();
	pEventRootNode->selectSingleNode(CComBSTR(L"root"),&pNode);
	}
}

void CCommandQueue::UnPackTime(long Handle, long &Time)
{
	char				 Value[30];
	CComPtr<IXMLDOMNode> pPacketNode;
	CComPtr<IXMLDOMNode> pTimeNode;
	CComBSTR			 filtr;
	CComBSTR			 bstrTime;

	ltoa(Handle,Value,10);
	CComBSTR	bstrHandle(Value);

	filtr += CComBSTR("answer[@handle = \"");
	filtr += bstrHandle;
	filtr += CComBSTR("\"]");
	
	//Find Handle
	pAnswerRootNode->selectSingleNode(filtr,&pPacketNode);
	if(pPacketNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	filtr.Empty();

	//Find response
	pPacketNode->selectSingleNode(CComBSTR(L"packet/response/time"),&pTimeNode);
	if(pTimeNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	pTimeNode->get_text(&bstrTime);
	USES_CONVERSION;
	Time = atol(OLE2T(bstrTime));
	return;
}


void CCommandQueue::UnPackUserDetails(long Handle, IUser **pUser)
{

	LPTSTR				 szTemplate = _T("answer[@handle = \"%d\"]");
	TCHAR				 szFiltr[100];
	sprintf(szFiltr,szTemplate,Handle);
	CComBSTR			 filtr(szFiltr);

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pUserNode;

	HRESULT				 hr = 0;
	CUser*				 m_pUser;	
	
	//Find Handle
	pAnswerRootNode->selectSingleNode(filtr,&pNode);
	if(pNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	filtr.Empty();
	
	
	pNode->selectSingleNode(CComBSTR("packet/response/user"), &pUserNode);
	if(pUserNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	hr = CUser::CreateInstance(pUser);
	m_pUser = static_cast<CUser*>(*pUser);

	if(FAILED(hr)) {ATLASSERT(FALSE); return;}
	UnPackUser(m_pUser->m_sUser,pUserNode);
	
}
 

HRESULT CCommandQueue::Init()
{
	return OutPutQueueInit();
}

VOID CCommandQueue::DeleteAllCommands(void *pWorkClass)
{
	CSession*				pSession;
	pSession = (CSession*)	pWorkClass;

	CComPtr<IXMLDOMNodeList> pChildNodes;
	CComPtr<IXMLDOMNode> pChildNode;
	CComPtr<IXMLDOMNode> pNode;
	CComBSTR			 bsHandle;
	LONG				 size,handle;
								   

		pRootNode->selectNodes(CComBSTR(_T("*/*/*")),&pChildNodes);
		if(pChildNodes) 
		{
			pChildNodes->get_length(&size);
			for(int k = 0;k<size;k++)
			{
				pChildNodes->get_item(k,&pChildNode);
				pChildNode->selectSingleNode(CComBSTR(_T("@handle")),&pNode);
				if(pNode != NULL)
				{
					bsHandle.Empty();
					pNode->get_text(&bsHandle);
					handle = _wtol(bsHandle);
					pSession->Fire_CommandError(handle,1,1);
				}
				pChildNode.Release();
				pNode.Release();
			}
		}

#ifdef _DEBUG
		pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
}

DWORD CCommandQueue::x_ChatCreate(BSTR CID, BSTR Name, BSTR Descr)
{
	CComPtr<IXMLDOMNode> pChatNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
//	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatCreate"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatCreate));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_create"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(CID));
	pRequestNode->appendChild(pNode,&pChatNode);
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();


	//Create name Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("name"),NULL,&pNode); 
	pNode->put_text(Name);
	pChatNode->appendChild(pNode,NULL);
	pNode.Release();

	//Create descr Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("descr"),NULL,&pNode); 
	pNode->put_text(Descr);
	pChatNode->appendChild(pNode,NULL);
	pNode.Release();
	

	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_ChatStatus(BSTR CID, LONG Status, LONG Param)
{
	//Param = 50;

	CComPtr<IXMLDOMNode> pChatNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatStatus"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatStatus));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_status"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(CID));
	pRequestNode->appendChild(pNode,&pChatNode);
	//pRequestNode.Release();
	pNode.Release();
	pEle.Release();

	//status
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("status"),NULL,&pNode); 
	ltoa(Status,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,NULL);
	pNode.Release();

	//Param
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("param"),NULL,&pNode); 
	ltoa(Param,Value,10);
	pNode->put_text(CComBSTR(Value));
	pRequestNode->appendChild(pNode,NULL);
	pNode.Release();

	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_ChatEdit(BSTR CID, BSTR Name, BSTR Descr)
{
	CComPtr<IXMLDOMNode> pChatNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
//	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatEdit"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatEdit));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_edit"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(CID));
	pRequestNode->appendChild(pNode,&pChatNode);
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();


	//Create name Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("name"),NULL,&pNode); 
	pNode->put_text(Name);
	pChatNode->appendChild(pNode,NULL);
	pNode.Release();

	//Create descr Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("descr"),NULL,&pNode); 
	pNode->put_text(Descr);
	pChatNode->appendChild(pNode,NULL);
	pNode.Release();
	

	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif

	return m_Handle;
}

DWORD CCommandQueue::x_ChatInvite(sChat* pChat,BSTR bsInvitation)
{

	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	CComPtr<IXMLDOMNode> pRecipientsNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatInvite"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += pChat->m_CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatInvite));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_invite"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(pChat->m_CID));
	pRequestNode->appendChild(pNode,NULL);
	pNode.Release();
	pEle.Release();

	//body
	if(bsInvitation != NULL)
	{
		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("body"),NULL,&pNode); 
		pNode->put_text(bsInvitation);
		pRequestNode->appendChild(pNode,NULL);
		pNode.Release();
		pEle.Release();
	}

	//recipients
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("recipients"),NULL,&pNode); 
	pRequestNode->appendChild(pNode,&pRecipientsNode);
	pNode.Release();
	
	list<long>::iterator iter;
	iter = pChat->m_Users.begin();

	while(iter != pChat->m_Users.end())
	{
		pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("user"),NULL,&pNode); 
		pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
		ltoa((long)*iter,Value,10);
		pEle->setAttribute(CComBSTR("id"),CComVariant(Value));
		pRecipientsNode->appendChild(pNode,NULL);
	 
		pNode.Release();
		pEle.Release();

		iter ++;
	}

	pChat->m_Users.clear();
	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;

}

DWORD CCommandQueue::x_ChatAccept(BSTR CID, LONG Result)
{

	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
//	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatAccept"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatAccept));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_accept"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(CID));
	pRequestNode->appendChild(pNode,NULL);
	//pRequestNode.Release();
	pNode.Release();
	pEle.Release();


	//Create List Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("result"),NULL,&pNode); 
	switch(Result)
	{
	case 1:
		bstrSTR=CComBSTR("accept");
		break;
	case 0:
		bstrSTR=CComBSTR("deny");
		break;
	default:
		throw(ErrorCode = WRONG_PARAM);
	}
	pNode->put_text(bstrSTR);
	pRequestNode->appendChild(pNode,NULL);
	pNode.Release();
	bstrSTR.Empty();
	

	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

DWORD CCommandQueue::x_ChatLeave(BSTR CID)
{

	CComPtr<IXMLDOMNode> pChatNode;
	CComPtr<IXMLDOMNode> pCommandNode;
	CComPtr<IXMLDOMNode> pRequestNode;
	
	CComPtr<IXMLDOMElement> pEle;
	CComPtr<IXMLDOMNode> pChildNode;

	CComPtr<IXMLDOMNode> pNode;
	

	CComBSTR			 bstrSTR;
	CComBSTR			 bstrFiltr;
	long				 ErrorCode;
//	char				 Value[30];

	pRootNode->selectSingleNode(CComBSTR("Commands/ChatLeave"),&pChildNode);
	if(!pChildNode) throw(ErrorCode = WRONG_QUEUE);

	bstrFiltr  = CComBSTR(L"*/*/chat[@cid =\"");
	bstrFiltr += CID;
	bstrFiltr += CComBSTR(L"\"]");

	pChildNode->selectSingleNode(bstrFiltr,&pNode);
	if(pNode) throw(ErrorCode = AlREADY_IN_QUEUE);

	//Create command Node
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("command"),NULL,&pCommandNode); 
	pCommandNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("handle"),CComVariant(++m_Handle));
	pEle->setAttribute(CComBSTR("type"),CComVariant(cmChatLeave));
	pEle.Release();
	
	//request
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("request"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("value"),CComVariant("c_ch_leave"));
	pCommandNode->appendChild(pNode,&pRequestNode);
	pNode.Release();
	pEle.Release();

	//chat
	pDom->createNode(CComVariant(NODE_ELEMENT),CComBSTR("chat"),NULL,&pNode); 
	pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pEle);
	pEle->setAttribute(CComBSTR("cid"),CComVariant(CID));
	pRequestNode->appendChild(pNode,&pChatNode);
	pRequestNode.Release();
	pNode.Release();
	pEle.Release();

	pChildNode->appendChild(pCommandNode,&pNode);
	if(pNode == NULL) throw(ErrorCode = WRONG_QUEUE);
//	Handle = m_Handle;

#ifdef _DEBUG
	pDom->save(CComVariant("c:\\im\\template.xml"));
#endif
	return m_Handle;
}

void CCommandQueue::UnPackSetChatStatus(long Handle, IUsers **lpUsers, BSTR *bsLog)
{

	LPTSTR				 szTemplate = _T("answer[@handle = \"%d\"]");
	TCHAR				 szFiltr[100];
	sprintf(szFiltr,szTemplate,Handle);
	CComBSTR			 filtr(szFiltr);

	CComPtr<IXMLDOMNode> pNode;
	CComPtr<IXMLDOMNode> pLog;
	CComPtr<IXMLDOMNode> pUserNode;
	CComPtr<IXMLDOMNode> pUserNodes;

	HRESULT				 hr = 0;
	CComPtr<IUsers>		pUsers;
	CComPtr<IUser>		pUser;
	CUser*				pcUser;


	//Find Handle
	pAnswerRootNode->selectSingleNode(filtr,&pNode);
	if(pNode == NULL) 
	{	
		ATLASSERT(FALSE);
		return;
	}
	filtr.Empty();
	
	hr = CUsers::CreateInstance(&pUsers);


	pNode->selectSingleNode(CComBSTR("packet/response/log"), &pLog);
	if(pLog != NULL) 
		pLog->get_xml(bsLog);
	else
		*bsLog = CComBSTR(L"<log/>").Detach();

	pNode->selectSingleNode(CComBSTR("packet/response/users/user"), &pUserNode);
	pNode.Release();
	if(pUserNode != NULL) 
	{	

		do
		{
		pUsers->AddUser(&pUser);
		pcUser = static_cast<CUser*>((LPVOID)pUser);
		UnPackUser(pcUser->m_sUser,pUserNode);
		pUser.Release();

		pUserNode->get_nextSibling(&pNode);
		pUserNode.Release();
		pUserNode = pNode;
		pNode.Release();
		}
		while(pUserNode);
			
	}

	(*lpUsers) = pUsers.Detach();
	
}
