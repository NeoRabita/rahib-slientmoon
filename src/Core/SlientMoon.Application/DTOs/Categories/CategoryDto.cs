namespace SlientMoon.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string CategoryTypeId { get; set; }
        public string? IconUrl { get; set; }
    }
}
