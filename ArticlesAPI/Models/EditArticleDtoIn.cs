namespace ArticlesAPI.Models
{
    public class EditArticleDtoIn
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
     }
}
