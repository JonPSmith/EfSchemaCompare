# EfSchemaCompare

The SchemaCompareDb package is designed to compare what Microsoft's
[Entity Framework](https://msdn.microsoft.com/en-gb/data/ef.aspx) (EF)
thinks the database should look like against what an actual Microsoft SQL Server database
schema contains. This is useful if you want to:

1. Take over the creation, definition or migration of the database.
2. You want to build a EF model that works with an existing database.

You can read a quick introduction to EfSchemaCompare
[here](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-4-release-of-efschemacompare/)
while this file contains the full documentation of all the methods and settings.

**EfSchemaCompare is an open-source project 
[(MIT licence)](https://github.com/JonPSmith/EfSchemaCompare/blob/master/licence.txt)
and is available on NuGet as
[EfSchemaCompare.EF6](https://www.nuget.org/packages/EfSchemaCompare.EF6/)**

## Why I built this project?
I was working on an e-commerce web site and was thinking through the problems of applying database migrations
to a production site. My concern was that EF's built-in
[data migration](https://msdn.microsoft.com/en-gb/data/jj591621)
approach is fine for development and some projects, but in a production environment,
where an error during a database migration could cost you some serious down time,
I needed a better approach.

You can see how I used EfSchemaCompare in my own application
[in this article](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-4-release-of-efschemacompare/).
I wrote a long article called
['Deploying an Entity Framework Database into Production'](https://www.simple-talk.com/dotnet/.net-framework/deploying-an-entity-framework-database-into-production/)
where I describe why and how I build SchemaCompareDb. This is a good article to read to get an overview
of SchemaCompareDb.

I have also have started a series on
[database migrations](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-1-applying-the-updates/)
on my own blog site which covers the same area, but with a bit more detail.

# Current limitations

1. CompareSqlToSql does not check on [Stored Procedures](https://msdn.microsoft.com/en-us/data/jj593489) at all.
*Not hard to add, but I don't need it at the moment. 
If you want it then happy for you to do a Pull Request and add it yourself.*
2. CompareSqlToSql does not check the [default contraint](http://www.w3schools.com/sql/sql_default.asp) on columns.
*Again, not hard to add but I haven't had a problem with this, although one of the testers has. 
Again, Pull request if you want to add that.*
3. Minor point, but EF 6 create two indexes on one end of a ZeroOrOne to Many relationships.
Currently I just report on what indexes EF has, but I'm not sure having both a clustered and non-clustered
index on the same column is necessary. *Let me know if I'm wrong on that!*
5. SchemaCompareDb does not support the complex type-to-table mappings options.
I found it is very difficult (impossible!) in EF6 to find that information in the EF model data.
The list of complex type-to-table mappings NOT supported are:
  * [table-per-type (TPT) inheritance](https://msdn.microsoft.com/en-us/data/jj618293) mapping.
  * [table-per-hierarchy (TPH) inheritance](https://msdn.microsoft.com/en-us/data/jj618292) mapping.
  * [Map an Entity to Multiple Tables](https://msdn.microsoft.com/en-us/data/jj715646).
  * [Map Multiple Entities to One Table](https://msdn.microsoft.com/en-us/data/jj715645).
4. Currently no support for [Entity Framework Core](https://github.com/aspnet/EntityFramework/wiki),
previously known as EF7.

# How to use SchemaCompareDb

There are three main ways of comparing EF and databases:

1. **CompareEfWithDb**: Compare EF's DbContext(s) against an actual SQL database.
2. **CompareEfGeneratedSqlToSql**: Compare a database created by EF against an actual SQL database.
3. **CompareSqlToSql**: Compare one SQL database against another SQL database.

I use all three (see 
[this article for examples](http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-4-release-of-efschemacompare/)):
The first gives the best error messages but cannot check all possible combinations (in EF6 anyway).
The second covers 100% of the EF differences, but the errors are more SQL-centric so sometimes harder
to relate to the EF code.
The last one, CompareSqlToSql, is really quick and useful to check that all of your databases are at
the same level.



## The difference between Errors and Warnings

All the methods return an Interface called 
[`ISuccessOrErrors`](https://github.com/JonPSmith/GenericServices/blob/master/GenericLibsBase/ISuccessOrErrors.cs).
This has two components, Errors and Warnings, and its really important to understand
what each is used for so that you can interpret the output sensibly.

### Errors

The returned `ISuccessOrErrors` has a boolean property called `IsValid` that is true if there 
are no errors (but there could be warnings). If there are errors then they can be accessed via 
the `Errors` property, which is a `IReadOnlyList<ValidationResult>`. The actual error message 
on each entry can be accessed by either `.ErrorMessage` or `.ToString()`;

Errors are things that EfSchemaCompare believes will stop EF working properly with your database.
Typical issues are missing tables, columns, relationships, indexes or a mismatch in 
type, size etc of a column or relationship.

### Warnings

The returned `ISuccessOrErrors` has a boolean property called `HasWarnings` that is true if there 
are warnings. If there are warnings then they can be accessed via 
the `Warnings` property, which is a `IReadOnlyList<string>`

Warnings are differences between the two databases which EfSchemaCompare believes should not
cause problems to EF. Typically they are:
- Extra tables in SQL that EF does not refer to - these are normally safe.
- Columns in a SQL table that EF does not refer to (in some cases these can cause problems, especially on create/update).
- The size of a string (varchar, nvarchar) or other type with a length is at max in SQL but not at max in EF.
*This can happen when you have a `[MaxLength(nn)]` setting on an EF column, but the size is over the 
point where SQL makes it max length.*
- When doing a SQL-to- SQL compare then indexes in the second, **database to be checked**,
but not in the **reference database**.

Later you will also see that
you can relegate differences in Indexes from errors to warnings, as EF adds lots of Indexes.
(see [`CompareSqlSql` ctor](#2-comparesqlsql))

However, if you want check for an **exact** match between two databases you should check that
`.IsValid` is true and `.HasWarnings` is false.
     

# Detailed description of each of the commands.

## 1. `CompareEfSql`

The `CompareEfSql` class is used to compare EF's DbContext(s) againts a SQL database. 
This catches 90% of issues and gives good, EF centric, error messages. It is slower than
the `CompareSqlSql`, especially when calling the `CompareEfGeneratedSqlToSql` version, but
the detail of this errors are worth having.

`CompareEfSql` can work with a single DbContext that covers the whole of the database, or multiple
DbContexts that cover different parts of the database.

The `CompareEfSql` ctor has an optional parameter
which takes a comma delimited list of tables in the SQL database to ignore when looking
for missing tables. Its default value is `"__MigrationHistory,SchemaVersions"`, which ignores
the "__MigrationHistory" that EF uses and the "SchemaVersions" table that
[DbUp](http://dbup.readthedocs.org/en/latest/) adds. 
*Note: DbUp is my chosen way of handling data migrations.*


### 1.a: EF classes in the same assembly as DbContext

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

### 1.b: EF classes in a different assembly

If you have your EF data classes in an separate assembly to your DbContext (I do)
then you need to use the form that takes a Type, which should be one of your EF data classes.
It uses this type to find the right assembly to scan for the data classes.
*Note: this cannot handle data classes in multiple assembly.*

```
var status = comparer.CompareEfWithDb<AnEfDataClass>(db);
```


### 1.c: Compare EF with different database

If you want compare EF with another database then you provide a second parameter, which should be the
name of a connection string in your App.Config/Web.Config, or a actual connection string, e.g.

```
var status = comparer.CompareEfWithDb(db, AConnectionStringName);
```


### 1.d: Compare multiple EF DbContexts with a database

Sometimes you may want to split the coverage of the database over multiple DbContexts.
In this case we can support this (with some limitations that I explain later). 
This works in three stages:

1. **Setup**: call `CompareEfPartStart(db)` or `CompareEfPartStart(AConnectionStringName)` to setup the compare.
2. **Compare**: call `CompareEfPartWithDb(db)` (or other variants) for each DbContext
3. **Finalise**: call `CompareEfPartFinalChecks()` to do a final check for unused tables.

Below is an example where two DbContexts, DbContext1 and DbContext2, cover the same database.
In this example I combine all the errors so I can check them at the end, but you can
check each one as you go if you like.


```
var comparer = new CompareEfSql();
ISuccessOrErrors status;

using (var db = new DbContext1())
{
    comparer.CompareEfPartStart(db);
    status = comparer.CompareEfPartWithDb(db);
}
using (var db = new DbContext2())
{
    status.Combine(comparer.CompareEfPartWithDb(db));
}
status.Combine(comparer.CompareEfPartFinalChecks());
//Now check the errors and warnings
```

#### Limitations on CompareEfParts

The only limitation is that the standard calls shown above assume that there is no overlap 
of the classes/tables that each DbContext references, e.g. DbContext1 and DbContext2 cannot 
both have an EF data class called MyClass. This is because one reference to a class by one
DbContext removes it from the list of available tables, so the second reference will fail.

If you do share classes between DbContexts then you should create a new `CompareEfSql` for
each DbContext and call just `CompareEfPartStart` and `CompareEfPartWithDb` and not call
`CompareEfPartFinalChecks`. You will miss out on some tests, like unused tables, but other
than that it will work OK.

## 1.e. CompareEfGeneratedSqlToSql

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

# 2. `CompareSqlSql`:

The `CompareSqlSql` class is used for comparing one SQL database against another. It is quicker and
more detailed than `CompareEfSql`, but the error messages are harder to relate to EF, as it doesn't
know anytheing about the EF classes.

The `CompareSqlSql` ctor, which is used by `CompareEfGeneratedSqlToSql` and `CompareSqlToSql` has
two optional parameters. They are:

1. **showMismatchedIndexsAsErrors** (default true). Normally differences in indexes will show as errors
but EF is rather heavy handed at adding non-clustered indexes, i.e. adds them on every foreign key.
You may therefore not add all the indexes EF does and therefore don't want an index mismatch to show
as an errors. Setting this to false means they show up as warnings.
2. **SQLTableNamesToIgnore** (default "__MigrationHistory,SchemaVersions"). These are the tables that it won't
complain about if the database referred to in the second parameter hasn't got them.

## 2.a. CompareSqlToSql

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


# Telling Entity Framework that you will handle migrations

If you are taking over the handling of database migrations then you turn off EF's
database migrations handling. If you don't turn EF's migration handling off
then when you change EF database classes EF will block you from running the application.

To stop EF trying to handle migrations we have to do is provide a null database Initializer.
There are two ways of doing this (you only need one):

1. Call `SetInitializer<YourDbContext>(null)` at startup. This adds what is known as the EF null initializer.
2. Add the following to the `<appSettings>` part of your web/application config file, e.g.

```XML
<appSettings>
    <add key="DatabaseInitializerForType YourApp.YourDbContext, YourApp" value="Disabled" />
</appSettings>
```

I personally add the appSetting to my config file as that way I know it is done.

Note: See this [useful documentation](http://www.entityframeworktutorial.net/code-first/turn-off-database-initialization-in-code-first.aspx)
for more on null database Initializers.

