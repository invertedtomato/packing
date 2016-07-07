using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Utilities {
	public static class EntityFrameworkMigrationsUtility {
		public static string DropDefaultConstraint(this DbMigration migration, string table, string column) {
			return string.Format(@"
        DECLARE @name sysname

        SELECT @name = dc.name
        FROM sys.columns c
        JOIN sys.default_constraints dc ON dc.object_id = c.default_object_id
        WHERE c.object_id = OBJECT_ID('{0}')
        AND c.name = '{1}'

        IF @name IS NOT NULL
            EXECUTE ('ALTER TABLE {0} DROP CONSTRAINT ' + @name)
        ",
				table, column);
		}
	}
}
