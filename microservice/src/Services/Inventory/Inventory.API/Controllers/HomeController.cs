﻿using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}