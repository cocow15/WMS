using ApplicationTest.Entities;

namespace ApplicationTest.Services;

public interface IJwtTokenProvider
{
    string Generate(User user);
}
