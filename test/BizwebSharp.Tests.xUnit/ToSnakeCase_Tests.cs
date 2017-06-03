using BizwebSharp.Entities;
using BizwebSharp.Extensions;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "ToSnakeCase")]
    public class ToSnakeCase_Tests
    {
        [Fact(DisplayName = "When convert class name to snake_case, it's should create a snake_case string")]
        public void ClassNameToSnakeCase()
        {
            Assert.Equal("order", typeof(Order).Name.ToSnakeCase());
            Assert.Equal("script_tag", typeof(ScriptTag).Name.ToSnakeCase());
            Assert.Equal("shipping_line", typeof(ShippingLine).Name.ToSnakeCase());
            Assert.Equal("custom_collection", typeof(CustomCollection).Name.ToSnakeCase());

            var snakeCaseClassName = typeof(ItIsALongClassNameToTestToSnakeCase).Name.ToSnakeCase();
            Assert.Equal(ItIsALongClassNameToTestToSnakeCase.SnakeCaseName, snakeCaseClassName);
        }
    }

    public class ItIsALongClassNameToTestToSnakeCase
    {
        public const string SnakeCaseName = "it_is_a_long_class_name_to_test_to_snake_case";
    }
}
