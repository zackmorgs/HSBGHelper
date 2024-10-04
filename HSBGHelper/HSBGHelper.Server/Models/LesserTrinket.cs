using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSBGHelper.Server.Models {
    public class LesserTrinket {
        [Key]
        public int Id { get; set;}

        public string Name { get; set; }
        public string Image { get; set; }
        
        public string HtmlGuide { get; set; }
    }
}