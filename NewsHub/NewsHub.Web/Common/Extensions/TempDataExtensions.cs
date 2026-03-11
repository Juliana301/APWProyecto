using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NewsHub.Application.Common;

namespace NewsHub.Web.Common.Extensions;

public static class TempDataExtensions
{
    public static void SetToast(this ITempDataDictionary tempData, string message, TypeMessage type)
    {
        tempData["ToastType"] = type.ToString().ToLower();
        tempData["ToastMessage"] = message;
    }
}