namespace Xperience.Community.Rest.Models
{
    public class GetAllSettings
    {
        public required string ObjectType { get; set; }


        public string? Where { get; set; }


        public string? OrderBy { get; set; }


        public string? Columns { get; set; }


        public int? TopN { get; set; }


        public int? PageSize { get; set; }


        public int? Page { get; set; }


        public bool IsPagedQuery => Page is not null && PageSize is not null;
    }
}
