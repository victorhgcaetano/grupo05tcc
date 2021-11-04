using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao.Account
{
    public class CheckDuplicates : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity account = (Entity)this.Context.InputParameters["Target"];
            string cnpj = account.Contains("jrcv_cnpj") ? account["jrcv_cnpj"].ToString() : string.Empty;
           
            Entity entidadeContexto = null;

            if (Context.InputParameters.Contains("Target"))
                entidadeContexto = (Entity)Context.InputParameters["Target"];

            if (entidadeContexto.LogicalName == "account")
            {

                var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='account'>
                                <attribute name='name' />
                                <attribute name='primarycontactid' />
                                <attribute name='telephone1' />
                                 <attribute name='accountid' />
                                 <order attribute='name' descending='false' />
                                <filter type='and'>
                                <condition attribute='jrcv_cnpj' operator='eq' value='{entidadeContexto["jrcv_cnpj"]}' />
                                 </filter>
                             </entity>
                           </fetch>";

                var Retorno = this.Service.RetrieveMultiple(new FetchExpression(fetch));

                if (Retorno.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException("CNPJ JÁ CADASTRADO!!");
                }
            }
        }
    }
}
