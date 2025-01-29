namespace Belvoir.DAL.Models
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public string Error { get; set; }

        public T Data { get; set; }


    }

    public class UserAndCount
    {
        public IEnumerable<User> data { get; set; }
        public CountUser count { get; set; }
    }

}
