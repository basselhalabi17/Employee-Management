using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers
{
    // [Route("Home")]
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IEmployeeRepository _employeerep;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment)
        {
            _employeerep = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        //[Route("")]
        //[Route("Home")]
        // [Route("Index")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeerep.GetAllEmployee();
            return View(model);
        }
        [AllowAnonymous]

        //[Route("Details/{id?}")]
        public ViewResult Details(int? id)
        {
            //throw new Exception("Error in Details View");

            Employee employee = _employeerep.GetEmployee(id.Value);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }
            HomeDetailsViewModel x = new HomeDetailsViewModel()
            {

                Employee = _employeerep.GetEmployee(id ?? 1),
                PageTitle = "Employee Details"
            };


            //Employee model = _employeerep.GetEmployee(1);
            //ViewBag.Employee = model;
            //ViewBag.PageTitle = "Employee Details";
            //ViewData["Employee"] = model;
            //ViewData["PageTitle"] = "Employee Details";
            return View(x); ;
        }
        [HttpGet]
        
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
       
            public IActionResult Create(EmployeeCreateViewModel model)
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = ProcessUpload(model);

               

                    Employee newEmployee = new Employee
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Department = model.Department,
                        // Store the file name in PhotoPath property of the employee object
                        // which gets saved to the Employees database table
                        photopath = uniqueFileName
                    };

                    _employeerep.add(newEmployee);
                    return RedirectToAction("details", new { id = newEmployee.Id });
                }

                return View();
            }

        [HttpPost]
        
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee e = _employeerep.GetEmployee(model.Id);
                e.Name = model.Name;
                e.Email = model.Email;
                e.Department = model.Department;
                if (model.photo != null)
                {
                    if (model.existingpath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                    "images", model.existingpath);
                        System.IO.File.Delete(filePath);
                    }

                   e.photopath = ProcessUpload(model);
                }
            

                _employeerep.Update(e);
                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUpload(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;

            // If the Photo property on the incoming model object is not null, then the user
            // has selected an image to upload.
            if (model.photo != null & model.photo.Count > 0)
            {
                foreach (IFormFile p in model.photo)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + p.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        p.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }

        [HttpGet]
  
        public ViewResult Edit(int id)
        {
            Employee e = _employeerep.GetEmployee(id);
            EmployeeEditViewModel empedit = new EmployeeEditViewModel
            {
                Id = e.Id,
                existingpath = e.photopath,
                Name = e.Name,
                Email = e.Email,
                Department=e.Department

            };
            return View(empedit);
        }
    }
    }
