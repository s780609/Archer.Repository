using Archer.Extension.SecurityHelper;
using Archer.Repository;
using Archer.Repository.Testing;
using Microsoft.Extensions.Configuration;
using System.Text;

Console.WriteLine("Repository.Testing Start...");

string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddEnvironmentVariables()
    .Build();

string defaultConnection = config.GetConnectionString("UbotLoan");

string keyConn = config.GetValue<string>("keyConn");
string ivConn = config.GetValue<string>("ivConn");
string keyData = config.GetValue<string>("keyData");
string ivData = config.GetValue<string>("ivData");

SecurityHelper securityHelper = new SecurityHelper(keyConn, ivConn, keyData, ivData);

Repository repository = new Repository(defaultConnection, securityHelper);

var loans = repository.Query<Loan>();

var groupCodes = repository.Query<GroupCodes>("SELECT * FROM GroupCode;");

var users = repository.Query<Users>(param: new { Account = "123456" });

repository.Create(new Users
{
    Name = "測試CREATE",
    Account = "t1123456",
    AccessCode = Encoding.UTF8.GetBytes("123456"),
    UpdateDate = DateTime.Now,
    CreateDate = DateTime.Now,
    Enable = true,
    BranchNo = "907",
    System = 1,
    Group = 0,
    Department = "1",
    IsAssigned = true,
    CmsSystem = 1,
    Role = 1,
});

repository.Update<Users>(new
{
    Group = 2,
},
new
{
    Name = "測試CREATE",
    Account = "t1123456",
});

repository.Delete<Users>(new
{
    Name = "測試CREATE",
    Account = "t1123456",
});

Console.ReadLine();