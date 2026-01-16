namespace Domain;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public required string UserId { get; set; }
    public required User User { get; set; }

    public required string ActivityId { get; set; }
    public required Activity Activity { get; set; }
}