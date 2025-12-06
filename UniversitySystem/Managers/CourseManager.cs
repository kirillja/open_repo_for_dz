using UniversitySystem.Entities;

namespace UniversitySystem.Managers
{
    public class CourseManager
    {
        private readonly List<Course> _courses = new List<Course>();
        private readonly List<Teacher> _teachers = new List<Teacher>();
        private readonly List<Student> _students = new List<Student>();

        private static readonly Lazy<CourseManager> _instance = 
            new Lazy<CourseManager>(() => new CourseManager());
        
        public static CourseManager Instance => _instance.Value;
        
        private CourseManager() { }

        public void AddTeacher(Teacher teacher) => _teachers.Add(teacher);
        
        public void AddStudent(Student student) => _students.Add(student);
        
        public void AddCourse(Course course) => _courses.Add(course);
        
        public bool RemoveCourse(int courseId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            _courses.Remove(course);
            return true;
        }

        public void AssignTeacherToCourse(int teacherId, int courseId)
        {
            var teacher = _teachers.FirstOrDefault(t => t.Id == teacherId);
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            
            if (teacher != null && course != null)
                course.Teacher = teacher;
        }

        public void EnrollStudentToCourse(int studentId, int courseId)
        {
            var student = _students.FirstOrDefault(s => s.Id == studentId);
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            
            if (student != null && course != null)
                course.Students.Add(student);
        }

        public List<Course> GetCoursesByTeacher(int teacherId) => 
            _courses.Where(c => c.Teacher?.Id == teacherId).ToList();

        public List<Student> GetStudentsByCourse(int courseId) => 
            _courses.FirstOrDefault(c => c.Id == courseId)?.Students ?? new List<Student>();
    }
}