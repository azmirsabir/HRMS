using Core.Domain.Users;

namespace Application.Contracts.Authentication;

public class MeResponse
{
    public string Id {get; set;}
    public string FullName {get; set;}
    public string UserName {get; set;}
    public string Email {get; set;}
    public string PhoneNumber{get; set;}

    public static MeResponse ToResponse(User user)
    {
        return new MeResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        };
    }

}