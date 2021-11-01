using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using PluginIntegracao;

namespace ConsoleIntegracao
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
        }
    }
}
