using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using demo.EF;
using demo.Auth;
using demo.DTOs;
using System.Data.Entity;
using demo.Models;

namespace demo.Controllers
{
    [Logged]
    public class ProgramController : Controller
    {
        private demoEntities db = new demoEntities();

        // Convert ProgramDTO to Program Entity
        public static Program Convert(ProgramDTO p)
        {
            return new Program()
            {
                ProgramId = p.ProgramId,
                ProgramName = p.ProgramName,
                TRPScore = p.TRPScore,
                ChannelId = p.ChannelId,
                AirTime = p.AirTime
            };
        }
        // Convert Program Entity to ProgramDTO
        public static ProgramDTO Convert(Program p)
        {
            return new ProgramDTO()
            {
                ProgramId = p.ProgramId,
                ProgramName = p.ProgramName,
                TRPScore = p.TRPScore,
                ChannelId = p.ChannelId,
                ChannelName = p.Channel?.ChannelName, // Fetch Channel Name
                AirTime = p.AirTime
            };
        }

        // Convert List of Programs to List of ProgramDTO
        public static List<ProgramDTO> Convert(List<Program> data)
        {
            var list = new List<ProgramDTO>();
            foreach (var p in data)
            {
                list.Add(Convert(p));
            }
            return list;
        }


        // GET: Add a new Program
        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Create()
        {
            return View(new ProgramDTO());
        }
[HttpPost]
public ActionResult Create(ProgramDTO program)
{
    if (ModelState.IsValid)
    {
        var channelExists = db.Channels.Any(c => c.ChannelName == program.ChannelName);
        if (!channelExists)
        {
            TempData["ErrorMessage"] = "No channel with the entered name exists.";
            return View(program); // Return the model to keep user input
        }

        var newProgram = new Program()
        {
            ProgramName = program.ProgramName,
            TRPScore = program.TRPScore,
            ChannelId = db.Channels.First(c => c.ChannelName == program.ChannelName).ChannelId,
            AirTime = program.AirTime
        };

        db.Programs.Add(newProgram);
        db.SaveChanges();
                TempData["SuccessMessage"] = "Program added successfully.";

                return RedirectToAction("List");
    }

    return View(program); // Return the model to keep user input in case of validation errors
}


        // GET: View Program Details
        public ActionResult Details(int id)
        {
            var data = db.Programs.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }

            var channel = db.Channels.Find(data.ChannelId); // Fetch associated channel
            ViewBag.ChannelName = channel?.ChannelName;

            return View(Convert(data));
        }

        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Edit(int id)
        {
            var program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }

            // Map the program entity to ProgramDTO
            var programDTO = new ProgramDTO
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName,
                TRPScore = program.TRPScore,
                ChannelName = db.Channels.FirstOrDefault(c => c.ChannelId == program.ChannelId)?.ChannelName,
                AirTime = program.AirTime
            };

            return View(programDTO);
        }

        [HttpPost]
        public ActionResult Edit(ProgramDTO programDTO)
        {
            if (ModelState.IsValid)
            {
                var program = db.Programs.Find(programDTO.ProgramId);
                if (program == null)
                {
                    return HttpNotFound();
                }

                // Check if the channel exists
                var channel = db.Channels.FirstOrDefault(c => c.ChannelName == programDTO.ChannelName);
                if (channel == null)
                {
                    TempData["ErrorMessage"] = "No channel with the entered name exists.";
                    return View(programDTO); // Return model to keep user input
                }

                // Update the program entity
                program.ProgramName = programDTO.ProgramName;
                program.TRPScore = programDTO.TRPScore;
                program.ChannelId = channel.ChannelId; // Update ChannelId using the new ChannelName
                program.AirTime = programDTO.AirTime;

                db.SaveChanges();
                TempData["SuccessEditMessage"] = "Program edited successfully.";
                return RedirectToAction("List");
            }

            return View(programDTO); // Return model to keep user input in case of validation errors
        }

        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Delete(int id)
        {
            var program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }

            // Map the program entity to ProgramDTO
            var programDTO = new ProgramDTO
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName,
                TRPScore = program.TRPScore,
                ChannelName = db.Channels.FirstOrDefault(c => c.ChannelId == program.ChannelId)?.ChannelName,
                AirTime = program.AirTime
            };

            return View(programDTO);
        }

        [HttpPost]
        public ActionResult Delete(int id, string dcsn)
        {
            if (!string.IsNullOrEmpty(dcsn) && dcsn.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                var program = db.Programs.Find(id);
                if (program != null)
                {
                    db.Programs.Remove(program);
                    db.SaveChanges();
                }
            }
            TempData["SuccessDeleteMessage"] = "Program deleted successfully.";
            return RedirectToAction("List");
        }

        public ActionResult List(string searchTerm = null, string sortOrder = null, string minTRP = null, string maxTRP = null, int page = 1)
        {
            int pageSize = 3; // Set the number of items per page

            var programs = string.IsNullOrEmpty(searchTerm)
                ? db.Programs.ToList()
                : (from p in db.Programs
                   where p.ProgramName.Contains(searchTerm) ||
                         p.TRPScore.ToString().Contains(searchTerm) ||
                         p.Channel.ChannelName.Contains(searchTerm)
                   select p).ToList();

            // Apply TRP range filtering if minTRP or maxTRP is provided
            if (!string.IsNullOrEmpty(minTRP) && decimal.TryParse(minTRP, out var min))
            {
                programs = programs.Where(p => p.TRPScore >= min).ToList();
                ViewBag.MinTRP = minTRP; // Keep input populated
            }

            if (!string.IsNullOrEmpty(maxTRP) && decimal.TryParse(maxTRP, out var max))
            {
                programs = programs.Where(p => p.TRPScore <= max).ToList();
                ViewBag.MaxTRP = maxTRP; // Keep input populated
            }

            // Apply sorting based on the selected sortOrder
            if (sortOrder == "ascending")
            {
                programs = programs.OrderBy(p => p.TRPScore).ToList();  // Ascending order
            }
            else if (sortOrder == "descending")
            {
                programs = programs.OrderByDescending(p => p.TRPScore).ToList();  // Descending order
            }

            // Apply pagination logic
            var totalPrograms = programs.Count(); // Get the total count of programs
            var totalPages = (int)Math.Ceiling((double)totalPrograms / pageSize); // Calculate the total number of pages

            var currentPagePrograms = programs
                .Skip((page - 1) * pageSize) // Skip the programs from the previous pages
                .Take(pageSize) // Take only the records for the current page
                .ToList();

            // If no programs match and any filter is applied, set an error message
            if ((!string.IsNullOrEmpty(searchTerm) || !string.IsNullOrEmpty(minTRP) || !string.IsNullOrEmpty(maxTRP)) && !currentPagePrograms.Any())
            {
                TempData["ErrorMessage"] = "No programs match the specified criteria.";
            }

            // Pass the current page, total pages, search term, and other filters to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;

            return View(currentPagePrograms);
        }



        public ActionResult ListGroupedByChannel()
        {
            // Group programs by Channel and map them to the GroupedProgramsViewModel
            var groupedPrograms = db.Programs
                .GroupBy(p => p.Channel)
                .Select(g => new GroupedProgramsViewModel
                {
                    Channel = g.Key,
                    Programs = g.ToList()
                })
                .ToList();

            return View(groupedPrograms);
        }







    }
}
