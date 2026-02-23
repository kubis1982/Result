namespace Kubis1982.Result
{
    using Kubis1982.Result.Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Threading.Tasks;
    using Wolverine;
    using Wolverine.Middleware;
    using Xunit;

    public class TupleResultTests
    {
        private static async Task<IHost> CreateHost()
        {
            return await Host.CreateDefaultBuilder()
                .UseWolverine(opts =>
                {
                    opts.CodeGeneration.AddContinuationStrategy<ResultContinuationStrategy>();
                    opts.Discovery.IncludeAssembly(typeof(TupleResultHandler).Assembly);
                })
                .StartAsync();
        }

        [Fact]
        public async Task TupleResult_Success_ShouldExtractTupleAndExecuteHandle()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new TupleResultCommand(ContractorId: Repository.CONTRACTOR_ID_JANE_SMITH, ArticleId: Repository.ARTICLE_ID_LAPTOP); // Both User and Product with ID 1 exist

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Repository.ARTICLE_ID_LAPTOP, result.Value.ArticleId);
            Assert.Equal(Repository.CONTRACTOR_ID_JANE_SMITH, result.Value.ContractorId);
            Assert.True(result.Value.Id > 0);
        }

        [Fact]
        public async Task TupleResult_ValidScenario_ShouldCreateOrder()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new TupleResultCommand(9999,9999);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
