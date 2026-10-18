using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasaGaillard.Core.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("/Pagina/Pagina/Index");
    }
}
