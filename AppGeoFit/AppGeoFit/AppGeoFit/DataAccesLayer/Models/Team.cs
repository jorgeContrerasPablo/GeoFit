﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class Team
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Team()
        {
            FeedBacks = new HashSet<FeedBack>();
            Games = new HashSet<Game>();
            Games1 = new HashSet<Game>();
            Joineds = new HashSet<Joined>();
            Places = new HashSet<Place>();
        }

        public int TeamID { get; set; }

        public string TeamName { get; set; }

        public string ColorTeam { get; set; }

        public Guid? EmblemID { get; set; }

        public double? Level { get; set; }

        public int SportID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Game> Games { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Game> Games1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Joined> Joineds { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Place> Places { get; set; }

        
        public override String ToString()
            {
                return TeamName;
            }
        public override int GetHashCode()
        {
            return TeamID;
        }

        public override bool Equals(object obj)
        {
            var team = obj as Team;

            if (team == null)
            {
                return false;
            }
            return TeamID == team.TeamID
                && TeamName.Equals(team.TeamName)
                && ColorTeam.Equals(team.ColorTeam)
                && Level == team.Level
                && SportID == team.SportID;

    }
    }
}
