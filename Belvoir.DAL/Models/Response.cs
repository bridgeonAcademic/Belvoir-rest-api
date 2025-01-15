namespace Belvoir.DAL.Models
{
    public class Response<T>
    {
        public int statuscode { get; set; }
        public string message { get; set; }

        public string error { get; set; }

        public T data { get; set; }


    }

    public class UserAndCount
    {
        public IEnumerable<User> data { get; set; }
        public int count {  get; set; } 
    }

}
