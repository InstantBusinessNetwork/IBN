#include "stdafx.h"
#include "resource.h"
#include "OFSMatrix.h"

//////////////////////////////////////////////////////////////////
inline int COFSMatrix::GetSimNum(TCHAR sim)
{
	switch(sim)
	{
	case _T('0'):return 0;
	case _T('1'):return 1;
	case _T('2'):return 2;
	case _T('3'):return 3;
	case _T('4'):return 4;
	case _T('5'):return 5;
	case _T('6'):return 6;
	case _T('7'):return 7;
	case _T('8'):return 8;
	case _T('9'):return 9;
	case _T('A'):return 10;
	case _T('B'):return 11;
	case _T('C'):return 12;
	case _T('D'):return 13;
	case _T('E'):return 14;
	case _T('F'):return 15;
	default: return 0xff;
	}
}
//////////////////////////////////////////////////////////////////
COFSMatrix::COFSMatrix()
{
	_count = 0;
}
//////////////////////////////////////////////////////////////////
COFSMatrix::~COFSMatrix()
{
	CloseMatrix();
}
//////////////////////////////////////////////////////////////////
void COFSMatrix::ClearMatrix()
{
}
//////////////////////////////////////////////////////////////////
void COFSMatrix::CloseMatrix()
{
	for(int i=0; i<MAX_TABLE_SIZE; i++)
	{
		CMatrixItem* pMatrixItem = &Table[i];
		pMatrixItem->WaitToWrite();

		std::list<_SPISOKITEM>::iterator j = pMatrixItem->m_ItemList.begin();
		while(j != pMatrixItem->m_ItemList.end())
		{
			try
			{
				delete (*j)._pData;
			}
			catch(...)
			{
			}
			j++;
		}

		try
		{
			pMatrixItem->m_ItemList.clear();
		}
		catch(...)
		{
			ASSERT(FALSE);
		}

		pMatrixItem->Done();
	}

	for(int i=0; i<MAX_TABLE_SIZE; i++)
	{
		DBLTable[i].WaitToWrite();

		try
		{
			DBLTable[i].m_ItemList.clear();
		}
		catch(...)
		{
			ASSERT(FALSE);
		}

		DBLTable[i].Done();
	}

	_count = 0;
}

//////////////////////////////////////////////////////////////////
BOOL COFSMatrix::IsEmpty( )
{
	return (_count > 0) ? FALSE : TRUE;
}
//////////////////////////////////////////////////////////////////
void COFSMatrix::Add(long id, LPCTSTR sid, CActiveSession* pData)
{
	if(!CheckSID(sid))
		throw long(ERR_WRONG_SID);

	/// Запись Строки в Массив по Id
	AddToDbl(id, sid);

	/// Элемента в матрицу
	int num = id % (MAX_TABLE_SIZE);

	CMatrixItem* pMatrixItem = &Table[num];

	pMatrixItem->WaitToRead();

	std::list<_SPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
	while(i != pMatrixItem->m_ItemList.end())
	{
		if((*i)._id == id)
		{
			pMatrixItem->Done();
			throw long(ERR_WRONG_SID);
		}
		i++;
	}

	pMatrixItem->Done();

	pMatrixItem->WaitToWrite();

	try
	{
		InterlockedIncrement(&_count);
		pMatrixItem->m_ItemList.push_back(_SPISOKITEM(id, sid, pData));
	}
	catch(...)
	{
		/// Генерить исключение Системная Ошибка Добавления
		pMatrixItem->Done();
		throw long(ERR_OUT_GLOBAL_1);
	}

	pMatrixItem->Done();
}

CActiveSession* COFSMatrix::Get(long id, LPTSTR sid, size_t sidSize)
{
	CActiveSession* pFindData = NULL;

	int num = id % (MAX_TABLE_SIZE);

	CMatrixItem* pMatrixItem = &Table[num];

	pMatrixItem->WaitToRead();

	try
	{
		std::list<_SPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
		while(i != pMatrixItem->m_ItemList.end())
		{
			if((*i)._id == id)
			{
				/// ура нашли ...
				if(sid != NULL)
					_tcscpy_s(sid, sidSize, (*i)._sid);
				pFindData = (*i)._pData->GetPointer();
				/// Выход из цикла
				break;
			}
			i++;
		}
	}
	catch(...)
	{
		/// Генерить исключение Системная Ошибка Добавления
		pMatrixItem->Done();
		throw long(ERR_OUT_GLOBAL_2);
	}

	pMatrixItem->Done();

	if(pFindData==NULL) 
		throw long(ERR_WRONG_ID);

	return pFindData;
}

CActiveSession* COFSMatrix::Get(LPCTSTR sid, long &id)
{
	/// Найти Id по SID
	if(!CheckSID(sid))
		throw long(ERR_WRONG_SID);

	if(!FindIdBySIDInDbl(sid, id))
		throw long(ERR_WRONG_SID);

	CActiveSession* pFind = Get(id, NULL, 0);

	return pFind;
}

CActiveSession* COFSMatrix::Remove(long id)
{
	CActiveSession* pFindData = NULL;

	int num = id % (MAX_TABLE_SIZE);

	CMatrixItem* pMatrixItem = &Table[num];
	/// не блокирована ли стока ???
	pMatrixItem->WaitToWrite();

	try
	{
		std::list<_SPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
		while(i != pMatrixItem->m_ItemList.end())
		{
			if((*i)._id == id)
			{
				InterlockedDecrement(&_count);
				/// ура нашли ...
				pFindData = (*i)._pData->GetPointer();
				TCHAR sid[_SPISOKITEM::_sidSize];
				_tcscpy_s(sid, _SPISOKITEM::_sidSize, (*i)._sid);
				pMatrixItem->m_ItemList.erase(i);
				RemoveDbl(sid);
				/// Выход из цикла
				break;
			}
			i++;
		}
	}
	catch(...)
	{
		/// Генерить исключение Системная Ошибка Добавления
		pMatrixItem->Done();
		throw long(ERR_OUT_GLOBAL_3);
	}

	pMatrixItem->Done();

	if(pFindData == NULL)
		throw long(ERR_WRONG_ID);

	return pFindData;
}

CActiveSession* COFSMatrix::Remove(LPCTSTR sid)
{
	/// Найти Id по SID
	long id = 0;
	if(FindIdBySIDInDbl(sid, id))
		return Remove(id);
	else
		throw long(ERR_WRONG_SID);
}


BOOL COFSMatrix::GetStartPosition(MYPOSITION& pos)
{
	pos.Clear();
	return (_count > 0) ? TRUE : FALSE;
}

void COFSMatrix::GetNextAssoc(MYPOSITION &pos, long &id, LPTSTR sid, size_t sidSize, CActiveSession* &pData)
{
	long in = 0;

	for(int i = pos.i; i < MAX_TABLE_SIZE; i++)
	{
		CMatrixItem* pMatrixItem = &Table[i];

		/// не блокирована ли стpока ???
		pMatrixItem->WaitToRead();

		try
		{
			std::list<_SPISOKITEM>::iterator iItem = pMatrixItem->m_ItemList.begin();
			in = 0;
			while(iItem != pMatrixItem->m_ItemList.end())
			{
				if(in >= pos.in)
				{
					_SPISOKITEM item = *iItem;
					id = item._id;
					pData = item._pData->GetPointer();
					_tcscpy_s(sid, sidSize, item._sid);
					pos.i = i;
					pos.in = in + 1;
					pMatrixItem->Done();
					return;
				}
				in++;
				iItem++;
			}
		}
		catch(...)
		{
		}

		pMatrixItem->Done();
		//////////////////////////////////////////////////////////////////
		pos.in = 0;
	}

	pos.Clear();
	id = -1;
	pData = NULL;
	sid[0] = _T('\0');
}

void COFSMatrix::AddToDbl(long id, LPCTSTR sid)
{
	CMatrixDBLItem* pMatrixItem = GetDblItemBySid(sid);
	if(pMatrixItem == NULL)
		throw long(ERR_WRONG_ID); /// Генерить исключение неправильные параметры :(

	pMatrixItem->WaitToRead();

	std::list<_DBLSPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
	while(i != pMatrixItem->m_ItemList.end())
	{
		if((*i)._id == id)
		{
			pMatrixItem->Done();
			throw long(ERR_WRONG_SID);
		}
		i++;
	}
	pMatrixItem->Done();

	pMatrixItem->WaitToWrite();
	try
	{
		pMatrixItem->m_ItemList.push_back(_DBLSPISOKITEM(id, sid));
	}
	catch(...)
	{
		/// Генерить исключение Системная Ошибка Добавления
		pMatrixItem->Done();
		throw long(ERR_OUT_GLOBAL_4);
	}

	pMatrixItem->Done();

	return;
}

BOOL COFSMatrix::FindIdBySIDInDbl(LPCTSTR sid, long &id)
{
	BOOL bRetValue = FALSE;

	CMatrixDBLItem* pMatrixItem = GetDblItemBySid(sid);
	if(pMatrixItem != NULL)
	{
		/// не блокирована ли стока ???
		pMatrixItem->WaitToRead();

		std::list<_DBLSPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
		while(i != pMatrixItem->m_ItemList.end())
		{
			if(0 == _tcscmp(sid, (*i)._sid))
			{
				id = (*i)._id;
				bRetValue = TRUE;
				break;
			}
			i++;
		}

		pMatrixItem->Done();
	}

	return bRetValue;
}

void COFSMatrix::RemoveDbl(LPCTSTR sid)
{
	CMatrixDBLItem* pMatrixItem = GetDblItemBySid(sid);
	if(pMatrixItem != NULL)
	{
		/// не блокирована ли стока ???
		pMatrixItem->WaitToWrite();

		try
		{
			std::list<_DBLSPISOKITEM>::iterator i = pMatrixItem->m_ItemList.begin();
			while(i!=pMatrixItem->m_ItemList.end())
			{
				if(0 == _tcscmp((*i)._sid, sid))
				{
					/// ура нашли ...
					pMatrixItem->m_ItemList.erase(i);
					/// Выход из цикла
					break;
				}
				i++;
			}
		}
		catch(...)
		{
			/// Генерить исключение Системная Ошибка Добавления
			pMatrixItem->Done();
			throw long(ERR_OUT_GLOBAL_5);
		}

		pMatrixItem->Done();
	}
}

long COFSMatrix::GetCount()
{
	return _count;
}

CMatrixDBLItem* COFSMatrix::GetDblItemBySid(LPCTSTR sid)
{
	CMatrixDBLItem* pItem = NULL;

	int num1 = GetSimNum(sid[0]);
	int num2 = GetSimNum(sid[1]);

	if(num1 != 0xff && num2 != 0xff)
		pItem = &DBLTable[num1 * 16 + num2];

	return pItem;
}
