using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vic.Api.Helpers.Mail;
using Vic.Api.Models.Contact;

namespace Vic.Api.Controllers
{
    /// <summary>
    /// ContactController class, provides a controller for contact endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        #region Variables
        private readonly IMailHelper _mail;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ContactController object, initializes with specified arguments.
        /// </summary>
        /// <param name="mail"></param>
        public ContactController(IMailHelper mail)
        {
            _mail = mail;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Send an email to the contact us address.
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> SubmitAsync(ContactModel model)
        {
            await _mail.SendEmailAsync(_mail.Options.ContactEmail, model.Subject, $"<p>{model.FirstName} {model.LastName} {model.Email}</p><div>{model.Body}</div>");
            return new JsonResult(new { Success = true });
        }
        #endregion
    }
}
