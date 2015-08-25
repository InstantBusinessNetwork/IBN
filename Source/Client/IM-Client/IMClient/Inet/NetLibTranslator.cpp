// MeTranslator.cpp: implementation of the CMeTranslator class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
//#include "OfsMessenger.h"
#include "NetLibTranslator.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CNetLibTranslator::CNetLibTranslator()
{
	hEventWnd = NULL;
	InitializeCriticalSection(&m_lock);
	NLT_Map.InitHashTable(100);
}

CNetLibTranslator::~CNetLibTranslator()
{
	DeleteCriticalSection(&m_lock);
}

HWND  CNetLibTranslator::NLT_GetWindow(LONG Handle)
{
	HWND hWnd = NULL;
	try
	{
		Translator_Item *pItem = NULL;
		if(NLT_Map.Lookup(Handle,pItem))
		{
			hWnd = pItem->hReturnToWnd;
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	return hWnd;
}


DWORD CNetLibTranslator::NLT_AddToTranslate(LONG Handle, HWND hReturnToWnd)
{
	try
	{
		Translator_Item *pItem = new Translator_Item;
		pItem->hReturnToWnd = hReturnToWnd;
		NLT_Map.SetAt(Handle,pItem);
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	return 0;
}

DWORD CNetLibTranslator::NLT_Remove(LONG Handle)
{
	try
	{
		Translator_Item *pItem = NULL;
		if(NLT_Map.Lookup(Handle,pItem))
		{
			if(pItem!=NULL)
			{
				delete pItem; pItem = NULL;
			}
			NLT_Map.RemoveKey(Handle);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	return 0;
}

void CNetLibTranslator::NLT_SetEventWindow(HWND hWnd)
{
	hEventWnd = hWnd;
}

HWND CNetLibTranslator::NLT_GetEventWindow()
{
	return hEventWnd;
}

void  CNetLibTranslator::Lock()
{
	EnterCriticalSection(&m_lock);
}

void  CNetLibTranslator::UnLock()
{
	LeaveCriticalSection(&m_lock);
}
