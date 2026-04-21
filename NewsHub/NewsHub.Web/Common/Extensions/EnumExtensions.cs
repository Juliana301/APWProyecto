using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewsHub.Web.Common.Extensions
{
    public static class EnumExtensions
    {
        public static List<SelectListItem> ToSelectList<TEnum>()
                where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = GetDescription(e)
                })
                .ToList();
        }

        public static string GetDescription(Enum value)
        {
            var field = value
                .GetType()
                .GetField(value.ToString());

            var attribute =
                field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description
                ?? value.ToString();
        }
    }
}