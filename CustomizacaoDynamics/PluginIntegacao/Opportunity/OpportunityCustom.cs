using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class OpportunityCustom : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];
            if (!opportunity.Contains("jrcv_codigodaoportunidade"))
                opportunity["jrcv_codigodaoportunidade"] = GetNextCode(this.Service);            
        }

        private string GetNextCode(IOrganizationService service)
        {
            string code = "OPP-";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            do
            {
                code += GenetateRandomCode(numbers, 5) + "-";
                code += GenetateRandomCode(letters, 1) + GenetateRandomCode(numbers, 1);
                code += GenetateRandomCode(letters, 1) + GenetateRandomCode(numbers, 1);
            } while (RetrieveOpportunityCode(service, code));
            return code;
        }

        private string GenetateRandomCode(string charSet, int length)
        {
            var random = new Random();
            var array = new char[length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = charSet[random.Next(charSet.Length)];
            }

            return new String(array);
        }

        private static Boolean RetrieveOpportunityCode(IOrganizationService service, string code)
        {
            QueryExpression query = new QueryExpression("opportunity");
            query.ColumnSet.AddColumns("jrcv_codigodaoportunidade");
            query.Criteria.AddCondition("jrcv_codigodaoportunidade", ConditionOperator.Equal, code);
            return service.RetrieveMultiple(query).TotalRecordCount > 0 ? true : false;
        }

    }
}