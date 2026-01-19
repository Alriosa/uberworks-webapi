namespace WebApi.Models
{
    public class User
    {
        public int userID { get; set; } //Primary Key
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string role { get; set; } = "CLIENT"; //'CLIENT','PROFESSIONAL','ADMINISTRATOR','SUPPORT'.
    }
}
