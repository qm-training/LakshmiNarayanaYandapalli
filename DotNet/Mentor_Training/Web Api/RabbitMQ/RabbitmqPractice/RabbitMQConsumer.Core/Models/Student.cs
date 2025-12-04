namespace RabbitMQConsumer.Core.Models;

public partial class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? Age { get; set; }

    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
}