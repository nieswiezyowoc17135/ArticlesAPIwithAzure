using ArticlesAPI.Data;
using ArticlesAPI.Entities;
using ArticlesAPI.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.EntityFrameworkCore;

namespace ArticlesAPI.Services
{
    public class ArticleService : IArticleService
    {
        //przekazywanie contextu bazy danych do serwisu
        private readonly ArticlesContext _context;
        private string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=psarticle;AccountKey=5vkxEfcI2YJEWi9HrIeP+lJZCOA48uEWz1eV9/bRPfASocHHfbcyb7h/hcLN6cCVfw9rI8AKm+xU+AStmyFYCg==;EndpointSuffix=core.windows.net";
        private string blobStorageContainername = "files";

        public ArticleService(ArticlesContext context)
        {
            _context = context;
        }

        //dodawanie
        public async Task<bool> AddArticle(AddArticleDto article)
        {
            var newArticle = new Article();
            newArticle.Title = article.Title;
            newArticle.Description = article.Description;
            newArticle.ImageURL = article.File.FileName;

            //dodawanie image do storage accounta
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainername);
            var blob = container.GetBlobClient(article.File.FileName);
            
            var stream = article.File.OpenReadStream();
            await blob.UploadAsync(stream);
            ///////////////blob////////////////

            _context.Articles.Add(newArticle);
            await _context.SaveChangesAsync();
            return true;
        }

        //usuwanie
        public async Task<bool> DeleteArticle(string articleTitle)
        {
            if (_context.Articles == null)
            {
                throw new Exception("Baza danych jest pusta");
            }

            var article = _context.Articles.FirstOrDefault(x => x.Title == articleTitle);

            if (article == null)
            {
                return false;
            }

            _context.Articles.Remove(article);

            //usuwanie image do storage accounta
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainername);
            var blob = container.GetBlobClient(article.ImageURL);
            await blob.DeleteAsync();

            if (await _context.SaveChangesAsync() == 1)
            {
                return true;
            }
            return false;
        }

        //edytowanie
        public async Task<bool> EditArticle(string articleTitle, EditArticleDtoIn article)
        {
            //szukanie artykulu do edycji
            var _article = _context.Articles.FirstOrDefault(x => x.Title == articleTitle);

            //sprawdzanie czy dany artykul istnieje
            if (_article == null)
            {
                throw new Exception("Nie ma takiego artykułu");
            }

            //usuwanie pliku z storage accounta AZURE
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainername);
            var blob = container.GetBlobClient(_article.ImageURL);
            await blob.DeleteAsync();

            //edytowanie krotek w bazie danych 
            _article.Title = article.Title;
            _article.Description = article.Description;
            _article.ImageURL = article.File.FileName;

            //dodawanie nowego pliku do storage accounta
            blob = container.GetBlobClient(_article.ImageURL);
            var stream = article.File.OpenReadStream();
            await blob.UploadAsync(stream);

            if (await _context.SaveChangesAsync() == 1)
            {
                return true;
            }

            throw new Exception("Nie dziala");

        }

        //wyswietlanie wszystkich
        public async Task<List<GetAllArticlesDto>> AllArticles()
        {
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainername);
            var containerUri = container.Uri.AbsoluteUri;

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                Resource = "c",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(20),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("psarticle", "5vkxEfcI2YJEWi9HrIeP+lJZCOA48uEWz1eV9/bRPfASocHHfbcyb7h/hcLN6cCVfw9rI8AKm+xU+AStmyFYCg==")).ToString();

            return await _context.Articles.Select(x => new GetAllArticlesDto
            {
                Title = x.Title,
                Description = x.Description,
                ImageURL = containerUri+"/"+x.ImageURL+"?"+sasToken
            }).ToListAsync();
        }

        public async Task<GetSingleArticleDto> SingleArticle(string title)
        {
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainername);
            var containerUri = container.Uri.AbsoluteUri;
            if (_context.Articles == null)
            {
                throw new Exception("Baza danych jest pusta");
            }

            var _article = _context.Articles.FirstOrDefault(x => x.Title == title);
            
            if (_article == null)
            {
                throw new Exception("Cos zawiodlo");
            }

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(5),
                BlobName = _article.ImageURL
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("psarticle", "5vkxEfcI2YJEWi9HrIeP+lJZCOA48uEWz1eV9/bRPfASocHHfbcyb7h/hcLN6cCVfw9rI8AKm+xU+AStmyFYCg==")).ToString();

            var _articleToShowOff = new GetSingleArticleDto();
            _articleToShowOff.Title = _article.Title;
            _articleToShowOff.Description = _article.Description;
            _articleToShowOff.ImageURL = containerUri + "/" + _article.ImageURL+ "?" + sasToken;
            return _articleToShowOff;
        }
    }
}
