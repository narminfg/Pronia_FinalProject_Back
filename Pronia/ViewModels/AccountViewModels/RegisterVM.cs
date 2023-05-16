﻿using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.AccountViewModels
{
    public class RegisterVM
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? SurName { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

      

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
