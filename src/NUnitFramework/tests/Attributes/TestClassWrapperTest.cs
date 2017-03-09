using System;
using NUnit.Framework.Interfaces;
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
            return new RetryAttribute.RetryCommand(command, _count);
        }
    }

    [RetryClass(3)]
    class TestClassWrapperTest
	{
		private int x = 0;

		[Test]
		public void TestMethod()
		{
		    Assert.AreEqual(x++, 2);
		}
    }
}