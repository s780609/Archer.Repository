using Archer.Extension.SecurityHelper;
using Archer.Repository;
using Uitc.Repository.Testing;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Archer.Repository.Testing Start...");

const string keyConn = "     ";
const string ivConn = "      ";
const string keyData = "     ";
const string ivData = "      ";
SecurityHelper securityHelper = new SecurityHelper(keyConn, ivConn, keyData, ivData);

Repository repository = new Repository("", securityHelper);

var loan = repository.Query<Loan>(param: new { LoanID = "20240517A2205193254001" }).Single();

var groupCodes = repository.Query<GroupCodes>("SELECT * FROM GroupCode;");

Console.ReadLine();