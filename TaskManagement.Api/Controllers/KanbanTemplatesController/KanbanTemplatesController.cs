using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers.KanbanTemplatesController;

public partial class KanbanTemplatesController(IKanbanTemplateService templateService) : ControllerBase
{
    private readonly IKanbanTemplateService _templateService = templateService;
    private int UserId => int.Parse(User.FindFirstValue("sub")!);
}