using Aiursoft.Identity.Attributes;
using Aiursoft.Warp.Models;
using Aiursoft.Warp.Models.DashboardViewModels;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Services;

namespace Aiursoft.Warp.Controllers
{
    [AiurForceAuth]
    [Route("Dashboard")]
    public class DashboardController : Controller
    {
        private readonly DirectoryAppTokenService _appsContainer;
        private readonly RecordsService _recordsService;
        private readonly UserManager<WarpUser> _userManager;

        public DashboardController(
            DirectoryAppTokenService appsContainer,
            RecordsService recordsService,
            UserManager<WarpUser> userManager)
        {
            _appsContainer = appsContainer;
            _recordsService = recordsService;
            _userManager = userManager;
        }

        [Route(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create(IndexViewModel model)
        {
            var token = await _appsContainer.GetAccessTokenAsync();
            var user = await GetCurrentUserAsync();
            try
            {
                await _recordsService.CreateNewRecordAsync(token, model.NewRecordName, model.Url, new[] { user.Id }, RecordType.Redirect, enabled: true);
            }
            catch (AiurUnexpectedServerResponseException e) when (e.Response.Code == Code.Conflict)
            {
                ModelState.AddModelError(nameof(model.NewRecordName), $"Sorry but the key:'{model.NewRecordName}' already exists. Try another one.");
                model.Recover(user);
                return View(nameof(Index), model);
            }
            model.Recover(user);
            return View("Created", model);
        }

        [Route(nameof(Records))]
        public async Task<IActionResult> Records()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.GetAccessTokenAsync();
            var records = await _recordsService.ViewMyRecordsAsync(token, user.Id);
            var model = new RecordsViewModel(user, records.Records);
            return View(model);
        }

        [Route("Records/{recordName}/Edit")]
        public async Task<IActionResult> Edit([FromRoute] string recordName)
        {
            var user = await GetCurrentUserAsync();
            var accessToken = _appsContainer.GetAccessTokenAsync();
            var allRecords = await _recordsService.ViewMyRecordsAsync(await accessToken);
            var recordDetail = allRecords.Records.FirstOrDefault(t => t.RecordUniqueName == recordName);
            if (recordDetail == null)
            {
                return NotFound();
            }
            var model = new EditViewModel(user)
            {
                RecordName = recordName,
                NewRecordName = recordName,
                Type = recordDetail.Type,
                URL = recordDetail.TargetUrl,
                Enabled = recordDetail.Enabled
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Records/{recordName}/Edit")]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(user);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.GetAccessTokenAsync();
                await _recordsService.UpdateRecordInfoAsync(token, model.RecordName, model.NewRecordName, model.Type, model.URL, new[] { user.Id }, model.Enabled);
                return RedirectToAction(nameof(Records), "Dashboard");
            }
            catch (AiurUnexpectedServerResponseException e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message!);
                model.Recover(user);
                model.NewRecordName = model.RecordName;
                return View(model);
            }
        }

        [Route("Records/{recordName}/Delete")]
        public async Task<IActionResult> Delete([FromRoute] string recordName)
        {
            var user = await GetCurrentUserAsync();
            var model = new DeleteViewModel(user)
            {
                RecordName = recordName
            };
            return View(model);
        }

        [HttpPost]
        [Route("Records/{recordName}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(user);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.GetAccessTokenAsync();
                await _recordsService.DeleteRecordAsync(token, model.RecordName);
                return RedirectToAction(nameof(Records), "Dashboard");
            }
            catch (AiurUnexpectedServerResponseException e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message!);
                model.Recover(user);
                return View(model);
            }
        }

        private async Task<WarpUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
