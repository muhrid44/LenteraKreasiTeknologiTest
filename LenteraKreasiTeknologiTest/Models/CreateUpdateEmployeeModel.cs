using System.ComponentModel.DataAnnotations;

namespace LenteraKreasiTeknologiTest.Models
{
    public class CreateUpdateEmployeeModel
    {
        public EmployeeModel Employee { get; set; }
        [Display(Name = "Departement")]
        public List<DepartementModel> DepartmentList { get; set; }
        [Display(Name = "Jabatan")]
        public List<JabatanModel> JabatanList { get; set; }
    }
}
