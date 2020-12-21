using System.Collections.Generic;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Home
{
    public class HomeIndexViewModel : IMapFrom<Image>
    {
        public CategoryHomeViewModel CategoryInfo { get; set; }

        public List<ImageHomeViewModel> CategoryImages { get; set; }
    }
}
