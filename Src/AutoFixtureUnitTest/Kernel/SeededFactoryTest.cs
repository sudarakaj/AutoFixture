﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Ploeh.AutoFixture.Kernel;
using Xunit.Extensions;

namespace Ploeh.AutoFixtureUnitTest.Kernel
{
    public class SeededFactoryTest
    {
        [Fact]
        public void SutIsSpecimenBuilder()
        {
            // Fixture setup
            Func<object, object> dummyFunc = obj => new object();
            // Exercise system
            var sut = new SeededFactory<object>(dummyFunc);
            // Verify outcome
            Assert.IsAssignableFrom<ISpecimenBuilder>(sut);
            // Teardown
        }

        [Fact]
        public void InitializeWithNullFuncThrows()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new SeededFactory<object>((Func<object, object>)null));
            // Teardown
        }

        [Fact]
        public void CreateWithAnonymousRequestReturnsCorrectResult()
        {
            // Fixture setup
            var sut = new SeededFactory<string>(s => s);
            var request = new object();
            // Exercise system
            var dummyContainer = new DelegatingSpecimenContainer();
            var result = sut.Create(request, dummyContainer);
            // Verify outcome
            var expectedResult = new NoSpecimen(request);
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("foo")]
        public void CreateWithInvalidRequestReturnsCorrectResult(object request)
        {
            // Fixture setup
            var sut = new SeededFactory<int>(s => s);
            // Exercise system
            var dummyContainer = new DelegatingSpecimenContainer();
            var result = sut.Create(request, dummyContainer);
            // Verify outcome
            var expectedResult = new NoSpecimen(request);
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(1, "foo")]
        [InlineData(1, 1)]
        [InlineData(typeof(decimal), 1)]
        [InlineData(typeof(int), "foo")]
        [InlineData(typeof(decimal), null)]
        public void CreateWithInvalidSeededRequestReturnsCorrectResult(object request, object seed)
        {
            // Fixture setup
            var sut = new SeededFactory<int>(s => s);
            var seededRequest = new SeededRequest(request, seed);
            // Exercise system
            var dummyContainer = new DelegatingSpecimenContainer();
            var result = sut.Create(seededRequest, dummyContainer);
            // Verify outcome
            var expectedResult = new NoSpecimen(seededRequest);
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Fact]
        public void CreateWithCorrectSeedWillReturnCorrectResult()
        {
            // Fixture setup
            var seed = 7m;
            var seededRequest = new SeededRequest(typeof(decimal), seed);
            var expectedResult = 3m;

            Func<decimal, decimal> factoryStub = s => s == seed ? expectedResult : 0m;

            var sut = new SeededFactory<decimal>(factoryStub);
            // Exercise system
            var dummyContainer = new DelegatingSpecimenContainer();
            var result = sut.Create(seededRequest, dummyContainer);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }
    }
}