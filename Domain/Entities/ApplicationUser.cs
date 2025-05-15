namespace Domain.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? AccessRole { get; set; }
        public int? Age { get; set; }
        public double? Salary { get; set; }
        public string? Title { get; set; }
        public double? Exp { get; set; }
        public string? Department { get; set; }
        public ICollection<Todo>? Todos { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
