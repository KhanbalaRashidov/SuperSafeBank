﻿using System.ComponentModel.DataAnnotations;

namespace SuperSafeBank.Service.Core.DTOs
{
    public record WithdrawDto
    {
        [Required]
        public string CurrencyCode { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}