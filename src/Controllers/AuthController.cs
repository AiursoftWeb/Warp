﻿using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.WebTools;
using Aiursoft.Warp.Models;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Directory.SDK.Models.ForApps.AddressModels;

namespace Aiursoft.Warp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService<WarpUser> _authService;
        public AuthController(
            AuthService<WarpUser> authService)
        {
            _authService = authService;
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: false)]
        public IActionResult GoAuth()
        {
            return RedirectToAction("Index", "Home");
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: true)]
        public IActionResult GoRegister()
        {
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AuthResult(AuthResultAddressModel model)
        {
            var user = await _authService.AuthApp(model);
            this.SetClientLang(user.PreferedLanguage);
            return Redirect(model.State);
        }
    }
}