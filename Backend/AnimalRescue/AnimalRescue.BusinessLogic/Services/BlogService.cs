﻿using AnimalRescue.Contracts;
using AnimalRescue.Contracts.Query;
using AnimalRescue.DataAccess.Contracts.Query;
using AnimalRescue.DataAccess.Mongodb.Interfaces.Repositories;
using AnimalRescue.Models.DTO.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalRescue.BusinessLogic.Services
{
    internal class BlogService : IBlogService
	{
		private readonly IBlogRepository _blogRepository;

		public BlogService(IBlogRepository blogRepository)
		{
			_blogRepository = blogRepository;
		}

		public async Task<(IList<BlogDto> blogDtos, int totalCount)> GetAllBlogsAsync(ApiQueryRequest apiQueryRequest)
		{
            var dbQuery = apiQueryRequest.ToDbQuery();

            int totalCount = await _blogRepository.GetBlogsCountAsync(dbQuery);

			var blogs = await _blogRepository.GetBlogsWithPagginationAsync(dbQuery);

			var blogModels = new List<BlogDto>();

			foreach (var blog in blogs)
			{
				var blogModel = new BlogDto()
				{
					Id = blog.Id,
					Description = blog.Description,
					Body = blog.Body,
					ImageIds = blog.ImagesIds,
					CreatedAt = blog.CreatedAt
				};

				blogModels.Add(blogModel);
			}

			return (blogModels, totalCount);
		}
	}
}
