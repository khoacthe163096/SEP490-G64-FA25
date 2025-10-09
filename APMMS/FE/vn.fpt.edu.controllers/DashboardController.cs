using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // User Management Actions
        public IActionResult Employees()
        {
            return View("UserManagement/Employees");
        }

        public IActionResult Users()
        {
            return View("UserManagement/Users");
        }

        public IActionResult Directors()
        {
            return View("UserManagement/Directors");
        }

        // Warehouse Management Actions
        public IActionResult ComponentTypes()
        {
            return View("WarehouseManagement/ComponentTypes");
        }

        public IActionResult Components()
        {
            return View("WarehouseManagement/Components");
        }

        public IActionResult ServicePackages()
        {
            return View("WarehouseManagement/ServicePackages");
        }

        // Maintenance Actions
        public IActionResult Schedules()
        {
            return View("Maintenance/Schedules");
        }

        public IActionResult Processes()
        {
            return View("Maintenance/Processes");
        }

        // Invoices Actions
        public IActionResult Invoices()
        {
            return View("Invoices/Invoices");
        }
    }
}