using MindfireTechnology.SimpleEmailRenderer;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using Xunit;

namespace UnitTests
{
	public class UnitTest
	{
		[Fact]
		public async void TestEmailGeneration()
		{
			var renderer = new SimpleEmailRenderer
			{
				BaseDirectory = ".\\EmailDirectory"
			};

			var replace = new MergeDictionary
			{ 
				{ "FirstName", "Nate" },
				{ "LastName", "Zaugg" },
				{ "Code", "12345" },
			};

			var result = await renderer.GenerateEmail("Welcome", "to@none.com", replace);
			MailMessage msg = result.ToMailMessage();

			// Send the message
		}
	}
}
