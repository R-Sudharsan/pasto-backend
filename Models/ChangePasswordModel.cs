using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using pasto_backend.Data;

namespace pasto_backend.Models
{
    public class ChangePasswordModel
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}