using System.ComponentModel.DataAnnotations;

namespace ACB_Prueba_Tecnica.Domain.Entities
{
    public class MatchEvent
    {
        [Key]
        public int Id { get; set; }
        public int IdGame { get; set; }
        public string PlayByPlayData { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
