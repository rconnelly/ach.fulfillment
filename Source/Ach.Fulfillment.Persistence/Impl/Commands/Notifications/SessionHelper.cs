namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System;
    using System.Diagnostics.Contracts;

    using NHibernate;

    internal static class SessionHelper
    {
        public static void EndConversation(this ISession session, Guid handle)
        {
            Contract.Assert(session != null);
            var query = session.CreateSQLQuery("end conversation :handle");
            query.SetGuid("handle", handle);
            query.ExecuteUpdate();
        }

        public static void BeginConversationTimer(this ISession session, Guid handle, TimeSpan timeout)
        {
            Contract.Assert(session != null);
            var query = session.CreateSQLQuery("begin conversation timer (:handle) timeout = :timeout");
            query.SetGuid("handle", handle);
            query.SetInt32("timeout", (int)timeout.TotalSeconds);
            query.ExecuteUpdate();
        }
    }
}