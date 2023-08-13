using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.DTOs;
using TestBankAPI.Services;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService loginService;

    public LoginController(LoginService loginService)
    {
        this.loginService = loginService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(AdminDto adminDto)
    {
        var admin = await loginService.GetAdmin(adminDto);

        if(admin is null)
            return BadRequest(new { message = "Credenciales Invalidas." });
        

        // Generar un token
        return Ok( new { token = "some value" });
    }


}