using application.Service;

namespace Consumer
{
    public class ConsumerService
    {
        public void Run()
        {
            var service = new ProductService();
            service.Receive();
        }
    }
}
