using HSBGHelper.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HSBGHelper.Server.Models
{
    public class HeroPower 
    {
        [Key]
        public int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<Spell> spellSynergies { get; set; }
        public List<Minion> minionSynergies { get; set; }
        public List<Hero> heroSynergies { get; set; }
        public string HtmlGuide { get; set; }
    }
}