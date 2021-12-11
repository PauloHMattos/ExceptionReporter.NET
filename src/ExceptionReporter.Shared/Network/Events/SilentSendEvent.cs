using System;

namespace ExceptionReporting.Shared.Network.Events
{
  /// <summary>
  /// A fake/slient version of the events responding to sending
  /// </summary>
  public class SilentSendEvent : IReportSendEvent
  {
	/// <summary>
	/// silent complete
	/// </summary>
	public void Completed(bool success)
	{
	  // silent
	}

	/// <summary>
	/// silent error
	/// </summary>
	public void ShowError(string message, Exception exception)
	{
	  // silent
	}
  }
}