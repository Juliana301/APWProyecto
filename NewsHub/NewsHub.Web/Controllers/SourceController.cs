using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Source;
using NewsHub.Application.Interfaces.Services.Source;
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
            ModelState.Clear();

            if (!TryValidateModel(vm.NewSource, nameof(vm.NewSource)))
            {

                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .LastOrDefault()?.ErrorMessage;

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

            TempData.SetToast(result.Result.Message!, TypeMessage.Success);

            return RedirectToAction("Index", "News");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSource(DashboardViewModel vm)
        {
            ModelState.Clear();
            if (!TryValidateModel(vm.EditSource, nameof(vm.EditSource)))
            {
                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;
                TempData.SetToast(error ?? "Error de validación", TypeMessage.Error);
                return RedirectToAction("Index", "News");
            }

            var dto = new SourceDto
            {
                Id = vm.EditSource.Id,
                Url = vm.EditSource.Url,
                Name = vm.EditSource.Name,
                Description = vm.EditSource.Description,
                ComponentType = vm.EditSource.ComponentType,
                ApiConfigJson = vm.EditSource.ApiConfigJson,
                RequiresSecret = vm.EditSource.RequiresSecret
            };

            var result = await _sourceService.UpdateAsync(dto.Id, dto);
            if (result.IsFailure)
            {
                TempData.SetToast(result.Error!, result.TypeMessage);
            }

            TempData.SetToast(result.Message!, result.TypeMessage);

            return RedirectToAction("Index", "News");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSource(DashboardViewModel vm)
        {
            ModelState.Clear();
            if (!TryValidateModel(vm.DeleteSource, nameof(vm.DeleteSource)))
            {
                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;
                TempData.SetToast(error ?? "Error de validación", TypeMessage.Error);
                return RedirectToAction("Index", "News");
            }

            var result = await _sourceService.DeleteAsync(vm.DeleteSource.Id, vm.DeleteSource.SourceName);

            if (result.IsFailure)
            {
                TempData.SetToast(result.Error ?? "Error al eliminar la fuente", result.TypeMessage);
                return RedirectToAction("Index", "News");
            }

            TempData.SetToast(result.Message ?? "Fuente eliminada correctamente", result.TypeMessage);

            return RedirectToAction("Index", "News");
        }
        
    }
}
