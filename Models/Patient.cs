using System.ComponentModel.DataAnnotations;

namespace HealthITDatim.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientDiagnosis { get; set;}
        public string HomeCounty { get; set; }
        public int delete_status  { get; set; }
        public int status { get; set; }
    }
}
