using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebTaskSnippet.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string  FullName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ChangeReason { get; set; }
        public string ChangeDescription { get; set; }

        public IEnumerable<PropertyInfo> GetVariance(Student student)
        {
            foreach (PropertyInfo pi in student.GetType().GetProperties())
            {

                object valueUser = typeof(Student).GetProperty(pi.Name).GetValue(student);
                object valueThis = typeof(Student).GetProperty(pi.Name).GetValue(this);

                if (valueUser != null && !valueUser.Equals(valueThis))
                    yield return pi;

            }
        }


    }

}