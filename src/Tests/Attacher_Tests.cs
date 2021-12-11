using ExceptionReporting.Shared;
using ExceptionReporting.Shared.Core;
using ExceptionReporting.Shared.Mail;
using ExceptionReporting.Shared.Zip;
using Moq;
using NUnit.Framework;

namespace Tests.ExceptionReporting
{
  public class Attacher_Tests
  {
	private Mock<IAttach> _iAttach;
	private Mock<IFileService> _file;
	private Mock<IZipper> _zip;

	[SetUp]
	public void SetUp()
	{
	  _iAttach = new Mock<IAttach>();
	  _file = new Mock<IFileService>();
	  _zip = new Mock<IZipper>();
	}

	[Test]
	public void Can_Attach_Nothing_If_None_To_Attach()
	{
	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new string[] { } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach(""), Times.Never);
	}

	[Test]
	public void Can_Attach_1_With_1_File()
	{
	  _file.Setup(f => f.Exists("file1")).Returns(true);
	  _file.Setup(f => f.TempFile(It.IsAny<string>())).Returns("attach.zip");

	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new[] { "file1" } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach("attach.zip"), Times.Once);
	}

	[Test]
	public void Can_Attach_1_With_Multiple_Files()
	{
	  _file.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
	  _file.Setup(f => f.TempFile(It.IsAny<string>())).Returns("attach.zip");

	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new[] { "file1", "file2", "file3" } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach("attach.zip"), Times.Once);
	}

	[Test]
	public void Can_Attach_0_When_Multiple_Files_But_None_Exist()
	{
	  _file.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
	  _file.Setup(f => f.TempFile(It.IsAny<string>())).Returns("attach.zip");

	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new[] { "file1", "file2", "file3" } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach("attach.zip"), Times.Never());
	}

	[Test]
	public void Can_Attach_1_For_Each_Already_Zipped()
	{
	  _file.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new[] { "file1.zip", "file2.zip" } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach("file1.zip"), Times.Once());
	  _iAttach.Verify(a => a.Attach("file2.zip"), Times.Once());
	}

	[Test]
	public void Can_Attach_1_Already_Zipped_And_1_File_To_Attach()
	{
	  _file.Setup(f => f.TempFile(It.IsAny<string>())).Returns("attach.zip");
	  _file.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);

	  var attacher = new Attacher(new ExceptionReportInfo { FilesToAttach = new[] { "file1.zip", "file2.txt" } }, new Mock<IScreenShooter>().Object)
	  {
		FileService = _file.Object,
		Zipper = _zip.Object
	  };

	  attacher.AttachFiles(_iAttach.Object);

	  _iAttach.Verify(a => a.Attach("file1.zip"), Times.Once());
	  _iAttach.Verify(a => a.Attach("attach.zip"), Times.Once());
	}

	[Test]
	public void Should_Take_Screenshot_If_Configured()
	{
	  var screenshot = new Mock<IScreenShooter>();
	  var attacher =
		  new Attacher(new ExceptionReportInfo { TakeScreenshot = true }, new Mock<IScreenShooter>().Object)
		  {
			ScreenShooter = screenshot.Object
		  };

	  attacher.AttachFiles(_iAttach.Object);

	  screenshot.Verify(s => s.TakeScreenShot(), Times.Once());
	}

	[Test]
	public void Should_Not_Take_Screenshot_If_Not_Configured()
	{
	  var screenshot = new Mock<IScreenShooter>();
	  var attacher =
		  new Attacher(new ExceptionReportInfo { TakeScreenshot = false }, new Mock<IScreenShooter>().Object)
		  {
			ScreenShooter = screenshot.Object
		  };

	  attacher.AttachFiles(_iAttach.Object);

	  screenshot.Verify(s => s.TakeScreenShot(), Times.Never);
	}
  }
}
