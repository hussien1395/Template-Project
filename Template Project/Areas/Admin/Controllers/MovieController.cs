using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using Template_Project.Utilities;

namespace Template_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        //ProductRepository _productRepository = new ProductRepository();
        private readonly IRepository<Category> _categoryRepository;// = new Repository<Category>();
        private readonly IRepository<Cinema> _cinemaRepository;// = new Repository<Cinema>();
        private readonly IRepository<Actor> _actorRepository;// = new Repository<Actor>();
        private readonly IRepository<Movie> _movieRepository;//= new Repository<Movie>();
        private readonly IRepository<MovieActor> _movieActorRepository;// = new Repository<MovieActor>();
        private readonly IRepository<MovieSubImage> _movieSubImageRepository;// = new Repository<MovieSubImage>();

        public MovieController(IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, IRepository<Movie> movieRepository, 
            IRepository<MovieActor> movieActorRepository, IRepository<MovieSubImage> movieSubImageRepository)
        {
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
            _movieActorRepository = movieActorRepository;
            _movieSubImageRepository = movieSubImageRepository;
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<ViewResult> Index(CancellationToken cancellationToken)
        {
            //var movies = _context.Products.Include(p=>p.Category).Include(p=>p.Brand).AsQueryable();
            var movies = await _movieRepository.GetAsync(
                includes: [p => p.Category, p => p.Cinema],
                cancellationToken: cancellationToken);
            return View(movies.AsEnumerable());
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var vm = new MoviesVM
            {
                Movie = new Movie(),
                Categories = (await _categoryRepository.GetAsync(cancellationToken:cancellationToken))
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Cinemas = (await _cinemaRepository.GetAsync(cancellationToken:cancellationToken))
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Actors = (await _actorRepository.GetAsync(cancellationToken:cancellationToken))  
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> Create(MoviesVM model, IFormFile img, List<IFormFile> SubImages, CancellationToken cancellationToken)
        {
            var movie = model.Movie;

            // حفظ الصورة الرئيسية
            if (img is not null && img.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\", filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
                movie.MainImage = filename;
            }

            // حفظ الفيلم
            var addedMovie = await _movieRepository.AddAsync(movie);
            await _movieRepository.CommitAsync(cancellationToken);

            // حفظ الصور الفرعية
            if (SubImages != null)
            {
                foreach (var item in SubImages)
                {
                    if (item.Length > 0)
                    {
                        var filenameSubImage = Guid.NewGuid().ToString() + "-" + item.FileName;
                        var filePathSubImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\MovieSubImages", filenameSubImage);
                        using (var stream = new FileStream(filePathSubImage, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var movieSubImage = new MovieSubImage()
                        {
                            ImagePath = filenameSubImage,
                            MovieId = addedMovie.Entity.Id,
                        };

                        await _movieSubImageRepository.AddAsync(movieSubImage);
                        await _movieSubImageRepository.CommitAsync(cancellationToken);
                    }
                }
            }

            // ربط الممثلين
            if (model.SelectedActors != null && model.SelectedActors.Any())
            {
                foreach (var actorId in model.SelectedActors)
                {
                    var movieActor = new MovieActor()
                    {
                        ActorId = actorId,
                        MovieId = addedMovie.Entity.Id
                    };
                    await _movieActorRepository.AddAsync(movieActor);
                }
                await _movieActorRepository.CommitAsync(cancellationToken);
            }

            TempData["Success"] = "Movie created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            var movie = await _movieRepository.GetOneAsync(
                expression: c => c.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
            p => p.MovieActors,
            p => p.MovieSubImages
                },
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

            var vm = new MoviesVM
            {
                Movie = movie,
                Categories = (await _categoryRepository.GetAsync(cancellationToken:cancellationToken))
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Cinemas = (await _cinemaRepository.GetAsync(cancellationToken: cancellationToken))
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Actors = (await _actorRepository.GetAsync(cancellationToken: cancellationToken))
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name,
                        Selected = movie.MovieActors.Any(ma => ma.ActorId == a.Id)
                    }),
                SelectedActors = movie.MovieActors.Select(ma => ma.ActorId).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(MoviesVM model, IFormFile img, List<IFormFile> SubImages, CancellationToken cancellationToken)
        {
            var movie = model.Movie;


            var current = await _movieRepository.GetOneAsync(
                c => c.Id == movie.Id,
                includes: new[] { (Expression<Func<Movie, object>>)(m => m.MovieActors) },
                tracked: false,
                cancellationToken: cancellationToken
            );

            if (current == null)
                return NotFound();


            if (img != null && img.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\", filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\", current.MainImage);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                movie.MainImage = filename;
            }
            else
            {
                movie.MainImage = current.MainImage;
            }


            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync(cancellationToken);


            foreach (var oldActor in current.MovieActors)
            {
                _movieActorRepository.Delete(oldActor);
            }


            if (model.SelectedActors != null && model.SelectedActors.Any())
            {
                foreach (var actorId in model.SelectedActors)
                {
                    var movieActor = new MovieActor { MovieId = movie.Id, ActorId = actorId };
                    await _movieActorRepository.AddAsync(movieActor);
                }
                await _movieActorRepository.CommitAsync(cancellationToken);
            }

            TempData["Success"] = "Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
                return NotFound();

            var movie = await _movieRepository.GetOneAsync(
                expression: c => c.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
            p => p.MovieSubImages,
            p => p.MovieActors
                },
                tracked: false,
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(
                expression: c => c.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
            p => p.MovieSubImages,
            p => p.MovieActors
                },
                tracked: false,
                cancellationToken: cancellationToken
            );

            if (movie == null)
            {
                TempData["Error"] = "Movie not found!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
         
                if (!string.IsNullOrEmpty(movie.MainImage))
                {
                    var mainImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies", movie.MainImage);
                    if (System.IO.File.Exists(mainImgPath))
                        System.IO.File.Delete(mainImgPath);
                }

            
                if (movie.MovieSubImages != null && movie.MovieSubImages.Any())
                {
                    foreach (var subImg in movie.MovieSubImages)
                    {
                        var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\MovieSubImages", subImg.ImagePath);
                        if (System.IO.File.Exists(subImgPath))
                            System.IO.File.Delete(subImgPath);

                        _movieSubImageRepository.Delete(subImg);
                    }
                    await _movieSubImageRepository.CommitAsync(cancellationToken);
                }

         
                if (movie.MovieActors != null && movie.MovieActors.Any())
                {
                    foreach (var actor in movie.MovieActors)
                    {
                        _movieActorRepository.Delete(actor);
                    }
                    await _movieActorRepository.CommitAsync(cancellationToken);
                }

    
                _movieRepository.Delete(movie);
                await _movieRepository.CommitAsync(cancellationToken);

                TempData["Success"] = "Movie deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting movie: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteSubImage(int movieId, string img, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(img))
                return RedirectToAction(nameof(Update), new { id = movieId });

            var movieSubImage = await _movieSubImageRepository.GetOneAsync(
                expression: p => p.MovieId == movieId && p.ImagePath == img,
                tracked: false,
                cancellationToken: cancellationToken
            );

            if (movieSubImage == null)
            {
                TempData["Error"] = "Sub image not found!";
                return RedirectToAction(nameof(Update), new { id = movieId });
            }

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Movies\\MovieSubImages", movieSubImage.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _movieSubImageRepository.Delete(movieSubImage);
                await _movieSubImageRepository.CommitAsync(cancellationToken);

                TempData["Success"] = "Sub image deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting sub image: " + ex.Message;
            }

            return RedirectToAction(nameof(Update), new { id = movieId });
        }

    }
}
