using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using pasto_backend.Models;

namespace pasto_backend.Data
{
    public class PasteDbHandler
    {
        private readonly AppDbContext _context;

        public PasteDbHandler(AppDbContext context)
        {
            _context = context;
        }

        public List<Paste> GetAllPastes()
        {
            return _context.Pastes.ToList();
        }

        public Paste GetPasteById(int id)
        {
            return _context.Pastes.FirstOrDefault(p => p.Id == id);
        }

        public void AddPaste(Paste paste)
        {
            _context.Pastes.Add(paste);
            _context.SaveChanges();
        }

        public void UpdatePaste(Paste paste)
        {
            _context.Pastes.Update(paste);
            _context.SaveChanges();
        }

        public void DeletePaste(int id)
        {
            var paste = _context.Pastes.FirstOrDefault(p => p.Id == id);
            if (paste != null)
            {
                _context.Pastes.Remove(paste);
                _context.SaveChanges();
            }
        }
    }
}
