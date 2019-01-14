using System.Threading;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class SomeDataFixture
    {
        public SomeDataFixture()
        {
            Thread.Sleep(5000);
        }
    }
}