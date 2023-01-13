using System;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Infrastructure
{
    public class Trigger
    {
        public Trigger()
        {
            Date = DateTime.Now;
            Version = 1;
            User = SessionWrapper.GetUser();
        }

        public Trigger(Audit audit)
        {
            Date = DateTime.Now;
            User = SessionWrapper.GetUser();
            Version = 1;
            Audit = audit;
        }

        public virtual DateTime Date { get; set; }

        public virtual string User { get; set; }

        public virtual int Version { get; set; }

        public virtual Audit Audit { get; set; }
    }
}
