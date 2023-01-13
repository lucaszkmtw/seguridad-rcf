using NHibernate;
using System.Data;
using System.Data.Common;

namespace TGP.Seguridad.DataAccess.Helpers
{
    public class UserDBContextSPFactory
    {
        public static void SetUserToDBContext(ISession session, string user)
        {
            IDbCommand command = new Oracle.DataAccess.Client.OracleCommand();
            //Step - 4 - Setting the connection property of the command instance 
            //with NHibernate session's Connection property.
            command.Connection = session.Connection;

            //Step - 5 - Setting the CommandType property 
            //as CommandType.StoredProcedure
            command.CommandType = CommandType.StoredProcedure;


            //Step - 6 - Setting the CommandText to the name 
            //of the stored procedure to invoke.
            command.CommandText = "pk_tgp_contexto.sp_setear_usuario_app";
            command.Parameters.Add(new Oracle.DataAccess.Client.OracleParameter("@parUsuario", user));

            session.Transaction.Enlist((DbCommand)command);
            //Step - 10 - Executing the stored procedure.
            command.ExecuteNonQuery();
        }

        public static void CleanUserToDBContext(ISession session)
        {
            IDbCommand command = new Oracle.DataAccess.Client.OracleCommand();
            //Step - 4 - Setting the connection property of the command instance 
            //with NHibernate session's Connection property.
            command.Connection = session.Connection;

            //Step - 5 - Setting the CommandType property 
            //as CommandType.StoredProcedure
            command.CommandType = CommandType.StoredProcedure;


            //Step - 6 - Setting the CommandText to the name 
            //of the stored procedure to invoke.
            command.CommandText = "pk_tgp_contexto.sp_limpiar_usuario_app";

            session.Transaction.Enlist((DbCommand)command);
            //Step - 10 - Executing the stored procedure.
            command.ExecuteNonQuery();
        }

    }
}
