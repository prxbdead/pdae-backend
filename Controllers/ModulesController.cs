using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pdae.Api.Data;
using Pdae.Api.DTOs.Content;

namespace Pdae.Api.Controllers;

[ApiController]
[Authorize]
[Route("modules")]
public class ModulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ModulesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ModuleSummary>>> GetModules()
    {
        var modules = await _context.LearningModules
            .AsNoTracking()
            .OrderBy(m => m.OrderIndex)
            .ThenBy(m => m.Id)
            .Select(m => new ModuleSummary(
                m.Id,
                m.Title,
                m.Description,
                m.OrderIndex,
                m.Lessons.Count()
            ))
            .ToListAsync();

        return Ok(modules);
    }

    [HttpGet("{id:guid}/lessons")]
    public async Task<ActionResult<List<LessonSummary>>> GetLessons(
        Guid id)
    {
        var moduleExists = await _context.LearningModules
            .AnyAsync(m => m.Id == id);

        if (!moduleExists)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Module not found",
                detail: "The requested learning module does not exist."
            );
        }

        var lessons = await _context.Lessons
            .AsNoTracking()
            .Where(l => l.LearningModuleId == id)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonSummary(
                l.Id,
                l.LearningModuleId,
                l.Title,
                l.Description,
                l.OrderIndex,
                l.XpReward
            ))
            .ToListAsync();

        return Ok(lessons);
    }
}