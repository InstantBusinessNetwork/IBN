#include "stdafx.h"
#include "dbfile.h"

CDBFile::CDBFile(void)
{	
}

CDBFile::~CDBFile(void)
{	
}

LONG CDBFile::CreateFile(BSTR fid)
{
	CParamArray params;
	params.AddGUID(fid);	
	LONG id = CADOUtil::RunSP_ReturnLong(_bstr_t("OM_BINARYCREATE"), &params);	
	return id;
}

void CDBFile::WriteFile(LONG id, LPBYTE data, LONG datasize)
{
	LONG wMaxWrite = 102400;
	CParamArray params;
	LONG lWritten = 0;
	LONG toWrite = 0;
	VARIANT var;
	VariantInit(&var);
	var.vt = VT_ARRAY | VT_UI1;

	LPSAFEARRAY psa = SafeArrayCreateVector(VT_UI1, 0, wMaxWrite);
	var.parray = psa;		

	while(lWritten<datasize)
	{
		toWrite = min(wMaxWrite, datasize-lWritten);
		params.RemoveAll();
		params.AddLong(id);	

		LPBYTE pTmpData = NULL;
		SafeArrayAccessData(psa,(LPVOID*)&pTmpData);
		CopyMemory(pTmpData, data+lWritten, toWrite);
		SafeArrayUnaccessData(psa);

		psa->rgsabound[0].cElements = toWrite;

		params.Add(adLongVarBinary, adParamInput, toWrite, var);
		CADOUtil::RunSP_ReturnLong(_bstr_t("OM_BINARYWRITE"), &params);		
		
		psa->rgsabound[0].cElements = wMaxWrite;
		lWritten += toWrite;
	}

	VariantClear(&var);
}

void CDBFile::DeleteFile(BSTR fid)
{
	CParamArray params;
	params.AddGUID(fid);	
	CADOUtil::RunSP_ReturnLong(_bstr_t("OM_BINARYDELETE"), &params);
}