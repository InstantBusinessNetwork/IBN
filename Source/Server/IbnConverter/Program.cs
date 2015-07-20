using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Mediachase.Ibn.Converter;


namespace Mediachase.Ibn.Converter
{
	class Program
	{
		static void Main()
		{
			Convert();
		}

		static void Convert()
		{
			try
			{
				string source = @"Data source=(local);Initial catalog=ibn45;Integrated Security=SSPI";
				string target = @"Data source=(local);Initial catalog=ibn47;Integrated Security=SSPI";
				IbnConverter converter = new IbnConverter(60, 1024 * 1024, source, target);
				converter.Progress += new EventHandler<ConverterEventArgs>(OnProgress);
				//converter.Convert45to47(false, true);
				converter.Convert(1, 1);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private static void OnProgress(object sender, ConverterEventArgs e)
		{
			if (e != null)
				Console.WriteLine("{0} {1}", DateTime.Now.ToString("T", CultureInfo.InvariantCulture), e.Message);
		}
	}
}
