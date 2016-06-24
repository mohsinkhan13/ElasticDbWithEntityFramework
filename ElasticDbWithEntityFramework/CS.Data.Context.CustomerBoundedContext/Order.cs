namespace CS.Data.Context.CustomerBoundedContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public int ProductId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Product Product { get; set; }
    }
}
