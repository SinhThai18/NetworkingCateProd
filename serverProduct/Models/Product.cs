using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace serverProduct.Models
{
    public partial class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int CatId { get; set; }

        public virtual Category? Cat { get; set; } = null!;

        //trường RowVersion để làm concurrency token
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
