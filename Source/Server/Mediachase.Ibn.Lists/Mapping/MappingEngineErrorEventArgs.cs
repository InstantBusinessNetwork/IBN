using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta.Management;
using System.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Lists.Mapping
{
    public class MappingEngineErrorEventArgs : EventArgs
    {
        private readonly MappingError _error;

        private bool _resolveError = false;

        public MappingRule MappingRule;
        public MappingElement MappingElement;
        public MetaObject MetaObject;
       
        public MappingEngineErrorEventArgs(MappingError error)
        {
            _error = error;
        }
        
        public MappingError  Error
        {
            get 
             { 
                 return _error; 
             }
        }

        public bool ResolveError
        {
            get
            {
                return _resolveError;
            }

            set
            {
                _resolveError = value;
            }
        }
	

    }
}
