 // The following are message definitions.
//
//  Values are 32 bit values layed out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//
#define FACILITY_SYSTEM                  0x0
#define FACILITY_STUBS                   0x3
#define FACILITY_RUNTIME                 0x2
#define FACILITY_IO_ERROR_CODE           0x4


//
// Define the severity codes
//
#define STATUS_SEVERITY_WARNING          0x2
#define STATUS_SEVERITY_SUCCESS          0x0
#define STATUS_SEVERITY_INFORMATIONAL    0x1
#define STATUS_SEVERITY_ERROR            0x3


//
// MessageId: SERVER_STARTED
//
// MessageText:
//
//  IM Server '%1' started. 
//
#define SERVER_STARTED                   ((DWORD)0x40000258L)

//
// MessageId: UNABLE_TO_START
//
// MessageText:
//
//  IM Server '%1' was unable to start. Reason: %2
//
#define UNABLE_TO_START                  ((DWORD)0xC0000259L)

//
// MessageId: SERVER_STOPPED
//
// MessageText:
//
//  IM Server '%1' stopped.
//
#define SERVER_STOPPED                   ((DWORD)0x4000025AL)

//
// MessageId: MSG_CANNOT_START_PERFMON
//
// MessageText:
//
//  Cannot start performance monitor. Data is HRESULT.
//
#define MSG_CANNOT_START_PERFMON         ((DWORD)0x8000025BL)

//
// MessageId: FAILED_LOGIN
//
// MessageText:
//
//  Failed IM Portal '%1' login. %2
//
#define FAILED_LOGIN                     ((DWORD)0x80000193L)

