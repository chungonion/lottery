namespace Lottery.Models;

public class Draw
{
    public long DrawId { get; set; }
    public string? WinningSequence { get; set; }
    public bool Drawn { get; set; } = false;
    public DateTime DrawDate { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}