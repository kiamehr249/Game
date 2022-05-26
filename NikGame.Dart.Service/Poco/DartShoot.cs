using System;

namespace NikGame.Dart.Service
{
    public class DartShoot
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public int MatchUserId { get; set; }
        public int ShotScore { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual DartMatch DartMatch { get; set; }
        public virtual AppUser User { get; set; }
        public virtual DartMatchUser MatchUser { get; set; }
    }
}
