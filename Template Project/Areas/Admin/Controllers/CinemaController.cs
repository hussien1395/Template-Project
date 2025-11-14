using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Template_Project.Models;
using Template_Project.Repos;
using Template_Project.ViewModel;

namespace Template_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Cinema> _cinemaRepository;// = new Repository<Cinema>();

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<ViewResult> Index(CancellationToken cancellationToken)
        {
            //var cinemas = _context.Cinemas.AsQueryable();
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            return View(cinemas.AsEnumerable());
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCinemaVM createCinemaVM, CancellationToken cancellationToken)
        {
            //ModelState.Remove(nameof(FormImg));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Inputs";
                return View(createCinemaVM);
            }

            //var cinema = new Cinema()
            //{
            //    Name = createCinemaVM.Name,
            //    Description = createCinemaVM.Description,
            //    Status = createCinemaVM.Status,
            //};

            var cinema = createCinemaVM.Adapt<Cinema>();

            if (createCinemaVM.FormImg is not null)
            {
                if (createCinemaVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + createCinemaVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Cinemas\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        createCinemaVM.FormImg.CopyTo(stream);
                    }
                    cinema.Image = filename;
                }
            }

            //_context.Cinemas.Add(cinema);
            //_context.SaveChanges();
            await _cinemaRepository.AddAsync(cinema, cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Cinema Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
        {
            //var cinema = _context.Cinemas.FirstOrDefault(c=>c.Id==id);
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            //var updateCinemaVM = new UpdateCinemaVM()
            //{
            //    Id = cinema.Id,
            //    Name=cinema.Name,
            //    Img = cinema.Img,
            //    Description=cinema.Description,
            //    Status=cinema.Status,
            //};

            var updateCinemaVM = cinema.Adapt<UpdateCinemaVM>();

            return View(updateCinemaVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCinemaVM updateCinemaVM, CancellationToken cancellationToken)
        {
            //var currentCinema = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == updateCinemaVM.Id);
            var currentCinema = await _cinemaRepository.GetOneAsync(b => b.Id == updateCinemaVM.Id, cancellationToken: cancellationToken, tracked: false);

            if (!ModelState.IsValid)
            {
                updateCinemaVM.Image = currentCinema.Image;
                return View(updateCinemaVM);
            }

            //var cinema = new Cinema()
            //{
            //    Id = updateCinemaVM.Id,
            //    Name = updateCinemaVM.Name,
            //    Description = updateCinemaVM.Description,
            //    Status = updateCinemaVM.Status,
            //};

            var cinema = updateCinemaVM.Adapt<Cinema>();

            if (updateCinemaVM.FormImg is not null)
            {
                if (updateCinemaVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + updateCinemaVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Cinemas\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateCinemaVM.FormImg.CopyTo(stream);
                    }
                    cinema.Image = filename;

                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Cinemas\\", currentCinema.Image);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                cinema.Image = currentCinema.Image;
            }

            //_context.Cinemas.Update(cinema);
            //_context.SaveChanges();
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Cinemas\\", cinema.Image);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            //_context.Cinemas.Remove(cinema);
            //_context.SaveChanges();
            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
