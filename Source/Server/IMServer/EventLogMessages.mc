MessageIdTypedef=DWORD

SeverityNames=(Success=0x0:STATUS_SEVERITY_SUCCESS
    Informational=0x1:STATUS_SEVERITY_INFORMATIONAL
    Warning=0x2:STATUS_SEVERITY_WARNING
    Error=0x3:STATUS_SEVERITY_ERROR
    )


FacilityNames=(System=0x0:FACILITY_SYSTEM
    Runtime=0x2:FACILITY_RUNTIME
    Stubs=0x3:FACILITY_STUBS
    Io=0x4:FACILITY_IO_ERROR_CODE
)

LanguageNames=(English=0x409:MSG00409)

; // The following are message definitions.

MessageId=600
Severity=Informational
Facility=System
SymbolicName=SERVER_STARTED
Language=English
IM Server '%1' started. 
.

MessageId=601
Severity=Error
Facility=System
SymbolicName=UNABLE_TO_START
Language=English
IM Server '%1' was unable to start. Reason: %2
.

MessageId=602
Severity=Informational
Facility=System
SymbolicName=SERVER_STOPPED
Language=English
IM Server '%1' stopped.
.

MessageId=603
Severity=Warning
Facility=System
SymbolicName=MSG_CANNOT_START_PERFMON
Language=English
Cannot start performance monitor. Data is HRESULT.
.

MessageId=403
Severity=Warning
Facility=System
SymbolicName=FAILED_LOGIN
Language=English
Failed IM Portal '%1' login. %2
.
