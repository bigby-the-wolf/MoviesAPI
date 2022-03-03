using Moq;
using MoviesAPI.CommandHandlerDecorators;
using MoviesAPI.CQS;
using MoviesAPI.Utilities;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.CommandHandlerDecorators
{
    [TestFixture]
    public class ResilientCommandHandlerAsyncDecoratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MustProvidePolicyRegistry()
        {
            var decoratee = Mock.Of<ICommandHandlerAsync<ICommand>>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new ResilientCommandHandlerAsyncDecorator<ICommand>(null, decoratee));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void MustProvidePolicyRegistryWithSqlResiliencePolicy()
        {
            var emptyPolicyRegistry = new PolicyRegistry();
            var decoratee = Mock.Of<ICommandHandlerAsync<ICommand>>();

            Assert.Throws<ArgumentException>(() => new ResilientCommandHandlerAsyncDecorator<ICommand>(emptyPolicyRegistry, decoratee));
        }

        [Test]
        public void MustProvidePolicyRegistryWithSqlResiliencePolicyThatIsAsync()
        {
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, Policy.NoOp() } };
            var decoratee = Mock.Of<ICommandHandlerAsync<ICommand>>();

            Assert.Throws<InvalidCastException>(() => new ResilientCommandHandlerAsyncDecorator<ICommand>(policyRegistry, decoratee));
        }

        [Test]
        public async Task MustUsePolicyExecuteAsyncOnce()
        {
            var asyncPolicyMock =  new Mock<IAsyncPolicy>();
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, asyncPolicyMock.Object } };
            var decoratee = Mock.Of<ICommandHandlerAsync<ICommand>>();
            var sut = new ResilientCommandHandlerAsyncDecorator<ICommand>(policyRegistry, decoratee);

            await sut.HandleAsync(new EmptyCommand());

            asyncPolicyMock.Verify(p => p.ExecuteAsync(It.IsAny<Func<Task>>()), Times.Once);
        }

        [Test]
        public async Task MustUseDecorateeHandleAsyncOnce()
        {
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, Policy.NoOpAsync() } };
            var decorateeMock = new Mock<ICommandHandlerAsync<ICommand>>();
            var sut = new ResilientCommandHandlerAsyncDecorator<ICommand>(policyRegistry, decorateeMock.Object);

            await sut.HandleAsync(new EmptyCommand());

            decorateeMock.Verify(p => p.HandleAsync(It.IsAny<ICommand>()), Times.Once);
        }

        private class EmptyCommand : ICommand {}
    }
}
