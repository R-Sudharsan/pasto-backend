
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using pasto_backend.Data;

namespace pasto_backend.Models
{


    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}