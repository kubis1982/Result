namespace Kubis1982.Result.Handlers
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Wolverine;

    public static class TupleResultHandler
    {
        public static Task<Result<(Contractor contractor, Article article)>> LoadAsync(TupleResultCommand command)
        {
            var contractor = Repository.Contractors.FirstOrDefault(u => u.Id == command.ContractorId);
            var article = Repository.Articles.FirstOrDefault(p => p.Id == command.ArticleId);
            if (contractor == null) return Task.FromResult((Result<(Contractor, Article)>)Error.NotFound("Contractor not found"));
            if (article == null) return Task.FromResult((Result<(Contractor, Article)>)Error.NotFound("Article not found"));
            return Task.FromResult(Result.Success((contractor, article)));
        }

        public static Task<Result<OrderDto>> Handle(TupleResultCommand command, (Contractor contractor, Article article) loadAsyncSuccessValue)
        {
            var orderDto = new OrderDto(Random.Shared.Next(1000, 2000),
                loadAsyncSuccessValue.article.Id,
                loadAsyncSuccessValue.contractor.Id                
            );

            return Task.FromResult(Result.Success(orderDto));
        }

        public static Task<Result<OrderDto>> InvokeAsync(this TupleResultCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.InvokeAsync<Result<OrderDto>>(command, cancellationToken);
        }
    }
}
