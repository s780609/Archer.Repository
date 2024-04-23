# Archer.Repository
用來幫忙產生並執行`SQL script`於SQL Server的套件

## 用法
```
using Archer.Repository

Repository repository = new Repository("your connection string", securityHelper);

var loans = repository.Query<Loan>();

var groupCodes = repository.Query<GroupCodes>("SELECT * FROM GroupCode;");
```