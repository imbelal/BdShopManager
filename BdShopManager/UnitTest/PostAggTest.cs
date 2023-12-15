using Domain.Entities;

namespace UnitTest
{
    public class PostAggTest
    {
        [Fact]
        public void ShouldUpdateTagsForPost()
        {
            // Arranage
            Guid tag1 = Guid.NewGuid();
            Guid tag2 = Guid.NewGuid();
            Guid tag3 = Guid.NewGuid();
            List<Guid> initiallistOfTagIds = new List<Guid>();
            initiallistOfTagIds.Add(tag1);
            Product post = new("Test title", "Test description", Guid.NewGuid(), 5, Domain.Enums.ProductUnit.Box, initiallistOfTagIds);

            //Act
            List<Guid> newlistOfTagIds = new List<Guid>();
            newlistOfTagIds.Add(tag2);
            newlistOfTagIds.Add(tag3);
            post.UpdateTags(newlistOfTagIds);

            // Assert
            Assert.DoesNotContain(tag1, post.PostTags.Select(pt => pt.TagId));
            Assert.Equal(newlistOfTagIds, post.PostTags.Select(pt => pt.TagId));
            Assert.Equal(newlistOfTagIds.Count, post.PostTags.Count);
        }
    }
}
