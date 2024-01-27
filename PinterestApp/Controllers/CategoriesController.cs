using PinterestApp.Data;
using PinterestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinterestApp.Data;
using PinterestApp.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PinterestApp.Controllers
{
   
    public class CategoriesController : Controller
    {

        //useri si roluri

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = from category in db.Categories
                             orderby category.Name
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        public IActionResult BookmarksByCategory(int categoryId)
        {
            var bookmarks = db.Bookmarks.Include("Category").Include("User").Include("Likes")
                                .Where(b => b.CategoryId == categoryId)
                                .OrderByDescending(b => b.Date)
                                .ToList();

            ViewBag.Bookmarks = bookmarks;
            ViewBag.SelectedCategoryId = categoryId;  // Adaugăm și id-ul categoriei selectate

            return View("Index");  // Redirecționăm către acțiunea Index, dar cu bookmark-urile filtrate
        }


        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            //SetAccessRights();

            var category = db.Categories
                .Include(c => c.Bookmarks)
                .ThenInclude(b => b.Likes)
                .ThenInclude(b => b.User)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["message"] = "Nu aveti drepturi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }

            return View(category);
        }



        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category cat)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                return RedirectToAction("Index");
            }

            else
            {
                return View(cat);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

            if (ModelState.IsValid)
            {

                category.Name = requestCategory.Name;
                db.SaveChanges();
                TempData["message"] = "Categoria a fost modificata!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
    }
}
