using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace FormFilling.Models
{
    [Table("SigData")]
    public class SigData
    {
        [Key]
        public int EmployeeID { get; set; }
        public string Signature { get; set; } = "";

        public const int DefaultSignature = 0;
    }
}
