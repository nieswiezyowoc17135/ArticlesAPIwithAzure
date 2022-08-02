namespace ArticlesAPI.Models
{
    public class AddArticleDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File{ get; set; }
    }
}
