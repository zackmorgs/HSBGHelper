using HSBGHelper.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSBGHelper.Server.Models
{
    public class Hero
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Armor { get; set; }
        // public Buddy buddy { get; set; }
        public HeroPower heroPower { get; set; }
        public string TierHSReplay {get; set;}
        public string TierJeef {get; set;}
        public List<Spell> spellSynergies { get; set; }
        public List<Minion> minionSynergies { get; set; }
        public List<Hero> heroSynergies { get; set; }
        public string HtmlGuide { get; set; }

    }
}
