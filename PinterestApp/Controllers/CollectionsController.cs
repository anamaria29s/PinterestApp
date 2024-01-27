using PinterestApp.Data;
using PinterestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PinterestApp.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CollectionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        // HttpGet - implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var collections = from collection in db.Collections.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                                  select collection;

                ViewBag.Collections = collections;

                return View();
            }
            else
            if (User.IsInRole("Admin"))
            {
                var collections = from collection in db.Collections.Include("User")
                                  select collection;

                ViewBag.Collections = collections;

                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi asupra colectiei";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }

        }
 
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var collections = db.Collections
                                  .Include("BookmarkCollections.Bookmark.Category")
                                  .Include("BookmarkCollections.Bookmark.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();

                if (collections == null)
                {
                    TempData["message"] = "Nu ai drepturi";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Bookmarks");
                }

                return View(collections);
            }

            else
            if (User.IsInRole("Admin"))
            {
                var collections = db.Collections
                                  .Include("BookmarkCollections.Bookmark.Category")
                                  .Include("BookmarkCollections.Bookmark.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();


                if (collections == null)
                {
                    TempData["message"] = "Resursa cautata nu poate fi gasita";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Bookmarks");
                }


                return View(collections);
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Collection cl)
        {
            cl.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Collections.Add(cl);
                db.SaveChanges();
                TempData["message"] = "Colectia a fost adaugata";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                return View(cl);
            }
        }
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(int id)
        {
            Collection collection = db.Collections.Find(id);

            if (collection == null)
            {
                TempData["message"] = "Colectia nu poate fi gasita";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            db.Collections.Remove(collection);
            TempData["message"] = "Colectia a fost stearsa";
            TempData["messageType"] = "alert-success";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }



        


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteFromCollection(int collectionId, int bookmarkId)
        {
            Collection collection = db.Collections
                .Include(c => c.BookmarkCollections)
                .FirstOrDefault(c => c.Id == collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            BookmarkCollection bookmarkCollectionToRemove = collection.BookmarkCollections
                .FirstOrDefault(bc => bc.BookmarkId == bookmarkId);

            if (bookmarkCollectionToRemove != null)
            {
                // Verificați dacă utilizatorul curent are permisiunea să șteargă această asociere
                if (User.IsInRole("Admin") || collection.UserId == _userManager.GetUserId(User))
                {
                    collection.BookmarkCollections.Remove(bookmarkCollectionToRemove);
                    db.SaveChanges();

                    // Actualizați și colecția de BookmarkCollections din entitatea Bookmark
                    Bookmark bookmark = db.Bookmarks
                        .Include(b => b.BookmarkCollections)
                        .FirstOrDefault(b => b.Id == bookmarkId);

                    if (bookmark != null)
                    {
                        bookmark.BookmarkCollections.Remove(bookmarkCollectionToRemove);
                        db.SaveChanges();
                    }

                    TempData["message"] = "Bookmarkul a fost eliminat din colecție";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Show", new { id = collectionId });
                }
                else
                {
                    TempData["message"] = "Nu aveți dreptul să eliminați bookmarkul din colecție";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", new { id = collectionId });
                }
            }

            TempData["message"] = "Bookmarkul nu se află în colecție";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Show", new { id = collectionId });
        }

    }
}