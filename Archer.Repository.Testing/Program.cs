using Archer.Extension.SecurityHelper;
using Archer.Repository;
using Uitc.Repository.Testing;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Archer.Repository.Testing Start...");

const string keyConn = "keyConn";
const string ivConn = "ivConn";
const string keyData = "keyData";
const string ivData = "ivData";
SecurityHelper securityHelper = new SecurityHelper(keyConn, ivConn, keyData, ivData);

Repository repository = new Repository("", securityHelper);

var loans = repository.Query<Loan>();

var groupCodes = repository.Query<GroupCodes>("SELECT * FROM GroupCode;");

Console.ReadLine();