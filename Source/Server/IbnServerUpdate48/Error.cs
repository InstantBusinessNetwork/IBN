using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
	enum Error
	{
		Unknown = -1,
		OK = 0,
		InvalidArgs = 10001,
		WrongVersion = 10002,
		LicenseExpired = 10003,
		UpdatesExpired = 10004,
	}
}
