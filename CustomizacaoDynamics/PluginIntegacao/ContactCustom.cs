using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class ContactCustom : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity contact = (Entity)this.Context.InputParameters["Target"];
            
            string cpf = contact.Contains("jrcv_cpf") ? contact["jrcv_cpf"].ToString() : string.Empty;

            if (cpf != string.Empty)
            {
                QueryExpression queryAccount = new QueryExpression(contact.LogicalName);
                queryAccount.ColumnSet.AddColumn("jrcv_cpf");
                queryAccount.Criteria.AddCondition("jrcv_cpf", ConditionOperator.Equal, cpf);
                EntityCollection accounts = this.Service.RetrieveMultiple(queryAccount);

                if (accounts.Entities.Count() > 0)
                    throw new InvalidPluginExecutionException("Já existe um contato com esse CPF no sistema.");
            }
        }

    }
}
