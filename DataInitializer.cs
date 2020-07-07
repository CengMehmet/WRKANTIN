using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN
{
    class DataInitializer : DropCreateDatabaseIfModelChanges<KantinContext>
    {
        protected override void Seed(KantinContext context)
        {           
            base.Seed(context);
        }
    }
}
