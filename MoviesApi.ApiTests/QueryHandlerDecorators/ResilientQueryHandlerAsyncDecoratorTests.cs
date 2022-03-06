using Moq;
using MoviesAPI.CQS;
using MoviesAPI.QueryHandlerDecorators;
using MoviesAPI.Utilities;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.QueryHandlerDecorators
{
    [TestFixture]
    public class ResilientQueryHandlerAsyncDecoratorTests
    {
        [Test]
        public void MustProvidePolicyRegistry()
        {
            var decoratee = Mock.Of<IQueryHandlerAsync<IQuery<object>, object>>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new ResilientQueryHandlerAsyncDecorator<IQuery<object>, object>(null, decoratee));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void MustProvidePolicyRegistryWithSqlResiliencePolicy()
        {
            var emptyPolicyRegistry = new PolicyRegistry();
            var decoratee = Mock.Of<IQueryHandlerAsync<IQuery<object>, object>>();

            Assert.Throws<ArgumentException>(() => new ResilientQueryHandlerAsyncDecorator<IQuery<object>, object>(emptyPolicyRegistry, decoratee));
        }

        [Test]
        public void MustProvidePolicyRegistryWithSqlResiliencePolicyThatIsAsync()
        {
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, Policy.NoOp() } };
            var decoratee = Mock.Of<IQueryHandlerAsync<IQuery<object>, object>>();

            Assert.Throws<InvalidCastException>(() => new ResilientQueryHandlerAsyncDecorator<IQuery<object>, object>(policyRegistry, decoratee));
        }

        [Test]
        public async Task MustUsePolicyExecuteAsyncOnce()
        {
            var asyncPolicyMock =  new Mock<IAsyncPolicy>();
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, asyncPolicyMock.Object } };
            var decoratee = Mock.Of<IQueryHandlerAsync<IQuery<object>, object>>();
            var sut = new ResilientQueryHandlerAsyncDecorator<IQuery<object>, object>(policyRegistry, decoratee);

            await sut.HandleAsync(new EmptyQuery());

            asyncPolicyMock.Verify(p => p.ExecuteAsync(It.IsAny<Func<Task<object>>>()), Times.Once);
        }

        [Test]
        public async Task MustUseDecorateeHandleAsyncOnce()
        {
            var policyRegistry = new PolicyRegistry { { PollyPolicies.SqlResiliencePolicy, Policy.NoOpAsync() } };
            var decorateeMock = new Mock<IQueryHandlerAsync<IQuery<object>, object>>();
            var sut = new ResilientQueryHandlerAsyncDecorator<IQuery<object>, object>(policyRegistry, decorateeMock.Object);

            await sut.HandleAsync(new EmptyQuery());

            decorateeMock.Verify(p => p.HandleAsync(It.IsAny<IQuery<object>>()), Times.Once);
        }

        private class EmptyQuery : IQuery<object> {}
    }
}
