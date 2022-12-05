using Nexile.Desktop.Core.Common.Extensions;
using NUnit.Framework;

namespace Nexile.Tests.Common
{
    public static class TestProjectExtensionMethods
    {
        public static void Log<T>(this T obj)
        {
            TestContext.WriteLine(obj.ToJsonString());
        }
    }
}