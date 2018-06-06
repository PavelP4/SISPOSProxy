using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Parsers
{
    interface IParser<T, TResult>
    {
        bool TryParse(T input, out TResult result);
    }
}
