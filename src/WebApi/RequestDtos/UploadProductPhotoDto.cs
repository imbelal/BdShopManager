namespace Domain.Dtos
{
    public class UploadProductPhotoDto
    {
        public IFormFile? File { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}
