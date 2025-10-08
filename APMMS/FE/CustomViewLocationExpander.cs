using Microsoft.AspNetCore.Mvc.Razor;

namespace FE
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // No values to populate
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // Add custom view locations
            var customLocations = new[]
            {
                "/vn.fpt.edu.views/{1}/{0}.cshtml",
                "/vn.fpt.edu.views/Shared/{0}.cshtml"
            };

            return customLocations.Concat(viewLocations);
        }
    }
}

