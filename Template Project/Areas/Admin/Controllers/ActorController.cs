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
<<<<<<< HEAD
    [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
    public class ActorController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Actor> _actorRepository;//= new Repository<Actor>();

        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

<<<<<<< HEAD
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
        public async Task<ViewResult> Index(CancellationToken cancellationToken)
        {
            //var actors = _context.Actors.AsQueryable();
            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);
            return View(actors.AsEnumerable());
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> Create(CreateCategoryVM createActorVM, CancellationToken cancellationToken)
        {
            //ModelState.Remove(nameof(FormImg));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Inputs";
                return View(createActorVM);
            }

            //var actor = new Actor()
            //{
            //    Name = createActorVM.Name,
            //    Description = createActorVM.Description,
            //    Status = createActorVM.Status,
            //};

            var actor = createActorVM.Adapt<Actor>();

            if (createActorVM.FormImg is not null)
            {
                if (createActorVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + createActorVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Actors\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        createActorVM.FormImg.CopyTo(stream);
                    }
                    actor.Image = filename;
                }
            }

            //_context.Actors.Add(actor);
            //_context.SaveChanges();
            await _actorRepository.AddAsync(actor, cancellationToken);
            await _actorRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Actor Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
        {
            //var actor = _context.Actors.FirstOrDefault(c=>c.Id==id);
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (actor is null)
                return RedirectToAction("NotFoundPage", "Home");

            //var updateActorVM = new UpdateActorVM()
            //{
            //    Id = actor.Id,
            //    Name=actor.Name,
            //    Img = actor.Img,
            //    Description=actor.Description,
            //    Status=actor.Status,
            //};

            var updateActorVM = actor.Adapt<UpdateActorVM>();

            return View(updateActorVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(UpdateActorVM updateActorVM, CancellationToken cancellationToken)
        {
            //var currentActor = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == updateActorVM.Id);
            var currentActor = await _actorRepository.GetOneAsync(b => b.Id == updateActorVM.Id, cancellationToken: cancellationToken, tracked: false);

            if (!ModelState.IsValid)
            {
                updateActorVM.Image = currentActor.Image;
                return View(updateActorVM);
            }

            //var actor = new Actor()
            //{
            //    Id = updateActorVM.Id,
            //    Name = updateActorVM.Name,
            //    Description = updateActorVM.Description,
            //    Status = updateActorVM.Status,
            //};

            var actor = updateActorVM.Adapt<Actor>();

            if (updateActorVM.FormImg is not null)
            {
                if (updateActorVM.FormImg.Length > 0)
                {
                    //var filename = Guid.NewGuid().ToString()+Path.GetExtension(img.FileName);
                    var filename = Guid.NewGuid().ToString() + "-" + updateActorVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Actors\\", filename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateActorVM.FormImg.CopyTo(stream);
                    }
                    actor.Image = filename;

                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Actors\\", currentActor.Image);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                actor.Image = currentActor.Image;
            }

            //_context.Actors.Update(actor);
            //_context.SaveChanges();
            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (actor is null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\Actors\\", actor.Image);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            //_context.Actors.Remove(actor);
            //_context.SaveChanges();
            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
