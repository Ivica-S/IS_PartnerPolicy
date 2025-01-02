using IS_PartnerPolicy.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IS_PartnerPolicy.Helpers
{
    public static class SelectListHelper
    {
        public static SelectList CreateSelectList<T>(IEnumerable<T> values, string selectedValue = null)
        {
            var listItems = values.Select(v => new { Value = v.ToString(), Text = v.ToString() }).ToList();
            return new SelectList(listItems, "Value", "Text", selectedValue);
        }

        public static SelectList CreateSelectList<T>(IEnumerable<T> values, Func<T, string> textSelector, string selectedValue = null)
        {
            var listItems = values.Select(v => new { Value = v.ToString(), Text = textSelector(v) }).ToList();
            return new SelectList(listItems, "Value", "Text", selectedValue);
        }

        public static SelectList CreateSelectListFromKeyValue(IEnumerable<KeyValuePair<int, string>> values, int? selectedValue = null)
        {
            return new SelectList(values, "Key", "Value", selectedValue);
        }

        public static SelectList GetGenderList(string gender)
        {
            return CreateSelectList(
            new[] { "M", "F", "N" },
            value => value == "M" ? "Male" : value == "F" ? "Female" : "Other",
            gender
            );
        }
        public static SelectList GetPartnerTypeOptionsList(int partnerTypeId)
        {
            return CreateSelectListFromKeyValue(
            new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, "Personal"),
                new KeyValuePair<int, string>(2, "Legal")
            },
            partnerTypeId
            );
        }
    }

}
