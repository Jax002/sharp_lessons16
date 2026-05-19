namespace sharp_lessons16
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsImportant { get; set; }
        public bool IsOverdue => !IsCompleted && Deadline.Date < DateTime.Now.Date;
    }
}
