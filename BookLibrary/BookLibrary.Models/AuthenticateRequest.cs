﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class AuthenticateRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
