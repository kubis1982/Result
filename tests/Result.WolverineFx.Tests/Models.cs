namespace kubis1982.Result
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

    public static class Repository
    {
        public static readonly List<Contractor> Contractors =
        [
            new Contractor { Id = 1, Name = "John Doe" },
            new Contractor { Id = 2, Name = "Jane Smith" }
        ];

        public static readonly List<Article> Articles =
        [
            new Article { Id = 1, Name = "Laptop" },
            new Article { Id = 2, Name = "Mouse" }
        ];
    }
}
