using System.Collections.Generic;

namespace CloudPhoto.Web.ViewModels.FilterBar
{
    public class FilterBarSearchDataViewModel
    {
        public List<CategoryCheckBoxViewModel> Category { get; set; }

        public string Name { get; set; }

        public int Page { get; set; }

        public int PerPage { get; set; }
    }
}