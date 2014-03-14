using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperEntities
{
    public class Performance : TableEntity
    {
        public Performance()
        {

        }

        public Performance(float ram, float cpu)
        {
            this.PartitionKey = "0";
            this.RowKey = "0";
            this.Ram = ram;
            this.CPU = cpu;

        }

        public float Ram { get; set; }

        public float CPU { get; set; }
    }
}
