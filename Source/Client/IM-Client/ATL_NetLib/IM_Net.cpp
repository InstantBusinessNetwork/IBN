// IM_Net.cpp: implementation of the CIM_Net class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "IM_Net.h"
#include "Session.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

extern long g_MessageSend;
extern long g_MessageReceveived;

extern long g_FileSent;
extern long g_FileReceived;

BASE_NET_MANAGER_CONFIG CIM_Net::m_BASE_NET_MANAGER_CONFIG;

CIM_Net::CIM_Net()
{
	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::CIM_Net() begin");
	
	::InitializeCriticalSection(&hCS);
	::InitializeCriticalSection(&hNewCommandCS);
	
	//init var
	m_bWaitForDisconnect= FALSE;
	m_bMustBeDestroied  = FALSE;
	m_WaitForReconnect = FALSE;
	m_State				= stDisconnected;
	m_OpenThreadCount	= 0;
	m_StartedChannelCount = 0;
	m_DisErrorCode = 0;
	m_DisErrorType = 0;

	//Output commands events
	for(int k=0; k<2; k++)
	{
		m_CurrentHandles[k]  = 0;
		m_CurrentCommands[k] = 0;
		m_EnableSend[k] = true;
	}

	hEvents[0] = ::CreateEvent(NULL,FALSE,FALSE,0); //New command
	hEvents[1] = ::CreateEvent(NULL,TRUE,FALSE,0); //Destroy

	//input answers and events events
	hCallBackEvents[NET_EVENT_RECEIVER_CHANNEL] = ::CreateEvent(NULL,FALSE,FALSE,0);
	hCallBackEvents[NET_COMMAND_CHANNEL] = ::CreateEvent(NULL,FALSE,FALSE,0);
	hCallBackEvents[NET_HISTORY_CHANNEL] = ::CreateEvent(NULL,FALSE,FALSE,0);
	hCallBackEvents[NET_FILESEND_CHANNEL] = ::CreateEvent(NULL,FALSE,FALSE,0);
	hCallBackEvents[NET_FILERECEIVE_CHANNEL] = ::CreateEvent(NULL,FALSE,FALSE,0);
	hCallBackEvents[5] = CreateEvent(NULL,FALSE,FALSE,0);//New Event signal 
	hCallBackEvents[6] = hEvents[1];//Destroy signal
	
	//Net configuration
	
	DWORD				dwThreadId;
	HANDLE				hThread;
	
	hThread = CreateThread(NULL,NULL,CheckQueueThread,this,0,&dwThreadId);
		CloseHandle(hThread);
	hThread = CreateThread(NULL,NULL,CallBackThread,this,0,&dwThreadId);
		CloseHandle(hThread);

	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::CIM_Net() end");
}

CIM_Net::~CIM_Net()
{ 
	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::~CIM_Net() beging");
	
	
	m_bMustBeDestroied = TRUE;

	m_BaseNetManager.StopAllOperations();

	SetEvent(hEvents[1]);

	while(m_StartedChannelCount)
		Sleep(100);
	
	//Sleep(1000);
	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::~CIM_Net() m_StartedChannelCount == 0");
	
	for(int k=0; k<2; k++)
		CloseHandle(hEvents[k]);
	
	for(int k=0; k<6; k++)
		CloseHandle(hCallBackEvents[k]);

	::DeleteCriticalSection(&hCS);
	::DeleteCriticalSection(&hNewCommandCS);
			
	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::~CIM_Net() end");
}

//DEL void CIM_Net::OpenInternet()
//DEL {
//DEL 	if (InetHandle != NULL) return;
//DEL 
//DEL 	InetHandle = InternetOpen(_T("Mediachase IM"), prAccessType, prProxyName,prBypass,NULL);
//DEL 
//DEL 	if (InetHandle == NULL) return;//throw();
//DEL }


bool CIM_Net::CancelCommand(long Handle)
{
	int k = m_CommandQueue.DeleteCommand(Handle);
	BOOL Find = FALSE;
	
	if(k>=0 && k<=4 && m_CurrentHandles[k] == Handle)
	{
		Find = TRUE;
		m_BaseNetManager.StopOperation((NET_CHANNEL_ENUM)k);
	}
	if(!Find)
		while(!m_ParentSession->PostMessage(IM_ANSWER_ERROR,Handle,MAKELONG(LOWORD(etCANCEL),LOWORD(1))));
	return true;
}
bool IsInternal(long pp)
{

	switch(pp)
	{
	case cmLogOn_int:
	case cmConfirmMessage:
	case cmConfirmPromo:
//	case cmConfirmFile:
	return true;
		break;
	}
return false;
};
//*****************************************************************************************
//
//
//*****************************************************************************************
DWORD WINAPI CIM_Net::CallBackThread(LPVOID param)
{
	CIM_Net* WorkClass;
	WorkClass = (CIM_Net*)param;
	WorkClass->ProcessingServerEvents();
	return 0;
}

//***************************************************************************************
//Sending
//
//
DWORD WINAPI CIM_Net::CheckQueueThread(LPVOID param)
{
	try
	{
		CIM_Net*				  WorkClass;	
		WorkClass = (CIM_Net*)param;
		WorkClass->CheckQueue();
	}
	catch(...)
	{
	}
	return 0;
}

void CIM_Net::CheckQueue()
{	
	
	LPCTSTR		ConstStr[4] = {LPCTSTR("Commands/*/*"), LPCTSTR("History/*/*"),
							   LPCTSTR("ReceiveFiles/*"), LPCTSTR("SendFiles/*")};
	
	
	
	::InterlockedIncrement(&m_OpenThreadCount);
	::CoInitializeEx(NULL,COINIT_MULTITHREADED);
	USES_CONVERSION;
do
{
	MCTRACE(2,"CIM_Net::CheckQueue got to waiting\r\n");
	DWORD kk = ::WaitForMultipleObjects(2,(const HANDLE *)(hEvents),FALSE,INFINITE);
	ATLASSERT(WAIT_FAILED != kk);
	kk -= WAIT_OBJECT_0;

	//Close Thread
	if(kk == 1)
	{
	    ::InterlockedDecrement(&m_OpenThreadCount);
		::CoUninitialize();	
		return;
	}

	MCTRACE(2,"CIM_Net::CheckQueue befor CS\r\n");
	::EnterCriticalSection(&hCS);
	MCTRACE(2, "CIM_Net::CheckQueue after CS\r\n");

	for(int intChannel=0; intChannel<4; intChannel++)
	{
		if(m_EnableSend[intChannel])
		{
			CComBSTR					bsFileName; //input, output file name
			CComBSTR					SID;
			CComPtr<IXMLDOMDocument>	pDoc = NULL;
			BOOL						bFileDirection;
			DWORD						dwCommandHandle, dwCommandType;
			long						dwSize;
			long						hBackWind;
			CComPtr<IStream>			pStream = NULL;
			CComPtr<IXMLDOMNode>	  pNode;
			CComBSTR				  bstrStatus;
			int i;
			if(!m_CommandQueue.CheckQueueItem(CComBSTR(ConstStr[intChannel]),&pDoc,
											 dwCommandHandle,dwCommandType,
											 &bsFileName,&hBackWind,&dwSize,&SID))
			continue;
#ifdef _DEBUG			
#ifdef _DOLOG
			CComBSTR	bsCommand;
			pDoc->get_xml(&bsCommand);
			MCTRACE(3 + intChannel,
			"CIM_Net::CheckQueue new command\r\n %S",bsCommand);
			pDoc->save(CComVariant(CComBSTR(_T("C:\\im\\command.xml"))));
#endif
#endif
			XMLtoStream(pDoc,&pStream);

			switch(intChannel)
			{
			case opCommand:
			case opHistory:
				switch(dwCommandType)
				{
				case cmLogOn:
					while(!m_ParentSession->PostMessage(IM_CHANGE_STATE,stConnecting,0))
						Sleep(10);
					break;
				case cmChangeStatus:
					pDoc->selectSingleNode(CComBSTR("packet/request/status"),&pNode);
					pNode->get_text(&bstrStatus);
					m_CurrentSendingStatus = atol(OLE2T(bstrStatus));
					break;
				default:
					//ATLASSERT(FALSE);
					break;
				}
			break;
			case opReceiveFile:
				bFileDirection = NET_FILE_RECEIVE;
				break;
			case opSendFile:
				bFileDirection = NET_FILE_SEND;
				break;
			default:
				break;
			}
			m_CurrentHandles[intChannel] = dwCommandHandle;
			m_CurrentCommands[intChannel]= dwCommandType;
					
			::InterlockedIncrement(&m_StartedChannelCount);	
			m_BaseNetManager.StartOperation((NET_CHANNEL_ENUM)intChannel,
											 OLE2T(SID),dwCommandHandle,
											 pStream,
											 hCallBackEvents[intChannel],
											 OLE2T(bsFileName),
											 (NET_FILE_DIRECTION)bFileDirection,
											 (HWND)hBackWind);
			m_EnableSend[intChannel] = FALSE;
			pStream.Release();
			switch(intChannel)
			{
			case opCommand:
			case opHistory:
				switch(dwCommandType)
				{
					case cmLogOff:
						m_bWaitForDisconnect = true;
						for(i=0; i<4; i++)
						{
							m_DisErrorCode = 0;
							m_DisErrorType = 0;
							m_BaseNetManager.StopOperation((NET_CHANNEL_ENUM)(i+1));
							m_EnableSend[i] = FALSE;
						}
							
					break;
					default:
					break;
				}
				break;
			default:
				break;
			}
		}
	}
	::LeaveCriticalSection(&hCS);		
}
while(!m_bMustBeDestroied);

	::CoUninitialize();
	::InterlockedDecrement(&m_OpenThreadCount);
	return;
}
	

//DEL void CIM_Net::SetNewCommand(opType type)
//DEL {	
//DEL 	ATLASSERT(type<5 && type >= 0);
//DEL 	::EnterCriticalSection(&hNewCommandCS);
//DEL 	::SetEvent(hEvents[type]);
//DEL 	 LeaveCriticalSection(&hNewCommandCS);
//DEL }

//DEL BOOL CIM_Net::XMLtoMEM(IXMLDOMDocument *pDom, PBYTE &pBuff, DWORD &dwSize)
//DEL {
//DEL     CComPtr<IPersistStreamInit> pPSI;
//DEL     CComPtr<IStream>            pStm;
//DEL     BYTE						*pbData = NULL;
//DEL     HGLOBAL						hGlobal;
//DEL     ULARGE_INTEGER				uli;
//DEL 	LARGE_INTEGER				li = {0, 0};
//DEL     HRESULT						hr = 0;
//DEL 	
//DEL try
//DEL {
//DEL 	pBuff = NULL;
//DEL 	dwSize = 0;
//DEL 
//DEL     hr = CreateStreamOnHGlobal(NULL, TRUE, &pStm);
//DEL 	if(FAILED(hr)) goto CleanUp;
//DEL 
//DEL     hr = pDom->QueryInterface( IID_IPersistStreamInit, (void **)&pPSI);
//DEL 	if(FAILED(hr)) goto CleanUp;
//DEL     
//DEL 	hr = pPSI->Save(pStm, TRUE);
//DEL 	if(FAILED(hr)) goto CleanUp;
//DEL 
//DEL     hr = pStm->Seek(li, STREAM_SEEK_CUR, &uli);
//DEL 	if(FAILED(hr)) goto CleanUp;
//DEL 
//DEL     dwSize = (int)uli.QuadPart;
//DEL 
//DEL     pBuff = new BYTE[dwSize]; MCTRACE(2,"XML TO MEM = NEW BUFFER");
//DEL 
//DEL     if (pBuff == NULL) goto CleanUp;
//DEL     
//DEL     hr = GetHGlobalFromStream(pStm, &hGlobal);
//DEL 	if(FAILED(hr)) goto CleanUp;
//DEL     
//DEL 	memcpy(pBuff, (PBYTE)GlobalLock(hGlobal),dwSize);
//DEL     GlobalUnlock(hGlobal);
//DEL 
//DEL }
//DEL catch(...){hr = E_FAIL;}
//DEL 
//DEL CleanUp:
//DEL 	if(FAILED(hr))
//DEL 	{
//DEL 		if(pBuff) delete[] pBuff; pBuff = NULL;
//DEL 		return false;
//DEL 	}
//DEL 		return true;
//DEL }

//DEL BOOL CIM_Net::MEMtoXML(IXMLDOMDocument **pDom, PBYTE &pBuff, DWORD &dwSize)
//DEL {
//DEL     CComPtr<IPersistStreamInit>     pPSI;
//DEL     CComPtr<IStream>                pStm;
//DEL 	CComPtr<IXMLDOMDocument>		pDoc;
//DEL     HRESULT hr;
//DEL     ULONG ulWritten;
//DEL     LARGE_INTEGER li = {0, 0};
//DEL try
//DEL {
//DEL 	pDoc.CoCreateInstance(CLSID_FreeThreadedDOMDocument);
//DEL 	if(!pDoc) return false;
//DEL     hr = CreateStreamOnHGlobal(NULL, TRUE, &pStm);
//DEL 	if(FAILED(hr)) return false;
//DEL     
//DEL 	hr = pStm->Write(pBuff, dwSize, &ulWritten);
//DEL 	if(FAILED(hr)) return false;
//DEL 
//DEL     
//DEL     hr = pStm->Seek(li, STREAM_SEEK_SET, NULL);
//DEL 	if(FAILED(hr)) return false;
//DEL 
//DEL     hr = pDoc->QueryInterface(IID_IPersistStreamInit, (void **)&pPSI);
//DEL 	if(FAILED(hr)) return false;
//DEL     
//DEL 	hr = pPSI->Load(pStm);
//DEL 	if(FAILED(hr)) return false;
//DEL 	
//DEL 	*pDom = pDoc.Detach();
//DEL 	return true;
//DEL }
//DEL catch(...)
//DEL {}
//DEL 	return false;
//DEL }

void CIM_Net::Config()
{
	USES_CONVERSION;

	m_BaseNetManager.Close();
	//prAccessType = INTERNET_OPEN_TYPE_PRECONFIG;
	//prProxyName = NULL;
	//prBypass = NULL;
	
	m_BaseNetManager.Init(m_BASE_NET_MANAGER_CONFIG);
/*
	if(prProxyName)
	{delete[] prProxyName;}
	prProxyName = new TCHAR[strlen(OLE2T(pConfig.m_ProxyServer))+1];
	strcpy(prProxyName,OLE2T(pConfig.m_ProxyServer));

	if(prBypass)
	{delete[] prBypass;}
	prBypass = new TCHAR[strlen(OLE2T(pConfig.m_ProxyBypass))+1];
	strcpy(prBypass,OLE2T(pConfig.m_ProxyBypass));*/

	
	prAccessType = INTERNET_OPEN_TYPE_DIRECT;

	//OpenInternet();
	
/*
	m_NetCommand.hInternet = InetHandle;
	m_NetEvent.hInternet = InetHandle;
	m_NetHistory.hInternet = InetHandle;
	m_NetReceiveFile.hInternet = InetHandle;
	m_NetSendFile.hInternet = InetHandle;
	m_NetSendFile.hInternet = InetHandle;
	
	
	m_NetCommand.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);
	m_NetEvent.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);
	m_NetHistory.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);
	m_NetReceiveFile.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);
	m_NetSendFile.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);
	m_NetSendFile.Config(OLE2T(pConfig.m_Server),OLE2T(pConfig.m_Path),pConfig.m_Port);*/

}

DWORD CIM_Net::ProcessingServerEvents()
{
//	HRESULT							hr;
	DWORD							res = 0;

	DWORD							OperationComplet;
	DWORD							ErrorCode = 0;
	DWORD							ErrorType = 0;
	DWORD							ErrorCount= 0;

	USES_CONVERSION;
	::InterlockedIncrement(&m_OpenThreadCount);
	::CoInitializeEx(NULL,COINIT_MULTITHREADED);

/*
NET_COMMAND_CHANNEL,
NET_HISTORY_CHANNEL,
NET_FILERECEIVE_CHANNEL,
NET_FILESEND_CHANNEL,
NET_EVENT_RECEIVER_CHANNEL
*/
	MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::ProcessingServerEvents STARTED");

	do
	{
		res = WaitForMultipleObjects(7,(const HANDLE *)(hCallBackEvents),FALSE,INFINITE);
		OperationComplet = res-WAIT_OBJECT_0;
		
		//==================================================================
			//Print state dump
			MCTRACE(OperationComplet+ 3,
			"CIM_Net::ProcessingServerEvents *State_dump_1*\r\n"
			"WaitForReconnect = %d;\r\n"
			"m_bWaitForDisconnect = %d;\r\n"
			"ErrorCount = %d;\r\n"
			"m_StartedChannelCount = %d\r\n",
			m_WaitForReconnect,m_bWaitForDisconnect,ErrorCount,m_StartedChannelCount);

		//===================================================================
		// Must be destroied //6 == STOP
		if(OperationComplet == 6)
		{
			MCTRACE(OBJECT_CREATE_DELETE,"CIM_Net::ProcessingServerEvents STOPTED");
			InterlockedDecrement(&m_OpenThreadCount);
			CoUninitialize();	
			return 0;
		}
		
		MCTRACE(OperationComplet+ 3,"CIM_Net::ProcessingServerEvents Operation Completed");
		
		//===================================================================
		// We have received new event //5 = new event
		if(OperationComplet == 5)
		{
			ErrorCount = 0;
			NewEventProcessing();
			continue;
		}
		InterlockedDecrement(&m_StartedChannelCount);      //Decrement Count Net Thread
		//===================================================================
		//Preprocess any operation completed
		DWORD	dwCommandHandle = 0;
		DWORD	dwCommandType = 0;
		
		
		if(OperationComplet<4)
		{
			dwCommandHandle = m_CurrentHandles[OperationComplet];
			dwCommandType   = m_CurrentCommands[OperationComplet];
			MCTRACE(OperationComplet+ 3,
					"CIM_Net::ProcessingServerEvents New Answer Command Handle = %d CommandType = %d",
					dwCommandHandle,dwCommandType);
		}

		//***************************************************************************
		//Obtain error codes
		m_BaseNetManager.GetResultError((NET_CHANNEL_ENUM)OperationComplet, ErrorType,ErrorCode);
		
		MCTRACE(OperationComplet+ 3,
				"CIM_Net::ProcessingServerEvents New Answer Error Type= %d Code= %d",
				ErrorType,ErrorCode);
		
		
		//****************************************************************************
		//Process errors
		//Что происходит если пришла ошибка когда 
		//WaitForReconnect или m_bWaitForDisconnect true???

		if(!m_WaitForReconnect && !m_bWaitForDisconnect)
		if(ErrorType != 0 || ErrorCode != 0)
		{
				MCTRACE(OperationComplet+ 3,
				"CIM_Net::ProcessingServerEvents error processing");
			
			switch(ErrorType)
			{			   
			case etWININET://Ошибки связи
			case etSTATUS:
					MCTRACE(OperationComplet+ 3,
					"CIM_Net::ProcessingServerEvents etSTATUS or etSTATUS");
				ErrorCount++;
				if(ErrorCount>3) //Отработка отключения (3 ошибки подряд)
				{
						MCTRACE(OperationComplet+ 3,
						"CIM_Net::ProcessingServerEvents ErrorCount > 3");
					
					//Подготовка к отключению!!!!!!!!!!!!!!!!!!!!!
					m_bWaitForDisconnect = true;	//
					m_DisErrorType = ErrorType;
					m_DisErrorCode = ErrorCode;
					LockSending();
					
					m_BaseNetManager.StopAllOperations();
					m_BaseNetManager.StopEventReceiver();
				}
				else
				{
						MCTRACE(OperationComplet+ 3,
						"CIM_Net::ProcessingServerEvents ErrorCount <= 3");
					
					// if command // try send again
					if(OperationComplet == NET_COMMAND_CHANNEL || 
					   OperationComplet == NET_HISTORY_CHANNEL)
					{
						UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
					}
					else
					if(OperationComplet == NET_FILERECEIVE_CHANNEL || 
					   OperationComplet == NET_FILESEND_CHANNEL)
					{
						ErrorCount --;
						m_CommandQueue.DeleteCommand(dwCommandHandle);
						
						UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);					
						while(!m_ParentSession->PostMessage(IM_ANSWER_ERROR,dwCommandHandle,MAKELONG(LOWORD(ErrorType),LOWORD(ErrorCode))));
					}
					else
					if(OperationComplet == NET_EVENT_RECEIVER_CHANNEL) //5 == GET_EVENT
					{
						
						InterlockedIncrement(&m_StartedChannelCount);
						m_BaseNetManager.StartEventReceiver(
							OLE2T(m_ParentSession->m_SID),
							hCallBackEvents[NET_EVENT_RECEIVER_CHANNEL],
							hCallBackEvents[5]);

					}
				}
				break;
						  //********************************************************
			case etSERVER://Ошибки сервера
						MCTRACE(OperationComplet+ 3,
						"CIM_Net::ProcessingServerEvents etSERVER");
				if(OperationComplet == opCommand && dwCommandType == cmLogOn)
				{
							MCTRACE(OperationComplet+ 3,
							"CIM_Net::ProcessingServerEvents command was LogOn");
					m_bWaitForDisconnect = true;	
					m_DisErrorType = ErrorType;
					m_DisErrorCode = ErrorCode;
					break;
				}
				else
				switch(ErrorCode)
				{
				case 112:
					
					MCTRACE(OperationComplet+ 3,
						"CIM_Net::ProcessingServerEvents 112 error");
					
					LockSending();
					m_BaseNetManager.StopAllOperations();
					m_BaseNetManager.StopEventReceiver();

					if(OperationComplet == opCommand && dwCommandType == cmLogOff)
					{
						m_DisErrorType = ErrorType;
						m_DisErrorCode = ErrorCode;
						m_bWaitForDisconnect = true;
					}
					else
						m_WaitForReconnect = true;

					break;
				
				//Any another server error
				default:
					
						MCTRACE(OperationComplet+ 3,
						"CIM_Net::ProcessingServerEvents Error Processing Serever Any another err");

					if(dwCommandHandle != 0 && !IsInternal(dwCommandHandle))
					{
						//?????????????????????????????????????????
						while(!m_ParentSession->PostMessage(IM_ANSWER_ERROR,dwCommandHandle,MAKELONG(LOWORD(ErrorType),LOWORD(ErrorCode))));
					}
					else
						m_CommandQueue.DeleteCommand(dwCommandHandle);
					
					UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
					break;
				}
				break;

			case etFILE:
					MCTRACE(OperationComplet+ 3,
					"CIM_Net::ProcessingServerEvents Error Processing Serever File err");

					UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
							
					if(dwCommandHandle != 0 && !IsInternal(dwCommandHandle))
					{
						while(!m_ParentSession->PostMessage(IM_ANSWER_ERROR,dwCommandHandle,MAKELONG(LOWORD(ErrorType),LOWORD(ErrorCode))));
						MCTRACE(2,"CIM_Net::CallBackThread Error Process Serever File err Send Message");
					}
				break;

			case etCANCEL:
					MCTRACE(OperationComplet+ 3,
					"CIM_Net::ProcessingServerEvents Error Processing Serever Error Cancel");
															
					ErrorCode = 0;
					UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
						
					if(opCommand == OperationComplet && dwCommandType == cmLogOn)
					{
						m_DisErrorType = etCANCEL;
						m_DisErrorCode = ErrorCode;
						m_bWaitForDisconnect = true;
						break;
					}

					if(dwCommandHandle != 0 && !IsInternal(dwCommandHandle))
					{
						while(!m_ParentSession->PostMessage(IM_ANSWER_ERROR,dwCommandHandle,MAKELONG(LOWORD(ErrorType),LOWORD(ErrorCode))));
						MCTRACE(2,"CIM_Net::CallBackThread Error Process Serever Error Cancel Send Message");
					}
					else
						m_CommandQueue.DeleteCommand(dwCommandHandle);
				break;
			default:
				break;
			}
		}
		else
		{
			ErrorCount = 0;
			MCTRACE(2,"CIM_Net::CallBackThread ErrorCount = 0");
		}
		
		//**************************************************************************
		//If errors not found
		if(ErrorCode == 0 && ErrorType == 0)
		{
			ErrorCount = 0;
			CComPtr<IStream>				pStream = NULL;
			CComPtr<IXMLDOMDocument>		pDoc    = NULL;
			CComPtr<IXMLDOMNode>			pNode   = NULL;

			pStream.Release();
			m_BaseNetManager.GetResult((NET_CHANNEL_ENUM)OperationComplet,&pStream);
			
			m_CommandQueue.DeleteCommand(dwCommandHandle);
			
			//IMonitor implementation

			if(OperationComplet == NET_COMMAND_CHANNEL
				&&dwCommandType == cmMessage)
			{
				::InterlockedIncrement(&g_MessageSend);
			}
			else
			if(OperationComplet == NET_COMMAND_CHANNEL
				&&dwCommandType == cmConfirmMessage)
			{
				::InterlockedIncrement(&g_MessageReceveived);
			}
			else
			if(OperationComplet == NET_FILESEND_CHANNEL)
			{
				::InterlockedIncrement(&g_FileSent);
			}
			else
			if(OperationComplet == NET_FILERECEIVE_CHANNEL)
			{
				::InterlockedIncrement(&g_FileReceived);
			}
			//end IMonitor implementation


			if(pStream != NULL)
			XMLfromSream(&pDoc,pStream);
			
				if(pDoc != NULL)
				{
					if(S_OK == pDoc->get_firstChild(&pNode))
						m_CommandQueue.AddAnswer(dwCommandHandle,dwCommandType,pNode);					
					
					if(!IsInternal(dwCommandType))
					while(!m_ParentSession->PostMessage(IM_ANSWER_BUFF,dwCommandHandle,dwCommandType));
				}
				else
				if(!IsInternal(dwCommandType))
					while(!m_ParentSession->PostMessage(IM_ANSWER_OK,dwCommandHandle,dwCommandType));
			
			pNode.Release();
			pDoc.Release();

			if(opCommand == OperationComplet)
				switch(dwCommandType)
				{
					case cmLogOn:
						m_bWaitForDisconnect = FALSE;
						m_WaitForReconnect  = FALSE;
						m_DisErrorCode = 0;
						m_DisErrorType = 0;
						//send StatusChanged message
						m_State = stConnected;
						while(!m_ParentSession->PostMessage(IM_CHANGE_STATE,stConnected,0))
							Beep(100,100);
						
						//start event receiver
					
						InterlockedIncrement(&m_StartedChannelCount);
						m_BaseNetManager.StartEventReceiver(
							OLE2T(m_ParentSession->m_SID),
							hCallBackEvents[NET_EVENT_RECEIVER_CHANNEL],
							hCallBackEvents[5]);

						
						
						//add Load List command
						try	{m_CommandQueue.x_LoadList(ltContact);}
						catch(...){};
						try	{m_CommandQueue.x_LoadList(ltChats);}
						catch(...){};
						//unlock output commads
						UnlockSending();
						break;

					case cmLogOff:
						MCTRACE(2,"CIM_Net::CallBackThread opCommand || opHistory new Answer LOGOFF");
						
						m_bWaitForDisconnect = true;
						
						//lock output command
						LockSending();
						
						//Stop any net activity
						m_BaseNetManager.StopAllOperations();
						m_BaseNetManager.StopEventReceiver();
						
						//Set disconnect errors
						m_DisErrorType = ErrorType;
						m_DisErrorCode = ErrorCode;
						//waiting all operation completed
						break;
					default:
						if(!m_bWaitForDisconnect && !m_WaitForReconnect)
						UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
						break;
				}
			else
			if(!m_bWaitForDisconnect && !m_WaitForReconnect)
			UnlockSendingOperation((NET_CHANNEL_ENUM)OperationComplet);
		}
		
		//========================================================================
		//All thread closed
		if(m_StartedChannelCount == 0)
		{
			if(m_bWaitForDisconnect)
			{
				MCTRACE(2,"CIM_Net::CallBackThread WaitForDisconnect");
				m_State = stDisconnected;
				m_ParentSession->SendMessage(IM_CHANGE_STATE,stDisconnected,MAKELONG(m_DisErrorType,m_DisErrorCode));
				
				ErrorCount = 0;
				m_bWaitForDisconnect = false;
				UnlockSendingOperation((NET_CHANNEL_ENUM)opCommand);
			}
			else
			if(m_WaitForReconnect)
			{
				MCTRACE(2,"CIM_Net::CallBackThread WaitForReconnect");
				
				try{m_CommandQueue.x_LogOn_Inter();}catch(...){};
				ErrorCount = 0;
				m_WaitForReconnect = FALSE;
				UnlockSendingOperation((NET_CHANNEL_ENUM)opCommand);
			}
		}
		
		MCTRACE(2,"CIM_Net::CallBackThread WaitForNew Event");
	}
	while(!m_bMustBeDestroied);
	
	::CoUninitialize();
	::InterlockedDecrement(&m_OpenThreadCount);
	MCTRACE(2,"CIM_Net::CallBackThread CLOSED");
	return 0;
}


long CIM_Net::XMLtoStream(IXMLDOMDocument *pDoc, IStream **lpStream)
{
	CComPtr<IPersistStreamInit> pPSI;
    ULARGE_INTEGER				uli;
	LARGE_INTEGER				li = {0, 0};
    HRESULT						hr = 0;
	
		ATLASSERT(*lpStream == NULL);

		*lpStream = NULL;

		hr = CreateStreamOnHGlobal(NULL, TRUE, lpStream);
		if(FAILED(hr)) goto CleanUp;
		
		hr = pDoc->QueryInterface( IID_IPersistStreamInit, (void **)&pPSI);
		if(FAILED(hr)) goto CleanUp;
		
		hr = pPSI->Save(*lpStream, TRUE);
		if(FAILED(hr)) goto CleanUp;
		
		hr = (*lpStream)->Seek(li, STREAM_SEEK_SET, &uli);
		if(FAILED(hr)) goto CleanUp;	

#ifdef _DEBUG
#ifdef _DOLOG
		pDoc->save(CComVariant(CComBSTR(_T("C:\\im\\XMLToStream.xml"))));
#endif
#endif
		return S_OK;

CleanUp:
	if(*lpStream != NULL)
		(*lpStream)->Release();
	
	return E_FAIL;
}

long CIM_Net::NewEventProcessing()
{
	HRESULT	hr = S_OK;
	CComPtr<IStream> pStream = NULL;
	CComPtr<IXMLDOMDocument> pDoc = NULL;
	CComPtr<IXMLDOMNode>	pNode = NULL;
	
	do
	{
		pStream.Release();
		pDoc.Release();
		pNode.Release();

		hr = m_BaseNetManager.GetNextEvent(&pStream);
		if(pStream == NULL || hr != S_OK) return hr;
		hr = XMLfromSream(&pDoc,pStream);
		if(hr != S_OK) return S_OK;
		hr = pDoc->get_firstChild(&pNode);
		if(!pNode) return S_OK;
		VARIANT_BOOL vb_HaveChild = VARIANT_FALSE;
		pNode->hasChildNodes(&vb_HaveChild);
		if(vb_HaveChild == VARIANT_FALSE) return S_OK;
		m_CommandQueue.AddEvent(pNode);
		while(!m_ParentSession->PostMessage(IM_NEW_EVENT,0,0));
	}
	while(true);

    return S_OK;
};

long CIM_Net::XMLfromSream(IXMLDOMDocument **lpDoc, IStream *pStream)
{
	ATLASSERT(*lpDoc == NULL);
	ATLASSERT(pStream != NULL);
    
	HRESULT hr;
	CComPtr<IPersistStreamInit>     pPSI;
	CComPtr<IXMLDOMDocument>		pDoc;
//	ULONG ulWritten;
	LARGE_INTEGER li = {0, 0};
	
	hr = pDoc.CoCreateInstance(CLSID_FreeThreadedDOMDocument40);
	if(FAILED(hr)) return hr;
	 
	hr = pStream->Seek(li,STREAM_SEEK_SET,NULL);
	if(FAILED(hr)) return hr;

    hr = pDoc->QueryInterface(IID_IPersistStreamInit, (void **)&pPSI);
	if(FAILED(hr)) return hr;
#ifdef DEBUG
	TCHAR pp[201];
	DWORD si= 200;
	DWORD wsi = 1;
	while(wsi)
	{
		hr = pStream->Read(pp,si,&wsi);
		pp[wsi] = '\0';
		ATLTRACE(pp);
	}
	hr = pStream->Seek(li,STREAM_SEEK_SET,NULL);
	if(FAILED(hr)) return hr;
#endif DEBUG
	hr = pPSI->Load(pStream);
	if(FAILED(hr)) return hr;

#ifdef _DEBUG
#ifdef _DOLOG
	pDoc->save(CComVariant(CComBSTR(_T("C:\\im\\XMLFromStream.xml"))));
#endif
#endif
	VARIANT_BOOL vbChild = VARIANT_FALSE;
	pDoc->hasChildNodes(&vbChild);
	if(vbChild == VARIANT_TRUE)
	 *lpDoc = pDoc.Detach();
	 return S_OK;	
}

void CIM_Net::LockSending()
{
MCTRACE(2,"LockSending  begin\r\n");
	EnterCriticalSection(&hCS);
	for(int k=0; k<4; k++)
		m_EnableSend[k] = FALSE;
	LeaveCriticalSection(&hCS);
MCTRACE(2,"LockSending  begin\r\n");
}

void CIM_Net::UnlockSending()
{
MCTRACE(2,"UnlockSending  begin\r\n");
	EnterCriticalSection(&hCS);
	for(int k=0; k<4; k++)
		m_EnableSend[k] = TRUE;
	SetEvent(hEvents[0]);
	LeaveCriticalSection(&hCS);
MCTRACE(2,"UnlockSending  end\r\n");
}

void CIM_Net::LockSendingOperation(NET_CHANNEL_ENUM NetChannel)
{
MCTRACE(2,"LockSendingOperation  begin\r\n");
	EnterCriticalSection(&hCS);
		m_EnableSend[NetChannel] = FALSE;
	LeaveCriticalSection(&hCS);
MCTRACE(2,"LockSendingOperation  end\r\n");
}

void CIM_Net::UnlockSendingOperation(NET_CHANNEL_ENUM NetChannel)
{
MCTRACE(2,"UnlockSendingOperation  begin\r\n");
	EnterCriticalSection(&hCS);
		m_EnableSend[NetChannel] = TRUE;
		SetEvent(hEvents[0]);
	LeaveCriticalSection(&hCS);
MCTRACE(2,"UnlockSendingOperation  end\r\n");
}

void CIM_Net::SetNewCommand()
{
	MCTRACE(2,"SetNewCommand  begin\r\n");
EnterCriticalSection(&hCS);
	SetEvent(hEvents[0]);
LeaveCriticalSection(&hCS);
	MCTRACE(2,"SetNewCommand  end\r\n");
}

HRESULT CIM_Net::SetStream(IStream *pStream)
{
 return S_OK;
}

BOOL CIM_Net::WriteFileToServer()
{
 return S_OK;
}
