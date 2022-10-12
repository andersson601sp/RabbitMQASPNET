using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using application.Entities;
using application.Interface;
using RabbitMQ.Client.Events;
using application.Infra;

namespace application.Service
{
    public class ProductService : IProductService
    {
        private readonly string _host = EnvConfig.RabbitMQHost();
        public ProductService()
        {
            
        }

        public int Generate(Productor request)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _host };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: request.GetType().ToString(),
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = JsonConvert.SerializeObject(request);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: request.GetType().ToString(),
                                         basicProperties: null,
                                         body: body);
                }

                return 200;
            }
            catch (Exception)
            {

                return 404;
            }
        }

        public void Receive()
        {
            
            var factory = new ConnectionFactory() { HostName = _host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var receive = new Productor().GetType().ToString();
                channel.QueueDeclare(queue: receive,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var _product = JsonConvert.DeserializeObject<Productor>(message);
                        Console.WriteLine(" [x] Received {0}", message);

                        //apaga da fila
                        //  channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        // log ex - Nack devolve para fila
                        // BasicNack - terceiro paramentro true permite re-tentativa
                        // channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };
                channel.BasicConsume(queue: receive,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        #region
        private static async Task<string> RaedAsync()
        {
            string message = "fila vazia";
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var receive = new Productor().GetType().ToString();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: receive,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                message = await ReceiveAsync(message, consumer);
                channel.BasicConsume(queue: receive,
                                     autoAck: false,
                                     consumer: consumer);

                 return message;
            }
        }

        private static async Task<string> ReceiveAsync(string message, EventingBasicConsumer consumer)
        {
             consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);

                    //apaga da fila
                    //  channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    // log ex - Nack devolve para fila
                    // BasicNack - terceiro paramentro true permite re-tentativa
                    // channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };
            return message;
        }

        public static void Receive(IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                var receive = new Productor().GetType().ToString();
                channel.QueueDeclare(queue: receive,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        //apaga da fila
                        //  channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        // log ex - Nack devolve para fila
                        // BasicNack - terceiro paramentro true permite re-tentativa
                        // channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };
                channel.BasicConsume(queue: receive,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        #endregion
    }
}
