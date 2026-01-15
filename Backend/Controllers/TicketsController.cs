using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Backend.Data;

namespace Backend.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController : ControllerBase
{
    private readonly TicketDbContext _db;
    private readonly TicketNotifier _notifier;

    public TicketsController(TicketDbContext db, TicketNotifier notifier)
    {
        _db = db;
        _notifier = notifier;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketRequest request)
    {
        var ticket = new Ticket
        {
            Name = request.Name,
            Email = request.Email,
            Message = request.Message
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        await _notifier.NotifyAsync(
            ticket.Id,
            ticket.Name,
            ticket.Email,
            ticket.Message
        );
        

        return Ok(new { ticket.Id });
    }
    [HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    var ticket = await _db.Tickets.FindAsync(id);
    if (ticket is null)
        return NotFound(new { error = $"Ticket mit Id={id} nicht gefunden." });

    return Ok(ticket);
}

[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    var ticket = await _db.Tickets.FindAsync(id);
    if (ticket is null)
        return NotFound(new { error = $"Ticket mit Id={id} nicht gefunden." });

    _db.Tickets.Remove(ticket);
    await _db.SaveChangesAsync();

    return NoContent();
}

}
