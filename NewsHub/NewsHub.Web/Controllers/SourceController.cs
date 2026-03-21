using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Source;
using NewsHub.Application.Interfaces.Services;
using NewsHub.Web.Common.Extensions;
using NewsHub.Web.ViewModels.Dashboard;

namespace NewsHub.Web.Controllers
{
    public class SourceController : Controller
    {
        private readonly ISourceService _sourceService;

        public SourceController(
            ISourceService sourceService
            )
        {
            _sourceService = sourceService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewSource(DashboardViewModel vm)
        {
            if (!TryValidateModel(vm.NewSource, nameof(vm.NewSource)))
            {
                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                TempData.SetToast(error ?? "Error de validación", TypeMessage.Error);
                return RedirectToAction("Index", "News");
            }

            var dto = new SourceDto
            {
                Url = vm.NewSource.Url,
                Name = vm.NewSource.Name,
                Description = vm.NewSource.Description,
                ComponentType = vm.NewSource.ComponentType,
                RequiresSecret = vm.NewSource.RequiresSecret
            };

            var result = _sourceService.CreateAsync(dto);

            if (result.Result.IsFailure)
            {
                TempData.SetToast(result.Result.Error!, TypeMessage.Error);
            }

            return RedirectToAction("Index", "News");
        }
    }
}
