using System.ComponentModel.DataAnnotations;

namespace LenteraKreasiTeknologiTest.Models
{
    public class EmployeeModel
    {
        [Required]
        [Display(Name = "NIK")]
        public string Nik { get; set; }
        [Required]
        [Display(Name = "Nama")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Alamat")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Tanggal Lahir")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        [Display(Name = "Jenis Kelamin")]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Departement")]
        public string Departement { get; set; }
        [Required]
        [Display(Name = "Jabatan")]
        public string Jabatan { get; set; }
    }
}
