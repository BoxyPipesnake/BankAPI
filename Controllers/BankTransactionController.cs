using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{

    private readonly BankContext _context;

    public BankTransactionController(BankContext context)
    {
        _context = context;
    }

    [HttpGet("accounts")]
    public IActionResult GetAccounts()
    {
        int clientId = GetAuthenticatedClientId();

        var accounts = _context.Accounts
            .Where(a => a.ClientId == clientId)
            .ToList();

        return Ok(accounts);
    }

    [HttpPost("withdraw")]
    public IActionResult Withdraw(WithdrawDto withdrawDto)
    {
        int clientId = GetAuthenticatedClientId();


        var account = _context.Accounts.FirstOrDefault(a => a.Id == withdrawDto.AccountId && a.ClientId == clientId);

        if (account == null)
        {
            return BadRequest(new { message = "Cuenta no disponible" });
        }


        if (account.Balance >= withdrawDto.Amount)
        {
            account.Balance -= withdrawDto.Amount;
            _context.SaveChanges();

            return Ok(new { message = "Retiro exitoso" });
        }
        else
        {
            return BadRequest(new { message = "Fondos insuficientes" });
        }
    }


    [HttpPost("deposit")]
    public IActionResult Deposit(DepositDto depositDto)
    {
        int clientId = GetAuthenticatedClientId();


        var account = _context.Accounts.FirstOrDefault(a => a.Id == depositDto.AccountId && a.ClientId == clientId);

        if (account == null)
        {
            return BadRequest(new { message = "Cuenta no disponible" });
        }


        account.Balance += depositDto.Amount;
        _context.SaveChanges();

        return Ok(new { message = "Deposito exitoso" });
    }


    [HttpDelete("delete-account/{accountId}")]
    public IActionResult DeleteAccount(int accountId)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null)
            return NotFound();

        if (account.Balance == 0)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return Ok(new { message = "Cuenta eliminada exitosamente" });
        }
        else
        {
            return BadRequest(new { message = "Cuenta no puede ser eliminada, contiene saldo" });
        }
    }

    private int GetAuthenticatedClientId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        
        if (identity != null && int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int clientId))
        {
            return clientId;
        }

        
        throw new InvalidOperationException("No fue posible obtener el ID del cliente");
    }

}