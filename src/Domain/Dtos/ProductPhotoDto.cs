namespace Domain.Dtos
{
    public class ProductPhotoDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string BlobUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
} 