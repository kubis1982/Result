namespace Kubis1982.Result
{
    public record ContractorDto(int Id, string Name);

    public record ArticleDto(int Id, string Name);

    public class Contractor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Article
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
