﻿using System.ComponentModel.DataAnnotations;

namespace SuperSafeBank.Service.Core.DTOs
{
    public record CreateCustomerDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}