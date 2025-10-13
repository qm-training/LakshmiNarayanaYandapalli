namespace RabbitMQConsumer.Core.Models;

public partial class Teacher
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? Age { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;
}