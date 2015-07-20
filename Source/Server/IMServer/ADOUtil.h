// ADOUtil.h: interface for the CADOUtil class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ADOUTIL_H__9B830484_D748_4A00_B550_4009D28E2553__INCLUDED_)
#define AFX_ADOUTIL_H__9B830484_D748_4A00_B550_4009D28E2553__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define ERR_UNABLE_CREATE_CONN          (LONG)108
#define ERR_UNABLE_CREATE_COMM          (LONG)109
#define ERR_UNABLE_CREATE_RECSET        (LONG)110
#define ERR_SQL_UNKNOWN_PROBLEM         (LONG)115
#define ERR_PRIMARY_KEY_CONSTRAINT      (LONG)116

#define ERR_NEW_PROCESS_DETECTED          (LONG)1001


class CParam
{
public:
	CParam();
	CParam(DataTypeEnum in_Type,ParameterDirectionEnum in_Direction,long in_Size);
	~CParam();

public:
	DataTypeEnum			Type;
	ParameterDirectionEnum	Direction;
	long					Size;
	_variant_t				Value;
};

//////////////////////////////////////////////
//Params for Run SP
class CParamArray
{
public:
	CParamArray();
	virtual ~CParamArray();
public:
	void AddGUID(BSTR Value);
	void AddLong(long Value);
	void AddBSTR50(BSTR Value);
	void RemoveAll();
	CParam* operator [](int i) {return &m_ParamArray[i];}
	int GetSize() {return m_LastIndex;}
	void Add(DataTypeEnum in_Type,ParameterDirectionEnum in_Direction,long in_Size, _variant_t Value);
private:
	CParam m_ParamArray[10];
	long	m_LastIndex;
};

class CADOUtil  
{
public:
	CADOUtil();
	virtual ~CADOUtil();

	static void RunSP_ReturnRS(_bstr_t SP_Name, _RecordsetPtr& out_pRs, CParamArray *ParamArray);
	static LONG RunSP_ReturnLong(_bstr_t SP_Name, CParamArray* ParamArray = NULL);
	static BOOL CheckRecordSetState(_RecordsetPtr RecSet);
	static TCHAR szConnectionString[500];
	static BOOL bGlobalCompany;
	static TCHAR szCompanyId[500];

	// Oleg Zhuk Addon: [7/16/2004]
	static long RunCommand_ReturnLong(_bstr_t CmdText, CParamArray *ParamArray);

	static void LockDatabase();
protected:
	static BOOL CheckCurrentProcess();
};

#endif // !defined(AFX_ADOUTIL_H__9B830484_D748_4A00_B550_4009D28E2553__INCLUDED_)
