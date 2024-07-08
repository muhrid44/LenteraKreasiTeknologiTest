# LenteraKreasiTeknologiTest

What's in this project ?
-  Using Dapper as ORM
-  Identity.sln is only for security (Authentication)
-  Using MVC Web .NET Core
-  Using Dependecy Injection design pattern
-  Database to code approachment
-  Not storing raw password to database instead storing the password hashing and salting

How to run project :
1.  in folder ScriptDatabase, contains 2 file (1 file for backup, 1 file for script)
2.  if backup failed, you can use ScriptLentera.sql and execute the script
3.  check your database, if all tables you need has already restored, you can move to next
4.  Open Identity.sln for security web API
5.  setup your own connection string database in appsettings.Development.json
6.  you can register your admin through Identity.sln web API or you cna use existing admin1 in database table passwordhashtable
7.  user : admin1, password : 123456
8.  run  Identity.sln
9.  Open LenteraKreasiTeknologiTest.sln for web MVC
10. setup your own connection string database in appsettings.Development.json
11.  install all packages required (if any missing packages)
12. run LenteraKreasiTeknologiTest.sln for web MVC

Note : Do not change any settings in appsettings.Development.json except for ConnectionString
