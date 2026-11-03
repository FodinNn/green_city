namespace FakeNewsGame.Api.Models;

public class NotificationPublicDto
{
    public int Id { get; set; }
    public string AppName { get; set; } = "";
    public string Sender { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; }
}