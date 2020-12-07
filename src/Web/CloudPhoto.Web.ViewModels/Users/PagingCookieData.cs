using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPhoto.Web.ViewModels.Users
{
    public class PagingCookieData
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string Type { get; set; }
    }
}
