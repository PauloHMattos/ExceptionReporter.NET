using System;

namespace ExceptionReporting.Shared.Network.Events
{
  /// <summary>
  /// Represents the events required of a report sender
  /// </summary>
  public interface IReportSendEvent
  {
	/// <summary>
	/// send completed
	/// </summary>
	void Completed(bool success);

	/// <summary>
	/// show an error
	/// </summary>
	void ShowError(string message, Exception exception);
  }
}
