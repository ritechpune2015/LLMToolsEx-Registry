using LLMToolsEx.Dtos;
using LLMToolsEx.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LLMToolsEx.Controllers
{
    public class ToolCallingController : Controller
    {
      private readonly IToolCallingLLMService _llm;
      public ToolCallingController(IToolCallingLLMService llm)
      {
            _llm = llm;
      }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

     [HttpPost]
      public async Task<IActionResult> GetBalance(ToolRequest request)
        {
            var result =
                await _llm
                    .GenerateWithToolsAsync(
                        request.Prompt);
            ViewBag.Answer = result;
            return View("Index");
           
        
    }
}
}
