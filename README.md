# Archer.Repository
用來幫忙產生並執行`SQL script`於SQL Server的套件

## API
|  名稱   | 說明   | 泛型 |  參數 |
|  ----  | ----  | ---- | ---- |
| Execute  | 執行SQL  | Table  | sql, param  |
| Query  | 查詢  | Table  | sql, param  |
| QuerySingle  | 單一查詢  | Table  | sql, param  |
| Create  | 新增  | Table  | model |
| Update  | 更新 | Table  | model, key  |
| Delete  | 刪除  | Table  | model  |

## 用法範例
引入並建立連線
```C#
using Archer.Repository

Repository repository = new Repository("your connection string", securityHelper);
```

根據要查詢的table，建立model class，並且記得都要預設為 null
```C#
public class Users
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Account { get; set; }
}
```

查詢
```C#
var loans = repository.Query<Loan>();

var groupCodes = repository.Query<GroupCodes>("SELECT * FROM GroupCode;");

var users = repository.Query<Users>(param: new { Account = "123456" });
```

新增
```C#
repository.Create(new Users
{
   Name = "測試CREATE",
   Account = "t1123456",
});
```

更新
```C#
repository.Update<Users>(new
{
    Group = 2,
},
new
{
    Name = "測試CREATE",
    Account = "t1123456",
});
```

刪除
```C# 
repository.Delete<Users>(new
{
    Name = "測試CREATE",
    Account = "t1123456",
});
```