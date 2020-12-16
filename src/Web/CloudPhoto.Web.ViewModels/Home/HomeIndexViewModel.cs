using System.Collections.Generic;

namespace CloudPhoto.Web.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public CategoryHomeViewModel CategoryInfo { get; set; }

        public List<ImageHomeViewModel> CategoryImages { get; set; }
    }
}
