using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MindfireTechnology.SimpleEmailRenderer
{
	// BaseDirectory/Settings.json
	// BaseDirectory/TemplateName/Settings.json
	// BaseDirectory/TemplateName/MessageBody.txt
	// BaseDirectory/TemplateName/MessageBody.html
	// BaseDirectory/TemplateName/CultureCode/Settings.json ...
	public class SimpleEmailRenderer : IEmailRenderer
	{
		protected static readonly string SettingsFileName = "settings.json";
		protected static readonly string TxtMessageBodyFileName = "messagebody.txt";
		protected static readonly string HtmlMessageBodyFileName = "messagebody.html";

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

			if (string.IsNullOrWhiteSpace(emailTemplate))
				throw new ArgumentNullException(nameof(emailTemplate));

			if (replaceDictionary == null)
				throw new ArgumentNullException(nameof(replaceDictionary));	

			var settings = await LoadSettings(emailTemplate);

			string plainTextTemplateFile = await LoadTemplate(TxtMessageBodyFileName, emailTemplate, culture);
			string htmlTemplateFile = await LoadTemplate(HtmlMessageBodyFileName, emailTemplate, culture);

			if (plainTextTemplateFile == null && htmlTemplateFile == null)
				throw new FileNotFoundException($"Could not find email template. Check that the directory name matches the email template you asked to be rendered and that the files are named `messagebody.txt` and/or `messagebody.html`");

			Email result = new();
			result.Subject = settings.EmailSubject;
			result.FromEmail = settings.FromEmail;
			result.FromDisplayName = settings.FromDisplayName;
			result.ToEmail = recipientEmail;

			if (replaceDictionary.ContainsKey("ToDisplayName"))
				result.ToDisplayName = replaceDictionary["ToDisplayName"];

			if (string.IsNullOrWhiteSpace(plainTextTemplateFile) is not true)
				result.BodyPlainText = Merge(plainTextTemplateFile, replaceDictionary);

			if (string.IsNullOrWhiteSpace(htmlTemplateFile) is not true)
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


			return Task.FromResult<string>(null);
		}

		protected virtual async Task<Settings> LoadSettings(string emailTemplate, CultureInfo culture = null)
		{
			// Load base settings
			Settings settings = new();
			StringBuilder errMsg = new();
			bool foundSettings = false;

			string settingsFile = Path.Combine(BaseDirectory, SettingsFileName);
			if (File.Exists(settingsFile))
			{ 
				settings = settings.Combine(await Settings.LoadFromFile(settingsFile));
				foundSettings = true;
			}
			else
				errMsg.Append($"'{settingsFile}', ");

			settingsFile = Path.Combine(BaseDirectory, emailTemplate, SettingsFileName);
			if (File.Exists(settingsFile))
			{ 
				settings = settings.Combine(await Settings.LoadFromFile(settingsFile));
				foundSettings = true;
			}
			else
				errMsg.Append($"'{settingsFile}', ");

			if (culture != null)
			{
				settingsFile = Path.Combine(BaseDirectory, emailTemplate, culture.TwoLetterISOLanguageName, SettingsFileName);
				if (File.Exists(settingsFile))
				{
					settings = settings.Combine(await Settings.LoadFromFile(settingsFile));
					foundSettings = true;
				}
				else
					errMsg.Append($"'{settingsFile}', ");
			}

			if (foundSettings is not true)
				throw new FileNotFoundException($"Could not find settings file in any of: {errMsg.ToString().Trim().Trim(',')}");

			// Validate the settings
			if (string.IsNullOrWhiteSpace(settings.FromEmail))
				throw new ConfigurationException($"{nameof(settings.FromEmail)} is required.");

			if (string.IsNullOrWhiteSpace(settings.EmailSubject))
				throw new ConfigurationException($"{nameof(settings.EmailSubject)} is required.");

			return settings;
		}
	}
}
