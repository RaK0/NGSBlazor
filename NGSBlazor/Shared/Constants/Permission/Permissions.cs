using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Constants.Permission
{
    public static class Permissions
    {
        [DisplayName("Slider")]
        [Description("Slider Permissions")]
        public static class Slider
        {
            public const string Add = "Permissions.Slider.Add";
            public const string Edit = "Permissions.Slider.Edit";
            public const string Delete = "Permissions.Slider.Delete";
        }

        public static List<string> GetRegisteredPermissions()
        {
            List<string> permissions = new();
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permissions.Add(propertyValue.ToString());
            }
            return permissions;
        }
    }
}
