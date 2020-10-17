using System;
using System.Collections.Generic;
using System.Text;

namespace MindfireTechnology.SimpleEmailRenderer
{
	public class MergeDictionary : Dictionary<string, string>
	{
		public MergeDictionary() : base() { }
		public MergeDictionary(IDictionary<string, string> dictionary) : base(dictionary) { }
		public MergeDictionary(IEnumerable<KeyValuePair<string, string>> collection) : base(collection) { }
	}
}
