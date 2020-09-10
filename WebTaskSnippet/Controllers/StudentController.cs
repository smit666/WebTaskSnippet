using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebTaskSnippet.Models;

namespace WebTaskSnippet.Controllers
{
    public class StudentController : Controller
    {
        SqlConnection connection = new SqlConnection("server=MSI\\SQLEXPRESSNEW;user id=sa;password=Password123;database=StudentPortal;persistsecurityinfo=True");
        // GET: Student
        public ActionResult Index()
        {
            List<Student> result = new List<Student>();
            string query = "select * from Student";

            SqlCommand cmd = new SqlCommand(query,connection);

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Student st = new Student();
                    st.FullName = reader["FullName"].ToString();
                    st.StudentId=int.Parse( reader["StudentId"].ToString());
                    result.Add(st); ;
                }
            }
           

          //  SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            return View(result);
        }


        public ActionResult Edit(int Id)
        {

            Student result = new Student();
            string query = "select * from Student where StudentId='"+Id.ToString()+"'";

            SqlCommand cmd = new SqlCommand(query, connection);

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.FullName = reader["FullName"].ToString();
                    result.StudentId = int.Parse(reader["StudentId"].ToString());
                }
            }
            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(Student stu)
        {
            Student oldStudent = GetStudentById(stu.StudentId);

            IEnumerable<PropertyInfo> variances = oldStudent.GetVariance(stu);

          
            List<Hashtable> ht = new List<Hashtable>();
            foreach (PropertyInfo pi in variances)
            {
                if (pi.Name.Equals( "ChangeReason") ||  pi.Name.Equals( "ChangeDescription"))
                {

                }
                else
                {
                    Hashtable hashTable = new Hashtable();
                    hashTable.Add("ColumnName", pi.Name);
                    hashTable.Add("RecordId", stu.StudentId);
                    hashTable.Add("NewValue", pi.GetValue(stu));
                    hashTable.Add("OldValue", pi.GetValue(oldStudent));
                    ht.Add(hashTable);
                }
               
                //var ColumnName = pi.Name;
                //var PreValue = pi.GetValue(stu);
                //var OldValue = pi.GetValue(oldStudent);
            }


            RecordAudit("Student", stu, ht);


            if (connection != null && connection.State == ConnectionState.Closed)
                connection.Open();
               
                string queryUpdate = "Update Student set FullName='"+stu.FullName+"' where StudentId='"+stu.StudentId+"'";

                using (SqlCommand cmd = new SqlCommand(queryUpdate, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();

            return RedirectToAction("Index");
        }

        public Student GetStudentById(int Id)
        {
            Student st = new Student();
            string query = "select * from Student where StudentId='" + Id.ToString() + "'";

            SqlCommand cmd = new SqlCommand(query, connection);

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                  
                    st.FullName = reader["FullName"].ToString();
                    st.StudentId = int.Parse(reader["StudentId"].ToString());
                   
                }
            }
            return st;
        }


    
        public void RecordAudit(string TableName,Student st,List<Hashtable> hashtable)
        {
            string xmlResult = "<AuditLog>";
            xmlResult += "<ChangeTitle>"+st.ChangeReason+"</ChangeTitle>";
            xmlResult += "<ChangeDescription>"+st.ChangeDescription+"</ChangeDescription>";
            xmlResult += "<TableName>"+TableName+"</TableName>";
            xmlResult += "<PageName>Test</PageName>";
            xmlResult += "<CreatedBy>1</CreatedBy>";
            xmlResult += "<CreatedDate>"+DateTime.Now+"</CreatedDate>"; 
            xmlResult += "</AuditLog>";
            xmlResult += "<AuditLogDetails>";
            foreach (var item in hashtable)
            {
                xmlResult += "<Details>";
                foreach (DictionaryEntry DE in item)
                {
                    xmlResult += "<"+DE.Key+">" + DE.Value + "</"+DE.Key+">";
                }
                xmlResult += "</Details>";
            }
            xmlResult += "</AuditLogDetails>";

            
                using (SqlCommand cmd = new SqlCommand("usp_Audit_Insert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@xmlDetails", SqlDbType.VarChar).Value = xmlResult;
                    if (connection != null && connection.State == ConnectionState.Closed)
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            
            //foreach ( DictionaryEntry item in hashtable)
            //{
            //    xmlResult +="<ColumnName>"+ item. + "</ColumnName>";
            //    xmlResult += "<OldValue>"+ item.Value + "</OldValue>";
            //    xmlResult += "<NewValue>"++"</NewValue>";


            //}
        }

    }



   
}