using Xunit;

namespace AutoAdmin.IntegrationTest.Setup
{
    [CollectionDefinition("AutoAdmin Collection")]
    public class TestFactoryCollection: ICollectionFixture<TestFactory>
    {
    }
}