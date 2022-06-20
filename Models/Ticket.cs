using System.ComponentModel.DataAnnotations;

namespace Lottery.Models;

public class Ticket
{
    [Key] public string TicketId { get; set; } = "";
    public long DrawId { get; set; }
    public string ContestantId { get; set; } = "";
    public Contestant Contestant { get; set; } 
    public string TicketSequence { get; set; } = "";
    public Draw Draw { get; set; }
    public bool HasWin { get; set; } = false;
    public DateTime CreateTime { get; set; }
}