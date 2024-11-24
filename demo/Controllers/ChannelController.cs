using demo.DTOs;
using demo.EF;
using demo.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo.Controllers
{
    [Logged]
    public class ChannelController : Controller
    {
        private demoEntities db = new demoEntities();

        // Convert ChannelDTO to Channel Entity
        public static Channel Convert(ChannelDTO c)
        {
            return new Channel()
            {
                ChannelId = c.ChannelId,
                ChannelName = c.ChannelName,
                EstablishedYear = c.EstablishedYear,
                Country = c.Country
            };
        }

        // Convert Channel Entity to ChannelDTO
        public static ChannelDTO Convert(Channel c)
        {
            return new ChannelDTO()
            {
                ChannelId = c.ChannelId,
                ChannelName = c.ChannelName,
                EstablishedYear = c.EstablishedYear,
                Country = c.Country
            };
        }

        // Convert List of Channels to List of ChannelDTO
        public static List<ChannelDTO> Convert(List<Channel> data)
        {
            var list = new List<ChannelDTO>();
            foreach (var c in data)
            {
                list.Add(Convert(c));
            }
            return list;
        }

        // GET: Add a new Channel
        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Create()
        {
            
            // Initialize a new ChannelDTO object and pass it to the view
            return View(new ChannelDTO());
        }

        // POST: Add a new Channel
        [HttpPost]
        public ActionResult Create(ChannelDTO channel)
        {
            if (ModelState.IsValid)
            {
                // Check if the channel already exists
                var channelExists = db.Channels.Any(c => c.ChannelName == channel.ChannelName);
                if (channelExists)
                {
                    TempData["ErrorMessage"] = "A channel with the entered name already exists.";
                    return View(channel); // Return the model to keep user input
                }

                // Create a new Channel entity from the DTO
                var newChannel = new Channel()
                {
                    ChannelName = channel.ChannelName,
                    EstablishedYear = channel.EstablishedYear,
                    Country = channel.Country
                };

                // Add the new channel to the database
                db.Channels.Add(newChannel);
                db.SaveChanges();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Channel added successfully.";
                
                return RedirectToAction("List"); // Redirect to a list view after successful creation
            }

            return View(channel); // Return the model to keep user input in case of validation errors
        }


        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Edit(int id)
        {
            var channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }

            var channelDTO = new ChannelDTO
            {
                ChannelId = channel.ChannelId,
                ChannelName = channel.ChannelName,
                EstablishedYear = channel.EstablishedYear,
                Country = channel.Country
            };

            return View(channelDTO);
        }

        [HttpPost]
        public ActionResult Edit(ChannelDTO channelDTO)
        {
            if (ModelState.IsValid)
            {
                var channel = db.Channels.Find(channelDTO.ChannelId);
                if (channel == null)
                {
                    return HttpNotFound();
                }

                db.Entry(channel).CurrentValues.SetValues(channelDTO);
                db.SaveChanges();

                TempData["SuccessEditMessage"] = "Channel edited successfully.";
                return RedirectToAction("List");
            }
            return View(channelDTO);
        }

        [HttpGet]
        [Logged(Role1 = "Admin,Editor")]
        public ActionResult Delete(int id)
        {
            var channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }

            var channelDTO = new ChannelDTO
            {
                ChannelId = channel.ChannelId,
                ChannelName = channel.ChannelName,
                EstablishedYear = channel.EstablishedYear,
                Country = channel.Country
            };

            return View(channelDTO);
        }
        [HttpPost]
        public ActionResult Delete(int id, string dcsn)
        {
            if (!string.IsNullOrEmpty(dcsn) && dcsn.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                var channel = db.Channels.Find(id);
                if (channel != null)
                {
                    // Check if there are associated programs
                    if (db.Programs.Any(p => p.ChannelId == id))
                    {
                        // Add an error message

                        TempData["ErrorMessage"] = "Cannot delete the channel because it has associated programs.";
                        return RedirectToAction("List");
                    }

                    // Proceed with deletion if no associated programs
                    db.Channels.Remove(channel);
                    db.SaveChanges();
                }
            }
            TempData["SuccessDeleteMessage"] = "Channel deleted successfully.";

            return RedirectToAction("List");
        }


        public ActionResult List(string searchTerm = null, int page = 1)
        {
            int pageSize = 3; // Set the number of items per page

            var channels = string.IsNullOrEmpty(searchTerm)
                ? db.Channels.ToList()
                : (from c in db.Channels
                   where c.ChannelName.Contains(searchTerm)
                   select c).ToList();

            // Apply pagination logic
            var totalChannels = channels.Count(); // Get the total count of channels
            var totalPages = (int)Math.Ceiling((double)totalChannels / pageSize); // Calculate the total number of pages

            var currentPageChannels = channels
                .Skip((page - 1) * pageSize) // Skip the channels from the previous pages
                .Take(pageSize) // Take only the records for the current page
                .ToList();

            // If no channels match and any filter is applied, set an error message
            if (!string.IsNullOrEmpty(searchTerm) && !currentPageChannels.Any())
            {
                TempData["ErrorMessage"] = $"No channel exists with the name '{searchTerm}'.";
            }

            // Pass the current page, total pages, and search term to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;

            return View(currentPageChannels);
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            var channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }

            var programs = (from p in db.Programs
                            where p.ChannelId == id // Filter programs by the ChannelId
                            select p).ToList();


            var channelDTO = new ChannelDTO
            {
                ChannelId = channel.ChannelId,
                ChannelName = channel.ChannelName,
                EstablishedYear = channel.EstablishedYear,
                Country = channel.Country,
                Programs = programs // Add the list of programs to the DTO
            };

            return View(channelDTO);
        }

    }
}