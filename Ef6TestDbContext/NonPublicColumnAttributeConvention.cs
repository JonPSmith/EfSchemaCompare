#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: NonPublicColumnAttributeConvention.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace Ef6TestDbContext
{
    //see https://stackoverflow.com/questions/7619955/mapping-private-property-entity-framework-code-first
    //Thanks to crimbo, https://stackoverflow.com/users/605067/crimbo

    /// <summary>
    /// Convention to support binding private or protected properties to EF columns.
    /// </summary>
    public sealed class NonPublicColumnAttributeConvention : Convention
    {
        public NonPublicColumnAttributeConvention()
        {
            Types().Having(NonPublicProperties)
                .Configure((config, properties) =>
                {
                    foreach (PropertyInfo prop in properties)
                    {
                        config.Property(prop);
                    }
                });
        }

        private IEnumerable<PropertyInfo> NonPublicProperties(Type type)
        {
            var matchingProperties =
                type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(propInfo => propInfo.GetCustomAttributes(typeof (ColumnAttribute), true).Length > 0)
                    .ToArray();
            return matchingProperties.Length == 0 ? null : matchingProperties;
        }
    }
}