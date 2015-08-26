// UserCollection.h: interface for the CUserCollection class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_USERCOLLECTION_H__EA6D63C9_6F59_4052_A9D7_C19A521A8896__INCLUDED_)
#define AFX_USERCOLLECTION_H__EA6D63C9_6F59_4052_A9D7_C19A521A8896__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include "User.h"

class CUserCollection  
{
public:
	CUser* GetAt(long Id);
	BOOL Delete(CUser& user);
	BOOL IsEmpty();
	BOOL GetNext(POSITION& pos, CUser*& pUser);
	POSITION InitIteration();
	BOOL LookUp(CUser& user);
    BOOL GetHasNewMessage(CUser &user);
	void SetAt(CUser& user);
	CUser* SetAtPointer(CUser &user);
	int GetCount();
	void Clear();
	CUserCollection();
	virtual ~CUserCollection();

protected:
	//POSITION m_pos;
	CCriticalSection m_lock;
	CMap<long,long,CUser*,CUser*> m_map;

};

#endif // !defined(AFX_USERCOLLECTION_H__EA6D63C9_6F59_4052_A9D7_C19A521A8896__INCLUDED_)
