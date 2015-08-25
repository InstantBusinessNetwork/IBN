// SaveDataBase.cpp: implementation of the CSaveDataBase class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "SaveDataBase.h"
#include "User.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSaveDataBase::CSaveDataBase(bstr_t &SID, IComHistIntPtr &pHistory, IMessagesPtr &pMessage, DWORD dwNotifyMessage, HWND MessageParent)
{
    pHistoryStream = NULL;
	pMessagesStream = NULL;
	hWindow   = MessageParent;
	dwMessage = dwNotifyMessage;
	LoadSID =  SID;
	hr = S_OK;
	if(SUCCEEDED(hr))
		hr = CoMarshalInterThreadInterfaceInStream(__uuidof(IComHistInt),
		pHistory,&pHistoryStream);
	if(SUCCEEDED(hr))
		hr = CoMarshalInterThreadInterfaceInStream(__uuidof(IMessages),
		pMessage,&pMessagesStream);
	if(SUCCEEDED(hr))
	{
		hWorkThread = CreateThread(NULL,5476UL,thThread,(LPVOID)this,NORMAL_PRIORITY_CLASS,&IDWorkThread);
	}
	hExitEvent = CreateEvent(NULL,FALSE,FALSE,NULL);
	bExit      = FALSE;
}

CSaveDataBase::~CSaveDataBase()
{
	CloseHandle(hWorkThread);
	CloseHandle(hExitEvent);
}

HRESULT CSaveDataBase::InitOk()
{
	return hr;
}

void CSaveDataBase::WorkFunction()
{
	IComHistIntPtr  pHistory = NULL;
	IMessagesPtr    pMessagesList = NULL;
	/// UnMarshaling ...
	if(SUCCEEDED(hr))
		hr = CoGetInterfaceAndReleaseStream(pHistoryStream,__uuidof(IComHistInt),(LPVOID*)&pHistory);
	if(SUCCEEDED(hr))
		hr = CoGetInterfaceAndReleaseStream(pMessagesStream,__uuidof(IMessages),(LPVOID*)&pMessagesList);
	
	if(SUCCEEDED(hr))
	{
		/// Save To History ...
		bstr_t SID, MID, Body; 
		long   Time, ToId , FromId, RecipientSize; 
		IMessagePtr pMessage;
		IUsersPtr pUserList;
		
		try
		{
			long Size = pMessagesList->GetCount();
			SID = LoadSID;
			
			for(int i=1;i<=Size;i++)
			{
				if(bExit)
				{
					SetEvent(hExitEvent);
					return;
				}
				pMessage = pMessagesList->GetItem(i);
				
				MID  = pMessage->GetMID(); 
				Body = pMessage->GetBody();
				Time = pMessage->Getdate_time(); 
				pUserList = pMessage->GetRecipients();
				RecipientSize = pUserList->GetCount();
				FromId = CUser(pMessage->GetSender()).GetGlobalID();
				
				for(int j=0;j<RecipientSize;j++)
				{
					ToId   = CUser(pUserList->GetItem(j)).GetGlobalID();
					try
					{
						hr = pHistory->AddMessage(FromId,ToId,SID, MID,Time,TRUE,Body,VARIANT_FALSE);
					}
					catch(...)
					{}
				}
			}
		}
		
		catch(...)
		{
			ASSERT(FALSE);
		}
	}

	/// PostMessage About Ok
	if(::IsWindow(hWindow))
		PostMessage(hWindow,dwMessage,(WPARAM)hr,(LPARAM)this);
	else
		delete this;		
	/// End 
	SetEvent(hExitEvent);
}

DWORD CSaveDataBase::thThread(LPVOID p)
{
	((CSaveDataBase*)p)->WorkFunction();
	return 0;
}

void CSaveDataBase::Stop()
{
	bExit = TRUE;
	WaitForSingleObject(hExitEvent,30000);
}
