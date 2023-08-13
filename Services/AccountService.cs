using Microsoft.EntityFrameworkCore;
using BankAPI.Data.BankModels;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Services;

public class AccountService
{
    private readonly BankContext _context;
    private readonly ClientService clientService;

    public AccountService(BankContext context, ClientService clientService)
    {
        _context = context;
        this.clientService = clientService;
    }

    public async Task<IEnumerable<AccountDtoOut>> GetAll()
    {
        return await _context.Accounts.Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }


    public async Task<AccountDtoOut?> GetDtoById(int id)
    {
        return await _context.Accounts.
            Where(a => a.Id == id).
            Select(a => new AccountDtoOut
            {
                Id = a.Id,
                AccountName = a.AccountTypeNavigation.Name,
                ClientName = a.Client != null ? a.Client.Name : "",
                Balance = a.Balance,
                RegDate = a.RegDate
            }).SingleOrDefaultAsync();
    }

    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Create(AccountDtoIn newAccountDTO)
    {
        var clientID = newAccountDTO.ClientId.GetValueOrDefault();
        var client = await clientService.GetById(clientID);

        if (client is null)
            return null; // Cliente no existe

        var newAccount = new Account();

        newAccount.AccountType = newAccountDTO.AccountType;
        newAccount.Balance = newAccountDTO.Balance;
        newAccount.ClientId = clientID;

        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return newAccount;
    }


    public async Task Update(AccountDtoIn account)
    {
        var existingAccount = await GetById(account.Id);

        if (existingAccount is not null)
        {
            if (account.ClientId.HasValue)
            {
                var client = await clientService.GetById(account.ClientId.Value);

                if (client is null)
                    throw new Exception($"El cliente con ID {account.ClientId.Value} no existe.");
            }

            existingAccount.AccountType = account.AccountType;
            existingAccount.ClientId = account.ClientId;
            existingAccount.Balance = account.Balance;

            await _context.SaveChangesAsync();
        }
    }


    public async Task Delete(int id)
    {
        var accountToDelete = await GetById(id);

        if (accountToDelete is not null)
        {
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }

    }
}