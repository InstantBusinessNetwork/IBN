using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mediachase.Ibn.ConfigurationUI.TestConsole
{
	/// <summary>
	/// Represents program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Mains the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		static int Main(string[] args)
		{
			if (args.Length > 0)
			{
				string arg = args[0];
				arg = arg.ToLower();
				arg = arg.Trim(' ', '-', '/');

				switch (arg)
				{
					case "s":
						WriteTextToConsole();
						return 0;
					case "e":
						WriteTextToConsole();
						return 1;
				}
			}

			return -1;
		}

		private static void WriteTextToConsole()
		{
			for (int index = 0; index < 30; index++ )
			{
				Console.WriteLine("Test private static void WriteTextToConsole()");
				Console.WriteLine("Test private static void WriteTextToConsole()");
				Console.WriteLine("Test private static void WriteTextToConsole()");
				Console.WriteLine("Test private static void WriteTextToConsole()");

				///Thread.Sleep(500);
			}
		}
	}
}
