﻿using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Cashier.Controllers
{

    [Area("Cashier")]
    [Authorize(Roles = SD.Role_Cashier)]
    public class HomeController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _categoryRepository;
        public HomeController(IInvoiceRepository invoiceRepository, ITableRepository tableRepository, IFoodRepository foodRepository, IFoodCategoryRepository categoryRepository)
        {
            _invoiceRepository = invoiceRepository;
            _tableRepository = tableRepository;
            _foodRepository = foodRepository;
            _categoryRepository = categoryRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Code
            var invoices = await _invoiceRepository.GetAllAsync();
            var tables = await _tableRepository.GetAllAsync();
            var foods = await _foodRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var totalRevenue = invoices.Sum(invoice => invoice.Price);
            var tableCount = tables.Count();
            var foodCount = foods.Count();
            var categoryCount = categories.Count();

            ViewBag.invoices = invoices;
            ViewBag.totalRevenue = totalRevenue;
            ViewBag.tableCount = tableCount;
            ViewBag.foodCount = foodCount;
            ViewBag.categoryCount = categoryCount;

            return View();
        }
    }
}
