using System.Reflection;
using UniversitySystem.Managers;
using UniversitySystem.Entities;
using UniversitySystem.Builders;
using Xunit;

namespace UniversitySystem.Tests
{
    public class CourseManagerTests
    {
        private readonly CourseManager _manager = CourseManager.Instance;
        private readonly Teacher _teacher1 = new Teacher { Id = 1, Name = "Dr. Smith" };
        private readonly Teacher _teacher2 = new Teacher { Id = 2, Name = "Prof. Johnson" };
        private readonly Student _student1 = new Student { Id = 1, Name = "Alice" };
        private readonly Student _student2 = new Student { Id = 2, Name = "Bob" };

        public CourseManagerTests()
        {
            // Очистка синглтона перед каждым тестом
            typeof(CourseManager)
                .GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, new Lazy<CourseManager>(() => new CourseManager()));
        }

        [Fact]
        public void AddAndRemoveCourse_CourseRemovedSuccessfully()
        {
            // Arrange
            var course = new OnlineCourse { Id = 1, Name = "Math" };
            _manager.AddCourse(course);
            
            // Act
            var result = _manager.RemoveCourse(1);
            
            // Assert
            Assert.True(result);
            Assert.Empty(_manager.GetCoursesByTeacher(0));
        }

        [Fact]
        public void AssignTeacherToCourse_TeacherAssignedCorrectly()
        {
            // Arrange
            _manager.AddTeacher(_teacher1);
            var course = new OfflineCourse { Id = 101, Name = "Physics" };
            _manager.AddCourse(course);
            
            // Act
            _manager.AssignTeacherToCourse(1, 101);
            
            // Assert
            var courses = _manager.GetCoursesByTeacher(1);
            Assert.Single(courses);
            Assert.Equal("Physics", courses[0].Name);
        }

        [Fact]
        public void EnrollStudentsToCourse_StudentsEnrolledSuccessfully()
        {
            // Arrange
            _manager.AddStudent(_student1);
            _manager.AddStudent(_student2);
            var course = new OnlineCourse { Id = 201, Name = "Programming" };
            _manager.AddCourse(course);
            
            // Act
            _manager.EnrollStudentToCourse(1, 201);
            _manager.EnrollStudentToCourse(2, 201);
            
            // Assert
            var students = _manager.GetStudentsByCourse(201);
            Assert.Equal(2, students.Count);
            Assert.Contains(students, s => s.Name == "Alice");
            Assert.Contains(students, s => s.Name == "Bob");
        }

        [Fact]
        public void GetCoursesByTeacher_ReturnsCorrectCourses()
        {
            // Arrange
            _manager.AddTeacher(_teacher1);
            _manager.AddTeacher(_teacher2);
            
            var course1 = new OfflineCourse { Id = 301, Name = "Chemistry", Teacher = _teacher1 };
            var course2 = new OnlineCourse { Id = 302, Name = "Biology", Teacher = _teacher1 };
            var course3 = new OfflineCourse { Id = 303, Name = "History", Teacher = _teacher2 };
            
            _manager.AddCourse(course1);
            _manager.AddCourse(course2);
            _manager.AddCourse(course3);
            
            // Act
            var teacher1Courses = _manager.GetCoursesByTeacher(1);
            
            // Assert
            Assert.Equal(2, teacher1Courses.Count);
            Assert.Contains(teacher1Courses, c => c.Name == "Chemistry");
            Assert.Contains(teacher1Courses, c => c.Name == "Biology");
        }

        [Fact]
        public void BuilderPattern_CreatesCorrectCourseTypes()
        {
            // Arrange
            var onlineBuilder = new OnlineCourseBuilder();
            var offlineBuilder = new OfflineCourseBuilder();
            
            // Act
            var onlineCourse = onlineBuilder
                .SetId(401)
                .SetName("Web Development")
                .SetPlatform("Coursera")
                .SetLink("https://coursera.org/webdev")
                .Build();
                
            var offlineCourse = offlineBuilder
                .SetId(402)
                .SetName("Advanced Math")
                .SetLocation("Building A, Room 101")
                .SetSchedule("Mon/Wed 10:00-12:00")
                .Build();
            
            // Assert
            Assert.IsType<OnlineCourse>(onlineCourse);
            Assert.Equal("Coursera", ((OnlineCourse)onlineCourse).Platform);
            
            Assert.IsType<OfflineCourse>(offlineCourse);
            Assert.Equal("Building A, Room 101", ((OfflineCourse)offlineCourse).Location);
        }
    }
}