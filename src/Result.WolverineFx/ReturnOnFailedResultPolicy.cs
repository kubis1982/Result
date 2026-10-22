namespace Kubis1982.Result
{
    using JasperFx;
    using JasperFx.CodeGeneration;
    using JasperFx.CodeGeneration.Frames;
    using JasperFx.CodeGeneration.Model;
    using Wolverine.Configuration;
    using Wolverine.Runtime.Handlers;

    /// <summary>
    /// Handler policy that stops the handler chain when the <c>Handle</c> method returns a failed <see cref="Result"/> or <see cref="Result{T}"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="ResultContinuationStrategy"/> only stops the chain for middleware methods (<c>LoadAsync</c>, <c>Before</c>, ...).
    /// Without this policy a failure returned from <c>Handle</c> still runs postprocessors such as <c>SaveChangesAsync</c>
    /// and the transaction commit. Cascading messages (the response for <c>InvokeAsync</c>) are still enqueued.
    /// </remarks>
    public sealed class ReturnOnFailedResultPolicy : IHandlerPolicy
    {
        /// <summary>
        /// Inserts a failure check before the postprocessors of every chain whose handler returns a Result.
        /// </summary>
        public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container)
        {
            foreach (var chain in chains)
            {
                var result = chain.Handlers.SelectMany(h => h.Creates).FirstOrDefault(v => IsResult(v.VariableType));
                if (result is not null)
                {
                    chain.Postprocessors.Insert(0, new ReturnOnFailureFrame(result));
                }
            }
        }

        private static bool IsResult(Type type) =>
            type == typeof(Result) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>));

        private sealed class ReturnOnFailureFrame(Variable result) : SyncFrame
        {
            public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
            {
                writer.WriteComment("Failed Result from Handle: skip postprocessors (SaveChanges, commit)");
                writer.Write($"BLOCK:if ({result.Usage}.IsFailure)");
                writer.Write("return;");
                writer.FinishBlock();
                Next?.GenerateCode(method, writer);
            }

            public override IEnumerable<Variable> FindVariables(IMethodVariables chain)
            {
                yield return result;
            }
        }
    }
}
