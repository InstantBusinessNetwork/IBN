// VariablesList.cpp: implementation of the CVariablesList class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "VariablesList.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
//#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CVariablesList::CVariablesList()
{
	m_pFirst = NULL;
	m_hCanModify = CreateEvent(NULL, TRUE, TRUE, NULL);
}

CVariablesList::~CVariablesList()
{
	WaitForSingleObject(m_hCanModify, INFINITE);
	ResetEvent(m_hCanModify);

	tagListElement *pElement = m_pFirst;
	tagListElement *pNext = NULL;

	while(pElement)
	{
		pNext = pElement->pNextElement;
		delete pElement;
		pElement = pNext;
	}

	m_pFirst = NULL;
	CloseHandle(m_hCanModify);
}

BOOL CVariablesList::SetVariable(LPCTSTR bstrName, LPCTSTR bstrValue)
{
	WaitForSingleObject(m_hCanModify, INFINITE);
	ResetEvent(m_hCanModify);

	_bstr_t strTmp = bstrName;

	tagListElement *pElement;

	pElement = m_pFirst;
	while(pElement)
	{
		if(!wcscmp(pElement->bstrName, strTmp))
		{
			break;
		}
		pElement = pElement->pNextElement;
	}

	if(!pElement)
	{
		pElement = new tagListElement;
		if(!pElement)
			return FALSE;
		pElement->pNextElement = m_pFirst;
		m_pFirst = pElement;

		pElement->bstrName = bstrName;
	}
	pElement->bstrValue = bstrValue;

	SetEvent(m_hCanModify);
	return TRUE;
}

BOOL CVariablesList::GetVariable(LPCTSTR bstrName, BSTR *bstrValue)
{
	WaitForSingleObject(m_hCanModify, INFINITE);
	ResetEvent(m_hCanModify);

	BOOL bResult = FALSE;
	_bstr_t strTmp = bstrName;

	tagListElement *pElement = m_pFirst;

	while(pElement)
	{
		if(!wcscmp(pElement->bstrName, strTmp))
		{
			bResult = TRUE;
			(*bstrValue) = pElement->bstrValue.copy();// CopyTo(bstrValue);
			break;
		}
		pElement = pElement->pNextElement;
	}

	SetEvent(m_hCanModify);
	return bResult;
}

void CVariablesList::DeleteVariable(LPCTSTR bstrName)
{
	WaitForSingleObject(m_hCanModify, INFINITE);
	ResetEvent(m_hCanModify);

	tagListElement leBegin, *pElement;
	leBegin.pNextElement = m_pFirst;
	_bstr_t strTmp = bstrName;	

	pElement = &leBegin;
	while(pElement->pNextElement)
	{
		if(!wcscmp(pElement->pNextElement->bstrName, strTmp))
		{
			pElement->pNextElement = pElement->pNextElement->pNextElement;
			delete pElement->pNextElement;
			break;
		}
		pElement = pElement->pNextElement;
	}
	m_pFirst = leBegin.pNextElement;

	SetEvent(m_hCanModify);
}
