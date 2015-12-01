#region licence
// =====================================================
// CompareEfSql Project - project to take over ef migrations
// Filename: DbUpRunner.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DbUp;
using GenericLibsBase;
using GenericLibsBase.Core;

[assembly: InternalsVisibleTo("Tests")]

namespace DbUpHelper
{
    public class DbUpRunner
    {
        public ISuccessOrErrors ApplyMigrations(string dbConnectionString)
        {
            var status = new SuccessOrErrors();
            var upgrader = DeployChanges.To
                    .SqlDatabase(dbConnectionString)
                    .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithTransaction()
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                var msg = result.Scripts.Any()
                    ? "Successfully applied the last " + result.Scripts.Count() + " script(s) to the database."
                    : "No updates done to database.";
                return status.SetSuccessMessage(msg);
            }

            return status.HasErrors
                ? status
                : status.AddSingleError(result.Error.Message);
        }
    }
}