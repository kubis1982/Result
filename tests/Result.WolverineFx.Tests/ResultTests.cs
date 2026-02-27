namespace Kubis1982.Result
{
    using JasperFx.CodeGeneration;
    using Kubis1982.Result.Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Wolverine;
    using Wolverine.Middleware;
    using Xunit;

    public class ResultTests
    {
        private static async Task<IHost> CreateHost()
        {
            return await Host.CreateDefaultBuilder()
                .UseWolverine(opts =>
                {
                    opts.CodeGeneration.AddContinuationStrategy<ResultContinuationStrategy>();
                    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
                    opts.Discovery.IncludeAssembly(typeof(ResultHandler).Assembly);
                })
                .StartAsync();
        }

        [Fact]
        public async Task Should_ExecuteHandle_When_ResultIsSuccess()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new ResultCommand(Repository.CONTRACTOR_ID_JOHN_DOE);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Processed command with ID: 1", result.Value);
        }

        [Fact]
        public async Task Should_ReturnNotFoundError_When_ResultIsNotFound()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new ResultCommand(100);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
            Assert.Equal(ResultHandler.ContractorNotFoundMessage, result.Error.Description);
        }
    }
}
