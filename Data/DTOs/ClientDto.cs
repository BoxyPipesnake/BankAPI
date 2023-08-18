
namespace BankAPI.Data.DTOs;

public class ClientDto
{
    public string Email { get; set; }
    public string Pwd { get; set; }

    public ClientDto()
    {
        Email = string.Empty;
        Pwd = string.Empty;
    }
}
