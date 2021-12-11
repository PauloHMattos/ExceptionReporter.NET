using ExceptionReporting.Shared.Core;
using ExceptionReporting.Shared.Mail;
using ExceptionReporting.Shared.Network.Events;
using Win32Mapi;

namespace ExceptionReporting.Shared.Network.Senders
{
  internal class MapiMailSender : MailSender, IReportSender
  {
	public MapiMailSender(ExceptionReportInfo reportInfo, IReportSendEvent sendEvent, IScreenShooter screenShooter) :
		base(reportInfo, sendEvent, screenShooter)
	{ }

	public override string Description => "Email Client";
	public override string ConnectingMessage => $"Launching {Description}...";

	/// <summary>
	/// Try send via installed Email client
	/// Uses Simple-MAPI.NET library - https://github.com/PandaWood/Simple-MAPI.NET
	/// </summary>
	public void Send(string report)
	{
	  if (_config.EmailReportAddress.IsEmpty())
	  {
		_sendEvent.ShowError("EmailReportAddress not set", new ConfigException("EmailReportAddress"));
		return;
	  }

	  var mapi = new SimpleMapi();
	  mapi.AddRecipient(_config.EmailReportAddress, null, false);
	  _attacher.AttachFiles(new AttachAdapter(mapi));

	  mapi.Send(EmailSubject, report);
	}
  }
}