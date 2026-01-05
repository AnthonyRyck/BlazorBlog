using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace BlazorBlog.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BlogController : ControllerBase
	{
		private readonly BlogContext blogContext;

		public BlogController(BlogContext context)
		{
			blogContext = context;
		}

		// GET api/blog/posts
		[HttpGet("{posts}")]
		public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
		{
			try
			{
				var posts= await blogContext.GetPublishedPostsAsync();
				return posts;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "API ERROR - GetPosts");
				return StatusCode(500);
			}
		}

		// GET api/blog/post/5
		[HttpGet("post/{idpost}")]
		public async Task<ActionResult<Post>> GetPost(int idpost)
		{
			try
			{
				var post = await blogContext.GetPostAsync(idpost);
				if (post == null)
				{
					return NotFound();
				}
				
				return post;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"API ERROR - GetPost {idpost}");
				return StatusCode(500);
			}
		}
	}
}
