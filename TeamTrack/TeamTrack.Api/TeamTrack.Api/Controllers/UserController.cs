using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using TeamTrack.Api.Models;
using TeamTrack.Api.Tools;
using TeamTrack.Core.Entities;
using TeamTrack.Core.Infrastructure;

namespace TeamTrack.Api.Controllers
{
    [Route("users")]
    public class UserController : BaseController
    {
        #region Constructor
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        [ActionContext]
        public ActionContext ActionContext { get; set; }


        public UserController(IRepository repository, IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager) : base(repository, mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        [Authorize(Policy = "Bearer", Roles = "Administrator")]
        [HttpGet("", Name = "Users")]
        public ActionResult Get(int page = 0, int pageSize = 10, bool includePassive = false, string userName = null)
        {
            var user = HttpContext.User;
            try
            {
                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    query = query.Where(x => x.UserName.ToLower().Contains(userName.ToLower()));
                }

                if (includePassive)
                {
                    query = query.OrderByDescending(x => x.CreateDate);
                }
                else
                {
                    query = query.Where(x => x.IsActive).OrderByDescending(x => x.CreateDate);
                }
                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var urlHelper = new UrlHelper(ActionContext);

                var prevLink = page > 0 ? urlHelper.Link("Users", new
                {
                    page = page - 1,
                    pageSize = pageSize,
                    includePassive = includePassive,
                    userName = userName
                }) : "";
                var nextLink = page < totalPages - 1 ? urlHelper.Link("Users", new
                {
                    page = page + 1,
                    pageSize = pageSize,
                    includePassive = includePassive,
                    userName = userName
                }) : "";

                var result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PrevPageLink = prevLink,
                    NextPageLink = nextLink,
                    Results = query.Skip(pageSize * page).Take(pageSize).ToList().Select(x => ModelFactory.Create(urlHelper, x))
                };

                return Ok(result);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return BadRequest(Resources.UnexpectedError);
            }
        }

        [HttpGet("{id}", Name = "UserById")]
        public ActionResult Get(int id, bool includePassive = false)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                var user = includePassive ? query.FirstOrDefault(x => x.Id == id) : query.FirstOrDefault(x => x.Id == id && x.IsActive);

                if (user != null)
                {
                    var urlHelper = new UrlHelper(ActionContext);
                    return Ok(ModelFactory.Create(urlHelper, user));
                }
                return BadRequest(Resources.NoDataFound);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return BadRequest(Resources.UnexpectedError);
            }
        }

    }
}


