using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaApi.Models
{
    public class VillaNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }

        [ForeignKey("Villa")]
        public int VillaID { get; set; }

        public Villa Villa { get; set; } // // Associated navigation property

        public string SpecialDetails { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
    }
}
