using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;

namespace TestOne.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmpController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmp()
        {
            using var connect = new SqlConnection(_config.GetConnectionString("mycon"));
            var emp = await connect.QueryAsync<Employee>("select * from Employee where Active='A'");
            return Ok(emp);
        }
        [HttpGet("{eId}")]
        public async Task<ActionResult<List<Employee>>> GetEmp(int eId)
        {
            using var connect = new SqlConnection(_config.GetConnectionString("mycon"));
            var emp = await connect.QueryFirstAsync<Employee>("select * from Employee where Id=@Id and Active='A'",
            new { Id = eId });
            return Ok(emp);
        }
        [HttpPost]
        public async Task<ActionResult<List<Employee>>> PostEmp(Employee emp)
        {
            using var connect = new SqlConnection(_config.GetConnectionString("mycon"));
            await connect.ExecuteAsync("insert into Employee values(@Name,@Age,@Position,@CompanyName,'A')", emp);
            return Ok(await SelectAllEmp(connect));
        }
        private static async Task<IEnumerable<Employee>> SelectAllEmp(SqlConnection connect)
        {
            return await connect.QueryAsync<Employee>("select * from Employee where Active = 'A'");
        }
        [HttpPut]
        public async Task<ActionResult<List<Employee>>> PuttEmp(Employee emp)
        {
            using var connect = new SqlConnection(_config.GetConnectionString("mycon"));
            await connect.ExecuteAsync("update  Employee set Name=@Name,Age=@Age,Position=@Position,CompanyName=@CompanyName,Active='A' where Id=@Id", emp);
            return Ok(await SelectAllEmp(connect));
        }

        //Here I use  Put Method instead of Delete Method, (changed Active='D')
        //Because for not deleting the data from the database
        //but we cant see the data when we call the function GetAllEmp()
        //because of the condition "select * from Employee where Active='A'"

        [HttpDelete]
        public async Task<ActionResult<List<Employee>>> DeleteEmp(Employee emp)
        {
            using var connect = new SqlConnection(_config.GetConnectionString("mycon"));
            await connect.ExecuteAsync("update  Employee set Name=@Name,Age=@Age,Position=@Position,CompanyName=@CompanyName,Active='D' where Id=@Id", emp);
            return Ok(await SelectAllEmp(connect));
        }


    }
    
}
