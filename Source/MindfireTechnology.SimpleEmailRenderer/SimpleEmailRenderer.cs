using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MindfireTechnology.SimpleEmailRenderer
{
	// BaseDirectory/Settings.json
	// BaseDirectory/TemplateName/Settings.json
	// BaseDirectory/TemplateName/MessageBody.txt
	// BaseDirectory/TemplateName/MessageBody.html
	// BaseDirectory/TemplateName/CultureCode/Settings.json ...
	public class SimpleEmailRenderer
	{
		public string BaseDirectory { get; set; }


		public SimpleEmailRenderer()
		{
			
		}

		public SimpleEmailRenderer(IConfiguration settings)
		{
			BaseDirectory = settings["SimpleEmailRenderer:BaseDirectory"];
		}

		public async Task<Email> GenerateEmail(string emailTemplate, string recipientEmail, MergeDictionary replaceDictionary, CultureInfo culture = null)
		{
			if (string.IsNullOrWhiteSpace(BaseDirectory))
				throw new InvalidOperationException("BaseDirectory is not setup! Please either set the BaseDirectory property on this class or configure the `SimpleEmailRenderer:BaseDirectory` setting");

			var settings = await LoadSettings(emailTemplate);

			string plainTextTemplateFile = await LoadTemplate("MessageBody.txt", emailTemplate, culture);
			string htmlTemplateFile = await LoadTemplate("MessageBody.html", emailTemplate, culture);

			Email result = new();
			result.Subject = settings.EmailSubject;
			result.FromEmail = settings.FromEmail;
			result.FromDisplayName = settings.FromDisplayName;
			result.ToEmail = recipientEmail;

			if (replaceDictionary.ContainsKey("ToDisplayName"))
				result.ToDisplayName = replaceDictionary["ToDisplayName"];

			if (!string.IsNullOrWhiteSpace(plainTextTemplateFile))
				result.BodyPlainText = Merge(plainTextTemplateFile, replaceDictionary);

			if (!string.IsNullOrWhiteSpace(htmlTemplateFile))
				result.BodyHtml = Merge(htmlTemplateFile, replaceDictionary);

			return result;
		}

		protected virtual string Merge(string template, MergeDictionary values)
		{
			foreach (string key in values.Keys)
			{
				template = template.Replace("{{" + key + "}}", values[key], StringComparison.InvariantCultureIgnoreCase);
			}

			return template;
		}

		protected virtual Task<string> LoadTemplate(string filename, string emailTemplate, CultureInfo culture)
		{
			// Check the most specific to the least specific
			string file = null;

			if (culture != null)
			{
				file = Path.Combine(BaseDirectory, emailTemplate, culture.TwoLetterISOLanguageName, filename);
				if (File.Exists(file))
					return File.ReadAllTextAsync(file);
			}

			file = Path.Combine(BaseDirectory, emailTemplate, filename);
			if (File.Exists(file))
				return File.ReadAllTextAsync(file);

			return null;
		}

		protected virtual async Task<Settings> LoadSettings(string emailTemplate, CultureInfo culture = null)
		{
			// Load base settings
			Settings settings = new();

			string settingsFile = Path.Combine(BaseDirectory, "Settings.json");
			if (File.Exists(settingsFile))
				settings = settings.Combine(await Settings.LoadFromFile(settingsFile));

			settingsFile = Path.Combine(BaseDirectory, emailTemplate, "Settings.json");
			if (File.Exists(settingsFile))
				settings = settings.Combine(await Settings.LoadFromFile(settingsFile));

			if (culture != null)
			{
				settingsFile = Path.Combine(BaseDirectory, emailTemplate, culture.TwoLetterISOLanguageName, "Settings.json");
				if (File.Exists(settingsFile))
					settings = settings.Combine(await Settings.LoadFromFile(settingsFile));
			}

			return settings;
		}
	}
}
