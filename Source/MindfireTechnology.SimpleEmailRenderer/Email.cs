using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace MindfireTechnology.SimpleEmailRenderer
{
	public class Email
	{
		public string FromEmail { get; set; }
		public string FromDisplayName { get; set; }

		public string ToEmail { get; set; }
		public string ToDisplayName { get; set; }

		public string Subject { get; set; }

		public string BodyPlainText { get; set; }

		public string BodyHtml { get; set; }

		public MailMessage ToMailMessage()
		{
			var result = new MailMessage
			{
				From = new MailAddress(FromEmail, FromDisplayName),
				Subject = Subject,
				Body = string.IsNullOrWhiteSpace(BodyHtml) ? BodyPlainText : BodyHtml,
				IsBodyHtml = !string.IsNullOrWhiteSpace(BodyHtml),
				BodyEncoding = Encoding.UTF8,
				SubjectEncoding = Encoding.UTF8,
			};

			result.To.Add(new MailAddress(ToEmail, ToDisplayName));

			return result;
		}
	}
}
