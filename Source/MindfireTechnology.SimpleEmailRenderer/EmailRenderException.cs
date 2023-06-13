using System;
using System.Runtime.Serialization;

[Serializable]
public class EmailRenderException : Exception
{
	public EmailRenderException() { }
	public EmailRenderException(string message) : base(message) { }
	public EmailRenderException(string message, Exception inner) : base(message, inner) { }
	protected EmailRenderException(
	  SerializationInfo info,
	  StreamingContext context) : base(info, context) { }
}
