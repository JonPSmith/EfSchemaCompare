// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Ef6CompareParts.cs
// Date Created: 2016/01/30
// © Copyright Selective Analytics 2016. All rights reserved
// =====================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using CompareCore;
using CompareCore.SqlInfo;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace Ef6Compare.InternalEf6
{
    internal class Ef6CompareParts
    {
        private readonly string _sqlTableNamesToIgnore;
        private string _sqlDbRefString;

        private SqlAllInfo _allSqlInfo;
        private Dictionary<string, SqlTableInfo> _sqlInfoDict;
        private EfCompare _comparer;

        public bool CompareStartCalled {  get { return _allSqlInfo != null; } }

        public Ef6CompareParts(string sqlTableNamesToIgnore)
        {
            _sqlTableNamesToIgnore = sqlTableNamesToIgnore;
        }


        public void CompareStart(string sqlDbRefString, string sqlConnectionString)
        {
            if (sqlConnectionString == null)
                throw new ArgumentNullException("sqlConnectionString");
            if (CompareStartCalled)
                throw new InvalidOperationException("You have already called CompareStart.");

            _sqlDbRefString = sqlDbRefString;
            _allSqlInfo = SqlAllInfo.SqlAllInfoFactory(sqlConnectionString);
            _sqlInfoDict = _allSqlInfo.TableInfos.ToDictionary(x => x.CombinedName);

            _comparer = new EfCompare(sqlDbRefString, _sqlInfoDict);
        }

        /// <summary>
        /// This compares the dbContext against the given sql database
        /// </summary>
        /// <param name="db"></param>
        /// <param name="classesAssembly"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfPart(DbContext db, Assembly classesAssembly)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            var decoder = new Ef6MetadataDecoder(classesAssembly);
            var efInfos = decoder.GetAllEfTablesWithColInfo(db);
            return _comparer.CompareEfWithSql(efInfos, _allSqlInfo);
        }

        /// <summary>
        /// Run this at the end to spot unused tables.
        /// </summary>
        /// <returns></returns>
        public ISuccessOrErrors CompareFinish()
        {
            var status = SuccessOrErrors.Success("All Ok");
            //now see what SQL tables haven't been mentioned
            if (!_sqlInfoDict.Any()) return status;

            var tablesToIgnore = _sqlTableNamesToIgnore.Split(',').Select(x => x.Trim()).ToList();
            foreach (var unusedTable in _sqlInfoDict.Values.Where(x => !tablesToIgnore.Contains(x.TableName)))
            {
                status.AddWarning("SQL {0} table {1} was not used by EF.", _sqlDbRefString, unusedTable.CombinedName);
            }

            return status;
        }
    }
}