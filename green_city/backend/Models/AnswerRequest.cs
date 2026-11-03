using System.ComponentModel.DataAnnotations;

namespace FakeNewsGame.Api.Models;

public class AnswerRequest
{
    [Required]
    public bool? IsFake { get; set; } // ответ пользователя
}