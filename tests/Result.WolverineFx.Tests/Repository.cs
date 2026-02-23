namespace Kubis1982.Result
{
    public static class Repository
    {
        public const int CONTRACTOR_ID_JOHN_DOE = 1;
        public const int CONTRACTOR_ID_JANE_SMITH = 2;

        public const string CONTRACTOR_NAME_JOHN_DOE = "John Doe";
        public const string CONTRACTOR_NAME_JANE_SMITH = "Jane Smith";

        public static readonly List<Contractor> Contractors =
        [
            new Contractor { Id = CONTRACTOR_ID_JOHN_DOE, Name = CONTRACTOR_NAME_JOHN_DOE },
            new Contractor { Id = CONTRACTOR_ID_JANE_SMITH, Name = CONTRACTOR_NAME_JANE_SMITH }
        ];

        public const int ARTICLE_ID_LAPTOP = 1;
        public const int ARTICLE_ID_MOUSE = 2;

        public static readonly List<Article> Articles =
        [
            new Article { Id = ARTICLE_ID_LAPTOP, Name = "Laptop" },
            new Article { Id = ARTICLE_ID_MOUSE, Name = "Mouse" }
        ];
    }
}
