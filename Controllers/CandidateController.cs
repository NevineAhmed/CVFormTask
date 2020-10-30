using CVSubTask.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;

namespace CVSubTask.Controllers
{
    public class CandidateController : Controller
    {
         CVFormTaskEntities _context=new CVFormTaskEntities();

        public bool isFileSupported(HttpPostedFileBase file)
        {
            if (file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                || file.ContentType == "application/pdf")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isImageSupported(HttpPostedFileBase file)
        {
            System.Drawing.Image sourceimage =
            System.Drawing.Image.FromStream(file.InputStream);

            var x = sourceimage.Width;
            var y = sourceimage.Height;

            if ( x <=256 && y<=256)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        // GET: Candidate
        public ActionResult Index()
        {

            var candidates = _context.Candidates.ToList();
            return View(candidates);

        }
        [HttpGet]
        public ActionResult SubmitCV()
        {
            return View();
        }
     
       
        
        [HttpPost]
        public ActionResult SubmitCV(Candidate model,HttpPostedFileBase cv , HttpPostedFileBase img ,string Genders, DateTime DateOBirth)
        {
            
            if (cv == null )
            {
                ModelState.AddModelError("FileError", "No File uploaded");
                return View();
            }

            if(img == null)
            {
                ModelState.AddModelError("Image", "No Image uploaded");
                return View();

            }

            if (!(isFileSupported(cv)))
            {
                ModelState.AddModelError("FileError", "Unsupported format, only WORD and PDF are supported");
                return View();
            }

            if (cv.ContentLength * 1048576 > 1)                         // larger than 1024*1024
            {
                ModelState.AddModelError("FileError", "file too large, exceeded 1 MB");
                
                return View();
            }

            if(DateOBirth == null)
            {
                ModelState.AddModelError("DateError", "Enter your date of birth");
                return View();
            }



            /////////////////////////////////////////////////////////////////////////////////
            if (!(isImageSupported(img)))
            {
                ModelState.AddModelError("ImageError", "Not an image or not a 256 X 256 pixel image ");
            }


            string ImageName = Guid.NewGuid() + Path.GetExtension(img.FileName);
            string fullPath = Path.GetFullPath(ImageName);
            

            if (ModelState.IsValid)
            {
                try
                {
                    string FileName = Guid.NewGuid() + Path.GetExtension(cv.FileName);
                    cv.SaveAs(Path.Combine(Server.MapPath("~/Content/Uploads"), FileName));
                    img.SaveAs(Path.Combine(Server.MapPath("~/Content/Uploads/imgs"), ImageName));

                    var today = DateTime.Today;
                    var age = today.Year - DateOBirth.Year;

                    model.CV = FileName;
                    model.Image = ImageName;
                    model.Gender = Genders;
                    model.Age = age;

                   
                   
                    _context.Candidates.Add(model);
                    _context.SaveChanges();

                    ModelState.Clear();
                    model = null;
                    ViewBag.msg = "Added!";
                }
                catch (Exception e)
                {
                    ViewBag.msg = "Error Occurred ,try again";
                    return View();
                }

            }
            return View();
        }

        
        public  ActionResult Delete(int? id)
        {
            var can=_context.Candidates.FirstOrDefault(c => c.Id == id);
            if(can != null)
            {
                _context.Candidates.Remove(can);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                return HttpNotFound();
            }
            
        }

        public ActionResult Details(int? id)
        {
            var can = _context.Candidates.FirstOrDefault(c => c.Id == id);
            if (can != null)
            {

                return View(can);
            }

            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult Edit(int? id)
        {
            var can = _context.Candidates.FirstOrDefault(c => c.Id == id);
            if (can != null)
            {

                return View(can);
            }

            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(Candidate model, HttpPostedFileBase cv, HttpPostedFileBase img, string Genders, DateTime? DateOBirth)
        {
            if (DateOBirth == null)
            {
                ModelState.AddModelError("DateError", "Enter your date of birth");
                return View();
            }


            if (cv == null)
            {
                ModelState.AddModelError("FileError", "No File uploaded");
                return View();
            }

            if (img == null)
            {
                ModelState.AddModelError("Image", "No Image uploaded");
                return View();

            }

            if (!(isFileSupported(cv)))
            {
                ModelState.AddModelError("FileError", "Unsupported format, only WORD and PDF are supported");
                return View();
            }

            if (cv.ContentLength * 1048576 > 1)                         // larger than 1024*1024
            {
                ModelState.AddModelError("FileError", "file too large, exceeded 1 MB");

                return View();
            }



            /////////////////////////////////////////////////////////////////////////////////
            if (!(isImageSupported(img)))
            {
                ModelState.AddModelError("ImageError", "Not an image or not a 256 X 256 pixel image ");
            }


            string ImageName = Guid.NewGuid() + Path.GetExtension(img.FileName);
            string fullPath = Path.GetFullPath(ImageName);


            if (ModelState.IsValid)
            {
                try
                {
                    string FileName = Guid.NewGuid() + Path.GetExtension(cv.FileName);
                    cv.SaveAs(Path.Combine(Server.MapPath("~/Content/Uploads"), FileName));
                    img.SaveAs(Path.Combine(Server.MapPath("~/Content/Uploads/imgs"), ImageName));

                    var today = DateTime.Today;
                    var age = today.Year - DateOBirth.Value.Year;

                    model.CV = FileName;
                    model.Image = ImageName;
                    model.Gender = Genders;
                    model.Age = age;



                  var candidate=  _context.Candidates.FirstOrDefault(a => a.Id == model.Id);
                    candidate.FullName = model.FullName;
                    candidate.Age = model.Age;
                    candidate.Gender = model.Gender;
                    candidate.City = model.City;
                    candidate.Area = model.Area;
                    candidate.Address = model.Address;
                    candidate.CV = model.CV;
                    candidate.Image = model.Image;
                    _context.SaveChanges();

                    ModelState.Clear();
                    model = null;
                    ViewBag.msg = "updated!";
                }
                catch (Exception e)
                {
                    ViewBag.msg = "Error Occurred ,try again";
                    return View();
                }

            }
            return View();
        }
    }
}