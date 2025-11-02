namespace Application.Interfaces;
public interface ICurrentUser
{
    string? UserName { get; }
    string? Id { get; }
    List<string> Roles { get; }
}