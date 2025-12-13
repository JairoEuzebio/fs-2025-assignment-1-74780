namespace DublinBikes.BlazorApp.Models;

public class QueryState
{
    public string Search { get; set; } = "";
    public string Status { get; set; } = "";
    public int? MinBikes { get; set; }

    public string Sort { get; set; } = "name";
    public string Dir { get; set; } = "asc";

    public string SortBy
    {
        get => Sort;
        set => Sort = value;
    }

    public bool SortDesc
    {
        get => Dir.Equals("desc", StringComparison.OrdinalIgnoreCase);
        set => Dir = value ? "desc" : "asc";
    }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}