using Dynamic_Form_Builder.Interface;
using Dynamic_Form_Builder.ViewModel;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Dynamic_Form_Builder.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormApiController : Controller
    {
        private readonly IFormRepository _formRepo;

        public FormApiController(IFormRepository formRepo)
        {
            _formRepo = formRepo;
        }

        [HttpPost("Save")]
        public IActionResult SaveForm([FromBody] FormVM model)
        {
            try
            {
                int formId = _formRepo.SaveForm(model);
                return Ok(new { success = true, message = "Form saved.", formId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Update")]
        public IActionResult UpdateForm([FromBody] FormVM model)
        {
            if (model == null || model.Fields == null || !model.Fields.Any())
            {
                return BadRequest(new { success = false, message = "Invalid form data." });
            }

            bool isUpdated = _formRepo.UpdateFormFields(model, out string error);

            if (isUpdated)
            {
                return Ok(new { success = true, message = "Form updated." });
            }
            else
            {
                return BadRequest(new { success = false, message = error });
            }
        }

    }
}
