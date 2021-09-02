using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIKafka.ViewModel
{
    public class RequestViewModel
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public double Cotacao { get; set; }
        public int Partition { get; set; }
        public string Status { get; set; }
        public long Offset { get; set; }
        public int TopicPartition { get; set; }
    }
}
