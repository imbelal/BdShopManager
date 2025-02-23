using Domain.Entities;

namespace UnitTest
{
    public class ProductAggTest
    {
        [Fact]
        public void ShouldUpdateTagsForProduct()
        {
            // Arrange
            Guid tag1 = Guid.NewGuid();
            Guid tag2 = Guid.NewGuid();
            Guid tag3 = Guid.NewGuid();
            List<Guid> initialListOfTagIds = [tag1];
            Product product = new("Test title", "Test description", Guid.NewGuid(), Domain.Enums.ProductUnit.Box, initialListOfTagIds);

            //Act
            List<Guid> newListOfTagIds = [tag2, tag3];
            product.UpdateTags(newListOfTagIds);

            // Assert
            Assert.DoesNotContain(tag1, product.ProductTags.Select(pt => pt.TagId));
            Assert.Equal(newListOfTagIds, product.ProductTags.Select(pt => pt.TagId));
            Assert.Equal(newListOfTagIds.Count, product.ProductTags.Count);
        }
    }
}
