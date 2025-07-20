using Common.Entities;

namespace Domain.Entities
{
    public class ProductPhoto : AuditableEntityBase<Guid>
    {
        public Guid ProductId { get; private set; }
        public string FileName { get; private set; }
        public string OriginalFileName { get; private set; }
        public string ContentType { get; private set; }
        public long FileSize { get; private set; }
        public string BlobUrl { get; private set; }
        public bool IsPrimary { get; private set; }
        public int DisplayOrder { get; private set; }

        public ProductPhoto() : base()
        {
        }

        public ProductPhoto(Guid productId, string fileName, string originalFileName, string contentType, long fileSize, string blobUrl, bool isPrimary = false, int displayOrder = 0) : base(Guid.NewGuid())
        {
            ProductId = productId;
            FileName = fileName;
            OriginalFileName = originalFileName;
            ContentType = contentType;
            FileSize = fileSize;
            BlobUrl = blobUrl;
            IsPrimary = isPrimary;
            DisplayOrder = displayOrder;
        }

        public void SetAsPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }

        public void SetDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }

        public void UpdateBlobUrl(string newBlobUrl)
        {
            BlobUrl = newBlobUrl;
        }
    }
}