using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MindfireTechnology.SimpleEmailRenderer
{
	public class Settings
	{
		public string FromEmail { get; set; }
		public string FromDisplayName { get; set; }

		public string EmailSubject { get; set; }

		public Settings Combine(Settings overrideSettings)
		{
			return new Settings
			{
				FromEmail = string.IsNullOrWhiteSpace(overrideSettings.FromEmail) ? FromEmail : overrideSettings.FromEmail,
				FromDisplayName = string.IsNullOrWhiteSpace(overrideSettings.FromDisplayName) ? FromDisplayName : overrideSettings.FromDisplayName,
				EmailSubject = string.IsNullOrWhiteSpace(overrideSettings.EmailSubject) ? EmailSubject : overrideSettings.EmailSubject,
			};
		}

		public static async Task<Settings> LoadFromFile(string filename)
		{
			using (var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
			{
				return await JsonSerializer.DeserializeAsync<Settings>(file);
			}
		}
	}
}
