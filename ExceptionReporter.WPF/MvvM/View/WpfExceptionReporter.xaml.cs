using System;
using System.Windows.Controls;
using ExceptionReporting.WPF.MvvM.ViewModel;

namespace ExceptionReporting.WPF.MvvM.View
{
	/// <summary>
	/// Interaction logic for WpfExceptionReporter
	/// </summary>
	public partial class WpfExceptionReporter : UserControl
	{
		public WpfExceptionReporter(Exception exception, ExceptionReportInfo info)
		{
			InitializeComponent();

			info.MainException = exception;
			this.DataContext = new MainViewModel(info);
		}
	}
}