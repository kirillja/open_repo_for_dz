namespace UniversitySystem.Entities
{
    public abstract class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Без required
        public Teacher? Teacher { get; set; }
        public List<Student> Students { get; } = new List<Student>();
        
        public abstract string GetCourseDetails();
    }

    public class OnlineCourse : Course
    {
        public string Platform { get; set; } = string.Empty; // Без required
        public string Link { get; set; } = string.Empty;     // Без required
        
        public override string GetCourseDetails() => 
            $"Online Course: {Name}, Platform: {Platform}, Link: {Link}";
    }

    public class OfflineCourse : Course
    {
        public string Location { get; set; } = string.Empty; // Без required
        public string Schedule { get; set; } = string.Empty; // Без required
        
        public override string GetCourseDetails() => 
            $"Offline Course: {Name}, Location: {Location}, Schedule: {Schedule}";
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}