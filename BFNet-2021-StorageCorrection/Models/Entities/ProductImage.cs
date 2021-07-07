using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Models.Entities
{
    public class ProductImage: TableEntity
    {
        public string Path { get; set; }
    }
}
