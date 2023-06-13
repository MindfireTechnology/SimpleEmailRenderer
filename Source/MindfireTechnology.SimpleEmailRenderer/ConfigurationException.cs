using System;
using System.Runtime.Serialization;

[Serializable]
public class ConfigurationException : EmailRenderException
{
	public ConfigurationException() { }
	public ConfigurationException(string message) : base(message) { }
	public ConfigurationException(string message, Exception inner) : base(message, inner) { }
	protected ConfigurationException(
	  SerializationInfo info,
	  StreamingContext context) : base(info, context) { }
}
