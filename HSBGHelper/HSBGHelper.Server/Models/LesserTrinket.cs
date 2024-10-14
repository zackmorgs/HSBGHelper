using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSBGHelper.Server.Models {
    public class LesserTrinket {
        [Key]
        public int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string HtmlGuide { get; set; }
        public int Cost { get; set;}
        public char Tier { get; set;}
        public bool IsInSolos { get; set;}
        public bool IsInDuos { get; set;}
    }
}