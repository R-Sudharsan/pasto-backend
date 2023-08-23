using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pasto_backend.Data;
using pasto_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pasto_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all routes in this controller
    public class PasteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PasteController(AppDbContext context)
        {
            _context = context;
        }

        private string GetLoggedInUserEmail()
        {
            // Retrieve the email from the User.Identity.Name property
            return User.Identity.Name;
        }

        [HttpGet]
        public IActionResult GetAllPastes()
        {
            List<Paste> pastes = _context.Pastes.ToList();
            return Ok(pastes);
        }

        [HttpGet("{id}")]
        public IActionResult GetPasteById(int id)
        {
            Paste paste = _context.Pastes.FirstOrDefault(p => p.Id == id);
            if (paste == null)
            {
                return NotFound(new { response = "Paste not found" });
            }

            // Check if the paste is private and the logged-in user is the owner or collaborator
            if (paste.IsPrivate && paste.OwnerEmail != GetLoggedInUserEmail() &&
                (paste.CollaboratorEmailList == null || !paste.CollaboratorEmailList.Split(',').Contains(GetLoggedInUserEmail())))
            {
                return Unauthorized(new { response = "Unauthorized" });
            }

            return Ok(paste);
        }

        [HttpPost]
        public IActionResult CreatePaste([FromBody] Paste paste)
        {
            if (paste.IsPrivate)
            {
                paste.OwnerEmail = GetLoggedInUserEmail(); // Set the owner's email
            }

            Console.Write(paste.OwnerEmail);

            _context.Pastes.Add(paste);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPasteById), new { id = paste.Id }, paste);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePaste(int id, [FromBody] Paste updatedPaste)
        {
            Paste paste = _context.Pastes.FirstOrDefault(p => p.Id == id);
            if (paste == null)
            {
                return NotFound(new { response = "Paste not found" });
            }

            // Check if the paste is private and the logged-in user is the owner or collaborator
            if (paste.IsPrivate && paste.OwnerEmail != GetLoggedInUserEmail() &&
                (paste.CollaboratorEmailList == null || !paste.CollaboratorEmailList.Split(',').Contains(GetLoggedInUserEmail())))
            {
                return Unauthorized(new { response = "Unauthorized" });
            }

            paste.PastedContent = updatedPaste.PastedContent;
            paste.IsPrivate = updatedPaste.IsPrivate;
            paste.CollaboratorEmailList = updatedPaste.CollaboratorEmailList;
            _context.SaveChanges();
            return Ok(new { response = "Paste updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePaste(int id)
        {
            Paste paste = _context.Pastes.FirstOrDefault(p => p.Id == id);
            if (paste == null)
            {
                return NotFound(new { response = "Paste not found" });
            }

            // Check if the paste is private and the logged-in user is the owner or collaborator
            if (paste.IsPrivate && paste.OwnerEmail != GetLoggedInUserEmail() &&
                (paste.CollaboratorEmailList == null || !paste.CollaboratorEmailList.Split(',').Contains(GetLoggedInUserEmail())))
            {
                return Unauthorized(new { response = "Unauthorized" });
            }

            _context.Pastes.Remove(paste);
            _context.SaveChanges();
            return Ok(new { response = "Paste deleted successfully" });
        }
    }
}
