namespace Kubis1982.Result.Handlers
{
    using System.Collections.Concurrent;
    using Wolverine;

    public record PostprocessedCommand(Guid Id, bool Fail);

    public static class PostprocessedHandler
    {
        public static readonly ConcurrentBag<Guid> Postprocessed = [];

        public static Result Handle(PostprocessedCommand command) =>
            command.Fail ? Error.Validation("Handle failed") : Result.Success();

        // Stands in for SaveChangesAsync / transaction commit added by persistence middleware.
        public static void Postprocess(PostprocessedCommand command) => Postprocessed.Add(command.Id);

        public static Task<Result> InvokeAsync(this PostprocessedCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.InvokeAsync<Result>(command, cancellationToken);
        }
    }
}
