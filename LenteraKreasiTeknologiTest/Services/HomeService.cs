using LenteraKreasiTeknologiTest.Models;
using System.Text.Json;
using System.Text;
using System.Reflection;
using Npgsql;
using Dapper;
using System.Transactions;
using System.Data;
using System.Data.Common;
using System;

namespace LenteraKreasiTeknologiTest.Services
{
    public class HomeService : IHomeService
    {
        readonly string _loginURL = "";
        readonly IConfiguration _configuration;
        readonly NpgsqlConnection npgsqlConnection;

        public IDbConnection? Connection => throw new NotImplementedException();

        public System.Data.IsolationLevel IsolationLevel => throw new NotImplementedException();

        public HomeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _loginURL = _configuration.GetSection("IdentityServer:URL").Value + "/login";
            npgsqlConnection = new NpgsqlConnection(_configuration.GetConnectionString("connstring"));
        }

        public async Task<bool> CreateEmployee(CreateUpdateEmployeeModel createEmployeeModel)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var db = npgsqlConnection)
                    {
                        try
                        {
                            db.Open();
                            //get departement id
                            var insertDepartemen = $@"SELECT id FROM ""departemen"" WHERE name = '{createEmployeeModel.Employee.Departement}'";
                            var executionDepartement = await db.QueryAsync<int>(insertDepartemen);
                            var newDepartementId = executionDepartement.FirstOrDefault();


                            //get jabatan id
                            var insertJabatan = $@"SELECT id FROM ""jabatan"" WHERE name = '{createEmployeeModel.Employee.Jabatan}'";
                            var executionJabatan = await db.QueryAsync<int>(insertJabatan);
                            var newJabatanId = executionJabatan.FirstOrDefault();


                            //insert employee
                            var insertKaryawan = $@"INSERT INTO ""karyawan"" (nik, name, address, birthdate, gender, departemen_id, jabatan_id) 
                                                    VALUES ('{createEmployeeModel.Employee.Nik}', '{createEmployeeModel.Employee.Name}', '{createEmployeeModel.Employee.Address}', '{createEmployeeModel.Employee.BirthDate}', '{createEmployeeModel.Employee.Gender}', {newDepartementId}, {newJabatanId})";
                            var executionKaryawan = await db.ExecuteAsync(insertKaryawan);

                            transactionScope.Complete();
                            return executionKaryawan > 0;
                        }
                        catch
                        {
                            transactionScope.Dispose();
                            return false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployee(CreateUpdateEmployeeModel updateEmployeeModel, string nIK)
        {
            try
            {
                using (var db = npgsqlConnection)
                {
                    //update employee
                    var insertKaryawan = $@"UPDATE ""karyawan"" 
                                            SET nik = '{updateEmployeeModel.Employee.Nik}',
                                            name = '{updateEmployeeModel.Employee.Name}',
                                            address = '{updateEmployeeModel.Employee.Address}',
                                            birthdate = '{updateEmployeeModel.Employee.BirthDate}',
                                            gender = '{updateEmployeeModel.Employee.Gender}',
                                            departemen_id = '{updateEmployeeModel.Employee.Departement}',
                                            jabatan_id = '{updateEmployeeModel.Employee.Jabatan}'
                                            WHERE nik = '{nIK}'";

                    var execution = await db.ExecuteAsync(insertKaryawan);
                    return execution > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeById(string nIK)
        {
            try
            {
                using (var db = npgsqlConnection)
                {
                    //update employee
                    var deleteKaryawan = $@"DELETE FROM ""karyawan"" 
                                            WHERE nik = '{nIK}'";

                    var execution = await db.ExecuteAsync(deleteKaryawan);
                    return execution > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IndexEmployeeListModelView> GetAllEmployees(GetAllEmployeeSearchModel searchModel)
        {
            try
            {
                var result = new IndexEmployeeListModelView();
                using (var db = npgsqlConnection)
                {
                    //employee list
                    var query = @"SELECT k.nik as NIK, k.name as Name, k.address as Address, k.birthdate as BirthDate, k.gender as Gender, d.name as Departement, j.name as Jabatan FROM ""karyawan"" as k
                                    LEFT JOIN ""departemen"" as d on d.id = k.departemen_id
                                    LEFT JOIN ""jabatan"" as j on j.id = k.jabatan_id ";

                    var conditions = @" WHERE ";

                    if (!String.IsNullOrEmpty(searchModel.Name))
                    {
                        conditions += $" LOWER(k.name) LIKE '%{searchModel.Name.ToLower()}%' AND";
                    }

                    if (!String.IsNullOrEmpty(searchModel.Department))
                    {
                        conditions += $" LOWER(d.name) = '{searchModel.Department.ToLower()}' AND";
                    }


                    if (!String.IsNullOrEmpty(searchModel.Jabatan))
                    {
                        conditions += $" LOWER(j.name) = '{searchModel.Jabatan.ToLower()}' AND";
                    }

                    if (!String.IsNullOrEmpty(searchModel.Gender))
                    {
                        conditions += $" LOWER(k.gender) = '{searchModel.Gender.ToLower()}' AND";
                    }

                    if(conditions.Length > 7)
                    {
                        query += conditions;
                        query = query.Substring(0, query.Length - 3);
                    }

                    var orderByName = @" order by k.name";
                    query += orderByName;
                    var execution = await db.QueryAsync<EmployeeModel>(query);
                    var employees = execution.ToList();
                    result.EmployeeList = employees;

                    //departements
                    var queryDepartement = @"SELECT * FROM ""departemen""";
                    var executionDepartement = await db.QueryAsync<DepartementModel>(queryDepartement);
                    var departement = executionDepartement.ToList();
                    result.DepartmentList = departement;

                    //departements
                    var queryJabatan = @"SELECT * FROM ""jabatan""";
                    var executionJabatan = await db.QueryAsync<JabatanModel>(queryJabatan);
                    var jabatan = executionJabatan.ToList();
                    result.JabatanList = jabatan;
                }

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<CreateUpdateEmployeeModel> GetEmployeeById(string nIK)
        {
            try
            {
                var newResult = new CreateUpdateEmployeeModel();
                using (var db = npgsqlConnection)
                {
                    //get employee
                    var getEmployee = $@"SELECT k.nik as NIK, k.name as Name, k.address as Address, k.birthdate as BirthDate, k.gender as Gender, d.name as Departement, j.name as Jabatan FROM karyawan as k
                                    LEFT JOIN departemen as d on d.id = k.departemen_id
                                    LEFT JOIN jabatan as j on j.id = k.jabatan_id where k.nik = '{nIK}'";

                    var execution = await db.QueryAsync<EmployeeModel>(getEmployee);
                    newResult.Employee = execution.FirstOrDefault();

                    //departements
                    var queryDepartement = @"SELECT * FROM ""departemen""";
                    var executionDepartement = await db.QueryAsync<DepartementModel>(queryDepartement);
                    var departement = executionDepartement.ToList();
                    newResult.DepartmentList = departement;

                    //departements
                    var queryJabatan = @"SELECT * FROM ""jabatan""";
                    var executionJabatan = await db.QueryAsync<JabatanModel>(queryJabatan);
                    var jabatan = executionJabatan.ToList();
                    newResult.JabatanList = jabatan;

                    return newResult;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<CreateUpdateEmployeeModel> GetAllDropdown()
        {
            try
            {
                var newResult = new CreateUpdateEmployeeModel();
                using (var db = npgsqlConnection)
                {
                    //departements
                    var queryDepartement = @"SELECT * FROM ""departemen""";
                    var executionDepartement = await db.QueryAsync<DepartementModel>(queryDepartement);
                    var departement = executionDepartement.ToList();
                    newResult.DepartmentList = departement;

                    //departements
                    var queryJabatan = @"SELECT * FROM ""jabatan""";
                    var executionJabatan = await db.QueryAsync<JabatanModel>(queryJabatan);
                    var jabatan = executionJabatan.ToList();
                    newResult.JabatanList = jabatan;

                    return newResult;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<string> Login(LoginModel loginModel)
        {
            using (var client = new HttpClient())
            {
                var body = JsonSerializer.Serialize(loginModel);
                var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_loginURL, requestContent);

                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return res;
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return res;
                }
            }
        }

    }
}
