using UniversitySystem.Entities;

namespace UniversitySystem.Builders
{
    public abstract class CourseBuilder
    {
        protected Course course = null!;
        
        public abstract CourseBuilder SetId(int id);
        public abstract CourseBuilder SetName(string name);
        public abstract CourseBuilder SetTeacher(Teacher teacher);
        public abstract Course Build();
    }

    public class OnlineCourseBuilder : CourseBuilder
    {
        public OnlineCourseBuilder() => course = new OnlineCourse();
        
        public override CourseBuilder SetId(int id)
        {
            course.Id = id;
            return this;
        }
        
        public override CourseBuilder SetName(string name)
        {
            course.Name = name;
            return this;
        }
        
        public override CourseBuilder SetTeacher(Teacher teacher)
        {
            course.Teacher = teacher;
            return this;
        }
        
        public OnlineCourseBuilder SetPlatform(string platform)
        {
            if (course is OnlineCourse onlineCourse)
                onlineCourse.Platform = platform;
            return this;
        }
        
        public OnlineCourseBuilder SetLink(string link)
        {
            if (course is OnlineCourse onlineCourse)
                onlineCourse.Link = link;
            return this;
        }
        
        public override Course Build() => course;
    }

    public class OfflineCourseBuilder : CourseBuilder
    {
        public OfflineCourseBuilder() => course = new OfflineCourse();
        
        public override CourseBuilder SetId(int id)
        {
            course.Id = id;
            return this;
        }
        
        public override CourseBuilder SetName(string name)
        {
            course.Name = name;
            return this;
        }
        
        public override CourseBuilder SetTeacher(Teacher teacher)
        {
            course.Teacher = teacher;
            return this;
        }
        
        public OfflineCourseBuilder SetLocation(string location)
        {
            if (course is OfflineCourse offlineCourse)
                offlineCourse.Location = location;
            return this;
        }
        
        public OfflineCourseBuilder SetSchedule(string schedule)
        {
            if (course is OfflineCourse offlineCourse)
                offlineCourse.Schedule = schedule;
            return this;
        }
        
        public override Course Build() => course;
    }
}