namespace ExceptionReporting.Shared.SystemInfo
{
  // ReSharper disable once ClassNeverInstantiated.Global
  internal class SysInfoQueries
  {
	public static readonly SysInfoQuery OperatingSystem = new SysInfoQuery("Operating System", "Win32_OperatingSystem", false);
	public static readonly SysInfoQuery Machine = new SysInfoQuery("Machine", "Win32_ComputerSystem", true);
  }
}