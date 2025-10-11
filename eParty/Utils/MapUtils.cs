
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace eParty.Utils
{
    public class MapUtils
    {
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public string FromName { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }
        public string ToName { get; set; }

        public MapUtils(double fromLatitude, double fromLongitude, string fromName,
                       double toLatitude, double toLongitude, string toName)
        {
            FromLatitude = fromLatitude;
            FromLongitude = fromLongitude;
            FromName = fromName;
            ToLatitude = toLatitude;
            ToLongitude = toLongitude;
            ToName = toName;
        }

        public IActionResult GetPartialView(ViewComponentContext context)
        {
            return new PartialViewResult
            {
                ViewName = "_MapPartial",
                ViewData = new ViewDataDictionary(context.ViewData)
                {
                    Model = this
                }
            };
        }
    }
}