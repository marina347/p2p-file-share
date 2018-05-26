using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.BazaF {
    interface ICrud {
        int Add();
        int Update(int i);
        int Delete();

    }
}
