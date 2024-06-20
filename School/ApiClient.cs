using SharedModels.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace School
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        public IRepository<StudentDto> Students { get; }
        public IRepository<AttendanceDto> Attendances { get; }
        public IUserRepository LoginUsers { get; }
        public ApiClient()
        {
            string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            _httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl)};
            Students = new Repository<StudentDto>(_httpClient, "Student");
            Attendances = new Repository<AttendanceDto>(_httpClient, "Attendance");
            LoginUsers = new UserRepository(_httpClient, "Auth/Login");
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
