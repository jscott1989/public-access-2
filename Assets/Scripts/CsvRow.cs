using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// Class to store one CSV row
public class CsvRow : List<string>
{
	public string LineText { get; set; }
}
