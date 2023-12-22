using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IDalTester
    {
        Task<bool> ExecuteTestAsync();
    }
}
