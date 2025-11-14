using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Template_Project.Models;
using Template_Project.Repos;
using Template_Project.Utilities;
using Template_Project.ViewModel;

namespace Template_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Category> _categoryRepository;// = new Repository<Category>();

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<ViewResult> Index(CancellationToken cancellationToken)
        {
            //var categories = _context.Categorys.AsQueryable();
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            return View(categories.AsEnumerable());
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public ViewResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> Create(CreateCategoryVM createCategoryVM, CancellationToken cancellationToken)
        {
            //ModelState.Remove(nameof(FormImg));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Inputs";
                return View(createCategoryVM);
            }

            //var category = new Category()
            //{
            //    Name = createCategoryVM.Name,
            //    Description = createCategoryVM.Description,
            //    Status = createCategoryVM.Status,
            //};

            var category = createCategoryVM.Adapt<Category>();

            if (createCategoryVM.FormImg is not null)
            {
                if (createCategoryVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + createCategoryVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Categories\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        createCategoryVM.FormImg.CopyTo(stream);
                    }
                    category.Image = filename;
                }
            }

            //_context.Categorys.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Category Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
        {
            //var category = _context.Categorys.FirstOrDefault(c=>c.Id==id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            //var updateCategoryVM = new UpdateCategoryVM()
            //{
            //    Id = category.Id,
            //    Name=category.Name,
            //    Img = category.Img,
            //    Description=category.Description,
            //    Status=category.Status,
            //};

            var updateCategoryVM = category.Adapt<UpdateCategoryVM>();

            return View(updateCategoryVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(UpdateCategoryVM updateCategoryVM, CancellationToken cancellationToken)
        {
            //var currentCategory = _context.Categorys.AsNoTracking().FirstOrDefault(b => b.Id == updateCategoryVM.Id);
            var currentCategory = await _categoryRepository.GetOneAsync(b => b.Id == updateCategoryVM.Id, cancellationToken: cancellationToken, tracked: false);

            if (!ModelState.IsValid)
            {
                updateCategoryVM.Image = currentCategory.Image;
                return View(updateCategoryVM);
            }

            //var category = new Category()
            //{
            //    Id = updateCategoryVM.Id,
            //    Name = updateCategoryVM.Name,
            //    Description = updateCategoryVM.Description,
            //    Status = updateCategoryVM.Status,
            //};

            var category = updateCategoryVM.Adapt<Category>();

            if (updateCategoryVM.FormImg is not null)
            {
                if (updateCategoryVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + updateCategoryVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Categories\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateCategoryVM.FormImg.CopyTo(stream);
                    }
                    category.Image = filename;

                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Categories\\", currentCategory.Image);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                category.Image = currentCategory.Image;
            }

            //_context.Categorys.Update(category);
            //_context.SaveChanges();
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var category = _context.Categorys.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Categories\\", category.Image);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            //_context.Categorys.Remove(category);
            //_context.SaveChanges();
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
