using System.Collections.Generic;

namespace NikGame.Dart.Service
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<DartMatch> WinDartMatches { get; set; }
        public virtual ICollection<DartMatchUser> DartMatchUsers { get; set; }
        public virtual ICollection<DartShoot> DartShoots { get; set; }
    }
}
