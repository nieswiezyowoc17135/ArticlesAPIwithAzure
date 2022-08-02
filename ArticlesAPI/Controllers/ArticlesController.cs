using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticlesAPI.Data;
using ArticlesAPI.Entities;
using ArticlesAPI.Models;
using ArticlesAPI.Services;

namespace ArticlesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<List<GetAllArticlesDto>>> GetArticles()
        {
            return await _articleService.AllArticles();
        }

        // GET: api/Articles/title
        [HttpGet("string")]
        public async Task<ActionResult<GetSingleArticleDto>> GetArticle(string title)
        {
            return await _articleService.SingleArticle(title);
        }

        // PUT: api/Articles/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<bool>> PutArticle(string title, EditArticleDtoIn article)
        {
            if (await _articleService.EditArticle(title, article))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostArticle([FromForm]AddArticleDto article)
        {

            if (await _articleService.AddArticle(article))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/Articles/title1
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteArticle(string articleTitle)
        {
            if (await _articleService.DeleteArticle(articleTitle))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
