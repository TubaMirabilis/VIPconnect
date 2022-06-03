using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using ProjectX.Models;

namespace ProjectX.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                category.DateCreated = DateTime.UtcNow;
                //Using the repository Insert method:
                _unitOfWork.CategoryRepository.Insert(category);
                await _unitOfWork.SaveChangesAsync();
                //Show the creator the support ticket that they have made:
                return RedirectToAction("Index", "Home");
            }
            //Show the create form until the user completes all of the required fields:
            return View();
        }
    }
}