using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ExceptionReporting.Shared.Core;

namespace Demo.WinForms
{
  public class YourCustomReporterView : IExceptionReportView
  {
	public string ProgressMessage { get; set; }
	public bool EnableEmailButton { get; set; }
	public bool ShowProgressBar { get; set; }
	public bool ShowFullDetail { get; set; }
	public string UserExplanation { get; }

	public void Completed(bool success) { }
	public void ShowError(string message, Exception exception) { }
	public void MapiSendCompleted() { }
	public void SetInProgressState() { }
	public void PopulateExceptionTab(IEnumerable<Exception> exceptions) { }
	public void PopulateAssembliesTab() { }
	public void PopulateSysInfoTab() { }
	public void SetProgressCompleteState() { }
	public void ToggleShowFullDetail() { }
	public void ShowWindow()
	{
	  MessageBox.Show("Demo custom report form");
	}
  }
}