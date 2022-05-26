using Microsoft.AspNetCore.Identity;

namespace NikGame.Dart.Service
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
