using Lottery.Models;
using Lottery.Models.Response;
using Lottery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Controllers;

[ApiController]
[Route("/Tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly LotteryContext _context;
    private readonly IConfiguration _configuration;

    public TicketController(ITicketService ticketService, LotteryContext context, IConfiguration configuration)
    {
        _ticketService = ticketService;
        _context = context;
        _configuration = configuration;
    }

    // GET
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }

    [HttpGet]
    [Route("{ticketId}")]
    public async Task<IActionResult> CheckTicketWithTicketId([FromRoute] string ticketId /*,[FromQuery] string token*/)
    {
        //TODO: requirement on the token has to be confirmed
        var ticket = await _context.Tickets
            .Include(x=>x.Draw)
            .Select(x => new
                { TicketId = x.TicketId, 
                    TicketSequence = x.TicketSequence, 
                    Drawn = x.Draw.Drawn,
                    DrawId = x.DrawId, 
                    HasWin = x.HasWin,
                    CreateTime = x.CreateTime
                })
            .FirstOrDefaultAsync(x => x.TicketId == ticketId);
        if (ticket == null)
        {
            return StatusCode(404, new ResponseModel { Status = 404, Error = "Not Found!" });
        }

        return StatusCode(200, new ResponseModel { Status = 200, Data = ticket });
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> CheckTickets()
    {
        //Maintenance purpose, 403 otherwise
        return StatusCode(403, new ResponseModel { Status = 403, Error = "Unauthorized" });
        //TODO: requirement on the token has to be confirmed
        var tickets = await _context.Tickets.Include(x => x.Contestant).ToListAsync();

        return StatusCode(200, new ResponseModel { Status = 200, Data = tickets });
    }


    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketBody body)
    {
        var status = await _context.Status.FirstOrDefaultAsync();
        if (status == null)
        {
            return StatusCode(500, new ResponseModel { Status = 500, Error = "Server Error" });
        }

        var ticket = await _ticketService.CreateTicket(body.ContestantId, status);

        if (ticket == null)
        {
            return StatusCode(400,
                new ResponseModel { Status = 400, Error = "You have one ticket for this draw already!" });
        }

        var returnData = await _context.Tickets
            .Include(x=>x.Draw)
            .Select(x => new
        {
            TicketId = x.TicketId,
            TicketSequence = x.TicketSequence,
            Drawn = x.Draw.Drawn,
            DrawId = x.DrawId,
            HasWin = x.HasWin,
            CreateTime = x.CreateTime
        }).FirstOrDefaultAsync(x => x.TicketId == ticket.TicketId);

        return StatusCode(200, new ResponseModel { Status = 200, Data = returnData });
    }
}

public class CreateTicketBody
{
    public string ContestantId { get; set; } = "";
}