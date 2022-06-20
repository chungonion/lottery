namespace Lottery.Models;

public class Status
{
    
    //Only to be updated, not to be inserted !
    public long StatusId { get; set; }
    public DateTime NextDrawTime { get; set; }
    public long LatestFinishedDrawId { get; set; } //Increment
    public long NextDrawId { get; set; } //Increment
    public long DrawInterval { get; set; }
}