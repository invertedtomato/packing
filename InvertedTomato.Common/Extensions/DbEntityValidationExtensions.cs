using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.Text;

namespace InvertedTomato {
    public static class DbEntityValidationExtensions {
        public static string ToExtendedString(this DbEntityValidationException target) {
            Contract.Requires(null != target);

            var sb = new StringBuilder();
            foreach (var entity in target.EntityValidationErrors) {
                sb.Append("Entity of type '");
                sb.Append(entity.Entry.Entity.GetType().Name);
                sb.Append("' in state '");
                sb.Append(entity.Entry.State);
                sb.Append("' has the following validation errors:\n");
                foreach (var validationError in entity.ValidationErrors) {
                    sb.Append("- Property: '");
                    sb.Append(validationError.PropertyName);
                    sb.Append("', Error: '");
                    sb.Append(validationError.ErrorMessage);
                    sb.Append("'.\n");
                }
            }

            return sb.ToString();
        }
        /*
            try {
                db.SaveChanges();
            } catch (DbUpdateException ex) {
                throw ex;
            } catch (DbEntityValidationException ex) {
                throw ex;
            }
         */
    }
}