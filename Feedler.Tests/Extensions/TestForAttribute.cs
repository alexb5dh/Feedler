using System;
using Xunit;

namespace Feedler.Tests.Extensions
{
    /// <summary>
    /// Used to provide controller/service method backreference when creating tests via <see cref="TestHost{TStartup}"/>.
    /// </summary>
    ///
    /// <example> <code>
    /// TestFor(nameof(MyController.MyAction))
    /// </code> </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TestForAttribute: FactAttribute
    {
        public TestForAttribute(string method) { }
    }
}