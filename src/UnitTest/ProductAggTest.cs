using Domain.Entities;

namespace UnitTest
{
    public class ProductAggTest
    {
        [Fact]
        public void ShouldUpdateTagsForproduct()
        {
            // Arranage
            Guid tag1 = Guid.NewGuid();
            Guid tag2 = Guid.NewGuid();
            Guid tag3 = Guid.NewGuid();
            List<Guid> initiallistOfTagIds = [tag1];
            Product product = new("Test title", "Test description", Guid.NewGuid(), Domain.Enums.ProductUnit.Box, initiallistOfTagIds);

            //Act
            List<Guid> newlistOfTagIds = [tag2, tag3];
            product.UpdateTags(newlistOfTagIds);

            // Assert
            Assert.DoesNotContain(tag1, product.ProductTags.Select(pt => pt.TagId));
            Assert.Equal(newlistOfTagIds, product.ProductTags.Select(pt => pt.TagId));
            Assert.Equal(newlistOfTagIds.Count, product.ProductTags.Count);
        }
    }
}
