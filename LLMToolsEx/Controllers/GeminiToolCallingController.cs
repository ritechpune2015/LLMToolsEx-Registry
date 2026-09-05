using LLMToolsEx.Dtos;
using LLMToolsEx.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LLMToolsEx.Controllers
{
    public class GeminiToolCallingController : Controller
    {
        private readonly IToolCallingLLMService _llm;

        public GeminiToolCallingController(IToolCallingLLMService llm)
        {
            _llm = llm;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetCustomerBalance(
            ToolRequest request)
        {
            if (string.IsNullOrWhiteSpace(
                request.Prompt))
            {
                return BadRequest(
                    "Prompt is required.");
            }


            var result =
                await _llm
                    .GenerateWithToolsAsync(
                        request.Prompt);

            ViewBag.Answer = result;
            return View("Index");
        }
    }
}
