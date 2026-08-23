
using SupportOps.Application.Interfaces;
using SupportOps.Application.Security;





namespace SupportOps.Application.Services;

public class AuthenticationService : IAuthenticationService
{

    private readonly IUserRepository _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
              IUserRepository userRepository,
        JwtTokenGenerator jwtTokenGenerator)
    {

        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        var validPassword = BCrypt.Net.BCrypt.Verify(
            password,
            user.PasswordHash);

        if (!validPassword)
            return null;

        return _jwtTokenGenerator.GenerateToken(user);
    }
}