namespace Kubis1982.Result
{
    using JasperFx;
    using JasperFx.CodeGeneration;
    using JasperFx.CodeGeneration.Frames;
    using JasperFx.RuntimeCompiler;
    using Kubis1982.Result.Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Wolverine;
    using Wolverine.Configuration;
    using Wolverine.Middleware;
    using Wolverine.Runtime.Handlers;
    using Xunit;

    public class ReturnOnFailedResultPolicyTests
    {
        private sealed class PostprocessorPolicy : IHandlerPolicy
        {
            public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container)
            {
                foreach (var chain in chains.Where(c => c.MessageType == typeof(PostprocessedCommand)))
                {
                    chain.Postprocessors.Add(new MethodCall(typeof(PostprocessedHandler), nameof(PostprocessedHandler.Postprocess)));
                }
            }
        }

        private static async Task<IHost> CreateHost()
        {
            return await Host.CreateDefaultBuilder()
                .UseWolverine(opts =>
                {
                    opts.CodeGeneration.AddContinuationStrategy<ResultContinuationStrategy>();
                    opts.Policies.Add<PostprocessorPolicy>();
                    opts.Policies.Add<ReturnOnFailedResultPolicy>();
                    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
                    opts.Services.AddRuntimeCompilation();
                    opts.Discovery.IncludeAssembly(typeof(PostprocessedHandler).Assembly);
                })
                .StartAsync();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task Should_RunPostprocessorsOnlyOnSuccess(bool fail, bool expectedPostprocessed)
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new PostprocessedCommand(Guid.NewGuid(), fail);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(fail, result.IsFailure);
            Assert.Equal(expectedPostprocessed, PostprocessedHandler.Postprocessed.Contains(command.Id));
        }
    }
}
