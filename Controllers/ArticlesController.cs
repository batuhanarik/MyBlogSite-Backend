using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogAngularCore.API.Models;
using MyBlogAngularCore.API.Responses;

namespace MyBlogAngularCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly MyBlogDBContext _context;

        public ArticlesController(MyBlogDBContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public IActionResult GetArticles()
        {
            var articles =_context.Articles.Include(a=>a.Category).Include(b=>b.Comments)
                .OrderByDescending(x=>x.PublishDate).ToList().Select(y=>new ArticleResponse() { 
                Id = y.Id,
                Title = y.Title,
                Picture = y.Picture,
                Category = new CategoryResponse() { Id = y.CategoryId, Name = y.Category.Name },
                CommentCount = y.Comments.Count,
                ViewCount = y.ViewCount,
                PublishDate = y.PublishDate
            });
            return Ok(articles);
        }




        public System.Tuple<IEnumerable<ArticleResponse>,int> ArticlesPagination(IQueryable<Article> query , int page ,  int pageSize)
        {
            int totalCount = query.Count();

            var articleResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).ToList().Select(x => new ArticleResponse()
            {
                Id = x.Id,
                Title = x.Title,
                ContentMain = x.ContentMain,
                ContentSummary = x.ContentSummary,
                Picture = x.Picture,
                ViewCount = x.ViewCount,
                CommentCount = x.Comments.Count,
                Category = new CategoryResponse() { Id = x.CategoryId, Name = x.Category.Name }
            });

            return new System.Tuple<IEnumerable<ArticleResponse>, int>(articleResponse, totalCount);


        }

        

        [HttpGet("{page}/{pageSize}")]
        public IActionResult GetArticles(int page=1,int pageSize = 5)
        {
            try
            {
                IQueryable<Article> query;
                query = _context.Articles.Include(x => x.Category).Include(y => y.Comments).OrderByDescending(z => z.PublishDate);

                int totalCount = query.Count();

                var articleResponse = query.Skip((pageSize * (page - 1))).Take(5).ToList().Select(x => new ArticleResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentMain = x.ContentMain,
                    ContentSummary = x.ContentSummary,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comments.Count,
                    Category = new CategoryResponse() { Id = x.CategoryId, Name = x.Category.Name }
                });
                var result = new
                {
                    TotalCount = totalCount,
                    Articles = articleResponse
                };
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            
        }


        [HttpGet]
        [Route("GetArticlesByCategoryId/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticlesByCategoryId(int categoryId,int page=1,int pageSize = 5)
        {
            IQueryable<Article> query = _context.Articles.Include(x => x.Category).Include(y => y.Comments).Where(z => z.CategoryId == categoryId)
                .OrderByDescending(x => x.PublishDate);

            var queryResult = ArticlesPagination(query, page, pageSize);

            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }

        [HttpGet]
        [Route("SearchArticles/{searchText}/{page}/{pageSize}")]
        public IActionResult SearchArticles(string searchText,int page=1,int pageSize=5)
        {
            IQueryable<Article> query;
            query = _context.Articles.Include(x => x.Category).Include(y => y.Comments)
                .Where(z => z.Title.Contains(searchText)).OrderByDescending(f => f.PublishDate);

            var resultQuery = ArticlesPagination(query, page, pageSize);
            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("GetArticlesByMostView")]
        public IActionResult GetArticlesByMostView()
        {
            var articles = _context.Articles.OrderByDescending(x => x.ViewCount).Take(5).Select(x => new ArticleResponse()
            {
                Title = x.Title,
                Id = x.Id
            });

            return Ok(articles);
        }

        [HttpGet]
        [Route("GetArticlesByArchive")]
        public IActionResult GetArticlesByArchive()
        {
            System.Threading.Thread.Sleep(900);

            CultureInfo cultureInfo = new CultureInfo("tr");

            var query = _context.Articles.GroupBy(x => new { x.PublishDate.Year, x.PublishDate.Month }).OrderByDescending(x => x.Key.Year).Select(y=> new { 
                year = y.Key.Year,
                month  = y.Key.Month,
                count = y.Count(),
                monthName = new DateTime(y.Key.Year,y.Key.Month,1).ToString("MMMM", cultureInfo)
            });

            return Ok(query);
        }
        [HttpGet]
        [Route("GetArticleArchiveList/{year}/{month}/{page}/{pageSize}")]
        public IActionResult GetArticleArchiveList(int year,int month,int page,int pageSize)
        { 
            IQueryable<Article> query;
            query = _context.Articles.Include(x => x.Category).Include(y => y.Comments).Where(z => z.PublishDate.Year == year &&
                  z.PublishDate.Month == month).OrderByDescending(f => f.PublishDate);

            var resultQuery = ArticlesPagination(query, page, pageSize);
            var result = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };

            return Ok(result);
        }



        // GET: api/Articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticle(int id)
        {
            var article = _context.Articles.Include(x => x.Category).Include(y => y.Comments).FirstOrDefault(z => z.Id == id);
            if(article == null)
            {
                return NotFound();
            }
            else
            {
                var articleResponse = new ArticleResponse()
                {
                    Id = article.Id,
                    Title = article.Title,
                    ContentMain = article.ContentMain,
                    ContentSummary = article.ContentSummary,
                    Picture = article.Picture,
                    PublishDate = article.PublishDate,
                    ViewCount = article.ViewCount,
                    Category = new CategoryResponse() { Id = article.Category.Id, Name = article.Category.Name },
                    CommentCount = article.Comments.Count
                };
                return Ok(articleResponse);
            }
            
            
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            Article firstArticle = _context.Articles.Find(id);

            firstArticle.Title = article.Title;
            firstArticle.ContentSummary = article.ContentSummary;
            firstArticle.ContentMain = article.ContentMain;
            firstArticle.CategoryId = article.Category.Id;
            firstArticle.Picture = article.Picture;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostArticle(Article article)
        {
            if (article.Category != null)
            {
                article.CategoryId = article.Category.Id;

            }
            article.Category = null;
            article.ViewCount = 0;
            article.PublishDate = DateTime.Now;
           

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }


        [HttpGet()]
        [Route("ArticleViewCountUp/{id}")]
        public IActionResult ArticleViewCountUp(int id)
        {
            Article article = _context.Articles.Find(id);
            article.ViewCount += 1;
            _context.SaveChanges();

            return Ok();

        }

        [HttpPost]
        [Route("SaveArticlePicture")]
        public async Task<IActionResult> SaveArticlePicture(IFormFile picture)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/articlePictures",fileName);

            using (var stream = new FileStream(path,FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            };

            var result = new
            {
                path = "https://" + Request.Host + "/articlePictures/" + fileName
            };

            return Ok(result);

        }
    }
}
