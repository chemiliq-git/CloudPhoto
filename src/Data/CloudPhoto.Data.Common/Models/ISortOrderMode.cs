namespace CloudPhoto.Data.Common.Models
{
    using System.ComponentModel.DataAnnotations;

    public interface ISortOrderMode
    {
        public int SortOrder { get; set; }
    }
}
