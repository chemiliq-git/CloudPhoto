namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.UsersServices;
    using CloudPhoto.Services.Data.VotesService;
    using ViewModels.Categories;
    using ViewModels.Images;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class ImagesController : BaseController
    {
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;

        public ImagesController(
            IImagesService imagesService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager,
            ILogger<ImagesController> logger)
        {
            this.imagesService = imagesService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
            this.logger = logger;
        }

        // GET: Images
        public IActionResult Index()
        {
            return View();
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            CreateImageViewModel model = new CreateImageViewModel
            {
                Categories = categoriesService.GetAll<CategoryDropDownViewModel>(),
            };
            return View(model);
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateImageViewModel image)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                List<string> lstImageTag = JsonSerializer.Deserialize<List<string>>(image.ImageTags);

                string newId = await imagesService.CreateAsync(
                    new CreateImageModelData()
                    {
                        Id = image.ImageId,
                        Title = image.Title,
                        Description = image.Description,
                        CategoryId = image.CategoryId,
                        ImageUrl = image.ImageUrl,
                        AuthorId = user.Id,
                        Tags = lstImageTag,
                    });

                if (string.IsNullOrEmpty(newId))
                {
                    image.Categories = categoriesService.GetAll<CategoryDropDownViewModel>();
                    return View(image);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            image.Categories = categoriesService.GetAll<CategoryDropDownViewModel>();
            return View(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetSearchingData(
          int pageIndex,
          int pageSize,
          string searchText,
          string selectCategory)
        {
            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = searchText,
            };
            if (!string.IsNullOrEmpty(selectCategory))
            {
                localSearchData.FilterCategory = JsonSerializer.Deserialize<List<string>>(selectCategory);
            }

            if (pageSize == 0)
            {
                return BadRequest();
            }

            if (pageIndex == 0)
            {
                return BadRequest();
            }

            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await userManager.GetUserAsync(User);
                localSearchData.LikeForUserId = user.Id;
            }

            var data = imagesService.GetByFilter<ListImageViewModel>(
                localSearchData, pageSize, pageIndex);

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((pageIndex - 1) * pageSize) + indexOfPage;
                indexOfPage++;
            }

            if (!data.Any())
            {
                return Json(string.Empty);
            }
            else
            {
                return PartialView("_ImageListPartial", data);
            }
        }

        public async Task<IActionResult> PreviewImage(int id)
        {
            if (!Request.Cookies.TryGetValue("searchData", out string readSearchDataCookie))
            {
                return BadRequest();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            SeachCookieData cookieSearchData = JsonSerializer.Deserialize<SeachCookieData>(readSearchDataCookie, options);

            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = cookieSearchData.SearchText,
                FilterCategory = cookieSearchData.SelectCategory,
            };

            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await userManager.GetUserAsync(User);
                localSearchData.LikeForUserId = user.Id;
            }

            var data = imagesService.GetByFilter<ImagePreviewViewModel>(
                    localSearchData, 1, id);

            if (!data.Any())
            {
                return Json(string.Empty);
            }
            else
            {
                ImagePreviewViewModel previewImage = data.First();
                previewImage.ImageIndex = id;

                return PartialView("_PreviewImagePartial", previewImage);
            }
        }

        /// <summary>
        /// Return images which related by user(upoload/vote).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUserImagesData(
           int pageIndex,
           int pageSize,
           string userId,
           string type)
        {
            if (pageSize == 0
               || pageIndex == 0
               || string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            SearchImageData localSearchData;
            switch (type)
            {
                case "uploads":
                    {
                        localSearchData = new SearchImageData
                        {
                            AuthorId = user.Id,
                        };
                        break;
                    }

                case "likes":
                    {
                        localSearchData = new SearchImageData
                        {
                            LikeByUser = user.Id,
                        };
                        break;
                    }

                default:
                    {
                        return BadRequest();
                    }
            }

            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await userManager.GetUserAsync(User);
                localSearchData.LikeForUserId = loginUser.Id;
            }

            var data = imagesService.GetByFilter<ListImageViewModel>(
                    localSearchData, pageSize, pageIndex);

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((pageIndex - 1) * pageSize) + indexOfPage;
                indexOfPage++;
            }

            if (!data.Any())
            {
                return Json(string.Empty);
            }
            else
            {
                return PartialView("_ImageListPartial", data);
            }
        }

        /// <summary>
        /// Return image which related by user(upload/vote).
        /// </summary>
        public async Task<IActionResult> PreviewUserImage(int id)
        {
            if (!Request.Cookies.TryGetValue("imageRelateByUserData", out string readPagingDataCookie))
            {
                return BadRequest();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            ImagePagingCookieData cookieSearchData = JsonSerializer.Deserialize<ImagePagingCookieData>(readPagingDataCookie, options);

            ApplicationUser userPreviewProfil = await userManager.FindByIdAsync(cookieSearchData.UserId);
            if (userPreviewProfil == null)
            {
                return BadRequest();
            }

            SearchImageData localSearchData = null;
            if (cookieSearchData.Type == "uploads")
            {
                localSearchData = new SearchImageData
                {
                    AuthorId = userPreviewProfil.Id,
                };
            }
            else if (cookieSearchData.Type == "likes")
            {
                localSearchData = new SearchImageData
                {
                    LikeByUser = userPreviewProfil.Id,
                };
            }

            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await userManager.GetUserAsync(User);
                localSearchData.LikeForUserId = loginUser.Id;
            }

            var data = imagesService.GetByFilter<ImagePreviewViewModel>(
                  localSearchData, 1, id);

            if (!data.Any())
            {
                return Json(string.Empty);
            }
            else
            {
                ImagePreviewViewModel previewImage = data.First();
                previewImage.ImageIndex = id;

                return PartialView("_PreviewImagePartial", previewImage);
            }
        }

        /// <summary>
        /// Download image.
        /// </summary>
        /// <param name="id">ImageId.</param>
        /// <returns>File content that must save to disc.</returns>
        [HttpPost("DownloadImage")]
        public async Task<IActionResult> DownloadImage(string imageId)
        {
            if (string.IsNullOrEmpty(imageId))
            {
                return BadRequest();
            }

            Data.Models.Image imageInfo = imagesService.GetImageById<Data.Models.Image>(imageId);
            if (imageInfo == null)
            {
                return BadRequest();
            }

            using (System.Drawing.Image sourceImage = await GetImageFromUrl(imageInfo.ImageUrl))
            {
                if (sourceImage != null)
                {
                    try
                    {
                        Stream outputStream = new MemoryStream();

                        sourceImage.Save(outputStream, sourceImage.RawFormat);
                        outputStream.Seek(0, SeekOrigin.Begin);
                        return File(outputStream, System.Net.Mime.MediaTypeNames.Image.Jpeg, Path.GetFileName(imageInfo.ImageUrl));
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error when send file from url:{imageInfo.ImageUrl}");
                    }
                }
            }

            return NotFound();
        }

        private async Task<System.Drawing.Image> GetImageFromUrl(string url)
        {
            System.Drawing.Image image = null;

            try
            {
                using HttpClient httpClient = new HttpClient();
                using HttpResponseMessage response = await httpClient.GetAsync(url);
                using Stream inputStream = await response.Content.ReadAsStreamAsync();
                using Bitmap temp = new Bitmap(inputStream);
                image = new Bitmap(temp);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when get image by url:{url}");
            }

            return image;
        }
    }
}
