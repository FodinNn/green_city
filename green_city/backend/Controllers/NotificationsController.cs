using FakeNewsGame.Api.Data;
using FakeNewsGame.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace FakeNewsGame.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationRepository _repo;

    public NotificationsController(NotificationRepository repo) => _repo = repo;

    /// <summary>Получить случайное уведомление (без ответа).</summary>
    [HttpGet("random")]
    [ProducesResponseType(typeof(NotificationPublicDto), 200)]
    public ActionResult<NotificationPublicDto> GetRandom()
    {
        var n = _repo.GetRandom();
        if (n is null) return NotFound(new { message = "Нет уведомлений" });
        return Ok(ToDto(n));
    }

    /// <summary>Получить уведомление по id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(NotificationPublicDto), 200)]
    [ProducesResponseType(404)]
    public ActionResult<NotificationPublicDto> GetById(int id)
    {
        var n = _repo.GetById(id);
        if (n is null) return NotFound(new { message = "Уведомление не найдено" });
        return Ok(ToDto(n));
    }

    /// <summary>Проверить ответ пользователя.</summary>
    [HttpPost("{id:int}/check")]
    [ProducesResponseType(typeof(AnswerResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<AnswerResponse> Check(int id, [FromBody] AnswerRequest req)
    {
        if (req?.IsFake is null)
            return BadRequest(new { message = "Поле isFake обязательно" });

        var n = _repo.GetById(id);
        if (n is null) return NotFound(new { message = "Уведомление не найдено" });

        var response = new AnswerResponse
        {
            Correct = req.IsFake.Value == n.IsFake,
            ActualIsFake = n.IsFake,
            Explanation = n.Explanation
        };

        return Ok(response);
    }

    private static NotificationPublicDto ToDto(Notification n) => new()
    {
        Id = n.Id,
        AppName = n.AppName,
        Sender = n.Sender,
        Title = n.Title,
        Text = n.Text,
        Timestamp = n.Timestamp
    };
}