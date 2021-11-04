using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{


    public class UptadeProduct : PluginImplementation
    {
        private string TableName = "product";

        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            try
            {
                if (this.Context.MessageName == "Create")
                {                    
                    CreateProduct();
                }
                else if (Context.MessageName == "Update")
                {
                    UpdateProducts();

                }
                else if (Context.MessageName == "Delete")
                {
                    DeleteProduct();

                }
            }
            catch(Exception e)
            {
                throw new InvalidPluginExecutionException("Erro ao sincronizar Dynamics: " + e.Message);
            }            
        }

        private void DeleteProduct()
        {
            //FUNÇÃO PARA DELETAR PRODUTOS NO DYNAMICS2

            Entity preDeleteImage = Context.PreEntityImages["PreImage"];
            string productName = preDeleteImage["name"].ToString();
            //PEGAR NOME DO PRODUTO PELA PRE-IMAGE

            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();

            EntityCollection productId = RetrieveProduct(productName);
            //CHAMAR FUNÇÃO PARA PEGAR O ID DO PRODUTO DO DYNAMICS2 

            foreach (Entity product in productId.Entities)
            {
                Guid ProductId = (Guid)product["productid"];
                service.Delete(TableName, ProductId);
            }
        }

        private void UpdateProducts()
        {
            //FUNÇÃO PARA ATUALIZAR DOS PRODUTOS
            Entity postProductImage = Context.PostEntityImages["PostImage"];            
            string productName = postProductImage["name"].ToString();            

            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();

            EntityCollection RetrieveProductId = RetrieveProduct(productName);
            ////CHAMAR FUNÇÃO PARA PEGAR O ID DO PRODUTO DO DYNAMICS2 
            
            foreach (Entity RetrieveProductsId in RetrieveProductId.Entities)
            {
                Guid productId = (Guid)RetrieveProductsId["productid"];
                Entity UpdateProduct = new Entity(TableName);
                UpdateProduct.Id = productId;
                UpdateProduct["name"] = postProductImage["name"];
                UpdateProduct["jrcv_categoriadasvagas"] = postProductImage["jrcv_categoriadasvagas"];
                UpdateProduct["jrcv_numerodecandidatos"] = postProductImage["jrcv_numerodecandidatos"];
                UpdateProduct["jrcv_valordotreinamento"] = postProductImage["jrcv_valordotreinamento"];
            
                service.Update(UpdateProduct);
            }           
        }
        private void CreateProduct()
        {
            //FUNÇÃO PARA CRIAR PRODUTOS NO DYNAMICS2
            Entity product = (Entity)this.Context.InputParameters["Target"];
            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
            EnablePlugin(service, false, "ProductDisabled");
            service.Create(product);
            
        }

        private EntityCollection RetrieveProduct(string name)
        {
            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
            //FUNÇÃO PARA FAZER RETRIEVE DO ID DO PRODUTO NO DYNAMICS2 POR MEIO DO NOME DO PRODUTO
            QueryExpression queryRetrieveProduct = new QueryExpression(TableName);
            queryRetrieveProduct.ColumnSet.AddColumns("productid");
            queryRetrieveProduct.Criteria.AddCondition("name", ConditionOperator.Equal, name);


            return service.RetrieveMultiple(queryRetrieveProduct);
        }

        public void EnablePlugin(IOrganizationService orgService, bool enable, string pluginName)

        {

            var qe = new QueryExpression("sdkmessageprocessingstep");

            qe.ColumnSet.AddColumns("sdkmessageprocessingstepid", "name");

            var step = orgService.RetrieveMultiple(qe).Entities.Where(x => x.Attributes["name"].ToString().Contains(pluginName)).First();

            var pluginId = (Guid)step.Attributes["sdkmessageprocessingstepid"];

            int pluginStateCode = enable ? 0 : 1;

            int pluginStatusCode = enable ? 1 : 2;

            orgService.Execute(new SetStateRequest

            {

                EntityMoniker = new EntityReference("sdkmessageprocessingstep", pluginId),

                State = new OptionSetValue(pluginStateCode),

                Status = new OptionSetValue(pluginStatusCode)

            });

        }    
    }
}