using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionReporting.Properties;
using ExceptionReporting.Shared;
using ExceptionReporting.Shared.Core;
using ExceptionReporting.Shared.Mail;
using ExceptionReporting.Shared.Network;
using ExceptionReporting.Shared.Report;
using ExceptionReporting.Shared.SystemInfo;
using ExceptionReporting.Shared.Templates;
using ExceptionReporting.Shared.Zip;
using ExceptionReporting.WinForms;

namespace ExceptionReporting.MVP.Presenters
{
  /// <summary>
  /// The Presenter in this MVP (Model-View-Presenter) implementation 
  /// </summary>
  internal class ExceptionReportPresenter
  {
	private readonly ReportGenerator _reportGenerator;
	private readonly ReportFileZipper _reportFileZipper;

	/// <summary>
	/// constructor accepting a view and the data/config of the report
	/// </summary>
	public ExceptionReportPresenter(IExceptionReportView view, ExceptionReportInfo info)
	{
	  _reportGenerator = new ReportGenerator(info);
	  _reportFileZipper = new ReportFileZipper(new FileService(), _reportGenerator, info);
	  View = view;
	  ReportInfo = info;
	}

	/// <summary> Report configuration and data  </summary>
	public ExceptionReportInfo ReportInfo { get; }

	/// <summary> The main dialog/view  </summary>
	private IExceptionReportView View { get; }

	private string CreateReport()
	{
	  ReportInfo.UserExplanation = View.UserExplanation;
	  return _reportGenerator.Generate();
	}

	/// <summary>
	/// Save the exception report to file/disk
	/// </summary>
	/// <param name="zipFilePath">the filename to save to</param>
	public void SaveZipReportToFile(string zipFilePath)
	{
	  if (string.IsNullOrEmpty(zipFilePath)) return;
	  var result = _reportFileZipper.Save(zipFilePath);
	  if (!result.Saved)
	  {
		View.ShowError(Resources.Unable_to_save_file + $"'{result}'", result.Exception);
	  }
	}

	/// <summary>
	/// Send the exception report using the configured send method
	/// </summary>
	public void SendReport()
	{
	  View.EnableEmailButton = false;
	  View.ShowProgressBar = true;

	  var sender = new SenderFactory(ReportInfo, View, new WinFormsScreenShooter()).Get();
	  View.ProgressMessage = sender.ConnectingMessage;

	  try
	  {
		var report = ReportInfo.IsSimpleMAPI() ? new EmailReporter(ReportInfo).Create() : CreateReport();
		sender.Send(report);
	  }
	  catch (Exception exception)
	  {
		View.Completed(false);
		View.ShowError(Resources.Unable_to_setup + $" {sender.Description}" +
					   Environment.NewLine + exception.Message, exception);
	  }
	  finally
	  {
		if (ReportInfo.IsSimpleMAPI())
		{
		  View.MapiSendCompleted();
		}
	  }
	}

	/// <summary>
	/// copy the report to clipboard
	/// </summary>
	public void CopyReportToClipboard()
	{
	  var report = CreateReport();
	  View.ProgressMessage = WinFormsClipboard.CopyTo(report) ? Resources.Copied_to_clipboard : Resources.Failed_to_copy_to_clipboard;
	}

	/// <summary>
	/// toggle the detail between 'simple' (just message) and showFullDetail (ie normal)
	/// </summary>
	public void ToggleDetail()
	{
	  View.ShowFullDetail = !View.ShowFullDetail;
	  View.ToggleShowFullDetail();
	}

	/// <summary>
	/// Fetch the WMI system information
	/// </summary>
	public IEnumerable<SysInfoResult> GetSysInfoResults()
	{
	  return _reportGenerator.GetOrFetchSysInfoResults();
	}

	/// <summary>
	/// The main entry point, populates the report with everything it needs
	/// </summary>
	public void PopulateReport()
	{
	  try
	  {
		View.SetInProgressState();

		View.PopulateExceptionTab(ReportInfo.Exceptions);
		View.PopulateAssembliesTab();
		View.PopulateSysInfoTab();
	  }
	  finally
	  {
		View.SetProgressCompleteState();
	  }
	}

	public List<AssemblyRef> GetReferencedAssemblies()
	{
	  return new AssemblyDigger(ReportInfo.AppAssembly).GetAssemblyRefs().ToList();
	}
  }
}