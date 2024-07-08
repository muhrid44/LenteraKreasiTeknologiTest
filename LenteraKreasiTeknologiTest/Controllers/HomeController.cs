using LenteraKreasiTeknologiTest.Models;
using LenteraKreasiTeknologiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace LenteraKreasiTeknologiTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<ActionResult<IndexEmployeeListModelView>> Index([FromQuery] GetAllEmployeeSearchModel model)
        {
            IndexEmployeeListModelView result = await _homeService.GetAllEmployees(model);
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var result = await _homeService.GetAllDropdown();
            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult<CreateUpdateEmployeeModel>> Edit(string id)
        {
            var result = await _homeService.GetEmployeeById(id);
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult<CreateUpdateEmployeeModel>> Edit([FromForm] CreateUpdateEmployeeModel model, string id)
        {
            var result = await _homeService.UpdateEmployee(model, id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUpdateEmployeeModel model)
        {
            var result = await _homeService.CreateEmployee(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string nIK)
        {
            var result = await _homeService.DeleteEmployeeById(nIK);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var result = await _homeService.Login(model);
            HttpContext.Response.Cookies.Append("token", result, new CookieOptions { Expires = DateTime.Now.AddHours(1), HttpOnly = true });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("token");
            return RedirectToAction("Login");
        }

    }
}
