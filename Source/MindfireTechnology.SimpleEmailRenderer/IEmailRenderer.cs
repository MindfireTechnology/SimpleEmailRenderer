using System.Globalization;
using System.Threading.Tasks;

namespace MindfireTechnology.SimpleEmailRenderer
{
	public interface IEmailRenderer
	{
		Task<Email> GenerateEmail(string emailTemplate, string recipientEmail, MergeDictionary replaceDictionary, CultureInfo culture = null);
	}
}
