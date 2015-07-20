#include "ActiveSession.h"

#include "SWMRG.h"
#include <list>

#ifndef _OFS_MATRIX_H
#define _OFS_MATRIX_H

#define MAX_TABLE_SIZE 256
#define SID_SIZE 40

enum OFS_MATRIX_ERROR
{
	OFSM_ERROR_NO_MEMORY=0,
	OFSM_ERROR_NO_IDUNIQUE,
	OFSM_ERROR_NO_SIDUNIQUE,
	OFSM_ERROR_ITEMNOTFOUND,
	OFSM_ERROR_WRONGSID
};

//////////////////////////////////////////////////////////////////

inline BOOL CheckSID(LPCTSTR SID)
{
	if(_tcslen(SID) != 36)
		return FALSE;

	if(SID[8] != _T('-') || SID[13] != _T('-') || SID[18] != _T('-') || SID[23] != _T('-'))
		return FALSE;

	return TRUE;
};

struct _DBLSPISOKITEM
{
	static const size_t _sidSize = SID_SIZE;

	_DBLSPISOKITEM()
	{
		_id = 0;
		_sid[0] = _T('\0');
	};

	_DBLSPISOKITEM(long id, LPCTSTR sid)
	{
		_id = id;
		_tcscpy_s(_sid, _sidSize, sid);
	};

	const _DBLSPISOKITEM& operator=(const _DBLSPISOKITEM& source)
	{
		_id = source._id;
		_tcscpy_s(_sid, _sidSize, source._sid);
	};

	TCHAR _sid[_sidSize]; // Строка с данными
	long _id;
};

struct _SPISOKITEM
{
	static const size_t _sidSize = SID_SIZE;

	_SPISOKITEM()
	{
		_id = 0;
		_sid[0]= _T('\0');
		_pData = NULL;
	};
	_SPISOKITEM(long id, LPCTSTR sid, CActiveSession* pData)
	{
		_id = id;
		_tcscpy_s(_sid, _sidSize, sid);
		_pData = pData;
	};
	const _SPISOKITEM& operator=(const _SPISOKITEM& source)
	{
		_id = source._id;
		_tcscpy_s(_sid, _sidSize, source._sid);
		_pData = source._pData;
	};

	CActiveSession* _pData; // Данные
	TCHAR _sid[_sidSize]; // Строка с данными SID
	long _id;
};

class CMatrixItem
{
public:
	std::list<_SPISOKITEM> m_ItemList;
	VOID WaitToRead()
	{
		m_lock.WaitToRead();
	};
	VOID WaitToWrite()
	{
		m_lock.WaitToWrite();
	}
	VOID Done()
	{
		m_lock.Done();
	}
protected:
	CSWMRG m_lock;
};

class CMatrixDBLItem
{
public:
	std::list<_DBLSPISOKITEM> m_ItemList;
	VOID WaitToRead()
	{
		m_lock.WaitToRead();
	};
	VOID WaitToWrite() 
	{
		m_lock.WaitToWrite();
	}
	VOID Done()
	{
		m_lock.Done();
	}
protected:
	CSWMRG m_lock;
};

class MYPOSITION
{
public:
	MYPOSITION()
	{
		i = 0;
		in = 0;
	};

	void Clear()
	{
		i = 0;
		in = 0;
	};

	int i;
	long in;
};

class COFSMatrix
{
public:
	static const size_t _sidSize = SID_SIZE;

	COFSMatrix();
	~COFSMatrix();

	long GetCount();
	void GetNextAssoc(MYPOSITION &pos, long &id, LPTSTR sid, size_t sidSize, CActiveSession* &pData);
	BOOL GetStartPosition(MYPOSITION& pos);
	void Add(long id,LPCTSTR sid,CActiveSession* pData);
	CActiveSession* Get(long id, LPTSTR sid, size_t sidSize);
	CActiveSession* Get(LPCTSTR sid,long &id);
	CActiveSession* Remove(long id);
	CActiveSession* Remove(LPCTSTR sid);
	BOOL IsEmpty();

protected:
	void RemoveDbl(LPCTSTR sid);
	BOOL FindIdBySIDInDbl(LPCTSTR sid,long &id);
	void AddToDbl (long id, LPCTSTR sid);
	void ClearMatrix();
	void CloseMatrix();
	int GetSimNum(TCHAR sim);
	CMatrixItem Table[MAX_TABLE_SIZE];
	CMatrixDBLItem DBLTable[MAX_TABLE_SIZE];
	long _count; // Количестов Элементов в матрице
	CMatrixDBLItem* GetDblItemBySid(LPCTSTR sid);
};

#endif _OFS_MATRIX_H
