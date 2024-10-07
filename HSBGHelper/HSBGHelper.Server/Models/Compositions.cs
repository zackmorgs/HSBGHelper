using HSBGHelper.Server.Models;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Composition
{
    [Key]
    public int Id { get; set; }
    public string Tier { get; set; }
    public string Name { get; set; }
    public string Difficulty { get; set; }
    public string Link { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public Minion mainMinion { get; set; }
    public List<Minion> AddOnMinions { get; set; }
    // public List<Buddy> AddOnBuddies { get; set; }
    public List<Minion> CommonEnablerMinions { get; set; }
    // public List<Buddy> CommonEnablerBuddies { get; set; }
    public string HtmlGuide { get; set; }

}