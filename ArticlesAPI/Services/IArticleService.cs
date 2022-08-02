using ArticlesAPI.Models;

namespace ArticlesAPI.Services
{
    public interface IArticleService
    {
        Task<bool> AddArticle(AddArticleDto article);
        Task<bool> DeleteArticle(string title);
        Task<bool> EditArticle(string title, EditArticleDtoIn article);
        Task<List<GetAllArticlesDto>> AllArticles();
        Task<GetSingleArticleDto> SingleArticle(string title);
    }
}
