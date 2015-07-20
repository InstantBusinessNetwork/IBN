#pragma once

#include "ADOUtil.h"

class CDBFile
{
public:
	CDBFile(void);
	static LONG CreateFile(BSTR fid);
	static void WriteFile(LONG id, LPBYTE data, LONG datasize);
	static void DeleteFile(BSTR fid);
	~CDBFile(void);	
};