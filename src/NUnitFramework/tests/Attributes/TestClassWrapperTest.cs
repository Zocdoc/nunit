using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    class RetryClassAttribute : PropertyAttribute, IWrapSetUpTearDown
    {
        private readonly int _count;

        public RetryClassAttribute(int count) : base(count)
        {
            _count = count;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new RetryWithExceptionCommand(command, _count);
        }
    }

    public class RetryWithExceptionCommand : DelegatingTestCommand
    {
        private readonly int _retryCount;
        
        public RetryWithExceptionCommand(TestCommand innerCommand, int retryCount)
            : base(innerCommand)
        {
            _retryCount = retryCount;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            var count = _retryCount;

            while (count-- > 0)
            {
                context.CurrentResult = innerCommand.Execute(context);

                if (context.CurrentResult.ResultState != ResultState.Failure &&
                    context.CurrentResult.ResultState != ResultState.Error)
                    break;

                // Clear result for retry
                if (count > 0)
                    context.CurrentResult = context.CurrentTest.MakeTestResult();
            }

            return context.CurrentResult;
        }
    }

    [RetryClass(3)]
    class TestClassWrapperTest
	{
		private int x = 0;

		[Test]
		public void TestMethod()
		{
			if (x++ < 2)
			{
				throw new Exception("test");
			}
		    Assert.AreEqual(x, 3);
		}

        [Test]
        public void TestMethod2()
        {
            Assert.AreEqual(3, 3);
        }
    }
}