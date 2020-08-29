using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vic.Api.Areas.Admin.Models.Pages;
using Vic.Api.Models;
using Vic.Data;

namespace Vic.Api.Controllers
{
    /// <summary>
    /// PagesController class, provides a controller for page endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PagesController : ControllerBase
    {
        #region Variables
        private readonly VicContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a PagesController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        public PagesController(VicContext context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Get a page of pages from the datasource.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int page = 1, int quantity = 0)
        {
            if (page < 1) page = 1;
            if (quantity < 0) quantity = 0;

            var query = _context.Pages.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ThenBy(p => p.Path).Select(p => new PageModel(p));

            if (page > 1 && quantity > 0)
                query = query.Skip((page - 1) & quantity).Take(quantity);

            var pages = query.ToArray();

            return new JsonResult(new PageModel<PageModel>(page, pages.Count(), pages));
        }

        /// <summary>
        /// Fetch the page from the datasource for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Get(int id)
        {
            var page = _context.Pages.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("Page does not exist");

            return new JsonResult(new PageModel(page));
        }

        /// <summary>
        /// Fetch the page from the datasource for the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("path")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Get(string path)
        {
            var page = _context.Pages.FirstOrDefault(i => i.Path == path) ?? throw new ArgumentOutOfRangeException("Page does not exist");

            return new JsonResult(new PageModel(page));
        }
        #endregion
    }
}
