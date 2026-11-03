namespace FakeNewsGame.Api.Models;

public class Notification
{
    public int Id { get; set; }
    public string AppName { get; set; } = "";
    public string Sender { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; }
    // ⚠️ Скрытые поля — клиенту не отдаём до ответа
    public bool IsFake { get; set; }
    public string Explanation { get; set; } = "";
}