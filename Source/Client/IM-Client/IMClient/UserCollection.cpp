// UserCollection.cpp: implementation of the CUserCollection class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "UserCollection.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CUserCollection::CUserCollection()
{
	//m_pos=NULL;
}

CUserCollection::~CUserCollection()
{
	Clear();
}

void CUserCollection::Clear()
{
    m_lock.Lock();	
	try
	{
		POSITION pos = m_map.GetStartPosition();
		long s;
		CUser* pUser;
		while(pos!=NULL)
		{
			m_map.GetNextAssoc(pos,s,pUser);
			delete pUser;
		}
		m_map.RemoveAll();
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
}

int CUserCollection::GetCount()
{
	return m_map.GetCount();
}

void CUserCollection::SetAt(CUser &user)
{
	m_lock.Lock();
	try
	{
		CUser* pUser = NULL;
		
		if(m_map.Lookup(user.GetGlobalID(),pUser))
		{
/*
			if(pUser->IsBad())
			{
				BOOL bTmp = pUser->m_bHasNewMessages;
				int TmpStatus = pUser->GetStatus();
				(*pUser) = user;
				pUser->m_bHasNewMessages =  bTmp;
				pUser->m_iStatus	=	TmpStatus;
			}
			else
*/			
			pUser->Update(user);
			(*pUser).m_iStatus  = user.m_iStatus ;
			//user = (*pUser);
		}
		else
			m_map.SetAt(user.GetGlobalID(), new CUser(user));
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
}

BOOL CUserCollection::LookUp(CUser &user)
{
	BOOL bFlagFind = FALSE;
    m_lock.Lock();
	try
	{
		CUser* pUser = NULL;
		if(m_map.Lookup(user.GetGlobalID(),pUser))
		{
			user=*pUser;
			bFlagFind = TRUE;
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
	return bFlagFind;
}

POSITION CUserCollection::InitIteration()
{
	//TRACE("\r\n CUserCollection::InitIteration ... ");
	//m_pos=m_map.GetStartPosition();
	return m_map.GetStartPosition();
}

BOOL CUserCollection::GetNext(POSITION& pos, CUser*& pUser)
{
	// ???? //
	if(pos==NULL)	return FALSE;
	
	m_lock.Lock();
	try
	{
		long s;
		if(pos) 
			m_map.GetNextAssoc(pos,s,pUser);
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();

	return TRUE;
}

BOOL CUserCollection::IsEmpty()
{
	return m_map.IsEmpty();
}	

BOOL CUserCollection::Delete(CUser &user)
{
	BOOL bFlagDel = FALSE;
	m_lock.Lock();
	try
	{
		CUser* pUser = NULL;
		long key = user.GetGlobalID();
		if(m_map.Lookup(key,pUser))
		{
			delete pUser;
			bFlagDel = m_map.RemoveKey(key);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
	return bFlagDel;
}

BOOL CUserCollection::GetHasNewMessage(CUser &user)
{
	BOOL bFlagReturn = FALSE;
	m_lock.Lock();
	try
	{
		CUser *pUser = NULL;
		if(m_map.Lookup(user.GetGlobalID(),pUser))
		{
			bFlagReturn = pUser->m_bHasNewMessages ;
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
    m_lock.Unlock();
	return bFlagReturn;

}

CUser* CUserCollection::SetAtPointer(CUser &user)
{
	m_lock.Lock();
	CUser* pUser = NULL;
	try
	{
		if(m_map.Lookup(user.GetGlobalID(),pUser))
		{
			(*pUser)=user;
		}
		else
		{
			pUser = new CUser(user);
			m_map.SetAt(user.GetGlobalID(), pUser);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
	
	return pUser;
}


CUser* CUserCollection::GetAt(long Id)
{
	CUser* pUser = NULL;
	m_lock.Lock();
	try
	{
		if(!m_map.Lookup(Id,pUser)) pUser = NULL;
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	m_lock.Unlock();
	return pUser;
}
