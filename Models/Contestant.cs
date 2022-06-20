namespace Lottery.Models;

public class Contestant
{
    public string ContestantId { get; set; } = "";
    
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}