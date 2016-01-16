# SchemaCompareDb

The SchemaCompareDb package is designed to compare what Microsoft's 
[Entity Framework](https://msdn.microsoft.com/en-gb/data/ef.aspx) (EF)
thinks the database should look like against what an actual Microsoft SQL Server database
schema contains. This is useful if you want to: 

1. Take over the creation, definition or migration of the database.
2. You want to build a EF model that works with an existing database.

# NOTE: This project is NOT for general release

1. It has a few limitations. see [Current Limitations](#Current-limitations) section.
2. It needs some more examples to show people how it works, especially if it is only
distributed as a NuGet package.
3. It is not a public domain project! I am reserving the licence on the code for now.


## Why I built this project?
I was working on an e-commerce web site and was thinking through the problems of applying database migrations
to a production site. My concern was that EF's built-in 
[data migration](https://msdn.microsoft.com/en-gb/data/jj591621) 
approach is fine for development and some projects, but in a production environment,
where an error during a database migration could cost you some serious down time,
I needed a better approach.

I wrote a long article called 
['Deploying an Entity Framework Database into Production'](https://www.simple-talk.com/dotnet/.net-framework/deploying-an-entity-framework-database-into-production/)
where I describe why and how I build SchemaCompareDb. This is a good article to read to get an overview 
of SchemaCompareDb.

I have also have started a series on 
[database migrations](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-1-applying-the-updates/)
on my own blog site which covers the same area, but with a bit more detail.

*Note: if you are thinking of taking over the migrations this you need to read the section near the end of
[this article](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-2-keeping-ef-and-sql-scheme-in-step/)
called 'Telling Entity Framework that you will handle migrations' which covers `Null database Initializers`*

# How to use SchemaCompareDb

There are three main ways of comparing EF and databases:

1. **CompareEfWithDb**: Compare EF's model against an actual SQL database. 
2. **CompareEfGeneratedSqlToSql**: Compare a database created by EF against an actual SQL database. 
3. **CompareSqlToSql**: Compare one SQL database against another SQL database. 

I use all three: The first gives the best error messages but cannot check all possible combinations (in EF6 anyway).
The second covers 100% of the EF differences, but the errors are more SQL-centric so sometimes hard to understand.
The last one, CompareSqlToSql, is really quick and useful to check that all of your databases are at
the same level.

What follows is a detailed description of each of the commands.

## 1. CompareEfWithDb

This compares EF's model against an actual SQL database. This catches 90% of issues and gives good, EF centric, error messages. 
The code below show a call to `CompareEfWithDb` that compares the EF internal model with the database 
that `YourDbContext` points to:


```
using (var db = new YourDbContext())
{
    var comparer = new CompareEfSql();
 
    var status = comparer.CompareEfWithDb(db);
    //status.IsValid is true if no errors.
    //status.Errors contains any errors. 
    //status.Warnings contains any warnings
}
```

*Note: All the compare methods return a ISuccessOrErrors result (you can find the 
[SuccessOrErrors code here](https://github.com/JonPSmith/GenericServices/blob/master/GenericLibsBase/Core/SuccessOrErrors.cs))*

The `CompareEfSql` has an optional parameter
which takes a comma delimited list of tables in the SQL database to ignore when looking
for missing tables. Its default value is `"__MigrationHistory,SchemaVersions"`, which ignores
the "__MigrationHistory" that EF uses and the "SchemaVersions" table that 
[DbUp](http://dbup.readthedocs.org/en/latest/) adds. *Note: DbUp is my chosen way of handling data migrations.*

There are two other variations of the `CompareEfWithDb` method call.

### 1.a: EF classes in a different assembly

If you have your EF data classes in an separate assembly to your DbContext (I do)
then you need to use the form that takes a Type, which should be one of your EF data classes. 
It uses this type to find the right assembly to scan for the data classes. 
*Note: it cannot handle data classes in multiple assembly.*

```
var status = comparer.CompareEfWithDb<AnEfDataClass>(db);
```


### 1.b: Compare EF with different database

If you want compare EF with another database then you provide a second parameter, which should be the
name of a connection string in your App.Config/Web.Config, or a actual connection string, e.g.

```
var status = comparer.CompareEfWithDb(db, AConnectionStringName);
```

## 2. CompareEfGeneratedSqlToSql

This version created a new EF database using `DbContext.Database.Create()` and then compares that
database against an actual SQL database. This catches 100% of the differences
(note: differences in the SQL that EF uses - it is not a full compare of SQL databases),
but errors are SQL based so a little harder to interpret. 

The code below will wipe/create a new database using the name of your database, but with `.EfGenerated`
added to the end. It compares this database with the second parameter, which should be the
name of a connection string in your App.Config/Web.Config, or a actual connection string.

```
using (var db = new YourDbContext())
{
    var comparer = new CompareSqlSql();

    var status = comparer.CompareEfGeneratedSqlToSql(db, AConnectionStringName);
    //status.IsValid is true if no errors.
    //status.Errors contains any errors. 
    //status.Warnings contains any warnings
}
```

The `CompareSqlSql` ctor has two optional parameters:

1. **showMismatchedIndexsAsErrors** (default true). Normally differences in indexes will show as errors
but EF is rather heavy handed at adding non-clustered indexes, i.e. adds them on every foreign key.
You may therefore not add all the indexes EF does and therefore don't want an index mismatch to show 
as an errors. Setting this to false means they show up as warnings.
2. **SQLTableNamesToIgnore** (default "__MigrationHistory,SchemaVersions"). These are the tables that it won't
complain about if the database referred to in the second parameter hasn't got them.

## 3. CompareSqlToSql

This final version compares one SQL database against another SQL database. This useful to check differences between say 
a production database and a development database. It is also very quick as it only uses SQL commands,
so you can include this at little cost in your unit tests.

The code below compares two databases. The first parameter holds the name of a connection string in your 
App.Config/Web.Config, or a actual connection string of the **reference database**. The second parameter
holds name/connection of the **database to be checked**.  

**NOTE: THE ORDER OF THE PARAMETERS IS REALLY IMPORTANT**:  
CompareSqlToSql will report **errors** for tables and/or columns that are in the 
database referred to in the first paramater (called the **reference database**),
but not in the database referred to in the second paramater (called the **database to be checked**).
If your reverse the order or the two parameters, 
then CompareSqlToSql will report **warnings**, not errors, for the same missing tables/columns. 

This is because it is valid for a database to have additional tables/columns 
that EF does not access, so the compare shows **extra** tables/columns in the database
referred to in the second parameter as warnings, not errors.

Below is a typical call to `CompareSqlToSql`. Note that The ctor is the same as used for
`CompareEfGeneratedSqlToSql`, so see last section for its optional parameters.

```
using (var db = new YourDbContext())
{
    var comparer = new CompareSqlSql();

    var status = comparer.CompareSqlToSql(refConnection, toBeCheckedConnection);

    //status.IsValid is true if no errors.
    //status.Errors contains any errors. 
    //status.Warnings contains any warnings
}
```

# Current limitations

1. It does not handle the case where you have multiple DbContext covering the same database.
It will show incorrect 'missing table' errors (and maybe other problems too - I haven' tried it).
2. Currently no support for EF7.
3. CompareSqlToSql does not check on [Stored Procedures](https://msdn.microsoft.com/en-us/data/jj593489) at all.
4. Minor point, but EF 6 create two indexes on one end of a ZeroOrOne to Many relationships. 
Currently I just report on what indexes EF has, but I'm not sure having both a clustered and non-clustered
index on the same column is necessary.

Also SchemaCompareDb will never support the complex type-to-table mappings options in EF 6 listed below.
I found it is very difficult (impossible!) in EF6 to find that information in the EF model data,
and EF7 does not currently plan to support these features in first release, or maybe never
(see [EF7, Section Bucket #4: Removal of features](http://blogs.msdn.com/b/adonet/archive/2014/10/27/ef7-v1-or-v7.aspx)).

The list of complex type-to-table mappings NOT supported are:

* [table-per-type (TPT) inheritance](https://msdn.microsoft.com/en-us/data/jj618293) mapping.
* [table-per-hierarchy (TPH) inheritance](https://msdn.microsoft.com/en-us/data/jj618292) mapping.
* [Map an Entity to Multiple Tables](https://msdn.microsoft.com/en-us/data/jj715646).
* [Map Multiple Entities to One Table](https://msdn.microsoft.com/en-us/data/jj715645).

