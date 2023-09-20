﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealWorldProjectUnitTest.Web.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public int? Stock { get; set; }

    [Required]
    public string? Color { get; set; }

    public string? Description { get; set; }
}
