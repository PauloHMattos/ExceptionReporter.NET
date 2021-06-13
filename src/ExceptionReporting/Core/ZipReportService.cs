using System.Collections.Generic;
using System.Linq;
using ExceptionReporting.Mail;

namespace ExceptionReporting.Core
{
	internal class ZipReportService : IZipReportService
	{
		private IZipper Zipper { get; }
		private IScreenshotTaker ScreenshotTaker { get; }
		private IFileService FileService { get; }

		public ZipReportService(IZipper zipper, IScreenshotTaker screenshotTaker, IFileService fileService)
		{
			Zipper = zipper;
			ScreenshotTaker = screenshotTaker;
			FileService = fileService;
		}

		public string CreateZipReport(ExceptionReportInfo reportInfo, IEnumerable<string> additionalFilesToAttach = null)
		{
			var zipFilePath = FileService.TempFile(reportInfo.AttachmentFilename);
			return CreateZipReport(reportInfo, zipFilePath, additionalFilesToAttach);
		}

		public string CreateZipReport(ExceptionReportInfo reportInfo, string zipFilePath, IEnumerable<string> additionalFilesToAttach = null)
		{
			if (string.IsNullOrWhiteSpace(zipFilePath)) return string.Empty;

			var files = new List<string>();
			if (reportInfo.FilesToAttach.Length > 0) files.AddRange(reportInfo.FilesToAttach);
			if (additionalFilesToAttach?.Count() > 0) files.AddRange(additionalFilesToAttach);
			try
			{
				if (reportInfo.TakeScreenshot) files.Add(ScreenshotTaker.TakeScreenShot());
			}
			catch { /* ignored */ }

			var filesThatExist = files.Where(f => FileService.Exists(f)).ToList();
			var filesToZip = filesThatExist;
			if (filesToZip.Any())
				Zipper.Zip(zipFilePath, filesToZip);
			else
				return string.Empty;

			return zipFilePath;
		}
	}
}
