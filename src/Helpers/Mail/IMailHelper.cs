using System.Threading.Tasks;

namespace Vic.Api.Helpers.Mail
{
    public interface IMailHelper
    {
        MailOptions Options { get; }
        Task SendEmailAsync(string to, string subject, string body);
    }
}