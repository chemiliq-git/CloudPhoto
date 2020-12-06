using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class SeachCookieData
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public List<string> SelectCategory { get; set; }

        public string SearchText { get; set; }
    }
}
