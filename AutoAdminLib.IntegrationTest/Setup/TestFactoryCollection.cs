using Xunit;

namespace AutoAdminLib.IntegrationTest.Setup
{
    [CollectionDefinition("AutoAdmin Collection")]
    public class TestFactoryCollection: ICollectionFixture<TestFactory>
    {
    }
}