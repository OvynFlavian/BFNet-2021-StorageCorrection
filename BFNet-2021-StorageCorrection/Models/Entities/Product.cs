using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Models.Entities
{
    public class Product: TableEntity
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public bool IsPromoted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
