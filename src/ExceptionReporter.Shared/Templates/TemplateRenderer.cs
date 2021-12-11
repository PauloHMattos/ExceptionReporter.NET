using System;
using System.IO;
using System.Reflection;
using System.Text;
using HandlebarsDotNet;

// ReSharper disable UnusedMember.Global
#pragma warning disable 1591

namespace ExceptionReporting.Shared.Templates
{
  internal class TemplateRenderer
  {
	private readonly object _model;     // model object is kept generic but we force a kind of typing via constructors
	private readonly string _name;

	public TemplateRenderer(EmailIntroModel model)
	{
	  _model = model;
	  _name = "EmailIntroTemplate";
	}

	public TemplateRenderer(ReportModel model)
	{
	  _model = model;
	  _name = "ReportTemplate";
	}

	private string Render(string template)
	{
	  var compile = Handlebars.Compile(template);
	  var report = compile(_model);
	  return report;
	}

	public string RenderPreset(TemplateFormat format = TemplateFormat.Text)
	{
	  var template = GetTemplate(format);
	  return Render(template);
	}

	public string RenderCustom(string template)
	{
	  return Render(template);
	}

	private string GetTemplate(TemplateFormat format)
	{
	  var resource = $"{GetType().Namespace}.{_name}.{format.ToString().ToLower()}";
	  var assembly = Assembly.GetExecutingAssembly();

	  using (var stream = assembly.GetManifestResourceStream(resource))
	  {
		using (var reader = new StreamReader(stream ?? throw new InvalidOperationException($"resource not found: {resource}"), Encoding.UTF8))
		{
		  var template = reader.ReadToEnd();
		  return template;
		}
	  }
	}
  }
}