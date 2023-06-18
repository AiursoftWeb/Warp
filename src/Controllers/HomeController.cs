﻿using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.Warp.Models;
using Aiursoft.Warp.Models.HomeViewModels;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Services;
using Microsoft.Extensions.Options;

namespace Aiursoft.Warp.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly SignInManager<WarpUser> _signInManager;
        private readonly DirectoryConfiguration _gatewayLocator;
        private readonly RecordsService _recordsService;
        private readonly DirectoryAppTokenService _appsContainer;

        public HomeController(
            SignInManager<WarpUser> signInManager,
            IOptions<DirectoryConfiguration> gatewayLocator,
            RecordsService recordsService,
            DirectoryAppTokenService appsContainer)
        {
            _signInManager = signInManager;
            _gatewayLocator = gatewayLocator.Value;
            _recordsService = recordsService;
            _appsContainer = appsContainer;
        }

        [AiurForceAuth(preferController: "Dashboard", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndexViewModel model)
        {
            var token = await _appsContainer.GetAccessTokenAsync();
            try
            {
                await _recordsService.CreateNewRecordAsync(token, model.NewRecordName, model.Url, new[] { "Anonymous" }, RecordType.Redirect, enabled: true);
            }
            catch (AiurUnexpectedResponse e) when (e.Code == ErrorType.Conflict)
            {
                ModelState.AddModelError(nameof(model.NewRecordName), $"Sorry but the key:'{model.NewRecordName}' already exists. Try another one.");
                return View(nameof(Index), model);
            }
            return View("Created", model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignOutRootServer(_gatewayLocator.Instance, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}
