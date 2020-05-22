using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sonar.Initialization
{
    public abstract class Initializer
    {
        public abstract Task SetData(DataBuilder data);
    }
}
