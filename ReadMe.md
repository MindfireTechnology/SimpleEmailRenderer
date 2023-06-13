# Project

This is a very simple email rendering package. Simple dictionary substitution occurrs. 

## Configuration

1. Make sure you set the `SimpleEmailRenderer:BaseDirectory` in your configuration like this:

```json
{
	"SimpleEmailRenderer": {
		"BaseDirectory": "..\\SimpleEmailRenderer\\EmailTemplates"
	},
}
```

1. Create a folder in your project called `EmailTemplates` (or whatever you set the `SimpleEmailRenderer:BaseDirectory` to)
1. Each folder in the `EmailTemplates` folder will be a template. The name of the folder will be the name of the template.
1. Each template folder should contain a `{TemplateName}.html` file. This is the template that will be used to render the HTML version of the email.
1. Each template folder should contain a `{TemplateName}.txt` file. This is the template that will be used to render the text version of the email.
1. There should be a `settings.json` file in the root of the `EmailTemplates` folder. This file contains the default settings for all templates.
	1. Each template folder may contain a `settings.json` that override the settings in the root `settings.json` file.

### Example `settings.json` file:
```json
{
	"FromEmail": "none@none.com",
	"FromDisplayName": "Jon Doe",

	"EmailSubject": "Override Me!"
}
```

### Example `settings.json` file in a template folder:
```json
{
	"EmailSubject": "This is a template specific subject"
}
```

The `EmailSubject` will be "This is a template specific subject" for the template that contains the `settings.json` file in the template folder. Otherwise the `EmailSubject` would be set to `Override Me!`.

## Multilingual Configuration

You can add multilingual email templates by using the `TwoLetterISOLanguageName` as part of the path to the template folder. E.g. `BaseDirectory\{TemplateName}\{TwoLetterISOLanguageName}\messagebody.html`


## Usage

```csharp
	// This really should be brought in via Dependency Injection (Singleton)
	var renderer = new SimpleEmailRenderer
	{
		BaseDirectory = ".\\EmailDirectory"
	};

	// Here are the values we're going to utilize in our email template:
	var replace = new MergeDictionary
	{ 
		{ "FirstName", "Nate" },
		{ "LastName", "Zaugg" },
		{ "Code", "12345" },
	};

	// Generate the email
	var result = await renderer.GenerateEmail("Welcome", "to@none.com", replace);

	// Send the message
	MailMessage msg = result.ToMailMessage();
	SmtpClient client = new SmtpClient();
	await client.SendMailAsync(msg);
```

Example of a template:

This file must be named `messagebody.html` and must be in the folder that matches the template name.
```html
<h2>{{FirstName}} {{LastName}},</h2>

<p>Welcome to our site!</p>

<p>Please use this link to confirm your account: <a href="{{Code}}">{{Code}}</a></p>

Thanks!
Test Email!
```

A plain text version would be mostly the same, but without the HTML markup, of course. It should be named `messagebody.txt` and must be in the folder that matches the template name.
