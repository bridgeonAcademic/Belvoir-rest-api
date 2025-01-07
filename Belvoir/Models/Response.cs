namespace Belvoir.Models
{
    public class Response<T>
    {
        public int statuscode { get; set; }
        public string message { get; set; }

        public string error { get; set; }

        public T data { get; set; }


    }

}
