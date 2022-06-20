using System.Security.Cryptography;
using System.Text;
using Lottery.Models;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Services;

public interface ITicketService
{
    string GenerateTicketSequence(int size);
    Task<Ticket?> CreateTicket(string contestantId, Status status);
}

public class TicketService : ITicketService
{
    private readonly LotteryContext _context;

    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public TicketService(LotteryContext context)
    {
        _context = context;
    }


    public async Task<Ticket?> CreateTicket(string contestantId, Status status)
    {
        if (_context.Tickets.Any(x => x.ContestantId == contestantId && x.DrawId == status.NextDrawId) ||
            string.IsNullOrEmpty(contestantId))
        {
            return null;
        }

        var sequence = "";

        do
        {
            sequence = GenerateTicketSequence();
        } while (_context.Tickets.Any(x => x.TicketSequence == sequence));


        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid().ToString(),
            CreateTime = DateTime.Now,
            TicketSequence = sequence,
            DrawId = status.NextDrawId,
            ContestantId = contestantId,
        };
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public string GenerateTicketSequence(int size = 10)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        byte[] data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }

        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }
}