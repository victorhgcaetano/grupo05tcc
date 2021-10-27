using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class NotificacaoCustom : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity account = (Entity)this.Context.InputParameters["Target"];

            string cnpj = account.Contains("jrcv_cnpj") ? account["jrcv_cnpj"].ToString() : string.Empty;

            if (cnpj != string.Empty)
            {
                QueryExpression queryAccount = new QueryExpression(account.LogicalName);
                queryAccount.ColumnSet.AddColumn("jrcv_cnpj");
                queryAccount.Criteria.AddCondition("jrcv_cnpj", ConditionOperator.Equal, cnpj);
                EntityCollection accounts = this.Service.RetrieveMultiple(queryAccount);

                if (accounts.Entities.Count() > 0)
                    throw new InvalidPluginExecutionException("Já existe uma conta com esse CNPJ no sistema.");
            }
        }

    }
}