using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao.Opportunity
{
    public class BlockUpdate : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];

            Guid opportunityId = (Guid)opportunity["opportunityid"];

            Entity opportunityCreation = new Entity("opportunity", opportunityId);
            opportunityCreation["criadoviaplugin"] = "NÃO";

            Service.Update(opportunityCreation);

        }
    }
}