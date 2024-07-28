using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSBGHelper.Server.Models
{
    public class Spell
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Tier { get; set; }
        public string HtmlGuide { get; set; }
        public List<Spell> spellSynergies { get; set; }
        public List<Minion> minionSynergies { get; set; }
        public List<Hero> heroSynergies { get; set; }
        
    }
}
