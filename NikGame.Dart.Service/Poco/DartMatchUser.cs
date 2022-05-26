using System.Collections.Generic;

namespace NikGame.Dart.Service
{
    public class DartMatchUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MatchId { get; set; }
        public int TotalScore { get; set; }

        public virtual AppUser User { get; set; }
        public virtual DartMatch DartMatch { get; set; }
        public virtual ICollection<DartShoot> DartShoots { get; set; }
    }
}
