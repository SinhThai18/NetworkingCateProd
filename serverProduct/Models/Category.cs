using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace serverProduct.Models
{
    public partial class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // trường RowVersion để làm concurrency token
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
