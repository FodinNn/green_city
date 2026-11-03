namespace FakeNewsGame.Api.Models;

public class AnswerResponse
{
    public bool Correct { get; set; }
    public bool ActualIsFake { get; set; }
    public string Explanation { get; set; } = "";
}