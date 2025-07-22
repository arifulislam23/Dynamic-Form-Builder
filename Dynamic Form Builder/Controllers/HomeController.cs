using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dynamic_Form_Builder.Models;
using Microsoft.Data.SqlClient;
using Dynamic_Form_Builder.ViewModel;
using Dynamic_Form_Builder.Interface;

namespace Dynamic_Form_Builder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFormRepository _formRepo;

    public HomeController(ILogger<HomeController> logger, IFormRepository formRepo)
    {
        _logger = logger;
        _formRepo = formRepo;
    }

    public IActionResult Create()
    {
        ViewBag.DropdownOptions = StaticOptions.DropdownOptions;
        return View();
    }

    [HttpPost("SaveForm")]
    [ValidateAntiForgeryToken]
    public IActionResult SaveForm([FromBody] FormVM formVM)
    {
        if (formVM == null || string.IsNullOrWhiteSpace(formVM.Title) || formVM.Fields == null || formVM.Fields.Count == 0)
        {
            return Json(new { success = false, message = "Invalid form data." });
        }

        try
        {
            _formRepo.SaveForm(formVM);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public IActionResult Index()
    {
        var forms = _formRepo.GetAllForms();
        return View(forms);
    }

    [HttpGet]
    public IActionResult Preview(int id)
    {
        var form = _formRepo.GetFormWithFields(id);
        return View(form);
    }

    [HttpPost("Update")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(FormVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid form data." });
        }

        bool updated = _formRepo.UpdateFormFields(model, out string error);
        return Json(new
        {
            success = updated,
            message = updated ? "Form updated successfully." : $"Error: {error}"
        });
    }

    public IActionResult ScriptBackup()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
