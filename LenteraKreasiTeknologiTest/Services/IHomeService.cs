using LenteraKreasiTeknologiTest.Models;

namespace LenteraKreasiTeknologiTest.Services
{
    public interface IHomeService
    {
        public Task<string> Login(LoginModel loginModel);
        public Task<IndexEmployeeListModelView> GetAllEmployees(GetAllEmployeeSearchModel searchModel);
        public Task<CreateUpdateEmployeeModel> GetEmployeeById (string nIK);
        public Task<bool> DeleteEmployeeById(string nIK);
        public Task<bool> CreateEmployee(CreateUpdateEmployeeModel createEmployeeModel);
        public Task<bool> UpdateEmployee(CreateUpdateEmployeeModel updateEmployeeModel, string nIK);
        public Task<CreateUpdateEmployeeModel> GetAllDropdown();

    }
}
