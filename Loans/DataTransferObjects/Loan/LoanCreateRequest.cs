﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Loans.DataTransferObjects.Loan
{
    public class LoanCreateRequest
    {
        public long Amount { get; set; }

        [Required]
        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }
    }
}
