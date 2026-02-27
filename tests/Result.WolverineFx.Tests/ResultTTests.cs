namespace Kubis1982.Result
{
    using JasperFx.CodeGeneration;
    using Kubis1982.Result.Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Wolverine;
    using Wolverine.Middleware;
    using Xunit;

    public class ResultTTests
    {
        private static async Task<IHost> CreateHost()
        {
            return await Host.CreateDefaultBuilder()
                .UseWolverine(opts =>
                {
                    opts.CodeGeneration.AddContinuationStrategy<ResultContinuationStrategy>();
                    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
                    opts.Discovery.IncludeAssembly(typeof(ResultTHandler).Assembly);
                })
                .StartAsync();
        }

        [Fact]
        public async Task Should_ExtractValueAndExecuteHandle_When_ResultIsSuccess()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new ResultTCommand(Repository.CONTRACTOR_ID_JOHN_DOE);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Repository.CONTRACTOR_ID_JOHN_DOE, result.Value.Id);
            Assert.Equal(Repository.CONTRACTOR_NAME_JOHN_DOE, result.Value.Name);
        }

        [Fact]
        public async Task Should_ReturnError_When_LoadFails()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var command = new ResultTCommand(100);

            // Act
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
            Assert.Equal(ResultHandler.ContractorNotFoundMessage, result.Error.Description);
        }

        [Fact]
        public async Task Should_HandleCorrectly_When_MultipleUsersExist()
        {
            // Arrange
            using var host = await CreateHost();
            using var scope = host.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            // Act & Assert for User 1
            var command = new ResultTCommand(Repository.CONTRACTOR_ID_JOHN_DOE);
            var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);
            Assert.True(result.IsSuccess);
            Assert.Equal(Repository.CONTRACTOR_NAME_JOHN_DOE, result.Value.Name);

            // Act & Assert for User 2
            command = new ResultTCommand(Repository.CONTRACTOR_ID_JANE_SMITH);
            result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);
            Assert.True(result.IsSuccess);
            Assert.Equal(Repository.CONTRACTOR_NAME_JANE_SMITH, result.Value.Name);
        }
    }
}
