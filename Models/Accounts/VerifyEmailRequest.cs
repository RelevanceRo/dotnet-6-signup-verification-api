namespace TCIG.MHUSIGTASAPI.Models.Accounts;

using System.ComponentModel.DataAnnotations;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}