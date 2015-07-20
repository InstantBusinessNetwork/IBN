using System;
using System.Collections.Generic;
using System.Text;

namespace IbnServerUpdate
{
	enum Error
	{
		Unknown = -1,
		OK = 0,
		InvalidArgs = 10001,
		UpdateMainDB = 10002,
		UpdatePortalDB = 10003
	}
}
