

/* this ALWAYS GENERATED file contains the proxy stub code */


 /* File created by MIDL compiler version 6.00.0366 */
/* at Tue Aug 19 11:28:26 2008
 */
/* Compiler settings for .\McConverterSessionServer.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#if !defined(_M_IA64) && !defined(_M_AMD64)


#pragma warning( disable: 4049 )  /* more than 64k source lines */
#if _MSC_VER >= 1200
#pragma warning(push)
#endif
#pragma warning( disable: 4100 ) /* unreferenced arguments in x86 call */
#pragma warning( disable: 4211 )  /* redefine extent to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma optimize("", off ) 

#define USE_STUBLESS_PROXY


/* verify that the <rpcproxy.h> version is high enough to compile this file*/
#ifndef __REDQ_RPCPROXY_H_VERSION__
#define __REQUIRED_RPCPROXY_H_VERSION__ 440
#endif


#include "rpcproxy.h"
#ifndef __RPCPROXY_H_VERSION__
#error this stub requires an updated version of <rpcproxy.h>
#endif // __RPCPROXY_H_VERSION__


#include "McConverterSessionServer.h"

#define TYPE_FORMAT_STRING_SIZE   1033                              
#define PROC_FORMAT_STRING_SIZE   81                                
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   2            

typedef struct _MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } MIDL_TYPE_FORMAT_STRING;

typedef struct _MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } MIDL_PROC_FORMAT_STRING;


static RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const MIDL_TYPE_FORMAT_STRING __MIDL_TypeFormatString;
extern const MIDL_PROC_FORMAT_STRING __MIDL_ProcFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IConvertSessionWrapper_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IConvertSessionWrapper_ProxyInfo;


extern const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ];

#if !defined(__RPC_WIN32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT40_OR_LATER)
#error You need a Windows NT 4.0 or later to run this stub because it uses these features:
#error   -Oif or -Oicf, [wire_marshal] or [user_marshal] attribute.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will die there with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const MIDL_PROC_FORMAT_STRING __MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure SaveMapiMessage */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x7 ),	/* 7 */
/*  8 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 10 */	NdrFcShort( 0x8 ),	/* 8 */
/* 12 */	NdrFcShort( 0x8 ),	/* 8 */
/* 14 */	0x6,		/* Oi2 Flags:  clt must size, has return, */
			0x4,		/* 4 */

	/* Parameter bstrFileName */

/* 16 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 18 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 20 */	NdrFcShort( 0x1a ),	/* Type Offset=26 */

	/* Parameter pUnkMessage */

/* 22 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 24 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 26 */	NdrFcShort( 0x3ec ),	/* Type Offset=1004 */

	/* Parameter ulEncType */

/* 28 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 30 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 32 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 34 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 36 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 38 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure SaveMapiMessageTmpFile */

/* 40 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 42 */	NdrFcLong( 0x0 ),	/* 0 */
/* 46 */	NdrFcShort( 0x8 ),	/* 8 */
/* 48 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 50 */	NdrFcShort( 0x8 ),	/* 8 */
/* 52 */	NdrFcShort( 0x8 ),	/* 8 */
/* 54 */	0x7,		/* Oi2 Flags:  srv must size, clt must size, has return, */
			0x4,		/* 4 */

	/* Parameter pUnkMessage */

/* 56 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 58 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 60 */	NdrFcShort( 0x3ec ),	/* Type Offset=1004 */

	/* Parameter ulEncType */

/* 62 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 64 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 66 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter tmpFileName */

/* 68 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 70 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 72 */	NdrFcShort( 0x3fe ),	/* Type Offset=1022 */

	/* Return value */

/* 74 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 76 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 78 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const MIDL_TYPE_FORMAT_STRING __MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x12, 0x0,	/* FC_UP */
/*  4 */	NdrFcShort( 0xc ),	/* Offset= 12 (16) */
/*  6 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/*  8 */	NdrFcShort( 0x2 ),	/* 2 */
/* 10 */	0x9,		/* Corr desc: FC_ULONG */
			0x0,		/*  */
/* 12 */	NdrFcShort( 0xfffc ),	/* -4 */
/* 14 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 16 */	
			0x17,		/* FC_CSTRUCT */
			0x3,		/* 3 */
/* 18 */	NdrFcShort( 0x8 ),	/* 8 */
/* 20 */	NdrFcShort( 0xfff2 ),	/* Offset= -14 (6) */
/* 22 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 24 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 26 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 28 */	NdrFcShort( 0x0 ),	/* 0 */
/* 30 */	NdrFcShort( 0x4 ),	/* 4 */
/* 32 */	NdrFcShort( 0x0 ),	/* 0 */
/* 34 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (2) */
/* 36 */	
			0x12, 0x0,	/* FC_UP */
/* 38 */	NdrFcShort( 0x3b2 ),	/* Offset= 946 (984) */
/* 40 */	
			0x2b,		/* FC_NON_ENCAPSULATED_UNION */
			0x9,		/* FC_ULONG */
/* 42 */	0x7,		/* Corr desc: FC_USHORT */
			0x0,		/*  */
/* 44 */	NdrFcShort( 0xfff8 ),	/* -8 */
/* 46 */	NdrFcShort( 0x2 ),	/* Offset= 2 (48) */
/* 48 */	NdrFcShort( 0x10 ),	/* 16 */
/* 50 */	NdrFcShort( 0x2f ),	/* 47 */
/* 52 */	NdrFcLong( 0x14 ),	/* 20 */
/* 56 */	NdrFcShort( 0x800b ),	/* Simple arm type: FC_HYPER */
/* 58 */	NdrFcLong( 0x3 ),	/* 3 */
/* 62 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 64 */	NdrFcLong( 0x11 ),	/* 17 */
/* 68 */	NdrFcShort( 0x8001 ),	/* Simple arm type: FC_BYTE */
/* 70 */	NdrFcLong( 0x2 ),	/* 2 */
/* 74 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 76 */	NdrFcLong( 0x4 ),	/* 4 */
/* 80 */	NdrFcShort( 0x800a ),	/* Simple arm type: FC_FLOAT */
/* 82 */	NdrFcLong( 0x5 ),	/* 5 */
/* 86 */	NdrFcShort( 0x800c ),	/* Simple arm type: FC_DOUBLE */
/* 88 */	NdrFcLong( 0xb ),	/* 11 */
/* 92 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 94 */	NdrFcLong( 0xa ),	/* 10 */
/* 98 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 100 */	NdrFcLong( 0x6 ),	/* 6 */
/* 104 */	NdrFcShort( 0xe8 ),	/* Offset= 232 (336) */
/* 106 */	NdrFcLong( 0x7 ),	/* 7 */
/* 110 */	NdrFcShort( 0x800c ),	/* Simple arm type: FC_DOUBLE */
/* 112 */	NdrFcLong( 0x8 ),	/* 8 */
/* 116 */	NdrFcShort( 0xff8e ),	/* Offset= -114 (2) */
/* 118 */	NdrFcLong( 0xd ),	/* 13 */
/* 122 */	NdrFcShort( 0xdc ),	/* Offset= 220 (342) */
/* 124 */	NdrFcLong( 0x9 ),	/* 9 */
/* 128 */	NdrFcShort( 0xe8 ),	/* Offset= 232 (360) */
/* 130 */	NdrFcLong( 0x2000 ),	/* 8192 */
/* 134 */	NdrFcShort( 0xf4 ),	/* Offset= 244 (378) */
/* 136 */	NdrFcLong( 0x24 ),	/* 36 */
/* 140 */	NdrFcShort( 0x302 ),	/* Offset= 770 (910) */
/* 142 */	NdrFcLong( 0x4024 ),	/* 16420 */
/* 146 */	NdrFcShort( 0x2fc ),	/* Offset= 764 (910) */
/* 148 */	NdrFcLong( 0x4011 ),	/* 16401 */
/* 152 */	NdrFcShort( 0x2fa ),	/* Offset= 762 (914) */
/* 154 */	NdrFcLong( 0x4002 ),	/* 16386 */
/* 158 */	NdrFcShort( 0x2f8 ),	/* Offset= 760 (918) */
/* 160 */	NdrFcLong( 0x4003 ),	/* 16387 */
/* 164 */	NdrFcShort( 0x2f6 ),	/* Offset= 758 (922) */
/* 166 */	NdrFcLong( 0x4014 ),	/* 16404 */
/* 170 */	NdrFcShort( 0x2f4 ),	/* Offset= 756 (926) */
/* 172 */	NdrFcLong( 0x4004 ),	/* 16388 */
/* 176 */	NdrFcShort( 0x2f2 ),	/* Offset= 754 (930) */
/* 178 */	NdrFcLong( 0x4005 ),	/* 16389 */
/* 182 */	NdrFcShort( 0x2f0 ),	/* Offset= 752 (934) */
/* 184 */	NdrFcLong( 0x400b ),	/* 16395 */
/* 188 */	NdrFcShort( 0x2da ),	/* Offset= 730 (918) */
/* 190 */	NdrFcLong( 0x400a ),	/* 16394 */
/* 194 */	NdrFcShort( 0x2d8 ),	/* Offset= 728 (922) */
/* 196 */	NdrFcLong( 0x4006 ),	/* 16390 */
/* 200 */	NdrFcShort( 0x2e2 ),	/* Offset= 738 (938) */
/* 202 */	NdrFcLong( 0x4007 ),	/* 16391 */
/* 206 */	NdrFcShort( 0x2d8 ),	/* Offset= 728 (934) */
/* 208 */	NdrFcLong( 0x4008 ),	/* 16392 */
/* 212 */	NdrFcShort( 0x2da ),	/* Offset= 730 (942) */
/* 214 */	NdrFcLong( 0x400d ),	/* 16397 */
/* 218 */	NdrFcShort( 0x2d8 ),	/* Offset= 728 (946) */
/* 220 */	NdrFcLong( 0x4009 ),	/* 16393 */
/* 224 */	NdrFcShort( 0x2d6 ),	/* Offset= 726 (950) */
/* 226 */	NdrFcLong( 0x6000 ),	/* 24576 */
/* 230 */	NdrFcShort( 0x2d4 ),	/* Offset= 724 (954) */
/* 232 */	NdrFcLong( 0x400c ),	/* 16396 */
/* 236 */	NdrFcShort( 0x2d2 ),	/* Offset= 722 (958) */
/* 238 */	NdrFcLong( 0x10 ),	/* 16 */
/* 242 */	NdrFcShort( 0x8002 ),	/* Simple arm type: FC_CHAR */
/* 244 */	NdrFcLong( 0x12 ),	/* 18 */
/* 248 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 250 */	NdrFcLong( 0x13 ),	/* 19 */
/* 254 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 256 */	NdrFcLong( 0x15 ),	/* 21 */
/* 260 */	NdrFcShort( 0x800b ),	/* Simple arm type: FC_HYPER */
/* 262 */	NdrFcLong( 0x16 ),	/* 22 */
/* 266 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 268 */	NdrFcLong( 0x17 ),	/* 23 */
/* 272 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 274 */	NdrFcLong( 0xe ),	/* 14 */
/* 278 */	NdrFcShort( 0x2b0 ),	/* Offset= 688 (966) */
/* 280 */	NdrFcLong( 0x400e ),	/* 16398 */
/* 284 */	NdrFcShort( 0x2b4 ),	/* Offset= 692 (976) */
/* 286 */	NdrFcLong( 0x4010 ),	/* 16400 */
/* 290 */	NdrFcShort( 0x2b2 ),	/* Offset= 690 (980) */
/* 292 */	NdrFcLong( 0x4012 ),	/* 16402 */
/* 296 */	NdrFcShort( 0x26e ),	/* Offset= 622 (918) */
/* 298 */	NdrFcLong( 0x4013 ),	/* 16403 */
/* 302 */	NdrFcShort( 0x26c ),	/* Offset= 620 (922) */
/* 304 */	NdrFcLong( 0x4015 ),	/* 16405 */
/* 308 */	NdrFcShort( 0x26a ),	/* Offset= 618 (926) */
/* 310 */	NdrFcLong( 0x4016 ),	/* 16406 */
/* 314 */	NdrFcShort( 0x260 ),	/* Offset= 608 (922) */
/* 316 */	NdrFcLong( 0x4017 ),	/* 16407 */
/* 320 */	NdrFcShort( 0x25a ),	/* Offset= 602 (922) */
/* 322 */	NdrFcLong( 0x0 ),	/* 0 */
/* 326 */	NdrFcShort( 0x0 ),	/* Offset= 0 (326) */
/* 328 */	NdrFcLong( 0x1 ),	/* 1 */
/* 332 */	NdrFcShort( 0x0 ),	/* Offset= 0 (332) */
/* 334 */	NdrFcShort( 0xffff ),	/* Offset= -1 (333) */
/* 336 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 338 */	NdrFcShort( 0x8 ),	/* 8 */
/* 340 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 342 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 344 */	NdrFcLong( 0x0 ),	/* 0 */
/* 348 */	NdrFcShort( 0x0 ),	/* 0 */
/* 350 */	NdrFcShort( 0x0 ),	/* 0 */
/* 352 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 354 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 356 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 358 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 360 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 362 */	NdrFcLong( 0x20400 ),	/* 132096 */
/* 366 */	NdrFcShort( 0x0 ),	/* 0 */
/* 368 */	NdrFcShort( 0x0 ),	/* 0 */
/* 370 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 372 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 374 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 376 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 378 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 380 */	NdrFcShort( 0x2 ),	/* Offset= 2 (382) */
/* 382 */	
			0x12, 0x0,	/* FC_UP */
/* 384 */	NdrFcShort( 0x1fc ),	/* Offset= 508 (892) */
/* 386 */	
			0x2a,		/* FC_ENCAPSULATED_UNION */
			0x49,		/* 73 */
/* 388 */	NdrFcShort( 0x18 ),	/* 24 */
/* 390 */	NdrFcShort( 0xa ),	/* 10 */
/* 392 */	NdrFcLong( 0x8 ),	/* 8 */
/* 396 */	NdrFcShort( 0x58 ),	/* Offset= 88 (484) */
/* 398 */	NdrFcLong( 0xd ),	/* 13 */
/* 402 */	NdrFcShort( 0x78 ),	/* Offset= 120 (522) */
/* 404 */	NdrFcLong( 0x9 ),	/* 9 */
/* 408 */	NdrFcShort( 0x94 ),	/* Offset= 148 (556) */
/* 410 */	NdrFcLong( 0xc ),	/* 12 */
/* 414 */	NdrFcShort( 0xbc ),	/* Offset= 188 (602) */
/* 416 */	NdrFcLong( 0x24 ),	/* 36 */
/* 420 */	NdrFcShort( 0x114 ),	/* Offset= 276 (696) */
/* 422 */	NdrFcLong( 0x800d ),	/* 32781 */
/* 426 */	NdrFcShort( 0x130 ),	/* Offset= 304 (730) */
/* 428 */	NdrFcLong( 0x10 ),	/* 16 */
/* 432 */	NdrFcShort( 0x148 ),	/* Offset= 328 (760) */
/* 434 */	NdrFcLong( 0x2 ),	/* 2 */
/* 438 */	NdrFcShort( 0x160 ),	/* Offset= 352 (790) */
/* 440 */	NdrFcLong( 0x3 ),	/* 3 */
/* 444 */	NdrFcShort( 0x178 ),	/* Offset= 376 (820) */
/* 446 */	NdrFcLong( 0x14 ),	/* 20 */
/* 450 */	NdrFcShort( 0x190 ),	/* Offset= 400 (850) */
/* 452 */	NdrFcShort( 0xffff ),	/* Offset= -1 (451) */
/* 454 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 456 */	NdrFcShort( 0x4 ),	/* 4 */
/* 458 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 460 */	NdrFcShort( 0x0 ),	/* 0 */
/* 462 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 464 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 466 */	NdrFcShort( 0x4 ),	/* 4 */
/* 468 */	NdrFcShort( 0x0 ),	/* 0 */
/* 470 */	NdrFcShort( 0x1 ),	/* 1 */
/* 472 */	NdrFcShort( 0x0 ),	/* 0 */
/* 474 */	NdrFcShort( 0x0 ),	/* 0 */
/* 476 */	0x12, 0x0,	/* FC_UP */
/* 478 */	NdrFcShort( 0xfe32 ),	/* Offset= -462 (16) */
/* 480 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 482 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 484 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 486 */	NdrFcShort( 0x8 ),	/* 8 */
/* 488 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 490 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 492 */	NdrFcShort( 0x4 ),	/* 4 */
/* 494 */	NdrFcShort( 0x4 ),	/* 4 */
/* 496 */	0x11, 0x0,	/* FC_RP */
/* 498 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (454) */
/* 500 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 502 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 504 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 506 */	NdrFcShort( 0x0 ),	/* 0 */
/* 508 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 510 */	NdrFcShort( 0x0 ),	/* 0 */
/* 512 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 516 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 518 */	NdrFcShort( 0xff50 ),	/* Offset= -176 (342) */
/* 520 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 522 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 524 */	NdrFcShort( 0x8 ),	/* 8 */
/* 526 */	NdrFcShort( 0x0 ),	/* 0 */
/* 528 */	NdrFcShort( 0x6 ),	/* Offset= 6 (534) */
/* 530 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 532 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 534 */	
			0x11, 0x0,	/* FC_RP */
/* 536 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (504) */
/* 538 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 540 */	NdrFcShort( 0x0 ),	/* 0 */
/* 542 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 544 */	NdrFcShort( 0x0 ),	/* 0 */
/* 546 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 550 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 552 */	NdrFcShort( 0xff40 ),	/* Offset= -192 (360) */
/* 554 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 556 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 558 */	NdrFcShort( 0x8 ),	/* 8 */
/* 560 */	NdrFcShort( 0x0 ),	/* 0 */
/* 562 */	NdrFcShort( 0x6 ),	/* Offset= 6 (568) */
/* 564 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 566 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 568 */	
			0x11, 0x0,	/* FC_RP */
/* 570 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (538) */
/* 572 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 574 */	NdrFcShort( 0x4 ),	/* 4 */
/* 576 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 578 */	NdrFcShort( 0x0 ),	/* 0 */
/* 580 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 582 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 584 */	NdrFcShort( 0x4 ),	/* 4 */
/* 586 */	NdrFcShort( 0x0 ),	/* 0 */
/* 588 */	NdrFcShort( 0x1 ),	/* 1 */
/* 590 */	NdrFcShort( 0x0 ),	/* 0 */
/* 592 */	NdrFcShort( 0x0 ),	/* 0 */
/* 594 */	0x12, 0x0,	/* FC_UP */
/* 596 */	NdrFcShort( 0x184 ),	/* Offset= 388 (984) */
/* 598 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 600 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 602 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 604 */	NdrFcShort( 0x8 ),	/* 8 */
/* 606 */	NdrFcShort( 0x0 ),	/* 0 */
/* 608 */	NdrFcShort( 0x6 ),	/* Offset= 6 (614) */
/* 610 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 612 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 614 */	
			0x11, 0x0,	/* FC_RP */
/* 616 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (572) */
/* 618 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 620 */	NdrFcLong( 0x2f ),	/* 47 */
/* 624 */	NdrFcShort( 0x0 ),	/* 0 */
/* 626 */	NdrFcShort( 0x0 ),	/* 0 */
/* 628 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 630 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 632 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 634 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 636 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 638 */	NdrFcShort( 0x1 ),	/* 1 */
/* 640 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 642 */	NdrFcShort( 0x4 ),	/* 4 */
/* 644 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 646 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 648 */	NdrFcShort( 0x10 ),	/* 16 */
/* 650 */	NdrFcShort( 0x0 ),	/* 0 */
/* 652 */	NdrFcShort( 0xa ),	/* Offset= 10 (662) */
/* 654 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 656 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 658 */	NdrFcShort( 0xffd8 ),	/* Offset= -40 (618) */
/* 660 */	0x36,		/* FC_POINTER */
			0x5b,		/* FC_END */
/* 662 */	
			0x12, 0x0,	/* FC_UP */
/* 664 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (636) */
/* 666 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 668 */	NdrFcShort( 0x4 ),	/* 4 */
/* 670 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 672 */	NdrFcShort( 0x0 ),	/* 0 */
/* 674 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 676 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 678 */	NdrFcShort( 0x4 ),	/* 4 */
/* 680 */	NdrFcShort( 0x0 ),	/* 0 */
/* 682 */	NdrFcShort( 0x1 ),	/* 1 */
/* 684 */	NdrFcShort( 0x0 ),	/* 0 */
/* 686 */	NdrFcShort( 0x0 ),	/* 0 */
/* 688 */	0x12, 0x0,	/* FC_UP */
/* 690 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (646) */
/* 692 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 694 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 696 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 698 */	NdrFcShort( 0x8 ),	/* 8 */
/* 700 */	NdrFcShort( 0x0 ),	/* 0 */
/* 702 */	NdrFcShort( 0x6 ),	/* Offset= 6 (708) */
/* 704 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 706 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 708 */	
			0x11, 0x0,	/* FC_RP */
/* 710 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (666) */
/* 712 */	
			0x1d,		/* FC_SMFARRAY */
			0x0,		/* 0 */
/* 714 */	NdrFcShort( 0x8 ),	/* 8 */
/* 716 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 718 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 720 */	NdrFcShort( 0x10 ),	/* 16 */
/* 722 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 724 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 726 */	0x0,		/* 0 */
			NdrFcShort( 0xfff1 ),	/* Offset= -15 (712) */
			0x5b,		/* FC_END */
/* 730 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 732 */	NdrFcShort( 0x18 ),	/* 24 */
/* 734 */	NdrFcShort( 0x0 ),	/* 0 */
/* 736 */	NdrFcShort( 0xa ),	/* Offset= 10 (746) */
/* 738 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 740 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 742 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (718) */
/* 744 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 746 */	
			0x11, 0x0,	/* FC_RP */
/* 748 */	NdrFcShort( 0xff0c ),	/* Offset= -244 (504) */
/* 750 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 752 */	NdrFcShort( 0x1 ),	/* 1 */
/* 754 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 756 */	NdrFcShort( 0x0 ),	/* 0 */
/* 758 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 760 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 762 */	NdrFcShort( 0x8 ),	/* 8 */
/* 764 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 766 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 768 */	NdrFcShort( 0x4 ),	/* 4 */
/* 770 */	NdrFcShort( 0x4 ),	/* 4 */
/* 772 */	0x12, 0x0,	/* FC_UP */
/* 774 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (750) */
/* 776 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 778 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 780 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 782 */	NdrFcShort( 0x2 ),	/* 2 */
/* 784 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 786 */	NdrFcShort( 0x0 ),	/* 0 */
/* 788 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 790 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 792 */	NdrFcShort( 0x8 ),	/* 8 */
/* 794 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 796 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 798 */	NdrFcShort( 0x4 ),	/* 4 */
/* 800 */	NdrFcShort( 0x4 ),	/* 4 */
/* 802 */	0x12, 0x0,	/* FC_UP */
/* 804 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (780) */
/* 806 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 808 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 810 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 812 */	NdrFcShort( 0x4 ),	/* 4 */
/* 814 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 816 */	NdrFcShort( 0x0 ),	/* 0 */
/* 818 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 820 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 822 */	NdrFcShort( 0x8 ),	/* 8 */
/* 824 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 826 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 828 */	NdrFcShort( 0x4 ),	/* 4 */
/* 830 */	NdrFcShort( 0x4 ),	/* 4 */
/* 832 */	0x12, 0x0,	/* FC_UP */
/* 834 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (810) */
/* 836 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 838 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 840 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 842 */	NdrFcShort( 0x8 ),	/* 8 */
/* 844 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 846 */	NdrFcShort( 0x0 ),	/* 0 */
/* 848 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 850 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 852 */	NdrFcShort( 0x8 ),	/* 8 */
/* 854 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 856 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 858 */	NdrFcShort( 0x4 ),	/* 4 */
/* 860 */	NdrFcShort( 0x4 ),	/* 4 */
/* 862 */	0x12, 0x0,	/* FC_UP */
/* 864 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (840) */
/* 866 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 868 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 870 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 872 */	NdrFcShort( 0x8 ),	/* 8 */
/* 874 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 876 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 878 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 880 */	NdrFcShort( 0x8 ),	/* 8 */
/* 882 */	0x7,		/* Corr desc: FC_USHORT */
			0x0,		/*  */
/* 884 */	NdrFcShort( 0xffd8 ),	/* -40 */
/* 886 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 888 */	NdrFcShort( 0xffee ),	/* Offset= -18 (870) */
/* 890 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 892 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 894 */	NdrFcShort( 0x28 ),	/* 40 */
/* 896 */	NdrFcShort( 0xffee ),	/* Offset= -18 (878) */
/* 898 */	NdrFcShort( 0x0 ),	/* Offset= 0 (898) */
/* 900 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 902 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 904 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 906 */	NdrFcShort( 0xfdf8 ),	/* Offset= -520 (386) */
/* 908 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 910 */	
			0x12, 0x0,	/* FC_UP */
/* 912 */	NdrFcShort( 0xfef6 ),	/* Offset= -266 (646) */
/* 914 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 916 */	0x1,		/* FC_BYTE */
			0x5c,		/* FC_PAD */
/* 918 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 920 */	0x6,		/* FC_SHORT */
			0x5c,		/* FC_PAD */
/* 922 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 924 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 926 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 928 */	0xb,		/* FC_HYPER */
			0x5c,		/* FC_PAD */
/* 930 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 932 */	0xa,		/* FC_FLOAT */
			0x5c,		/* FC_PAD */
/* 934 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 936 */	0xc,		/* FC_DOUBLE */
			0x5c,		/* FC_PAD */
/* 938 */	
			0x12, 0x0,	/* FC_UP */
/* 940 */	NdrFcShort( 0xfda4 ),	/* Offset= -604 (336) */
/* 942 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 944 */	NdrFcShort( 0xfc52 ),	/* Offset= -942 (2) */
/* 946 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 948 */	NdrFcShort( 0xfda2 ),	/* Offset= -606 (342) */
/* 950 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 952 */	NdrFcShort( 0xfdb0 ),	/* Offset= -592 (360) */
/* 954 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 956 */	NdrFcShort( 0xfdbe ),	/* Offset= -578 (378) */
/* 958 */	
			0x12, 0x10,	/* FC_UP [pointer_deref] */
/* 960 */	NdrFcShort( 0x2 ),	/* Offset= 2 (962) */
/* 962 */	
			0x12, 0x0,	/* FC_UP */
/* 964 */	NdrFcShort( 0x14 ),	/* Offset= 20 (984) */
/* 966 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 968 */	NdrFcShort( 0x10 ),	/* 16 */
/* 970 */	0x6,		/* FC_SHORT */
			0x1,		/* FC_BYTE */
/* 972 */	0x1,		/* FC_BYTE */
			0x8,		/* FC_LONG */
/* 974 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 976 */	
			0x12, 0x0,	/* FC_UP */
/* 978 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (966) */
/* 980 */	
			0x12, 0x8,	/* FC_UP [simple_pointer] */
/* 982 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/* 984 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 986 */	NdrFcShort( 0x20 ),	/* 32 */
/* 988 */	NdrFcShort( 0x0 ),	/* 0 */
/* 990 */	NdrFcShort( 0x0 ),	/* Offset= 0 (990) */
/* 992 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 994 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 996 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 998 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1000 */	NdrFcShort( 0xfc40 ),	/* Offset= -960 (40) */
/* 1002 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1004 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 1006 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1008 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1010 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1012 */	NdrFcShort( 0xfc30 ),	/* Offset= -976 (36) */
/* 1014 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 1016 */	NdrFcShort( 0x6 ),	/* Offset= 6 (1022) */
/* 1018 */	
			0x13, 0x0,	/* FC_OP */
/* 1020 */	NdrFcShort( 0xfc14 ),	/* Offset= -1004 (16) */
/* 1022 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 1024 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1026 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1028 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1030 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (1018) */

			0x0
        }
    };

static const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ] = 
        {
            
            {
            BSTR_UserSize
            ,BSTR_UserMarshal
            ,BSTR_UserUnmarshal
            ,BSTR_UserFree
            },
            {
            VARIANT_UserSize
            ,VARIANT_UserMarshal
            ,VARIANT_UserUnmarshal
            ,VARIANT_UserFree
            }

        };



/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IDispatch, ver. 0.0,
   GUID={0x00020400,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IConvertSessionWrapper, ver. 0.0,
   GUID={0xE8960C15,0x6EE6,0x492D,{0xB0,0x74,0xE2,0x33,0x65,0x27,0x53,0xBB}} */

#pragma code_seg(".orpc")
static const unsigned short IConvertSessionWrapper_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0,
    40
    };

static const MIDL_STUBLESS_PROXY_INFO IConvertSessionWrapper_ProxyInfo =
    {
    &Object_StubDesc,
    __MIDL_ProcFormatString.Format,
    &IConvertSessionWrapper_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IConvertSessionWrapper_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    __MIDL_ProcFormatString.Format,
    &IConvertSessionWrapper_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(9) _IConvertSessionWrapperProxyVtbl = 
{
    &IConvertSessionWrapper_ProxyInfo,
    &IID_IConvertSessionWrapper,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* (void *) (INT_PTR) -1 /* IDispatch::GetTypeInfoCount */ ,
    0 /* (void *) (INT_PTR) -1 /* IDispatch::GetTypeInfo */ ,
    0 /* (void *) (INT_PTR) -1 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IConvertSessionWrapper::SaveMapiMessage */ ,
    (void *) (INT_PTR) -1 /* IConvertSessionWrapper::SaveMapiMessageTmpFile */
};


static const PRPC_STUB_FUNCTION IConvertSessionWrapper_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IConvertSessionWrapperStubVtbl =
{
    &IID_IConvertSessionWrapper,
    &IConvertSessionWrapper_ServerInfo,
    9,
    &IConvertSessionWrapper_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};

static const MIDL_STUB_DESC Object_StubDesc = 
    {
    0,
    NdrOleAllocate,
    NdrOleFree,
    0,
    0,
    0,
    0,
    0,
    __MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x20000, /* Ndr library version */
    0,
    0x600016e, /* MIDL Version 6.0.366 */
    0,
    UserMarshalRoutines,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0   /* Reserved5 */
    };

const CInterfaceProxyVtbl * _McConverterSessionServer_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &_IConvertSessionWrapperProxyVtbl,
    0
};

const CInterfaceStubVtbl * _McConverterSessionServer_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &_IConvertSessionWrapperStubVtbl,
    0
};

PCInterfaceName const _McConverterSessionServer_InterfaceNamesList[] = 
{
    "IConvertSessionWrapper",
    0
};

const IID *  _McConverterSessionServer_BaseIIDList[] = 
{
    &IID_IDispatch,
    0
};


#define _McConverterSessionServer_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _McConverterSessionServer, pIID, n)

int __stdcall _McConverterSessionServer_IID_Lookup( const IID * pIID, int * pIndex )
{
    
    if(!_McConverterSessionServer_CHECK_IID(0))
        {
        *pIndex = 0;
        return 1;
        }

    return 0;
}

const ExtendedProxyFileInfo McConverterSessionServer_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _McConverterSessionServer_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _McConverterSessionServer_StubVtblList,
    (const PCInterfaceName * ) & _McConverterSessionServer_InterfaceNamesList,
    (const IID ** ) & _McConverterSessionServer_BaseIIDList,
    & _McConverterSessionServer_IID_Lookup, 
    1,
    2,
    0, /* table of [async_uuid] interfaces */
    0, /* Filler1 */
    0, /* Filler2 */
    0  /* Filler3 */
};
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* !defined(_M_IA64) && !defined(_M_AMD64)*/

