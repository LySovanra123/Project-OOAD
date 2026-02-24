using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Mart.Pattern
{
    //======================Propotype Interface=========================
    public interface IPropotype<T>
    {
        T Clone();
    }
   
}
