namespace Odasoft.XBOL.Commons.Email;

public interface ITemplateService
{
    Task<string> RenderAsync(string templateName, object model);
}
