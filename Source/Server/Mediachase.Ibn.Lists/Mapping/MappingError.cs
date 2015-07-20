using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Lists.Mapping
{
	[Serializable]
    public class MappingError
    {
        public MappingErrorType ErrorType = MappingErrorType.UnknowError;
        public String  ErrorDescr;
        public DataRow Row;
        public DataColumn Column;
        public DataTable Table;
        public Exception Exception;

        public MappingError(MappingErrorType type, String descr)
        {
            ErrorType = type;
            ErrorDescr = descr;
        }

        public MappingError(MappingErrorType type, String descr, DataRow row)
        {
            ErrorType = type;
            ErrorDescr = descr;
            Row = row;
        }

        public override string ToString()
        {
            return null;
        }
               
   
	

    }
}
