using System;
using System.Collections.Generic;

namespace NikGame.Dart.Service
{
    public class DartMatch
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? WinerId { get; set; }
        public int WinerScore { get; set; }
        public int Players { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual AppUser Winer { get; set; }
        public virtual ICollection<DartMatchUser> DartMatchUsers { get; set; }
        public virtual ICollection<DartShoot> DartShoots { get; set; }
    }
}
